// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events.ReleaseEvent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events
{
  [KnownType(typeof (ReleaseCreatedEvent))]
  [KnownType(typeof (ReleaseAbandonedEvent))]
  [KnownType(typeof (DeploymentCompletedEvent))]
  [KnownType(typeof (DeploymentStartedEvent))]
  [KnownType(typeof (DeploymentApprovalCompletedEvent))]
  [KnownType(typeof (DeploymentApprovalPendingEvent))]
  [DataContract]
  [ServiceEventObject]
  public abstract class ReleaseEvent
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Url { get; set; }
  }
}
