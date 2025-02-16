using System.Collections;
using UnityEngine;

public class Anim_Scale : MonoBehaviour
{
    public float animationDuration = 0.5f; // Duration of the animation
    public AnimationCurve scaleCurve; // Animation curve for wobbling effect

    public void ScaleUp(GameObject objToScale)
    {
        StartCoroutine(ScaleUpAnimation(objToScale, Vector3.one)); // Default target scale: (1, 1, 1)
    }

    public void ScaleUp(GameObject objToScale, Vector3 targetScale)
    {
        StartCoroutine(ScaleUpAnimation(objToScale, targetScale));
    }

    public void ScaleDown(GameObject objToScale)
    {
        StartCoroutine(ScaleDownAnimation(objToScale, false));
    }
    public void ScaleDownAndDestroy(GameObject objToScale)
    {
        StartCoroutine(ScaleDownAnimation(objToScale, true));
    }

    IEnumerator ScaleUpAnimation(GameObject objToScale, Vector3 targetScale)
    {
        Vector3 initialScale = objToScale.transform.localScale;

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            float curveValue = scaleCurve.Evaluate(t);
            objToScale.transform.localScale = Vector3.Lerp(initialScale, targetScale, curveValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        objToScale.transform.localScale = targetScale;
    }

    IEnumerator ScaleDownAnimation(GameObject objToScale, bool destroy)
    {
        if(objToScale.GetComponent<Rigidbody>() != null)
        {
            objToScale.GetComponent<Rigidbody>().isKinematic = true;
        }
        
        Vector3 initialScale = objToScale.transform.localScale;
        Vector3 targetScale = Vector3.zero;

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            float curveValue = scaleCurve.Evaluate(t);
            objToScale.transform.localScale = Vector3.Lerp(initialScale, targetScale, curveValue);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        if (objToScale != null)
            objToScale.transform.localScale = targetScale;

        if (destroy && elapsedTime >= animationDuration)
        {
            Destroy(objToScale, animationDuration);
        }
    }
}
