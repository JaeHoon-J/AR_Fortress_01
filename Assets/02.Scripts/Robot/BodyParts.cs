using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyParts : MonoBehaviour
{
    public Transform weaponPos;
    [SerializeField]
    protected Projectile projectile;
    [SerializeField]
    protected DepthTarget depthTarget;
    public void OnDepthMateiral()
    {
        projectile.enabled = true;
        depthTarget.enabled = true;
    }
}
