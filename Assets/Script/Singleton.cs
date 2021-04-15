using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _Instance;
    public  static T  Instance
    {
        get
        {
            if (_Instance == null) {
                _Instance = FindObjectOfType<T>();
                
                if (_Instance == null) {
                    _Instance = new GameObject(typeof(T).Name, typeof(T)) as T;
                }
            }
            return _Instance;
        }
    }
}
