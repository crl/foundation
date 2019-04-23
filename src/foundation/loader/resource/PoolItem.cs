using System.Collections;
using UnityEngine;

namespace foundation
{
    public class PoolItem : MonoBehaviour
    {
        internal AssetResource manager;

        public bool isNew = true;
        public bool recycle(float recycleTime=0)
        {
            if (Application.isPlaying==false)
            {
                return false;
            }
            if (gameObject == null)
            {
                return false;
            }

            if (manager == null || AbstractApp.PoolContainer == null)
            {
                GameObject.Destroy(this.gameObject, recycleTime);
                return true;
            }

            gameObject.transform.SetParent(AbstractApp.PoolContainer.transform, true);
            gameObject.SetActive(false);
            if (recycleTime > 0)
            {
                StartCoroutine(laterRecycle(recycleTime));
            }
            else
            {
                manager.recycleToPool(this);
            }
            return true;
        }


        public void Dispose()
        {
            if (isDisposed == true)
            {
                return;
            }
            isDisposed = true;
            GameObject.Destroy(this.gameObject);
        }

        private bool isDisposed = false;
        protected virtual void OnDestroy()
        {
            isDisposed = true;
        }

        private IEnumerator laterRecycle(float recycleTime)
        {
            if (recycleTime > 0)
            {
                yield return new WaitForSeconds(recycleTime);
            }

            manager.recycleToPool(this);
        }
    }
}