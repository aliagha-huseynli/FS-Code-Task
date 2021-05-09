using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerControl : MonoBehaviour, IDamagable
{
    #region Field 1

    //All Sprites
    [SerializeField] private Sprite[] _idleAnimation;
    [SerializeField] private Sprite[] _jumpAnimation;
    [SerializeField] private Sprite[] _walkAnimation;

    //All Text
    [SerializeField] private Text _hpText;
    [SerializeField] private Text _pointText;
    [SerializeField] private Text _totalPointText;
    [SerializeField] private Text _totalPointTextFinish;

    //All GameObjects
    [SerializeField] private GameObject _deathScreen;
    [SerializeField] private GameObject _finishScreen;
    [SerializeField] private GameObject _playerBullet;

    //All Audio
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _walkAudioClip;
    [SerializeField] private AudioClip _finishAudioClip;

    //All float
    [SerializeField] private float _startTimeBtwShoots;
    [SerializeField] private float _speed;

    //All bool
    [SerializeField] private bool _oneTimeJump = true;

    #endregion

    #region Field 2
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _physics;
    private GameObject _camGameObject;
    private Vector3 _velocity;
    private Vector3 _camLastPosition;
    private Vector3 _camFirstPosition;
    private int _idleAnimationCount;
    private int _walkAnimationCount;
    private int _hp = 100;
    private int _pointQuantity = 0;
    private float _horizontal;
    private float _idleAnimationTime;
    private float _walkAnimationTime;
    private float _timeBtwShoots;
    private bool _isDead = false;
    #endregion

    //Change Enemy Damage HERE -->
    public int DamageToTake { get; set; } = 15;

    #region Start
    private void Start()
    {
        Time.timeScale = 1;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
        _physics = GetComponent<Rigidbody2D>();
        _camGameObject = GameObject.FindGameObjectWithTag("MainCamera");
        _camFirstPosition = _camGameObject.transform.position - transform.position;
        _timeBtwShoots = _startTimeBtwShoots;
        UpdateHpLabel();
        UpdatePointLabel();
    }
    #endregion

    #region All Update Methods
    //Update Method
    private void Update()
    {
        Transform target = GetClosestEnemy();
        if (target == null || !target.gameObject.activeSelf)
        {
            return;
        }

        TryShoot(target);

    }
    //This update method for camera movement
    private void LateUpdate()
    {
        CamControl();
    }
    //This update method for Health Percentage Label/Text
    private void UpdateHpLabel()
    {
        _hpText.text = "" + HealthPercentage;
    }
    //This update method for Point Label/Text
    private void UpdatePointLabel()
    {
        _pointText.text = "" + _pointQuantity;
    }
    //This update method try to control methods which must be deactivate after player death
    private void FixedUpdate()
    {

        if (!_isDead)
        {
            CharacterMovement();
            Animation();
            Jump();
            if (_oneTimeJump)
            {
                _physics.gravityScale = 4;
            }
            else
            {
                _physics.gravityScale = 1;
            }
        }
    }
    #endregion

    #region Get Closest Enemy
    //This method find closest enemy
    private Transform GetClosestEnemy()
    {
        int closestEnemyIndex = -100;
        float leastDistance = 0;
        if (EnemyHolder.Instance.enemies.Count == 0)
        {
            return null;
        }

        for (int i = 0; i < EnemyHolder.Instance.enemies.Count; i++)
        {
            float distance = Vector2.Distance(transform.position, EnemyHolder.Instance.enemies[i].position);
            if (leastDistance == 0)
            {
                leastDistance = distance;
                closestEnemyIndex = i;
            }
            else
            {
                if (distance < leastDistance)
                {
                    leastDistance = distance;
                    closestEnemyIndex = i;
                }
            }
        }
        return EnemyHolder.Instance.enemies[closestEnemyIndex];
    }
    #endregion

    #region Shoot // Fire
    //This method try to shoot or fire
    private void TryShoot(Transform target)
    {
        if (_timeBtwShoots <= 0)
        {
            var bullet = Instantiate(_playerBullet, transform.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().SetTarget(target);
            _timeBtwShoots = _startTimeBtwShoots;
        }
        else
        {
            _timeBtwShoots -= Time.deltaTime;
        }
    }
    #endregion

    #region Jump
    //This method for jumping
    private void Jump()
    {
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            if (_oneTimeJump)
            {
                if (_audioSource.isPlaying)
                {
                    _audioSource.Stop();
                }
                _physics.velocity = new Vector2(_physics.velocity.x, 0);
                _physics.AddForce(new Vector2(0, 500));
                _oneTimeJump = false;
            }
        }
    }
    #endregion

    #region Death
    //When Player Hp is 0 Zero
    private int HealthPercentage
    {
        get => _hp;
        set
        {
            if (value <= 0)
            {
                value = 0;
                _isDead = true;
                ShowDeathScene();
            }
            _hp = value;
        }
    }
    //When Player die
    private void ShowDeathScene()
    {
        Time.timeScale = 0;
        _audioSource.Stop();
        _deathScreen.SetActive(true);
        _totalPointText.text = "Total point: " + _pointQuantity;
    }
    #endregion

    #region Finish Scene
    public void ShowFinishScene()
    {
        Time.timeScale = 0;
        _audioSource.Stop();
        _finishScreen.SetActive(true);
        _totalPointTextFinish.text = "Total point: " + _pointQuantity;
    }
    #endregion

    #region Camera Control
    //Camera Control Method
    private void CamControl()
    {
        _camLastPosition = _camFirstPosition + transform.position;
        _camGameObject.transform.position = Vector3.Lerp(_camGameObject.transform.position, _camLastPosition, 0.07f);
    }

    #endregion

    #region Move // Walk
    //This method for moving or walking
    private void CharacterMovement()
    {
        _horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        if (_horizontal == 0)
        {
            _audioSource.clip = _walkAudioClip;
            _audioSource.Stop();
        }
        else
        {
            if (!_audioSource.isPlaying && _oneTimeJump)
            {
                _audioSource.clip = _walkAudioClip;
                _audioSource.Play();
            }
        }
        _velocity = new Vector3(_horizontal * 5, _physics.velocity.y, 0);
        _physics.velocity = _velocity;
    }
    #endregion

    #region OnCollisionEnter2D
    //This method control jumping and showing finish scene
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _oneTimeJump = true;
        }

        if (other.gameObject.CompareTag("Finishline"))
        {
            ShowFinishScene();
        }
    }
    #endregion

    #region Hit & Reduce Health & Take Damage
    public void HitPlayer(int damage)
    {
        ReduceHealth(damage);
        UpdateHpLabel();
    }

    private void ReduceHealth(int quantity)
    {
        HealthPercentage -= quantity;
    }

    public void TakeDamage()
    {
        HitPlayer(DamageToTake);
    }
    #endregion

    #region Increase Player Point
    //This method for increasing Player Point
    public void IncreasePoint(int quantity)
    {
        _pointQuantity += quantity;
        _pointText.text = _pointQuantity.ToString();
    }
    #endregion

    #region Player Animation - Idle -> Walk -> Jump
    //This method for Animation of Player. Idle, Walk, Jump
    private void Animation()
    {
        if (_oneTimeJump)
        {
            if (_horizontal == 0)
            {
                _idleAnimationTime += Time.deltaTime;
                if (_idleAnimationTime > 0.40f)
                {
                    _spriteRenderer.sprite = _idleAnimation[_idleAnimationCount++];
                    if (_idleAnimationCount == _idleAnimation.Length)
                    {
                        _idleAnimationCount = 0;

                    }
                    _idleAnimationTime = 0;
                }
            }
            else if (_horizontal > 0)
            {
                _walkAnimationTime += Time.deltaTime;
                if (_walkAnimationTime > 0.05f)
                {
                    _spriteRenderer.sprite = _walkAnimation[_walkAnimationCount++];
                    if (_walkAnimationCount == _walkAnimation.Length)
                    {
                        _walkAnimationCount = 0;
                    }
                    _walkAnimationTime = 0;
                }
                transform.localScale = new Vector3(8, 8, 1);
            }
            else if (_horizontal < 0)
            {
                _walkAnimationTime += Time.deltaTime;
                if (_walkAnimationTime > 0.05f)
                {
                    _spriteRenderer.sprite = _walkAnimation[_walkAnimationCount++];
                    if (_walkAnimationCount == _walkAnimation.Length)
                    {
                        _walkAnimationCount = 0;
                    }
                    _walkAnimationTime = 0;
                }
                transform.localScale = new Vector3(-8, 8, 1);
            }
        }
        else
        {
            _spriteRenderer.sprite = _physics.velocity.y > 0 ? _jumpAnimation[0] : _jumpAnimation[1];

            if (_horizontal > 0)
            {
                transform.localScale = new Vector3(8, 8, 1);
            }
            else if (_horizontal < 0)
            {
                transform.localScale = new Vector3(-8, 8, 1);
            }
        }
    }
    #endregion
}