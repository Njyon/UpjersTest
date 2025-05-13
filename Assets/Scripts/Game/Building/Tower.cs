using System.Collections.Generic;
using UnityEngine;

public enum TowerBuildingState
{
    Unkown,
    TryToBuild,
    Blocked,
    Build
}

public enum TowerState
{
    DoNothing,
    Attack
}

public class Tower : MonoBehaviour, IHoverable
{
    // Serialized for debugging in runtime
    [SerializeField] List<Material> materials;
    public Color buildingColor;
    public Color cantBuildColor;
    [SerializeField] ColliderScript towerCollider;
    [SerializeField] MeshRenderer towerRangeVisulizer;
    [SerializeField] float towerRange;
    [SerializeField] float checkForFurthestEnemyUpdateTime = 0.1f;
    [SerializeField] float attackSpeed;

    TowerVisualizerComponent visualizerComponent;
    Ultra.Timer checkForFurthestEnemyTimer;
    Ultra.Timer attackTimer;
    [HideInInspector] public AEnemy attackingTarget;
    [HideInInspector] public List<AEnemy> enemiesInRange;

    [SerializeReference, SubclassSelector]
    public AAttackLogic attackLogic;

    TowerBuildingState buildingState;
    public TowerBuildingState BuildingState
    {
        get { return buildingState; }
        set
        {
            switch (value)
            {
                case TowerBuildingState.TryToBuild:
                    if (buildingState != value)
                        visualizerComponent.MakeTowerTransparent();
                    break;
                case TowerBuildingState.Blocked:
                    if (buildingState != value)
                        visualizerComponent.MakeTowerTileIsBlockedColor();
                    break;
                case TowerBuildingState.Build:
                    if (buildingState != value)
                        visualizerComponent.ResetColor();
                        towerRangeVisulizer.enabled = false;
                    break;
                default:
                    Ultra.Utilities.Instance.DebugErrorString("Tower", "BuildingState", "BuildingState not Implemented");
                    break;
            }

            buildingState = value;
        }
    }

    TowerState towerState;
    public TowerState TowerState
    {
        get { return towerState; }
        set
        {
            switch (value)
            {
                case TowerState.DoNothing:
                    StopAttacking();
                    break;
                case TowerState.Attack:
                    StartAttacking();
                    break;
                default:
                    break;
            }
            towerState = value;
        }
    }

    void Awake()
    {
        visualizerComponent = new TowerVisualizerComponent(this);
        checkForFurthestEnemyTimer = new Ultra.Timer(checkForFurthestEnemyUpdateTime);
        attackTimer = new Ultra.Timer(attackSpeed);
        checkForFurthestEnemyTimer.onTimerFinished += OncheckForFurthestEnemyTimerFinished;
        attackTimer.onTimerFinished += OnAttackTimerFinished;

        visualizerComponent.ChacheMaterials();

        towerCollider.gameObject.transform.localScale = new Vector3(towerRange, towerCollider.gameObject.transform.localScale.y, towerRange);
        enemiesInRange = new List<AEnemy>();
        towerCollider.onOverlapEnter += OnTowerColliderOverlapEnter;
        towerCollider.onOverlapExit += OnTowerColliderOverlapExit;

        attackLogic.Init(this);
    }

    void Update()
    {
        if (checkForFurthestEnemyTimer != null) checkForFurthestEnemyTimer.Update(Time.deltaTime);
        if (attackTimer != null) attackTimer.Update(Time.deltaTime);
    }

    private void OnDestroy()
    {
        if (towerCollider != null)
        {
            towerCollider.onOverlapEnter -= OnTowerColliderOverlapEnter;
            towerCollider.onOverlapExit -= OnTowerColliderOverlapExit;
        }
        if (checkForFurthestEnemyTimer != null)
            checkForFurthestEnemyTimer.onTimerFinished -= OncheckForFurthestEnemyTimerFinished;
        if (attackTimer != null)
            attackTimer.onTimerFinished -= OnAttackTimerFinished;


    }

    void OnTowerColliderOverlapEnter(Collider other)
    {
        if (other != null)
        {
            AEnemy enemy = other.GetComponent<AEnemy>();
            if (enemy != null)
            {
                enemiesInRange.Add(enemy);
                enemy.onEnemyDied += OnEnemyDied;
                TowerState = TowerState.Attack;
            }
        } 
    }

    void OnTowerColliderOverlapExit(Collider other)
    {
        if (other != null)
        {
            AEnemy enemy = other.GetComponent<AEnemy>();
            if (enemy != null)
            {
                enemy.onEnemyDied -= OnEnemyDied;
                enemiesInRange.Remove(enemy);
                UpdateAttackingTarget();
                if (enemiesInRange.Count <= 0)
                {
                    towerState = TowerState.DoNothing;
                }
            }
        }
    }

    void OncheckForFurthestEnemyTimerFinished()
    {
        UpdateAttackingTarget();
        checkForFurthestEnemyTimer.Start();
    }

    void OnAttackTimerFinished()
    {
        AttackTarget();
        attackTimer.Start();
    }

    void StartAttacking()
    {
        if (attackTimer.IsRunning) return;
        
        UpdateAttackingTarget();
        checkForFurthestEnemyTimer.Start();

        AttackTarget();
        attackTimer.Start();
    }

    void UpdateAttackingTarget()
    {
        float progress = -1;
        foreach (AEnemy enemy in enemiesInRange)
        {
            float enemyProgress = enemy.GetPathProgress();
            if (enemyProgress > progress)
            {
                progress = enemyProgress;
                attackingTarget = enemy;
            }
        }
    }

    void StopAttacking()
    {
        checkForFurthestEnemyTimer.Stop();
        attackTimer.Stop();
        attackingTarget = null;
    }

    protected virtual void AttackTarget()
    {
        if (attackingTarget != null)
        {
            attackLogic.Attack();
        }
    }

    public void Hovered()
    {
        if (buildingState == TowerBuildingState.Build)
        {
            towerRangeVisulizer.enabled = true;
        }
    }

    public void UnHovered()
    {
        if (buildingState == TowerBuildingState.Build)
        {
            towerRangeVisulizer.enabled = false;
        }
    }

    void OnEnemyDied(AEnemy enemy)
    {
        enemiesInRange.Remove(enemy);
        UpdateAttackingTarget();
    }
}
