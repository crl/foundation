using UnityEngine;

namespace foundation
{
    public class FoundationBehaviour : MonoBehaviour
    {
        public bool isDebug = false;

        private bool _isDisposed = false;
        private bool _isDisposing = false;
        public bool isDisposed
        {
            get { return _isDisposed; }
        }

        public bool isDisposing
        {
            get { return _isDisposing; }
        }
        internal void OnDestroy()
        {
            _isDisposing = true;
            onDestroy();
            _isDisposed = true;
        }
        protected virtual void onDestroy()
        {
            
        }
    }
}