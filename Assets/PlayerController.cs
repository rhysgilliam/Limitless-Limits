using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Vector2 _moveInput;
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private bool _canMove;
    private bool _attack;
    private int _health;
    private int _invTimer;
    private bool _dying;
    private bool _attackSuccess;

    private int _switchTimer;
    private const int SwitchTimerMax = 400;
    
    private static readonly int Side = Animator.StringToHash("attackSide");
    private static readonly int Up = Animator.StringToHash("attackUp");
    private static readonly int Down = Animator.StringToHash("attackDown");
    private static readonly int Left = Animator.StringToHash("attackLeft");
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int PlayerDie = Animator.StringToHash("playerDie");

    [SerializeField]
    private HealthbarController healthbarController;

    private UnityEvent _endGame;

    private GameControl _gameControl;
    
    private void Awake()
    {
        GameControl.Restart();
        _gameControl = GameControl.ConstructGameControl();

        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _health = _gameControl.GetPlayerHealth();
        
        _switchTimer = 0;

        SetDrag();
        _gameControl.GetChangeEvent().AddListener(SetDrag);

        _attack = false;
        _canMove = true;
    }

    private void SetDrag()
    {
        gameObject.GetComponent<Rigidbody2D>().drag = _gameControl.GetGimmick() == GameControl.Gimmick.Ice ? 0 : 12;
    }

    private void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            SceneManager.LoadScene("DeathScreen");
        } else if (Input.GetKeyDown("l"))
        {
            _gameControl.SwapGimmick();
        }
    }
    
    private void FixedUpdate() {
        switch (_invTimer)
        {
            case >= 25:
                _invTimer = -1;
                break;
            case >= 0:
                _invTimer++;
                break;
        }

        switch (_switchTimer)
        {
            case >= SwitchTimerMax:
                _gameControl.SwapGimmick();
                _switchTimer = 0;
                break;
            case < SwitchTimerMax:
                _switchTimer++;
                break;
        }

        if(_canMove)
        {
            if (_moveInput != Vector2.zero)
            {
                Move(_moveInput);
                _animator.SetBool(IsMoving, true);
            }
            else
            {
                _animator.SetBool(IsMoving, false);
            }
            
            _spriteRenderer.flipX = _moveInput.x switch
            {
                // flip sprite when needed
                < 0 => true,
                > 0 => false,
                _ => _spriteRenderer.flipX
            };
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Debug.Log("Attack: " + _attack);
        if (other.CompareTag("Enemy") && _attack) 
        {
            // Debug.Log("Colliding with enemy");
            var enemyController = other.GetComponent<EnemyController>();
            var direction = (Vector2)(other.gameObject.transform.position - gameObject.transform.position).normalized;
            enemyController.GetHit(_gameControl.GetSwordDamage(), direction * _gameControl.GetPlayerKnockback());
        } else if (other.CompareTag("Enemy") && !_attack)
        {
            GetHit(_gameControl.GetSlimeDamage(), (gameObject.transform.position - other.gameObject.transform.position).normalized);
        }
    }

    private void Move(Vector2 direction) {
        
        if (_gameControl.GetGimmick() == GameControl.Gimmick.CantMoveDir)
        {
            if (_gameControl.GetDirection() == GameControl.Direction.Up)
                _moveInput.y = Mathf.Clamp(_moveInput.y, -1, 0);
            else if (_gameControl.GetDirection() == GameControl.Direction.Down)
                _moveInput.y = Mathf.Clamp(_moveInput.y, 0, 1);
            else if (_gameControl.GetDirection() == GameControl.Direction.Right)
                _moveInput.x = Mathf.Clamp(_moveInput.x, -1, 0);
            else if (_gameControl.GetDirection() == GameControl.Direction.Left)
                _moveInput.x = Mathf.Clamp(_moveInput.x, 0, 1);
        }
        
        _rb.AddForce(direction * _gameControl.GetPlayerMoveSpeed());
    }

    private void OnMove(InputValue movementValue) {
        _moveInput = movementValue.Get<Vector2>();
        
    }

    private void OnFire()
    {
        _attackSuccess = false;
        if (_attack == false)
        {
            switch (_moveInput.y)
            {
                case 0:
                    switch (_spriteRenderer.flipX)
                    {
                        case true:
                            if (!(_gameControl.GetGimmick() == GameControl.Gimmick.CantAttackDir
                                 && _gameControl.GetDirection() == GameControl.Direction.Left))
                            {
                                AttackLeft();
                                _attackSuccess = true;
                            }

                            break;
                        case false:
                            if (!(_gameControl.GetGimmick() == GameControl.Gimmick.CantAttackDir
                                 && _gameControl.GetDirection() == GameControl.Direction.Right))
                            {
                                AttackSide();
                                _attackSuccess = true;
                            }

                            break;
                    }
                    break;
                case < 0:
                    if (!(_gameControl.GetGimmick() == GameControl.Gimmick.CantAttackDir
                         && _gameControl.GetDirection() == GameControl.Direction.Down))
                    {
                        AttackDown();
                        _attackSuccess = true;
                    }
                    break;
                case > 0:
                    if (!(_gameControl.GetGimmick() == GameControl.Gimmick.CantAttackDir
                         && _gameControl.GetDirection() == GameControl.Direction.Up))
                    {
                        AttackUp();
                        _attackSuccess = true;
                    }

                    break;
            }
            if (_attackSuccess)
                _attack = true;
        }
    }

    private void GetHit(int damage, Vector2 knockbackDir)
    {
        if (_invTimer == -1 && !_dying && !_attack)
        {
            // deal damage
            _health -= damage;
            healthbarController.TakeDamage(_health);
            
            if (_health <= 0)
                _animator.SetTrigger(PlayerDie);
            else
                _rb.AddForce(knockbackDir * _gameControl.GetSlimeKnockback(), ForceMode2D.Impulse);
            
            _invTimer = 0;
        }
    }

    // the rest of this should really be in a separate file but do i care? no
    private void AttackUp()
    {
        _animator.SetTrigger(Up);
    }

    private void AttackSide()
    {
        _animator.SetTrigger(Side);
    }

    private void AttackLeft()
    {
        _animator.SetTrigger(Left);
    }

    private void AttackDown()
    {
        _animator.SetTrigger(Down);
    }
    
    
    private void LockMovement()
    {
        _canMove = false;
    }

    public void UnlockMovement()
    {
        _canMove = true;
    }

    public void StartAttack()
    {
        // redundant but i'm too lazy to fix
        _attack = true;
    }

    public void StopAttack()
    {
        _attack = false;
    }

    public void StartDie()
    {
        LockMovement();
        _dying = true;
        _rb.velocity = Vector2.zero;
        _gameControl.GetEndgame().Invoke();
    }

    public void GoToDeathScreen()
    {
        SceneManager.LoadScene("DeathScreen");
    }
    
    public int GetHealth()
    {
        return _health;
    }


}
