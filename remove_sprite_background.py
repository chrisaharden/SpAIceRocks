import os
from rembg import remove
from PIL import Image

# Set the directory containing the images
image_dir = 'Assets/Sprites/'

# Loop through the image files in the directory
for filename in os.listdir(image_dir):
    # Skip files that already have 'transparent_' prefix
    if filename.startswith('transparent_'):
        continue
        
    if filename.endswith('.png'):
        # Check if the transparent version already exists
        transparent_filename = f"transparent_{filename}"
        output_path = os.path.join(image_dir, transparent_filename)
        
        # Skip if the transparent version already exists
        if os.path.exists(output_path):
            print(f"Skipping {filename} - transparent version already exists")
            continue
            
        # Open the image file
        image_path = os.path.join(image_dir, filename)
        
        # Read the input image
        input_image = Image.open(image_path)
        
        # Remove the background
        output_image = remove(input_image)
        
        # Save the processed image
        output_image.save(output_path, 'PNG')
        print(f"Processed {filename} -> {transparent_filename}")

print('Image processing complete!')
