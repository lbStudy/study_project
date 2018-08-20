using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Data;

//常量定义的一些参数
namespace Hotfix
{
    public static class ConstDefine
    {
        public const float g = 10f;
        public const float inspectionTime = 0.5f;
        public static float shrink = 0.001f;
        public static int enlarge = 1000;
        public static float errorRange = 0.6f;
        public static float pingInterval = 5;
        public static float moveTime = 0.1f;              //惯性操控需要按住一个方向键的时间
        public static float intervalMoveTime = 5;         //惯性维持的时长
        public static bool isOpenDec = true;
        //public static Dictionary<CharacterState, string> stateAniNameDic = new Dictionary<CharacterState, string>() {
        //    {CharacterState.Idle, "f_idle"},
        //    {CharacterState.Move, "f_run"},
        //    //{CharacterState.XuLi, "f_storage"},
        //    {CharacterState.Jump, "f_dead01"},
        //    {CharacterState.Rebound, "n_wait2"},
        //    {CharacterState.Buffer, "f_keep"},
        //    {CharacterState.Hited, "f_hurt"},
        //    {CharacterState.Dead, "f_dead"},
        //    {CharacterState.Attack, "f_atk"},
        //    {CharacterState.Eat, "f_idle"},
        //};
        public const string BGMSoundPath = "Sound/";
        public const string UISoundPath = "Sound/";
        public const string BattleSoundPath = "Sound/";
        public const string hitPerFxName = "vfx_prefect_hit";
        public const string hitFxName = "vfx_normal_hit";
        public const string jumpdownFxName = "dilie";
        public const string ModelPath = "";
        public const string EffectPath = "";


        public const string ui_prefab_path = "Prefab/UI_ab_sg/";
    }

}
