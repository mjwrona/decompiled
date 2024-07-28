// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.PackagingSecuredObject
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9764DF62-33FE-41B6-9E79-DE201B497BE0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public abstract class PackagingSecuredObject : ISecuredObject
  {
    private ISecuredObject securedObject;

    public PackagingSecuredObject()
    {
    }

    public PackagingSecuredObject(ISecuredObject securedObject) => this.securedObject = securedObject;

    public virtual void SetSecuredObject(ISecuredObject securedObject) => this.securedObject = securedObject;

    [IgnoreDataMember]
    Guid ISecuredObject.NamespaceId
    {
      get
      {
        this.CheckForNullSecuredObject();
        return this.securedObject.NamespaceId;
      }
    }

    [IgnoreDataMember]
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
