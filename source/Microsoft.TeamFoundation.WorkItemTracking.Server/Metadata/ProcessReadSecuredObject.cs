// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ProcessReadSecuredObject
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class ProcessReadSecuredObject : ISecuredObject
  {
    private int permission;
    private string token;
    private Guid namespaceId;

    internal ProcessReadSecuredObject()
    {
    }

    internal ProcessReadSecuredObject(Guid namespaceId, int permission, string token)
    {
      this.permission = permission;
      this.namespaceId = namespaceId;
      this.token = token;
    }

    public ProcessReadSecuredObject(ProcessReadSecuredObject securedObject)
    {
      if (securedObject == null)
        return;
      ISecuredObject securedObject1 = (ISecuredObject) securedObject;
      this.permission = securedObject1.RequiredPermissions;
      this.namespaceId = securedObject1.NamespaceId;
      this.token = securedObject1.GetToken();
    }

    public void SetSecuredToken(string token) => this.token = token;

    public ProcessReadSecuredObject SecureForProject(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ProcessReadSecuredObject processReadSecuredObject = (ProcessReadSecuredObject) null;
      if (requestContext.WitContext().ProcessReadPermissionChecker.HasProcessReadPermissionForProject(projectId, out processReadSecuredObject))
      {
        this.permission = ((ISecuredObject) processReadSecuredObject).RequiredPermissions;
        this.namespaceId = ((ISecuredObject) processReadSecuredObject).NamespaceId;
        this.token = ((ISecuredObject) processReadSecuredObject).GetToken();
      }
      return processReadSecuredObject;
    }

    internal static ProcessReadSecuredObject GetSecuredObject(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return new ProcessReadSecuredObject().SecureForProject(requestContext, projectId);
    }

    internal void SetRequiredPermissions(int requiredPermissions) => this.permission = requiredPermissions;

    internal void SetNamespaceId(Guid namespaceId) => this.namespaceId = namespaceId;

    internal virtual void SetSecuredObjectProperties(ProcessReadSecuredObject securedObject)
    {
      if (securedObject == null)
        return;
      this.SetNamespaceId(securedObject.namespaceId);
      this.SetRequiredPermissions(((ISecuredObject) securedObject).RequiredPermissions);
      this.SetSecuredToken(securedObject.token);
    }

    Guid ISecuredObject.NamespaceId => this.namespaceId;

    int ISecuredObject.RequiredPermissions => this.permission;

    string ISecuredObject.GetToken() => this.token;
  }
}
