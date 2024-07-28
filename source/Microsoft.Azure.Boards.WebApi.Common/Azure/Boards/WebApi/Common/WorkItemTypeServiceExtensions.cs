// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.WorkItemTypeServiceExtensions
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Boards.WebApi.Common
{
  public static class WorkItemTypeServiceExtensions
  {
    public static bool HasProcessDescriptor(IVssRequestContext requestContext, Guid projectId) => requestContext.GetService<IWorkItemTrackingProcessService>().TryGetProjectProcessDescriptor(requestContext, projectId, out ProcessDescriptor _);

    public static IReadOnlyDictionary<string, WorkItemTypeColorAndIcon> GetProjectWorkItemTypeColorsAndIconsForProjectsWithNoDescriptor(
      IVssRequestContext tfsRequestContext,
      Guid projectId)
    {
      tfsRequestContext.GetService<IWorkItemMetadataFacadeService>();
      Dictionary<string, WorkItemTypeColorAndIcon> withNoDescriptor = new Dictionary<string, WorkItemTypeColorAndIcon>();
      IReadOnlyCollection<WorkItemTypeColorAndIcon> source;
      if (tfsRequestContext.GetService<IWorkItemMetadataFacadeService>().GetWorkItemTypeColorAndIconsByProjectIds(tfsRequestContext, (IReadOnlyCollection<Guid>) new List<Guid>()
      {
        projectId
      }).TryGetValue(projectId, out source))
        withNoDescriptor = source.ToDictionary<WorkItemTypeColorAndIcon, string, WorkItemTypeColorAndIcon>((Func<WorkItemTypeColorAndIcon, string>) (c => c.WorkItemTypeName), (Func<WorkItemTypeColorAndIcon, WorkItemTypeColorAndIcon>) (c => c));
      return (IReadOnlyDictionary<string, WorkItemTypeColorAndIcon>) withNoDescriptor;
    }
  }
}
