﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class quest
{
    public bool Active;
    public bool AllowDialogueActivation;
    public string questTitle;
    public string currentQuestInfo;
    public GameObject qButton;
    public int subQuest;
    public int maxSubQuest;
    public int progress;
    public int maxProgress;
    public List<string> questInfo;

    public quest(string title)
    {
        Active = false;
        AllowDialogueActivation = false;
        questTitle = title;
        currentQuestInfo = " ";
        qButton = null;
        subQuest = 0;
        maxSubQuest = 1;
        progress = 0;
        maxProgress = 1;
        questInfo = new List<string>();
    }
    public void activate()
    {
        Active = true;
        qButton.SetActive(true);
        updateQuestInfo();
    }
    public void deActivate()
    {
        Active = false;
        qButton.SetActive(false);
    }
    public void updateQuestInfo()
    {
        currentQuestInfo = questInfo[subQuest];
    }
    
}

public class Adventureog : MonoBehaviour
{
    public static Adventureog advLogInstance = null;
    private GameObject AdventureLogCanvas;
    [Tooltip("Use \"Quest Text\" text")]
    public Text QuestText;
    [Tooltip("Use \"Quest Title\" text")]
    public Text QuestTitle;
    [Tooltip("Use \"Quest Idle\" sprite")]
    public Sprite unpressed;
    [Tooltip("Use \"Quest Pressed\" sprite")]
    public Sprite pressed;
    private Vector3 advLogPos;
    [Tooltip("Open the menu by default")]
    public bool isOpen;

    public List<quest> Quest = new List<quest>();
    public List<string> QuestTitles;
    [Tooltip("The name of the quest. The total number put here also registers that many quests")]
    private int totalQuests;
    [Tooltip("Must be same length as QuestTitles. These should map to a button in the adventure log panel")]
    public List<GameObject> QuestButton;

    // Start is called before the first frame update
    void Start()
    {
        //Create Quests based on Titles
        totalQuests = QuestTitles.Count;
        for(int i = 0; i < totalQuests; i++)
        {
            Quest.Add(new quest(QuestTitles[i]));
            Quest[i].qButton = QuestButton[i];
        }

        if (advLogInstance == null) advLogInstance = this;
        else if (advLogInstance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        AdventureLogCanvas = GameObject.Find("AdventureLogPanel/AdventureLogBox");
        advLogPos = AdventureLogCanvas.transform.position;

        for (int i = 0; i < totalQuests; i++)
            Quest[i].qButton.SetActive(false);
    }

    void Update()
    {
            
    }

    bool checkAnyButtonActive()
    {
        for (var i = 0; i < totalQuests; i++)
            if (Quest[i].qButton.activeInHierarchy == true)
                return true;
        return false;
    }

    public void openLog()
    {
        AdventureLogCanvas.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        GameManager.instance.inConversation = true;
        QuestTitle.text = " ";
        QuestText.text = " ";
        if (checkAnyButtonActive() == false)
            QuestText.text = "Adventure Log is currently empty.";
        isOpen = true;
    }

    public void closeLog()
    {
        AdventureLogCanvas.transform.position = advLogPos;
        GameManager.instance.inConversation = false;
        resetButtons();
        isOpen = false;
    }

    void resetButtons()
    {
        for (var i = 0; i < totalQuests; i++)
            Quest[i].qButton.GetComponent<Button>().image.sprite = unpressed;
    }

    public void updateQuestText(int num)
    {
        if (num > 0 && num <= totalQuests) {
            QuestTitle.text = Quest[num - 1].questTitle;
            QuestText.text = Quest[num - 1].currentQuestInfo;
            Quest[num - 1].qButton.GetComponent<Button>().image.sprite = pressed;
        }
        else {
            Debug.Log("Error setting questText!!");
        }
    }
    
    public void addProgress(int questNum, int progressToAdd)
    {
        var Q = Quest[questNum - 1];
        Q.progress += progressToAdd;
        if (Q.progress >= Q.maxProgress)
        {
            Q.progress = Q.maxProgress;
            activateNextSubQuest(questNum);
        }
    }

    private void activateNextSubQuest(int questNum)
    {
        var Q = Quest[questNum - 1];
        Q.subQuest += 1;
        if (Q.subQuest == Q.maxSubQuest)
        {
            Q.deActivate();
        }
        else
        {
            Q.updateQuestInfo();
        }
    }
}
