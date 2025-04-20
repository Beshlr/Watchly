import requests
import pandas as pd
import time
from tqdm import tqdm  # لعرض شريط التقدم

# إعدادات API
OMDB_API_KEY = "3d5fe386d89641603bd792b59254710a"  # استبدل بمفتاح API الخاص بك
OMDB_URL = "http://www.omdbapi.com/"

# ملف الإدخال (يجب أن يحتوي على عمود imdbID)
INPUT_CSV = "movies_2017_2025.csv"  # استبدل بمسار ملفك
OUTPUT_CSV = "movies_detailed_info.csv"  # ملف المخرجات

def get_movie_details(imdb_id):
    """جلب تفاصيل الفيلم من OMDB API"""
    params = {
        'i': imdb_id,
        'apikey': OMDB_API_KEY,
        'plot': 'full'  # للحصول على الوصف الكامل
    }
    
    try:
        response = requests.get(OMDB_URL, params=params)
        response.raise_for_status()
        data = response.json()
        
        if data.get('Response') == 'True':
            return data
        else:
            print(f"Error fetching {imdb_id}: {data.get('Error', 'Unknown error')}")
            return None
            
    except requests.exceptions.RequestException as e:
        print(f"Request failed for {imdb_id}: {e}")
        return None

def process_movies(input_file, output_file):
    """معالجة الأفلام وحفظ النتائج"""
    # قراءة ملف الإدخال
    try:
        df_input = pd.read_csv(input_file)
        if 'imdbID' not in df_input.columns:
            raise ValueError("Input CSV must contain 'imdbID' column")
    except Exception as e:
        print(f"Error reading input file: {e}")
        return

    # قائمة لتخزين النتائج
    all_movies = []
    
    # معالجة كل فيلم مع شريط التقدم
    for imdb_id in tqdm(df_input['imdbID'], desc="Processing movies"):
        # جلب تفاصيل الفيلم
        movie_data = get_movie_details(imdb_id)
        
        if movie_data:
            # إضافة البيانات إلى القائمة
            all_movies.append(movie_data)
        
        # تأخير لتجنب تجاوز حد معدل الطلبات (40 طلب/دقيقة لـ OMDB API المجاني)
        time.sleep(1.5)  # 1.5 ثانية بين الطلبات
    
    # تحويل القائمة إلى DataFrame
    if all_movies:
        df_output = pd.DataFrame(all_movies)
        
        # حفظ النتائج في ملف CSV
        try:
            df_output.to_csv(output_file, index=False, encoding='utf-8')
            print(f"\nSuccessfully saved detailed info for {len(all_movies)} movies to {output_file}")
        except Exception as e:
            print(f"Error saving output file: {e}")
    else:
        print("No movie data was retrieved")

if __name__ == "__main__":
    process_movies(INPUT_CSV, OUTPUT_CSV)