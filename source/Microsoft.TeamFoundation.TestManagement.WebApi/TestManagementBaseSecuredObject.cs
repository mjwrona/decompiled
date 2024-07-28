// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestManagementBaseSecuredObject
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public abstract class TestManagementBaseSecuredObject : ISecuredObject
  {
    private ISecuredObject securedObject;

    public TestManagementBaseSecuredObject()
    {
    }

    public TestManagementBaseSecuredObject(ISecuredObject securedObject) => this.securedObject = securedObject;

    Guid ISecuredObject.NamespaceId
    {
      get
      {
        ArgumentUtility.CheckForNull<ISecuredObject>(this.securedObject, "securedObject");
        return this.securedObject.NamespaceId;
      }
    }

    int ISecuredObject.RequiredPermissions
    {
      get
      {
        ArgumentUtility.CheckForNull<ISecuredObject>(this.securedObject, "securedObject");
        return this.securedObject.RequiredPermissions;
      }
    }

    string ISecuredObject.GetToken()
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(this.securedObject, "securedObject");
      return this.securedObject.GetToken();
    }

    internal bool IsSecured() => this.securedObject != null;

    internal virtual void InitializeSecureObject(ISecuredObject securedObject)
    {
      if (this.IsSecured())
        return;
      this.securedObject = securedObject;
    }

    protected void SecureIdentityRef(IdentityRef identity)
    {
      if (identity == null || identity.Links == null || identity.Links.Links == null)
        return;
      ReferenceLinks referenceLinks = new ReferenceLinks();
      foreach (KeyValuePair<string, object> link in (IEnumerable<KeyValuePair<string, object>>) identity.Links.Links)
        referenceLinks.AddLink(link.Key, link.Value is ReferenceLink referenceLink ? referenceLink.Href : (string) null, (ISecuredObject) identity);
      identity.Links = referenceLinks;
    }
  }
}
