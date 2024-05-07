using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField2D : MonoBehaviour
{
    public GameObject _cam;
    FastNoise _fastNoise;
    public Vector2Int _gridSize;
    public Vector2[,] _flowFieldDirection;
    public float _cellSize;
    public GameObject _particlePrefab;
    public float _increment;
    public Vector2 _offset;
    public Vector2 _offsetSpeed;
    public int _particleCount;
    [HideInInspector]
    public List<particle> _particles;
    public float _particleScale, _particleVelocity, _particleRotationSpeed;
    public float _particleSpawnRadius;
    bool _particleSpawnValid(Vector2 position)
    {
        bool valid = true;
        foreach (particle particle in _particles)
        {
            if (Vector2.Distance(position, particle.transform.position) < _particleSpawnRadius)
            {
                valid = false;
                break;
            }
        }
        return valid;
    }
    // Start is called before the first frame update
    void Start()
    {
        _cam.transform.position = new Vector3(_gridSize.x * _cellSize * 0.5f, _gridSize.y * _cellSize * 0.5f, -300);
        _fastNoise = new FastNoise();
        _particles = new List<particle>();

        for (int i = 0; i < _particleCount; i++)
        {
            Vector2 randomPos = new Vector2(
                Random.Range(this.transform.position.x, this.transform.position.x + _gridSize.x * _cellSize),
                Random.Range(this.transform.position.y, this.transform.position.y + _gridSize.y * _cellSize));

            bool isValid = _particleSpawnValid(randomPos);

            if (isValid)
            {
                GameObject particleInstance = Instantiate(_particlePrefab);
                particleInstance.transform.position = randomPos;
                particleInstance.transform.parent = this.transform;
                particleInstance.transform.localScale = new Vector2(_particleScale, _particleScale);
                _particles.Add(particleInstance.GetComponent<particle>());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateFlowFieldDirections();
        particleBehavoir();
    }

    void CalculateFlowFieldDirections()
    {
        _offset = new Vector2(_offset.x + (_offsetSpeed.x * Time.deltaTime), _offset.y + (_offsetSpeed.y * Time.deltaTime));
        _fastNoise = new FastNoise();
        float xOff = 0f;
        for (int x = 0; x < _gridSize.x; x++)
        {
            float yOff = 0f;
            for (int y = 0; y < _gridSize.y; y++)
            {
                float noise = _fastNoise.GetSimplex(xOff + _offset.x, yOff + _offset.y) + 1;
                Vector2 noiseDirection = new Vector2(Mathf.Cos(noise * Mathf.PI), Mathf.Sin(noise * Mathf.PI));
                _flowFieldDirection[x, y] = noiseDirection;
                yOff += _increment;
            }
            xOff += _increment;
        }
    }

    void particleBehavoir()
    {
        foreach (particle p in _particles)
        {
            //X direction
            if (p.transform.position.x > this.transform.position.x + (_gridSize.x * _cellSize))
            {
                p.transform.position = new Vector3(this.transform.position.x, p.transform.position.y, p.transform.position.z);
            }
            if (p.transform.position.x < this.transform.position.x)
            {
                p.transform.position = new Vector3(this.transform.position.x + (_gridSize.x * _cellSize), p.transform.position.y, p.transform.position.z);
            }

            //Y direction
            if (p.transform.position.y > this.transform.position.y + (_gridSize.y * _cellSize))
            {
                p.transform.position = new Vector3(p.transform.position.x, this.transform.position.y, p.transform.position.z);
            }
            if (p.transform.position.y < this.transform.position.y)
            {
                p.transform.position = new Vector3(p.transform.position.x, this.transform.position.y + (_gridSize.y * _cellSize), p.transform.position.z);
            }

            Vector3Int particlePos = new Vector3Int(
               Mathf.FloorToInt(Mathf.Clamp((p.transform.position.x - this.transform.position.x) / _cellSize, 0, _gridSize.x - 1)),
               Mathf.FloorToInt(Mathf.Clamp((p.transform.position.y - this.transform.position.y) / _cellSize, 0, _gridSize.y - 1))
            );
            p.ApplyRotation(_flowFieldDirection[particlePos.x, particlePos.y], _particleRotationSpeed);
            p._velocity = _particleVelocity;
            p.transform.localScale = new Vector3(_particleScale, _particleScale, _particleScale);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(this.transform.position + new Vector3(0.5f * _gridSize.x * _cellSize, 0.5f * _gridSize.y * _cellSize), new Vector3(_gridSize.x * _cellSize, _gridSize.y * _cellSize));
    }
}
