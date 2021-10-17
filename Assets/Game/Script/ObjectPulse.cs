using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPulse : MonoBehaviour
{
    [SerializeField] float minScale = 2f;
    [SerializeField] float maxnScale = 2.6f;
    [SerializeField] float timeAnim = 0.5f;
    void OnEnable()
    {
        StartCoroutine(Pulse());
    }

    void OnDesable()
    {
        StopCoroutine(Pulse());
    }

    IEnumerator Pulse()
    {
        transform.localScale = Vector3.one * minScale;

        while (true)
        {
            bool finishAnim = false;

            LTDescr maxAnim = transform.LeanScale(Vector3.one * maxnScale, timeAnim).setOnComplete(
                () => {
                    transform.LeanScale(Vector3.one * minScale, timeAnim).setOnComplete(() => finishAnim = true);
                });

            yield return new WaitWhile(() => !finishAnim);
        }

        yield return null;
    }
}
