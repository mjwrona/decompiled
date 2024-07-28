// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ConstantIdentityRef
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class ConstantIdentityRef : IdentityRef, ISecuredObject
  {
    private int permission;
    private string token;
    private Guid namespaceId;

    public ConstantIdentityRef(string token, int permission, Guid namespaceId)
    {
      this.token = token;
      this.permission = permission;
      this.namespaceId = namespaceId;
    }

    public ConstantIdentityRef(ISecuredObject secureInfoSourceObject)
    {
      if (secureInfoSourceObject == null)
        return;
      this.token = secureInfoSourceObject.GetToken();
      this.permission = secureInfoSourceObject.RequiredPermissions;
      this.namespaceId = secureInfoSourceObject.NamespaceId;
    }

    Guid ISecuredObject.NamespaceId => this.namespaceId;

    int ISecuredObject.RequiredPermissions => this.permission;

    string ISecuredObject.GetToken() => this.token;
  }
}
