// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.BacklogConfigurationExtensions
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web
{
  internal static class BacklogConfigurationExtensions
  {
    public static Microsoft.TeamFoundation.Work.WebApi.BacklogConfiguration Convert(
      this Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration,
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId)
    {
      WorkItemTrackingRequestContext witRequestContext = requestContext.WitContext();
      IWorkItemTypeService workItemTypeService = requestContext.GetService<IWorkItemTypeService>();
      return new Microsoft.TeamFoundation.Work.WebApi.BacklogConfiguration()
      {
        BacklogFields = new BacklogFields()
        {
          TypeFields = (IReadOnlyDictionary<string, string>) backlogConfiguration.BacklogFields.TypeFields.ToDictionary<KeyValuePair<FieldTypeEnum, string>, string, string>((Func<KeyValuePair<FieldTypeEnum, string>, string>) (kv => kv.Key.ToString()), (Func<KeyValuePair<FieldTypeEnum, string>, string>) (kv => kv.Value))
        },
        BugsBehavior = backlogConfiguration.BugsBehavior,
        HiddenBacklogs = backlogConfiguration.HiddenBacklogs,
        PortfolioBacklogs = (IReadOnlyCollection<Microsoft.TeamFoundation.Work.WebApi.BacklogLevelConfiguration>) backlogConfiguration.PortfolioBacklogs.Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration, Microsoft.TeamFoundation.Work.WebApi.BacklogLevelConfiguration>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration, Microsoft.TeamFoundation.Work.WebApi.BacklogLevelConfiguration>) (pb => pb.Convert(backlogConfiguration, witRequestContext, workItemTypeService, projectId))).ToList<Microsoft.TeamFoundation.Work.WebApi.BacklogLevelConfiguration>(),
        RequirementBacklog = backlogConfiguration.RequirementBacklog.Convert(backlogConfiguration, witRequestContext, workItemTypeService, projectId),
        TaskBacklog = backlogConfiguration.TaskBacklog.Convert(backlogConfiguration, witRequestContext, workItemTypeService, projectId),
        WorkItemTypeMappedStates = BacklogConfigurationExtensions.Convert(backlogConfiguration.WorkItemTypeMappedStates),
        Url = BacklogConfigurationExtensions.GetSelfUrl(witRequestContext.RequestContext, projectId, teamId),
        IsBugsBehaviorConfigured = backlogConfiguration.IsBugsBehaviorConfigValid
      };
    }

    public static Microsoft.TeamFoundation.Work.WebApi.BacklogLevelConfiguration Convert(
      this Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration backlogLevel,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration,
      WorkItemTrackingRequestContext witRequestContext,
      IWorkItemTypeService workItemTypeService,
      Guid projectId)
    {
      return new Microsoft.TeamFoundation.Work.WebApi.BacklogLevelConfiguration()
      {
        AddPanelFields = (IReadOnlyCollection<WorkItemFieldReference>) ((IEnumerable<string>) backlogLevel.AddPanelFields).Select<string, WorkItemFieldReference>((Func<string, WorkItemFieldReference>) (fieldRefName => BacklogConfigurationExtensions.GetFieldRererence(witRequestContext, fieldRefName, projectId))).ToList<WorkItemFieldReference>(),
        Color = backlogLevel.Color,
        ColumnFields = ((IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn>) backlogLevel.ColumnFields).Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn, Microsoft.TeamFoundation.Work.WebApi.BacklogColumn>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogColumn, Microsoft.TeamFoundation.Work.WebApi.BacklogColumn>) (cf => new Microsoft.TeamFoundation.Work.WebApi.BacklogColumn()
        {
          ColumnFieldReference = BacklogConfigurationExtensions.GetFieldRererence(witRequestContext, cf.ColumnReferenceName, projectId),
          Width = cf.Width
        })).ToArray<Microsoft.TeamFoundation.Work.WebApi.BacklogColumn>(),
        DefaultWorkItemType = string.IsNullOrEmpty(backlogLevel.DefaultWorkItemType) ? (WorkItemTypeReference) null : BacklogConfigurationExtensions.GetWorkItemTypeReference(witRequestContext, workItemTypeService, backlogLevel.DefaultWorkItemType, projectId),
        Id = backlogLevel.Id,
        IsHidden = !backlogConfiguration.IsBacklogVisible(backlogLevel.Id),
        Name = backlogLevel.Name,
        Rank = backlogLevel.Rank / 10,
        Type = BacklogConfigurationExtensions.GetBacklogLevelType(backlogConfiguration, backlogLevel.Id),
        WorkItemCountLimit = backlogLevel.WorkItemCountLimit,
        WorkItemTypes = (IReadOnlyCollection<WorkItemTypeReference>) backlogLevel.WorkItemTypes.Select<string, WorkItemTypeReference>((Func<string, WorkItemTypeReference>) (wit => BacklogConfigurationExtensions.GetWorkItemTypeReference(witRequestContext, workItemTypeService, wit, projectId))).ToList<WorkItemTypeReference>()
      };
    }

    private static BacklogType GetBacklogLevelType(
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration,
      string levelId)
    {
      if (TFStringComparer.BacklogLevelId.Equals(backlogConfiguration.RequirementBacklog.Id, levelId))
        return BacklogType.Requirement;
      return TFStringComparer.BacklogLevelId.Equals(backlogConfiguration.TaskBacklog.Id, levelId) ? BacklogType.Task : BacklogType.Portfolio;
    }

    private static string GetSelfUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      Dictionary<string, object> routeValues = new Dictionary<string, object>();
      routeValues["project"] = (object) projectId;
      routeValues["team"] = !(teamId != Guid.Empty) ? (object) string.Empty : (object) teamId;
      IVssRequestContext requestContext1 = requestContext;
      Guid empty = Guid.Empty;
      return service.GetLocationData(requestContext1, empty).GetResourceUri(requestContext, "work", WorkWebConstants.BacklogConfigurationLocationId, (object) routeValues, true).ToString();
    }

    private static WorkItemFieldReference GetFieldRererence(
      WorkItemTrackingRequestContext witRequestContext,
      string fieldReferenceName,
      Guid projectId)
    {
      FieldEntry field;
      if (witRequestContext.FieldDictionary.TryGetField(fieldReferenceName, out field))
        return new WorkItemFieldReference()
        {
          Name = field.Name,
          ReferenceName = field.ReferenceName,
          Url = WitUrlHelper.GetFieldUrl(witRequestContext.RequestContext, field.ReferenceName)
        };
      witRequestContext.RequestContext.Trace(59999, TraceLevel.Error, nameof (BacklogConfigurationExtensions), nameof (BacklogConfigurationExtensions), string.Format("Could not find field by referncename {0} for project {1}", (object) fieldReferenceName, (object) projectId));
      return (WorkItemFieldReference) null;
    }

    private static WorkItemTypeReference GetWorkItemTypeReference(
      WorkItemTrackingRequestContext witRequestContext,
      IWorkItemTypeService workItemTypeService,
      string workItemTypeName,
      Guid projectId)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType typeByReferenceName = workItemTypeService.GetWorkItemTypeByReferenceName(witRequestContext.RequestContext, projectId, workItemTypeName);
      WorkItemTypeReference itemTypeReference = new WorkItemTypeReference();
      itemTypeReference.Name = workItemTypeName;
      itemTypeReference.Url = WitUrlHelper.GetWorkItemTypeUrl(witRequestContext, typeByReferenceName);
      return itemTypeReference;
    }

    private static IReadOnlyCollection<Microsoft.TeamFoundation.Work.WebApi.WorkItemTypeStateInfo> Convert(
      IReadOnlyCollection<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemTypeStateInfo> workItemTypeMappedStates)
    {
      List<Microsoft.TeamFoundation.Work.WebApi.WorkItemTypeStateInfo> itemTypeStateInfoList = new List<Microsoft.TeamFoundation.Work.WebApi.WorkItemTypeStateInfo>();
      itemTypeStateInfoList.AddRange(workItemTypeMappedStates.Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemTypeStateInfo, Microsoft.TeamFoundation.Work.WebApi.WorkItemTypeStateInfo>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemTypeStateInfo, Microsoft.TeamFoundation.Work.WebApi.WorkItemTypeStateInfo>) (witState => new Microsoft.TeamFoundation.Work.WebApi.WorkItemTypeStateInfo()
      {
        WorkItemTypeName = witState.WorkItemTypeName,
        States = (IReadOnlyDictionary<string, string>) witState.States.ToDictionary<KeyValuePair<string, WorkItemStateCategory>, string, string>((Func<KeyValuePair<string, WorkItemStateCategory>, string>) (kv => kv.Key), (Func<KeyValuePair<string, WorkItemStateCategory>, string>) (kv => kv.Value.ToString()))
      })));
      return (IReadOnlyCollection<Microsoft.TeamFoundation.Work.WebApi.WorkItemTypeStateInfo>) itemTypeStateInfoList;
    }
  }
}
