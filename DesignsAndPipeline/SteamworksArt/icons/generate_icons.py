import sys
from PIL import Image
import os

def generate_icons(input_file):
    """
    Generate square icons of different sizes from an input image.
    Saves PNG, ICO, TGA, and JPG formats.
    
    Args:
        input_file (str): Path to the input PNG file
    """
    try:
        # Open the original image
        with Image.open(input_file) as img:
            # Convert to RGBA if not already
            img = img.convert('RGBA')
            
            # Define the sizes we want - now including 184
            sizes = [1024, 512, 256, 184, 128, 64, 48, 32, 16]
            
            # Get the base filename without extension
            base_name = os.path.splitext(input_file)[0]
            
            # Create directories for output if they don't exist
            output_dir = 'generated_icons'
            os.makedirs(output_dir, exist_ok=True)
            
            # Store all resized images for ICO file
            ico_images = []
            
            # Generate each size
            for size in sizes:
                # Create a square image by cropping to aspect ratio first
                aspect = img.width / img.height
                if aspect > 1:
                    # Width is greater than height
                    new_width = int(img.height * 1)
                    left = (img.width - new_width) // 2
                    img_square = img.crop((left, 0, left + new_width, img.height))
                else:
                    # Height is greater than width
                    new_height = int(img.width * 1)
                    top = (img.height - new_height) // 2
                    img_square = img.crop((0, top, img.width, top + new_height))
                
                # Resize the square image
                resized = img_square.resize((size, size), Image.Resampling.LANCZOS)
                
                # Save as PNG
                png_path = os.path.join(output_dir, f"{base_name}_{size}x{size}.png")
                resized.save(png_path, "PNG", optimize=True)
                print(f"Generated {size}x{size} PNG: {png_path}")
                
                # Save as JPG with black background
                jpg_image = Image.new('RGB', resized.size, (0, 0, 0))  # Black background
                if resized.mode == 'RGBA':
                    jpg_image.paste(resized, mask=resized.split()[3])  # Use alpha channel as mask
                else:
                    jpg_image.paste(resized)
                jpg_path = os.path.join(output_dir, f"{base_name}_{size}x{size}.jpg")
                jpg_image.save(jpg_path, "JPEG", quality=95, optimize=True)
                print(f"Generated {size}x{size} JPG: {jpg_path}")
                
                # Save as TGA
                tga_path = os.path.join(output_dir, f"{base_name}_{size}x{size}.tga")
                # For TGA, we need to ensure we're in the right mode
                tga_image = resized
                if tga_image.mode != 'RGBA':
                    tga_image = tga_image.convert('RGBA')
                tga_image.save(tga_path, "TGA")
                print(f"Generated {size}x{size} TGA: {tga_path}")
                
                # Store images <= 256 for ICO file
                if size <= 256:
                    ico_images.append(resized)
            
            # Save ICO files
            # First, save a multi-resolution ICO containing all sizes <= 256
            ico_path = os.path.join(output_dir, f"{base_name}.ico")
            # Sort in descending order so largest (256) is first
            ico_images.sort(key=lambda x: x.size[0], reverse=True)
            ico_images[0].save(ico_path, format='ICO', sizes=[(img.size[0], img.size[0]) for img in ico_images])
            print(f"Generated multi-resolution ICO file: {ico_path}")
            
            # Also save individual ICO files for each size
            for img in ico_images:
                size = img.size[0]
                single_ico_path = os.path.join(output_dir, f"{base_name}_{size}x{size}.ico")
                img.save(single_ico_path, format='ICO')
                print(f"Generated {size}x{size} ICO: {single_ico_path}")
                
    except Exception as e:
        print(f"Error processing image: {str(e)}")
        sys.exit(1)

def main():
    if len(sys.argv) != 2:
        print("Usage: python script.py <input_icon.png>")
        sys.exit(1)
        
    input_file = sys.argv[1]
    
    if not os.path.exists(input_file):
        print(f"Error: File '{input_file}' not found")
        sys.exit(1)
        
    if not input_file.lower().endswith('.png'):
        print("Error: Input file must be a PNG image")
        sys.exit(1)
        
    generate_icons(input_file)

if __name__ == "__main__":
    main()