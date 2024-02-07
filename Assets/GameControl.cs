using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GameControl
{
        public enum Gimmick
        {
                CantMoveDir,
                CantAttackDir,
                CantHitHard,
                CantKnockback,
                CantEscape,
                Ice,
                CantRun,
                CantWin
        }

        public enum Direction
        {
                Up,
                Down,
                Left,
                Right
        }
        
        private int _swordDamage;
        private float _playerMoveSpeed;
        private float _playerKnockback;
        private int _playerHealth;
        
        private int _slimeHealth;
        private float _slimeSpeed;
        private int _slimeDamage;
        private float _slimeKnockback;
        
        private int _numberOfSlimes;

        private Gimmick _gimmick;
        private Direction _direction;

        private static GameControl _self;

        private readonly UnityEvent _endGame;
        private readonly UnityEvent _changeGimmick;

        private GameControl()
        {
                SetParams();
                _numberOfSlimes = 15;
                _endGame = new UnityEvent();
                _changeGimmick = new UnityEvent();
        }

        private void SetParams()
        {
                _gimmick = (Gimmick) Random.Range(0, Enum.GetNames(typeof(Gimmick)).Length);
                _direction = (Direction)Random.Range(0, 3);
                

                _swordDamage = _gimmick == Gimmick.CantHitHard ? 1 : 2;
                
                _playerMoveSpeed = _gimmick switch
                {
                        Gimmick.CantRun => 9f,
                        Gimmick.Ice => 1f,
                        Gimmick.CantWin => 0f,
                        _ => 14f
                };

                _playerKnockback = _gimmick == Gimmick.CantKnockback ? 0f : 1.5f;
                _playerHealth = 6;
                _slimeHealth = 4;
                _slimeSpeed = _gimmick is Gimmick.CantEscape or Gimmick.CantWin ? 2.5f : 1.5f;
                _slimeDamage = 1;
                _slimeKnockback = 4f;
        }

        public void SwapGimmick()
        {
                var prev = _gimmick;
                var temp = _gimmick;

                while (prev == temp)
                {
                        temp = (Gimmick) Random.Range(0, Enum.GetNames(typeof(Gimmick)).Length);
                        _direction = (Direction)Random.Range(0, 3);
                }

                _gimmick = temp;
                SetParams();
                _changeGimmick.Invoke();
        }

        public static GameControl ConstructGameControl()
        {
                return _self ??= new GameControl();
        }

        public int GetSwordDamage()
        {
                return _swordDamage;
        }

        public int GetSlimeHealth()
        {
                return _slimeHealth;
        }

        public float GetPlayerMoveSpeed()
        {
                return _playerMoveSpeed;
        }

        public float GetPlayerKnockback()
        {
                return _playerKnockback;
        }

        public float GetSlimeSpeed()
        {
                return _slimeSpeed;
        }

        public int GetSlimeDamage()
        {
                return _slimeDamage;
        }

        public int GetPlayerHealth()
        {
                return _playerHealth;
        }

        public float GetSlimeKnockback()
        {
                return _slimeKnockback;
        }

        public UnityEvent GetEndgame()
        {
                return _endGame;
        }

        public UnityEvent GetChangeEvent()
        {
                return _changeGimmick;
        }

        public int GetNumSlimes()
        {
                return _numberOfSlimes;
        }

        public Gimmick GetGimmick()
        {
                return _gimmick;
        }

        public Direction GetDirection()
        {
                return _direction;
        }

        public static void Restart()
        {
                _self = null;
        }

        public int KillSlime()
        {
                // Debug.Log(_numberOfSlimes - 1);
                return --_numberOfSlimes;
        }

        public void SpawnSlime(int num)
        {
                _numberOfSlimes+= num;
        }
}