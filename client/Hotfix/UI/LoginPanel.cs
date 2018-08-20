using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;

namespace Hotfix
{
    public class LoginPanel : Frame
    {
        InputField input_account;
        InputField input_password;

        private Button btn;
        private Button btn_ReplaceAccount;
        private Button btn_RepeatCG;

        //字符合法性检测前声明
        //纯数字检测
        private string num = "^[0-9]+$";
        //数字或英文
        private string numOrLetter = "^[A-Za-z0-9]+$";

        //纯汉字
        //private string chn = "^[\u4e00-\u9fa5]+$";

        public override void Init()
        {

            base.Init();
            ReferenceCollector rc = panel.GetComponent<ReferenceCollector>();
            input_account = rc.Get<GameObject>("input_account").GetComponent<InputField>();
            input_password = rc.Get<GameObject>("input_password").GetComponent<InputField>();
            btn = rc.Get<GameObject>("btn_login").GetComponent<Button>();
            btn_ReplaceAccount = rc.Get<GameObject>("btn_ReplaceAccount").GetComponent<Button>();
            btn_RepeatCG = rc.Get<GameObject>("btn_RepeatCG").GetComponent<Button>();


            btn.onClick.AddListener(loginOnClick);    //监听登陆按钮
            btn_ReplaceAccount.onClick.AddListener(clearPlayerAccount);  //监听更换帐号按钮
            btn_RepeatCG.onClick.AddListener(playCG);   //监听重新运行CG按钮



        }
        //登陆按纽点击事件，检测输入合法性

        void loginOnClick()
        {
            //以下是using System.Text.RegularExpressions;中的方法
            Regex regex1 = new Regex(num);
            Regex regex2 = new Regex(numOrLetter);
            if (!regex1.IsMatch(input_account.text))
            {
                Debug.Log("帐号不是数字");
                UIManager.Instance.Show(FrameType.HintPanel, new System.Object[] { "大王，您不能这样做！帐号必须是6至12个数字！" }, ShowLayer.Layer_5);
                return;
            }
            if (!regex2.IsMatch(input_password.text))
            {
                Debug.Log("密码不是数字或字母");
                UIManager.Instance.Show(FrameType.HintPanel, new System.Object[] { "大王，您不能这样做！密码必须是6至12个字母或数字！" }, ShowLayer.Layer_5);
                return;
            }
            //检测长度
            if (input_account.text.Length < 6 || input_account.text.Length > 12)
            {
                Debug.Log("帐号长度不对");
                UIManager.Instance.Show(FrameType.HintPanel, new System.Object[] { "大王，帐号只能是6至12个数字，请重新输入！" }, ShowLayer.Layer_5);
                return;
            }
            if (input_password.text.Length < 6 || input_password.text.Length > 12)
            {
                Debug.Log("密码长度不对");
                UIManager.Instance.Show(FrameType.HintPanel, new System.Object[] { "大王，密码只能是6至12个英文或数字，请重新输入！" }, ShowLayer.Layer_5);
                return;
            }
            //输入合法，把信息存在本地
            savePlayerAccount();
            //输入合法，检查联结
            //SoundManager.Instance.PlaySound(SoundCategory.UI, "button");   //通用按钮音效
                                                                           //if (Game.Instance.isConnect)
                                                                           //{
                                                                           //    Debug.Log("帐号密码都对，且联网状态把帐号和密码发给服务器");
                                                                           //    Game.Instance.Login(0, input_account.text, input_password.text);
                                                                           //}
                                                                           //else
                                                                           //{
                                                                           //    Debug.Log("未联网，直接无网络进入主界面");
                                                                           //    SceneSkip.Instance.GoScene(SceneType.Main);
                                                                           //}
        }
        //输入合法，把帐号和密码存在本地
        public void savePlayerAccount()
        {
            //存储信息  
            PlayerPrefs.SetString("pAccount", input_account.text);
            PlayerPrefs.SetString("pPassword", input_password.text);
        }
        //检测是否有本地帐号密码信息，如有，取之
        //删除信息      PlayerPrefs.DeleteKey("TestString"); 
        //删除所有      PlayerPrefs.DeleteAll();
        void findPlayerLocalInfo()
        {
            Regex regex3 = new Regex(num);
            Regex regex4 = new Regex(numOrLetter);
            //本地是否有帐号和密码
            if (PlayerPrefs.GetString("pAccount") != null)
            {
                input_account.text = PlayerPrefs.GetString("pAccount");
                input_password.text = PlayerPrefs.GetString("pPassword");
                //既然有帐号密码了，就不用再输入了
                input_account.gameObject.SetActive(false);
                input_password.gameObject.SetActive(false);
                btn_ReplaceAccount.gameObject.SetActive(true);
            }
            //判断是否运行过CG
            //if (PlayerPrefs.GetInt("BoliStoryPlayNum") != 1)
            //{
            //    playCG();
            //}
        }
        //清空本地数据，更换一个帐号
        void clearPlayerAccount()
        {
            input_account.gameObject.SetActive(true);
            input_password.gameObject.SetActive(true);

            btn_ReplaceAccount.gameObject.SetActive(false);
            //SoundManager.Instance.PlaySound(SoundCategory.UI, "button");   //通用按钮音效
        }

        //运行CG页面
        void playCG()
        {
            UIManager.Instance.HideFrame(FrameType.LoginPanel);  //隐藏自身
            UIManager.Instance.Show(FrameType.LoginCGPanel, null, ShowLayer.Layer_4); //显示CG界面  
            //SoundManager.Instance.PlaySound(SoundCategory.UI, "button");   //通用按钮音效
        }
        public override void Show(System.Object[] arg)
        {
            base.Show(arg);
            //进入游戏，即播放这首音乐，此时还可配动画CG        
            //SoundManager.Instance.PlaySound(SoundCategory.BGM, "BGM_JQ");//播放背景音乐
                                                                         //检测是否有本地帐号密码信息，以及CG播放信息
            findPlayerLocalInfo();
        }
    }
}

