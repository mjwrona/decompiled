// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.PlanCriteriaUtils
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Agile.Server.QueryHelpers;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  internal static class PlanCriteriaUtils
  {
    private const string c_selectStatement = "SELECT [System.Id] FROM WorkItems WHERE ";

    internal static string ToCriteriaString(Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterModel filterModel) => filterModel == null ? (string) null : JsonConvert.SerializeObject((object) filterModel);

    internal static string TransformWiqlNamesToIds(IVssRequestContext requestContext, string wiql)
    {
      if (string.IsNullOrEmpty(wiql))
        return wiql;
      string queryText = "SELECT [System.Id] FROM WorkItems WHERE " + wiql;
      return WiqlTransformUtils.TransformNamesToIds(requestContext, queryText, false).Substring("SELECT [System.Id] FROM WorkItems WHERE ".Length);
    }

    internal static string TransformWiqlIdsToNames(
      IVssRequestContext requestContext,
      string filterCriteria)
    {
      WiqlIdToNameTransformer toNameTransformer = new WiqlIdToNameTransformer();
      string str = "SELECT [System.Id] FROM WorkItems WHERE " + TimelineQueryBuilder.CreateCriteriaClauses((IReadOnlyList<Microsoft.TeamFoundation.Work.WebApi.FilterClause>) PlanCriteriaUtils.GetFilterModel(requestContext, filterCriteria).Clauses.Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause, Microsoft.TeamFoundation.Work.WebApi.FilterClause>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause, Microsoft.TeamFoundation.Work.WebApi.FilterClause>) (c => new Microsoft.TeamFoundation.Work.WebApi.FilterClause(c.FieldName, c.Index, c.LogicalOperator, c.Operator, c.Value))).ToList<Microsoft.TeamFoundation.Work.WebApi.FilterClause>().AsReadOnly(), string.Empty);
      IVssRequestContext requestContext1 = requestContext;
      string wiql = str;
      return toNameTransformer.ReplaceIdWithText(requestContext1, wiql).Substring("SELECT [System.Id] FROM WorkItems WHERE ".Length);
    }

    internal static Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterModel GetFilterModel(
      IVssRequestContext requestContext,
      string wiql)
    {
      if (string.IsNullOrEmpty(wiql))
        return (Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterModel) null;
      QueryEditorModel queryEditorModel = new QueryEditorModel(requestContext, "SELECT [System.Id] FROM WorkItems WHERE " + wiql, false, (IDictionary<string, int>) new Dictionary<string, int>(), (string) null);
      PlanCriteriaUtils.ResolveFieldReferences(requestContext, (Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterModel) queryEditorModel.SourceFilter);
      return (Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterModel) queryEditorModel.SourceFilter;
    }

    internal static void ResolveFieldReferences(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterModel filterModel)
    {
      WorkItemTrackingRequestContext witRequestContext = new WorkItemTrackingRequestContext(requestContext);
      QueryAdapter queryAdapter = new QueryAdapter(requestContext);
      filterModel.Clauses = (ICollection<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause>) filterModel.Clauses.Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause>) (c =>
      {
        string str;
        try
        {
          str = witRequestContext.FieldDictionary.GetFieldByNameOrId(c.FieldName).ReferenceName;
        }
        catch (WorkItemTrackingFieldDefinitionNotFoundException ex)
        {
          str = c.FieldName;
        }
        c.FieldName = str;
        c.Operator = queryAdapter.GetInvariantOperator(c.Operator);
        c.LogicalOperator = queryAdapter.GetInvariantOperator(c.LogicalOperator);
        if (c.Value.StartsWith("@", StringComparison.OrdinalIgnoreCase))
        {
          try
          {
            c.Value = queryAdapter.GetInvariantFieldValue(c.FieldName, c.Operator, c.Value);
          }
          catch (LegacyValidationException ex)
          {
          }
        }
        return c;
      })).ToList<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause>();
    }
  }
}
