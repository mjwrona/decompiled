// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.ODataJsonSchemaGeneratorFactory
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.Edm;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Model;
using Microsoft.VisualStudio.Services.Analytics.OData;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class ODataJsonSchemaGeneratorFactory
  {
    public static IODataJsonSchemaGenerator GetSchemaGenerator(
      IVssRequestContext requestContext,
      AnalyticsViewType viewType,
      AnalyticsViewQuery query)
    {
      string odataQuery = new Uri(query.ODataUrl).Query.Remove(0, 1);
      return ODataJsonSchemaGeneratorFactory.GetSchemaGenerator(requestContext, query.EndpointVersion, viewType, query.EntitySet, query.ODataTemplate, odataQuery);
    }

    public static IODataJsonSchemaGenerator GetSchemaGenerator(
      IVssRequestContext requestContext,
      string modelVersion,
      AnalyticsViewType viewType,
      string entitySet,
      string queryTemplate,
      string odataQuery)
    {
      IEdmModel edmModel = requestContext.GetService<IAnalyticsViewsEdmModelService>().GetEdmModel(requestContext, modelVersion);
      if (viewType == AnalyticsViewType.WorkItems)
      {
        WorkItemsViewType workItemsViewType = entitySet == AnalyticsModelBuilder.s_clrTypeToEntitySetName[typeof (WorkItemRevision)] ? WorkItemsViewType.Historical : WorkItemsViewType.Current;
        WorkItemsViewODataJsonSchemaGeneratorData schemaGeneratorData = new WorkItemsViewODataJsonSchemaGeneratorData();
        schemaGeneratorData.EntitySet = entitySet;
        schemaGeneratorData.ODataQuery = odataQuery;
        schemaGeneratorData.ODataQueryTemplate = queryTemplate;
        schemaGeneratorData.QueryType = workItemsViewType;
        WorkItemsViewODataJsonSchemaGeneratorData data = schemaGeneratorData;
        return (IODataJsonSchemaGenerator) new WorkItemsViewODataJsonSchemaGenerator(requestContext, edmModel, data);
      }
      ODataJsonSchemaGeneratorData data1 = new ODataJsonSchemaGeneratorData()
      {
        EntitySet = entitySet,
        ODataQuery = odataQuery
      };
      return (IODataJsonSchemaGenerator) new ODataJsonSchemaGenerator(edmModel, data1);
    }
  }
}
