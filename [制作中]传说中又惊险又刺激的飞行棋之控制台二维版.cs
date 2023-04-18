using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Xml.Linq;
using static System.Collections.Specialized.BitVector32;
using static System.Formats.Asn1.AsnWriter;

namespace LessonMai//传说中又惊险又刺激的飞行棋之控制台二维版
{
    public struct GamePlayer//结构体，区分是玩家还是Ai、玩家名字、当前走过的部署
    {
        public bool ai;
        public string name;
        public int steps;
    }
    public struct PropsProps//游戏道具构造体，可以在游戏主函数中
    {
        public int[] 折跃门;
        public int[] 妙妙屋;
        public int[] 占卜师;
        public int[] 商店;
        public int[] 太空掠食者;
    }
    public enum E_PropsProps//游戏场景枚举
    {
        折跃门,
        妙妙屋,
        占卜师,
        商店,
        太空掠食者,
    }
    //—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线
    enum E_GameScene//游戏场景枚举
    {
        lowerLimit,
        mainMenu,
        mainGame,
        upperLimit,
    }
    enum E_MainMenu//主菜单枚举
    {
        lowerLimit,
        startGame,
        quitGame,
        upperLimit,
    }
    enum E_GameMod//游戏模式枚举
    {
        lowerLimit,
        HotSeat,//热座模式，即单设备的多人本地对战
        BabyBus,//宝宝巴士模式，即简单人机对战
        IntellectualEquipmentCrisis,//智械危机模式，即普通人机对战
        upperLimit,
    }
    enum E_LevelData
    {
        Height,
        Width,
    }
    //—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线
    public class Windows
    //窗体的类
    {
        public static int GetWindowsHeight()//高度接口
        {
            const int Height = 29;
            return Height;
        }
        public static int GetWindowsWidth()//宽度接口
        {
            const int Width = 160;
            return Width;
        }
        public static void SetWindows()//默认调用接口并设置
        {
            Console.CursorVisible = false;
            int Height = GetWindowsHeight();
            int Width = GetWindowsWidth();
            Console.SetWindowSize(Width, Height);
            Console.SetBufferSize(Width, Height);
        }
    }
    //—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线
    class Snake
    {
        //—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线
        //—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线
        static void Main(string[] args)//游戏主函数
        {
            Windows.SetWindows();//进入游戏时，设置窗口
            //PlotBroadcast();//开始剧情
            E_GameScene e_GameScene = E_GameScene.mainMenu;//调用枚举
            Scene(ref e_GameScene);

        }
        //—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线
        //—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线
        static void Scene(ref E_GameScene e_GameScene)
        {
            while (true)
            {
                switch (e_GameScene)
                {
                    case E_GameScene.mainMenu:
                        mainMenu(ref e_GameScene);
                        break;
                    case E_GameScene.mainGame:
                        GameScene(ref e_GameScene);
                        break;
                }
            }
        }
        //—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线
        static void GameScene(ref E_GameScene e_GameScene)
        {
            Console.Clear();
            PrintingMap();//印刷场景
            PrintingRules();
            int LevelData = FightMap(true);
            #region 声明会用到的变量
            int Progress = 0;
            int steps = 0;
            bool firsTime = true;
            bool Manual = false;
            int action = 1;
            string ActorName = "";
            bool gameRuns = true;
            #endregion
            //—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线
            #region 声明玩家和电脑的结构体
            GamePlayer Player;
            Player.steps = 0;
            Player.name = "★";
            Player.ai = false;
            GamePlayer AlienStar;
            AlienStar.steps = 0;
            AlienStar.name = "●";
            AlienStar.ai = true;
            #endregion
            //—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线
            #region 声明场景道具，初始化它们的参数，枚举参数的数量代表着道具的数量
            PropsProps propsProps;
            propsProps.折跃门 = new int[10];//折跃门**************************************************************************************
            propsProps.妙妙屋 = new int[5];//妙妙屋**************************************************************************************
            propsProps.占卜师 = new int[5];//占卜师**************************************************************************************
            propsProps.商店 = new int[5];//商店******************************************************************************************
            propsProps.太空掠食者 = new int[15];//太空掠食者********************************************************************************
            #endregion
            #region 将道具的坐标储存进propsPlacement，为他们编号
            int[][] propsPlacement = { propsProps.折跃门, propsProps.妙妙屋, propsProps.占卜师, propsProps.商店, propsProps.太空掠食者 };
            #endregion
            #region 将编好号的道具列表传入方法，将其随机化
            propsPlacement = PropsPlacement(LevelData, propsPlacement);//生成随机道具参数
            #endregion
            //—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线
            PropMap(propsPlacement);//地图印刷
            //—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线
            #region 投掷骰子并前进的逻辑
            Random random = new Random();
            int[] printCoordinates;
            while (gameRuns)
            {
                FightMap();//场景印刷
                PropMap(propsPlacement);//地图印刷
                action++;//循环开始时，进行行动
                action = action == 3 ? 1 : action;
                if (action == 1)//行动为单数时，是玩家的行动。行动为双数时，是电脑的行动
                {
                    ActorName = Player.name;
                    Progress = Player.steps;
                    Manual = Player.ai;
                }
                else
                {
                    ActorName = AlienStar.name;
                    Progress = AlienStar.steps;
                    Manual = AlienStar.ai;
                }
                #region 存在优化空间的代码，应该有更好的印刷方式
                printCoordinates = CoordinateSystemConversion(Player.steps);
                Console.SetCursorPosition(printCoordinates[0], printCoordinates[1]);
                WriteLineColorOnce(Player.name, false);

                printCoordinates = CoordinateSystemConversion(AlienStar.steps);
                Console.SetCursorPosition(printCoordinates[0], printCoordinates[1]);
                WriteLineColorOnce(AlienStar.name, false, ConsoleColor.DarkGreen);
                if (Player.steps == AlienStar.steps)
                {
                    printCoordinates = CoordinateSystemConversion(AlienStar.steps);
                    Console.SetCursorPosition(printCoordinates[0], printCoordinates[1]);
                    WriteLineColorOnce("★", false, ConsoleColor.DarkGreen);
                }
                #endregion
                if (firsTime)
                {
                    Console.SetCursorPosition(Windows.GetWindowsWidth() - Windows.GetWindowsWidth() / 4, Windows.GetWindowsHeight() - (Windows.GetWindowsHeight() / 3) + 2);
                    Console.Write("按下回车键开始丢骰子");
                    firsTime = false;
                }
                else
                {
                    if (!Manual)
                    {
                        Console.SetCursorPosition(Windows.GetWindowsWidth() - Windows.GetWindowsWidth() / 4, Windows.GetWindowsHeight() - (Windows.GetWindowsHeight() / 3) + 3);
                        Console.Write("按下回车键开始丢骰子");
                    }
                    else
                    {
                        Thread.Sleep(1000);
                        Console.SetCursorPosition(Windows.GetWindowsWidth() - Windows.GetWindowsWidth() / 4, Windows.GetWindowsHeight() - (Windows.GetWindowsHeight() / 3) + 3);
                        Console.Write("敌人的回合等待丢骰子");
                    }
                }
                if (!Manual)
                {
                    Console.ReadLine();
                }
                for (int i = 0; i < 10; i++)
                {
                    steps = random.Next(1, 7);
                    Thread.Sleep(10);
                    Console.SetCursorPosition(Windows.GetWindowsWidth() - Windows.GetWindowsWidth() / 4, Windows.GetWindowsHeight() - (Windows.GetWindowsHeight() / 3) + 2);
                    Console.Write("本回合骰子的点数是:{0}", steps);
                }
                Console.SetCursorPosition(Windows.GetWindowsWidth() - Windows.GetWindowsWidth() / 4, Windows.GetWindowsHeight() - (Windows.GetWindowsHeight() / 3) + 2);
                Console.Write("本回合骰子的点数是:");
                WriteLineColorOnce(steps.ToString(), false);
                Progress = Progress + steps * 2;
                if (action == 1)//行动结束时，清除行动的记录。
                {
                    Player.steps = Progress;//结算的数值进行保存
                }
                else
                {
                    AlienStar.steps = Progress;//结算的数值进行保存
                }
                if (Progress >= LevelData)
                {
                    gameRuns = false;
                    Console.Clear();
                    if (Player.steps >= Progress)
                    {
                        WriteLineColorOnce("游戏结束！" + Player.name + "胜利！");
                    }
                    else
                    {
                        WriteLineColorOnce("游戏结束！" + AlienStar.name + "胜利！", true, ConsoleColor.DarkGreen);
                    }
                    Thread.Sleep(10000);
                    Environment.Exit(0);//正常退出
                }
            }
            #endregion
        }
        //—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线
        static void PropMap(int[][] propsPlacement)
        {
            int[] printCoordinates;
            for (int I = 0; I < propsPlacement.Length; I++)
            {
                for (int i = 0; i < propsPlacement[I].Length; i++)
                {
                    printCoordinates = CoordinateSystemConversion(propsPlacement[I][i]);
                    Console.SetCursorPosition(printCoordinates[0], printCoordinates[1]);
                    switch (I)
                    {
                        case (int)E_PropsProps.折跃门:
                            WriteLineColorOnce("门", false, ConsoleColor.Blue);
                            break;
                        case (int)E_PropsProps.妙妙屋:
                            WriteLineColorOnce("妙", false, ConsoleColor.DarkRed);
                            break;
                        case (int)E_PropsProps.占卜师:
                            WriteLineColorOnce("卜", false, ConsoleColor.Magenta);
                            break;
                        case (int)E_PropsProps.商店:
                            WriteLineColorOnce("商", false, ConsoleColor.Yellow);
                            break;
                        case (int)E_PropsProps.太空掠食者:
                            WriteLineColorOnce("掠", false, ConsoleColor.Red);
                            break;
                    }
                }
            }
        }
        static int[][] PropsPlacement(int LevelData, params int[][] propsIncoming)
        {
            Random random = new Random();
            for (int I = 0; I < propsIncoming.Length; I++)
            {
                for (int i = 0; i < propsIncoming[I].Length; i++)
                {
                    propsIncoming[I][i] = random.Next(LevelData);
                }
            }
            return propsIncoming;//生成随机地图
        }
        //—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线
        #region 场景居中打印
        static void inTheMiddle(string writeLine)//场景居中打印
        {
            int Height = Windows.GetWindowsHeight();
            int Width = Windows.GetWindowsWidth();
            Console.SetCursorPosition(Width / 2 - writeLine.Length, Height / 2);
        }
        static void WriteLineColorOnce(string writeLine)//默认不写则是居中,红色印刷，结束后初始化回白色
        {
            inTheMiddle(writeLine);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(writeLine);
            Console.ForegroundColor = ConsoleColor.White;
        }
        static void WriteLineColorOnce(string writeLine, bool middle)//默认不写则是红色印刷，结束后初始化回白色
        {
            if (middle) { inTheMiddle(writeLine); }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(writeLine);
            Console.ForegroundColor = ConsoleColor.White;
        }
        static void WriteLineColorOnce(string writeLine, bool middle, ConsoleColor colorStart)//重载，可填写印刷颜色，结束后初始化回白色
        {
            if (middle) { inTheMiddle(writeLine); }
            Console.ForegroundColor = colorStart;
            Console.WriteLine(writeLine);
            Console.ForegroundColor = ConsoleColor.White;
        }
        static void WriteLineColorOnce(string writeLine, bool middle, ConsoleColor colorStart, ConsoleColor colorEnd)//重载，可填写印刷颜色，结束后可填写初始化颜色
        {
            if (middle) { inTheMiddle(writeLine); }
            Console.ForegroundColor = colorStart;
            Console.WriteLine(writeLine);
            Console.ForegroundColor = colorEnd;
        }
        #endregion
        //—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线
        static E_GameScene mainMenu(ref E_GameScene e_GameScene)//主菜单逻辑
        {
            int Height = Windows.GetWindowsHeight();
            int Width = Windows.GetWindowsWidth();
            string[] text =
                {
                "传说中又惊险又刺激的飞行棋之控制台二维版",
                "开始游戏",
                "退出游戏"
                };
            E_MainMenu mainMenu = E_MainMenu.startGame;
            for (mainMenu = 0; ((int)mainMenu) < text.Length; ++mainMenu)
            {
                Console.SetCursorPosition(Width / 2 - text[((int)mainMenu)].Length, Height / 2 + ((int)mainMenu) * 2);
                Console.Write(text[((int)mainMenu)]);
            }
            mainMenu = E_MainMenu.startGame;
            //进入主菜单时，打印主菜单文字，打印完毕后重新初始化

            Console.SetCursorPosition(Width / 2 - text[((int)mainMenu)].Length, Height / 2 + ((int)mainMenu) * 2);
            WriteLineColorOnce(text[((int)mainMenu)], false);
            //进入主菜单时，标红以高亮默认选项

            while (true)
            {
                Char Input = Console.ReadKey(true).KeyChar;
                Console.SetCursorPosition(Width / 2 - text[((int)mainMenu)].Length, Height / 2 + ((int)mainMenu) * 2);
                WriteLineColorOnce(text[((int)mainMenu)], false, ConsoleColor.White);
                switch (Input)
                {
                    case 'W':
                    case 'w':
                        mainMenu = (int)mainMenu - 1 > (int)(E_MainMenu.lowerLimit) ? (E_MainMenu)((int)mainMenu - 1) : mainMenu;
                        break;
                    case 'S':
                    case 's':
                        mainMenu = (int)mainMenu + 1 < (int)(E_MainMenu.upperLimit) ? (E_MainMenu)((int)mainMenu + 1) : mainMenu;
                        break;
                    case (Char)ConsoleKey.Enter://当按下的是Enter时，改变场景状态逻辑，根据当前选择的选项，跳转进入对应的游戏
                        switch (mainMenu)
                        {
                            case E_MainMenu.startGame://这是正常进入游戏的选项
                                return e_GameScene = E_GameScene.mainGame;
                            case E_MainMenu.quitGame:
                                Console.WriteLine("\nExit(0)：退出成功");//这是正常退出的选项
                                Environment.Exit(0);//正常退出
                                break;
                            default:
                                Console.WriteLine("\nExit(1)：错误退出，选择到了不存在的选项");//选择到了不存在的选项时的情况
                                Environment.Exit(0);//错误退出
                                break;
                        }
                        break;
                }
                //通过循环，将玩家的当前选项高亮
                Console.SetCursorPosition(Width / 2 - text[((int)mainMenu)].Length, Height / 2 + ((int)mainMenu) * 2);
                WriteLineColorOnce(text[((int)mainMenu)], false);
            }
            //按键监听，将选项高亮
        }
        //—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线

        static void PrintingMap()//墙体打印
        {
            for (int Printing = 0; Printing < Windows.GetWindowsWidth() - 1; ++Printing)
            {
                if (Printing % 2 == 0)
                {
                    Console.SetCursorPosition(Printing, 0);
                    Console.Write("■");
                    Console.SetCursorPosition(Printing, Windows.GetWindowsHeight() - 1);//减1是因为打印是从第0行开始算第1行，如果不减一会溢出崩溃
                    Console.Write("■");
                    Console.SetCursorPosition(Printing, Windows.GetWindowsHeight() - (Windows.GetWindowsHeight() / 3));//除以三分之一，印刷场景
                    Console.Write("■");
                }
                if (Printing < Windows.GetWindowsHeight())
                {
                    Console.SetCursorPosition(0, Printing);
                    Console.Write("■");
                    Console.SetCursorPosition(Windows.GetWindowsWidth() - 2, Printing);//减2是因为中文字符占两格
                    Console.Write("■");
                }
            }
        }
        //—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线
        static int FightMap(bool firstPrint)//重载，true即是初次打印战斗地图，会记录关卡的进度，关卡坐标系;不输入则不会记录，但是会更新游戏内容
        {
            int record = 0;
            int PrintingWidth = 0;
            int PrintingHeight = 2;
            bool continuousPrinting = true;
            while (continuousPrinting)
            {
                #region 是否达到印刷页面上限？
                if (PrintingHeight > Windows.GetWindowsHeight() - (Windows.GetWindowsHeight() / 3) - 7)
                {
                    continuousPrinting = false;
                }
                #endregion
                ++PrintingHeight;
                #region 打印顺行的逻辑
                for (PrintingWidth = 6; PrintingWidth < Windows.GetWindowsWidth() - 6; PrintingWidth++)
                {
                    if (PrintingWidth % 2 == 0)
                    {
                        Console.SetCursorPosition(PrintingWidth, PrintingHeight);
                        Console.Write("□");
                        record++;
                    }
                }
                #endregion
                #region 打印右侧衔接
                ++PrintingHeight;
                Console.SetCursorPosition(PrintingWidth - 2, PrintingHeight);
                Console.Write("□");
                record++;
                #endregion
                ++PrintingHeight;
                #region 打印逆行的逻辑
                for (; PrintingWidth > 6; PrintingWidth--)
                {
                    if (PrintingWidth % 2 == 0)
                    {
                        Console.SetCursorPosition(PrintingWidth - 2, PrintingHeight);
                        Console.Write("□");
                        record++;
                    }
                }
                #endregion
                #region 打印左侧衔接
                ++PrintingHeight;
                Console.SetCursorPosition(PrintingWidth, PrintingHeight);
                Console.Write("□");
                record++;
                #endregion
            }
            int LevelData = record;//关卡的进度上限
            return LevelData;//关卡的进度上限
        }
        //—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线
        static void FightMap()//重载，true即是初次打印战斗地图，会记录关卡的进度，关卡坐标系;不写则不会记录，但是会更新游戏内容
        {
            int PrintingWidth = 0;
            int PrintingHeight = 2;
            bool continuousPrinting = true;
            while (continuousPrinting)
            {
                #region 是否达到印刷页面上限？
                if (PrintingHeight > Windows.GetWindowsHeight() - (Windows.GetWindowsHeight() / 3) - 7)
                {
                    continuousPrinting = false;
                }
                #endregion
                ++PrintingHeight;
                #region 打印顺行的逻辑
                for (PrintingWidth = 6; PrintingWidth < Windows.GetWindowsWidth() - 6; PrintingWidth++)
                {
                    if (PrintingWidth % 2 == 0)
                    {
                        Console.SetCursorPosition(PrintingWidth, PrintingHeight);
                        Console.Write("□");
                    }
                }
                #endregion
                #region 打印右侧衔接
                ++PrintingHeight;
                Console.SetCursorPosition(PrintingWidth - 2, PrintingHeight);
                Console.Write("□");
                #endregion
                ++PrintingHeight;
                #region 打印逆行的逻辑
                for (; PrintingWidth > 6; PrintingWidth--)
                {
                    if (PrintingWidth % 2 == 0)
                    {
                        Console.SetCursorPosition(PrintingWidth - 2, PrintingHeight);
                        Console.Write("□");
                    }
                }
                #endregion
                #region 打印左侧衔接
                ++PrintingHeight;
                Console.SetCursorPosition(PrintingWidth, PrintingHeight);
                Console.Write("□");
                #endregion
            }
        }
        //—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线
        static int[] CoordinateSystemConversion(int recordProgress)//输入关卡进度，输出显示坐标
        {
            int[] printCoordinates = new int[2];
            int record = 0;
            int PrintingWidth = 0;
            int PrintingHeight = 2;
            bool continuousPrinting = true;
            while (continuousPrinting)
            {
                #region 是否达到印刷页面上限？
                if (PrintingHeight > Windows.GetWindowsHeight() - (Windows.GetWindowsHeight() / 3) - 7)
                {
                    continuousPrinting = false;
                }
                #endregion
                ++PrintingHeight;
                #region 打印顺行的逻辑
                for (PrintingWidth = 6; PrintingWidth < Windows.GetWindowsWidth() - 6; PrintingWidth++)
                {
                    if (PrintingWidth % 2 == 0)
                    {
                        if (recordProgress == record)
                        {
                            printCoordinates[0] = PrintingWidth;
                            printCoordinates[1] = PrintingHeight;
                            return printCoordinates;
                        }
                        record++;
                    }
                }
                #endregion
                #region 打印右侧衔接
                ++PrintingHeight;
                if (recordProgress == record)
                {
                    printCoordinates[0] = PrintingWidth - 2;
                    printCoordinates[1] = PrintingHeight;
                    return printCoordinates;
                }
                record++;
                #endregion
                ++PrintingHeight;
                #region 打印逆行的逻辑
                for (; PrintingWidth > 6; PrintingWidth--)
                {
                    if (PrintingWidth % 2 == 0)
                    {
                        if (recordProgress == record)
                        {
                            printCoordinates[0] = PrintingWidth - 2;
                            printCoordinates[1] = PrintingHeight;
                            return printCoordinates;
                        }
                        record++;
                    }
                }
                #endregion
                #region 打印左侧衔接
                ++PrintingHeight;
                if (recordProgress == record)
                {
                    printCoordinates[0] = PrintingWidth;
                    printCoordinates[1] = PrintingHeight;
                    return printCoordinates;
                }
                record++;
                #endregion
            }
            return printCoordinates;
        }
        //—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线
        static void PrintingRules()//印刷游戏规则
        {
            string[] PlayerRules =
                {
                "★表示人类阵营的星舰:",
                "战机根据每点骰子移动两格",
                "骰子点数为六时，发动空空导弹攻击前方",
                "空空导弹：击落的敌人两回合将无法移动",
                "俯冲轰炸：踩到击落状态敌人可直接获胜",
                "☆表示人类阵营的基地，进入可补给",
                "补给：立刻多骰一次骰子或发射空空导弹",
                };
            string[] AiRules =
                {
                "●表示异星阵营的飞碟:",
                "飞碟根据每点骰子移动两格",
                "如果骰子点数为三以下，移动加一格",
                "地球研究：踩到生物时，加一金币",
                "隐身外形：百分之五十概率摆脱导弹",
                "不稳定折跃：每回合随机前进或后退两格",
                "☆表示人类阵营的基地，可踩毁基地",
                };
            string[] interactiveProps =
                {
                "■表示双方位置重叠",
                "■表示终点，先到达终点即可胜利",
                "折,折跃门：踩到随机前进或后退六格",
                "妙,妙妙屋：被摔跤，后退两格",
                "卜,占卜师：随机获得一张卡牌",
                "商,商店：可以购买卡牌",
                "掠,太空掠食者：需击败后才能通过"
                };
            for (int i = 0; i < PlayerRules.Length; ++i)
            {
                Console.SetCursorPosition(2, Windows.GetWindowsHeight() - (Windows.GetWindowsHeight() / 3) + 1 + i);
                //2是因为字符占两格，加1是避免覆盖到了划分格子的方块，加i是每次+1行打印
                Console.Write(PlayerRules[i]);
            }
            for (int i = 0; i < AiRules.Length; ++i)
            {
                Console.SetCursorPosition(Windows.GetWindowsWidth() / 4, Windows.GetWindowsHeight() - (Windows.GetWindowsHeight() / 3) + 1 + i);
                //2是因为字符占两格，加1是避免覆盖到了划分格子的方块，加i是每次+1行打印
                Console.Write(AiRules[i]);
            }
            for (int i = 0; i < interactiveProps.Length; ++i)
            {
                Console.SetCursorPosition(Windows.GetWindowsWidth() / 2, Windows.GetWindowsHeight() - (Windows.GetWindowsHeight() / 3) + 1 + i);
                Console.Write(interactiveProps[i]);
            }
            Console.SetCursorPosition(2, Windows.GetWindowsHeight() - (Windows.GetWindowsHeight() / 3) + 1);
            WriteLineColorOnce("★", false);
            Console.SetCursorPosition(Windows.GetWindowsWidth() / 4, Windows.GetWindowsHeight() - (Windows.GetWindowsHeight() / 3) + 1);
            WriteLineColorOnce("●", false, ConsoleColor.DarkGreen);
            Console.SetCursorPosition(Windows.GetWindowsWidth() / 2, Windows.GetWindowsHeight() - (Windows.GetWindowsHeight() / 3) + 1);
            WriteLineColorOnce("★", false, ConsoleColor.DarkGreen);
            Console.SetCursorPosition(Windows.GetWindowsWidth() / 2, Windows.GetWindowsHeight() - (Windows.GetWindowsHeight() / 3) + 2);
            WriteLineColorOnce("■", false);
            Console.SetCursorPosition(Windows.GetWindowsWidth() / 2, Windows.GetWindowsHeight() - (Windows.GetWindowsHeight() / 3) + 3);
            WriteLineColorOnce("门", false, ConsoleColor.Blue);
            Console.SetCursorPosition(Windows.GetWindowsWidth() / 2, Windows.GetWindowsHeight() - (Windows.GetWindowsHeight() / 3) + 4);
            WriteLineColorOnce("妙", false, ConsoleColor.DarkRed);
            Console.SetCursorPosition(Windows.GetWindowsWidth() / 2, Windows.GetWindowsHeight() - (Windows.GetWindowsHeight() / 3) + 5);
            WriteLineColorOnce("卜", false, ConsoleColor.Magenta);
            Console.SetCursorPosition(Windows.GetWindowsWidth() / 2, Windows.GetWindowsHeight() - (Windows.GetWindowsHeight() / 3) + 4);
            WriteLineColorOnce("商", false, ConsoleColor.Yellow);
            Console.SetCursorPosition(Windows.GetWindowsWidth() / 2, Windows.GetWindowsHeight() - (Windows.GetWindowsHeight() / 3) + 5);
            WriteLineColorOnce("掠", false, ConsoleColor.Red);
        }
        //—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线
        static void PlotBroadcast()//开始的剧情播报，开发过程中为了不每次都看会跳过这个
        {
            string[] Plot =
                {
                "X零二三年四月十七日",
                "一场危机爆发",
                "世界摇摇欲坠",
                "你是被选中的人",
                "来拯救一切吧",
                "和邪恶的敌人来一场",
                "又惊险又刺激的",
                "太空飞行棋",
                "这场比赛的结果将决定",
                "人类的",
                "命运"
                };
            for (int i = 0; i < Plot.Length; ++i)
            {
                WriteLineColorOnce(Plot[i]);
                Thread.Sleep(1250);
                Console.Clear();
            }
        }
        //—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线—————— ฅ՞• •՞ฅ ——————华丽分割线
    }
}
