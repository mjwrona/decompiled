// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events.DeploymentManualInterventionPendingEvent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events
{
  [DataContract]
  [ServiceEventObject]
  [NotificationEventBindings(EventSerializerType.Json, "ms.vss-release.deployment-mi-pending-event")]
  public class DeploymentManualInterventionPendingEvent
  {
    [DataMember]
    public ManualIntervention ManualIntervention { get; set; }

    [DataMember]
    public ProjectReference Project { get; set; }

    [DataMember]
    public Release Release { get; set; }

    [DataMember]
    public ReleaseApproval Approval { get; set; }

    [DataMember]
    public IdentityRef EnvironmentOwner { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "XML serializer cannot serialize collections/interfaces")]
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [DataMember]
    public List<Guid> EmailRecipients { get; set; }

    [DataMember]
    public Deployment Deployment { get; set; }
  }
}
