using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comunicate : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAnim()
    {
        switch (Random.Range(1, 8))
        {
            case 1:
                anim.SetTrigger("miku1");
                break;
            case 2:
                anim.SetTrigger("miku2");
                break;
            case 3:
                anim.SetTrigger("miku3");
                break;
            case 4:
                anim.SetTrigger("miku4");
                break;
            case 5:
                anim.SetTrigger("miku5");
                break;
            case 6:
                anim.SetTrigger("miku6");
                break;
            case 7:
                anim.SetTrigger("miku7");
                break;
            case 8:
                anim.SetTrigger("miku8");
                break;
        }
    }
}
