using System;
using System.Collections.Generic;
using UnityEngine;

public class Option
{
    public string Text { get; set; }
    public string Tip { get; set; }
    public bool IsValid
    {
        get
        {
            GameState global = GameState.Instance;
            GameState state = global.Clone() as GameState;
            OnChoose(state);

            // 只允许资源第一次为负
            if (global.Food < 0 && state.Food < global.Food)
            {
                return false;
            }

            if (global.Money < 0 && state.Money < global.Money)
            {
                return false;
            }
            
            return true;
        }
    }
    public Action<GameState> OnChoose { get; set; }
}

public class Question
{
    public string Text { get; set; }
    public Option[] Options { get; set; }
}

public enum SatisfactionTrend
{
    Increase,
    Keep,
    Decrease,
}

public class GameState : Singleton<GameState>, ICloneable
{
    // 回合数
    public int Round { get; set; }

    // 食物
    public int Food { get; set; }

    // 金钱
    public int Money { get; set; }

    // 进度
    public int Progress { get; set; }

    // 食物产出
    public int FoodProductionPerRound
    {
        get
        {
            if (IsFoodProductionStopped)
            {
                return 0;
            }
            else
            {
                return FoodBaseProductionPerRound + FoodAdditionalProductionPerRound;
            }
        }
    }

    // 食物消耗
    public int FoodConsumptionPerRound
    {
        get => FoodBaseConsumptionPerRound + FoodAdditionalConsumptionPerRound;
    }

    // 金币产出
    public int MoneyProductionPerRound
    {
        get
        {
            if (IsMoneyProductionStopped)
            {
                return 0;
            }
            else
            {
                return MoneyBaseProductionPerRound + MoneyAdditionalProductionPerRound;
            }
        }
    }

    // 金钱消耗
    public int MoneyConsumptionPerRound
    {
        get => MoneyBaseConsumptionPerRound + MoneyAdditionalConsumptionPerRound;
    }

    // 进度产出
    public int ProgressProductionPerRound
    {
        get
        {
            if (IsProgressProductionStopped)
            {
                return 0;
            }
            else
            {
                return ProgressBaseProductionPerRound + ProgressAdditionalProductionPerRound;
            }
        }
    }

    // 进度消耗
    public int ProgressConsumptionPerRound
    {
        get
        {
            if (IsBoosted)
            {
                return 0;
            }
            else
            {
                return ProgressBaseConsumptionPerRound + ProgressAdditionalConsumptionPerRound;
            }
        }
    }

    // 工人满意度
    public int WorkerSatisfaction
    {
        get => _workerSatisfaction;
        set
        {
            int satisfaction = Regulate(value);
            if (IsWorkerSatisfactionProtected && satisfaction < _workerSatisfaction)
            {
                return;
            }

            if (IsWorkerSatisfactionSaturated && satisfaction > _workerSatisfaction)
            {
                return;
            }
            _workerSatisfaction = satisfaction;
        }
    }

    private int _workerSatisfaction;

    // 平民满意度
    public int CivilianSatisfaction
    {
        get => _civilianSatisfaction;
        set         
        {
            int satisfaction = Regulate(value);
            if (IsCivilianSatisfactionProtected && satisfaction < _civilianSatisfaction)
            {
                return;
            }

            if (IsCivilianSatisfactionSaturated && satisfaction > _civilianSatisfaction)
            {
                return;
            }
            _civilianSatisfaction = satisfaction;
        }
    }

    private int _civilianSatisfaction;

    // 商人满意度
    public int BusinessmanSatisfaction
    {
        get => _businessmanSatisfaction;
        set
        {
            int satisfaction = Regulate(value);
            if (IsBusinessmanSatisfactionProtected && satisfaction < _businessmanSatisfaction)
            {
                return;
            }

            if (IsCivilianSatisfactionSaturated && satisfaction > _businessmanSatisfaction)
            {
                return;
            }
            _businessmanSatisfaction = satisfaction;
        }
    }

    private int _businessmanSatisfaction;

    // 贵族满意度
    public int AristocratSatisfaction
    {
        get => _aristocratSatisfaction;
        set
        {
            int satisfaction = Regulate(value);
            if (IsAristocratSatisfactionProtected && satisfaction < _aristocratSatisfaction)
            {
                return;
            }

            if (IsAristocratSatisfactionSaturated && satisfaction > _aristocratSatisfaction)
            {
                return;
            }
            _aristocratSatisfaction = satisfaction;
        }
    }

    private int _aristocratSatisfaction;

    // 入轨进度暂停（比如工人都去修神像去了，没人帮着入轨）
    public bool IsProgressProductionStopped { get; set; }

    // 食物供给停止
    public bool IsFoodProductionStopped { get; set; }
    
    // 金钱供给停止
    public bool IsMoneyProductionStopped { get; set; }
    
    // 工人满意度无法下降
    public bool IsWorkerSatisfactionProtected { get; set; }

    // 工人满意度无法上升
    public bool IsWorkerSatisfactionSaturated { get; set; }

    // 平民满意度无法下降
    public bool IsCivilianSatisfactionProtected { get; set; }

    // 平民满意度无法上升
    public bool IsCivilianSatisfactionSaturated { get; set; }
    
    // 商人满意度无法下降
    public bool IsBusinessmanSatisfactionProtected { get; set; }

    // 商人满意度无法上升  
    public bool IsBusinessmanSatisfactionSaturated { get; set; }

    // 贵族满意度无法下降
    public bool IsAristocratSatisfactionProtected { get; set; }
    
    // 贵族满意度无法上升
    public bool IsAristocratSatisfactionSaturated { get; set; }

    // 贵族满意度达到顶峰，火车入轨度不再随着每回合减少，每个选项带来的满意度效果提高 100%
    public bool IsBoosted
    {
        get => _aristocratSatisfaction == 100;
    }

    // 满意度向上还是向下
    public SatisfactionTrend CivilianSatisfactionTrend { get; set; }
    public SatisfactionTrend AristocratSatisfactionTrend { get; set; }
    public SatisfactionTrend WorkerSatisfactionTrend { get; set; }
    public SatisfactionTrend BusinessmanSatisfactionTrend { get; set; }
    
    // 获取当前的问题
    public Question CurrentQuestion()
    {
        return _questionManager.CurrentQuestion();
    }

    // 做出选择，进入下一回合
    public void NextRound(Option option)
    {
        if (IsGameEnd)
        {
            Debug.Log("game already ended, please invoke GameState.Reset() to start the next game");
            return;
        }

        int oldBusinessman = BusinessmanSatisfaction;
        int oldCivilian = CivilianSatisfaction;
        int oldAristocrat = AristocratSatisfaction;
        int oldWorker = WorkerSatisfaction;
        
        option.OnChoose(this);
        if (Progress <= 0 || Progress == 100)
        {
            EndGame();
        }

        foreach (var buff in _buffs)
        {
            buff(Round);
            if (Progress <= 0 || Progress == 100)
            {
                EndGame();
            }
        }

        // 结算资源
        Food += FoodProductionPerRound - FoodConsumptionPerRound;
        Money += MoneyProductionPerRound - MoneyConsumptionPerRound;
        Progress += ProgressProductionPerRound - ProgressConsumptionPerRound;

        Round += 1;
        if (Round == Config.MaxRound)
        {
            EndGame();
        }

        _questionManager.MoveToNextQuestion();
        
        if (oldAristocrat == AristocratSatisfaction)
        {
            AristocratSatisfactionTrend = SatisfactionTrend.Keep;
        } 
        else if (oldAristocrat < AristocratSatisfaction)
        {
            AristocratSatisfactionTrend = SatisfactionTrend.Increase;
        }
        else
        {
            AristocratSatisfactionTrend = SatisfactionTrend.Decrease;
        }        
        
        if (oldCivilian == CivilianSatisfaction)
        {
            CivilianSatisfactionTrend = SatisfactionTrend.Keep;
        } 
        else if (oldCivilian < CivilianSatisfaction)
        {
            CivilianSatisfactionTrend = SatisfactionTrend.Increase;
        }
        else
        {
            CivilianSatisfactionTrend = SatisfactionTrend.Decrease;
        }    
        
        if (oldBusinessman == BusinessmanSatisfaction)
        {
            BusinessmanSatisfactionTrend = SatisfactionTrend.Keep;
        } 
        else if (oldBusinessman < BusinessmanSatisfaction)
        {
            BusinessmanSatisfactionTrend = SatisfactionTrend.Increase;
        }
        else
        {
            BusinessmanSatisfactionTrend = SatisfactionTrend.Decrease;
        }        
        
        if (oldWorker == WorkerSatisfaction)
        {
            WorkerSatisfactionTrend = SatisfactionTrend.Keep;
        } 
        else if (oldWorker < WorkerSatisfaction)
        {
            WorkerSatisfactionTrend = SatisfactionTrend.Increase;
        }
        else
        {
            WorkerSatisfactionTrend = SatisfactionTrend.Decrease;
        }
    }

    // 可以设置游戏结束时的回调
    public void SetGameEndCallback(Action<Ending> callback)
    {
        _endGameCallback = callback;
    }

    // 也可以轮询，看看游戏是否已经结束了
    public bool IsGameEnd { get; private set; }
    public Ending GameEnding { get; private set; }


    // 重置游戏
    public void Reset()
    {
        _questionManager = new QuestionManager();
        _buffs = new List<Action<int>>(); 
        
        Round = 0;
        Food = Config.InitFood;
        Money = Config.InitMoney;
        Progress = Config.InitProgress;
        
        WorkerSatisfaction = Config.InitWorkerSatisfaction;
        CivilianSatisfaction = Config.InitCivilianSatisfaction;
        BusinessmanSatisfaction = Config.InitBusinessmanSatisfaction;
        AristocratSatisfaction = Config.InitAristocratSatisfaction;

        FoodBaseConsumptionPerRound = Config.InitFoodConsumptionRate;
        FoodAdditionalConsumptionPerRound = 0;
        FoodAdditionalProductionPerRound = 0;

        MoneyBaseConsumptionPerRound = Config.InitMoneyConsumptionRate;
        MoneyAdditionalConsumptionPerRound = 0;
        MoneyAdditionalProductionPerRound = 0;

        ProgressAdditionalConsumptionPerRound = 0;
        ProgressAdditionalProductionPerRound = 0;
        
        IsFoodProductionStopped = false;
        IsProgressProductionStopped = false;
        IsMoneyProductionStopped = false;

        IsCivilianSatisfactionProtected = false;
        IsCivilianSatisfactionSaturated = false;
        IsAristocratSatisfactionProtected = false;
        IsAristocratSatisfactionSaturated = false;
        IsBusinessmanSatisfactionProtected = false;
        IsBusinessmanSatisfactionSaturated = false;
        IsWorkerSatisfactionProtected = false;
        IsWorkerSatisfactionSaturated = false;

        BusinessmanSatisfactionTrend = SatisfactionTrend.Keep;
        AristocratSatisfactionTrend = SatisfactionTrend.Keep;
        WorkerSatisfactionTrend = SatisfactionTrend.Keep;
        CivilianSatisfactionTrend = SatisfactionTrend.Keep;

        IsGameEnd = false;
    }

    /******* internal *******/

    public void AddBuff(Action<int> buff)
    {
        _buffs.Add(buff);
    }

    public void RemoveBuff(Action<int> buff)
    {
        _buffs.Remove(buff);
    }

    public void AddOnceTimer(int interval, Action onTimeout)
    {
        int addRound = Round;
        Action<int> buff = delegate(int i)
        {
            if (i - addRound == interval)
            {
                onTimeout();
            }

            // TODO remove 自己
        };

        AddBuff(buff);
    }

    public void AddForeverTimer(int interval, Action onTimeout)
    {
        int addRound = Round;
        Action<int> buff = delegate(int i)
        {
            if ((i - addRound) % interval == 0)
            {
                onTimeout();
            }
        };

        AddBuff(buff);
    }

    public GameState()
    {
        Reset();
    }

    private QuestionManager _questionManager;
    private Action<Ending> _endGameCallback;
    private List<Action<int>> _buffs;


    // 金钱基础产出 （根据居民满意度）
    public int MoneyBaseProductionPerRound
    {
        get
        {
            foreach (var tuple in Config.FoodRate)
            {
                int upperBound = tuple.Item1;
                int production = tuple.Item2;
                if (CivilianSatisfaction <= upperBound)
                {
                    return production;
                }
            }

            return 0;
        }
    }

    // 金钱额外产出 （受某些事件影响）
    public int MoneyAdditionalProductionPerRound { get; set; }

    // 金钱基础消耗 
    public int MoneyBaseConsumptionPerRound { get; set; }

    // 金钱额外消耗（如某些事件触发的）
    public int MoneyAdditionalConsumptionPerRound { get; set; }
    
    // 食物基础产出 （根据商人满意度）
    public int FoodBaseProductionPerRound
    {
        get
        {
            foreach (var tuple in Config.FoodRate)
            {
                int upperBound = tuple.Item1;
                int production = tuple.Item2;
                if (BusinessmanSatisfaction <= upperBound)
                {
                    return production;
                }
            }

            return 0;
        }
    }

    // 食物额外产出 （受某些事件影响）
    public int FoodAdditionalProductionPerRound { get; set; }
    
    // 食物基础消耗
    public int FoodBaseConsumptionPerRound { get; set; }

    // 师傅额外消耗（如某些事件触发的）
    public int FoodAdditionalConsumptionPerRound { get; set; }

    // 进度基础产出 (根据工人满意度)
    public int ProgressBaseProductionPerRound
    {
        get
        {
            foreach (var tuple in Config.ProgressRate)
            {
                int upperBound = tuple.Item1;
                int production = tuple.Item2;
                if (WorkerSatisfaction <= upperBound)
                {
                    return production;
                }
            }

            return 0;
        }
    }
    
    // 进度额外产出 （受某些事件影响）
    public int ProgressAdditionalProductionPerRound;
    
    // 进度基础消耗 （根据贵族满意度）
    public int ProgressBaseConsumptionPerRound
    {
        get
        {
            foreach (var tuple in Config.ProgressConsumptionRate)
            {
                int upperBound = tuple.Item1;
                int production = tuple.Item2;
                if (AristocratSatisfaction <= upperBound)
                {
                    return production;
                }
            }

            return 0;
        }
    }

    // 进度额外消耗 （受某些事件影响）
    public int ProgressAdditionalConsumptionPerRound;

    private int Regulate(int value)
    {
        if (value > 100)
        {
            return 100;
        }
        else if (value < 0)
        {
            return 0;
        }
        else
        {
            return value;
        }
    }

    private void EndGame()
    {
        IsGameEnd = true;

        foreach (var tuple in Config.Endings)
        {
            int upperBound = tuple.Item1;
            Ending ending = tuple.Item2;
            if (Progress <= upperBound)
            {
                GameEnding = ending;
                break;
            }
        }

        if (_endGameCallback != null)
        {
            _endGameCallback(GameEnding);
        }
    }

    // 仅用于 验证
    public object Clone()
    {
        GameState state = new GameState();
        state.Food = Food;
        state.Money = Money;
        return state;
    }
}
