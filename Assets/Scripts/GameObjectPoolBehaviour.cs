using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Papae.UnitySDK.DesignPatterns
{
    public class GameObjectPoolBehaviour : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] GameObject _prefab;
        [SerializeField] Transform _parent;
        [SerializeField] int _poolLimit = 0;
        [SerializeField] bool _allowGrowth = false;
        [SerializeField] bool _preloadOnStart = false;
        [SerializeField] int _preloadAmount = 0;
        [Space(4)]
        [SerializeField] GameObjectPoolEvent _onRequestEvent = null;
        [SerializeField] GameObjectPoolEvent _onReleaseEvent = null;
        #endregion

        public GameObject Prefab
        {
            get { return _prefab; }
            set { _prefab = value; }
        }

        public Transform Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public int PoolLimit
        {
            set { _poolLimit = value; }
            get { return _poolLimit; }
        }

        public bool CanGrow
        {
            get { return _allowGrowth; }
            set { _allowGrowth = value; }
        }

        public int Size
        {
            get { return inactivePool.Count + activePool.Count; }
        }

        public int ActiveGameObjects
        {
            get { return Size - InactiveGameObjects; }
        }

        public int InactiveGameObjects
        {
            get { return inactivePool.Count; }
        }

        public UnityEvent<GameObject> OnRequestEvent
        {
            get { return _onRequestEvent; }
        }

        public UnityEvent<GameObject> OnReleaseEvent
        {
            get { return _onReleaseEvent; }
        }


        int id = 1;
        List<GameObject> activePool = new List<GameObject>();
        List<GameObject> inactivePool = new List<GameObject>();

        void Awake()
        {
            inactivePool.Clear();
            activePool.Clear();
        }

        void Start()
        {
            if (_preloadOnStart)
            {
                Preload(_preloadAmount);
            }
        }

        /// <summary>
        /// Initialise pool.
        /// </summary>
        /// <param name="amount">Amount.</param>
        void Preload(int amount)
        {
            // Initialize pooling with the initial size.
            for (int i = 0; i < amount; i++)
            {
                CreateInstance();
            }
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <returns>The instance.</returns>
        GameObject CreateInstance()
        {
            // Validation for prefab.
            if (_prefab == null)
            {
                Debug.LogError("There is no prefab defined!");
                return null;
            }

            GameObject clone = Instantiate(_prefab, _parent) as GameObject;
            clone.gameObject.name = _prefab.name + " (" + (id++) + ")";
            PoolableGameObject poolable = clone.GetComponent<PoolableGameObject>();
            if (poolable == null)
            {
                poolable = clone.AddComponent<PoolableGameObject>();
            }
            poolable.Pool = this;

            clone.gameObject.SetActive(false);
            inactivePool.Add(clone);

            return clone;
        }


        /// <summary>
        /// Request object, return a reference to one of its components.
        /// </summary>
        public C RequestComponent<C>() where C : Component
        {
            GameObject clone = RequestGameObject();
            return clone.GetComponent<C>();
        }

        /// <summary>
        /// Request object, but return a reference to one of its components.
        /// </summary>
        public IList<C> RequestComponents<C>(int count) where C : Component
        {
            return RequestGameObjects(count).Select(o => o.GetComponent<C>()).ToList();
        }

        /// <summary>
        /// Gets from pool.
        /// </summary>
        /// <returns>The from pool.</returns>
        public GameObject RequestGameObject(UnityAction<GameObject> callbackAction = null)
        {
            foreach (GameObject clone in inactivePool)
            {
                if (clone != null && !clone.gameObject.activeInHierarchy)
                {
                    return RemoveFromPool(clone, callbackAction);
                }
            }

            if (CanGrow || Size < PoolLimit)
            {
                return RemoveFromPool(CreateInstance(), callbackAction);
            }

            return null;
        }

        /// <summary>
        /// Returns a list of objects from the pool, creating any if the pool is
        /// exhausted.
        /// </summary>
        /// <param name="count">Number of objecs to return.</param>
        public IList<GameObject> RequestGameObjects(int count)
        {
            if (count <= 0) LogError("Pool count must be positive: " + count);

            return Enumerable
                .Repeat<GameObject>(null, count)
                .Select(o => RequestGameObject())
                .ToList();
        }


        /// <summary>
        /// Prepares the game object.
        /// </summary>
        /// <returns>The game object.</returns>
        /// <param name="obj">Object.</param>
        /// <param name="callbackAction">Callback action.</param>
        GameObject RemoveFromPool(GameObject obj, UnityAction<GameObject> callbackAction = null)
        {
            if (obj != null && inactivePool.Count > 0 && inactivePool.Contains(obj))
            {
                obj = PrepareGameObject(obj, null, _prefab, true);
                inactivePool.Remove(obj);
                activePool.Add(obj);
                TryInvokeUnityAction(callbackAction, obj);
                TryInvokeUnityEvent(OnRequestEvent, obj);
            }

            return obj;
        }

        /// <summary>
        /// Return objects to the object pool.
        /// </summary>
        public void ReturnToPool(IEnumerable<GameObject> gameObjects)
        {
            foreach (var go in gameObjects)
            {
                ReturnToPool(go);
            }
        }

        public void ReturnToPool(GameObject go)
        {
            AddToPool(go);
        }

        /// <summary>
        /// Returns to pool.
        /// </summary>
        /// <param name="obj">Reference.</param>
        void AddToPool(GameObject obj, UnityAction<GameObject> callbackAction = null)
        {
            if (obj != null)
            {
                obj = PrepareGameObject(obj, _parent, Vector3.zero, Quaternion.identity, Vector3.one, false);
                activePool.Remove(obj);
                inactivePool.Add(obj);
                TryInvokeUnityAction(callbackAction, obj);
                TryInvokeUnityEvent(OnReleaseEvent, obj);
            }
        }


        GameObject PrepareGameObject(GameObject copyObject, Transform parent, GameObject originalObject, bool active)
        {
            return PrepareGameObject(copyObject,
                parent,
                originalObject.transform.position,
                originalObject.transform.rotation,
                originalObject.transform.localScale,
                active);
        }

        /// <summary>
        /// Prepare game object to response.
        /// </summary>
        /// <returns>A object.</returns>
        /// <param name="obj">Object reference.</param>
        /// <param name="root">A Parent.</param>
        /// <param name="position">A Position.</param>
        /// <param name="rotation">A Rotation.</param>
        /// <param name="scale">Scale.</param>
        /// <param name="active">If set to <c>true</c> gameobject will become active.</param>
        GameObject PrepareGameObject(GameObject obj, Transform root, Vector3 position, Quaternion rotation, Vector3 scale, bool active)
        {
            if (obj != null)
            {
                obj.transform.parent = root;
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.transform.localScale = scale;
                obj.gameObject.SetActive(active);
            }

            return obj;
        }

        public void Clean()
        {
            foreach (GameObject clone in activePool)
            {
                if (clone != null)
                {
                    AddToPool(clone, null);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unityEvent"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryInvokeUnityEvent<T>(UnityEvent<T> unityEvent, T value)
        {
            if (unityEvent != null)
            {
                unityEvent.Invoke(value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callbackAction"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
		bool TryInvokeUnityAction(UnityAction<GameObject> callbackAction, GameObject obj)
        {
            if (callbackAction != null)
            {
                callbackAction.Invoke(obj);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        void LogError(object obj)
        {
#if UNITY_EDITOR
            Debug.LogError(obj.ToString());
#endif
        }
    }

    [Serializable]
    public class GameObjectPoolEvent : UnityEvent<GameObject>
    {

    }
}