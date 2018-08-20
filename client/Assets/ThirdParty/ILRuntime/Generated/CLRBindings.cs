using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {


        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            System_Object_Binding.Register(app);
            System_Reflection_Assembly_Binding.Register(app);
            System_Type_Binding.Register(app);
            System_Reflection_MemberInfo_Binding.Register(app);
            System_Activator_Binding.Register(app);
            UnityEngine_Debug_Binding.Register(app);
            Singleton_1_ConfigDataManager_Binding.Register(app);
            ConfigDataManager_Binding.Register(app);
            UnityEngine_Input_Binding.Register(app);
            System_Diagnostics_Stopwatch_Binding.Register(app);
            System_TimeSpan_Binding.Register(app);
            System_String_Binding.Register(app);
            System_Runtime_CompilerServices_AsyncVoidMethodBuilder_Binding.Register(app);
            Data_L2C_LoginResponse_Binding.Register(app);
            Data_C2L_EnterAreaRequest_Binding.Register(app);
            System_Collections_Generic_List_1_AreaInfo_Binding.Register(app);
            Data_AreaInfo_Binding.Register(app);
            Base_ClientNetwork_Binding.Register(app);
            Data_Gate2C_PlayerGateVerifyResponse_Binding.Register(app);
            Begin_Binding.Register(app);
            System_Threading_Tasks_Task_1_Boolean_Binding.Register(app);
            System_Runtime_CompilerServices_TaskAwaiter_1_Boolean_Binding.Register(app);
            Data_C2L_LoginRequest_Binding.Register(app);
            Data_L2C_EnterAreaResponse_Binding.Register(app);
            Data_C2Gate_PlayerGateVerifyRequest_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Action_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Action_1_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Int32_Action_1_ILTypeInstance_Binding.Register(app);
            System_WeakReference_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_List_1_WeakReference_Binding.Register(app);
            System_Collections_Generic_List_1_WeakReference_Binding.Register(app);
            System_IDisposable_Binding.Register(app);
            System_Action_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_List_1_GameObject_Binding.Register(app);
            UnityEngine_GameObject_Binding.Register(app);
            UnityEngine_Transform_Binding.Register(app);
            UnityEngine_Object_Binding.Register(app);
            System_Collections_Generic_List_1_Action_1_GameObject_Binding.Register(app);
            UnityEngine_Resources_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_String_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_List_1_ILTypeInstance_Binding.Register(app);
            UnityEngine_SceneManagement_SceneManager_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_String_Binding.Register(app);
            System_GC_Binding.Register(app);
            BehaviourSingleton_1_BundleManager_Binding.Register(app);
            BundleManager_Binding.Register(app);
            UnityEngine_AudioSource_Binding.Register(app);
            UnityEngine_Audio_AudioMixer_Binding.Register(app);
            UnityEngine_Events_UnityAction_1_AudioClip_Binding.Register(app);
            System_Collections_Generic_List_1_AudioSource_Binding.Register(app);
            UnityEngine_Vector3_Binding.Register(app);
            UnityEngine_Component_Binding.Register(app);
            UnityEngine_Audio_AudioMixerGroup_Binding.Register(app);
            UnityEngine_AudioClip_Binding.Register(app);
            System_Threading_Tasks_Task_Binding.Register(app);
            System_Runtime_CompilerServices_TaskAwaiter_Binding.Register(app);
            System_NotImplementedException_Binding.Register(app);
            UnityEngine_UI_Text_Binding.Register(app);
            UnityEngine_Animator_Binding.Register(app);
            UnityEngine_RuntimeAnimatorController_Binding.Register(app);
            UnityEngine_AnimationClip_Binding.Register(app);
            ReferenceCollector_Binding.Register(app);
            System_Collections_Generic_Queue_1_String_Binding.Register(app);
            UnityEngine_Time_Binding.Register(app);
            UnityEngine_Quaternion_Binding.Register(app);
            UnityEngine_RectTransform_Binding.Register(app);
            UnityEngine_Rect_Binding.Register(app);
            UnityEngine_Vector2_Binding.Register(app);
            System_Collections_Generic_List_1_Text_Binding.Register(app);
            UnityEngine_UI_Button_Binding.Register(app);
            UnityEngine_Events_UnityEvent_Binding.Register(app);
            System_Threading_CancellationTokenSource_Binding.Register(app);
            UnityEngine_PlayerPrefs_Binding.Register(app);
            UnityEngine_TextAsset_Binding.Register(app);
            System_Char_Binding.Register(app);
            UnityEngine_UI_Image_Binding.Register(app);
            System_Text_RegularExpressions_Regex_Binding.Register(app);
            UnityEngine_UI_InputField_Binding.Register(app);
            System_Action_Binding.Register(app);
            System_Collections_Generic_List_1_Transform_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_ILTypeInstance_Binding_ValueCollection_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_ILTypeInstance_Binding_ValueCollection_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_List_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Int32_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_List_1_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Enum_Binding.Register(app);
            UIListenEvent_Binding.Register(app);
            UnityEngine_EventSystems_PointerEventData_Binding.Register(app);

            ILRuntime.CLR.TypeSystem.CLRType __clrType = null;
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
        }
    }
}
