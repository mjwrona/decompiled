// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildController
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class BuildController : XamlBuildControllerReference, ISecuredObject
  {
    [DataMember(Name = "_links", EmitDefaultValue = false)]
    private ReferenceLinks m_links;

    public BuildController() => this.Status = ControllerStatus.Unavailable;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Uri Uri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ControllerStatus Status { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool Enabled { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime CreatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime UpdatedDate { get; set; }

    public ReferenceLinks Links
    {
      get
      {
        if (this.m_links == null)
          this.m_links = new ReferenceLinks();
        return this.m_links;
      }
    }

    Guid ISecuredObject.NamespaceId => Security.BuildNamespaceId;

    int ISecuredObject.RequiredPermissions => 1;

    string ISecuredObject.GetToken() => throw new NotImplementedException();
  }
}
