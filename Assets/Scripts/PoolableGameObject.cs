using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Papae.UnitySDK.DesignPatterns
{
    /// <summary>
    /// Component for objects being used by <c>GameObjectPool</c>. Do not add
    /// this behaviour manually, it will be added to any <c>GameObject</c>
    /// instantiated by <c>GameObjectPool</c>.
    /// </summary>
    public class PoolableGameObject : MonoBehaviour
    {
        public GameObjectPoolBehaviour Pool;

        /// <summary>
        /// Return the <c>GameObject</c> to its pool. This method should be
        /// called instead of destroying the <c>GameObject</c>.
        /// </summary>
        public void ReturnToPool()
        {
            if (Pool == null)
            {
                LogError("Pool is null!");
                return;
            }

            Pool.ReturnToPool(gameObject);
        }

        bool hasQuit;

        /// <summary>
        /// 
        /// </summary>
        void OnApplicationQuit()
        {
            hasQuit = true;
        }

        /// <summary>
        /// Display a warning if an object with a PoolableGameObject component
        /// is destroyed, but not if the application is unplayed in Editor or
        /// exitted
        /// </summary>
        void OnDestroy()
        {
            // Apparently OnApplicationQuit does not get called when the
            // gameObject is not active. Hence the need for the extra condition
            if (!hasQuit && isActiveAndEnabled)
            {
                LogWarning(
                    "Do not destroy objects with a Poolable component - " +
                    "they should be removed from play with 'ReturnToPool()'!"
                );
            }
        }

        void LogError(string message)
        {
#if UNITY_EDITOR
            Debug.LogError(message);
#endif
        }

        void LogWarning(string message)
        {
#if UNITY_EDITOR
            Debug.LogWarning(message);
#endif
        }
    }
}