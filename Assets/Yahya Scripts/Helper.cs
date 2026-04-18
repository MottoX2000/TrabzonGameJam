using System;
using System.Collections;
using UnityEngine;

public static class Helper
{
    public static void DoAfterDelay(float delay, Action action)
    {
        GameManager.Instance.StartCoroutine(DoAfterDelayCoroutine(delay, action));
    }

    private static IEnumerator DoAfterDelayCoroutine(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
