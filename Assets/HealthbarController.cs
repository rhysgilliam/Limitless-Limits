using UnityEngine;

public class HealthbarController : MonoBehaviour
{
    [SerializeField] private HeartController heart1;
    [SerializeField] private HeartController heart2;
    [SerializeField] private HeartController heart3;

    private GameControl _gameControl;

    private void Start()
    {
        _gameControl = GameControl.ConstructGameControl();
    }

    public void TakeDamage(int health)
    {
        switch (health)
        {
            case 5 or 4:
                heart3.TakeDamage();
                break;
            case 3 or 2:
                heart2.TakeDamage();
                break;
            case 1 or 0:
                heart1.TakeDamage();
                break;
        }
    }
}