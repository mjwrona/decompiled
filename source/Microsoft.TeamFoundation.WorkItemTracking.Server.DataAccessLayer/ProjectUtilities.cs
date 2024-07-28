// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProjectUtilities
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal static class ProjectUtilities
  {
    private const string c_projectAdminsGroupCacheKey = "ProjectAdminsGroup";
    private const string c_projectGroupCacheKeyPrefix = "ProjectGroup";

    public static string GetProjectScopedGroupCacheKey(string projectUri, string groupName) => ("ProjectGroup_" + projectUri + "_" + groupName).ToLower();

    public static Microsoft.VisualStudio.Services.Identity.Identity GetProjectAdministratorsGroup(
      IVssRequestContext requestContext,
      string projectUri)
    {
      string scopedGroupCacheKey = ProjectUtilities.GetProjectScopedGroupCacheKey(projectUri, "ProjectAdminsGroup");
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity;
      if (!requestContext.TryGetItem<Microsoft.VisualStudio.Services.Identity.Identity>(scopedGroupCacheKey, out readIdentity))
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        Guid id = service.GetScope(requestContext, ProjectInfo.GetProjectId(projectUri)).Id;
        readIdentity = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          IdentityDomain.MapFromWellKnownIdentifier(id, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup)
        }, QueryMembership.None, (IEnumerable<string>) null)[0];
        requestContext.Items[scopedGroupCacheKey] = (object) readIdentity;
      }
      return readIdentity;
    }
  }
}
