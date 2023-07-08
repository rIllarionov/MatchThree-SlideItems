using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TileInitializer : MonoBehaviour
{
    [SerializeField] private Image _image;
    private string _type;
    public string Type => _type;

    public void Initialize(ScriptableItemSettings itemSettings)
    {
        _image.gameObject.SetActive(true);
        _image.sprite = itemSettings.Sprite;
        _type = itemSettings.Type;

        PlayTileAnimation();
    }

    private void PlayTileAnimation()
    {
        _image.transform.localScale = new Vector3(0, 0, 0);
        _image.transform.
            DOScale(new Vector3(1, 1, 0), 0.5f).
            SetEase(Ease.InSine);
    }
}
