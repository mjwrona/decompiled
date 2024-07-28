// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.OrchestrationState
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  [DataContract]
  public class OrchestrationState
  {
    [DataMember]
    public OrchestrationInstance OrchestrationInstance;
    [DataMember]
    public ParentInstance ParentInstance;
    [DataMember]
    public string Name;
    [DataMember]
    public string Version;
    [DataMember]
    public string Status;
    [DataMember]
    public string Tags;
    [DataMember]
    public OrchestrationStatus OrchestrationStatus;
    [DataMember]
    public DateTime CreatedTime;
    [DataMember]
    public DateTime? CompletedTime;
    [DataMember]
    public DateTime LastUpdatedTime;
    [DataMember]
    public long Size;
    [DataMember]
    public long CompressedSize;
    [DataMember]
    public string Input;
    [DataMember]
    public string Output;
  }
}
