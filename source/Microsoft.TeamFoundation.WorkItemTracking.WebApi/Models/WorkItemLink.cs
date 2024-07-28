// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemLink
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [DataContract]
  public class WorkItemLink : ISecuredObject
  {
    private string m_token;

    [DataMember]
    public string Rel { get; set; }

    [DataMember]
    public WorkItemReference Source { get; set; }

    [DataMember]
    public WorkItemReference Target { get; set; }

    public WorkItemLink()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public WorkItemLink(string token) => this.m_token = token;

    string ISecuredObject.GetToken() => this.m_token;

    Guid ISecuredObject.NamespaceId => WitConstants.SecurityConstants.CommonStructureNodeSecurityNamespaceGuid;

    int ISecuredObject.RequiredPermissions => 16;
  }
}
