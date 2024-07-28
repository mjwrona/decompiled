// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.WorkItemTypeExtensionComponent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class WorkItemTypeExtensionComponent : WorkItemTrackingResourceComponent
  {
    private const int c_duplicateWorkItemTypeletNameException = 600451;
    private const int c_duplicateBehaviorTypeletNameException = 604003;
    private const int c_behaviorLimitExceeded = 600453;
    private static readonly IDictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = (IDictionary<int, SqlExceptionFactory>) new Dictionary<int, SqlExceptionFactory>();
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[23]
    {
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent>(1),
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent2>(2),
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent3>(3),
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent4>(4),
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent5>(5),
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent6>(6),
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent7>(7),
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent8>(8),
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent9>(9),
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent10>(10),
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent11>(11),
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent12>(12),
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent13>(13),
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent14>(14),
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent15>(15),
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent16>(16),
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent17>(17),
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent18>(18),
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent19>(19),
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent20>(20),
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent21>(21),
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent22>(22),
      (IComponentCreator) new ComponentCreator<WorkItemTypeExtensionComponent23>(23)
    }, "WorkItemTypeExtension", "WorkItem");

    static WorkItemTypeExtensionComponent()
    {
      WorkItemTypeExtensionComponent.s_sqlExceptionFactories[600177] = new SqlExceptionFactory(typeof (ArgumentException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((r, i, ex, e) => (Exception) new ArgumentException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemStringInvalidException())));
      WorkItemTypeExtensionComponent.s_sqlExceptionFactories[600451] = new SqlExceptionFactory(typeof (ArgumentException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((r, i, ex, e) => (Exception) new ProcessWorkItemTypeAlreadyExistException()));
      WorkItemTypeExtensionComponent.s_sqlExceptionFactories[604003] = new SqlExceptionFactory(typeof (ArgumentException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((r, i, ex, e) => (Exception) new BehaviorNameInUseException()));
      WorkItemTypeExtensionComponent.s_sqlExceptionFactories[600453] = new SqlExceptionFactory(typeof (ArgumentException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((r, i, ex, e) => (Exception) new PortfolioBehaviorLimitExceededException()));
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => WorkItemTypeExtensionComponent.s_sqlExceptionFactories;

    public static WorkItemTypeExtensionComponent CreateComponent(IVssRequestContext requestContext) => requestContext.CreateComponent<WorkItemTypeExtensionComponent>();

    protected virtual int MaxPredicateLength => 4000;

    public virtual WorkItemTypeletRecord UpdateExtension(
      Guid extensionId,
      Guid projectId,
      Guid ownerId,
      Guid changedBy,
      string name,
      string description,
      IList<CustomFieldEntry> fields,
      WorkItemExtensionPredicate predicate,
      IList<WorkItemFieldRule> fieldRules,
      string form,
      int rank)
    {
      return this.UpdateExtension(extensionId, projectId, ownerId, changedBy, name, description, fields, predicate, fieldRules, form);
    }

    public virtual WorkItemTypeletRecord UpdateExtension(
      Guid extensionId,
      Guid projectId,
      Guid ownerId,
      Guid changedBy,
      string name,
      string description,
      IList<CustomFieldEntry> fields,
      WorkItemExtensionPredicate predicate,
      IList<WorkItemFieldRule> fieldRules,
      string ignoredForm)
    {
      this.PrepareStoredProcedure("prc_UpdateWorkItemTypeExtension");
      this.BindGuid("@eventAuthor", this.Author);
      this.BindExtension(extensionId, projectId, ownerId, name, description, fields, predicate, fieldRules);
      return this.ReadExtensions().FirstOrDefault<WorkItemTypeletRecord>();
    }

    public virtual WorkItemTypeletRecord CreateExtension(
      Guid extensionId,
      Guid projectId,
      Guid ownerId,
      Guid changedBy,
      string name,
      string description,
      IList<CustomFieldEntry> fields,
      WorkItemExtensionPredicate predicate,
      IList<WorkItemFieldRule> fieldRules,
      string form,
      int rank)
    {
      return this.CreateExtension(extensionId, projectId, ownerId, changedBy, name, description, fields, predicate, fieldRules, form);
    }

    public virtual WorkItemTypeletRecord CreateExtension(
      Guid extensionId,
      Guid projectId,
      Guid ownerId,
      Guid changedBy,
      string name,
      string description,
      IList<CustomFieldEntry> fields,
      WorkItemExtensionPredicate predicate,
      IList<WorkItemFieldRule> fieldRules,
      string ignoredForm)
    {
      this.PrepareStoredProcedure("prc_CreateWorkItemTypeExtension");
      this.BindGuid("@eventAuthor", this.Author);
      this.BindExtension(extensionId, projectId, ownerId, name, description, fields, predicate, fieldRules);
      return this.ReadExtensions().FirstOrDefault<WorkItemTypeletRecord>();
    }

    private void BindExtension(
      Guid extensionId,
      Guid projectId,
      Guid ownerId,
      string name,
      string description,
      IList<CustomFieldEntry> fields,
      WorkItemExtensionPredicate predicate,
      IList<WorkItemFieldRule> fieldRules)
    {
      string serializedRuleXml = fieldRules == null || !fieldRules.Any<WorkItemFieldRule>() ? (string) null : CommonWITUtils.GetSerializedRuleXML(fieldRules.ToArray<WorkItemFieldRule>());
      string parameterValue = predicate == null ? (string) null : TeamFoundationSerializationUtility.SerializeToString<WorkItemExtensionPredicate>(predicate);
      this.BindGuid("@id", extensionId);
      this.BindGuid("@projectId", projectId);
      this.BindGuid("@ownerId", ownerId);
      this.BindString("@name", name, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@description", description, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      if (parameterValue != null && this.MaxPredicateLength > 0 && parameterValue.Length > this.MaxPredicateLength)
        throw new NotSupportedException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.ErrorSerializedExtensionPredicateTooLong());
      this.BindString("@predicate", parameterValue, this.MaxPredicateLength, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@fieldRules", serializedRuleXml, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindCustomFieldTable("@fields", (IEnumerable<CustomFieldEntry>) fields);
    }

    public void UpdateExtensionReconciliationStatus(
      Guid extensionId,
      WorkItemTypeExtensionReconciliationStatus reconciliationStatus,
      string reconciliationMessage,
      out bool everReconciled)
    {
      this.PrepareStoredProcedure("prc_UpdateWorkItemTypeExtensionReconciliationStatus");
      this.BindGuid("@id", extensionId);
      this.BindInt("@reconciliationStatus", (int) reconciliationStatus);
      this.BindString("@reconciliationMessage", reconciliationMessage, 4000, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("@eventAuthor", this.Author);
      IDataReader dataReader = this.ExecuteReader();
      everReconciled = false;
      if (!dataReader.Read())
        return;
      everReconciled = dataReader.GetBoolean(0);
    }

    public WorkItemTypeExtensionReconciliationStatus GetReconciliationStatus(
      Guid extensionId,
      out bool everReconciled)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemTypeExtensionReconciliationStatus");
      this.BindGuid("@id", extensionId);
      IDataReader dataReader = this.ExecuteReader();
      everReconciled = false;
      WorkItemTypeExtensionReconciliationStatus reconciliationStatus = WorkItemTypeExtensionReconciliationStatus.NeverReconciled;
      if (dataReader.Read())
      {
        reconciliationStatus = (WorkItemTypeExtensionReconciliationStatus) dataReader.GetInt32(0);
        everReconciled = dataReader.GetBoolean(1);
      }
      return reconciliationStatus;
    }

    internal virtual void SetReconciliationWatermark(Guid extensionId, Guid reconciliationWatermark)
    {
      this.PrepareStoredProcedure("prc_SetWorkItemTypeExtensionReconciliationWatermark");
      this.BindGuid("@id", extensionId);
      this.BindGuid("@reconciliationWatermark", reconciliationWatermark);
      this.BindGuid("@eventAuthor", this.Author);
      this.ExecuteNonQueryEx();
    }

    internal virtual void SetReconciliationWatermarks(
      IEnumerable<Guid> extensionIds,
      Guid reconciliationWatermark)
    {
      foreach (Guid extensionId in extensionIds)
        this.SetReconciliationWatermark(extensionId, reconciliationWatermark);
    }

    internal Guid GetReconciliationWatermark(Guid extensionId)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemTypeExtensionReconciliationWatermark");
      this.BindGuid("@id", extensionId);
      IDataReader dataReader = this.ExecuteReader();
      return dataReader.Read() && !dataReader.IsDBNull(0) ? dataReader.GetGuid(0) : Guid.Empty;
    }

    public virtual List<WorkItemTypeletRecord> GetExtensionsById(IList<Guid> ids)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemTypeExtensionsByIds");
      this.BindGuidTable("@ids", (IEnumerable<Guid>) ids);
      this.BindBoolean("@includePredicate", true);
      this.BindBoolean("@includeRules", true);
      this.BindBoolean("@includeFields", true);
      return this.ReadExtensions();
    }

    public virtual List<WorkItemTypeletRecord> GetExtensions(Guid? projectId, Guid? ownerId)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemTypeExtensions");
      if (projectId.HasValue)
        this.BindGuid("@projectId", projectId.Value);
      else
        this.BindNullValue("@projectId", SqlDbType.UniqueIdentifier);
      if (ownerId.HasValue)
        this.BindGuid("@ownerId", ownerId.Value);
      else
        this.BindNullValue("@ownerId", SqlDbType.UniqueIdentifier);
      this.BindBoolean("@includePredicate", true);
      this.BindBoolean("@includeRules", true);
      this.BindBoolean("@includeFields", true);
      return this.ReadExtensions();
    }

    protected virtual WorkItemTypeExtensionBinder GetWorkItemTypeExtensionBinder() => new WorkItemTypeExtensionBinder();

    protected List<WorkItemTypeletRecord> ReadExtensions() => this.ReadExtensions((ObjectBinder<WorkItemTypeletRecord>) this.GetWorkItemTypeExtensionBinder());

    protected List<WorkItemTypeletRecord> ReadExtensions(ObjectBinder<WorkItemTypeletRecord> binder) => this.ReadExtensions(new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext), binder);

    protected List<WorkItemTypeletRecord> ReadExtensions(
      ResultCollection rc,
      ObjectBinder<WorkItemTypeletRecord> binder)
    {
      rc.AddBinder<WorkItemTypeletRecord>(binder);
      rc.AddBinder<Tuple<Guid, int>>((ObjectBinder<Tuple<Guid, int>>) new TupleBinder<Guid, int>());
      List<WorkItemTypeletRecord> items = rc.GetCurrent<WorkItemTypeletRecord>().Items;
      Dictionary<Guid, WorkItemTypeletRecord> dictionary = items.ToDictionary<WorkItemTypeletRecord, Guid, WorkItemTypeletRecord>((System.Func<WorkItemTypeletRecord, Guid>) (e => e.Id), (System.Func<WorkItemTypeletRecord, WorkItemTypeletRecord>) (e => e));
      rc.NextResult();
      foreach (IGrouping<Guid, int> source in rc.GetCurrent<Tuple<Guid, int>>().Items.GroupBy<Tuple<Guid, int>, Guid, int>((System.Func<Tuple<Guid, int>, Guid>) (t => t.Item1), (System.Func<Tuple<Guid, int>, int>) (t => t.Item2)))
      {
        WorkItemTypeletRecord itemTypeletRecord;
        if (dictionary.TryGetValue(source.Key, out itemTypeletRecord))
          itemTypeletRecord.Fields = source.Select<int, WorkItemTypeletFieldRecord>((System.Func<int, WorkItemTypeletFieldRecord>) (fieldId => new WorkItemTypeletFieldRecord()
          {
            FieldId = fieldId
          })).ToArray<WorkItemTypeletFieldRecord>();
      }
      return items;
    }

    public virtual List<int> GetExtendedWorkItemIds(Guid typeletId, int markedField)
    {
      this.PrepareStoredProcedure("prc_GetExtendedWorkItemIds");
      this.BindNullableGuid("@extensionId", typeletId);
      this.BindInt("@markerFieldId", markedField);
      IDataReader dataReader = this.ExecuteReader();
      List<int> extendedWorkItemIds = new List<int>();
      while (dataReader.Read())
        extendedWorkItemIds.Add(dataReader.GetInt32(0));
      return extendedWorkItemIds;
    }

    public virtual IEnumerable<IGrouping<Guid, string>> GetWorkItemCategoryDetailsForWITExtensionReconciliation(
      IList<int> categoryIds,
      IList<int> categoryMemberIds,
      IList<Tuple<int, string>> categoryReferenceNames)
    {
      return Enumerable.Empty<IGrouping<Guid, string>>();
    }

    internal virtual bool DeleteExtensions(IEnumerable<Guid> extensionIds, Guid changedBy) => false;

    public virtual List<WorkItemTypeletRecord> GetWorkItemTypelets(Guid processId) => new List<WorkItemTypeletRecord>();

    public virtual List<Tuple<string, DateTime>> GetWorkItemTypeletsReferenceNamesDeletedSince(
      DateTime deletedSince,
      int typeletType,
      out DateTime asOf)
    {
      asOf = DateTime.MinValue;
      return new List<Tuple<string, DateTime>>();
    }

    public virtual WorkItemTypeletRecord CreateWorkItemTypelet(
      Guid extensionId,
      Guid processId,
      string name,
      string refName,
      string description,
      string parentTypeRefName,
      IList<int> fields,
      IList<WorkItemFieldRule> fieldRules,
      IList<Guid> disabledRules,
      string form,
      Guid changedBy,
      string color = null,
      string icon = null,
      IReadOnlyCollection<WorkItemStateDeclaration> states = null,
      bool? isDisabled = null)
    {
      return (WorkItemTypeletRecord) null;
    }

    public virtual WorkItemTypeletRecord UpdateWorkItemTypelet(
      Guid extensionId,
      Guid processId,
      string description,
      IList<int> fields,
      IList<WorkItemFieldRule> fieldRules,
      IList<Guid> disabledRules,
      string form,
      Guid changedBy,
      DateTime readVersion = default (DateTime),
      string color = null,
      string icon = null,
      bool? isDisabled = null)
    {
      return (WorkItemTypeletRecord) null;
    }

    protected virtual SqlParameter BindWitConstantTable(
      string parameterName,
      IEnumerable<string> constants)
    {
      return (SqlParameter) null;
    }

    internal virtual void EnsureConstantsForBackcompat(IList<string> constants) => throw new NotSupportedException();

    internal virtual void AddIgnoreCaseBindingForConstants(bool ignoreCase)
    {
    }

    internal virtual void DestroyWorkItemTypelets(Guid processId, Guid changedBy, Guid witId) => throw new NotSupportedException();

    internal virtual void CreateWorkItemTypeBehaviorReference(
      Guid processId,
      Guid witId,
      string behaviorReferenceName,
      Guid changedBy,
      bool isDefault)
    {
      throw new NotSupportedException();
    }

    internal virtual void DeleteWorkItemTypeBehaviorReference(
      Guid processId,
      Guid witId,
      string behaviorReferenceName,
      Guid changedBy)
    {
      throw new NotSupportedException();
    }

    public virtual WorkItemTypeletRecord CreateBehavior(
      Guid extensionId,
      Guid processId,
      string name,
      string refName,
      string parentTypeRefName,
      Guid changedBy,
      string color,
      int rank,
      bool isAbstract,
      int limitCount)
    {
      throw new NotSupportedException();
    }

    internal virtual List<WorkItemTypeletRecord> ReadTypelets(
      ObjectBinder<WorkItemTypeletRecord> binder,
      ObjectBinder<WorkItemTypeExtensionBehaviorRecord> behaviorRecordBinder)
    {
      throw new NotSupportedException();
    }

    internal virtual void UpdateDefaultWorkItemTypeForBehavior(
      Guid processId,
      Guid witId,
      string behaviorReferenceName,
      Guid changerId,
      bool isDefault)
    {
      throw new NotSupportedException();
    }

    public virtual WorkItemTypeletRecord UpdateBehavior(
      Guid extensionId,
      Guid processId,
      string refName,
      string name,
      string color,
      bool overridden,
      Guid changedBy)
    {
      throw new NotSupportedException();
    }

    public virtual void DeleteBehavior(
      Guid extensionId,
      Guid processId,
      string refName,
      Guid changedBy)
    {
      throw new NotSupportedException();
    }

    public IEnumerable<WorkItemTypeletFieldProperties> GetAllWorkItemTypeletFieldProperties() => this.GetWorkItemTypeletFieldPropertiesInternal();

    public IEnumerable<WorkItemTypeletFieldProperties> GetWorkItemTypeletFieldProperties(
      Guid typeletId)
    {
      return this.GetWorkItemTypeletFieldPropertiesInternal(new Guid?(typeletId));
    }

    protected virtual IEnumerable<WorkItemTypeletFieldProperties> GetWorkItemTypeletFieldPropertiesInternal(
      Guid? typeletId = null)
    {
      return (IEnumerable<WorkItemTypeletFieldProperties>) null;
    }

    protected virtual SqlParameter BindCustomFieldTable(
      string parameterName,
      IEnumerable<CustomFieldEntry> fieldEntries)
    {
      return this.BindBasicTvp<CustomFieldEntry>((WorkItemTrackingResourceComponent.TvpRecordBinder<CustomFieldEntry>) new WorkItemTypeExtensionComponent.CustomFieldTableRecordBinder(), parameterName, fieldEntries);
    }

    protected class CustomFieldTableRecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<CustomFieldEntry>
    {
      protected static readonly SqlMetaData[] s_metadata = new SqlMetaData[9]
      {
        new SqlMetaData("Name", SqlDbType.NVarChar, 128L),
        new SqlMetaData("ReferenceName", SqlDbType.NVarChar, 386L),
        new SqlMetaData("Type", SqlDbType.Int),
        new SqlMetaData("ReportingType", SqlDbType.Int),
        new SqlMetaData("ReportingFormula", SqlDbType.Int),
        new SqlMetaData("ReportingEnabled", SqlDbType.Bit),
        new SqlMetaData("ReportingName", SqlDbType.NVarChar, 128L),
        new SqlMetaData("ReportingReferenceName", SqlDbType.NVarChar, 386L),
        new SqlMetaData("Usage", SqlDbType.Int)
      };

      public override string TypeName => "typ_WitCustomFieldTable2";

      protected override SqlMetaData[] TvpMetadata => WorkItemTypeExtensionComponent.CustomFieldTableRecordBinder.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        CustomFieldEntry fieldEntry)
      {
        record.SetString(0, fieldEntry.Name);
        record.SetString(1, fieldEntry.ReferenceName);
        record.SetInt32(2, fieldEntry.Type);
        record.SetInt32(3, fieldEntry.ReportingType);
        record.SetInt32(4, fieldEntry.ReportingFormula);
        record.SetBoolean(5, fieldEntry.ReportingEnabled);
        if (fieldEntry.ReportingName != null)
          record.SetString(6, fieldEntry.ReportingName);
        if (fieldEntry.ReportingReferenceName != null)
          record.SetString(7, fieldEntry.ReportingReferenceName);
        record.SetInt32(8, fieldEntry.Usage);
      }
    }
  }
}
