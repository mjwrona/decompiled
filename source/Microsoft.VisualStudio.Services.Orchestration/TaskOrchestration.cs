// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.TaskOrchestration
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  public abstract class TaskOrchestration
  {
    public OrchestrationSerializer Serializer { get; set; }

    public OrchestrationSerializer StateSerializer { get; set; }

    public abstract Task<string> Execute(OrchestrationContext context, string input);

    public abstract void RaiseEvent(OrchestrationContext context, string name, string input);

    public abstract string GetStatus();
  }
}
