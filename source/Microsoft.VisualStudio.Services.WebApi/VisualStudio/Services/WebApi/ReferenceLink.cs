// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.ReferenceLink
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [DataContract]
  public class ReferenceLink : ISecuredObject
  {
    private readonly ISecuredObject m_securedObject;

    public ReferenceLink()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public ReferenceLink(ISecuredObject securedObject) => this.m_securedObject = securedObject;

    [DataMember]
    public string Href { get; set; }

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
