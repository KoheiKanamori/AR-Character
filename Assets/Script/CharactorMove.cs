using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharactorMove : MonoBehaviour {

    public GameObject Unitychan;
    public GameObject bed;
    public GameObject walking_target_bed;
    public GameObject bed_on_position;
    private Animator animator;
    private const string isWalk = "isWalk";
    private bool walkflag = false;

    public static int status = 0; //状態変数 0:待機 1:ベッドに寝転ぶ 2:ベッドから降りる
    private bool first_status = true;
    private bool status_control_flag = false;
    private float count = 0;
    private float initialize_count = 0;
    private bool already_use_bed_flag = false;

    public static bool Tracked_bed_flag = false;
    public static bool Tracked_Unitychan_flag = false;

    private bool bed_arrive_flag = false;
    private bool sleep_flag = false;
    public static bool bed_not_move_flag = false;
    private bool on_bed_flag = false;

    private bool wakeup_flag = false;

    private CharacterController controller;
    private float distance = 0f;
    private Vector3 move_vector = Vector3.zero;
    private float walk_speed = 0.01f;

    private float second = 0f;

    public BezierCurve bedin_move;
    public BezierCurve wakeup_move;
    private float t = 0f;

    NavMeshAgent agent;
    public float range = 0.3f;
    private Vector3 before_position = Vector3.zero;

    public GameObject sofa;
    public GameObject walking_target_sofa;
    public GameObject sofa_on_position;
    public static bool Tracked_sofa_flag = false;
    public static bool sofa_not_move_flag = false;
    private bool sofa_arrive_flag = false;
    private bool sit_flag = false;
    private bool on_sofa_flag = false;
    private bool standup_flag = false;
    private bool already_use_sofa_flag = false;

    public static bool kneeldown_flag = false;

    // Use this for initialization
    void Start () {

        animator = Unitychan.GetComponent<Animator>();
        controller = Unitychan.GetComponent<CharacterController>();

        agent = Unitychan.GetComponent<NavMeshAgent>();

	}
	
	// Update is called once per frame
	void Update () {

        //Unityちゃんが3秒以上トラッキングされなくなったら初期化
        if (Tracked_Unitychan_flag == false)
        {
            initialize_count += Time.deltaTime;

            if (initialize_count > 3)
            {
                first_status = true;
                initialize_count = 0;
                count = 0;
                status_control_flag = false;
                status = 0;
                t = 0;

                animator.SetBool(isWalk, false);
                walkflag = false;

                bed_arrive_flag = false;
                if(sleep_flag == true)
                {
                    animator.SetTrigger("WakeUpTrigger");
                }
                if (sit_flag == true)
                {
                    animator.SetTrigger("StandUpTrigger");
                }

                sleep_flag = false;
                bed_not_move_flag = false;
                on_bed_flag = false;
                wakeup_flag = false;
                already_use_bed_flag = false;

                sofa_not_move_flag = false;
                sofa_arrive_flag = false;
                sit_flag = false;
                on_sofa_flag = false;
                standup_flag = false;
                already_use_sofa_flag = false;

            }

        }

        if (kneeldown_flag == false)
        {

            //シーン遷移
            if (status_control_flag == true)
            {
                if (Tracked_Unitychan_flag == true && Tracked_bed_flag == true && on_bed_flag == false && already_use_bed_flag == false)
                {
                    status = 1;
                    status_control_flag = false;
                    count = 0;
                }

                if (Tracked_Unitychan_flag == true && Tracked_bed_flag == true && on_bed_flag == true)
                {
                    status = 2;
                    status_control_flag = false;
                    count = 0;
                }

                if (Tracked_Unitychan_flag == true && Tracked_sofa_flag == true && on_sofa_flag == false && already_use_sofa_flag == false)
                {
                    status = 3;
                    status_control_flag = false;
                    count = 0;
                }

                if (Tracked_Unitychan_flag == true && Tracked_sofa_flag == true && on_sofa_flag == true)
                {
                    status = 4;
                    status_control_flag = false;
                    count = 0;
                }

                if (Tracked_Unitychan_flag == true && status == 0)
                {
                    status = 5;
                    status_control_flag = false;
                    count = 0;
                }

            }


            //ベッドに寝転ぶシーン
            if (status == 1)
            {

                if (Tracked_bed_flag == true && MoveOnFloor.bed_active_flag == true)
                {
                    distance = Vector3.Distance(Unitychan.transform.position, walking_target_bed.transform.position);

                    if (distance > 0.03f && bed_arrive_flag == false)
                    {
                        //move_vector = (walking_target_bed.transform.position - Unitychan.transform.position);
                        //move_vector = move_vector.normalized * 0.1f * Time.deltaTime;
                        //controller.Move(move_vector);
                        //Unitychan.transform.rotation = Quaternion.LookRotation(move_vector);
                        agent.destination = walking_target_bed.transform.position;


                        if (walkflag == false)
                        {
                            animator.SetBool(isWalk, true);
                            walkflag = true;
                        }

                    }
                    else
                    {
                        if (walkflag == true)
                        {
                            animator.SetBool(isWalk, false);
                            walkflag = false;
                            bed_arrive_flag = true;
                        }

                        if (bed_arrive_flag == true) //ベッドに到着
                        {
                            second += Time.deltaTime;
                            bed_not_move_flag = true;

                            if (second > 2f) //ベッドに寝転ぶ処理
                            {
                                if (sleep_flag == false)
                                {
                                    //ベッドの反対方向を向く
                                    Vector3 chara_look_vector;
                                    chara_look_vector = walking_target_bed.transform.position - bed.transform.position;
                                    Unitychan.transform.rotation = Quaternion.LookRotation(chara_look_vector);

                                    animator.SetTrigger("SleepTrigger");

                                    //Debug.Log("check");


                                    Vector3 bezier_first_position;
                                    Vector3 bezier_second_position;


                                    bezier_first_position.x = (float)(2 * walking_target_bed.transform.position.x + bed_on_position.transform.position.x) / 100.0f;
                                    bezier_first_position.y = bed_on_position.transform.position.y;
                                    bezier_first_position.z = (float)(2 * walking_target_bed.transform.position.z + bed_on_position.transform.position.z) / 100.0f;

                                    bezier_second_position.x = (float)(walking_target_bed.transform.position.x + 2 * bed_on_position.transform.position.x) / 100.0f;
                                    bezier_second_position.y = bed_on_position.transform.position.y + 0.02f;
                                    bezier_second_position.z = (float)(walking_target_bed.transform.position.z + 2 * bed_on_position.transform.position.z) / 100.0f;

                                    bedin_move = new BezierCurve(walking_target_bed.transform.position, bezier_first_position, bezier_second_position, bed_on_position.transform.position);
                                    sleep_flag = true;

                                    //Debug.Log(walking_target_bed.transform.position);
                                    //Debug.Log(bezier_first_position);
                                    //Debug.Log(bezier_second_position);
                                    //Debug.Log(bed_on_position);
                                }
                                else //上記処理が一回行われた後(sleep_flag == true)
                                {
                                    //ベッドの上まで違和感なく移動
                                    if (t < 1)
                                    {
                                        Vector3 vec = bedin_move.GetPointAtTime(t);
                                        Unitychan.transform.position = vec;
                                        Unitychan.transform.rotation = Quaternion.LookRotation(walking_target_bed.transform.position - bed.transform.position);
                                        //t += 0.02f;
                                        t += 1.5f * Time.deltaTime;
                                    }
                                    else
                                    {
                                        //ベッドの上に固定
                                        Unitychan.transform.position = bed_on_position.transform.position;
                                        Unitychan.transform.rotation = bed_on_position.transform.rotation;

                                        //ベッドに寝転んだら値をリセットして待機状態へ
                                        //bed_not_move_flag == trueのまま
                                        //if (Input.GetKey(KeyCode.Escape))
                                        if (second > 10)
                                        {
                                            bed_arrive_flag = false;
                                            walkflag = false;
                                            second = 0;
                                            sleep_flag = false;
                                            t = 0;

                                            //status = 0;
                                            status = 2;
                                            on_bed_flag = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (walkflag == true)
                    {
                        animator.SetBool(isWalk, false);
                        walkflag = false;
                    }
                }

            }

            //ベッドから降りるシーン
            else if (status == 2)
            {
                if (wakeup_flag == false)
                {

                    animator.SetTrigger("WakeUpTrigger");

                    //Debug.Log("check");


                    Vector3 bezier_first_position;
                    Vector3 bezier_second_position;


                    bezier_first_position.x = (float)(2 * walking_target_bed.transform.position.x + bed_on_position.transform.position.x) / 100.0f;
                    bezier_first_position.y = bed_on_position.transform.position.y;
                    bezier_first_position.z = (float)(2 * walking_target_bed.transform.position.z + bed_on_position.transform.position.z) / 100.0f;

                    bezier_second_position.x = (float)(walking_target_bed.transform.position.x + 2 * bed_on_position.transform.position.x) / 100.0f;
                    bezier_second_position.y = bed_on_position.transform.position.y + 0.02f;
                    bezier_second_position.z = (float)(walking_target_bed.transform.position.z + 2 * bed_on_position.transform.position.z) / 100.0f;

                    //逆の動きになるように引数を入れ替え
                    wakeup_move = new BezierCurve(bed_on_position.transform.position, bezier_second_position, bezier_first_position, walking_target_bed.transform.position);
                    wakeup_flag = true;

                    //Debug.Log(walking_target_bed.transform.position);
                    //Debug.Log(bezier_first_position);
                    //Debug.Log(bezier_second_position);
                    //Debug.Log(bed_on_position);
                }
                else //上記処理が一回行われた後(wakeup_flag == true)
                {
                    //ベッドの上まで違和感なく移動
                    if (t < 1)
                    {
                        Vector3 vec = wakeup_move.GetPointAtTime(t);
                        Unitychan.transform.position = vec;
                        Unitychan.transform.rotation = Quaternion.LookRotation(walking_target_bed.transform.position - bed.transform.position);
                        t += 1.5f * Time.deltaTime;
                    }
                    else
                    {
                        //ベッドに寝転んだら値をリセットして待機状態へ
                        //bed_not_move_flag == trueのまま

                        //ベッドの反対方向を向く
                        Vector3 chara_look_vector;
                        chara_look_vector = walking_target_bed.transform.position - bed.transform.position;
                        Unitychan.transform.rotation = Quaternion.LookRotation(chara_look_vector);

                        //before_position = Unitychan.transform.position;
                        wakeup_flag = false;
                        t = 0;

                        status = 0;
                        on_bed_flag = false;
                        bed_not_move_flag = false;
                        already_use_bed_flag = true;
                    }
                }
            }

            //ソファに座る
            if (status == 3)
            {

                if (Tracked_sofa_flag == true && MoveOnFloor.sofa_active_flag == true)
                {
                    distance = Vector3.Distance(Unitychan.transform.position, walking_target_sofa.transform.position);

                    if (distance > 0.03f && sofa_arrive_flag == false)
                    {
                        //move_vector = (walking_target_bed.transform.position - Unitychan.transform.position);
                        //move_vector = move_vector.normalized * 0.1f * Time.deltaTime;
                        //controller.Move(move_vector);
                        //Unitychan.transform.rotation = Quaternion.LookRotation(move_vector);
                        agent.destination = walking_target_sofa.transform.position;


                        if (walkflag == false)
                        {
                            animator.SetBool(isWalk, true);
                            walkflag = true;
                        }

                    }
                    else
                    {
                        if (walkflag == true)
                        {
                            animator.SetBool(isWalk, false);
                            walkflag = false;
                            sofa_arrive_flag = true;
                        }

                        if (sofa_arrive_flag == true) //ソファに到着
                        {
                            second += Time.deltaTime;
                            sofa_not_move_flag = true;

                            if (second > 2f) //ベッドに寝転ぶ処理
                            {
                                if (sit_flag == false)
                                {
                                    //ベッドの反対方向を向く
                                    Vector3 chara_look_vector;
                                    chara_look_vector = walking_target_sofa.transform.position - sofa.transform.position;
                                    Unitychan.transform.rotation = Quaternion.LookRotation(chara_look_vector);

                                    animator.SetTrigger("SitTrigger");

                                    //Debug.Log("check");


                                    Vector3 bezier_first_position;
                                    Vector3 bezier_second_position;


                                    bezier_first_position.x = (float)(2 * walking_target_sofa.transform.position.x + sofa_on_position.transform.position.x) / 100.0f;
                                    bezier_first_position.y = sofa_on_position.transform.position.y;
                                    bezier_first_position.z = (float)(2 * walking_target_sofa.transform.position.z + sofa_on_position.transform.position.z) / 100.0f;

                                    bezier_second_position.x = (float)(walking_target_sofa.transform.position.x + 2 * sofa_on_position.transform.position.x) / 100.0f;
                                    bezier_second_position.y = sofa_on_position.transform.position.y + 0.02f;
                                    bezier_second_position.z = (float)(walking_target_sofa.transform.position.z + 2 * sofa_on_position.transform.position.z) / 100.0f;

                                    bedin_move = new BezierCurve(walking_target_sofa.transform.position, bezier_first_position, bezier_second_position, sofa_on_position.transform.position);
                                    sit_flag = true;

                                    //Debug.Log(walking_target_bed.transform.position);
                                    //Debug.Log(bezier_first_position);
                                    //Debug.Log(bezier_second_position);
                                    //Debug.Log(bed_on_position);
                                }
                                else //上記処理が一回行われた後(sleep_flag == true)
                                {
                                    //ベッドの上まで違和感なく移動
                                    if (t < 1)
                                    {
                                        Vector3 vec = bedin_move.GetPointAtTime(t);
                                        Unitychan.transform.position = vec;
                                        Unitychan.transform.rotation = Quaternion.LookRotation(walking_target_sofa.transform.position - sofa.transform.position);
                                        //t += 0.02f;
                                        t += 2.5f * Time.deltaTime;

                                    }
                                    else
                                    {
                                        //ベッドの上に固定
                                        Unitychan.transform.position = sofa_on_position.transform.position;
                                        Unitychan.transform.rotation = sofa_on_position.transform.rotation;

                                        //ベッドに寝転んだら値をリセットして待機状態へ
                                        //bed_not_move_flag == trueのまま
                                        //if (Input.GetKey(KeyCode.Escape))

                                        if (second > 10)
                                        {
                                            sofa_arrive_flag = false;
                                            walkflag = false;
                                            second = 0;
                                            sit_flag = false;
                                            t = 0;

                                            //status = 0;
                                            status = 4;
                                            on_sofa_flag = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (walkflag == true)
                    {
                        animator.SetBool(isWalk, false);
                        walkflag = false;
                    }
                }

            }


            //ソファから降りるシーン
            else if (status == 4)
            {
                if (standup_flag == false)
                {

                    animator.SetTrigger("StandUpTrigger");

                    //Debug.Log("check");


                    Vector3 bezier_first_position;
                    Vector3 bezier_second_position;


                    bezier_first_position.x = (float)(2 * walking_target_sofa.transform.position.x + sofa_on_position.transform.position.x) / 100.0f;
                    bezier_first_position.y = sofa_on_position.transform.position.y;
                    bezier_first_position.z = (float)(2 * walking_target_sofa.transform.position.z + sofa_on_position.transform.position.z) / 100.0f;

                    bezier_second_position.x = (float)(walking_target_sofa.transform.position.x + 2 * sofa_on_position.transform.position.x) / 100.0f;
                    bezier_second_position.y = sofa_on_position.transform.position.y + 0.02f;
                    bezier_second_position.z = (float)(walking_target_sofa.transform.position.z + 2 * sofa_on_position.transform.position.z) / 100.0f;

                    //逆の動きになるように引数を入れ替え
                    wakeup_move = new BezierCurve(sofa_on_position.transform.position, bezier_second_position, bezier_first_position, walking_target_sofa.transform.position);
                    standup_flag = true;

                    //Debug.Log(walking_target_bed.transform.position);
                    //Debug.Log(bezier_first_position);
                    //Debug.Log(bezier_second_position);
                    //Debug.Log(bed_on_position);
                }
                else //上記処理が一回行われた後(wakeup_flag == true)
                {
                    //ベッドの上まで違和感なく移動
                    if (t < 1)
                    {
                        Vector3 vec = wakeup_move.GetPointAtTime(t);
                        Unitychan.transform.position = vec;
                        Unitychan.transform.rotation = Quaternion.LookRotation(walking_target_sofa.transform.position - sofa.transform.position);
                        t += 2.5f * Time.deltaTime;
                    }
                    else
                    {
                        //ベッドに寝転んだら値をリセットして待機状態へ
                        //bed_not_move_flag == trueのまま

                        //ベッドの反対方向を向く
                        Vector3 chara_look_vector;
                        chara_look_vector = walking_target_sofa.transform.position - sofa.transform.position;
                        Unitychan.transform.rotation = Quaternion.LookRotation(chara_look_vector);

                        //before_position = Unitychan.transform.position;
                        standup_flag = false;
                        t = 0;

                        status = 0;
                        on_sofa_flag = false;
                        sofa_not_move_flag = false;
                        already_use_sofa_flag = true;
                    }
                }
            }

            //ランダムウォーク
            if (status == 5)
            {
                second += Time.deltaTime;

                if (walkflag == false)
                {
                    var nextpoint = range * Random.insideUnitCircle;
                    agent.destination = agent.transform.position + new Vector3(nextpoint.x, 0, nextpoint.y);
                    animator.SetBool(isWalk, true);
                    walkflag = true;
                    //Debug.Log(Vector3.Distance(Unitychan.transform.position, agent.destination));
                }
                //Debug.Log(agent.remainingDistance);
                //終了条件
                if (Vector3.Distance(Unitychan.transform.position, agent.destination) < 0.01f || second > 10)
                {

                    animator.SetBool(isWalk, false);
                    walkflag = false;
                    status = 0;
                    second = 0;

                    //Debug.Log("end");

                }

            }



            //待機状態
            else
            {

                //トラッキング開始時はUnityちゃんを初期座標に固定
                if (first_status == true)
                {
                    Unitychan.transform.position = Vector3.zero;
                }

                count += Time.deltaTime;
                if (count > 5) //待機状態に入って5秒後に状態遷移可能
                {
                    status_control_flag = true;
                    count = 0;
                    first_status = false;

                }


            }
        }
	}
}
