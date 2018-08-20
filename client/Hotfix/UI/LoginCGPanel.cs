using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Hotfix
{
    public class LoginCGPanel : Frame
    {
        //CG字幕
        public Text StoryTxt;
        public Button endStory;
        private Image cgPic;
        Sprite pic1;
        Sprite pic2;
        Sprite pic3;
        Sprite pic4;
        Sprite pic5;
        Sprite pic6;
        Sprite pic7;
        Sprite pic8;
        Sprite pic9;
        Sprite pic10;

        //字幕行数  
        int lineCount;
        Task taskDelay;
        CancellationTokenSource cancellationTokenSource;      //定义中止一个字段，用来表示这是中止线程的参数，以备在show（界面显现时）时进行初始化（new），发出取消信号
        public override void Init()                 //采用override重载了基类的初始化信息，这里的Init就相当于Awake和Start，当游戏运行时，通过主程序对所有继承了Frame的子类的Init其中的信息，进行唯一的一次加载，当然，如果本对象在内存中被指定销毁，下次它被创建时，会再次加载Init中的内容
        {
            base.Init();       //继承基类预装的信息
            ReferenceCollector rc = panel.GetComponent<ReferenceCollector>();
            StoryTxt = rc.Get<GameObject>("StoryTxt").GetComponent<Text>();
            endStory = rc.Get<GameObject>("endStory").GetComponent<Button>();
            cgPic = rc.Get<GameObject>("cgPic").GetComponent<Image>();
            pic1 = Resources.Load("boliCG/1", typeof(Sprite)) as Sprite;
            pic2 = Resources.Load("boliCG/2", typeof(Sprite)) as Sprite;
            pic3 = Resources.Load("boliCG/3", typeof(Sprite)) as Sprite;
            pic4 = Resources.Load("boliCG/4", typeof(Sprite)) as Sprite;
            pic5 = Resources.Load("boliCG/5", typeof(Sprite)) as Sprite;
            pic6 = Resources.Load("boliCG/6", typeof(Sprite)) as Sprite;
            pic7 = Resources.Load("boliCG/7", typeof(Sprite)) as Sprite;
            pic8 = Resources.Load("boliCG/8", typeof(Sprite)) as Sprite;
            pic9 = Resources.Load("boliCG/9", typeof(Sprite)) as Sprite;
            pic10 = Resources.Load("boliCG/10", typeof(Sprite)) as Sprite;

            endStory.onClick.AddListener(EndStoryCG);

        }

        public override void Hide()    //override重载基类Frame已有的方法Hide，在里面添加因为隐藏UI，需要异常中断其中运行的某线程的处理逻辑
        {
            base.Hide();
            if (taskDelay.Status == TaskStatus.Running)       //taskDelay.Status 这里我们使用了taskDelay，如果它的状态（Status）还在运行（TaskStatus.Running）
            {
                cancellationTokenSource.Cancel();             //则向应该被终止的线程发出取消（Cancel）信号，CancellationTokenSource类就是这个意思，它只能控制被打上关联标记（cancellationTokenSource.Token）的对象
            }
        }
        public override void Destroy()                       //如果某人想把本对象销毁，则也对基类作加强处理，需要向里面正在运行的异步线程发出取消信号
        {
            base.Destroy();
            if (taskDelay.Status == TaskStatus.Running)
                cancellationTokenSource.Cancel();
        }
        async void PlayStoryTxt()    //async是个表示其内允许执行异步线程的关键字，“允许”表示可能没有，也可能会有，但不一定是全部，比如下面的代码中，只有Task是真正的开启异步线程的方法
        {
            var ct = cancellationTokenSource.Token;      //定义标记，凡是被打上此标记，表示就是一个“应被取消的线程”
            try
            {
                string mPath = "Config/loginBoliStory";  //定义路径，这里表示是放在根目录下的文件
                TextAsset storyText = Resources.Load<TextAsset>(mPath);
                //使用Resources.Load读取一个路径的文件，获得此文件
                //StreamReader sr = new StreamReader(mPath);   //同样的路径，采用StreamReader(path) 会有问题，因为StreamReader(path)首先默认从client根目录直接找文件，
                //如找不到，则它会转义路径，成为奇怪的路径，如果是本地文件，需要在路径前加上@以固化路径，如@"C:\folder\文件名.txt"
                //如果上一行正确，则可以采用ReadToEnd
                //sr.ReadToEnd();     //把sr从头读到尾，使用一个字符串strAll进行接收，其中的换行会被自动转为"\n"
                //下面继续Resources的方法，把读进来的文件的文本取出来
                string strAll = storyText.text;
                //把字符串strAll采用Split关键字，按照'\n'标记进行分分割，成为str数组内的元数
                string[] str = strAll.Split('\n');
                //sr.Close();   //如果采用的是StreamReader，需要把文件进行关闭。但Resources.Load则不需要如此处理，它会在场景转换之后自动释放
                for (int i = 0; i < str.Length; i++)
                {
                    StoryTxt.text = str[i];
                    int j = str[i].Length;
                    switch (i)
                    {
                        case 0:
                            cgPic.sprite = pic1;
                            cgPic.gameObject.SetActive(true);
                            break;
                        case 2:
                            cgPic.sprite = pic2;
                            break;
                        case 3:
                            cgPic.sprite = pic3;
                            break;
                        case 5:
                            cgPic.sprite = pic4;
                            break;
                        case 6:
                            cgPic.sprite = pic5;
                            break;
                        case 7:
                            cgPic.sprite = pic6;
                            break;
                        case 9:
                            cgPic.sprite = pic7;
                            break;
                        case 13:
                            cgPic.sprite = pic8;
                            break;
                        case 17:
                            cgPic.sprite = pic9;
                            break;
                    }
                    taskDelay = Task.Delay(j * 150 + 1500, ct);//很不幸，Task.Delay(,ct)被打上了标记“ct”
                    await taskDelay;
                    if (State != FrameState.Show)
                    {
                        Debug.Log("exit PlayStoryTxt");
                        return;
                    }
                }
                await Task.Delay(1000, ct);  //再次采用了异步线程，因此再次给它打上标记
                if (State != FrameState.Show)
                {
                    Debug.Log("exit PlayStoryTxt");
                    return;
                }

                EndStoryCG();
            }
            catch (Exception e)   //抛出异常信息
            {
                if (taskDelay.IsCompleted == false)
                {
                    Debug.Log("exit PlayStoryTxt");         //这里是玩家因为主动操作（if已判断），导致 IsCanceled为true，所以它其实是“正常性抛出”
                }
                else
                {
                    Debug.Log(e.ToString());         //真正的异常
                }
            }
        }

        void EndStoryCG()
        {
            Hide();               //调用本地方法，关闭了异步线程
                                  //UIManager.Instance.HideFrame(FrameType.LoginCGPanel);  //隐藏CG界面
            UIManager.Instance.Show(FrameType.LoginPanel, null, ShowLayer.Layer_3);     //显示LoginPanel界面

        }
        public override void Show(System.Object[] arg)   //这里的Show，是表示本对象（GameObject）在显示时，其中的内容就会被再次执行
        {
            base.Show(arg);   //先继承基类展示对像的方法Show
            PlayerPrefs.SetInt("BoliStoryPlayNum", 1);
            //SoundManager.Instance.PlaySound(SoundCategory.BGM, "BGM_MAIN");//播放背景音乐
            cancellationTokenSource = new CancellationTokenSource();  //初始化中止线程类的新实例
            PlayStoryTxt();
        }
    }
}

