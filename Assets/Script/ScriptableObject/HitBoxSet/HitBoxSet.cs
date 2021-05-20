using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitBoxDatas
{
    Damage, Duration, Stun, CameraShakingTime, CameraShakingForce, HitStop
}
[CreateAssetMenu(fileName = "HitBox_", menuName = "Scriptable Object/HitBoxSet")]
public class HitBoxSet : ScriptableObject
{
    public float this[HitBoxDatas hitBox]
    {
        get
        {
            switch (hitBox)
            {
                case HitBoxDatas.Damage:
                    return _Damage;

                case HitBoxDatas.Duration:
                    return _Duration;

                case HitBoxDatas.Stun:
                    return _Stun;

                case HitBoxDatas.CameraShakingTime:
                    return _CameraShakingTime;

                case HitBoxDatas.CameraShakingForce:
                    return _CameraShakingForce;

                case HitBoxDatas.HitStop:
                    return _HitStop;
            }
            return 0;
        }
    }

    [Space(3)][Header("HitBox Property")]
    [SerializeField, Min(0)] private float _Damage;
    [SerializeField, Min(0)] private float _Duration;
    [SerializeField, Min(0)] private float _Stun;
    [SerializeField, Min(0)] private float _HitStop;

    [SerializeField] private Vector2 _PushForce;

    [Space(3)][Header("Camera Shaking Property")]
    [SerializeField, Min(0)] private float _CameraShakingTime;
    [SerializeField, Min(0)] private float _CameraShakingForce;

    [Space(3)][Header("Json Data Property")]
    [Obsolete] public string FileName;
    [Obsolete] public string ID;

    // ========== public property ========== //
    #region public property
    public float Damage 
    { get => _Damage; }
    public float Duration
    { get => _Duration; }
    public float Stun
    { get => _Stun; }
    public Vector2 PushForce
    { get => _PushForce; }
    public float CameraShakingTime
    { get => _CameraShakingTime; }
    public float CameraShakingForce
    { get => _CameraShakingForce; }
    public float HitStop
    { get => _HitStop; }
    #endregion
    // ========== public property ========== //

    [Obsolete]
    public void Set(float damage, float duration, float stun, Vector2 push, float shakingForce, float hitStop)
    {
        _Damage = damage;
        _Duration = duration;
        _Stun = stun;
        _PushForce = push;
        _CameraShakingForce = shakingForce;
        _CameraShakingTime = hitStop * 1.1f;
        _HitStop = hitStop;
    }
}
