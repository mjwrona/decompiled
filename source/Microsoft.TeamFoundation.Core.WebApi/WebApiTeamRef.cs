// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.WebApiTeamRef
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [DataContract]
  public class WebApiTeamRef : ISecuredObject
  {
    private readonly ISecuredObject m_securedObject;

    public WebApiTeamRef()
    {
    }

    public WebApiTeamRef(ISecuredObject securedObject) => this.m_securedObject = securedObject;

    public WebApiTeamRef(WebApiTeamRef teamRef, ISecuredObject securedObject)
    {
      this.m_securedObject = securedObject;
      this.Id = teamRef.Id;
      this.Name = teamRef.Name;
      this.Url = teamRef.Url;
    }

    [DataMember(Order = 1)]
    public Guid Id { get; set; }

    [DataMember(Order = 2)]
    public string Name { get; set; }

    [DataMember(Order = 3)]
    public string Url { get; set; }

    Guid ISecuredObject.NamespaceId
    {
      get
      {
        ArgumentUtility.CheckForNull<ISecuredObject>(this.m_securedObject, "m_securedObject");
        return this.m_securedObject.NamespaceId;
      }
    }

    int ISecuredObject.RequiredPermissions
    {
      get
      {
        ArgumentUtility.CheckForNull<ISecuredObject>(this.m_securedObject, "m_securedObject");
        return this.m_securedObject.RequiredPermissions;
      }
    }

    string ISecuredObject.GetToken()
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(this.m_securedObject, "m_securedObject");
      return this.m_securedObject.GetToken();
    }
  }
}
