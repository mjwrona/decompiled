// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.AbandonJobResult
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  [DataContract]
  [JsonConverter(typeof (AbandonJobResultConverter))]
  public class AbandonJobResult
  {
    [DataMember]
    public bool Abandoned { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? ExpirationTime { get; set; }
  }
}
