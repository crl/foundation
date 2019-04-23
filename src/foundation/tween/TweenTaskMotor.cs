using UnityEngine;

namespace foundation
{
    public class TweenTaskMotor : MonoBehaviour
    {
        [SerializeField]
        private bool paused;
        [SerializeField]
        private bool _useFixedUpdate;

        [SerializeField]
        private RFTweenerTask firstNode;
        private RFTweenerTask lastNode;

        public bool addTask(RFTweenerTask t,float delay=-1,EaseAction ease=null)
        {
            if (lastNode != null)
            {
                lastNode.next = t;
                t.prev = lastNode;
                lastNode = t;
            }
            else
            {
                firstNode = lastNode = t;
            }

            if (delay > 0)
            {
                t.delay = delay;
            }
            if (ease != null)
            {
                t.ease = ease;
            }
            t.reset();
            return true;
        }

        public void pause()
        {
            this.paused = true;
        }

        public void play()
        {
            this.paused = false;
        }

        public void stopAll()
        {
            RFTweenerTask t = firstNode;
            while (t != null)
            {
                t.stop();
                t = t.next;
            }
        }

        public void stopTask(RFTweenerTask animationTask)
        {
            RFTweenerTask t =firstNode;
            while (t!=null)
            {
                if (t == animationTask)
                {
                    animationTask.stop();
                }
                t = t.next;
            }
        }

        protected void FixedUpdate()
        {
            if (this._useFixedUpdate)
            {
                this.tick(Time.time);
            }
        }
        protected void Update()
        {
            if (!this._useFixedUpdate)
            {
                this.tick(Time.time);
            }
        }

        private void tick(float now)
        {
            if (this.paused)
            {
                return;
            }

            RFTweenerTask t = firstNode;
            RFTweenerTask n;
            while (t!=null)
            {
                n = t.next;
                if (t.state == NodeActiveState.ToDoDelete)
                {
                    if (t.prev != null)
                    {
                        t.prev.next = t.next;
                    }
                    else
                    {
                        firstNode = t.next;
                    }

                    if (t.next != null)
                    {
                        t.next.prev = t.prev;
                    }
                    else
                    {
                        lastNode = t.prev;
                    }
                    t.prev = null;
                    t.next = null;
                }else
                {
                    t.tick(now);
                }
                t = n;
            }
        }

        public bool hasTasks
        {
            get
            {
                return (this.firstNode!=null);
            }
        }

        public bool useFixedUpdate
        {
            get
            {
                return this._useFixedUpdate;
            }
            set
            {
                this._useFixedUpdate = value;
            }
        }
    }
}