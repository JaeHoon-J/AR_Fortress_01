using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponParts : MonoBehaviour
{
    [SerializeField]
    protected Transform weaponUpDownTransform;//무기 부분에서 포신의 위아래를 위한 트랜스폼
    [SerializeField]
    protected Transform firePos1;//미사일 발사될 부분
    [SerializeField]
    protected Transform firePos2;//미사일 발사될 부분
    protected int firePosCnt;//총구 수
    [SerializeField]
    protected Projectile projectile;
    [SerializeField]
    protected DepthTarget depthTarget;

    public Transform ReturnTransform()
    {
        return weaponUpDownTransform;
    }
    public Transform ReturnFirePos1()
    {
        return firePos1;
    }
    public Transform ReturnFirePos2()
    {
        return firePos2;
    }
    public int ReturnFirePosCnt()
    {
        if (firePos2 == null)
        {
            return firePosCnt = 1;
        }
        else
        {
            return firePosCnt = 2;
        }
    }
    public void OnDepthMateiral()
    {
        projectile.enabled = true;
        depthTarget.enabled = true;
    }
}
