using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AugmentManager : MonoBehaviour
{
    [SerializeField] string commonAugmentPath = "Augment/CommonAugment";
    [SerializeField] string archerAugmentPath = "Augment/ArcherAugment";
    [SerializeField] string warriorAugmentPath = "Augment/WarriorAugment";
    [SerializeField] string wizardAugmentPath = "Augment/WizardAugment";

    public Dictionary<int, CommonAugment> CommonAugments = new Dictionary<int, CommonAugment>();
    public Dictionary<int, WarriorAugment> WarriorAugments = new Dictionary<int, WarriorAugment>();
    public Dictionary<int, ArcherAugment> ArcherAugments = new Dictionary<int, ArcherAugment>();
    public Dictionary<int, WizardAugment> WizardAugments = new Dictionary<int, WizardAugment>();
    
    private void Awake()
    {
        LoadAugmentData();
    }

    private void LoadAugmentData()
    {
        
        // Load all augment data from the specified paths
        TSVLoader.LoadTableAsync<WarriorAugment>(warriorAugmentPath).ContinueWith(
            (taskResult) =>
            {
                var list = taskResult.Result;
                Debug.Log($"[AugmentDataManager] Warrior Augments loaded successfully. Total: {list.Count}");
                foreach (var it in list)
                {
                    if (!WarriorAugments.ContainsKey(it.ID))
                    {
                        WarriorAugments.Add(it.ID, it);
                    }
                    else
                    {
                        Debug.LogError($"[AugmentDataManager] Duplicate Warrior Augment ID: {it.ID} Name: {it.Name}");
                    }
                    
                }
            }
        );
        TSVLoader.LoadTableAsync<ArcherAugment>(archerAugmentPath).ContinueWith(
            (taskResult) =>
            {
                var list = taskResult.Result;
                Debug.Log($"[AugmentDataManager] Archer Augments loaded successfully. Total: {list.Count}");
                foreach (var it in list)
                {
                    if (!ArcherAugments.ContainsKey(it.ID))
                    {
                        ArcherAugments.Add(it.ID, it);
                    }
                    else
                    {
                        Debug.LogError($"[AugmentDataManager] Duplicate Warrior Augment ID: {it.ID} Name: {it.Name}");
                    }
                    
                }
            }
        );
        TSVLoader.LoadTableAsync<WizardAugment>(wizardAugmentPath).ContinueWith(
            (taskResult) =>
            {
                var list = taskResult.Result;
                Debug.Log($"[AugmentDataManager] Wizard Augments loaded successfully. Total: {list.Count}");
                foreach (var it in list)
                {
                    if (!WizardAugments.ContainsKey(it.ID))
                    {
                        WizardAugments.Add(it.ID, it);
                    }
                    else
                    {
                        Debug.LogError($"[AugmentDataManager] Duplicate Warrior Augment ID: {it.ID} Name: {it.Name}");
                    }
                    
                }
            }
        );
            
        TSVLoader.LoadTableAsync<CommonAugment>(commonAugmentPath).ContinueWith(
            (taskResult) =>
            {
                var list = taskResult.Result;
                Debug.Log($"[AugmentDataManager] Common Augments loaded successfully. Total: {list.Count}");
                foreach (var it in list)
                {
                    if (!CommonAugments.ContainsKey(it.ID))
                    {
                        CommonAugments.Add(it.ID, it);
                    }
                    else
                    {
                        Debug.LogError($"[AugmentDataManager] Duplicate Common Augment ID: {it.ID} Name: {it.AugmentName}");
                    }
                    
                }
            }
        );
            
            
    }

    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
