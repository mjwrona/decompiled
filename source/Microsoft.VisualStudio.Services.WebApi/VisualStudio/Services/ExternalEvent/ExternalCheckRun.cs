// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExternalEvent.ExternalCheckRun
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExternalEvent
{
  [DataContract]
  public class ExternalCheckRun
  {
    [DataMember]
    public string Id;
    [DataMember]
    public string Name;
    [DataMember]
    public string WorkflowName;
    [DataMember]
    public DateTime? StartedAt;
    [DataMember]
    public DateTime? CompletedAt;
    [DataMember]
    public string Status;
    [DataMember]
    public string Conclusion;
  }
}
