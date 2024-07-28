// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.WorkItemTypeServiceHelper
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Boards.WebApi.Common
{
  public static class WorkItemTypeServiceHelper
  {
    public static IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType> GetWorkItemTypesInternal(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType> workItemTypes = requestContext.GetService<IWorkItemTypeService>().GetWorkItemTypes(requestContext, projectId);
      if (WorkItemTypeServiceHelper.HasProcessDescriptor(requestContext, projectId))
        return (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType>) workItemTypes.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType>) (wit => WorkItemTypeFactory.Create(new WorkItemTrackingRequestContext(requestContext), wit, false, false))).ToList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType>();
      IReadOnlyDictionary<string, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon> witColorsAndIcons = WorkItemTypeServiceHelper.GetProjectWorkItemTypeColorsAndIconsForProjectsWithNoDescriptor(requestContext, projectId);
      return (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType>) workItemTypes.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType>) (wit =>
      {
        Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon typeColorAndIcon = (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon) null;
        witColorsAndIcons.TryGetValue(wit.Name, out typeColorAndIcon);
        return WorkItemTypeFactory.Create(new WorkItemTrackingRequestContext(requestContext), wit, false, false, typeColorAndIcon?.Color, typeColorAndIcon?.Icon);
      })).ToList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType>();
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemType GetWorkItemTypeInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      string witNameOrReferenceName)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType typeByReferenceName = requestContext.GetService<IWorkItemTypeService>().GetWorkItemTypeByReferenceName(requestContext, projectId, witNameOrReferenceName);
      if (WorkItemTypeServiceHelper.HasProcessDescriptor(requestContext, projectId))
        return WorkItemTypeFactory.Create(new WorkItemTrackingRequestContext(requestContext), typeByReferenceName, true, false);
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon typeColorAndIcon = (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon) null;
      WorkItemTypeServiceHelper.GetProjectWorkItemTypeColorsAndIconsForProjectsWithNoDescriptor(requestContext, projectId).TryGetValue(typeByReferenceName.Name, out typeColorAndIcon);
      return WorkItemTypeFactory.Create(new WorkItemTrackingRequestContext(requestContext), typeByReferenceName, true, false, typeColorAndIcon?.Color, typeColorAndIcon?.Icon);
    }

    private static bool HasProcessDescriptor(IVssRequestContext requestContext, Guid projectId) => requestContext.GetService<IWorkItemTrackingProcessService>().TryGetProjectProcessDescriptor(requestContext, projectId, out ProcessDescriptor _);

    private static IReadOnlyDictionary<string, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon> GetProjectWorkItemTypeColorsAndIconsForProjectsWithNoDescriptor(
      IVssRequestContext tfsRequestContext,
      Guid projectId)
    {
      tfsRequestContext.GetService<IWorkItemMetadataFacadeService>();
      Dictionary<string, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon> withNoDescriptor = new Dictionary<string, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon>();
      IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon> source;
      if (tfsRequestContext.GetService<IWorkItemMetadataFacadeService>().GetWorkItemTypeColorAndIconsByProjectIds(tfsRequestContext, (IReadOnlyCollection<Guid>) new List<Guid>()
      {
        projectId
      }).TryGetValue(projectId, out source))
        withNoDescriptor = source.ToDictionary<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon, string, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon, string>) (c => c.WorkItemTypeName), (Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon>) (c => c));
      return (IReadOnlyDictionary<string, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon>) withNoDescriptor;
    }
  }
}
