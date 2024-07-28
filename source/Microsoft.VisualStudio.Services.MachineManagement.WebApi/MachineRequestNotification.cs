// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineRequestNotification
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public sealed class MachineRequestNotification
  {
    internal MachineRequestNotification()
    {
    }

    internal MachineRequestNotification(
      MachineRequest machineRequest,
      MachineRequestNotificationType notificationType)
      : this(machineRequest, (MachineRequestResult) null, notificationType)
    {
    }

    internal MachineRequestNotification(
      MachineRequest machineRequest,
      MachineRequestResult machineRequestResult,
      MachineRequestNotificationType notificationType)
    {
      this.MachineRequest = machineRequest;
      this.MachineRequestResult = machineRequestResult;
      this.NotificationType = notificationType;
    }

    [DataMember(IsRequired = true)]
    public MachineRequest MachineRequest { get; set; }

    [DataMember(IsRequired = false)]
    public MachineRequestResult MachineRequestResult { get; set; }

    [DataMember(IsRequired = true)]
    public MachineRequestNotificationType NotificationType { get; set; }
  }
}
