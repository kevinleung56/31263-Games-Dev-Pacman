using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tweener : MonoBehaviour
{
    private List<Tween> activeTweens;

    // Start is called before the first frame update
    void Start()
    {
        activeTweens = new List<Tween>();
    }

    public bool TweenExists(Transform target)
    {
        foreach (var tween in activeTweens)
        {
            if (tween.Target == target)
            {
                return true;
            }
        }

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < activeTweens.Count; i++)
        {
            if (activeTweens[i] != null)
            {
                if (Vector3.Distance(activeTweens[i].Target.position, activeTweens[i].EndPos) > 0.1f)
                {
                    var proportion = (Time.time - activeTweens[i].StartTime) / activeTweens[i].Duration;
                    activeTweens[i].Target.position = Vector3.Lerp(activeTweens[i].StartPos, activeTweens[i].EndPos, proportion);
                }

                if (Vector3.Distance(activeTweens[i].Target.position, activeTweens[i].EndPos) <= 0.1f)
                {
                    activeTweens[i].Target.position = activeTweens[i].EndPos;
                    activeTweens[i] = null;
                    activeTweens.Remove(activeTweens[i]);
                }
            }
        }
    }

    public bool AddTween(Transform targetObject, Vector3 startPos, Vector3 endPos, float duration)
    {
        if (TweenExists(targetObject))
        {
            return false;
        }

        activeTweens.Add(new Tween(targetObject, startPos, endPos, Time.time, duration));
        return true;
    }
}
