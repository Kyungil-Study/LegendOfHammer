using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class GenericResourceManager<TRecord, TContainer> : SingletonBase<TContainer>, ILoadable
    where TContainer : GenericResourceManager<TRecord, TContainer>, new()
{
    [SerializeField] protected string resourcePath;
    protected bool isLoaded = false;
    public bool IsLoaded => isLoaded;

    public IReadOnlyList<TRecord> Records { get; protected set; }

    public virtual void Load(Action<LoadCompleteEventArg> onComplete = null)
    {
        TSVLoader.LoadTableAsync<TRecord>(resourcePath).ContinueWith(taskResult =>
        {
            if (taskResult.IsCompleted && taskResult.Result != null)
            {
                Records = taskResult.Result;
                isLoaded = true;
                onComplete?.Invoke(new LoadCompleteEventArg(true));
            }
            else
            {
                isLoaded = false;
                onComplete?.Invoke(new LoadCompleteEventArg(false, "Failed to load data from " + resourcePath));
            }
        });
    }

    public async Task<LoadCompleteEventArg> LoadAsync()
    {
        var tcs = new TaskCompletionSource<LoadCompleteEventArg>();
        Load(result => tcs.SetResult(result));
        return await tcs.Task;
    }
}