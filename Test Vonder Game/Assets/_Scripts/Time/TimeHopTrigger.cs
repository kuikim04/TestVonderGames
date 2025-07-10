using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeHopTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TimeManager.Instance.ForceAdvanceTime();
        }
    }
}
