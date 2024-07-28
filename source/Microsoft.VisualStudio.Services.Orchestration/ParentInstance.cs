// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.ParentInstance
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  [DataContract]
  public class ParentInstance
  {
    [DataMember]
    public OrchestrationInstance OrchestrationInstance;
    [DataMember]
    public string Name;
    [DataMember]
    public string Version;
    [DataMember]
    public int TaskScheduleId;

    internal ParentInstance Clone() => new ParentInstance()
    {
      Name = this.Name,
      Version = this.Version,
      TaskScheduleId = this.TaskScheduleId,
      OrchestrationInstance = this.OrchestrationInstance.Clone()
    };
  }
}
