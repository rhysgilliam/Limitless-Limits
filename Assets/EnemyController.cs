using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    private GameObject _player;
    
    private int _health;
    private int _invTimer;
    private bool _dying;

    private Vector2 _towardsPlayer;
    private bool _canSeePlayer;
    
    private SpriteRenderer _spriteRenderer;

    private Animator _animator;

    private Rigidbody2D _rb;
    
    private static readonly int IsMoving = Animator.StringToHash("enemyMoving");
    private static readonly int EnemyDie = Animator.StringToHash("enemyDie");

    private GameControl _gameControl;

    private void Start()
    {
        _gameControl = GameControl.ConstructGameControl();
        
        _player = GameObject.FindGameObjectWithTag("Main_Player");
        _health = _gameControl.GetSlimeHealth();
        _invTimer = -1;
        _animator = GetComponent<Animator>();
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _dying = false;
        
        _canSeePlayer = false;
        
        _gameControl.GetEndgame().AddListener(EndGame);
    }

    private void FixedUpdate()
    {
        switch (_invTimer)
        {
            case >= 22:
                _invTimer = -1;
                break;
            case >= 0:
                _invTimer++;
                break;
        }
        
        if (_canSeePlayer && !_dying)
        {
            _towardsPlayer = (_player.transform.position - gameObject.transform.position).normalized;
            Move(_towardsPlayer);
            _animator.SetBool(IsMoving, true);
            
            if (_towardsPlayer.x < 0)
            {
                _spriteRenderer.flipX = true;
            }
            else if (_towardsPlayer.x > 0)
            {
                _spriteRenderer.flipX = false;
            }
        }
        else
        {
            _animator.SetBool(IsMoving, false);
        }

        if (_gameControl.GetGimmick() != GameControl.Gimmick.CantEscape &&
            _gameControl.GetGimmick() != GameControl.Gimmick.CantWin)
        {
            _canSeePlayer = Vector2.Distance(
                _player.transform.position, gameObject.transform.position) < 1.6f;
        }
        else
            _canSeePlayer = true;
    }

    private void Move(Vector2 direction) {
        _rb.AddForce(direction * _gameControl.GetSlimeSpeed());
    }

    public void GetHit(int damage, Vector2 knockback)
    {
        if (_invTimer == -1 && !_dying)
        {
            // deal damage
            _health -= damage;

            if (_health <= 0)
                _animator.SetTrigger(EnemyDie);
            else
                _rb.AddForce(knockback, ForceMode2D.Impulse);
            
            _invTimer = 0;
        }
    }

    public void StartDie()
    {
        _rb.velocity = Vector2.zero;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        _dying = true;
    }

    public void Destroy()
    {
        if (_gameControl.KillSlime() <= 0)
        {
            SceneManager.LoadScene("WinScreen");
        }
        Destroy(gameObject);
    }

    private void EndGame()
    {
        _dying = true;
    }
}
