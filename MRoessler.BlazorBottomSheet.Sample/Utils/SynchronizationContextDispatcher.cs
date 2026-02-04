using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace MRoessler.BlazorBottomSheet.Sample.Utils;

/// <summary>
/// thread-safe
/// </summary>
public sealed class SynchronizationContextDispatcher
{
    private SynchronizationContext? _syncContext;
    private TaskFactory? _taskFactory;

    public void InitFromCurrentSyncContext()
    {
        _syncContext = SynchronizationContext.Current ?? throw new InvalidOperationException("SynchronizationContext.Current is null");
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
        var syncContext = _syncContext ?? throw new InvalidOperationException("not initialized yet");
        syncContext.Post(_ => action(), null);
    }
}
