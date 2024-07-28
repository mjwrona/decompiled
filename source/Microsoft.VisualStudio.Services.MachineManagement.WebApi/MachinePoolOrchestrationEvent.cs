// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachinePoolOrchestrationEvent
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  [KnownType(typeof (MachinePoolOrchestrationEvent))]
  [JsonConverter(typeof (MachinePoolEventJsonConverter))]
  public abstract class MachinePoolOrchestrationEvent
  {
    protected MachinePoolOrchestrationEvent(string name) => this.Name = name;

    [DataMember]
    public string Name { get; private set; }
  }
}
