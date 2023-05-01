using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Dzajna {
public class EnemyManager : CharacterManager {
    private EnemyAnimatorManager enemyAnimatorM;
    private EnemyStatsManager enemyStatsManager;

    public NavMeshAgent NavMeshAgent;
    public Rigidbody Rigidbody;
    public State CurrentState;
    public CharacterStats CurrentTarget;

    [Header("AI Settings")] 
    public bool IsPerformingAction;
    public bool IsPhaseShifting;
    public float MinDetectionAngle = 50;
    public float MaxDetectionAngle = -50;
    public float CurrentRecoveryTime = 0;
    public float AggroRange = 10;
    public float RotationSpeed = 100f;
    public float MaxAttackRange = 3f;
    public float MaxAggroRadius = 10f;
    public bool AllowAIToPerformCombos = true;
    public float ComboChance = 50;
    public bool CanRotate;

    protected override void Awake() {
        base.Awake();
        enemyAnimatorM = GetComponent<EnemyAnimatorManager>();
        enemyStatsManager = GetComponent<EnemyStatsManager>();
        NavMeshAgent = GetComponentInChildren<NavMeshAgent>();
        Rigidbody = GetComponent<Rigidbody>();
        BackStabCollider = GetComponentInChildren<CriticalDamageCollider>();
    }
    
    protected override void Start() {
        base.Start();
        NavMeshAgent.enabled = false;
        Rigidbody.isKinematic = false;
    }

    private void Update() {
        
        HandleRecoveryTimer();
        IsPhaseShifting = enemyAnimatorM.Anim.GetBool(enemyAnimatorM.IsPhaseShiftingHash);
        IsInteracting = enemyAnimatorM.Anim.GetBool(enemyAnimatorM.IsInteractingHash);
        CanDoCombo = enemyAnimatorM.Anim.GetBool(enemyAnimatorM.CanDoComboHash);
        CanRotate = enemyAnimatorM.Anim.GetBool(enemyAnimatorM.CanRotateHash);
        IsRotatingWithRM = enemyAnimatorM.Anim.GetBool(enemyAnimatorM.IsRotatingWithRMHash);
        IsInvulnerable = enemyAnimatorM.Anim.GetBool(enemyAnimatorM.IsInvulnerableHash);
        
        enemyAnimatorM.Anim.SetBool(enemyAnimatorM.IsDeadHash, enemyStatsManager.IsDead);
        
        HandleStateMachine();
    }

    private void LateUpdate() {
        NavMeshAgent.transform.localPosition = Vector3.zero;
        NavMeshAgent.transform.localRotation = Quaternion.identity;
        IsInteracting = false;

    }

    private void HandleStateMachine() {
        // Actually handle this error
        if (CurrentState == null) return;
        if (enemyStatsManager.IsDead) return;

        State _nextState = CurrentState.Tick(this, enemyStatsManager, enemyAnimatorM);
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