using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace foundation
{
    public interface ISceneManager:IEventDispatcher
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="isLoop"></param>
        /// <returns></returns>
        bool load(string sceneName);

        void clear();

        Camera getSceneCamera();

        Scene getCurrentActiveScene();

        bool isReady { get; }

        void sceneReadySync();

        Action<GameObject> checkSceneGameObject { get; set; }

        string currentSceneName { get; }
        void setCenterPosition(Vector3 v);

        float getNavHeight(Vector3 position);

        bool getNearNavPosition(Vector3 position,out Vector3 nearPosition);

        List<Vector3> getNavPathList(Vector3 from, Vector3 to,bool useWaypoint = false);

        Vector3 bornPosition { get; set; }

        Vector3 bornRotation { get; set; }

        void routerMapElement(MonoCFG elementCfg);
    }
}