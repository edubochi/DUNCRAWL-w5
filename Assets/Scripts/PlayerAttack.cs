using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // able to check radius
    // able to attack
    Collider[] _enemies;
    public float _radius = 5.0f;
    public LayerMask _enemyLayer;
    public float reach;

    public Transform PlayerTransform;
    void Start()
    {
        
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            EnemyDetermine();
        }

    }
    private void OnDrawGizmos()
    {
        // Draw the sphere.
        Gizmos.DrawWireSphere(transform.position + (PlayerTransform.forward * reach), _radius);

   
    }

    void EnemyDetermine()
    {
        _enemies = Physics.OverlapSphere(transform.position + (PlayerTransform.forward * reach), _radius, _enemyLayer);
        // Get an array for the enemies

        foreach (Collider enemy in _enemies)
        {
            if (enemy.name == "Dummy")
            {
                enemy.GetComponent<DummyEnemy>().TakeDamage();
                //change to variable damage
                return;
            }
            enemy.GetComponent < EnemyHP>().TakeDamage();
        }
    }
}
