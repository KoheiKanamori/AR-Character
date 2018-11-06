using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touch : MonoBehaviour {
    private Animator animator;
    private int jumpcount = 0;
    private float second = 0;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        if (CharactorMove.kneeldown_flag == true)
        {
            second += Time.deltaTime;
        }

        //if (OnTouchDown())
        if(Input.GetMouseButtonDown(0))
        {
            if (CharactorMove.kneeldown_flag == true && second > 3)
            {           
                animator.SetTrigger("KneelUpTrigger");
                jumpcount = 0;
                second = 0;
                CharactorMove.kneeldown_flag = false;
            }
            else
            {

                if (CharactorMove.status == 0)
                {
                    if (jumpcount >= 5)
                    {
                        animator.SetTrigger("KneelDownTrigger");
                        CharactorMove.kneeldown_flag = true;
                        //jumpcount = 0;
                    }
                    else
                    {
                        animator.SetTrigger("StandToJumpTrigger");
                        jumpcount++;
                    }
                }
                else if (CharactorMove.status == 5)
                {
                    animator.SetTrigger("WalkToJumpTrigger");
                    jumpcount++;
                }
            }

            //Debug.Log("タップされました");
        }
    }

    //スマホ向け そのオブジェクトがタッチされていたらtrue（マルチタップ対応）
    bool OnTouchDown()
    {
        // タッチされているとき
        if (0 < Input.touchCount)
        {
            // タッチされている指の数だけ処理
            for (int i = 0; i < Input.touchCount; i++)
            {
                // タッチ情報をコピー
                //Touch t = Input.GetTouch(i);
                
                // タッチしたときかどうか
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    //タッチした位置からRayを飛ばす
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                    RaycastHit hit = new RaycastHit();
                    if (Physics.Raycast(ray, out hit))
                    {
                        //Rayを飛ばしてあたったオブジェクトが自分自身だったら
                        if (hit.collider.gameObject == this.gameObject)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false; //タッチされてなかったらfalse
    }

}
