using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class Aiming : MonoBehaviour
{
    //컨트롤러에서 위아래 값을 받아 x축으로 회전시킬것임
    //컨트롤러의 좌우 값을 받아 y축으로 회전시킬것임

    public float x; //x축기준으로 위아래 회전위한 값
    public float y;//y 축 기준 좌우 회전을 위한 값
    public bool isRotateUpDown = false; //위아래 버튼을 눌렀는지
    public bool isRotateLeftRight = false; //좌우 버튼을 눌렀는지
    public bool isControlPower = false;

    private void Start()
    {
        isRotateUpDown = false;
        isRotateLeftRight = false;
    }
    private void Update()
    {
        if(PhotonManager.Instance.isMaster == true)
        {
            if(isControlPower == true)
            {

            }

            if(isRotateUpDown == true)
            {
                //위아래 회전중일때는 회전중인 불값과 방향에 대한 정보를 포톤으로 넘겨준다
                PhotonManager.Instance.isRotateUpDown = true;
                PhotonManager.Instance.rotateX = x;
            }
            else
            {
                //회전중이 아니일때 불값을 동기화한다
                PhotonManager.Instance.isRotateUpDown = false;
            }

            if(isRotateLeftRight == true)
            {
                //좌우 회전중일때는 회전중인 불값과 방향에 대한 정보를 포톤으로 넘겨준다
                PhotonManager.Instance.isRotateLeftRight = true;
                PhotonManager.Instance.rotateY = y;
            }
            else
            {
                //회전중이 아닐때 불값을 동기화한다
                PhotonManager.Instance.isRotateLeftRight = false;
            }

            if(isControlPower == true)
            {
                //제어권 있는 마스터 클라이언트일때 게이지가 올라간다
                if((PhotonManager.Instance.lange <DataManager.Instance.weaponlange)&&(PhotonManager.Instance.isMaster == true))
                {
                    //게이지를 올린다
                PhotonManager.Instance.lange += 3*Time.deltaTime;
                }
            }
            else
            {

            }
        }

    }
    //private void Update()
    //{
    //    if(isRotateUpDown == true)
    //    {
    //        //방향에 따른 위아래 회전 포신돌리는 부븐
    //        cube.Rotate(Vector3.right * -x );
    //    }
    //    if(isRotateLeftRight==true)
    //    {
    //        //방향에 따른 좌우 회전 포탑 돌리는 부분
    //        cube.Rotate(Vector3.up * -y);
    //    }
    //}

}
