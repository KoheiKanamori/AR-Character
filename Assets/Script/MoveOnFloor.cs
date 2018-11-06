using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnFloor : MonoBehaviour {

    public GameObject Image_bed;
    public GameObject bed;

    public GameObject Image_sofa;
    public GameObject sofa;

    public static bool bed_active_flag = false;
    public static bool sofa_active_flag = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        if(CharactorMove.Tracked_bed_flag == true)
        {
            if(bed_active_flag == false)
            {
                bed.SetActive(true);
                bed_active_flag = true;
            }
            if (CharactorMove.bed_not_move_flag == false)
            {
                MoveFloor(Image_bed, bed);
            }
        }else
        {
            if (bed_active_flag == true)
            {
                bed.SetActive(false);
                bed_active_flag = false;
            }
        }



        if (CharactorMove.Tracked_sofa_flag == true)
        {
            if (sofa_active_flag == false)
            {
                sofa.SetActive(true);
                sofa_active_flag = true;
            }
            if (CharactorMove.sofa_not_move_flag == false)
            {
                MoveFloor(Image_sofa, sofa);
            }
        }
        else
        {
            if (sofa_active_flag == true)
            {
                sofa.SetActive(false);
                sofa_active_flag = false;
            }
        }


    }

    private void MoveFloor(GameObject image, GameObject obj)
    {

        Vector3 tmp_position = Vector3.zero;
        float image_rotation_y;

        tmp_position = image.transform.position;
        tmp_position.y = 0f;
        obj.transform.position = tmp_position;

        image_rotation_y = image.transform.eulerAngles.y;
        obj.transform.rotation = Quaternion.Euler(0f, image_rotation_y, 0f);

    }

}
