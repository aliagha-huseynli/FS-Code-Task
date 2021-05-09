using UnityEngine;

public class EnemyControl : MonoBehaviour, IDamagable
{
    #region Field
    [SerializeField] private Transform _player;
    [SerializeField] private GameObject _enemyBullet;
    [SerializeField] private float _speed;
    [SerializeField] private float _stoppingDistance;
    [SerializeField] private float _retreatDistance;
    [SerializeField] private float _startTimeBtwShoots;
    private float _timeBtwShoots;
    public int EnemyMaxHealth = 100;
    public int EnemyCurrentHp;
    public EnemyHpBar HpBar;
    #endregion

    //Change Player Damage HERE -->
    public int DamageToTake { get; set; } = 20;

    #region Start
    private void Start()
    {
        EnemyCurrentHp = EnemyMaxHealth;
        HpBar.SetMaxHealth(EnemyMaxHealth);
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _timeBtwShoots = _startTimeBtwShoots;
    }
    #endregion

    #region Update
    private void Update()
    {
        if (_player == null) return;

        if (Vector2.Distance(transform.position, _player.position) > _stoppingDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, _player.position, _speed * Time.deltaTime);
        }
        else if (Vector2.Distance(transform.position, _player.position) < _stoppingDistance &&
                 Vector2.Distance(transform.position, _player.position) > _retreatDistance)
        {
            transform.position = this.transform.position;
        }
        else if (Vector2.Distance(transform.position, _player.position) < _retreatDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, _player.position, -_speed * Time.deltaTime);
        }

        if (_timeBtwShoots <= 0)
        {
            GameObject bullet = Instantiate(_enemyBullet, transform.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().SetTarget(_player);
            _timeBtwShoots = _startTimeBtwShoots;
        }
        else
        {
            _timeBtwShoots -= Time.deltaTime;
        }
    }
    #endregion

    #region Hit Enemy & Take Damage
    public void HitEnemy(int damage)
    {
        EnemyCurrentHp -= damage;
        HpBar.SetHeath(EnemyCurrentHp);
        if (EnemyCurrentHp <= 0)
        {
            EnemyHolder.Instance.RemoveObjectFromList(transform);
            _player.GetComponent<PlayerControl>().IncreasePoint(10);
            Destroy(gameObject);

        }
    }

    public void TakeDamage()
    {
        HitEnemy(DamageToTake);
    }
    #endregion
}
