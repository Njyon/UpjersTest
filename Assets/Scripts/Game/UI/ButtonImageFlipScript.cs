using UnityEngine;
using UnityEngine.UI;

public class ButtonImageFlipScript : MonoBehaviour
{
    Button button;

    [SerializeField] Image image;
    [SerializeField] Sprite spriteA;
    [SerializeField] Sprite spriteB;

    bool isA;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(FlipImage);

        if (image != null) image.sprite = spriteA;
        isA = true;
    }

    void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(FlipImage);
    }

    void FlipImage()
    {
        if (isA)
        {
            image.sprite = spriteB;
            isA = false;
        }
        else
        {
            image.sprite = spriteA;
            isA = true;
        }
    }
}
