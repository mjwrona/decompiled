// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.AccountRecentMentionsController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Mention.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "accountRecentMentions", ResourceVersion = 1)]
  [ClientInternalUseOnly(true)]
  public class AccountRecentMentionsController : TfsApiController
  {
    private const int MaxWorkItems = 200;
    private static readonly string[] s_pagedFields = new string[6]
    {
      CoreFieldReferenceNames.Id,
      CoreFieldReferenceNames.WorkItemType,
      CoreFieldReferenceNames.Title,
      CoreFieldReferenceNames.State,
      CoreFieldReferenceNames.AssignedTo,
      CoreFieldReferenceNames.TeamProject
    };

    [HttpGet]
    [ClientLocationId("D60EEB6E-E18C-4478-9E94-A0094E28F41C")]
    [ClientResponseType(typeof (IList<AccountRecentMentionWorkItemModel>), null, null)]
    public HttpResponseMessage GetRecentMentions()
    {
      IReadOnlyCollection<AccountRecentMentionWorkItemModel> mentionWorkItemModels = (IReadOnlyCollection<AccountRecentMentionWorkItemModel>) null;
      PerformanceScenarioHelper performanceScenarioHelper = new PerformanceScenarioHelper(this.TfsRequestContext, "Agile", "AccountMyWorkController.GetRecentMentions");
      IDictionary<string, PerformanceTimingGroup> allTimings = PerformanceTimer.GetAllTimings(this.TfsRequestContext);
      performanceScenarioHelper.Add("WebServerTimings", (object) allTimings);
      try
      {
        mentionWorkItemModels = (IReadOnlyCollection<AccountRecentMentionWorkItemModel>) this.GetRecentMentionsData();
      }
      finally
      {
        performanceScenarioHelper.EndScenario();
      }
      return this.Request.CreateResponse<IReadOnlyCollection<AccountRecentMentionWorkItemModel>>(HttpStatusCode.OK, mentionWorkItemModels);
    }

    private IReadOnlyList<AccountRecentMentionWorkItemModel> GetRecentMentionsData()
    {
      using (PerformanceTimer.StartMeasure(this.TfsRequestContext, nameof (GetRecentMentionsData)))
      {
        IEnumerable<LastMentionedInfo> mentionsForCurrentUser = this.TfsRequestContext.GetService<ITeamFoundationMentionService>().GetRecentMentionsForCurrentUser(this.TfsRequestContext, new Guid?(), "WorkItem", 200);
        return this.PageRecentMentionWorkItems(mentionsForCurrentUser.Select<LastMentionedInfo, string>((Func<LastMentionedInfo, string>) (m => m.NormalizedSourceId)).Select<string, int>((Func<string, int>) (@string =>
        {
          int result;
          int.TryParse(@string, out result);
          return result;
        })).Where<int>((Func<int, bool>) (id => id > 0)), mentionsForCurrentUser);
      }
    }

    private IReadOnlyList<AccountRecentMentionWorkItemModel> PageRecentMentionWorkItems(
      IEnumerable<int> workItemIds,
      IEnumerable<LastMentionedInfo> lastMentionedInfo)
    {
      using (PerformanceTimer.StartMeasure(this.TfsRequestContext, nameof (PageRecentMentionWorkItems)))
      {
        ITeamFoundationWorkItemService service = this.TfsRequestContext.GetService<ITeamFoundationWorkItemService>();
        IFieldTypeDictionary fieldDict = this.TfsRequestContext.WitContext().FieldDictionary;
        IEnumerable<FieldEntry> fieldEntries = ((IEnumerable<string>) AccountRecentMentionsController.s_pagedFields).Select<string, FieldEntry>((Func<string, FieldEntry>) (fname => fieldDict.GetField(fname)));
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        IEnumerable<int> workItemIds1 = workItemIds;
        IEnumerable<FieldEntry> fields = fieldEntries;
        DateTime? asOf = new DateTime?();
        return (IReadOnlyList<AccountRecentMentionWorkItemModel>) service.GetWorkItemFieldValues(tfsRequestContext, workItemIds1, fields, asOf: asOf).Join<WorkItemFieldData, LastMentionedInfo, string, AccountRecentMentionWorkItemModel>(lastMentionedInfo, (Func<WorkItemFieldData, string>) (fieldData => fieldData.Id.ToString()), (Func<LastMentionedInfo, string>) (mention => mention.NormalizedSourceId), (Func<WorkItemFieldData, LastMentionedInfo, AccountRecentMentionWorkItemModel>) ((fieldData, mention) => new AccountRecentMentionWorkItemModel()
        {
          AssignedTo = fieldData.AssignedTo,
          Id = fieldData.Id,
          State = fieldData.State,
          Title = fieldData.Title,
          WorkItemType = fieldData.WorkItemType,
          TeamProject = fieldData.GetProjectName(this.TfsRequestContext),
          MentionedDateField = mention.LastMentionedDate
        })).OrderByDescending<AccountRecentMentionWorkItemModel, DateTime>((Func<AccountRecentMentionWorkItemModel, DateTime>) (item => item.MentionedDateField)).ToList<AccountRecentMentionWorkItemModel>();
      }
    }
  }
}
