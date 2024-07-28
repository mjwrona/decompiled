// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.QueryDualIntentSecuredObject
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi
{
  public class QueryDualIntentSecuredObject : DualIntentSecuredObject
  {
    public QueryDualIntentSecuredObject(Guid? filterUnderProjectId)
      : base(filterUnderProjectId)
    {
    }

    protected override Guid CollectionScopeNamespaceId => WitConstants.SecurityConstants.QueryItemSecurityNamespaceGuid;

    protected override Guid ProjectScopeNamespaceId => TeamProjectSecurityConstants.NamespaceId;

    protected override int CollectionScopeRequiredPermissions => 32;

    protected override int ProjectScopeRequiredPermissions => TeamProjectSecurityConstants.GenericRead;

    protected override string CollectionScopeToken => "$";

    protected override string ProjectScopeToken
    {
      get
      {
        ArgumentUtility.CheckForNull<Guid>(this.m_projectId, "m_projectId");
        return TeamProjectSecurityConstants.GetToken(ProjectInfo.GetProjectUri(this.m_projectId.Value));
      }
    }
  }
}
