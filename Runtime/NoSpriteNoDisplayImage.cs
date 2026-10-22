using UnityEngine;
using UnityEngine.UI;

namespace MyTools
{
    /// <summary>
    /// Ensures that the attached <see cref="Image"/> component is not displayed if it does not have a sprite assigned.
    /// </summary>
    /// <remarks>This class requires the GameObject to have an <see cref="Image"/> component. It is typically
    /// used to hide UI elements  that rely on sprites when no sprite is available. The behavior is managed during the
    /// Unity lifecycle methods.</remarks>
    [RequireComponent(typeof(Image))]
    public class NoSpriteNoDisplayImage : MonoBehaviour
    {
        private Image imageRef;

        private void Awake()
        {
            imageRef = GetComponent<Image>();
        }

        private void Update()
        {
            imageRef.enabled = imageRef.sprite != null;
        }
    }
}