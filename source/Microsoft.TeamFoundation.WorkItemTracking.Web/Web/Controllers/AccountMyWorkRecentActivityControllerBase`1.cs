// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.AccountMyWorkRecentActivityControllerBase`1
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.RecentActivity;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  public abstract class AccountMyWorkRecentActivityControllerBase<T> : TfsApiController
  {
    private static string[] s_pagedFields = new string[7]
    {
      CoreFieldReferenceNames.Id,
      CoreFieldReferenceNames.WorkItemType,
      CoreFieldReferenceNames.Title,
      CoreFieldReferenceNames.State,
      CoreFieldReferenceNames.AssignedTo,
      CoreFieldReferenceNames.TeamProject,
      CoreFieldReferenceNames.ChangedDate
    };

    protected IReadOnlyList<T> GetMyRecentWorkActivities()
    {
      using (PerformanceTimer.StartMeasure(this.TfsRequestContext, nameof (GetMyRecentWorkActivities)))
        return this.PageRecentActivityWorkItems(this.TfsRequestContext.GetService<ITeamFoundationRecentActivityService>().GetUserActivities(this.TfsRequestContext, this.TfsRequestContext.GetUserIdentity().Id, WorkItemArtifactKinds.WorkItem));
    }

    protected IReadOnlyList<T> PageRecentActivityWorkItems(
      IReadOnlyDictionary<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity> recentActivityDictionary)
    {
      using (PerformanceTimer.StartMeasure(this.TfsRequestContext, nameof (PageRecentActivityWorkItems)))
      {
        ITeamFoundationWorkItemService service = this.TfsRequestContext.GetService<ITeamFoundationWorkItemService>();
        List<int> list = recentActivityDictionary.Keys.Select<string, int>((Func<string, int>) (artifactId =>
        {
          int result = -1;
          int.TryParse(artifactId, out result);
          return result;
        })).Where<int>((Func<int, bool>) (id => id > 0)).ToList<int>();
        IFieldTypeDictionary fieldDict = this.TfsRequestContext.WitContext().FieldDictionary;
        IEnumerable<FieldEntry> fieldEntries = ((IEnumerable<string>) AccountMyWorkRecentActivityControllerBase<T>.s_pagedFields).Select<string, FieldEntry>((Func<string, FieldEntry>) (fname => fieldDict.GetField(fname)));
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        List<int> workItemIds = list;
        IEnumerable<FieldEntry> fields = fieldEntries;
        DateTime? asOf = new DateTime?();
        return (IReadOnlyList<T>) this.CreateRecentActivityPayload(service.GetWorkItemFieldValues(tfsRequestContext, (IEnumerable<int>) workItemIds, fields, asOf: asOf), recentActivityDictionary).ToList<T>();
      }
    }

    protected abstract IEnumerable<T> CreateRecentActivityPayload(
      IEnumerable<WorkItemFieldData> fieldValues,
      IReadOnlyDictionary<string, Microsoft.Azure.Boards.RecentActivity.RecentActivity> recentActivities);

    public override string ActivityLogArea => "WorkItem Tracking";
  }
}
