// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MmsProvisionerTrace
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public class MmsProvisionerTrace
  {
    [DataMember(IsRequired = true)]
    public string Level { get; set; }

    [DataMember(IsRequired = true)]
    public string MachineInstanceName { get; set; }

    [DataMember(IsRequired = true)]
    public string MachinePoolName { get; set; }

    [DataMember(IsRequired = true)]
    public long MachineRequestId { get; set; }

    [DataMember(IsRequired = true)]
    public string Message { get; set; }

    [DataMember(IsRequired = true)]
    public DateTime TimeStamp { get; set; }

    [DataMember(IsRequired = false)]
    public string OrchestrationId { get; set; }

    [DataMember(IsRequired = false)]
    public Guid ExecutionId { get; set; }
  }
}
