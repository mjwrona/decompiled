// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.ProjectScopedSecuredSetting
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  public abstract class ProjectScopedSecuredSetting : ISecuredObject
  {
    protected string token;
    private Guid namespaceId;
    private int requiredPermissions;
    private string tokenName;
    private string rootToken;

    public ProjectScopedSecuredSetting(Guid namespaceId, int requiredPermissions, string token)
    {
      this.namespaceId = namespaceId;
      this.requiredPermissions = requiredPermissions;
      this.token = token;
    }

    public ProjectScopedSecuredSetting(
      Guid namespaceId,
      int requiredPermissions,
      string rootToken,
      string tokenName)
    {
      this.namespaceId = namespaceId;
      this.requiredPermissions = requiredPermissions;
      this.rootToken = rootToken;
      this.tokenName = tokenName;
    }

    Guid ISecuredObject.NamespaceId => this.namespaceId;

    int ISecuredObject.RequiredPermissions => this.requiredPermissions;

    string ISecuredObject.GetToken() => this.token;

    public void Secure(Guid projectId) => this.token = this.token ?? ProjectScopedSecuredSetting.CreateToken(projectId, this.rootToken, this.tokenName);

    public static string CreateToken(Guid projectId, string rootToken, string tokenName) => string.Format("/{0}/{1}/{2}", (object) rootToken, (object) projectId, (object) tokenName);
  }
}
