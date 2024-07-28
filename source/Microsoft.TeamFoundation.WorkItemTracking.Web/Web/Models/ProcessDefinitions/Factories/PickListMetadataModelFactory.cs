// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories.PickListMetadataModelFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Location.Server;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories
{
  internal static class PickListMetadataModelFactory
  {
    public static PickListMetadataModel Create(
      IVssRequestContext requestContext,
      WorkItemPickListMetadata metadata)
    {
      return new PickListMetadataModel()
      {
        Id = metadata.Id,
        Name = metadata.Name,
        Type = metadata.Type.ToString(),
        IsSuggested = metadata.IsSuggested(requestContext, Guid.Empty, 0),
        Url = PickListMetadataModelFactory.GetLocationUrlForPicklist(requestContext, metadata.Id)
      };
    }

    public static string GetLocationUrlForPicklist(IVssRequestContext requestContext, Guid listId) => requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "processDefinitions", WorkItemTrackingLocationIds.SpecificPickList, (object) new
    {
      listId = listId
    }).ToString();
  }
}
