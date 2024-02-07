using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    private GameControl _gameControl;

    [SerializeField] private float upperMapLimit; 
    [SerializeField] private float lowerMapLimit;
    [SerializeField] private float leftMapLimit;
    [SerializeField] private float rightMapLimit;

    private int _spawnTimer;

    private BoxCollider2D _enemyCollider;

    [SerializeField] private GameObject enemyPrefab;
    // Start is called before the first frame update
    private void Start()
    {
        _gameControl = GameControl.ConstructGameControl();
        _enemyCollider = enemyPrefab.GetComponent<BoxCollider2D>();
        
        SpawnEnemies(_gameControl.GetNumSlimes());
        
        _gameControl.GetEndgame().AddListener(EndGame);

        _spawnTimer = 0;
    }

    private void Update()
    {
        if (_gameControl.GetGimmick() == GameControl.Gimmick.CantWin)
        {
            _spawnTimer++;
            if (_spawnTimer >= 200)
            {
                _spawnTimer = 0;
                SpawnEnemies(5);
                _gameControl.SpawnSlime(5);
                
            }
        }
    }

    private bool CheckValidity(float x, float y)
    {
        // raycasts in each direction from a given point to check if it is safe to spawn there
        var point = new Vector2(x, y);
        var up = Physics2D.Raycast(point,
            new Vector2(0, 1),
            _enemyCollider.size.y);
        var down = Physics2D.Raycast(point,
            new Vector2(0, -1),
            _enemyCollider.size.y);
        var left = Physics2D.Raycast(point,
            new Vector2(-1, 0),
            _enemyCollider.size.x);
        var right = Physics2D.Raycast(point,
            new Vector2(1, 0),
            _enemyCollider.size.x);

        return up.transform == null && down.transform == null && left.transform == null && right.transform == null;
    }

    private void SpawnEnemies(int numEnemies)
    {
        for (var i = 0; i < numEnemies; i++)
        {
            float x;
            float y;
            bool valid;
            
            // spawner
            do
            {
                x = Random.Range(leftMapLimit, rightMapLimit);
                y = Random.Range(lowerMapLimit, upperMapLimit);
                valid = CheckValidity(x, y);
            } while (!valid);

            var location = new Vector3(x, y);
            Instantiate(enemyPrefab, location, new Quaternion());
        }
    }
    
    private void EndGame()
    {
        Destroy(gameObject);
    }
}
