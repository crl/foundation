using UnityEngine;

namespace foundation
{
    public static class AnimatorExtensions
    {
        public static void SetSafeTrigger(this Animator self,string name)
        {
            if (self.isActiveAndEnabled && self.runtimeAnimatorController != null)
            {
                self.SetTrigger(name);
            }
        }
        public static void SetSafeTrigger(this Animator self, int name)
        {
            if (self.isActiveAndEnabled && self.runtimeAnimatorController!=null)
            {
                self.SetTrigger(name);
            }
        }

        public static void SetSafeBool(this Animator self, string name, bool value)
        {
            if (self.isActiveAndEnabled && self.runtimeAnimatorController != null)
            {
                self.SetBool(name,value);
            }
        }
        public static void SetSafeBool(this Animator self, int name, bool value)
        {
            if (self.isActiveAndEnabled && self.runtimeAnimatorController != null)
            {
                self.SetBool(name, value);
            }
        }

        public static void SetSafeInteger(this Animator self, string name, int value)
        {
            if (self.isActiveAndEnabled && self.runtimeAnimatorController != null)
            {
                self.SetInteger(name, value);
            }
        }
        public static void SetSafeInteger(this Animator self, int name, int value)
        {
            if (self.isActiveAndEnabled && self.runtimeAnimatorController != null)
            {
                self.SetInteger(name, value);
            }
        }

        public static void SetSafeFloat(this Animator self, string name, float value)
        {
            if (self.isActiveAndEnabled && self.runtimeAnimatorController != null)
            {
                self.SetFloat(name, value);
            }
        }
        public static void SetSafeFloat(this Animator self, int name, float value)
        {
            if (self.isActiveAndEnabled && self.runtimeAnimatorController != null)
            {
                self.SetFloat(name, value);
            }
        }
    }
}