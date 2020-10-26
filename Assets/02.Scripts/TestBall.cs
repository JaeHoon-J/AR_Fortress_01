using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBall : MonoBehaviour
{
    public bool isFire = false;
    public Vector3 dir;//포신이 향하고 있는 방향
    public float speed;// 속도
    //public float angle;//각도
    public float gravity;
    public float vertical;
    public Vector3 nowPos;
    public Vector3 lastPos;
    [SerializeField]
    protected Rigidbody rb;
    //[Header("토네이도")]
    //public bool isTonado = false;
    //public GameObject tonado;
    //public Tonado tonadoinfo;
    public void Start()
    {

        gravity = 10*0.05f;
        nowPos = transform.position;
        lastPos = transform.position;
    }
    void Update()
    {
        //if (isTonado == false)
        //{
        //    //토네이토를 안 만났을때 움직음
           Paravola();
        //}
        //else
        //{
            //토네이도를 만났을떄 움직임
           // MoveTonado();
        //}
    }
    private void FixedUpdate()
    {
        //CheckPos();
    }
    public void Paravola()
    {
        vertical += gravity * Time.deltaTime;
        transform.position += dir * speed * Time.deltaTime;
        transform.position += Vector3.down * vertical * Time.deltaTime;
        CheckPos();
    }
    /// <summary>
    /// Fixed Update문에서 사용
    /// 탄의 진행 방향으로 탄의 머리 부분이 가게 하기 
    /// </summary>
    public void CheckPos()
    {
        nowPos = transform.position;//현재 프레임의 위치를 넣고
        //Debug.Log(nowPos);
        //Debug.Log(lastPos);
        //현재위치 - 이전위치로 방향 구하기
        Vector3 rot = nowPos - lastPos; 
        //Debug.Log(rot);
        //방향으로 윗부분이 바라보게 회전
        transform.up = rot;//회전시키고
        //현재위치를 이전 위치에 넣으면 다음 업데이트문 시작시에 nowPos는 움직인 방향으로 바뀌면서
        //방향을 구할수 있게된다.
        lastPos = nowPos;
    }
    public void OnTriggerEnter(Collider other)
    {
        //if(other.CompareTag("Ground"))
        //{
        //Destroy(this.gameObject);
        //}
        //if(other.CompareTag("Tonado"))
        //{
        //    //tonadoinfo = other.GetComponent<Tonado>();
        //    //isTonado = true;//토네이도를 만났다.
        //}
        rb.isKinematic = false;
        rb.useGravity = true;
    }
    public void nCollisionEnter(Collision collision)
    {
        rb.useGravity = true;
    }
    public void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Tonado"))
        {
            //tonado = other.gameObject;
        }
    }
    //탄이 회오리를 만나면?
    //현재 움직이는것을 멈춘다.
    //회전하면서 토네이도의 높이까지 올라간다.
    //토네이토의 움직임데로 움직인다.
    //토네이도의 중심을 기준으로 회전하면서 올라간다.
    //public void MoveTonado()
    //{
       
    //    //토네이도의 최고 높이까지 도달했는가?
    //    if(transform.position.y < tonadoinfo.tonadoHigh.position.y)
    //    {
    //        //이동
    //        transform.position += Vector3.up * tonadoinfo.tonadoSpeed * Time.deltaTime;
    //        transform.RotateAround(tonadoinfo.tonadoCenter.position, Vector3.down, 700 * Time.deltaTime);
    //    }
    //    else
    //    {
    //        //발사
    //        dir = tonado.GetComponent<Tonado>().tonadoEdge.transform.forward;
    //        speed = 15;
    //        //탄이 발사되면서 중력이 이미 가속되있어서 다시 위로 쏘면 이전에 누적된 중력때문에 위로 쏴도 올라가지 않는다.
    //        //다시 쏘는 순간 새로 수직값(중력값)적용되었던거를 0으로 해서 새로 쏘는것처럼 한다.
    //        vertical = 0;
    //        isTonado = false;
    //    }
    //}
}
