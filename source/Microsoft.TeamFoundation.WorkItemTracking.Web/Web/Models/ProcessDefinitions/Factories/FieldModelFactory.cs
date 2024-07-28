// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories.FieldModelFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories
{
  internal static class FieldModelFactory
  {
    internal static FieldModel Create(
      IVssRequestContext requestContext,
      Guid processId,
      string witReferenceName,
      ProcessFieldResult field)
    {
      return new FieldModel()
      {
        Id = field.ReferenceName,
        Name = field.Name,
        Type = (FieldType) Enum.Parse(typeof (FieldType), field.Type.ToString()),
        Description = field.Description,
        PickList = FieldModelFactory.GetPickListMetadataModelOrNull(requestContext, field.PickListId),
        Url = FieldModelFactory.GetLocationUrlForField(requestContext, processId, field.ReferenceName)
      };
    }

    internal static FieldModel Create(
      IVssRequestContext requestContext,
      Guid processId,
      FieldEntry field)
    {
      return new FieldModel()
      {
        Id = field.ReferenceName,
        Name = field.Name,
        Type = field.IsIdentity ? FieldType.Identity : (FieldType) Enum.Parse(typeof (FieldType), field.FieldType.ToString()),
        Description = field.Description,
        PickList = FieldModelFactory.GetPickListMetadataModelOrNull(requestContext, field.PickListId),
        Url = FieldModelFactory.GetLocationUrlForField(requestContext, processId, field.ReferenceName)
      };
    }

    private static string GetLocationUrlForField(
      IVssRequestContext requestContext,
      Guid processId,
      string fieldId)
    {
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "processDefinitions", WorkItemTrackingLocationIds.ProcessDefinitionFields, (object) new
      {
        processId = processId,
        field = fieldId
      }).ToString();
    }

    internal static PickListMetadataModel GetPickListMetadataModelOrNull(
      IVssRequestContext requestContext,
      Guid? pickListId)
    {
      if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) && pickListId.HasValue)
      {
        WorkItemPickListMetadata metadata = requestContext.GetService<WorkItemPickListService>().GetListsMetadata(requestContext).Where<WorkItemPickListMetadata>((Func<WorkItemPickListMetadata, bool>) (l => l.Id == pickListId.Value)).FirstOrDefault<WorkItemPickListMetadata>();
        if (metadata != null)
          return PickListMetadataModelFactory.Create(requestContext, metadata);
      }
      return (PickListMetadataModel) null;
    }
  }
}
