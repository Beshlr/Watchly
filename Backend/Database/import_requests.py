import requests
import pandas as pd
from time import sleep

# استبدل هذا بمفتاح API الخاص بك
API_KEY = '482c791e158bee1c2cbd8064a773229e'  # ← ضع مفتاحك هنا

def get_imdb_movies(start_year=2017, end_year=2025):
    base_url = "https://imdb-api.com/API/AdvancedSearch/" + API_KEY
    all_movies = []
    
    for year in range(start_year, end_year + 1):
        params = {
            'title_type': 'feature',
            'release_date': f'{year}-01-01,{year}-12-31',
            'count': 250,
            'groups': 'top_1000'  # للحصول على أشهر الأفلام
        }
        
        try:
            response = requests.get(base_url, params=params)
            response.raise_for_status()
            data = response.json()
            
            if 'results' in data:
                print(f"تم جلب {len(data['results'])} فيلم لعام {year}")
                all_movies.extend(data['results'])
            else:
                print(f"لا توجد نتائج لعام {year}")
            
            sleep(1)  # تجنب حظر الطلبات الكثيرة
            
        except Exception as e:
            print(f"حدث خطأ لعام {year}: {str(e)}")
    
    return pd.DataFrame(all_movies)

def get_movie_details(movie_id):
    url = f"https://imdb-api.com/API/Title/{API_KEY}/{movie_id}/FullActor,Posters,Trailer"
    try:
        response = requests.get(url)
        response.raise_for_status()
        return response.json()
    except:
        return None

def process_data(raw_df):
    processed_data = []
    
    for _, row in raw_df.iterrows():
        details = get_movie_details(row['id'])
        if not details:
            continue
            
        movie = {
            'ID': details.get('id'),
            'color': 'Color',  # معظم الأفلام ملونة
            'director_name': details.get('directors'),
            'num_critic_for_reviews': details.get('metacriticRating') or 0,
            'duration': int(details.get('runtimeMins') or 0),
            'director_facebook_likes': 0,  # غير متوفر مباشرة
            'actor_3_facebook_likes': 0,
            'actor_2_name': details.get('actorList')[1]['name'] if len(details.get('actorList', [])) > 1 else '',
            'actor_1_facebook_likes': 0,
            'gross': details.get('boxOffice', {}).get('cumulativeWorldwideGross', '0').replace('$', '').replace(',', '') or 0,
            'actor_1_name': details.get('actorList')[0]['name'] if details.get('actorList') else '',
            'movie_title': details.get('title'),
            'num_voted_users': details.get('imDbRatingVotes', 0),
            'cast_total_facebook_likes': 0,
            'actor_3_name': details.get('actorList')[2]['name'] if len(details.get('actorList', [])) > 2 else '',
            'facenumber_in_poster': len(details.get('posters', {}).get('posters', [])),
            'movie_imdb_link': f"https://www.imdb.com/title/{details.get('id')}/",
            'num_user_for_reviews': details.get('userReviews', 0),
            'language': details.get('languages', '').split(',')[0] if details.get('languages') else '',
            'country': details.get('countries', '').split(',')[0] if details.get('countries') else '',
            'content_rating': details.get('contentRating'),
            'budget': details.get('boxOffice', {}).get('budget', '0').replace('$', '').replace(',', '') or 0,
            'title_year': int(details.get('year')) if details.get('year') else None,
            'actor_2_facebook_likes': 0,
            'imdb_score': float(details.get('imDbRating', 0)),
            'aspect_ratio': None,  # غير متوفر في API
            'movie_facebook_likes': 0,
            'poster_url': details.get('image'),
            'trailer_url': details.get('trailer', {}).get('linkEmbed') if details.get('trailer') else '',
            'ContentRatingID': 1  # يمكنك تغيير هذا حسب التصنيف
        }
        processed_data.append(movie)
        print(f"تم معالجة فيلم: {movie['movie_title']}")
        sleep(1)  # تجنب حظر الطلبات
    
    return pd.DataFrame(processed_data)

# التنفيذ الرئيسي
if __name__ == "__main__":
    print("بدأ جلب بيانات الأفلام من IMDb...")
    movies_df = get_imdb_movies(2017, 2025)
    
    if not movies_df.empty:
        print("\nبدأ معالجة تفاصيل الأفلام...")
        final_df = process_data(movies_df.head(20))  # جرب مع 20 فيلم أولاً للتجربة
        
        if not final_df.empty:
            final_df.to_csv('imdb_movies_2017_2025_full.csv', index=False)
            print(f"\nتم حفظ {len(final_df)} فيلم في ملف imdb_movies_2017_2025_full.csv")
        else:
            print("لم يتم العثور على تفاصيل للأفلام")
    else:
        print("لم يتم جلب أي أفلام، يرجى التحقق من مفتاح API")