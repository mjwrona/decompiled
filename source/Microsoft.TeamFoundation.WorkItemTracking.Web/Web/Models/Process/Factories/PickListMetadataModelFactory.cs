// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories.PickListMetadataModelFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories
{
  internal static class PickListMetadataModelFactory
  {
    public static PickListMetadata Create(
      IVssRequestContext requestContext,
      WorkItemPickListMetadata metadata)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WorkItemPickListMetadata>(metadata, nameof (metadata));
      return new PickListMetadata()
      {
        Id = metadata.Id,
        Name = metadata.Name,
        Type = metadata.Type.ToString(),
        IsSuggested = metadata.IsSuggested(requestContext, Guid.Empty, 0),
        Url = PickListMetadataModelFactory.GetLocationUrlForPicklist(requestContext, metadata.Id)
      };
    }

    public static string GetLocationUrlForPicklist(IVssRequestContext requestContext, Guid listId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "processes", WorkItemTrackingLocationIds.ProcessPickList, (object) new
      {
        listId = listId
      }).ToString();
    }
  }
}
