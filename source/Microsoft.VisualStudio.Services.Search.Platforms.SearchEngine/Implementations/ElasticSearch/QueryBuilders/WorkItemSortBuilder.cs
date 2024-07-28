// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.WorkItemSortBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using Microsoft.VisualStudio.Services.Search.Server.WorkItemFieldCache;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  internal class WorkItemSortBuilder : EntitySortBuilder
  {
    public WorkItemSortBuilder(IEnumerable<EntitySortOption> sortOptions)
      : base(sortOptions)
    {
    }

    protected override IEnumerable<EntitySortOption> GetPlatformSortOptions(
      IVssRequestContext requestContext,
      IEnumerable<EntitySortOption> sortOptions)
    {
      IWorkItemFieldCacheService service = requestContext.GetService<IWorkItemFieldCacheService>();
      HashSet<EntitySortOption> platformSortOptions = new HashSet<EntitySortOption>();
      foreach (EntitySortOption sortOption in sortOptions)
      {
        WorkItemField fieldData;
        if (service.TryGetFieldByRefName(requestContext, sortOption.Field, out fieldData))
          platformSortOptions.Add(new EntitySortOption()
          {
            Field = WorkItemSortBuilder.GetSortablePlatformFieldName(requestContext, fieldData),
            SortOrder = sortOption.SortOrder
          });
        else
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082216, "Search Engine", "Query Builder", FormattableString.Invariant(FormattableStringFactory.Create("Field [{0}] not found in field cache. Sorting won't work. User query won't fail though.", (object) sortOption.Field)));
      }
      return (IEnumerable<EntitySortOption>) platformSortOptions;
    }

    protected override Nest.FieldType GetNestFieldType(string platformFieldName) => WorkItemSortBuilder.GetNestFieldType(WorkItemIndexedField.FromPlatformFieldName(platformFieldName).Type);

    private static string GetSortablePlatformFieldName(
      IVssRequestContext requestContext,
      WorkItemField workItemField)
    {
      WorkItemIndexedField itemIndexedField = WorkItemSortBuilder.GetWorkItemIndexedField(requestContext, workItemField);
      if (itemIndexedField.Type != WorkItemContract.FieldType.String && itemIndexedField.Type != WorkItemContract.FieldType.Html && itemIndexedField.Type != WorkItemContract.FieldType.Path && itemIndexedField.Type != WorkItemContract.FieldType.Identity && itemIndexedField.Type != WorkItemContract.FieldType.Name)
        return itemIndexedField.PlatformFieldName;
      return FormattableString.Invariant(FormattableStringFactory.Create("{0}", (object) itemIndexedField.NonAnalyzedPlatformFieldName));
    }

    private static WorkItemIndexedField GetWorkItemIndexedField(
      IVssRequestContext requestContext,
      WorkItemField workItemField)
    {
      if (!requestContext.IsFeatureEnabled("Search.Server.WorkItem.QueryIdentityFields") || !workItemField.IsIdentity)
        return WorkItemIndexedField.FromWitField(workItemField);
      return (bool) requestContext.Items["isUserAnonymousKey"] ? WorkItemIndexedField.FromWitField(workItemField.ReferenceName, WorkItemContract.FieldType.Name) : WorkItemIndexedField.FromWitField(workItemField.ReferenceName, WorkItemContract.FieldType.Identity);
    }

    private static Nest.FieldType GetNestFieldType(WorkItemContract.FieldType fieldType)
    {
      switch (fieldType)
      {
        case WorkItemContract.FieldType.Boolean:
          return Nest.FieldType.Boolean;
        case WorkItemContract.FieldType.Real:
          return Nest.FieldType.Float;
        case WorkItemContract.FieldType.Integer:
          return Nest.FieldType.Integer;
        case WorkItemContract.FieldType.String:
        case WorkItemContract.FieldType.Path:
        case WorkItemContract.FieldType.Html:
        case WorkItemContract.FieldType.Identity:
        case WorkItemContract.FieldType.Name:
          return Nest.FieldType.Text;
        case WorkItemContract.FieldType.DateTime:
          return Nest.FieldType.Date;
        default:
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Field type [{0}] is not supported.", (object) fieldType)));
      }
    }
  }
}
