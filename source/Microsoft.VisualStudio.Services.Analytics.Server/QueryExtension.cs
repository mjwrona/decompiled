// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.QueryExtension
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System;
using System.Data.SqlTypes;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public static class QueryExtension
  {
    public static void ExpandODataProperties(
      this AnalyticsViewQuery query,
      IVssRequestContext requestContext,
      AnalyticsView view,
      AnalyticsViewScope viewScope,
      bool preview)
    {
      IAnalyticsViewsEdmModelService service = requestContext.GetService<IAnalyticsViewsEdmModelService>();
      if (string.IsNullOrEmpty(query.EndpointVersion))
        query.EndpointVersion = service.GetLatestSupportedModelVersionForViews(requestContext);
      query.ODataUrl = QueryExtension.ExpandODataUrl(requestContext, view, viewScope, query, preview);
      query.Schema = QueryExtension.ExpandODataSchema(requestContext, view.ViewType, query);
    }

    private static ODataJsonSchema ExpandODataSchema(
      IVssRequestContext requestContext,
      AnalyticsViewType viewType,
      AnalyticsViewQuery query)
    {
      try
      {
        return ODataJsonSchemaGeneratorFactory.GetSchemaGenerator(requestContext, viewType, query).Generate();
      }
      catch (ODataException ex)
      {
        requestContext.TraceException(12019003, TraceLevel.Error, nameof (QueryExtension), nameof (ExpandODataSchema), (Exception) ex);
        throw new AnalyticsViewsODataMetadataReadException(AnalyticsResources.VIEW_ODATA_METADATA_READ_FAILED(), (Exception) ex);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12019002, TraceLevel.Error, nameof (QueryExtension), nameof (ExpandODataSchema), ex);
        throw new AnalyticsViewSchemaGenerationFailedException(AnalyticsResources.VIEW_SCHEMA_GENERATION_FAILED(), ex);
      }
    }

    private static string ExpandODataUrl(
      IVssRequestContext requestContext,
      AnalyticsView view,
      AnalyticsViewScope viewScope,
      AnalyticsViewQuery query,
      bool preview)
    {
      AnalyticsViewsUriBuilder.GetEndpointUrl(requestContext, query.EndpointVersion, query.EntitySet);
      return AnalyticsViewsUriBuilder.GetODataUrl(requestContext, query.EndpointVersion, query.EntitySet, QueryExtension.ExpandODataQuery(requestContext, view, viewScope, query, preview));
    }

    private static string ExpandODataQuery(
      IVssRequestContext requestContext,
      AnalyticsView view,
      AnalyticsViewScope viewScope,
      AnalyticsViewQuery query,
      bool preview)
    {
      try
      {
        return new ODataQueryExpanderFactory().GetFactory(requestContext, view.ViewType).ExpandQuery(requestContext, view, viewScope, query, preview);
      }
      catch (SqlNullValueException ex)
      {
        throw new AnalyticsViewDataNotFoundException(AnalyticsResources.VIEW_DATA_NOT_FOUND(), (Exception) ex);
      }
    }
  }
}
