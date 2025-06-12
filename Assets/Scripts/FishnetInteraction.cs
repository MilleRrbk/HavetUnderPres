
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class FishnetInteraction : MonoBehaviour
{
    public List<GameObject> deadObjects;
    public List<GameObject> crabs;
    public Transform fishnet;
    public float crabMoveSpeed = 1f;
    public float fadeDuration = 2f;
    public float crabEscapeDistance = 5f;

    private int deadClickedCount = 0;

    void Start()
    {
        foreach (var obj in deadObjects)
        {
            AddClickHandler(obj);
        }
    }

    void AddClickHandler(GameObject obj)
    {
        var interactable = obj.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (interactable == null)
            interactable = obj.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        interactable.selectEntered.AddListener(_ => OnDeadObjectClicked(obj));
    }

    void OnDeadObjectClicked(GameObject obj)
    {
        obj.SetActive(false);
        deadClickedCount++;

        if (deadClickedCount >= deadObjects.Count)
        {
            StartCoroutine(FreeCrabsAndMoveNet());
        }
    }

    IEnumerator FreeCrabsAndMoveNet()
    {
        List<Vector3> targetPositions = new List<Vector3>();

        foreach (var crab in crabs)
        {
            Vector3 target = crab.transform.position + new Vector3(Random.Range(-crabEscapeDistance, crabEscapeDistance), 0, Random.Range(-crabEscapeDistance, crabEscapeDistance));
            targetPositions.Add(target);
            StartCoroutine(FadeOut(crab));
        }

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * crabMoveSpeed;
            for (int i = 0; i < crabs.Count; i++)
            {
                crabs[i].transform.position = Vector3.Lerp(crabs[i].transform.position, targetPositions[i], t);
            }
            yield return null;
        }

        yield return new WaitForSeconds(fadeDuration);

        // Move net upward
        Vector3 netPosition = fishnet.position;
        netPosition.y = 17.51f;
        fishnet.position = netPosition;
    }

    IEnumerator FadeOut(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            Material mat = r.material; // Use unique instance for fade
            if (mat.HasProperty("_Color"))
            {
                Color c = mat.color;
                for (float t = 0; t < fadeDuration; t += Time.deltaTime)
                {
                    c.a = Mathf.Lerp(1, 0, t / fadeDuration);
                    mat.color = c;
                    yield return null;
                }
                c.a = 0;
                mat.color = c;
            }
        }

        obj.SetActive(false);
    }
}
