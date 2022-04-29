using UnityEngine;
using UnityEngine.UI;


public class BridgeInfo : MonoBehaviour
{
    public GameObject bridgeInfo;
    private void Start()
    {
        bridgeInfo.SetActive(false);
    }




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            bridgeInfo.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            bridgeInfo.SetActive(false);
        }
    }
}
