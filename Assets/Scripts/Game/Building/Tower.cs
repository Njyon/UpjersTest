using System;
using System.Collections.Generic;
using UnityEngine;

public enum TowerBuildingState
{
    Unkown,
    TryToBuild,
    Blocked,
    Build,
    Replace
}

public enum TowerState
{
    DoNothing,
    Attack
}

public class Tower : MonoBehaviour, IHoverable, ISelectable, IRequestOwner
{
    // Serialized for debugging in runtime
    [SerializeField] List<Material> materials;
    public Color buildingColor;
    public Color cantBuildColor;
    [HideInInspector] public Vector2Int towerSize; 
    [SerializeField] ColliderScript towerCollider;
    [SerializeField] MeshRenderer towerRangeVisulizer;
    [SerializeField] float towerRange;
    [SerializeField] float checkForFurthestEnemyUpdateTime = 0.1f;
    [SerializeField] float attackSpeed;
    public Vector3 lastPlacedPosition;

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
                    {
                        visualizerComponent.ResetColor();
                        towerRangeVisulizer.enabled = false;
                        // Only cache when tower gets build the first time, otherwise it would be recached when replaced
                        if (towerCost == null)
                            towerCost = GameManager.Instance.ResourceAccountant.GetLastTransaction(); // not safe but good enough for now
                        CreateTowerRequest();
                    } break;
                case TowerBuildingState.Replace:
                    if (buildingState != value)
                    {
                        visualizerComponent.MakeTowerTransparent();
                        lastPlacedPosition = transform.position;
                    } break;
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

    // Should probably be stored in a Prent or somewhere else but good for now
    CurrencyTransaction towerCost;
    SelectorPanelElement requestSelectorPanel;
    [SerializeField] ScriptableRequest scriptableObjectTowerReplace;
    [SerializeField] ScriptableRequest scriptableObjectTowerSell;

    // Keep visiable for debug
    public List<GridTile> gridTilesTheTowerIsBuildOn;

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
            if (towerRangeVisulizer != null) towerRangeVisulizer.enabled = true;
        }
    }

    public void UnHovered()
    {
        if (buildingState == TowerBuildingState.Build)
        {
            if (towerRangeVisulizer != null) towerRangeVisulizer.enabled = false;
        }
    }

    void OnEnemyDied(AEnemy enemy)
    {
        enemiesInRange.Remove(enemy);
        UpdateAttackingTarget();
    }

    /// <summary>
    /// Kinda hacky but i dont want to create a new System for this now
    /// </summary>
    void CreateTowerRequest()
    {
        CreateTowerReplacementRequest();
        CreateTowerSellRequest();
    }

    void CreateTowerSellRequest()
    {
        ScriptableRequest towerSellSR = ScriptableObject.CreateInstance<ScriptableRequest>();
        CopyScriptableRequest(ref towerSellSR, scriptableObjectTowerSell);
        scriptableObjectTowerSell = towerSellSR;
        SellTowerRequest str = new SellTowerRequest();
        if (str != null)
        {
            str.towerCost = towerCost;
            str.tower = this;
            str.Owner = this;
        }
        scriptableObjectTowerSell.Requests.Add(str);
    }

    void CreateTowerReplacementRequest()
    {
        ScriptableRequest towerReplaceSR = ScriptableObject.CreateInstance<ScriptableRequest>();
        CopyScriptableRequest(ref towerReplaceSR, scriptableObjectTowerReplace);
        scriptableObjectTowerReplace = towerReplaceSR;
        ReplaceTowerRequest rtr = new ReplaceTowerRequest();
        if (rtr != null)
        {
            rtr.towerToReplace = this;
            rtr.Owner = this;
        }
        scriptableObjectTowerReplace.Requests.Add(rtr);
    }

    void CopyScriptableRequest(ref ScriptableRequest sr, ScriptableRequest original)
    {
        sr.RequestName = original.RequestName;
        sr.RequestSprite = original.RequestSprite;
        sr.RequestDescription = original.RequestDescription;
        sr.Requests = new List<ARequest>();
        sr.Costs = original.Costs;
    }

    public void Select()
    {
        if (BuildingManager.Instance.TryingToBuild || BuildingManager.Instance.TryingToReplace) return;

        if (buildingState == TowerBuildingState.Build)
        {
            List<ScriptableRequest> requests = new List<ScriptableRequest>();
            requests.Add(scriptableObjectTowerReplace);
            requests.Add(scriptableObjectTowerSell);
            UIManager.Instance.CreateSelectorPanelForRequests(requests, out requestSelectorPanel, this);
            SelectionManager.Instance.contextEvent += OnOpenContext;
        }
      
    }

    public void Unselect()
    {
        if (requestSelectorPanel != null)
        {
            CloseRequestPanel();
        }
    }

    void CloseRequestPanel()
    {
        SelectionManager.Instance.contextEvent -= OnOpenContext;
        UIManager.Instance.UnselectSelectorPanel(requestSelectorPanel);
        requestSelectorPanel = null;
    }

    public void QueueRequest(RequestTransaction requestTransaction)
    {
        CurrencyTransaction currencyTransaction = new();
        foreach (SRequestCost cost in requestTransaction.costs)
        {
            switch (cost.Currency.CurrencyTyp)
            {
                case CurrencyType.Gold:
                    {
                        currencyTransaction.Costs.Add(cost);
                    }
                    break;
                case CurrencyType.Health:
                    {
                        currencyTransaction.Costs.Add(cost);
                    }
                    break;
                default:
                    Ultra.Utilities.Instance.DebugErrorString("BuildingPhase", "QueueRequest", "CurrencyType not implemented or UNKNOWN!");
                    break;
            }
        }

        GameManager.Instance.ResourceAccountant.AddTransaction(currencyTransaction);

        foreach (ARequest request in requestTransaction.requests)
        {
            request.ExecuteRequest(this);
        }

        CloseRequestPanel();
    }

    void OnOpenContext(IContextAction newContext, IContextAction oldContext)
    {
        CloseRequestPanel();
    }
}
