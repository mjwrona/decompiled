// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.SecuredDictionary`2
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server
{
  public class SecuredDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISecuredObject
  {
    private ISecuredObject securedObject;

    public SecuredDictionary()
    {
    }

    public SecuredDictionary(ISecuredObject securedObject) => this.securedObject = securedObject;

    public virtual void SetSecuredObject(ISecuredObject securedObject) => this.securedObject = securedObject;

    Guid ISecuredObject.NamespaceId
    {
      get
      {
        this.CheckForNullSecuredObject();
        return this.securedObject.NamespaceId;
      }
    }

    int ISecuredObject.RequiredPermissions
    {
      get
      {
        this.CheckForNullSecuredObject();
        return this.securedObject.RequiredPermissions;
      }
    }

    string ISecuredObject.GetToken()
    {
      this.CheckForNullSecuredObject();
      return this.securedObject.GetToken();
    }

    private void CheckForNullSecuredObject() => ArgumentUtility.CheckForNull<ISecuredObject>(this.securedObject, "securedObject");
  }
}
