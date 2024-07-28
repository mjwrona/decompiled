// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem.WorkItemContract
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.Utils;
using Microsoft.VisualStudio.Services.Search.Crawler.Entities;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Nest;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem
{
  public class WorkItemContract : AbstractSearchDocumentContract
  {
    public const string WorkItemFieldIndexCriteria = "-WEF_????????????????????????????????_*,-System.IterationLevel*,-System.AreaLevel*,-System.Watermark";
    public const string WorkItemUpdateScript = "update_work_item";
    public const string UpdateWorkItemScript = "update_workitem";
    public const string IsReindexingParameter = "isReindexing";
    private int m_size;
    internal static readonly IReadOnlyDictionary<WorkItemContract.FieldType, string> PlatformFieldTypeStringMap = (IReadOnlyDictionary<WorkItemContract.FieldType, string>) new Dictionary<WorkItemContract.FieldType, string>()
    {
      [WorkItemContract.FieldType.Boolean] = "bool",
      [WorkItemContract.FieldType.DateTime] = "date",
      [WorkItemContract.FieldType.Html] = "html",
      [WorkItemContract.FieldType.Integer] = "int",
      [WorkItemContract.FieldType.IntegerAsString] = "intstr",
      [WorkItemContract.FieldType.Path] = "path",
      [WorkItemContract.FieldType.Real] = "real",
      [WorkItemContract.FieldType.String] = "str",
      [WorkItemContract.FieldType.Identity] = "identity",
      [WorkItemContract.FieldType.Name] = "name",
      [WorkItemContract.FieldType.AllTypes] = "*"
    };

    [Keyword(Ignore = true)]
    public override string DocumentId { get; set; }

    [Keyword(Name = "item")]
    public override string Item { get; set; }

    [Keyword(Ignore = true)]
    public override string Routing { get; set; }

    [Boolean(Name = "isDiscussionOnly")]
    public bool IsDiscussionsOnlyDocument { get; set; }

    [Keyword(Name = "collectionId")]
    public override string CollectionId { get; set; }

    [Nest.Text(Name = "collectionName")]
    public override string CollectionName { get; set; }

    [Keyword(Name = "indexedTimeStamp")]
    public override int IndexedTimeStamp { get; set; }

    [Keyword(Name = "projectId")]
    public string ProjectId { get; set; }

    [Nest.Text(Name = "projectName")]
    public string ProjectName { get; set; }

    [Object(Name = "discussionRevisions")]
    public List<int> DiscussionsRevisions { get; set; }

    [Object(Name = "fields")]
    public IDictionary<string, object> Fields { get; set; }

    [Object(Name = "nonAnalyzedFields")]
    public IDictionary<string, object> NonAnalyzedFields { get; set; }

    [Keyword(Ignore = true)]
    public override string ParentDocumentId
    {
      get => (string) null;
      set => throw new NotImplementedException();
    }

    [StringEnum]
    public override DocumentContractType ContractType => DocumentContractType.WorkItemContract;

    [Keyword(Ignore = true)]
    public override long? PreviousDocumentVersion
    {
      get => new long?();
      set => throw new NotImplementedException();
    }

    [Keyword(Ignore = true)]
    public override long CurrentDocumentVersion { get; set; }

    public void PopulateFileContractDetails(
      IVssRequestContext requestContext,
      object data,
      IMetaDataStore treeStore,
      ParsedData workItemParsedData,
      ParsedData discussionsParsedData,
      IMetaDataStoreItem documentItem,
      bool? indexIdentityFields)
    {
      int documentId = int.Parse(documentItem.DocumentId, (IFormatProvider) CultureInfo.InvariantCulture);
      List<DiscussionEntity> discussionEntityList = new List<DiscussionEntity>();
      Dictionary<string, WorkItemField> fieldDefinitions = (Dictionary<string, WorkItemField>) data;
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem = !this.IsDiscussionsOnlyDocument ? this.GetWorkItemFromWorkItemBytes(workItemParsedData.Content, documentId) : this.GetWorkItemWithOnlyDiscussions(discussionsParsedData.Content);
      List<DiscussionEntity> fromDiscussionBytes = this.GetDiscussionsFromDiscussionBytes(discussionsParsedData.Content, documentId);
      this.PopulateFileContractDetails(requestContext, treeStore, workItem, fromDiscussionBytes, (IDictionary<string, WorkItemField>) fieldDefinitions, indexIdentityFields);
    }

    internal virtual void PopulateFileContractDetails(
      IVssRequestContext requestContext,
      IMetaDataStore treeStore,
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem,
      List<DiscussionEntity> discussionEntityList,
      IDictionary<string, WorkItemField> fieldDefinitions,
      bool? indexIdentityFields)
    {
      this.CollectionId = treeStore["CollectionId"];
      this.CollectionName = treeStore["CollectionName"];
      this.ProjectId = treeStore["ProjectId"];
      this.ProjectName = treeStore["ProjectName"];
      this.DocumentId = FormattableString.Invariant(FormattableStringFactory.Create("{0}@{1}", (object) this.CollectionId, (object) workItem.Id));
      this.DiscussionsRevisions = this.AddDiscussionStringsAndExtractRevisionsFromWorkItemFields(workItem, discussionEntityList);
      if (!this.IsDiscussionsOnlyDocument && !workItem.Fields.ContainsKey("System.IsDeleted"))
        workItem.Fields["System.IsDeleted"] = (object) false;
      this.Item = workItem.Id.ToString();
      this.Fields = (IDictionary<string, object>) new FriendlyDictionary<string, object>();
      this.NonAnalyzedFields = (IDictionary<string, object>) new FriendlyDictionary<string, object>();
      foreach (KeyValuePair<string, object> field in (IEnumerable<KeyValuePair<string, object>>) workItem.Fields)
      {
        WorkItemField witField = (WorkItemField) null;
        if (!fieldDefinitions.TryGetValue(field.Key, out witField))
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080355, "Indexing Pipeline", "Feed", FormattableString.Invariant(FormattableStringFactory.Create("Could not find the field {0} in the field cache.", (object) field.Key)));
        }
        else
        {
          WorkItemIndexedField indexedField = (WorkItemIndexedField) null;
          object fieldValue = field.Value;
          try
          {
            if (indexIdentityFields.Value && witField.IsIdentity)
            {
              string identityName = fieldValue as string;
              indexedField = WorkItemIndexedField.FromWitField(witField.ReferenceName, WorkItemContract.FieldType.Identity);
              this.SetPlatformFieldValue(indexedField, fieldValue);
              indexedField = WorkItemIndexedField.FromWitField(witField.ReferenceName, WorkItemContract.FieldType.Name);
              this.SetPlatformFieldValue(indexedField, (object) this.ExtractDisplayNameFromIdentityString(identityName));
            }
            else
            {
              indexedField = WorkItemIndexedField.FromWitField(witField);
              this.SetPlatformFieldValue(indexedField, fieldValue);
            }
          }
          catch (Exception ex) when (WorkItemContract.LogError(workItem, indexedField, ex))
          {
          }
        }
      }
    }

    private void SetPlatformFieldValue(WorkItemIndexedField indexedField, object fieldValue)
    {
      if (indexedField.Type == WorkItemContract.FieldType.Html)
      {
        if (!(fieldValue is string))
        {
          if (fieldValue is IEnumerable enumerable)
          {
            List<string> stringList = new List<string>();
            foreach (object obj in enumerable)
              stringList.Add(HtmlTextAnalyzer.Clean(WorkItemContract.ConvertSafelyToString(obj)));
            fieldValue = (object) stringList;
          }
        }
        else
          fieldValue = (object) HtmlTextAnalyzer.Clean(WorkItemContract.ConvertSafelyToString(fieldValue));
        if (indexedField.ReferenceName.Equals("Microsoft.VSTS.TCM.Steps", StringComparison.OrdinalIgnoreCase))
          fieldValue = (object) HtmlTextAnalyzer.Clean(WorkItemContract.ConvertSafelyToString(fieldValue));
      }
      if (indexedField.Type == WorkItemContract.FieldType.Real)
        fieldValue = this.SanitizeLargeDoubleValues(fieldValue);
      if (string.IsNullOrWhiteSpace(WorkItemContract.ConvertSafelyToString(fieldValue)))
        return;
      this.Fields[indexedField.ContractFieldName] = fieldValue;
      if (indexedField.ShouldBeIndexedAsString)
        this.Fields[indexedField.AsStringIndexedField.ContractFieldName] = fieldValue;
      if (indexedField.IsCompositeFieldEligible)
      {
        object obj;
        if (this.Fields.TryGetValue(indexedField.CompositeContractFieldName, out obj))
          (obj as List<object>).Add(fieldValue);
        else
          this.Fields[indexedField.CompositeContractFieldName] = (object) new List<object>()
          {
            fieldValue
          };
      }
      if (!indexedField.IsEligibleForNonAnalyzedIndex)
        return;
      if (indexedField.ContractFieldName.Equals(WorkItemContract.ContractFieldNames.Tags, StringComparison.Ordinal))
      {
        string[] array = ((IEnumerable<string>) WorkItemContract.ConvertSafelyToString(fieldValue).ToLowerInvariant().Split(new char[2]
        {
          ';',
          ','
        }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (tag => tag.Trim())).ToArray<string>();
        this.NonAnalyzedFields[indexedField.ContractFieldName] = (object) array;
      }
      else
        this.NonAnalyzedFields[indexedField.ContractFieldName] = (object) WorkItemContract.ConvertSafelyToString(fieldValue).ToLowerInvariant();
    }

    private object SanitizeLargeDoubleValues(object fieldValue) => fieldValue is double num && float.IsInfinity(Convert.ToSingle(num)) ? (object) null : fieldValue;

    internal string ExtractDisplayNameFromIdentityString(string identityName)
    {
      if (string.IsNullOrWhiteSpace(identityName))
        return string.Empty;
      int length = identityName.IndexOf('<');
      return length > 0 ? identityName.Substring(0, length).Trim() : identityName.Trim();
    }

    private static bool LogError(
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem,
      WorkItemIndexedField indexedField,
      Exception ex)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080355, "Indexing Pipeline", "Feed", FormattableString.Invariant(FormattableStringFactory.Create("Exception while parsing {0} on field {1} with exception {2}", (object) workItem.Id, (object) indexedField.ReferenceName, (object) ex)));
      return false;
    }

    public static string ConvertSafelyToString(object value) => value is string str ? str : Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture);

    public override string GetStoredFieldForFieldName(string field) => throw new NotImplementedException(FormattableString.Invariant(FormattableStringFactory.Create("Use {0}.{1} instead.", (object) "WorkItemIndexedField", (object) "PlatformFieldName")));

    public override string GetSearchFieldForType(string type) => type;

    public override int GetSize() => this.m_size;

    public override EntitySearchPlatformResponse Search(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request)
    {
      throw new NotImplementedException();
    }

    public override ResultsCountPlatformResponse Count(
      IVssRequestContext requestContext,
      ResultsCountPlatformRequest request)
    {
      throw new NotImplementedException();
    }

    public override EntitySearchSuggestPlatformResponse Suggest(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request,
      EntitySearchSuggestPlatformRequest searchSuggestRequest)
    {
      throw new NotImplementedException();
    }

    public override bool IsSourceEnabled(IVssRequestContext requestContext) => true;

    public override string GetStoredFieldValue(string field, string fieldValue) => fieldValue;

    public override string GetFieldNameForStoredField(string storedField)
    {
      WorkItemIndexedField itemIndexedField = WorkItemIndexedField.FromPlatformFieldName(storedField);
      if (itemIndexedField.IsWitField)
        return itemIndexedField.ReferenceName;
      return itemIndexedField.IsInternalField ? itemIndexedField.PlatformFieldName : throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("There is no service field name for platform field name [{0}].", (object) storedField)));
    }

    private List<int> AddDiscussionStringsAndExtractRevisionsFromWorkItemFields(
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem,
      List<DiscussionEntity> discussionEntityList)
    {
      List<int> fromWorkItemFields = new List<int>();
      if (discussionEntityList != null && discussionEntityList.Any<DiscussionEntity>())
      {
        List<string> stringList = new List<string>();
        discussionEntityList = discussionEntityList.OrderBy<DiscussionEntity, int>((Func<DiscussionEntity, int>) (d => d.Revision)).ToList<DiscussionEntity>();
        foreach (DiscussionEntity discussionEntity in discussionEntityList)
        {
          stringList.Add(discussionEntity.Discussion);
          fromWorkItemFields.Add(discussionEntity.Revision);
        }
        workItem.Fields["System.History"] = (object) stringList;
      }
      return fromWorkItemFields;
    }

    private List<DiscussionEntity> GetDiscussionsFromDiscussionBytes(
      byte[] discussionEntityListBytes,
      int documentId)
    {
      if (discussionEntityListBytes == null || ((IEnumerable<byte>) CrawlerHelpers.GetDummyByteContent()).SequenceEqual<byte>((IEnumerable<byte>) discussionEntityListBytes))
        return (List<DiscussionEntity>) null;
      this.m_size += discussionEntityListBytes.Length;
      string str = Encoding.UTF8.GetString(discussionEntityListBytes);
      return (!string.IsNullOrEmpty(str) ? JsonConvert.DeserializeObject<List<DiscussionEntity>>(str) : throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Failed to encode discussionEntityListBytes to discussionEntityListJson for workItem id : {0}", (object) documentId)))) ?? throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Failed to deserialize discussionEntityListJson to List<DiscussionEntity> object for workItem id : {0}", (object) documentId)));
    }

    private Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem GetWorkItemFromWorkItemBytes(
      byte[] workItemBytes,
      int documentId)
    {
      if (workItemBytes == null)
        return (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem) null;
      this.m_size += workItemBytes.Length;
      string str = Encoding.UTF8.GetString(workItemBytes);
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem fromWorkItemBytes = !string.IsNullOrEmpty(str) ? JsonConvert.DeserializeObject<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>(str) : throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Failed to encode workItemBlob to workItemsJson for workItem id : {0}", (object) documentId)));
      if (fromWorkItemBytes == null)
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Failed to deserialize workItemsJson to WorkItem object for workItem id : {0}", (object) documentId)));
      fromWorkItemBytes.Fields.Remove("System.History");
      return fromWorkItemBytes;
    }

    private Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem GetWorkItemWithOnlyDiscussions(
      byte[] discussionEntityBlob)
    {
      List<DiscussionEntity> discussionEntityList = JsonConvert.DeserializeObject<List<DiscussionEntity>>(Encoding.UTF8.GetString(discussionEntityBlob));
      if (discussionEntityList == null || discussionEntityList.Count == 0)
        return (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem) null;
      DiscussionEntity discussionEntity = discussionEntityList[discussionEntityList.Count - 1];
      int revision = discussionEntity.Revision;
      int id = discussionEntity.Id;
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem withOnlyDiscussions = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem();
      IDictionary<string, object> dictionary = (IDictionary<string, object>) new FriendlyDictionary<string, object>()
      {
        ["System.History"] = (object) discussionEntityList,
        ["System.Id"] = (object) id,
        ["System.Rev"] = (object) revision
      };
      withOnlyDiscussions.Id = new int?(id);
      withOnlyDiscussions.Fields = dictionary;
      return withOnlyDiscussions;
    }

    public enum FieldType
    {
      None,
      Boolean,
      Real,
      Integer,
      String,
      Path,
      Html,
      DateTime,
      IntegerAsString,
      Identity,
      Name,
      AllTypes,
    }

    public static class PlatformFieldNames
    {
      public const string CollectionId = "collectionId";
      public const string CollectionName = "collectionName";
      public const string ProjectName = "projectName";
      public const string ProjectId = "projectId";
      public const string DiscussionRevisions = "discussionRevisions";
      public const string IsDiscussionsOnlyDocument = "isDiscussionOnly";
      public const string WitFieldsPrefix = "fields";
      public static readonly string Id = WorkItemIndexedField.FromWitField("System.Id", WorkItemContract.FieldType.Integer).PlatformFieldName;
      public static readonly string WorkItemType = WorkItemIndexedField.FromWitField("System.WorkItemType", WorkItemContract.FieldType.String).PlatformFieldName;
      public static readonly string Title = WorkItemIndexedField.FromWitField("System.Title", WorkItemContract.FieldType.String).PlatformFieldName;
      public static readonly string AssignedTo = WorkItemIndexedField.FromWitField("System.AssignedTo", WorkItemContract.FieldType.String).PlatformFieldName;
      public static readonly string AssignedToIdentity = WorkItemIndexedField.FromWitField("System.AssignedTo", WorkItemContract.FieldType.Identity).PlatformFieldName;
      public static readonly string AssignedToName = WorkItemIndexedField.FromWitField("System.AssignedTo", WorkItemContract.FieldType.Name).PlatformFieldName;
      public static readonly string State = WorkItemIndexedField.FromWitField("System.State", WorkItemContract.FieldType.String).PlatformFieldName;
      public static readonly string Tags = WorkItemIndexedField.FromWitField("System.Tags", WorkItemContract.FieldType.String).PlatformFieldName;
      public static readonly string AreaId = WorkItemIndexedField.FromWitField("System.AreaId", WorkItemContract.FieldType.Integer).PlatformFieldName;
      public static readonly string AreaPath = WorkItemIndexedField.FromWitField("System.AreaPath", WorkItemContract.FieldType.Path).PlatformFieldName;
      public static readonly string IterationId = WorkItemIndexedField.FromWitField("System.IterationId", WorkItemContract.FieldType.Integer).PlatformFieldName;
      public static readonly string IterationPath = WorkItemIndexedField.FromWitField("System.IterationPath", WorkItemContract.FieldType.Path).PlatformFieldName;
      public static readonly string Revision = WorkItemIndexedField.FromWitField("System.Rev", WorkItemContract.FieldType.Integer).PlatformFieldName;
      public static readonly string TeamProject = WorkItemIndexedField.FromWitField("System.TeamProject", WorkItemContract.FieldType.String).PlatformFieldName;
      public static readonly string ChangedDate = WorkItemIndexedField.FromWitField("System.CreatedDate", WorkItemContract.FieldType.DateTime).PlatformFieldName;
      public static readonly string CreatedDate = WorkItemIndexedField.FromWitField("System.ChangedDate", WorkItemContract.FieldType.DateTime).PlatformFieldName;
      public static readonly string Description = WorkItemIndexedField.FromWitField("System.Description", WorkItemContract.FieldType.Html).PlatformFieldName;
      public static readonly string IsDeleted = WorkItemIndexedField.FromWitField("System.IsDeleted", WorkItemContract.FieldType.Boolean).PlatformFieldName;
    }

    public static class NonAnalyzedPlatformFieldNames
    {
      public const string WitFieldsPrefix = "nonAnalyzedFields";
      public static readonly string WorkItemType = WorkItemIndexedField.FromWitField("System.WorkItemType", WorkItemContract.FieldType.String).NonAnalyzedPlatformFieldName;
      public static readonly string Title = WorkItemIndexedField.FromWitField("System.Title", WorkItemContract.FieldType.String).NonAnalyzedPlatformFieldName;
      public static readonly string AssignedTo = WorkItemIndexedField.FromWitField("System.AssignedTo", WorkItemContract.FieldType.String).NonAnalyzedPlatformFieldName;
      public static readonly string AssignedToIdentity = WorkItemIndexedField.FromWitField("System.AssignedTo", WorkItemContract.FieldType.Identity).NonAnalyzedPlatformFieldName;
      public static readonly string AssignedToName = WorkItemIndexedField.FromWitField("System.AssignedTo", WorkItemContract.FieldType.Name).NonAnalyzedPlatformFieldName;
      public static readonly string State = WorkItemIndexedField.FromWitField("System.State", WorkItemContract.FieldType.String).NonAnalyzedPlatformFieldName;
    }

    public static class ContractFieldNames
    {
      public const string CollectionId = "collectionId";
      public const string CollectionName = "collectionName";
      public const string ProjectName = "projectName";
      public const string ProjectId = "projectId";
      public static readonly string Id = WorkItemIndexedField.FromWitField("System.Id", WorkItemContract.FieldType.Integer).ContractFieldName;
      public static readonly string WorkItemType = WorkItemIndexedField.FromWitField("System.WorkItemType", WorkItemContract.FieldType.String).ContractFieldName;
      public static readonly string Title = WorkItemIndexedField.FromWitField("System.Title", WorkItemContract.FieldType.String).ContractFieldName;
      public static readonly string AssignedTo = WorkItemIndexedField.FromWitField("System.AssignedTo", WorkItemContract.FieldType.String).ContractFieldName;
      public static readonly string AssignedToIdentity = WorkItemIndexedField.FromWitField("System.AssignedTo", WorkItemContract.FieldType.Identity).ContractFieldName;
      public static readonly string AssignedToName = WorkItemIndexedField.FromWitField("System.AssignedTo", WorkItemContract.FieldType.Name).ContractFieldName;
      public static readonly string State = WorkItemIndexedField.FromWitField("System.State", WorkItemContract.FieldType.String).ContractFieldName;
      public static readonly string Tags = WorkItemIndexedField.FromWitField("System.Tags", WorkItemContract.FieldType.String).ContractFieldName;
      public static readonly string AreaId = WorkItemIndexedField.FromWitField("System.AreaId", WorkItemContract.FieldType.Integer).ContractFieldName;
      public static readonly string AreaPath = WorkItemIndexedField.FromWitField("System.AreaPath", WorkItemContract.FieldType.Path).ContractFieldName;
      public static readonly string IterationId = WorkItemIndexedField.FromWitField("System.IterationId", WorkItemContract.FieldType.Integer).ContractFieldName;
      public static readonly string IterationPath = WorkItemIndexedField.FromWitField("System.IterationPath", WorkItemContract.FieldType.Path).ContractFieldName;
      public static readonly string Revision = WorkItemIndexedField.FromWitField("System.Rev", WorkItemContract.FieldType.Integer).ContractFieldName;
      public static readonly string TeamProject = WorkItemIndexedField.FromWitField("System.TeamProject", WorkItemContract.FieldType.String).ContractFieldName;
      public static readonly string ChangedDate = WorkItemIndexedField.FromWitField("System.CreatedDate", WorkItemContract.FieldType.DateTime).ContractFieldName;
      public static readonly string CreatedDate = WorkItemIndexedField.FromWitField("System.ChangedDate", WorkItemContract.FieldType.DateTime).ContractFieldName;
      public static readonly string Description = WorkItemIndexedField.FromWitField("System.Description", WorkItemContract.FieldType.Html).ContractFieldName;
      public static readonly string History = WorkItemIndexedField.FromWitField("System.History", WorkItemContract.FieldType.Html).ContractFieldName;
      public static readonly string IsDeleted = WorkItemIndexedField.FromWitField("System.IsDeleted", WorkItemContract.FieldType.Boolean).ContractFieldName;
    }

    public static class ServiceFieldNames
    {
      public const string ProjectName = "projectName";
      public const string ProjectId = "projectId";
      public static readonly string Id = "System.Id".ToLowerInvariant();
      public static readonly string Type = "System.WorkItemType".ToLowerInvariant();
      public static readonly string Title = "System.Title".ToLowerInvariant();
      public static readonly string AssignedTo = "System.AssignedTo".ToLowerInvariant();
      public static readonly string State = "System.State".ToLowerInvariant();
      public static readonly string Tags = "System.Tags".ToLowerInvariant();
      public static readonly string Revision = "System.Rev".ToLowerInvariant();
      public static readonly string CreatedDate = "System.CreatedDate".ToLowerInvariant();
      public static readonly string ChangedDate = "System.ChangedDate".ToLowerInvariant();
    }
  }
}
