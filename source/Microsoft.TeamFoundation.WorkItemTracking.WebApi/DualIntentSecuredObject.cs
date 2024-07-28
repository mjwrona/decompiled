// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.DualIntentSecuredObject
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi
{
  public abstract class DualIntentSecuredObject : ISecuredObject
  {
    protected Guid? m_projectId;
    protected bool m_isCollectionApi;

    public DualIntentSecuredObject(Guid? filterUnderProjectId)
    {
      this.m_isCollectionApi = !filterUnderProjectId.HasValue || filterUnderProjectId.Value == Guid.Empty;
      this.m_projectId = filterUnderProjectId;
    }

    Guid ISecuredObject.NamespaceId => !this.m_isCollectionApi ? this.ProjectScopeNamespaceId : this.CollectionScopeNamespaceId;

    int ISecuredObject.RequiredPermissions => !this.m_isCollectionApi ? this.ProjectScopeRequiredPermissions : this.CollectionScopeRequiredPermissions;

    string ISecuredObject.GetToken() => !this.m_isCollectionApi ? this.ProjectScopeToken : this.CollectionScopeToken;

    protected abstract Guid CollectionScopeNamespaceId { get; }

    protected abstract Guid ProjectScopeNamespaceId { get; }

    protected abstract int CollectionScopeRequiredPermissions { get; }

    protected abstract int ProjectScopeRequiredPermissions { get; }

    protected abstract string CollectionScopeToken { get; }

    protected abstract string ProjectScopeToken { get; }
  }
}
