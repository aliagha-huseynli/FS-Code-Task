using UnityEngine;

#region enum
public enum TargetType
{
    Player,
    Enemy
}
#endregion


public class Bullet : MonoBehaviour
{
    #region Field
    private Transform _target = null;
    public TargetType TargetType = TargetType.Enemy;
    [SerializeField] private float _speed = 100;
    private bool _haveTarget = false;
    private Vector3 _targetPosition;
    #endregion

    #region Update
    private void Update()
    {
        if (_target == null)
        {
            return;
        }

        if (!_haveTarget)
        {
            _targetPosition = _target.position;
            _haveTarget = true;
        }
        transform.position = Vector2.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);
    }
    #endregion

    #region OnTriggerEnter2D
    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamagable vulnerable = null;
        if (other.CompareTag("Player") && TargetType == TargetType.Player)
        {
            vulnerable = other.GetComponent<PlayerControl>();
        }
        else if (other.CompareTag("Enemy") && TargetType == TargetType.Enemy)
        {
            vulnerable = other.GetComponent<EnemyControl>();
        }
        if (vulnerable == null) { return; }
        vulnerable.TakeDamage();
        Destroy(gameObject);
    }
    #endregion

    #region Set Target
    public void SetTarget(Transform target)
    {
        this._target = target;
    }
    #endregion
}
