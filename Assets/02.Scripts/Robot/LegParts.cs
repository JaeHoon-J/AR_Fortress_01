using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegParts : MonoBehaviour
{
    public Transform bodyPos;
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
