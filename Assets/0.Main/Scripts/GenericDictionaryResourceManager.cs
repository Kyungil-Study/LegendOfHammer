using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public abstract class GenericDictionaryResourceManager<TRecord, TKey, TContainer> : SingletonBase<TContainer>, ILoadable
    where TContainer : GenericDictionaryResourceManager<TRecord, TKey, TContainer>, new()
{
    [SerializeField] protected string resourcePath;
    protected bool isLoaded = false;
    public bool IsLoaded => isLoaded;

    public Dictionary<TKey, TRecord> Records { get; protected set; }
    protected abstract TKey GetKey(TRecord record);

    public virtual void Load(Action<LoadCompleteEventArg> onComplete = null)
    {
        TSVLoader.LoadTableAsync<TRecord>(resourcePath).ContinueWith(taskResult =>
        {
            if (taskResult.IsCompleted && taskResult.Result != null)
            {
                Records = taskResult.Result.ToDictionary(GetKey);
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