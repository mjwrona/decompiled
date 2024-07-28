// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Charting.WitChartingDalWrapper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Charting
{
  internal class WitChartingDalWrapper
  {
    internal static void ValidateQuery(Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItem query, IDictionary queryContext)
    {
      if (query.QueryText == null)
        throw new InvalidTransformOptionsException("CannotQueryFolder");
      if (query.QueryText.ToLower().Contains("@project") && !queryContext.Contains((object) "project"))
        throw new InvalidTransformOptionsException("QueryHasNoProject");
    }

    internal static Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItem GetQuery(
      IVssRequestContext requestContext,
      IDataAccessLayer dataAccessLayer,
      Guid queryGuid,
      IDictionary queryContext)
    {
      requestContext.TraceEnter(901606, "WITCharting", "WitQueryProvider", nameof (GetQuery));
      try
      {
        return AssignedToMeQuery.Id == queryGuid ? WitChartingDalWrapper.GetAssignedToMeQuery(requestContext, queryContext) : WitChartingDalWrapper.GetQueryItemFromService(requestContext, queryGuid);
      }
      catch (LegacyValidationException ex)
      {
        throw new AccessCheckException(requestContext.UserContext, string.Empty, 0, Guid.Empty, ex.Message);
      }
      catch (LegacyDeniedOrNotExist ex)
      {
        throw new TransformFilterValueNotFound(ex.Message, (Exception) ex);
      }
      catch (WorkItemTrackingUnauthorizedOperationException ex)
      {
        throw new TransformFilterValueNotFound(ex.Message, (Exception) ex);
      }
      finally
      {
        requestContext.TraceLeave(901607, "WITCharting", "WitQueryProvider", nameof (GetQuery));
      }
    }

    private static Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItem GetQueryItemFromService(
      IVssRequestContext requestContext,
      Guid queryId)
    {
      return Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItem.Create(requestContext.GetService<TeamFoundationQueryItemService>().GetQueryById(requestContext, queryId, new int?(0), true, false, false, new Guid?()));
    }

    internal static IEnumerable<string> IdentifyRequiredFields(IEnumerable<TransformOptions> options)
    {
      HashSet<string> stringSet = new HashSet<string>();
      foreach (TransformOptions option in options)
      {
        if (!string.IsNullOrEmpty(option.GroupBy))
          stringSet.Add(option.GroupBy);
        if (option.Measure.IsPropertyNeeded())
          stringSet.Add(option.Measure.PropertyName);
        if (!string.IsNullOrEmpty(option.Series))
          stringSet.Add(option.Series);
        if (!string.IsNullOrEmpty(option.HistoryRange))
          stringSet.Add(WitTrendPolicyHelper.TrendFieldRefName);
      }
      return (IEnumerable<string>) stringSet;
    }

    internal static ISet<FieldEntry> GetColumns(
      IVssRequestContext requestContext,
      IEnumerable<string> fieldNames)
    {
      WorkItemTrackingFieldService service = requestContext.GetService<WorkItemTrackingFieldService>();
      HashSet<FieldEntry> columns = new HashSet<FieldEntry>();
      try
      {
        foreach (string fieldName in fieldNames)
          columns.Add(service.GetField(requestContext, fieldName));
        if (columns.Contains(service.GetFieldById(requestContext, -105)))
          columns.Add(service.GetFieldById(requestContext, -104));
      }
      catch (WorkItemTrackingFieldDefinitionNotFoundException ex)
      {
        throw new InvalidTransformOptionsException(ex.Message);
      }
      catch (LegacyValidationException ex)
      {
        throw new InvalidTransformOptionsException(ex.Message);
      }
      return (ISet<FieldEntry>) columns;
    }

    internal static Dictionary<string, int> GetFieldIdMap(
      IVssRequestContext requestContext,
      IEnumerable<TransformOptions> options)
    {
      return WitChartingDalWrapper.GetColumns(requestContext, WitChartingDalWrapper.IdentifyRequiredFields(options)).ToDictionary<FieldEntry, string, int>((Func<FieldEntry, string>) (kvp => kvp.ReferenceName), (Func<FieldEntry, int>) (kvp => kvp.FieldId));
    }

    internal static Dictionary<int, bool> GetIdentityFieldsMap(
      IVssRequestContext requestContext,
      IEnumerable<TransformOptions> options)
    {
      return WitChartingDalWrapper.GetColumns(requestContext, WitChartingDalWrapper.IdentifyRequiredFields(options)).ToDictionary<FieldEntry, int, bool>((Func<FieldEntry, int>) (kvp => kvp.FieldId), (Func<FieldEntry, bool>) (kvp => kvp.IsIdentity));
    }

    internal static Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItem GetAssignedToMeQuery(
      IVssRequestContext requestContext,
      IDictionary context)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string str1 = string.Format("/Users/{0}/WebAccess/Projects/{1}/Queries/a2108d31-086c-4fb0-afda-097e4cc46df4/Query", (object) requestContext.GetUserId(), context[(object) "projectId"]);
      IVssRequestContext requestContext1 = requestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) str1;
      string str2 = service.GetValue(requestContext1, in local, "SELECT [System.Id], [System.WorkItemType], [System.Title], [System.State], [System.AreaPath], [System.IterationPath] FROM WorkItems WHERE [System.TeamProject] = @project AND [System.AssignedTo] = @me ORDER BY [System.ChangedDate] DESC");
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem queryItem = Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem.Create(new QueryItemEntry()
      {
        Wiql = str2
      });
      WiqlIdToNameTransformer.Transform(requestContext, queryItem);
      return Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItem.Create(queryItem);
    }

    internal static Guid GetQueryGuid(TransformOptions transformOptions)
    {
      Guid result;
      if (!Guid.TryParse(transformOptions.Filter, out result))
        throw new InvalidTransformOptionsException("ErrorInvalidQueryId");
      return result;
    }
  }
}
