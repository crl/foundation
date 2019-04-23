using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

using UnityEngine;

namespace foundation
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Lingyu/AnimatorClipRef")]
    public class AnimatorClipRef : MonoBehaviour
    {
        public AnimationClip[] animationClips=new AnimationClip[0];
        public AnimationClip[] placeholderClips = new AnimationClip[0];

        public RuntimeAnimatorController controller;
        public bool TryGetValue(string name, out AnimationClip clip)
        {
            for (int i = 0,len= animationClips.Length; i < len; i++)
            {
                AnimationClip temp = animationClips[i];
                if (temp && temp.name == name)
                {
                    clip = temp;
                    return true;
                }
            }

            for (int i = 0, len = placeholderClips.Length; i < len; i++)
            {
                AnimationClip temp = animationClips[i];
                if (temp && temp.name == name)
                {
                    clip = temp;
                    return true;
                }
            }

            clip = null;
            return false;
        }
        public static RuntimeAnimatorController GetEditorAnimatorController(AnimatorClipRef animatorClipRef,string prefix, string altName="")
        {
#if UNITY_EDITOR
            Dictionary<string, AnimationClip> dic = new Dictionary<string, AnimationClip>();
            AnimationClip defaultAnimationClip=null;
            string name;
            foreach (AnimationClip animationClip in animatorClipRef.animationClips)
            {
                if (animationClip)
                {
                    name = animationClip.name;
                    dic[name] = animationClip;
                    if (defaultAnimationClip == null || name.ToLower().IndexOf(IDLE_NAME) == 0)
                    {
                        defaultAnimationClip = animationClip;
                    }
                }
            }
            foreach (AnimationClip animationClip in animatorClipRef.placeholderClips)
            {
                if (animationClip && dic.ContainsKey(animationClip.name)==false)
                {
                    dic[animationClip.name] = animationClip;
                }
            }

            RuntimeAnimatorController t = animatorClipRef.controller;
            if (t != null)
            {
                altName = t.name;
            }

            if (string.IsNullOrEmpty(altName))
            {
                return null;
            }
            string fullName = prefix + altName + ".controller";
            UnityEditor.Animations.AnimatorController animatorController;
            if (EditorAnimatorControllerMap.TryGetValue(fullName, out animatorController))
            {
                return animatorController;
            }
            if (File.Exists(fullName) == false)
            {
                return null;
            }
            animatorController = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(fullName);
            if (animatorController == null)
            {
                return null;
            }
            DebugX.Log("AnimatorControllerAutoCreate name:" + altName);
            animatorController = Instantiate(animatorController);
            animatorController.hideFlags = HideFlags.DontSave;
            EditorAnimatorControllerMap.Add(fullName, animatorController);

            UnityEditor.Animations.AnimatorControllerLayer layer = animatorController.layers[0];
            DoAnimatorStateMachine(layer.stateMachine, defaultAnimationClip,dic);
            return animatorController;
#else
            return null;
#endif
        }
#if UNITY_EDITOR
        private static Dictionary<string, AnimatorController> EditorAnimatorControllerMap =
            new Dictionary<string, AnimatorController>();
        public static string IDLE_NAME = "idle";
        protected static void DoAnimatorStateMachine(UnityEditor.Animations.AnimatorStateMachine stateMachine, AnimationClip defaultAnimationClip,Dictionary<string, AnimationClip> dic)
        {
            if (stateMachine == null)
            {
                return;
            }

            foreach (ChildAnimatorState childAnimatorState in stateMachine.states)
            {
                string name = childAnimatorState.state.name;
                AnimationClip clip;
                if (dic.TryGetValue(name, out clip))
                {
                    childAnimatorState.state.motion = clip;
                }
                else
                {
                    childAnimatorState.state.motion = defaultAnimationClip;
                }
            }

            foreach (ChildAnimatorStateMachine childAnimatorStateMachine in stateMachine.stateMachines)
            {
                DoAnimatorStateMachine(childAnimatorStateMachine.stateMachine, defaultAnimationClip, dic);
            }
        }
#endif

    }
}