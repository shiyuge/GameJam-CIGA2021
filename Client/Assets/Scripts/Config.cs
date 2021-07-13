using System;
using System.Collections.Generic;

public enum Ending
{
    Banished,
    Useless,
    Forgotten,
    Hope,
    Glory,
    Victory,
}

public static class Config
{
    public static readonly int MaxRound = 18;
    
    public static readonly int InitFood = 10;
    public static readonly int InitMoney = 10;
    public static readonly int InitProgress = 25;

    public static readonly int InitWorkerSatisfaction = 50;
    public static readonly int InitBusinessmanSatisfaction = 50;
    public static readonly int InitCivilianSatisfaction = 50;
    public static readonly int InitAristocratSatisfaction = 50;

    public static readonly int InitFoodConsumptionRate = 1;
    public static readonly int InitMoneyConsumptionRate = 1;

    public static readonly string WelcomeText =
        "这是一个关于末日火车的故事，\n" +
        "这是一列末日列车，只有持续不断的在轨道上行驶才能生存下去，而现在列车已经脱轨，前任列车长也被赶下了火车。\n" +
        "你被选为末日列车的新一任列车长，列车里有4个派系代表了列车脆弱的平衡关系。\n" +
        "你做出的每一个选择都会影响派系对你的满意度。\n" +
        " * 平民派系决定了<color=red>金钱</color>产出，没有<color=red>食物</color>会愤怒\n" +
        " * 劳工派系决定了<color=red>火车入轨进度</color>，没有<color=red>金钱</color>会愤怒\n" +
        " * 商人派系决定了<color=red>食物</color>产出，和贵族派系对立\n" +
        " * 贵族派系给予了你权利，但是会提出不合理的要求。\n\n" +
        "每一个决策都会改变派系对你的态度 ，在维持你的威望的同时，尽量将<color=red>火车入轨进度</color>提升到<color=red>100</color>\n" +
        "你能否把这个火车拖上正轨，还是与你的乘客一起堕入黑暗。\n";
    
    // 平民满意度上限 -> 每回合金钱产出
    public static List<Tuple<int, int>> MoneyRate = new List<Tuple<int, int>>
    {
        Tuple.Create<int, int>(0,0),
        Tuple.Create<int, int>(25, 1),
        Tuple.Create<int, int>(50, 2),
        Tuple.Create<int, int>(75, 3),
        Tuple.Create<int, int>(99, 4),
        Tuple.Create<int, int>(100, 7),
    };
    
    // 工人满意度上限 -> 每回合入轨度产出
    public static List<Tuple<int, int>> ProgressRate = new List<Tuple<int, int>>
    {
        Tuple.Create<int, int>(0,0),
        Tuple.Create<int, int>(25, 1),
        Tuple.Create<int, int>(50, 2),
        Tuple.Create<int, int>(75, 3),
        Tuple.Create<int, int>(99, 4),
        Tuple.Create<int, int>(100, 7),
    };    
    
    // 贵族满意度上限 -> 每回合入轨度消耗
    public static List<Tuple<int, int>> ProgressConsumptionRate = new List<Tuple<int, int>>
    {
        Tuple.Create<int, int>(25, 4),
        Tuple.Create<int, int>(50, 2),
        Tuple.Create<int, int>(75, 1),
        Tuple.Create<int, int>(100, 0),
    };
        
    // 商人满意度上限 -> 每回合食物产出
    public static List<Tuple<int, int>> FoodRate = new List<Tuple<int, int>>
    {
        Tuple.Create<int, int>(0,0),
        Tuple.Create<int, int>(25, 1),
        Tuple.Create<int, int>(50, 2),
        Tuple.Create<int, int>(75, 3),
        Tuple.Create<int, int>(99, 4),
        Tuple.Create<int, int>(100, 7),
    };

    // 火车进度上限 -> 结局
    public static List<Tuple<int, Ending>> Endings = new List<Tuple<int, Ending>>
    {
        Tuple.Create<int, Ending>(0, Ending.Banished),
        Tuple.Create<int, Ending>(25, Ending.Useless),
        Tuple.Create<int, Ending>(50, Ending.Forgotten),
        Tuple.Create<int, Ending>(75, Ending.Hope),
        Tuple.Create<int, Ending>(99, Ending.Glory),
        Tuple.Create<int, Ending>(100, Ending.Victory),
    };

    public static Dictionary<Ending, string> Localization = new Dictionary<Ending, string>
    {
        {Ending.Banished, @"你因为毫无作为而最终被赶下了火车，在这末日荒野里，你与列车的结局都显而易见。"},
        {Ending.Forgotten, @"尽管你用尽了所有的手段，但似乎并没有什么作用，带着人们的诅咒与抱怨，你最终回到了自己的车厢，似乎一切都没有改变。"},
        {Ending.Useless, @"筋疲力尽的你只是将一半列车拉回了正轨，有少数人感谢你的功绩，但是你会很快被遗忘，列车依然在末日荒野之中寸步难行。"},
        {Ending.Hope, @"你赢得了大部分人的尊重，尽管没能在任期内将列车带上正轨，但是你的乘客们看到了希望，也许下一任可以完成你未尽的责任"},
        {Ending.Glory, @"你在列车世界里留下伟大的功绩，你的语音被收集成了语录，你的照片被挂在了车头，每个继任者都将活在你的耀眼光芒之下。"},
        {Ending.Victory, @"在你的英明领导之下，你最终将自己的人民拉上了向着希望前进的轨道，没有人会在乎，在这期间到底牺牲了什么。"},
    };

    // index -> questions
    // 支持不同的 pool 包含同一个 question，也能保证本局游戏内一个问题只会出现一次
    public static readonly List<List<Question>> QuestionPool;

    private static int SatisfactionMultiplier()
    {
        if (GameState.Instance.IsBoosted)
        {
            return 2;
        }

        return 1;
    }
    
    static Config()
    {
        Question decreaseTax = new Question
        {
            Text = "越来越苛刻的税收正在摧毁末日列车的平民们。\n请将征税金额下调一个档次，安抚越来越不满的平民阶级。",
            Options = new[]
            {
                new Option
                {
                    Text = "好的，享受我的仁慈",
                    Tip = "<color=red>每回合金钱获取-1\n<color=green>平民满意度上升20</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.MoneyAdditionalProductionPerRound -= 1;
                        state.CivilianSatisfaction += 20 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "休想偷走我们的钱",
                    Tip = "<color=green>平民满意度下降20</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.CivilianSatisfaction -= 20 * SatisfactionMultiplier();
                    }
                },
            }
        };
       
        Question increaseTax = new Question
        {
            Text = "来自一等车厢的代表提议：\n昨天隔壁车厢投诉了我们发出的噪音，说我们正穷得叮当响。\n我们需要更多的钱，以此来填满我们日益空虚的金库。",
            Options = new[]
            {
                new Option
                {
                    Text = "立刻开始",
                    Tip = "<color=green>每回合金钱获取+1</color>\n<color=green>贵族满意度上升15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.MoneyAdditionalProductionPerRound += 1;
                        state.AristocratSatisfaction += 15 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "不，他们已经一文不名",
                    Tip = "<color=green>平民满意度上升15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.CivilianSatisfaction += 15 * SatisfactionMultiplier();
                    }
                },
            }
        };
        

        Question increaseLuxury = new Question
        {
            Text = "来自一等车厢的代表抱怨道，一等车厢的幸福指数正在快速下降，\n将更多商品供应给一等车厢。",
            Options = new[]
            {
                new Option
                {
                    Text = "您的命令，我的责任",
                    Tip = "<color=red>商人满意度下降10</color>\n<color=greed>贵族满意度上升15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.BusinessmanSatisfaction -= 15 * SatisfactionMultiplier();
                        state.AristocratSatisfaction += 15 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "不，你们已经占用了太多资源",
                    Tip = "<color=greed>商人满意度上升15</color>\n<color=red>贵族满意度下降15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.BusinessmanSatisfaction += 15 * SatisfactionMultiplier();
                        state.AristocratSatisfaction -= 15 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question buildUnion = new Question
        {
            Text = "蒸汽车厢发生了一场暴动，劳工们要求成立工会组织，\n劳工诉求：取消每周96小时工作制度，增加加班津贴。",
            Options = new[]
            {
                new Option
                {
                    Text = "好,让我们来商定合同 ",
                    Tip = "<color=red>每回合金钱消耗+2</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.MoneyAdditionalConsumptionPerRound += 2;
                    }
                },
                new Option
                {
                    Text = "不，这是赤裸裸的敲诈",
                    Tip = "<color=red>劳工满意度下降15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.WorkerSatisfaction -= 15 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question outsource = new Question
        {
            Text = "奥威尔先生发明了一种新的制度，通过他来雇佣劳工。\n可以有效降低我们的金钱消耗，当然，他很残酷。",
            Options = new[]
            {
                new Option
                {
                    Text = "聪明的决策",
                    Tip = "<color=green>每回合金钱消耗-2</color>\n<color=red>劳工满意度下降15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.MoneyAdditionalConsumptionPerRound -= 2;
                        state.WorkerSatisfaction -= 15 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "不，我们要保证每一个工人兄弟的待遇",
                    Tip = "<color=green>劳工满意度上升15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.WorkerSatisfaction += 15 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question cyberPunk = new Question
        {
            Text = "商人们要求改造列车，在车厢中加装传动装置来运送货物。\n毫无疑问，对于追求优雅的乘客来说，这是对传统的亵渎。\n",
            Options = new[]
            {
                new Option
                {
                    Text = "机械永不灭",
                    Tip = "<color=green>每回合食品供应+1</color>\n<color=red>贵族满意度下降15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.FoodAdditionalConsumptionPerRound += 1;
                        state.AristocratSatisfaction -= 15 * SatisfactionMultiplier();
                    }
                    
                },
                new Option
                {
                    Text = "我们不需要机械怪物",
                    Tip = "<color=red>商人满意度下降15</color>\n<color=green>贵族满意度上升15</color>",
                    OnChoose  = delegate(GameState state)
                    {
                        state.BusinessmanSatisfaction -= 15 * SatisfactionMultiplier();
                        state.AristocratSatisfaction += 15 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question starvation = new Question
        {
            Text = "我们正在挨饿，很显然四等车厢的食物配给正在短缺，\n请增加四等车厢的食物供给。",
            Options = new[]
            {
                new Option
                {
                    Text = "好，让每个人都吃饱",
                    Tip = "<color=red>每回合食品消耗+1</color>\n<color=green>平民满意度上升15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.FoodAdditionalConsumptionPerRound += 1;
                        state.CivilianSatisfaction += 15 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "为了伟大的使命，有些挨饿不可避免",
                    Tip = "<color=red>平民满意度下降15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.CivilianSatisfaction -= 15 * SatisfactionMultiplier();
                    }
                },
            }
        };

        Question newTechnology = new Question
        {
            Text = "商人们向你提供一种新的工具，可以有效提高劳工们的效率\n当然价格嘛，有那么一点点昂贵。\n",
            Options = new[]
            {
                new Option
                {
                    Text = "时间就是金钱，我的朋友",
                    Tip = "<color=red>一次性金钱-5</color>\n<color=green>劳工满意度上升15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.Money -= 5;
                        state.WorkerSatisfaction += 15 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "不，我们有足够的人手",
                    Tip = "<color=red>商人满意度下降15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.BusinessmanSatisfaction -= 15 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question stopPlague = new Question
        {
            Text = "四等车厢糟糕的卫生状况引发了一场瘟疫，情况正在变糟\n我们要加派医生还是封闭四等车厢来阻止瘟疫蔓延。\n",
            Options = new[]
            {
                new Option
                {
                    Text = "封闭车厢，请患者不要死在走廊上",
                    Tip = "<color=green>贵族满意度上升5</color>\n<color=green>一次性金钱 -1</color>\n<color=red>平民满意度下降25</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.AristocratSatisfaction += 5 * SatisfactionMultiplier();
                        state.Money -= 1;
                        state.CivilianSatisfaction -= 25 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "把一等车厢的专用医生派到那里去",
                    Tip = "<color=green>一次性金钱-2</color>\n<color=red>贵族满意度下降5</color>\n<color=green>平民满意度上升15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.Money -= 2;
                        state.AristocratSatisfaction -= 5 * SatisfactionMultiplier();
                        state.CivilianSatisfaction += 15 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question theConversion = new Question
        {
            Text = "大主教认为恶魔已经降临在了这趟列车，我们需要拯救\n他要求我们提供一节专用车厢用来传达主的声音。",
            Options = new[]
            {
                new Option
                {
                    Text = "神能创造一个他得不到的车厢吗",
                    Tip = "<color=red>平民满意度下降15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.CivilianSatisfaction -= 15 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "好的，救赎就在其中",
                    Tip = "<color=green>劳工满意度上升15</color>\n<color=red>入轨进度停止增长1回合</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.CivilianSatisfaction += 15 * SatisfactionMultiplier();
                        state.IsProgressProductionStopped = true;
                        state.AddOnceTimer(1, delegate
                        {
                            state.IsProgressProductionStopped = false;
                        });
                    }
                },
            }
        };
        
        Question murder = new Question
        {
            Text = "一位头等车厢的乘客“不小心”谋杀了一位商人\n很显然，贵族们认为您的前途会与这件不幸事息息相关。\n",
            Options = new[]
            {
                new Option
                {
                    Text = "下不为例",
                    Tip = "<color=green>贵族满意上升15</color>\n<color=red>商人满意度下降20</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.AristocratSatisfaction += 15 * SatisfactionMultiplier();
                        state.BusinessmanSatisfaction -= 20 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "这是可耻的谋杀，给我真相",
                    Tip = "<color=red>贵族满意度下降20</color>\n<color=green>商人满意度提升20</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.AristocratSatisfaction -= 20 * SatisfactionMultiplier();
                        state.BusinessmanSatisfaction += 20 * SatisfactionMultiplier();
                    }
                },
            }
        };        
        
        Question fuelShortage = new Question
        {
            Text = "我们的燃料出现了短缺，只足够我们供应给一个车厢。\n是优先供应给锅炉车厢，还是车厢供暖",
            Options = new[]
            {
                new Option
                {
                    Text = "火车入轨进度大于一切",
                    Tip = "<color=green>劳工满意度上升15</color>\n<color=red>平民满意度下降15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.WorkerSatisfaction += 15 * SatisfactionMultiplier();
                        state.CivilianSatisfaction -= 15 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "我们要有一颗温暖的心",
                    Tip = "<color=green>平民满意度上升15</color>\n<color=red>劳工满意度下降15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.CivilianSatisfaction += 15 * SatisfactionMultiplier();
                        state.WorkerSatisfaction -= 15 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question displayRoom = new Question
        {
            Text = "贵族们要求获得一节车厢来作为艺术品陈列馆，\n很显然四等车厢很适合作这样的改造。",
            Options = new[]
            {
                new Option
                {
                    Text = "艺术大于生活",
                    Tip = "<color=green>贵族满意度上升10</color>\n<color=red>平民满意度下降15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.AristocratSatisfaction += 10 * SatisfactionMultiplier();
                        state.CivilianSatisfaction -= 15 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "我们不需要带着眼泪的艺术",
                    Tip = "<color=green>平民满意度上升10</color>\n<color=red>贵族满意度下降15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.CivilianSatisfaction += 10 * SatisfactionMultiplier();
                        state.AristocratSatisfaction -= 15 * SatisfactionMultiplier();
                    }
                },
            }
        };       
        
        Question moonshine = new Question
        {
            Text = "日益猖狂的私酒贩卖正毁灭我们的脆弱的末日火车！！！\n请禁止这样的事情发生。\n",
            Options = new[]
            {
                new Option
                {
                    Text = "这不是垄断，这是独特供应",
                    Tip = "<color=green>每回合金钱获取+2</color>\n<color=green>商人满意度上升15</color>\n<color=red>平民满意度下降10</color>\n<color=red>劳工满意度下降10</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.MoneyAdditionalProductionPerRound += 1;
                        state.BusinessmanSatisfaction += 15 * SatisfactionMultiplier();
                        state.CivilianSatisfaction -= 10 * SatisfactionMultiplier();
                        state.WorkerSatisfaction -= 10 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "我无权干涉他们的自由",
                    Tip = "<color=red>每回合金钱获取-2</color>\n<color=red>商人满意度下降15</color>\n<color=green>平民满意度上升10</color>\n<color=green>劳工满意度上升10</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.MoneyAdditionalConsumptionPerRound -= 1;
                        state.BusinessmanSatisfaction -= 15 * SatisfactionMultiplier();
                        state.CivilianSatisfaction += 10 * SatisfactionMultiplier();
                        state.WorkerSatisfaction += 10 * SatisfactionMultiplier();
                    }
                },
            }
        };       
        
        Question increaseSalary = new Question
        {
            Text = "劳工协会要求提高他们的工资，否则他们就会罢工。\n你可以选择答应他们的要求，或者和劳工协会会长聊聊",
            Options = new[]
            {
                new Option
                {
                    Text = "我同意新的劳资协议",
                    Tip = "<color=red>每回合金钱-3</color>\n<color=green>劳工满意度上升15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.MoneyAdditionalConsumptionPerRound += 3;
                        state.WorkerSatisfaction += 15 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "这里有一笔钱，拿着它，搞定他们",
                    Tip = "<color=red>一次性金钱-3</color>\n<color=red>劳工满意度下降15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.Money -= 3;
                        state.WorkerSatisfaction -= 15 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question overtime = new Question
        {
            Text = "劳工们同意进行一天额外周末高强度加班工作，\n但要支付双倍薪水，需要等待你的批准，先生",
            Options = new[]
            {
                new Option
                {
                    Text = "加班双倍，天经地义",
                    Tip = "<color=red>一次性金钱-15</color>\n<color=green>火车入轨进度+5</color>\n<color=green>劳工满意度上升15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.Money -= 15;
                        state.Progress += 5;
                        state.WorkerSatisfaction += 15 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "额，我们可以晚一点再谈这个问题",
                    Tip = "<color=red>劳工满意度下降15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.WorkerSatisfaction -= 15 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question futureTax = new Question
        {
            Text = "我们的财政出现了赤字，聪明的财政官员提出建议\n我们可以通过收取未来的税收来充实我们的金库。\n",
            Options = new[]
            {
                new Option
                {
                    Text = "你在开玩笑吗？",
                    Tip = "<color=red>每回合金钱获取-1</color>\n<color=green>平民满意度上升10</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.MoneyAdditionalProductionPerRound -= 1;
                        state.CivilianSatisfaction += 15 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "不如，预收到10年之后吧",
                    Tip = "<color=green>一次性金钱+5</color>\n<color=red>平民满意度下降15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.Money += 5;
                        state.CivilianSatisfaction -= 15 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question businessmanMoney = new Question
        {
            Text = "我们的财政出现了赤字，聪明的财政官员提出建议\n我们可以通过向商人收取通过每个车厢的费用来增加收入",
            Options = new[]
            {
                new Option
                {
                    Text = "我不会做这样的事",
                    Tip = "<color=red>每回合金钱获取-1</color>\n<color=green>商人满意度上升15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.Money -= 1;
                        state.BusinessmanSatisfaction += 15 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "不如我们叫它“关税”",
                    Tip = "<color=green>每回合金钱+2</color>\n<color=red>商人满意度下降15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.MoneyAdditionalProductionPerRound += 2;
                        state.BusinessmanSatisfaction -= 15 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question workerMoney = new Question
        {
            Text = "我们的财政出现了赤字，聪明的财政官员提出建议\n我们可以把工人们的税收起始点变成负数来增加收入",
            Options = new[]
            {
                new Option
                {
                    Text = "我不会做这样无耻的事",
                    Tip = "<color=red>每回合金钱获取-1</color>\n<color=green>工人满意度上升15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.MoneyAdditionalProductionPerRound -= 1;
                        state.BusinessmanSatisfaction += 15 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "不如我们叫它“负税”",
                    Tip = "<color=green>每回合金钱获取+2</color>\n<color=red>工人满意度下降15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.MoneyAdditionalProductionPerRound += 2;
                        state.WorkerSatisfaction -= 15 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question aristocratMoney = new Question
        {
            Text = "我们的财政出现了赤字，聪明的财政官员提出建议\n我们可以向贵族征收额外的奢侈品税来增加收入",
            Options = new[]
            {
                new Option
                {
                    Text = "这么做太困难了",
                    Tip = "<color=red>每回合金钱获取-1</color>\n<color=green>贵族满意度上升10</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.MoneyAdditionalProductionPerRound -= 1;
                        state.AristocratSatisfaction += 10 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "是时候让他们知道制度的残酷了",
                    Tip = "<color=green>每回合金钱获取+2</color>\n<color=red>贵族满意度下降10</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.MoneyAdditionalProductionPerRound += 2;
                        state.AristocratSatisfaction -= 10 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question workerFood = new Question
        {
            Text = "食物开始短缺，物资官员被迫调整了供给比例\n我们可以降低工人的分配来实现新的供需平衡",
            Options = new[]
            {
                new Option
                {
                    Text = "不，这太可耻了",
                    Tip = "<color=red>每回合食物获取-1</color>\n<color=green>工人满意度上升10</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.FoodAdditionalConsumptionPerRound += 1;
                        state.WorkerSatisfaction += 10 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "我们可以稍微削减一点点",
                    Tip = "<color=green>每回合食物+1</color>\n<color=red>工人满意度下降10</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.FoodAdditionalConsumptionPerRound -= 1;
                        state.WorkerSatisfaction -= 10 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question aristocratFood = new Question
        {
            Text = "食物开始短缺，物资官员被迫调整了供给比例\n我们可以降低贵族的下午茶种类来实现新的供需平衡",
            Options = new[]
            {
                new Option
                {
                    Text = "太好了，立刻去办",
                    Tip = "<color=green>每回合食物获取+1</color>\n<color=red>贵族满意度下降10</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.FoodAdditionalConsumptionPerRound -= 1;
                        state.AristocratSatisfaction -= 10 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "下午茶的标准是火车的良心！",
                    Tip = "<color=red>每回合食物-1</color>\n<color=green>贵族满意度上升10</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.FoodAdditionalConsumptionPerRound += 1;
                        state.AristocratSatisfaction += 10 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question businessmanFood = new Question
        {
            Text = "食物开始短缺，物资官员被迫调整了供给比例\n我们可以找借口没收商人的货物，这样可以充实我们的食物库存",
            Options = new[]
            {
                new Option
                {
                    Text = "他们违反了即将生效的第二十二条规定",
                    Tip = "<color=green>食物一次性+5</color>\n<color=red>商人满意度下降20</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.Food += 5;
                        state.BusinessmanSatisfaction -= 20 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "可耻的想法！",
                    Tip = "<color=red>平民/贵族满意度下降15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.CivilianSatisfaction -= 15 * SatisfactionMultiplier();
                        state.AristocratSatisfaction -= 15 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question civilianWork = new Question
        {
            Text = "我们的火车入轨进度没有达到新任头等车厢代表的预期，\n他要求我们将更多的平民派去进行无偿劳动",
            Options = new[]
            {
                new Option
                {
                    Text = "更多，更便宜",
                    Tip = "<color=green>每回合火车入轨进度+3</color>\n<color=red>平民满意度下降10</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.Progress += 3;
                        state.CivilianSatisfaction -= 10 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "这是可怕的奴役！",
                    Tip = "<color=red>每回合火车入轨进度-1</color>\n<color=red>贵族满意度下降10</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.Progress -= 1;
                        state.AristocratSatisfaction -= 10 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question moreWorkers = new Question
        {
            Text = "我们刚刚遇到了一批其他列车的工人，他们请求我们收留他们\n当然，安顿这样一大帮人的费用不会很便宜",
            Options = new[]
            {
                new Option
                {
                    Text = "更多，更快",
                    Tip = "<color=red>金钱一次性-5</color>\n<color=green>工人满意度上升15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.Money -= 5;
                        state.WorkerSatisfaction += 15 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "我们已经没有额外的预算了",
                    Tip = "<color=red>工人满意度下降15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.WorkerSatisfaction -= 15 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question letgo = new Question
        {
            Text = "高尚的行政官向你建议，可以将部分车厢脱离列车\n这样可以更快让列车走上正轨。",
            Options = new[]
            {
                new Option
                {
                    Text = " 伟大的事业总是需要那么一点点“牺牲”",
                    Tip = "<color=green>火车入轨进度+10</color>\n<color=red>全部派系满意度下降15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.Progress += 10;
                        state.AristocratSatisfaction -= 15 * SatisfactionMultiplier();
                        state.BusinessmanSatisfaction -= 15 * SatisfactionMultiplier();
                        state.CivilianSatisfaction -= 15 * SatisfactionMultiplier();
                        state.WorkerSatisfaction -= 15 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "我绝不会放弃我们的乘客！",
                    Tip = "<color=green>全部派系满意度上升5</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.AristocratSatisfaction += 5 * SatisfactionMultiplier();
                        state.BusinessmanSatisfaction += 5 * SatisfactionMultiplier();
                        state.CivilianSatisfaction += 5 * SatisfactionMultiplier();
                        state.WorkerSatisfaction += 5 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question challenge = new Question
        {
            Text = "《末日列车报》向你提出一系列挑衅的问题\n你要对此无动于衷还是做出回应?",
            Options = new[]
            {
                new Option
                {
                    Text = "批评本身也是他们的自由",
                    Tip = "<color=green>火车入轨进度+5</color>\n<color=red>全部派系满意度下降10</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.Progress += 5;
                        state.AristocratSatisfaction -= 10 * SatisfactionMultiplier();
                        state.BusinessmanSatisfaction -= 10 * SatisfactionMultiplier();
                        state.CivilianSatisfaction -= 10 * SatisfactionMultiplier();
                        state.WorkerSatisfaction -= 10 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "这个报纸的存在是一个错误",
                    Tip = "<color=green>全部派系满意度上升5</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.AristocratSatisfaction += 5 * SatisfactionMultiplier();
                        state.BusinessmanSatisfaction += 5 * SatisfactionMultiplier();
                        state.CivilianSatisfaction += 5 * SatisfactionMultiplier();
                        state.WorkerSatisfaction += 5 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question holiday = new Question
        {
            Text = "人民要求允许庆祝火车独立日，这一天所有人都不想工作\n你要支持建立这个节日吗？",
            Options = new[]
            {
                new Option
                {
                    Text = "这是一个重要的节日，值得如此",
                    Tip = "<color=red>所有产出停止一回合</color>\n<color=green>所有派系满意度上升10</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.IsFoodProductionStopped = true;
                        state.IsMoneyProductionStopped = true;
                        state.AddOnceTimer(1, delegate
                        {
                            state.IsFoodProductionStopped = false;
                            state.IsMoneyProductionStopped = false;
                        });
                        state.AristocratSatisfaction += 10 * SatisfactionMultiplier();
                        state.BusinessmanSatisfaction += 10 * SatisfactionMultiplier();
                        state.CivilianSatisfaction += 10 * SatisfactionMultiplier();
                        state.WorkerSatisfaction += 10 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "这会破坏我们的排期，导致项目延期",
                    Tip = "<color=red>全部派系满意度下降15</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.AristocratSatisfaction -= 15 * SatisfactionMultiplier();
                        state.BusinessmanSatisfaction -= 15 * SatisfactionMultiplier();
                        state.CivilianSatisfaction -= 15 * SatisfactionMultiplier();
                        state.WorkerSatisfaction -= 15 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question criticize = new Question
        {
            Text = "《末日列车报》连载了一部小说\n上面描写了一个趋炎附势，谎话连篇的列车长，\n毫无疑问，这是在影射你，你要为此采取措施吗？",
            Options = new[]
            {
                new Option
                {
                    Text = "记得给我订一份",
                    Tip = "<color=red>全部派系满意度下降10</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.AristocratSatisfaction -= 10 * SatisfactionMultiplier();
                        state.BusinessmanSatisfaction -= 10 * SatisfactionMultiplier();
                        state.CivilianSatisfaction -= 10 * SatisfactionMultiplier();
                        state.WorkerSatisfaction -= 10 * SatisfactionMultiplier();
                    }
                },
                new Option
                {
                    Text = "回收这一期的所有报纸",
                    Tip = "<color=green>全部派系满意度上升5</color>\n<color=red>金钱一次性-10</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.AristocratSatisfaction += 5 * SatisfactionMultiplier();
                        state.BusinessmanSatisfaction += 5 * SatisfactionMultiplier();
                        state.CivilianSatisfaction += 5 * SatisfactionMultiplier();
                        state.WorkerSatisfaction += 5 * SatisfactionMultiplier();
                        state.Money -= 10;
                    }
                },
            }
        };
        
        Question businessmanHouse = new Question
        {
            Text = "商人们渴望权利，他们希望组成由商人主导的影子议会\n毫无疑问，时代已经改变了，财富才是权利真正的主人",
            Options = new[]
            {
                new Option
                {
                    Text = "好吧，至少我还是列车长",
                    Tip = "<color=red>贵族满意度不再提升</color>\n<color=green>商人满意度不再下降</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.IsAristocratSatisfactionSaturated = true;
                        state.IsBusinessmanSatisfactionProtected = true;
                    }
                },
                new Option
                {
                    Text = "你们这些肮脏的金钱蛆虫，做梦",
                    Tip = "<color=red>商人满意度下降20</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.BusinessmanSatisfaction -= 20 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question workerHouse = new Question
        {
            Text = "工人们渴望权利，他们希望由工人构成的团结政府\n毫无疑问，时代已经改变了，工人才是时代的主人",
            Options = new[]
            {
                new Option
                {
                    Text = "团结的力量",
                    Tip = "<color=red>贵族满意度不再提升</color>\n<color=green>工人满意度不再下降</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.IsAristocratSatisfactionSaturated = true;
                        state.IsWorkerSatisfactionProtected = true;
                    }
                },
                new Option
                {
                    Text = "你们这些卑鄙的工贼，做梦",
                    Tip = "<color=red>工人的满意度下降20</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.WorkerSatisfaction -= 20 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question aristocratHouse = new Question
        {
            Text = "贵族们渴望荣光，他们希望组成一个只有贵族的上级议会\n毫无疑问，他们厌倦了和下等人共同分享权力的悲伤事实",
            Options = new[]
            {
                new Option
                {
                    Text = "光荣的革命？",
                    Tip = "<color=red>所有产出停止5周</color>\n<color=red>贵族满意度不再下降</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.IsMoneyProductionStopped = true;
                        state.IsFoodProductionStopped = true;
                        state.AddOnceTimer(5, delegate
                        {
                            state.IsMoneyProductionStopped = false;
                            state.IsFoodProductionStopped = false;
                        });
                        state.IsAristocratSatisfactionProtected = true;
                    }
                },
                new Option
                {
                    Text = "你们这些披着华服的空心菜，做梦",
                    Tip = "<color=red>贵族满意度下降20</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.AristocratSatisfaction -= 20 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question revolution = new Question
        {
            Text = "工人们发起了一场暴动革命，将贵族老爷们赶进了蒸汽车厢\n你要动用手中的权利来干涉吗",
            Options = new[]
            {
                new Option
                {
                    Text = "这是正常的历史进程",
                    Tip = "<color=red>火车入轨进度停止增长1回合</color>\n贵族与工人的满意度对调",
                    OnChoose = delegate(GameState state)
                    {
                        state.IsProgressProductionStopped = true;
                        state.AddOnceTimer(1, delegate
                        {
                            state.IsProgressProductionStopped = false;
                        });
                        int temp = state.AristocratSatisfaction;
                        state.AristocratSatisfaction = state.WorkerSatisfaction;
                        state.WorkerSatisfaction = temp;
                    }
                },
                new Option
                {
                    Text = "平衡不可以被轻易打破",
                    Tip = "<color=red>火车入轨进度停止增长2回合</color>",
                    OnChoose = delegate(GameState state)
                    {
                        state.IsProgressProductionStopped = true;
                        state.AddOnceTimer(2, delegate
                        {
                            state.IsProgressProductionStopped = false;
                        });
                    }
                },
            }
        };
        
        // 特殊问题
        Question mtoInvitation = new Question
        {
            Text = "历史来到了关键性的转折点，MTO组织向你发出了邀请\n人们要求打开车厢的后门，让其他火车商人进入车厢\n毫无疑问，商人和贵族不会喜欢这个提议，谨慎做出你的选择。",
            Options = new[]
            {
                new Option
                {
                    Text = "开放，大于一切",
                    Tip = "3回合后 → 每回合食物获取+3，食物供应将停止3个回合\n3回合后 → 每回合金钱获取+3，金钱供应将停止3个回合\n平民满意度大幅度上升20",
                    OnChoose = delegate(GameState state)
                    {
                        state.IsFoodProductionStopped = true;
                        state.IsMoneyProductionStopped = true;
                        state.CivilianSatisfaction += 20 * SatisfactionMultiplier();
                        state.AddOnceTimer(3, delegate
                        {
                            state.IsFoodProductionStopped = false;
                            state.IsMoneyProductionStopped = false;
                            state.FoodAdditionalProductionPerRound += 5;
                            state.MoneyAdditionalProductionPerRound += 5;
                        });
                    }
                },
                new Option
                {
                    Text = "我们不需要外来的污染",
                    Tip = "贵族满意度上升25\n商人满意度上升25\n平民满意度下降25",
                    OnChoose = delegate(GameState state)
                    {
                        state.AristocratSatisfaction += 25;
                        state.BusinessmanSatisfaction += 25 * SatisfactionMultiplier();
                        state.CivilianSatisfaction -= 25 * SatisfactionMultiplier();
                    }
                },
            }
        };
        
        Question democracy = new Question
        {
            Text = "历史又一次来到了关键性的转折点，一位西服先生来到了车厢，带来了一些有趣的观点。\n人们要求在末日列车中实现真正的投票制度。\n毫无疑问，民主是更好的制度.但是民主和猴子投飞镖的区别又是什么？",
            Options = new[]
            {
                new Option
                {
                    Text = "我将誓死捍卫你不同意我的权利",
                    Tip = "平民/商人/劳工满意度上升15\n决策失控：每个回合随机一个阵营满意度上升10，随机一个阵营满意度下降10",
                    OnChoose = delegate(GameState state)
                    {
                        state.CivilianSatisfaction += 15 * SatisfactionMultiplier();
                        state.BusinessmanSatisfaction += 15 * SatisfactionMultiplier();
                        state.WorkerSatisfaction += 15 * SatisfactionMultiplier();
                        state.AddForeverTimer(1, delegate
                        {
                            Random r = new Random();
                            int incItem = r.Next(4);
                            int decItem = r.Next(3);
                            if (incItem == 0)
                            {
                                state.CivilianSatisfaction += 10 * SatisfactionMultiplier();
                                if (decItem == 0)
                                {
                                    state.AristocratSatisfaction -= 10 * SatisfactionMultiplier();
                                } else if (decItem == 1)
                                {
                                    state.BusinessmanSatisfaction -= 10 * SatisfactionMultiplier();
                                } else
                                {
                                    state.WorkerSatisfaction -= 10 * SatisfactionMultiplier();
                                }
                            }
                            else if(incItem==1)
                            {
                                state.AristocratSatisfaction += 10 * SatisfactionMultiplier();
                                if (decItem == 0)
                                {
                                    state.CivilianSatisfaction -= 10 * SatisfactionMultiplier();
                                } else if (decItem == 1)
                                {
                                    state.BusinessmanSatisfaction -= 10 * SatisfactionMultiplier();
                                } else
                                {
                                    state.WorkerSatisfaction -= 10 * SatisfactionMultiplier();
                                }
                            }
                            else if(incItem==2)
                            {
                                state.BusinessmanSatisfaction += 10 * SatisfactionMultiplier();
                                if (decItem == 0)
                                {
                                    state.CivilianSatisfaction -= 10 * SatisfactionMultiplier();
                                } else if (decItem == 1)
                                {
                                    state.AristocratSatisfaction -= 10 * SatisfactionMultiplier();
                                } else
                                {
                                    state.WorkerSatisfaction -= 10 * SatisfactionMultiplier();
                                }
                            }
                            else
                            {
                                state.WorkerSatisfaction += 10 * SatisfactionMultiplier();
                                if (decItem == 0)
                                {
                                    state.CivilianSatisfaction -= 10 * SatisfactionMultiplier();
                                } else if (decItem == 1)
                                {
                                    state.AristocratSatisfaction -= 10 * SatisfactionMultiplier();
                                } else
                                {
                                    state.BusinessmanSatisfaction -= 10 * SatisfactionMultiplier();
                                }
                            }
                        });
                    }
                },
                new Option
                {
                    Text = "我不同意",
                    Tip = "贵族满意度上升30\n之后每回合平民/商人/劳工满意度下降3",
                    OnChoose = delegate(GameState state)
                    {
                        state.AristocratSatisfaction += 30;
                        state.AddForeverTimer(1, delegate()
                        {
                            state.CivilianSatisfaction -= 3 * SatisfactionMultiplier();
                            state.BusinessmanSatisfaction -= 3 * SatisfactionMultiplier();
                            state.WorkerSatisfaction -= 3 * SatisfactionMultiplier();
                        });
                    }
                },
            }
        };

        List<Question> normalPool1 = new List<Question>
        {
            decreaseTax,
            increaseTax,
            increaseLuxury,
            buildUnion,
            outsource,
            cyberPunk,
            starvation,
            newTechnology,
            stopPlague,
            theConversion,
            murder,
            fuelShortage,
            displayRoom,
            moonshine,
            increaseSalary,
            overtime,
            futureTax,
            businessmanMoney,
            workerMoney,
            aristocratMoney,
            workerFood,
            aristocratFood,
            businessmanFood,
            civilianWork,
            moreWorkers,
            letgo,
            challenge,
            holiday,
            criticize,
            businessmanHouse,
            workerHouse,
            aristocratHouse,
            revolution,
        };
        List<Question> specialPool1 = new List<Question>{mtoInvitation};
        List<Question> specialPool2 = new List<Question>{democracy};

        QuestionPool = new List<List<Question>>
        {
            normalPool1,
            normalPool1,
            normalPool1,
            normalPool1,
            normalPool1,
            specialPool1,   // 第 6 个是特殊问题
            normalPool1,
            normalPool1,
            normalPool1,
            normalPool1,
            normalPool1,
            specialPool2,  // 第 12 个是特殊问题
            normalPool1,
            normalPool1,
            normalPool1,
            normalPool1,
            normalPool1,
            normalPool1,
            normalPool1,
            normalPool1,
        };
    }
}
