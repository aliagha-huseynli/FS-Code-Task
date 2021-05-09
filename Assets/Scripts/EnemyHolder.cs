using System.Collections.Generic;
using UnityEngine;

public class EnemyHolder : MonoBehaviour
{
    #region EnemyHolder
    private static EnemyHolder _instance;

    public static EnemyHolder Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EnemyHolder>();
            }

            return _instance;
        }
    }

    public List<Transform> enemies = null;

    private void Awake()
    {
        _instance = this;
    }

    public void RemoveObjectFromList(Transform obj)
    {
        if (enemies.Contains(obj)) enemies.Remove(obj);
    }
    #endregion
}
