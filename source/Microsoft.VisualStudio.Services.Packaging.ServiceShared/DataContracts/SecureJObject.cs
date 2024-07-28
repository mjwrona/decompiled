// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.SecureJObject
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts
{
  public class SecureJObject : JObject, ISecuredObject
  {
    private readonly string token;

    public SecureJObject()
    {
    }

    public SecureJObject(JObject other, ISecuredObject securedObject)
      : base(other)
    {
      if (securedObject == null)
        return;
      this.NamespaceId = securedObject.NamespaceId;
      this.RequiredPermissions = securedObject.RequiredPermissions;
      this.token = securedObject.GetToken();
    }

    public Guid NamespaceId { get; }

    public int RequiredPermissions { get; }

    public string GetToken() => this.token;
  }
}
