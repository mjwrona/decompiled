// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmAutoTriggerIssuesController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(4.0)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "autotriggerissues")]
  public class RmAutoTriggerIssuesController : ReleaseManagementProjectControllerBase
  {
    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    public IEnumerable<AutoTriggerIssue> GetAutoTriggerIssues(
      string artifactType,
      string sourceId,
      string artifactVersionId)
    {
      if (string.IsNullOrWhiteSpace(artifactType))
        throw new ArgumentNullException(nameof (artifactType));
      if (string.IsNullOrWhiteSpace(sourceId))
        throw new ArgumentNullException(nameof (sourceId));
      if (string.IsNullOrWhiteSpace(artifactVersionId))
        throw new ArgumentNullException(nameof (artifactVersionId));
      return (IEnumerable<AutoTriggerIssue>) this.GetFilteredAutoTriggerIssues((IList<AutoTriggerIssue>) this.TfsRequestContext.GetService<ReleasesService>().GetAutoTriggerIssues(this.TfsRequestContext, this.ProjectId, artifactType, artifactVersionId, sourceId).ToAutoTriggerIssuesContract(this.TfsRequestContext).ToList<AutoTriggerIssue>());
    }

    private IList<AutoTriggerIssue> GetFilteredAutoTriggerIssues(
      IList<AutoTriggerIssue> autoTriggerIssues)
    {
      if (autoTriggerIssues.IsNullOrEmpty<AutoTriggerIssue>())
        return autoTriggerIssues;
      IList<AutoTriggerIssue> collection = (IList<AutoTriggerIssue>) new List<AutoTriggerIssue>();
      foreach (IGrouping<Guid, AutoTriggerIssue> grouping in autoTriggerIssues.GroupBy<AutoTriggerIssue, Guid>((Func<AutoTriggerIssue, Guid>) (i => i.Project.Id)))
      {
        Guid projectId = grouping.First<AutoTriggerIssue>().Project.Id;
        IDictionary<int, string> folderPaths = ReleaseManagementSecurityProcessor.GetFolderPaths(this.TfsRequestContext, projectId, (IList<int>) grouping.Select<AutoTriggerIssue, int>((Func<AutoTriggerIssue, int>) (i => i.ReleaseDefinitionReference.Id)).ToList<int>());
        IList<AutoTriggerIssue> autoTriggerIssueList = ReleaseManagementSecurityProcessor.FilterComponents<AutoTriggerIssue>(this.TfsRequestContext, (IEnumerable<AutoTriggerIssue>) grouping, (Func<AutoTriggerIssue, ReleaseManagementSecurityInfo>) (a => ReleaseManagementSecurityProcessor.GetSecurityInfo(projectId, folderPaths[a.ReleaseDefinitionReference.Id], a.ReleaseDefinitionReference.Id, ReleaseManagementSecurityPermissions.ViewReleases)), true);
        if (autoTriggerIssueList.Any<AutoTriggerIssue>())
          collection.AddRange<AutoTriggerIssue, IList<AutoTriggerIssue>>((IEnumerable<AutoTriggerIssue>) autoTriggerIssueList);
      }
      return collection;
    }
  }
}
