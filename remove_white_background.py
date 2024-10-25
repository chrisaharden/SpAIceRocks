from PIL import Image
import os

def is_white(pixel, threshold=230):
    """Check if a pixel is white-ish based on RGB values"""
    return all(value >= threshold for value in pixel[:3])

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
        image = Image.open(image_path).convert('RGBA')
        
        # Get the pixel data
        data = image.getdata()
        
        # Create a new list for the modified pixels
        new_data = []
        
        # Process each pixel
        for pixel in data:
            # If the pixel is white-ish, make it transparent
            if is_white(pixel):
                new_data.append((255, 255, 255, 0))  # Transparent
            else:
                new_data.append(pixel)  # Keep original pixel
        
        # Update the image with the new pixel data
        image.putdata(new_data)
        
        # Save the processed image
        image.save(output_path, 'PNG')
        print(f"Processed {filename} -> {transparent_filename}")

print('Image processing complete!')
