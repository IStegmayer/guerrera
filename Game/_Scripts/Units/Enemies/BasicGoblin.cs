using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicGoblin : BaseUnit
{
    [Header("Combat")] 
    [SerializeField] private EnemyMeleeDamageDealer _damageDealer;
    [SerializeField] private float _attackRange = 1.6f;
    [SerializeField] private float _aggroRange = 6f;
    [SerializeField] private float _attackCD = 2f;
    [SerializeField] private float _reaggroCD = 0.5f;


    private int _currentHealth;
    private Transform _playerTransform;
    private Coroutine _deathRoutine;
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;

    private float _attackTimer;

    private void Start() {
        _currentHealth = Stats.Health;
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _playerTransform = GameObject.FindWithTag("Player").transform;
    }

    private void Update() {
        if (_playerTransform == null) return;

        // attack 
        if (_attackTimer >= _attackCD) {
            if (Vector3.Distance(_playerTransform.position, transform.position) <= _attackRange) {
                _damageDealer.ToggleEnableDamage(true);
                _animator.SetTrigger("Attack");
                _attackTimer = 0;
            }
        }

        _attackTimer += Time.deltaTime;

        // aggro
        if (_reaggroCD <= 0 && Vector3.Distance(_playerTransform.position, transform.position) <= _aggroRange) {
            _reaggroCD = 0.5f;
            _navMeshAgent.destination = _playerTransform.position;
            _animator.SetBool("isMoving", true);
        }

        _reaggroCD -= Time.deltaTime;
        transform.LookAt(_playerTransform.position);
    }

    //TODO: Create a separate health/damage script for everyone
    public override void TakeDamage(int dmg) {
        if (_deathRoutine != null) return;
        Debug.Log("HEALTH:" + _currentHealth + "/" + Stats.Health);

        _animator.Play("Damage");
        _currentHealth -= dmg;
        if (_currentHealth < 0) {
            _deathRoutine = StartCoroutine(DeathRoutine());
        }
    }

    private IEnumerator DeathRoutine() {
        // start death animation
        _animator.Play("Death");
        yield return new WaitForSeconds(2f);
        UnitManager.Instance.DespawnEnemy(_id, this.gameObject);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _aggroRange);
    }
}