using UnityEngine;
using UnityEngine.UI;

public class ThumbnailClickHandler : MonoBehaviour
{
    [Tooltip("Drag the large Preview Image object here")]
    public Image previewImage;

    // This method will be hooked up to each thumbnail Button's OnClick.
    // The 'thumbSprite' parameter is the thumbnail's own Sprite.
    public void ShowOnPreview(Sprite thumbSprite)
    {
        if (previewImage != null && thumbSprite != null)
        {
            previewImage.sprite = thumbSprite;
            // Optional: if you want to preserve aspect ratio
            previewImage.SetNativeSize();
        }
    }
}