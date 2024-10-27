import os
import argparse
from rembg import remove
from PIL import Image

# Set up argument parser
parser = argparse.ArgumentParser(description='Process images and rename them with specified parameters.')
parser.add_argument('category', type=str, help='Category of the image')
parser.add_argument('subcategory1', type=str, nargs='?', default='', help='First subcategory of the image (optional)')
parser.add_argument('subcategory2', type=str, nargs='?', default='', help='Second subcategory of the image (optional)')
parser.add_argument('start_number', type=int, nargs='?', default=1, help='Starting number for the filenames (optional)')

# Check if no arguments are provided and print help
if len(os.sys.argv) == 1:
    parser.print_help()
    os.sys.exit(1)

args = parser.parse_args()

# Define input and output directories
image_dir = './images/input'
output_dir = './images/output'

# Ensure output directory exists
os.makedirs(output_dir, exist_ok=True)

# Initialize the counter with the starting number
counter = args.start_number

# Loop through the image files in the directory
for filename in os.listdir(image_dir):
    if filename.endswith('.png'):
        # Generate the new filename
        if args.subcategory1 and args.subcategory2:
            new_filename = f"{args.category}_{args.subcategory1}_{args.subcategory2}_{counter}.png"
        elif args.subcategory1:
            new_filename = f"{args.category}_{args.subcategory1}_{counter}.png"
        else:
            new_filename = f"{args.category}_{counter}.png"
        
        output_path = os.path.join(output_dir, new_filename)
        
        # Skip if the processed version already exists
        if os.path.exists(output_path):
            print(f"Skipping {filename} - processed version already exists")
            continue
            
        # Open the image file
        image_path = os.path.join(image_dir, filename)
        
        # Read the input image
        input_image = Image.open(image_path)
        
        # Remove the background
        output_image = remove(input_image)
        
        # Save the processed image
        output_image.save(output_path, 'PNG')
        print(f"Processed {filename} as {new_filename}")
        
        # Increment the counter
        counter += 1

print('Image processing complete!')
