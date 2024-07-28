// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.SecuredDictionary`2
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [CollectionDataContract]
  public class SecuredDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISecuredObject
  {
    private ISecuredObject m_securedObject;

    public SecuredDictionary()
    {
    }

    public SecuredDictionary(ISecuredObject securedObject) => this.m_securedObject = securedObject;

    public void SetSecuredObject(ISecuredObject securedObject) => this.m_securedObject = securedObject;

    [IgnoreDataMember]
    Guid ISecuredObject.NamespaceId
    {
      get
      {
        this.CheckForNullSecuredObject();
        return this.m_securedObject.NamespaceId;
      }
    }

    [IgnoreDataMember]
    int ISecuredObject.RequiredPermissions
    {
      get
      {
        this.CheckForNullSecuredObject();
        return this.m_securedObject.RequiredPermissions;
      }
    }

    string ISecuredObject.GetToken()
    {
      this.CheckForNullSecuredObject();
      return this.m_securedObject.GetToken();
    }

    private void CheckForNullSecuredObject() => ArgumentUtility.CheckForNull<ISecuredObject>(this.m_securedObject, "m_securedObject");
  }
}
