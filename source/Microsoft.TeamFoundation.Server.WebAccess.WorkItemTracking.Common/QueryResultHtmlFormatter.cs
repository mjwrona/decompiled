// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.QueryResultHtmlFormatter
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Presentation;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class QueryResultHtmlFormatter
  {
    private readonly IVssRequestContext m_tfsRequestContext;
    private readonly TswaServerHyperlinkService m_hyperlinkService;
    private readonly WorkItemTrackingFieldService m_fieldTypeDictionary;
    private readonly Uri m_projectUri;
    private readonly string m_collectionName;

    public QueryResultHtmlFormatter(IVssRequestContext requestContext)
    {
      this.m_tfsRequestContext = requestContext;
      this.m_hyperlinkService = this.m_tfsRequestContext.GetService<TswaServerHyperlinkService>();
      this.m_fieldTypeDictionary = this.m_tfsRequestContext.GetService<WorkItemTrackingFieldService>();
      this.m_collectionName = this.m_tfsRequestContext.ServiceHost.Name;
      ProjectInfo project = this.m_tfsRequestContext.GetService<IRequestProjectService>().GetProject(this.m_tfsRequestContext);
      if (project == null)
        this.m_projectUri = (Uri) null;
      else
        this.m_projectUri = new Uri(project.Uri);
    }

    public string GenerateHtmlForQueryResult(
      QueryResultModel queryResultModel,
      Guid? queryId,
      bool isItemQuery)
    {
      IEnumerable<FormattingFieldDefinition> displayFieldList = QueryResultHtmlFormatter.MapToFormattingDisplayFieldList(queryResultModel.Columns);
      FormattingWorkItemCollection workItemCollection = this.MapToFormattingWorkItemCollection(this.m_collectionName, this.m_projectUri, queryId, isItemQuery, queryResultModel.Payload);
      HtmlFormatter formatter = new HtmlFormatter();
      this.SetDefaultColumnsDisplayName(formatter);
      bool withProjectContext = WorkItemTrackingFeatureFlags.GenerateWorkItemURLsWithProjectContext(this.m_tfsRequestContext);
      return SafeHtmlWrapper.MakeSafe(formatter.FormatInputData(workItemCollection, displayFieldList, (IWorkItemUrlBuilder) new WorkItemUrlFormatter(this.m_tfsRequestContext, this.m_hyperlinkService, withProjectContext), true));
    }

    public string GenerateHtmlForWorkItems(
      IEnumerable<WorkItemModel> workItems,
      IEnumerable<string> fields)
    {
      ArgumentUtility.CheckForNull<IEnumerable<WorkItemModel>>(workItems, nameof (workItems));
      FormattingWorkItemCollection workItemCollection = this.MapToFormattingWorkItemCollection(this.m_collectionName, this.m_projectUri, workItems);
      HtmlFormatter formatter = new HtmlFormatter();
      this.SetDefaultColumnsDisplayName(formatter);
      IEnumerable<FormattingFieldDefinition> fieldDefinitions = this.MapToFormattingFieldDefinitions(fields);
      bool withProjectContext = WorkItemTrackingFeatureFlags.GenerateWorkItemURLsWithProjectContext(this.m_tfsRequestContext);
      return formatter.FormatInputData(workItemCollection, fieldDefinitions, (IWorkItemUrlBuilder) new WorkItemUrlFormatter(this.m_tfsRequestContext, this.m_hyperlinkService, withProjectContext), true);
    }

    private IEnumerable<FormattingFieldDefinition> MapToFormattingFieldDefinitions(
      IEnumerable<string> fields)
    {
      List<FormattingFieldDefinition> fieldDefinitions = new List<FormattingFieldDefinition>();
      if (fields != null)
      {
        FieldEntry[] array = fields.Select<string, FieldEntry>((Func<string, FieldEntry>) (f => this.m_fieldTypeDictionary.GetField(this.m_tfsRequestContext, f))).ToArray<FieldEntry>();
        if (array.Length != 0)
        {
          this.m_tfsRequestContext.GetService<WorkItemTrackingFieldService>().GetAllFields(this.m_tfsRequestContext);
          foreach (FieldEntry fieldEntry in array)
            fieldDefinitions.Add(new FormattingFieldDefinition(fieldEntry.Name, fieldEntry.ReferenceName, fieldEntry.FieldType));
        }
      }
      return (IEnumerable<FormattingFieldDefinition>) fieldDefinitions;
    }

    private static IEnumerable<FormattingFieldDefinition> MapToFormattingDisplayFieldList(
      IEnumerable<QueryDisplayColumn> displayColumns)
    {
      List<FormattingFieldDefinition> displayFieldList = new List<FormattingFieldDefinition>();
      foreach (QueryDisplayColumn displayColumn in displayColumns)
      {
        FormattingFieldDefinition formattingFieldDefinition = new FormattingFieldDefinition(displayColumn.Text, displayColumn.Name, displayColumn.FieldType);
        displayFieldList.Add(formattingFieldDefinition);
      }
      return (IEnumerable<FormattingFieldDefinition>) displayFieldList;
    }

    private void SetDefaultColumnsDisplayName(HtmlFormatter formatter)
    {
      ArgumentUtility.CheckForNull<HtmlFormatter>(formatter, nameof (formatter));
      FieldEntry field1 = this.m_fieldTypeDictionary.GetField(this.m_tfsRequestContext, CoreFieldReferenceNames.Id);
      formatter.SetDefaultColumnDisplayName(CoreFieldReferenceNames.Id, field1.Name);
      FieldEntry field2 = this.m_fieldTypeDictionary.GetField(this.m_tfsRequestContext, CoreFieldReferenceNames.Title);
      formatter.SetDefaultColumnDisplayName(CoreFieldReferenceNames.Title, field2.Name);
    }

    private FormattingWorkItemCollection MapToFormattingWorkItemCollection(
      string collectionName,
      Uri projectUri,
      IEnumerable<WorkItemModel> workItems)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(collectionName, nameof (collectionName));
      ArgumentUtility.CheckForNull<IEnumerable<WorkItemModel>>(workItems, nameof (workItems));
      FormattingWorkItemCollection workItemCollection = new FormattingWorkItemCollection(collectionName, projectUri);
      foreach (WorkItemModel workItem in workItems)
      {
        FormattingWorkItem formattingWorkItem = new FormattingWorkItem((int) workItem.LatestData.Where<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (p => p.Key == -3)).First<KeyValuePair<int, object>>().Value);
        foreach (KeyValuePair<int, object> keyValuePair in (IEnumerable<KeyValuePair<int, object>>) workItem.LatestData)
        {
          FieldEntry fieldById = this.m_fieldTypeDictionary.GetFieldById(this.m_tfsRequestContext, keyValuePair.Key);
          object obj = keyValuePair.Value;
          if (fieldById != null)
          {
            if (fieldById.FieldType == InternalFieldType.DateTime && obj != null)
              obj = this.GetFormattedDateTime(obj);
            else if (fieldById.IsIdentity && obj != null)
              obj = (object) this.GetFormattedIdentityName(obj);
            formattingWorkItem.FieldValues[fieldById.ReferenceName] = obj;
          }
        }
        workItemCollection.WorkItems.Add(formattingWorkItem);
      }
      return workItemCollection;
    }

    private FormattingWorkItemCollection MapToFormattingWorkItemCollection(
      string collectionName,
      Uri projectUri,
      Guid? queryId,
      bool isItemQuery,
      QueryResultPayload payload)
    {
      QueryItem queryItem;
      FormattingWorkItemCollection workItemCollection = isItemQuery || !queryId.HasValue || !QueryHelpers.TryGetQueryItem(this.m_tfsRequestContext, queryId.Value, out queryItem) ? new FormattingWorkItemCollection(collectionName, projectUri) : new FormattingWorkItemCollection(collectionName, projectUri, queryId, queryItem.Name);
      if (payload == null || payload.Columns == null || payload.Rows == null)
      {
        this.m_tfsRequestContext.Trace(516471, TraceLevel.Warning, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, "Cannot construct FormattingWorkItemCollection without payload.");
        return workItemCollection;
      }
      List<string> columnList = payload.Columns.ToList<string>();
      foreach (IEnumerable<object> row in payload.Rows)
      {
        Dictionary<string, object> dictionary = row.Zip(Enumerable.Range(0, columnList.Count), (field, index) => new
        {
          FieldValue = field,
          FieldIndex = index
        }).ToDictionary(x => columnList[x.FieldIndex], x => x.FieldValue);
        FormattingWorkItem formattingWorkItem = new FormattingWorkItem((int) dictionary[CoreFieldReferenceNames.Id]);
        foreach (KeyValuePair<string, object> keyValuePair in dictionary)
        {
          object obj = keyValuePair.Value;
          FieldEntry field = this.m_fieldTypeDictionary.GetField(this.m_tfsRequestContext, keyValuePair.Key);
          if (field.FieldType == InternalFieldType.DateTime && obj != null)
            obj = this.GetFormattedDateTime(obj);
          else if (field.IsIdentity && obj != null)
            obj = (object) this.GetFormattedIdentityName(obj);
          formattingWorkItem.FieldValues[keyValuePair.Key] = obj;
        }
        workItemCollection.WorkItems.Add(formattingWorkItem);
      }
      return workItemCollection;
    }

    private string ConvertToUserTimeZone(string value)
    {
      DateTime result;
      return DateTime.TryParse(value, out result) ? TimeZoneInfo.ConvertTime(result, TimeZoneInfo.Utc, this.m_tfsRequestContext.GetTimeZone()).ToString() : value;
    }

    private object GetFormattedDateTime(object value)
    {
      DateTime dateTime = (DateTime) value;
      string str;
      try
      {
        str = dateTime.ToString((IFormatProvider) CultureInfo.CurrentCulture);
      }
      catch (ArgumentOutOfRangeException ex)
      {
        str = dateTime.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        this.m_tfsRequestContext.TraceException(516462, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, (Exception) ex);
      }
      return (object) this.ConvertToUserTimeZone(str);
    }

    private string GetFormattedIdentityName(object value)
    {
      if (value is WitIdentityRef witIdentityRef)
        return witIdentityRef.IdentityRef == null ? string.Empty : witIdentityRef.IdentityRef.DisplayName;
      string distinctDisplayName = value as string;
      if (!string.IsNullOrWhiteSpace(distinctDisplayName))
        distinctDisplayName = IdentityHelper.GetDisplayNameFromDistinctDisplayName(distinctDisplayName);
      return distinctDisplayName;
    }
  }
}
