// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.FeedSecuredObject
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  [DataContract]
  public class FeedSecuredObject : ISecuredObject
  {
    private static readonly Guid FeedSecurityNamespaceId = new Guid("{9FED0191-DCA2-4112-86B7-A6A48D1B204C}");
    internal Guid NamespaceId = FeedSecuredObject.FeedSecurityNamespaceId;
    internal int requiredPermissions;
    internal string token;

    Guid ISecuredObject.NamespaceId => this.NamespaceId;

    int ISecuredObject.RequiredPermissions => this.requiredPermissions;

    string ISecuredObject.GetToken() => this.token;
  }
}
