using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LoadCompleteEventArg
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public LoadCompleteEventArg(bool success = true, string errorMessage = "")
    {
        Success = success;
        ErrorMessage = errorMessage;
    }
}

public interface ILoadable 
{
    public bool IsLoaded { get; }
    public void Load(Action<LoadCompleteEventArg> onComplete = null);
    
    public Task<LoadCompleteEventArg> LoadAsync();

}