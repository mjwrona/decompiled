// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events.ReleaseEnvironmentCompletedEvent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Conditions;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Events
{
  [DataContract]
  [ServiceEventObject]
  public class ReleaseEnvironmentCompletedEvent
  {
    [DataMember]
    public ReleaseEnvironment Environment { get; set; }

    [DataMember]
    public string Title { get; set; }

    [DataMember]
    public string Status { get; set; }

    [DataMember]
    public string ReleaseName { get; set; }

    [DataMember]
    public string CreatedByName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef ReleaseCreatedBy { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "This is used for xsl transforms, cannot be an object")]
    [DataMember]
    public string WebAccessUri { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "This is used for xsl transforms, cannot be an object")]
    [DataMember]
    public string ReleaseLogsUri { get; set; }

    [DataMember]
    public string ProjectName { get; set; }

    [DataMember]
    public int DefinitionId { get; set; }

    [DataMember]
    public string DefinitionName { get; set; }

    [DataMember]
    public int EnvironmentId { get; set; }

    [DataMember]
    public DeploymentReason Reason { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "XML serializer cannot serialize collections/interfaces")]
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "This is set by event listener")]
    public List<Condition> Conditions { get; set; }

    public ReleaseEnvironmentCompletedEvent() => this.Conditions = new List<Condition>();
  }
}
