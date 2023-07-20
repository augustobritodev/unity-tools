using UnityEngine;
using System;

public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour {
    public static T Instance {
        get {
            if (!isShutingDown) {
                
            }
            return instance;
        }
    }
    private static T instance;

    private Type type = typeof(T);

    private static bool isShutingDown = false;

    private void OnDestroy() {
        isShutingDown = true;
    }

    private void OnApplicationQuit() {
        isShutingDown = true;
    }
    
    protected virtual void Awake() {
        if (instance == null) {
            instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
        else {
            if (this != instance) {
                Debug.LogWarning("Multiple instances of this: " + type.Name);
                Destroy(this.gameObject);
            }
        }
    }
}
