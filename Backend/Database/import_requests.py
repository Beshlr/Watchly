import os
import requests
import pandas as pd
from dotenv import load_dotenv
from typing import List, Dict, Any, Optional
import time

load_dotenv()

class TMDBClient:
    BASE_URL = "https://api.themoviedb.org/3"
    
    def __init__(self):
        self.access_token = os.getenv("TMDB_ACCESS_TOKEN")
        if not self.access_token:
            raise ValueError("TMDB_ACCESS_TOKEN not found in environment variables.")
        self.session = requests.Session()
        self.session.headers.update({
            "Authorization": f"Bearer {self.access_token}",
            "accept": "application/json"
        })

    def get(self, endpoint: str, params: Optional[Dict] = None) -> Dict:
        url = f"{self.BASE_URL}{endpoint}"
        response = self.session.get(url, params=params or {})
        response.raise_for_status()
        return response.json()

def fetch_trending_movies(time_window: str = "day", max_movies: int = 200) -> List[Dict[str, Any]]:
    """
    time_window: يمكن أن يكون "day" أو "week"
    """
    client = TMDBClient()
    movies = []
    page = 1
    
    print(f"🎬 جلب الأفلام الأكثر انتشارًا (Trending) خلال آخر {time_window}...")
    
    try:
        while len(movies) < max_movies:
            print(f"📄 جلب الصفحة {page}... (إجمالي الأفلام: {len(movies)})")
            
            # التغيير الرئيسي هنا: استخدام trending endpoint بدلاً من discover
            trending_data = client.get(
                f"/trending/movie/{time_window}",  # النقطة الأساسية للتعديل
                params={"language": "en-US", "page": page}
            )
            
            if not trending_data.get("results"):
                print("⚠️ لم يتم العثور على أفلام إضافية.")
                break
            
            for movie in trending_data["results"]:
                if len(movies) >= max_movies:
                    break
                    
                try:
                    movie_data = {
                        "id": movie["id"],
                        "title": movie["title"],
                        "popularity": movie.get("popularity", 0),
                        "trending_score": movie.get("vote_average", 0),  # مقياس الانتشار
                        "release_date": movie.get("release_date", "N/A"),
                        "poster_url": f"https://image.tmdb.org/t/p/w500{movie['poster_path']}" if movie.get("poster_path") else "",
                        "overview": movie.get("overview", "")
                    }
                    movies.append(movie_data)
                    
                except Exception as e:
                    print(f"⚠️ خطأ في معالجة الفيلم {movie.get('id')}: {str(e)}")
                    continue
            
            page += 1
            time.sleep(0.5)  # تجنب rate limiting
            
        print(f"✅ تم جلب {len(movies)} فيلم بنجاح.")
        return movies
    
    except requests.exceptions.RequestException as e:
        print(f"❌ خطأ في جلب البيانات: {e}")
        return []

# باقي الكود يبقى كما هو (دالة save_to_csv والتنفيذ الرئيسي)

def save_to_csv(movies: List[Dict], output_path: str) -> None:
    if not movies:
        print("⚠️ لا توجد أفلام لحفظها.")
        return
    
    df = pd.DataFrame(movies)
    # الأعمدة المتوافقة مع البيانات المجموعة
    columns = [
        'id', 'title', 'popularity', 'trending_score',
        'release_date', 'poster_url', 'overview'
    ]
    df = df[columns]
    df.to_csv(output_path, index=False, encoding="utf-8")
    print(f"💾 تم الحفظ في {output_path}")

if __name__ == "__main__":
    movies_data = fetch_trending_movies(time_window="week", max_movies=100)
    save_to_csv(movies_data, "trending_movies_this_week.csv")
    input("اضغط Enter للخروج...")