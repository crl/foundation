using UnityEngine;

namespace foundation
{
    public interface ICanbeReplacBehaviour
    {

    }
    public class CanbeReplacBehaviour : MonoBehaviour, ICanbeReplacBehaviour
    {
        protected virtual void Awake()
        {
            AbstractApp.ReplacBehaviour(this);
        }
    }
}