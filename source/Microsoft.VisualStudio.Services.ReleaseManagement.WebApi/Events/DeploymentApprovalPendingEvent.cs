// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events.DeploymentApprovalPendingEvent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events
{
  [DataContract]
  [ServiceEventObject]
  [NotificationEventBindings(EventSerializerType.Json, "ms.vss-release.deployment-approval-pending-event")]
  public class DeploymentApprovalPendingEvent : DeploymentEvent
  {
    [DataMember]
    public Release Release { get; set; }

    [DataMember]
    public ReleaseApproval Approval { get; set; }

    [DataMember]
    public ProjectReference Project { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "XML serializer cannot serialize collections/interfaces")]
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember]
    public List<ReleaseApproval> PendingApprovals { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "XML serializer cannot serialize collections/interfaces")]
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember]
    public List<ReleaseApproval> CompletedApprovals { get; set; }

    [DataMember]
    public ApprovalOptions ApprovalOptions { get; set; }

    [DataMember]
    public bool IsMultipleRankApproval { get; set; }

    [DataMember]
    public Deployment Deployment { get; set; }

    [DataMember]
    public IDictionary<string, object> Data { get; set; }

    public DeploymentApprovalPendingEvent() => this.Data = (IDictionary<string, object>) new Dictionary<string, object>();
  }
}
