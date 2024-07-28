// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories.WorkItemTagDefinitionFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories
{
  internal static class WorkItemTagDefinitionFactory
  {
    internal static WorkItemTagDefinition Create(
      IVssRequestContext requestContext,
      TagDefinition tagDefinition,
      bool excludeUrl = false)
    {
      ArgumentUtility.CheckForNull<TagDefinition>(tagDefinition, nameof (tagDefinition));
      WorkItemTagDefinition itemTagDefinition = new WorkItemTagDefinition()
      {
        Id = tagDefinition.TagId,
        Name = tagDefinition.Name,
        LastUpdated = tagDefinition.LastUpdated
      };
      if (!excludeUrl)
        itemTagDefinition.Url = WitUrlHelper.GetWorkItemTagsUrl(requestContext, tagDefinition.Scope, tagDefinition);
      return itemTagDefinition;
    }
  }
}
