using UnityEngine;
using UnityEngine.UI;


public class controlInfo : MonoBehaviour
{
    public GameObject controlinfo;
    private void Start()
    {
        controlinfo.SetActive(false);
    }




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            controlinfo.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            controlinfo.SetActive(false);
        }
    }
}
