// NineSliceImage.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class NineSliceImage : MonoBehaviour
{
    [Header("Border Settings")]
    [SerializeField] private float leftBorder = 10f;
    [SerializeField] private float rightBorder = 10f;
    [SerializeField] private float topBorder = 10f;
    [SerializeField] private float bottomBorder = 10f;

    private void Start()
    {
        SetupNineSlice();
    }

    private void SetupNineSlice()
    {
        Image image = GetComponent<Image>();
        
        if (image.sprite != null)
        {
            // Create a new sprite with our border settings
            Texture2D originalTexture = image.sprite.texture;
            Rect rect = image.sprite.rect;
            Vector2 pivot = new Vector2(0.5f, 0.5f); // Center pivot
            Vector4 border = new Vector4(leftBorder, bottomBorder, rightBorder, topBorder);
            
            // Create new sprite with border settings
            Sprite newSprite = Sprite.Create(originalTexture, rect, pivot, 100.0f, 0, SpriteMeshType.FullRect, border);
            
            // Apply the new sprite and set it to sliced mode
            image.sprite = newSprite;
            image.type = Image.Type.Sliced;
            image.fillCenter = true;
        }
    }

    void OnDestroy()
    {
        // Clean up the created sprite to prevent memory leaks
        Image image = GetComponent<Image>();
        if (image != null && image.sprite != null)
        {
            Destroy(image.sprite);
        }
    }
}