// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.WorkItemSecuredObject
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public class WorkItemSecuredObject : ISecuredObject
  {
    private int permission;
    private string token;

    internal WorkItemSecuredObject()
    {
    }

    public WorkItemSecuredObject(int permission)
      : this(permission, (string) null)
    {
    }

    public WorkItemSecuredObject(int permission, string token)
    {
      this.permission = permission;
      this.token = token;
    }

    public void SetSecuredToken(string token) => this.token = token;

    internal void SetRequiredPermissions(int requiredPermissions) => this.permission = requiredPermissions;

    Guid ISecuredObject.NamespaceId => AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid;

    int ISecuredObject.RequiredPermissions => this.permission;

    string ISecuredObject.GetToken() => this.token;
  }
}
