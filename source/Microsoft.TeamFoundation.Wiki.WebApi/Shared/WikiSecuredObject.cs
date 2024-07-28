// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.WebApi.Shared.WikiSecuredObject
// Assembly: Microsoft.TeamFoundation.Wiki.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A8C8A50-70A8-447A-B2AD-300BEAACF074
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Wiki.WebApi.Shared
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public abstract class WikiSecuredObject : ISecuredObject
  {
    private ISecuredObject m_securedObject;

    public virtual void SetSecuredObject(ISecuredObject securedObject) => this.m_securedObject = securedObject;

    [IgnoreDataMember]
    Guid ISecuredObject.NamespaceId
    {
      get
      {
        ArgumentUtility.CheckForNull<ISecuredObject>(this.m_securedObject, "m_securedObject");
        return this.m_securedObject.NamespaceId;
      }
    }

    [IgnoreDataMember]
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
