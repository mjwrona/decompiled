// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.EtagHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Utilities;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public static class EtagHelper
  {
    private const string m_metadataDBStampsKey = "WITMetadataDbStamp";
    private const string m_workItemTypesMetadataStampKey = "WorkItemTypesMetadataStamp";
    private const string c_newWebPlatformVersion = "2.2";
    private const string c_oldWebPlatformVersion = "1.2";

    public static string GetLinkTypesETag(IVssRequestContext tfsRequestContext)
    {
      string str = CommonWITUtils.IsRemoteLinkingEnabled(tfsRequestContext) ? "-remotelinks" : "";
      return EtagHelper.CalculateMetadataETag((IEnumerable<MetadataTable>) new MetadataTable[1]
      {
        MetadataTable.LinkTypes
      }, tfsRequestContext) + str;
    }

    public static string GetNodesETag(IVssRequestContext tfsRequestContext) => EtagHelper.CalculateMetadataETag((IEnumerable<MetadataTable>) new MetadataTable[1], tfsRequestContext);

    public static string GetConstantSetsETag(IVssRequestContext tfsRequestContext) => EtagHelper.CalculateMetadataETag((IEnumerable<MetadataTable>) new MetadataTable[2]
    {
      MetadataTable.ConstantsNonIdentity,
      MetadataTable.ConstantSetsNonIdentity
    }, tfsRequestContext);

    public static string GetAllowedValuesETag(IVssRequestContext tfsRequestContext)
    {
      IList<MetadataTable> tables = (IList<MetadataTable>) new List<MetadataTable>()
      {
        MetadataTable.ConstantsNonIdentity,
        MetadataTable.ConstantSetsNonIdentity,
        MetadataTable.Rules
      };
      if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(tfsRequestContext))
        tables.Add(MetadataTable.Typelet);
      return EtagHelper.CalculateMetadataETag((IEnumerable<MetadataTable>) tables, tfsRequestContext);
    }

    public static string GetWorkItemTypeCategoriesETag(IVssRequestContext tfsRequestContext)
    {
      List<MetadataTable> tables = new List<MetadataTable>()
      {
        MetadataTable.WorkItemTypeCategories,
        MetadataTable.WorkItemTypeCategoryMembers
      };
      if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(tfsRequestContext))
        tables.Add(MetadataTable.Typelet);
      return EtagHelper.CalculateMetadataETag((IEnumerable<MetadataTable>) tables, tfsRequestContext) + (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(tfsRequestContext) ? "nc" : string.Empty);
    }

    public static string GetFieldsETag(IVssRequestContext tfsRequestContext) => EtagHelper.CalculateMetadataETag((IEnumerable<MetadataTable>) new List<MetadataTable>()
    {
      MetadataTable.Fields,
      MetadataTable.FieldUsages,
      MetadataTable.Rules
    }, tfsRequestContext) + (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(tfsRequestContext) ? "nf" : string.Empty);

    public static string GetTeamProjectsETag(
      IEnumerable<Project> projects,
      IVssRequestContext tfsRequestContext)
    {
      int num = tfsRequestContext.UserContext != (IdentityDescriptor) null ? 1 : 0;
      List<MetadataTable> tables = new List<MetadataTable>()
      {
        MetadataTable.Hierarchy,
        MetadataTable.Rules,
        MetadataTable.WorkItemTypes,
        MetadataTable.WorkItemTypeUsages,
        MetadataTable.Fields
      };
      if (EtagHelper.IsAddFieldEnabled(tfsRequestContext))
        tables.Add(MetadataTable.Typelet);
      string teamProjectsEtag = EtagHelper.CalculateMetadataETag((IEnumerable<MetadataTable>) tables, tfsRequestContext) + "-" + EtagHelper.CalculateProjectsTag(projects, tfsRequestContext);
      if (num == 0)
        teamProjectsEtag += "-restricted";
      return teamProjectsEtag;
    }

    public static string GetWorkItemTypesETag(
      IVssRequestContext tfsRequestContext,
      Guid projectGuid)
    {
      string workItemTypesEtag1 = (string) null;
      if (tfsRequestContext.Items.TryGetValue<string, string>("WorkItemTypesMetadataStamp", out workItemTypesEtag1))
        return workItemTypesEtag1;
      IList<MetadataTable> tables = (IList<MetadataTable>) new List<MetadataTable>()
      {
        MetadataTable.Hierarchy,
        MetadataTable.HierarchyProperties,
        MetadataTable.Rules,
        MetadataTable.WorkItemTypes,
        MetadataTable.WorkItemTypeUsages,
        MetadataTable.ConstantsNonIdentity,
        MetadataTable.ConstantSetsNonIdentity,
        MetadataTable.Fields
      };
      tables.Add(MetadataTable.NewWitFormLayout);
      if (EtagHelper.IsAddFieldEnabled(tfsRequestContext))
        tables.Add(MetadataTable.Typelet);
      string workItemTypesEtag2 = EtagHelper.CalculateMetadataETag((IEnumerable<MetadataTable>) tables, tfsRequestContext) + "-" + EtagHelper.CalculateProcessTagForProject(projectGuid, tfsRequestContext) + "-v3" + EtagHelper.CalculateContributionsETag(tfsRequestContext).ToString() + "-stateColors" + EtagHelper.GetStateColorETagPart(tfsRequestContext, projectGuid) + EtagHelper.GetForNotRulesETagPart(tfsRequestContext) + EtagHelper.GetForNotRulesETagPartPhase2(tfsRequestContext, projectGuid) + (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(tfsRequestContext) ? "nw" : string.Empty) + (CommonWITUtils.HasReadRulesPermission(tfsRequestContext) ? "wr" : "") + (CommonWITUtils.IsRemoteLinkingEnabled(tfsRequestContext) ? "-r" : "") + CommonWITUtils.GetOOBProcessChangedETag(tfsRequestContext, projectGuid) + "-n-sc";
      tfsRequestContext.Items.Add("WorkItemTypesMetadataStamp", (object) workItemTypesEtag2);
      return workItemTypesEtag2;
    }

    public static string GetStateColorETagPart(
      IVssRequestContext tfsRequestContext,
      Guid projectGuid)
    {
      return tfsRequestContext.GetService<IWorkItemMetadataFacadeService>().GetWorkItemStateColorETag(tfsRequestContext, projectGuid);
    }

    public static string GetWorkItemTypeExtensionETag(
      IEnumerable<WorkItemTypeExtension> extensions,
      IVssRequestContext tfsRequestContext)
    {
      return EtagHelper.CalculateMetadataETag((IEnumerable<MetadataTable>) new List<MetadataTable>()
      {
        MetadataTable.Fields
      }, tfsRequestContext) + "-" + extensions.OrderBy<WorkItemTypeExtension, Guid>((Func<WorkItemTypeExtension, Guid>) (x => x.Id)).Select<WorkItemTypeExtension, string>((Func<WorkItemTypeExtension, string>) (e => Convert.ToString(e.LastChangedDate.Ticks, 16))).StringJoin<string>('-');
    }

    public static string CalculateMetadataETag(
      IEnumerable<MetadataTable> tables,
      IVssRequestContext tfsRequestContext)
    {
      string str = "";
      if (tables != null && tables.Any<MetadataTable>())
        str = EtagHelper.GetMetadataTableStamps(tfsRequestContext).SubSet(tables).OrderBy<KeyValuePair<MetadataTable, long>, MetadataTable>((Func<KeyValuePair<MetadataTable, long>, MetadataTable>) (pair => pair.Key)).Select<KeyValuePair<MetadataTable, long>, long>((Func<KeyValuePair<MetadataTable, long>, long>) (pair => pair.Value)).Concat<long>((IEnumerable<long>) new long[1]
        {
          (long) tfsRequestContext.AuthenticatedUserName.GetStableHashCode()
        }).Select<long, string>((Func<long, string>) (stamp => Convert.ToString(stamp, 16))).StringJoin<string>('-');
      string fullTextEtag = EtagHelper.GetFullTextETag(tfsRequestContext);
      return "2.2" + "-" + str + fullTextEtag;
    }

    public static MetadataDBStamps GetMetadataTableStamps(IVssRequestContext tfsRequestContext)
    {
      MetadataDBStamps metadataTableStamps = (MetadataDBStamps) null;
      if (tfsRequestContext.Items.TryGetValue<string, MetadataDBStamps>("WITMetadataDbStamp", out metadataTableStamps))
        return metadataTableStamps;
      MetadataDBStamps metadataTableTimestamps = tfsRequestContext.GetService<WebAccessWorkItemService>().GetMetadataTableTimestamps(tfsRequestContext);
      tfsRequestContext.Items.Add("WITMetadataDbStamp", (object) metadataTableTimestamps);
      return metadataTableTimestamps;
    }

    public static bool IsAddFieldEnabled(IVssRequestContext requestContext) => WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext);

    public static string CalculateProjectsTag(
      IEnumerable<Project> projects,
      IVssRequestContext requestContext)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (Project project in (IEnumerable<Project>) projects.OrderBy<Project, int>((Func<Project, int>) (p => p.Id)))
      {
        stringBuilder.Append(Convert.ToString(project.Id, 16));
        if (EtagHelper.IsAddFieldEnabled(requestContext))
          stringBuilder.Append(project.ProcessTemplateId.GetHashCode().ToString());
      }
      stringBuilder.Append("-v2");
      return stringBuilder.ToString();
    }

    public static string CalculateProcessTagForProject(
      Guid projectId,
      IVssRequestContext requestContext)
    {
      string processTagForProject = "0";
      ProcessDescriptor processDescriptor;
      if (!EtagHelper.IsAddFieldEnabled(requestContext) || !requestContext.GetService<IWorkItemTrackingProcessService>().TryGetLatestProjectProcessDescriptor(requestContext, projectId, out processDescriptor))
        return processTagForProject;
      processTagForProject = processDescriptor.RowId.GetHashCode().ToString();
      return processTagForProject;
    }

    private static int CalculateContributionsETag(IVssRequestContext requestContext)
    {
      IEnumerable<Contribution> source = requestContext.GetService<IContributionService>().QueryContributions(requestContext, (IEnumerable<string>) new string[1]
      {
        "ms.vss-work-web.work-item-form"
      }, queryOptions: ContributionQueryOptions.IncludeChildren);
      int result = 0;
      int contributionsEtag = 0;
      List<string> values = new List<string>();
      foreach (Contribution contribution in source.Where<Contribution>((Func<Contribution, bool>) (c => ((IEnumerable<string>) WorkItemFormExtensionsConstants.ValidFormContributionTypes).Contains<string>(c.Type, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase))))
      {
        int num = contributionsEtag;
        string id = contribution.Id;
        int stableHashCode = id != null ? id.GetStableHashCode() : 0;
        contributionsEtag = num ^ stableHashCode;
        contributionsEtag ^= contribution.Type.GetStableHashCode();
        JToken jtoken;
        if (contribution.Properties != null && contribution.Properties.TryGetValue("height", out jtoken) && int.TryParse(jtoken.ToString(), out result))
          contributionsEtag ^= result;
        values.Add(string.Format("{0};{1};{2};{3}", (object) contributionsEtag, (object) contribution.Id, (object) contribution.Type, (object) result));
      }
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("Contributions", values);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "WIT", "ContributionEtags", properties);
      return contributionsEtag;
    }

    public static Dictionary<string, string> GetWorkItemMetadataCacheStamps(
      IVssRequestContext requestContext,
      Guid projectGuid)
    {
      IEnumerable<Project> projects = requestContext.GetService<WebAccessWorkItemService>().GetProjects(requestContext, true);
      return new Dictionary<string, string>()
      {
        {
          "LinkTypes",
          EtagHelper.GetProcessedEtag(requestContext, EtagHelper.GetLinkTypesETag(requestContext))
        },
        {
          "WorkItemTypes",
          EtagHelper.GetProcessedEtag(requestContext, EtagHelper.GetWorkItemTypesETag(requestContext, projectGuid))
        },
        {
          "Nodes",
          EtagHelper.GetProcessedEtag(requestContext, EtagHelper.GetNodesETag(requestContext))
        },
        {
          "Fields",
          EtagHelper.GetProcessedEtag(requestContext, EtagHelper.GetFieldsETag(requestContext))
        },
        {
          "ConstantSets",
          EtagHelper.GetProcessedEtag(requestContext, EtagHelper.GetConstantSetsETag(requestContext))
        },
        {
          "AllowedValues",
          EtagHelper.GetProcessedEtag(requestContext, EtagHelper.GetAllowedValuesETag(requestContext))
        },
        {
          "WorkItemTypeCategories",
          EtagHelper.GetProcessedEtag(requestContext, EtagHelper.GetWorkItemTypeCategoriesETag(requestContext))
        },
        {
          "TeamProjects",
          EtagHelper.GetProcessedEtag(requestContext, EtagHelper.GetTeamProjectsETag(projects, requestContext))
        },
        {
          "AdhocQueries",
          EtagHelper.GetProcessedEtag(requestContext, AdhocQueryProvider.Etags.GetAdHocQueriesETag(requestContext, projectGuid))
        }
      };
    }

    public static string GetProcessedEtag(IVssRequestContext requestContext, string etag)
    {
      etag = ConfigurationManager.AppSettings["webApiVersion"].ToString((IFormatProvider) NumberFormatInfo.InvariantInfo) + "-" + etag;
      if (etag.Length > 256)
        etag = Convert.ToBase64String(MD5Util.CalculateMD5(Encoding.UTF8.GetBytes(etag)));
      return etag;
    }

    private static string GetForNotRulesETagPart(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || WorkItemTrackingFeatureFlags.IsInheritedProcessCustomizationOnlyAccount(requestContext))
        return string.Empty;
      using (requestContext.TraceBlock(290753, 290754, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, nameof (GetForNotRulesETagPart)))
        return "-gm" + string.Concat<int>(((IEnumerable<bool>) requestContext.GetService<ForNotGroupMembershipService>().GetForNotGroupMembership(requestContext)).Select<bool, int>((Func<bool, int>) (b => b ? 1 : 0)));
    }

    private static string GetForNotRulesETagPartPhase2(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      using (requestContext.TraceBlock(290772, 290773, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, nameof (GetForNotRulesETagPartPhase2)))
        return "-gm" + string.Concat<int>(((IEnumerable<bool>) requestContext.GetService<ForNotGroupMembershipService>().GetForNotGroupMembershipForPhase2(requestContext, projectId)).Select<bool, int>((Func<bool, int>) (b => b ? 1 : 0)));
    }

    private static string GetFullTextETag(IVssRequestContext requestContext) => !requestContext.WitContext().ServerSettings.FullTextEnabled ? "-ftd" : string.Empty;
  }
}
