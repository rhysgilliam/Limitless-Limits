using TMPro;
using UnityEngine;

public class GimmickTextController : MonoBehaviour
{
    private GameControl _gameControl;

    private TextMeshProUGUI _text;
    
    // Start is called before the first frame update
    private void Start()
    {
        _text = gameObject.GetComponent<TextMeshProUGUI>();
        
        _gameControl = GameControl.ConstructGameControl();
        _text.color = Color.white;

        ChangeText();
        _gameControl.GetChangeEvent().AddListener(ChangeText);
    }

    private void ChangeText()
    {
        _text.color = Color.white;
        switch (_gameControl.GetGimmick())
        {
            case GameControl.Gimmick.CantMoveDir:
                _text.text = _gameControl.GetDirection() switch
                {
                    GameControl.Direction.Up => "move upwards",
                    GameControl.Direction.Right => "move right",
                    GameControl.Direction.Down => "move down",
                    GameControl.Direction.Left => "move left",
                    _ => _text.text
                };

                break;
            case GameControl.Gimmick.CantAttackDir:
                _text.text = _gameControl.GetDirection() switch
                {
                    GameControl.Direction.Up => "attack upwards",
                    GameControl.Direction.Right => "attack right",
                    GameControl.Direction.Down => "attack down",
                    GameControl.Direction.Left => "attack left",
                    _ => _text.text
                };
                break;
            case GameControl.Gimmick.CantHitHard:
                _text.text = "hit very hard";
                break;
            case GameControl.Gimmick.CantKnockback:
                _text.text = "knock enemies back";
                break;
            case GameControl.Gimmick.CantEscape:
                _text.text = "escape the slimes";
                break;
            case GameControl.Gimmick.Ice:
                _text.text = "stop quickly";
                break;
            case GameControl.Gimmick.CantRun:
                _text.text = "run";
                break;
            case GameControl.Gimmick.CantWin:
                _text.color = Color.red;
                _text.text = "survive.";
                break;
            default:
                _text.text = "error lol";
                break;
        }
    }
}
