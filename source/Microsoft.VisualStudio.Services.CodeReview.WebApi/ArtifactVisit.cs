// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.ArtifactVisit
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  [DataContract]
  public class ArtifactVisit : ISecuredObject
  {
    private ISecuredObject m_securedObject;

    [DataMember]
    public string ArtifactId { get; set; }

    [DataMember]
    public IdentityRef User { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? LastVisitedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? PreviousLastVisitedDate { get; set; }

    [DataMember]
    public string ViewedState { get; set; }

    [IgnoreDataMember]
    Guid ISecuredObject.NamespaceId => this.m_securedObject.NamespaceId;

    [IgnoreDataMember]
    int ISecuredObject.RequiredPermissions => this.m_securedObject.RequiredPermissions;

    public void SetSecuredObject(ISecuredObject securedObject) => this.m_securedObject = securedObject;

    string ISecuredObject.GetToken() => this.m_securedObject.GetToken();
  }
}
