using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Dzajna {
public class EnemyManager : CharacterManager {
    private EnemyAnimatorManager enemyAnimatorM;
    private EnemyStats enemyStats;

    public NavMeshAgent NavMeshAgent;
    public Rigidbody Rigidbody;
    public State CurrentState;
    public CharacterStats CurrentTarget;

    [Header("AI Settings")] 
    public bool IsPerformingAction;
    public bool IsInteracting;
    public float MinDetectionAngle = 50;
    public float MaxDetectionAngle = -50;
    public float CurrentRecoveryTime = 0;
    public float AggroRange = 10;
    public float RotationSpeed = 100f;
    public float MaxAttackRange = 3f;
    public bool AllowAIToPerformCombos = true;
    public float ComboChance = 50;
    public bool CanRotate;

    [SerializeField] private CapsuleCollider characterCollider;
    [SerializeField] private CapsuleCollider characterCollisionBlockerCollider;
    private void Awake() {
        enemyAnimatorM = GetComponent<EnemyAnimatorManager>();
        enemyStats = GetComponent<EnemyStats>();
        NavMeshAgent = GetComponentInChildren<NavMeshAgent>();
        Rigidbody = GetComponent<Rigidbody>();
        BackStabCollider = GetComponentInChildren<CriticalDamageCollider>();
    }
    
    private void Start() {
        NavMeshAgent.enabled = false;
        Rigidbody.isKinematic = false;
        Physics.IgnoreCollision(characterCollider, characterCollisionBlockerCollider, true);
    }

    private void Update() {
        HandleRecoveryTimer();
        IsInteracting = enemyAnimatorM.Anim.GetBool(enemyAnimatorM.IsInteractingHash);
        CanDoCombo = enemyAnimatorM.Anim.GetBool(enemyAnimatorM.CanDoComboHash);
        CanRotate = enemyAnimatorM.Anim.GetBool(enemyAnimatorM.CanRotateHash);
        IsRotatingWithRM = enemyAnimatorM.Anim.GetBool(enemyAnimatorM.IsRotatingWithRMHash);
        enemyAnimatorM.Anim.SetBool(enemyAnimatorM.IsDeadHash, enemyStats.IsDead);
        
        HandleStateMachine();
    }

    private void LateUpdate() {
        NavMeshAgent.transform.localPosition = Vector3.zero;
        NavMeshAgent.transform.localRotation = Quaternion.identity;
    }

    private void HandleStateMachine() {
        // Actually handle this error
        if (CurrentState == null) return;

        State _nextState = CurrentState.Tick(this, enemyStats, enemyAnimatorM);
        if (_nextState != null) SwitchToNextState(_nextState);
    }

    private void SwitchToNextState(State _newState) {
        CurrentState = _newState;
    }
    
    private void HandleRecoveryTimer() {
        if (CurrentRecoveryTime > 0) CurrentRecoveryTime -= Time.deltaTime;
        if (IsPerformingAction && CurrentRecoveryTime <= 0) IsPerformingAction = false;
    }
}
}