// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.TaskOrchestration`4
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  public abstract class TaskOrchestration<TResult, TInput, TEvent, TStatus> : TaskOrchestration
  {
    public override async Task<string> Execute(OrchestrationContext context, string input)
    {
      TaskOrchestration<TResult, TInput, TEvent, TStatus> taskOrchestration = this;
      taskOrchestration.ThrowIfSerializerNotSet();
      TInput input1 = taskOrchestration.Serializer.Deserialize<TInput>(input);
      TResult result1 = default (TResult);
      TResult result2;
      try
      {
        result2 = await taskOrchestration.RunTask(context, input1);
      }
      catch (Exception ex)
      {
        OrchestrationSerializer serializer = taskOrchestration.Serializer;
        string details = Utils.SerializeCause(ex, serializer);
        throw new OrchestrationFailureException(ex.Message, details);
      }
      return taskOrchestration.Serializer.Serialize((object) result2);
    }

    public override void RaiseEvent(OrchestrationContext context, string name, string input)
    {
      this.ThrowIfSerializerNotSet();
      TEvent input1 = this.Serializer.Deserialize<TEvent>(input);
      this.OnEvent(context, name, input1);
    }

    public override string GetStatus()
    {
      this.ThrowIfSerializerNotSet();
      return this.StateSerializer.Serialize((object) this.OnGetStatus());
    }

    public abstract Task<TResult> RunTask(OrchestrationContext context, TInput input);

    public virtual void OnEvent(OrchestrationContext context, string name, TEvent input)
    {
    }

    public virtual TStatus OnGetStatus() => default (TStatus);

    private void ThrowIfSerializerNotSet()
    {
      if (this.Serializer == null)
        throw new InvalidOperationException("Serializer property not set. Please set it before using any of the methods.");
    }
  }
}
