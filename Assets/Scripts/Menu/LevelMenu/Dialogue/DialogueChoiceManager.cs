using UnityEngine;
using System.Collections;
using Cinemachine;

public class DialogueChoiceManager : MonoBehaviour
{
    public static DialogueChoiceManager dialogueChoice;

    public CinemachineVirtualCamera mainCamera;
    public GameObject startDialogueTrigger;
    public GameObject jumpStartDialogueTrigger;
    public GameObject jumpDialogueTrigger;
    public GameObject crystallDialogueTrigger;
    public GameObject jumpBonusDialogueTrigger;
    public GameObject gunGetDialogueTrigger;
    public GameObject finalDialogueTrigger;
    public GameObject finalLevel;
    public GameObject gunGetFinalDialogueTrigger;

    [Header("Музыка")]
    public GameObject standard;
    public GameObject final;

    private void Awake()
    {
        dialogueChoice = this;
    }

    public void DoSomethingFromId(int id)
    {
        switch (id) {
            case 0: StartCoroutine(CameraOffset(4.5f, 15, 0.025f)); break;
            case 1: StartCoroutine(CameraOffset(15f, 0, -0.025f)); break;
            case 2: Destroy(startDialogueTrigger); break;
            case 3: Destroy(jumpStartDialogueTrigger); break;
            case 4: Destroy(jumpDialogueTrigger); break;
            case 5: Destroy(crystallDialogueTrigger); break;
            case 6: Destroy(jumpBonusDialogueTrigger); break;
            case 7: Destroy(gunGetDialogueTrigger); break;
            case 8:  Destroy(finalDialogueTrigger); break;
            case 9: Destroy(gunGetFinalDialogueTrigger); break;
        }
    }

    IEnumerator CameraOffset(float start, float end, float step) {
        var transposer = mainCamera.GetCinemachineComponent<CinemachineTransposer>();
        float nowOffset = start;
        while (Mathf.Abs(end - nowOffset) >= Mathf.Abs(step))
        {
            transposer.m_FollowOffset = new Vector3(nowOffset, transposer.m_FollowOffset.y, transposer.m_FollowOffset.z);
            for (int i=0; i < 6; i++)
                nowOffset += step;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public void SetCoolMusic()
    {
        standard.SetActive(false);
        final.SetActive(true);
    }
}
