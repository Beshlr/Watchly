import requests
import time
import csv

OMDB_API_KEY = '151a4f95'  # ← استبدله بمفتاحك الخاص
CSV_FILE_NAME = 'movies_2017_2025.csv'

def fetch_all_movies_to_csv(start_year=2017, end_year=2025, max_pages=5):
    url = 'http://www.omdbapi.com/'
    all_movies = []
    search_terms = ['a', 'the', 'movie', 'film']  # كلمات بحث متنوعة

    for year in range(start_year, end_year + 1):
        print(f"\n📅 جاري استرجاع الأفلام من سنة {year}...")
        
        for term in search_terms:
            for page in range(1, max_pages + 1):
                params = {
                    'apikey': OMDB_API_KEY,
                    'type': 'movie',
                    'y': year,
                    's': term,  # استخدام مصطلح بحث مختلف
                    'page': page
                }

                try:
                    response = requests.get(url, params=params)
                    data = response.json()

                    if data.get('Response') == 'True':
                        for movie in data.get('Search', []):
                            all_movies.append([
                                movie.get('Title', 'N/A'),
                                movie.get('Year', 'N/A'),
                                movie.get('imdbID', 'N/A'),
                                movie.get('Type', 'N/A'),
                                movie.get('Poster', 'N/A')
                            ])
                        print(f"تمت إضافة {len(data['Search'])} أفلام للصفحة {page}")
                    
                    elif data.get('Error') == 'Too many results.':
                        print(f"⚠️ تخطي البحث بـ '{term}' لسنة {year} (كثرة النتائج)")
                        break  # انتقل إلى مصطلح البحث التالي
                    
                    else:
                        print(f"⚠️ {data.get('Error', 'خطأ غير معروف')}")
                        break  # توقف إذا كان هناك خطأ آخر

                except Exception as e:
                    print(f"❌ خطأ: {e}")
                    break

                time.sleep(1.5)  # زيادة التأخير بين الطلبات

    # حفظ النتائج في CSV
    with open(CSV_FILE_NAME, 'w', encoding='utf-8', newline='') as file:
        writer = csv.writer(file)
        writer.writerow(['Title', 'Year', 'imdbID', 'Type', 'Poster'])
        writer.writerows(all_movies)

    print(f"\n✅ تم حفظ {len(all_movies)} فيلم بنجاح في {CSV_FILE_NAME}")

if __name__ == '__main__':
    fetch_all_movies_to_csv()