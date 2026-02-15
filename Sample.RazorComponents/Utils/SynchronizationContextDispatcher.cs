using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace MRoessler.BlazorBottomSheet.Sample.RazorComponents.Utils;

/// <summary>
/// thread-safe
/// </summary>
public sealed class SynchronizationContextDispatcher
{
    private SynchronizationContext? _syncContext;
    private TaskFactory _taskFactory = Task.Factory;

    public void InitFromCurrentSyncContext()
    {
        _syncContext = SynchronizationContext.Current;
        if (_syncContext != null) // check WASM case
            _taskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
    }

    [SuppressMessage("Reliability", "CA2008:Do not create tasks without passing a TaskScheduler", Justification = "we initialized the TaskFactory with a default TaskScheduler")]
    public Task InvokeAsync(Func<Task> task)
    {
        var taskFactory = _taskFactory ?? throw new InvalidOperationException("not initialized yet");
        return taskFactory.StartNew(task);
    }

    public void Invoke(Action action)
    {
        if (_syncContext != null) // check WASM case
            _syncContext.Post(_ => action(), null);
        else
            action();
    }
}
