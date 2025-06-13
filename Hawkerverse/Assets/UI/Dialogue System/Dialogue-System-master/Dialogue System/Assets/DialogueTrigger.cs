using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

	public Dialogue dialogue;
    public GameObject TestButton; // Drag your button GameObject into this field in the Inspector

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        TestButton.SetActive(false); // Hides the button after it's pressed
    }

}
