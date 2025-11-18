using Unity.Mathematics;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject _projectileWeaponPrefab;
    public GameObject _muzzlePosition;
    public float _speed = 10f;             
    public float _lifePeriod = 3f;
    public LayerMask _ignoreLayer;

    public bool _isChargingLaunch = false; 
    public float _maxChargeTime = 2f;      
    public float _maxChargeMultiplier = 3f;

    private float _currentChargeTime = 0f;
    private bool _isCharging = false;
    private GameObject _projectileBullet;

    float chargePercent;

    void Update()
    {
        if (_isChargingLaunch)
        {
            HandleChargingMode();
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Fire(_muzzlePosition.transform.position, _projectileWeaponPrefab, _speed);
            }
        }
    }

    void HandleChargingMode()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isCharging = true;
            _currentChargeTime = 0f;
        }

        if (Input.GetMouseButton(0) && _isCharging)
        {
            _currentChargeTime += Time.deltaTime;
            _currentChargeTime = Mathf.Min(_currentChargeTime, _maxChargeTime); 
        }

        if (Input.GetMouseButtonUp(0) && _isCharging)
        {
            _isCharging = false;
            chargePercent = _currentChargeTime / _maxChargeTime;
            float chargedSpeed = _speed * Mathf.Lerp(1f, _maxChargeMultiplier, chargePercent);

            Fire(_muzzlePosition.transform.position, _projectileWeaponPrefab, chargedSpeed);
        }
    }

    void Fire(Vector3 _Muzzle, GameObject _prefab, float _prefabSpeed)
    {
        _projectileBullet = Instantiate(_prefab, _Muzzle, transform.rotation);
        Rigidbody rb = _projectileBullet.GetComponent<Rigidbody>();
        _projectileBullet.GetComponent<ShotScript>().charge = chargePercent;

        Vector3 dir = Camera.main.transform.forward;

        if (rb != null)
        {
            rb.linearVelocity = dir.normalized * _prefabSpeed;
            Destroy(_projectileBullet, _lifePeriod);
        }
    }
}
