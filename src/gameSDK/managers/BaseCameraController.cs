using System;
using System.Collections.Generic;
using foundation;
using UnityEngine;

namespace gameSDK
{
    public class BaseCameraController: AbstractCameraController
    {
        protected Transform _cameraTransform;
        public GameObject followObject;
        protected AbstractBaseObject _followBaseObject;
        /// <summary>
        /// 当没有followObject时 的shake摄像机;
        /// </summary>
        public Action<float,Vector3> nonFollowObjectRouterHitcamByFactor;

        public float rotationAvgSpeed = 10f;
        public float positionAvgSpeed = 10f;

        [HideInInspector] public bool enableChangeDistance = false;

        [Range(0,100)]
        public float distance = 6f;

        [HideInInspector]
        public float stepDistance = 0.5f;
        [HideInInspector]
        public float minDistance = 2f;
        [HideInInspector]
        public float maxDistance = 10;

        [Range(0, 50)]
        public float height = 6;
        [Range(0, 50)]
        public float heightLookAt=2;

        /// <summary>
        /// 美术调整好的地图原始角度
        /// </summary>
        public Vector3 originRotationAngle = Vector3.zero;

        public Vector3 rotationAngle = Vector3.zero;

        protected float toDistance = 6f;

        protected float touchScaleSpeed = 0.04f;
        protected float touchRotateSpeed = 2;

        /// <summary>
        /// 相机偏移
        /// </summary>
        public Vector3 offset = new Vector3(0f, 0f, 0f);

        public Vector3 cameraOffset= new Vector3(0f, 0f, 0f);
        public Vector3 offsetLookAt = new Vector3(0f, 0f, 0f);

        public Vector3 hitShakeVector = new Vector3(0f, 0.6f, 0.4f);
        protected Vector3 _shakeOffset = Vector3.zero;

        [HideInInspector]
        public bool isCameraFocusing=false;
        [HideInInspector]
        public bool isPauseSlerp = false;

        protected bool _isStartSlep=false;

        [HideInInspector]
        public float slepTime=0;
        [HideInInspector]
        public float elapseTime=0;

        private float _startSlepTime;
        private float _startResetSlepTime;

        /// <summary>
        /// 被跟随者 反向盯的位置
        /// </summary>
        protected Vector3 _followLookAt;
        private CameraFollowParam _defaultCameraFollowParam = null;
        private CameraFollowParam _toCameraParam;
        [SerializeField]
        private CameraFollowParam _fromCameraParam=new CameraFollowParam();

        public float resetTime = 1f;
        private bool _isNeedResetDistance=true;
        public BaseCameraController() { 
        }
        
        protected virtual void Start()
        {
            refreashCamera();
            if (_defaultCameraFollowParam == null)
            {
                toDistance = distance;
                _defaultCameraFollowParam = getCurrentFollowParam(_defaultCameraFollowParam);
            }

            this._isStartSlep = false;
        }


        public CameraFollowParam defaultCameraFollowParam
        {
            get { return _defaultCameraFollowParam; }
        }

        public virtual void refreashCamera()
        {
            if (AbstractApp.MainCamera == null)
            {
                return;
            }
            this._cameraTransform = AbstractApp.MainCamera.transform;
        }

        public virtual void setFollow(AbstractBaseObject value,bool forceChange=false)
        {
            if (value)
            {
                this._followBaseObject = value;
                this.followObject = value.gameObject;
            }
            else
            {
                this._followBaseObject = null;
                this.followObject = null;
            }
            if (forceChange)
            {
                updateTransform(1.0f, 1.0f);
            }

            AbstractApp.rectTriggerManager.refreashHero(value);
        }

        public virtual AbstractBaseObject getFollowObject()
        {
            return _followBaseObject;
        }

        /// <summary>
        /// 被跟随者 反向盯的位置
        /// </summary>
        /// <returns></returns>
        public virtual Vector3 getFollowLookAt()
        {
            return _followLookAt;
        }

        public virtual void resetDefault(CameraFollowParam cameraFollowParam=null,bool hasTween=true,float slepTime=1.0f)
        {
            if (cameraFollowParam != null)
            {
                _defaultCameraFollowParam = cameraFollowParam;
            }
            isCameraFocusing = false;
            this.toDistance = _defaultCameraFollowParam.distance;
            this.height = _defaultCameraFollowParam.height;
            this.heightLookAt = _defaultCameraFollowParam.heightLookAt;
            this.rotationAngle = _defaultCameraFollowParam.rotationAngle;
            this.offset = _defaultCameraFollowParam.offset;
            this.cameraOffset = _defaultCameraFollowParam.cameraOffset;
            this.offsetLookAt = _defaultCameraFollowParam.offsetLookAt;
            this._shakeOffset=Vector3.zero;
            _startResetSlepTime = Time.time;
            _isNeedResetDistance = true;

            if (hasTween == false)
            {
                this.distance = this.toDistance;
                updateTransform(1.0f, 1.0f);
            }
            else
            {
                this.slepTime = slepTime - this.elapseTime;
            }
        }
        public virtual void hitcamByFactor(float factor)
        {
            hitcamByFactor(factor, hitShakeVector);
        }
        public virtual void hitcamByFactor(float factor,Vector3 shakeVector)
        {
            if (this.isCameraFocusing)
            {
                return;
            }
            _shakeOffset = (shakeVector * factor);

            if (this.followObject == null)
            {
                //直接做掉
                if (nonFollowObjectRouterHitcamByFactor != null)
                {
                    nonFollowObjectRouterHitcamByFactor(factor,shakeVector);
                }
            }
        }

        protected virtual void LateUpdate()
        {
            if (this._cameraTransform == null)
            {
                return;
            }

            if (!this.followObject || this.isCameraFocusing)
            {
                TickManager.LateCameraUpdate();
                return;
            }

            if (this._isNeedResetDistance)
            {
                float deltaSlepTime = Time.time - this._startResetSlepTime;
                if (deltaSlepTime > this.resetTime)
                {
                    deltaSlepTime = this.resetTime;
                    this._isNeedResetDistance = false;
                }

                this.distance = Mathf.Lerp(this.distance, this.toDistance, deltaSlepTime / this.resetTime);
            }


            float t = Time.fixedDeltaTime;
            this.updateTransform(positionAvgSpeed * t, rotationAvgSpeed * t);
        }

        protected Vector3 lastPosition;
        public bool isLockRotateV=true;
        protected virtual void updateControl()
        {
            if (Input.GetMouseButtonDown(2))
            {
                lastPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(2))
            {
                float dalteX = Input.mousePosition.x - lastPosition.x;
                rotationAngle.y = Mathf.Repeat(rotationAngle.y + (dalteX / 20f * rotationAvgSpeed), 360);

                if (isLockRotateV==false)
                {
                    float dalteY = Input.mousePosition.y - lastPosition.y;
                    rotationAngle.x = rotationAngle.x - dalteY / 20f * rotationAvgSpeed;

                    limitAngle();
//                    if (rotationAngle.x > 80)
//                    {
//                        rotationAngle.x = 80;
//                    }
//                    else if (rotationAngle.x < 20)
//                    {
//                        rotationAngle.x = 20;
//                    }
                }
                lastPosition = Input.mousePosition;
                updateTransform(1.0f, 1.0f);
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
//                this.distance = Mathf.Max(this.distance - 1, 2);
                changeDistanceScale(-1);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
//                this.distance = Mathf.Min(this.distance + 1, 20);
                changeDistanceScale(1);
            }

            if (_isStartSlep)
            {
                limitAngle();
            }
        }

        protected Quaternion rotation=Quaternion.identity;
        protected Vector3 newPosition;
        protected Vector3 lookAtPosition;
        protected Vector3 calDistance;
        protected Vector3 calOffset;
        protected override void updateTransform(float positionAvg,float rotationAvg)
        {
            if (_cameraTransform == null || followObject==null)
            {
                return;
            }

            lookAtPosition = getLookatPosition();
            newPosition = getPosition();
 
            if (_shakeOffset!=Vector3.zero)
            {
                this._cameraTransform.position = newPosition + _shakeOffset;
            }
            else
            {
                newPosition = Vector3.Lerp(this._cameraTransform.position, newPosition, positionAvg);
                rotation = Quaternion.LookRotation(lookAtPosition, Vector3.up);

                this._cameraTransform.position = newPosition ;
                this._cameraTransform.rotation = Quaternion.Lerp(this._cameraTransform.rotation, rotation, rotationAvg);
            }

            TickManager.LateCameraUpdate();
        }

        protected virtual Vector3 getPosition()
        {
            if (_cameraTransform == null || followObject == null)
            {
                return Vector3.zero;
            }

            Vector3 pos = this.followObject.transform.position+this.offset - calDistance;
            _followLookAt = pos;

            pos.y = pos.y + height;
            pos = pos + calOffset;
            return pos;
        }

        protected virtual Vector3 getLookatPosition()
        {
            float step = 1.0f;
            if (BaseAppSetting.GetInstance().isFarView == false)
            {
                step = 0.8f;
            }

            Quaternion rotation = Quaternion.Euler(this.rotationAngle);
            calDistance = rotation * Vector3.forward * this.distance * step;
            calOffset = rotation * this.cameraOffset;
            Vector3 retV = calDistance - calOffset + (rotation * this.offsetLookAt);

            retV.y = retV.y - height + this.heightLookAt;

            return retV;
        }

        /// <summary>
        /// 调整视距
        /// </summary>
        /// <param name="value"></param>
        public virtual void changeDistance(float value)
        {
            if (_isStartSlep == true)
            {
                return;
            }
            if (enableChangeDistance == false) return;

            this.distance = value;
            limitAngle();
        }

        public virtual void changeDistanceScale(float deltaStep)
        {
            if (_isStartSlep == true)
            {
                return;
            }
            if (enableChangeDistance == false)
            {
                return;
            }
            float deltaScale = deltaStep * touchScaleSpeed;
            float newScale = 1 + deltaScale;
            float newDistance = distance * newScale;
            if (newDistance > maxDistance || newDistance < minDistance)
            {
                return;
            }
            distance = newDistance;
            height *= newScale;

        }

        public virtual void changeRotation(float deltaX, float deltaY)
        {
            if(deltaX != 0)
            {
                rotationAngle.y = Mathf.Repeat(rotationAngle.y + deltaX / 40 * rotationAvgSpeed, 360);
            }

            if (deltaY != 0)
            {
                rotationAngle.x -= deltaY / 40 * rotationAvgSpeed;
                limitAngle();
            }

            //updateTransform(1.0f, 1.0f);
        }

        protected virtual void limitAngle()
        {
            double angle = Mathf.Atan2(height, distance) * 180 / Math.PI;
            float minAngle = -(float)angle + 5;
            float maxAngle = 90 - (float)angle;
            if (rotationAngle.x >= maxAngle)
            {
                rotationAngle.x = maxAngle;
            }
            else if (rotationAngle.x < minAngle)
            {
                rotationAngle.x = minAngle;
            }
        }



        protected virtual void Update()
        {
            this.updateControl();
            if (this._isStartSlep==false)
            {
                return;
            }
            if (Mathf.Approximately(this.slepTime, 0f))
            {
                this._isStartSlep = false;
                this.distance = this._toCameraParam.distance;
                this.height = this._toCameraParam.height;
                this.heightLookAt = this._toCameraParam.heightLookAt;
                this.rotationAngle = this._toCameraParam.rotationAngle;
                this.offset = this._toCameraParam.offset;
                this.cameraOffset = this._toCameraParam.cameraOffset;
                this.offsetLookAt = this._toCameraParam.offsetLookAt;
                return;
            }

            if (this.isPauseSlerp)
            {
                return;
            }
            this.elapseTime = Time.time - this._startSlepTime;
            float avg = elapseTime / this.slepTime;
            if (avg >= 1f)
            {
                avg = 1f;
                this._isStartSlep = false;
            }
            this.distance = Mathf.Lerp(this._fromCameraParam.distance , this._toCameraParam.distance,avg);
            this.height = Mathf.Lerp(this._fromCameraParam.height , this._toCameraParam.height,avg);
            this.heightLookAt = Mathf.Lerp(this._fromCameraParam.heightLookAt, this._toCameraParam.heightLookAt, avg);

            this.rotationAngle.x = Mathf.LerpAngle(this.rotationAngle.x, this._toCameraParam.rotationAngle.x, avg);
            this.rotationAngle.y = Mathf.LerpAngle(this.rotationAngle.y, this._toCameraParam.rotationAngle.y, avg);
            this.rotationAngle.z = Mathf.LerpAngle(this.rotationAngle.z, this._toCameraParam.rotationAngle.z, avg);

            this.offset = Vector3.Lerp(this._fromCameraParam.offset, this._toCameraParam.offset, avg);
            this.cameraOffset = Vector3.Lerp(this._fromCameraParam.cameraOffset, this._toCameraParam.cameraOffset, avg);
            this.offsetLookAt = Vector3.Lerp(this._fromCameraParam.offsetLookAt, this._toCameraParam.offsetLookAt, avg);
        }

        public virtual void StartSlep(CameraFollowParam toCameraParam, float slepTime = 1f)
        {
            if (toCameraParam == null)
            {
                return;
            }
            DebugX.Log("===============StartSlep");
            getCurrentFollowParam(this._fromCameraParam);
            this._toCameraParam = toCameraParam;
            //if (this._isStartSlep == false)
            {
                this.elapseTime = 0f;
            }

            this.slepTime = slepTime - this.elapseTime;
            this._startSlepTime = Time.time;
            this._isStartSlep = true;
        }

        public virtual CameraFollowParam getCurrentFollowParam(CameraFollowParam cmeraFollowParam=null)
        {
            if (cmeraFollowParam == null)
            {
                cmeraFollowParam = new CameraFollowParam();
            }

            if (this._toCameraParam != null)
            {
                cmeraFollowParam.distance = _toCameraParam.distance;
                cmeraFollowParam.height = _toCameraParam.height;
                cmeraFollowParam.heightLookAt = _toCameraParam.heightLookAt;
                cmeraFollowParam.rotationAngle = _toCameraParam.rotationAngle;
                cmeraFollowParam.offset = _toCameraParam.offset;
                cmeraFollowParam.cameraOffset = _toCameraParam.offset;
                cmeraFollowParam.offsetLookAt = _toCameraParam.offsetLookAt;
            }
            else
            {
                cmeraFollowParam.distance = this.distance;
                cmeraFollowParam.height = this.height;
                cmeraFollowParam.heightLookAt = this.heightLookAt;
                cmeraFollowParam.rotationAngle = this.rotationAngle;
                cmeraFollowParam.offset = this.offset;
                cmeraFollowParam.cameraOffset = this.offset;
                cmeraFollowParam.offsetLookAt = this.offsetLookAt;
            }

            return cmeraFollowParam;
        }

        public virtual CameraFollowParam getFollowParam(Vector3 position)
        {
            CameraFollowParam cameraFollowParam = new CameraFollowParam();
            Vector3 followPosition = followObject.transform.position;
            int rotationAngle = (int)Angle_360(Vector3.forward, new Vector3(followPosition.x - position.x, 0f, followPosition.z - position.z), Vector3.up);
            cameraFollowParam.rotationAngle = new Vector3(0, rotationAngle, 0);
            float dis = Vector3.Distance(new Vector3(followPosition.x, 0f, followPosition.z), new Vector3(position.x, 0f, position.z));
            cameraFollowParam.distance = dis;
            cameraFollowParam.height = position.y - this.followObject.transform.position.y;
            return cameraFollowParam;
        }

        protected float Angle_360(Vector3 a, Vector3 b, Vector3 upAx)
        {
            float angle = Vector3.Angle(a, b);
            float sign = Mathf.Sign(Vector3.Dot(upAx, Vector3.Cross(a, b)));
            float result = angle*sign;
            return (result > 0f) ? result : (360f + result);
        }

        protected float maxRayCastDistance = 1000;
        public virtual bool raycast(Ray ray, out RaycastHit _raycastHit)
        {
            return Physics.Raycast(ray, out _raycastHit, maxRayCastDistance, Physics.DefaultRaycastLayers);
        }

        protected IRayHitReceiver _currentReceiver;
        protected GameObject _currentCollidingObject;
        protected RaycastHit _raycastHit = new RaycastHit();
        protected bool _hasRaycastHit = false;
        protected virtual bool handleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase)
        {
            if (touchPhase != TouchPhase.Began)
            {
                return false;
            }
            if (AbstractApp.IsPointerOverUI(touchPosition, touchFingerId) || AbstractApp.MainCamera==null)
            {
                return false;
            }
            _hasRaycastHit = false;
             //摄像机到点击位置的的射线  
             Ray ray = AbstractApp.MainCamera.ScreenPointToRay(touchPosition);
            if (raycast(ray, out _raycastHit))
            {
                IRayHitReceiver receiver = null;
                GameObject go = _raycastHit.collider.gameObject;
                if (go != null)
                {
                    Transform parent = go.transform;
                    while (parent)
                    {
                        receiver = parent.GetComponent<IRayHitReceiver>();
                        if (receiver != null)
                        {
                            break;
                        }
                        parent = parent.parent;
                    }
                    if (receiver != null)
                    {
                        //检测是否选择了自己
                        _hasRaycastHit= receiver.OnRayHit(_raycastHit);
                        if (_hasRaycastHit)
                        {
                            //尝试选择更多，包括自己周围的重叠的一些单位
                            if (receiver.TryRayHitMore(_raycastHit) == false)
                            {
                                //如果没有更多，那么就选择自己
                                receiver.OnRayHitSelf();
                            }
                        }
                    }
                    _currentCollidingObject = go;
                    _currentReceiver = receiver;
                    
                    handleTouch(_raycastHit, touchPhase);
                }
            }

            return _hasRaycastHit;
        }

        public virtual bool hasRaycastHit
        {
            get { return _hasRaycastHit; }
            set { _hasRaycastHit = value; }
        }

        protected virtual void handleTouch(RaycastHit raycastHit, TouchPhase touchPhase)
        {
        }

        public virtual GameObject currentCollidingObject
        {
            get { return _currentCollidingObject; }
        }
        public virtual IRayHitReceiver currentReceiver
        {
            get { return _currentReceiver; }
        }

        public virtual RaycastHit raycastHit
        {
            get { return _raycastHit; }
        }


    }
}