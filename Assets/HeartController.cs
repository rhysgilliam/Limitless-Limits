using UnityEngine;
using UnityEngine.UI;

public class HeartController : MonoBehaviour
{
    [SerializeField]
    private Sprite fullHeart;
    [SerializeField]
    private Sprite halfHeart;
    private bool _takenDamage;

    private void Start()
    {
        _takenDamage = false;
        gameObject.GetComponent<Image>().sprite = fullHeart;
    }

    public void TakeDamage()
    {
        if (!_takenDamage)
        {
            gameObject.GetComponent<Image>().sprite = halfHeart;
            _takenDamage = true;
        }
        else
            gameObject.GetComponent<Image>().enabled = false;

    }
}
