// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.OrchestrationInstance
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  [DataContract]
  public class OrchestrationInstance
  {
    [DataMember]
    public string InstanceId { get; set; }

    [DataMember]
    public string ExecutionId { get; set; }

    internal OrchestrationInstance Clone() => new OrchestrationInstance()
    {
      ExecutionId = this.ExecutionId,
      InstanceId = this.InstanceId
    };

    public override int GetHashCode() => (this.InstanceId ?? string.Empty).GetHashCode() ^ (this.ExecutionId ?? string.Empty).GetHashCode();

    public override string ToString() => string.Format("[InstanceId: {0}, ExecutionId: {1}]", (object) this.InstanceId, (object) this.ExecutionId);
  }
}
