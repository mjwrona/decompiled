// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.ProjectScopedFeedSecuredObject
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public class ProjectScopedFeedSecuredObject : ISecuredObject
  {
    private readonly Guid namespaceId;
    private readonly int requiredPermissions;
    private readonly string token;

    public ProjectScopedFeedSecuredObject(Guid namespaceId, int requiredPermissions, string token)
    {
      this.namespaceId = namespaceId;
      this.requiredPermissions = requiredPermissions;
      this.token = token;
    }

    string ISecuredObject.GetToken() => this.token;

    Guid ISecuredObject.NamespaceId => this.namespaceId;

    int ISecuredObject.RequiredPermissions => this.requiredPermissions;
  }
}
