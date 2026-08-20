from pathlib import Path
from PIL import Image

# Directory da processare
cartella = Path(r".")

for file_png in cartella.rglob("*.png"):
    try:
        with Image.open(file_png) as img:
            if img.size != (16, 16):
                img = img.convert("RGBA")
                img_ridimensionata = img.resize((16, 16), Image.LANCZOS)
                img_ridimensionata.save(file_png)
                print(f"Ridimensionata: {file_png} ({img.size} -> (16, 16))")
            else:
                print(f"Già corretta: {file_png}")
    except Exception as ex:
        print(f"Errore su {file_png}: {ex}")
