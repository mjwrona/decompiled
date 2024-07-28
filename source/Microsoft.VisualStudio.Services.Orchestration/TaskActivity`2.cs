// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.TaskActivity`2
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  public abstract class TaskActivity<TInput, TResult> : AsyncTaskActivity<TInput, TResult>
  {
    protected TaskActivity(OrchestrationSerializer serializer)
      : base(serializer)
    {
    }

    protected abstract TResult Execute(TaskContext context, TInput input);

    protected override Task<TResult> ExecuteAsync(TaskContext context, TInput input) => Task.FromResult<TResult>(this.Execute(context, input));
  }
}
