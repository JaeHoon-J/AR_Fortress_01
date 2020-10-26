using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 캐릭터를 움직이는 클래스
/// </summary>
public class VirtualJoystick : MonoBehaviour
{

    // 공개
    public Transform Stick;         // 조이스틱.

    // 비공개
    private Vector3 StickFirstPos;  // 조이스틱의 처음 위치.
    public Vector3 JoyVec;         // 조이스틱의 벡터(방향)
    private float Radius;           // 조이스틱 배경의 반 지름.

    //public Transform cube;

    void Start()
    {
        Radius = GetComponent<RectTransform>().sizeDelta.y * 0.5f;
        StickFirstPos = Stick.transform.position;

        // 캔버스 크기에대한 반지름 조절.
        float Can = transform.parent.GetComponent<RectTransform>().localScale.x;
        Radius *= Can;
    }

    // 드래그
    public void Drag(BaseEventData _Data)
    {
        PointerEventData Data = _Data as PointerEventData;
        Vector3 Pos = Data.position;

        // 조이스틱을 이동시킬 방향을 구함.(오른쪽,왼쪽,위,아래)
        JoyVec = (Pos - StickFirstPos).normalized;

        // 조이스틱의 처음 위치와 현재 내가 터치하고있는 위치의 거리를 구한다.
        float Dis = Vector3.Distance(Pos, StickFirstPos);

        // 거리가 반지름보다 작으면 조이스틱을 현재 터치하고 있는곳으로 이동. 
        if (Dis < Radius)
            Stick.position = StickFirstPos + JoyVec * Dis;
        // 거리가 반지름보다 커지면 조이스틱을 반지름의 크기만큼만 이동.
        else
            Stick.position = StickFirstPos + JoyVec * Radius;

        if (PhotonManager.Instance.isMaster == true) //제어권이 있는 방장일때만 컨트롤러의 값을 동기화시킨다
        {
            PhotonManager.Instance.isInput = true;
            PhotonManager.Instance.z = JoyVec.y;//전방 움직임값
            PhotonManager.Instance.x = JoyVec.x;//좌우 움직임값
        }
        else
        {
            Debug.Log("턴이 아님");
        }
    }

    // 드래그 끝.
    public void DragEnd()
    {
        Stick.position = StickFirstPos; // 스틱을 원래의 위치로.
        if(PhotonManager.Instance.isMaster==true)//제어권 있는 방장이면 스틱놓았을때 값을 동기화한다
        {
            JoyVec = Vector3.zero;          // 방향을 0으로.
            PhotonManager.Instance.z = JoyVec.y;//전방 움직임값
            PhotonManager.Instance.x = JoyVec.x;//좌우 움직임값
            PhotonManager.Instance.isInput =false;
        }
    }

    //void Update()
    //{
    //    print(JoyVec.x + ", " + JoyVec.y);

    //    if (JoyVec.x != 0)
    //    {
    //        if(JoyVec.y>0)
    //        {
    //        cube.Rotate(Vector3.up * JoyVec.x);
    //        }
    //        if(JoyVec.y<0)
    //        {
    //            cube.Rotate(Vector3.up * -JoyVec.x);
    //        }
    //    }

    //    if (JoyVec.y != 0)
    //    {
    //        Vector3 dir = cube.forward * JoyVec.y;
    //        dir.Normalize();
    //        cube.position += dir * 0.5f * Time.deltaTime;
    //    }
    //}
}




