// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseWorkItemRef
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class ReleaseWorkItemRef : ReleaseManagementSecuredObject
  {
    [DataMember(Name = "id")]
    public string Id { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "The name Url is appropariate name for this property and it signifies REST URL of resource")]
    [DataMember(Name = "url", EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(Name = "title", EmitDefaultValue = false)]
    public string Title { get; set; }

    [DataMember(Name = "type", EmitDefaultValue = false)]
    public string Type { get; set; }

    [DataMember(Name = "assignee", EmitDefaultValue = false)]
    [Obsolete]
    public string Assignee { get; set; }

    [DataMember(Name = "state", EmitDefaultValue = false)]
    public string State { get; set; }

    [DataMember(Name = "provider", EmitDefaultValue = false)]
    public string Provider { get; set; }
  }
}
