using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;        // til Editor-mus-test

public class FishnetInteraction : MonoBehaviour
{
    // ────────── Inspector-felter ─────────────────────────────────
    [Header("References")]
    public List<GameObject> deadObjects;   // fisk/affald der klikkes væk
    public List<GameObject> crabs;         // krabber som flygter
    public Transform        fishnet;       // selve fiskenettet

    [Header("Timing / Speed")]
    public float crabMoveSpeed  = 0.5f;    // hvor hurtigt krabber løber
    public float netLiftSpeed   = 0.3f;    // hvor hurtigt nettet stiger
    public float fadeDuration   = 2f;      // hvor hurtigt krabber fader
    public float crabEscapeDist = 6f;      // max afstand krabbe flytter sig
    public float netTargetY     = 17.51f;  // slut-højde for nettet
    public float netDelayAfterCleared = 3f; // tid fra alle fisk væk → net stiger

    // ────────── intern tilstand ─────────────────────────────────
    Camera cam;
    int    deadClicked = 0;
    bool   sequenceStarted = false;

    // ────────── setup ───────────────────────────────────────────
    void Awake()
    {
        cam = Camera.main;
        XRInteractionManager mgr = FindAnyObjectByType<XRInteractionManager>();

        foreach (GameObject obj in deadObjects)
        {
            var grab = obj.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>() ??
                       obj.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

            grab.interactionManager = mgr;
            grab.selectEntered.AddListener(_ => OnDeadClicked(obj));
        }
    }

#if UNITY_EDITOR
    // ────────── mus-klik til hurtig test i Editor ───────────────
    void Update()
    {
        if (!sequenceStarted &&
            Mouse.current?.leftButton.wasPressedThisFrame == true)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit) &&
                deadObjects.Contains(hit.transform.gameObject))
            {
                OnDeadClicked(hit.transform.gameObject);
            }
        }
    }
#endif

    // ────────── håndter klik på “dødt” objekt ───────────────────
    void OnDeadClicked(GameObject obj)
    {
        if (!obj.activeSelf) return;

        obj.SetActive(false);
        deadClicked++;
        Debug.Log($"Klikket: {deadClicked}/{deadObjects.Count}");

        if (!sequenceStarted && deadClicked >= deadObjects.Count)
        {
            sequenceStarted = true;
            StartCoroutine(CrabSequence());          // bevæg/fade krabber
            StartCoroutine(LiftNetAfterDelay());     // vent 1.3 s → løft net
        }
    }

    // ────────── krabber flygter ─────────────────────────────────
    IEnumerator CrabSequence()
    {
        List<Vector3> targets = new List<Vector3>();
        foreach (GameObject crab in crabs)
        {
            targets.Add(crab.transform.position +
                        new Vector3(Random.Range(-crabEscapeDist,  crabEscapeDist),
                                    0,
                                    Random.Range(-crabEscapeDist,  crabEscapeDist)));
            StartCoroutine(FadeOut(crab));
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * crabMoveSpeed;
            for (int i = 0; i < crabs.Count; i++)
                crabs[i].transform.position =
                    Vector3.Lerp(crabs[i].transform.position, targets[i], t);

            yield return null;
        }
    }

    // ────────── løft net efter forsinkelse ──────────────────────
    IEnumerator LiftNetAfterDelay()
    {
        yield return new WaitForSeconds(netDelayAfterCleared);

        Vector3 startPos = fishnet.position;
        Vector3 endPos   = new Vector3(startPos.x, netTargetY, startPos.z);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * netLiftSpeed;
            fishnet.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
    }

    // ────────── fade-out helper ─────────────────────────────────
    IEnumerator FadeOut(GameObject obj)
    {
        Renderer[] rends = obj.GetComponentsInChildren<Renderer>();
        List<Material> mats = new List<Material>();

        foreach (Renderer r in rends)
        {
            Material m = r.material;               // instans
            if (!m.HasProperty("_Color")) continue;

            m.SetFloat("_Surface", 1);             // Transparent
            m.SetOverrideTag("RenderType", "Transparent");
            m.SetInt("_SrcBlend",  (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            m.SetInt("_DstBlend",  (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            m.SetInt("_ZWrite",    0);
            m.DisableKeyword("_ALPHATEST_ON");
            m.EnableKeyword ("_ALPHABLEND_ON");
            m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            mats.Add(m);
        }

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            foreach (Material m in mats)
            {
                Color c = m.color; c.a = a; m.color = c;
            }
            yield return null;
        }
    }
}
