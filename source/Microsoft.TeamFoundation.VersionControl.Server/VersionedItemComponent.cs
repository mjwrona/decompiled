// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.VersionedItemComponent
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class VersionedItemComponent : VersionControlSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[15]
    {
      (IComponentCreator) new ComponentCreator<VersionedItemComponent>(1),
      (IComponentCreator) new ComponentCreator<VersionedItemComponent18>(18),
      (IComponentCreator) new ComponentCreator<VersionedItemComponent19>(19),
      (IComponentCreator) new ComponentCreator<VersionedItemComponent20>(20),
      (IComponentCreator) new ComponentCreator<VersionedItemComponent21>(21),
      (IComponentCreator) new ComponentCreator<VersionedItemComponent22>(22),
      (IComponentCreator) new ComponentCreator<VersionedItemComponent23>(23),
      (IComponentCreator) new ComponentCreator<VersionedItemComponent24>(24),
      (IComponentCreator) new ComponentCreator<VersionedItemComponent25>(25),
      (IComponentCreator) new ComponentCreator<VersionedItemComponent26>(26),
      (IComponentCreator) new ComponentCreator<VersionedItemComponent27>(27),
      (IComponentCreator) new ComponentCreator<VersionedItemComponent28>(28),
      (IComponentCreator) new ComponentCreator<VersionedItemComponent29>(29),
      (IComponentCreator) new ComponentCreator<VersionedItemComponent30>(30),
      (IComponentCreator) new ComponentCreator<VersionedItemComponent31>(31)
    }, "VersionedItem");
    private static readonly SqlMetaData[] typ_PolicyOverride = new SqlMetaData[2]
    {
      new SqlMetaData("PolicyName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Message", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_VersionControlPropertyValue = new SqlMetaData[6]
    {
      new SqlMetaData("PropertyName", SqlDbType.NVarChar, 400L),
      new SqlMetaData("IntValue", SqlDbType.Int),
      new SqlMetaData("DatetimeValue", SqlDbType.DateTime),
      new SqlMetaData("StringValue", SqlDbType.NVarChar, -1L),
      new SqlMetaData("DoubleValue", SqlDbType.Float),
      new SqlMetaData("BinaryValue", SqlDbType.VarBinary, -1L)
    };
    private static readonly SqlMetaData[] typ_PendingChangeSecurity = new SqlMetaData[6]
    {
      new SqlMetaData("SourceItemId", SqlDbType.Int),
      new SqlMetaData("TargetItemId", SqlDbType.Int),
      new SqlMetaData("FailedSecurity", SqlDbType.Bit),
      new SqlMetaData("LockStatus", SqlDbType.TinyInt),
      new SqlMetaData("FailedPatternMatch", SqlDbType.Bit),
      new SqlMetaData("FailedRestrictions", SqlDbType.Bit)
    };
    private static readonly SqlMetaData[] typ_WorkspaceLink = new SqlMetaData[2]
    {
      new SqlMetaData("LinkType", SqlDbType.Int),
      new SqlMetaData("Url", SqlDbType.VarChar, 2048L)
    };
    protected static readonly SqlMetaData[] typ_ItemSpec = new SqlMetaData[4]
    {
      new SqlMetaData("Item", SqlDbType.NVarChar, -1L),
      new SqlMetaData("IsServerItem", SqlDbType.Bit),
      new SqlMetaData("Depth", SqlDbType.TinyInt),
      new SqlMetaData("InputIndex", SqlDbType.Int)
    };
    protected static readonly SqlMetaData[] typ_PendingAdd = new SqlMetaData[5]
    {
      new SqlMetaData("ServerItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ItemType", SqlDbType.TinyInt),
      new SqlMetaData("Encoding", SqlDbType.Int),
      new SqlMetaData("LocalItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("LockStatus", SqlDbType.TinyInt)
    };
    protected static readonly SqlMetaData[] typ_PendingMerge = new SqlMetaData[4]
    {
      new SqlMetaData("WorkspaceId", SqlDbType.Int),
      new SqlMetaData("SequenceId", SqlDbType.Int),
      new SqlMetaData("TargetServerItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("IsRenameSource", SqlDbType.Bit)
    };
    protected static readonly SqlMetaData[] typ_PendingPropertyChange = new SqlMetaData[3]
    {
      new SqlMetaData("ServerItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("PropertyId", SqlDbType.Int),
      new SqlMetaData("LockStatus", SqlDbType.TinyInt)
    };
    private static readonly SqlMetaData[] typ_Mapping = new SqlMetaData[4]
    {
      new SqlMetaData("ServerItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("LocalItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("MappingType", SqlDbType.Bit),
      new SqlMetaData("Depth", SqlDbType.TinyInt)
    };
    protected static readonly SqlMetaData[] typ_ExpandedChange = new SqlMetaData[6]
    {
      new SqlMetaData("ServerItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("TargetServerItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("LocalItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("RequestIndex", SqlDbType.Int),
      new SqlMetaData("ItemId", SqlDbType.Int),
      new SqlMetaData("LockLevel", SqlDbType.TinyInt)
    };
    protected static readonly SqlMetaData[] typ_LocalVersion = new SqlMetaData[5]
    {
      new SqlMetaData("ItemIndex", SqlDbType.Int),
      new SqlMetaData("ItemId", SqlDbType.Int),
      new SqlMetaData("VersionFrom", SqlDbType.Int),
      new SqlMetaData("ServerItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("LocalItem", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_ReleaseNoteDetail = new SqlMetaData[2]
    {
      new SqlMetaData("FieldName", SqlDbType.NVarChar, 64L),
      new SqlMetaData("BaseValue", SqlDbType.NVarChar, -1L)
    };
    protected static readonly string s_service = nameof (VersionedItemComponent);
    private static readonly SqlMetaData[] typ_ItemIdWithIndex = new SqlMetaData[2]
    {
      new SqlMetaData("OrderId", SqlDbType.Int),
      new SqlMetaData("ItemId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_ItemIdWithIndexAndDataspaceId = new SqlMetaData[3]
    {
      new SqlMetaData("OrderId", SqlDbType.Int),
      new SqlMetaData("ItemDataspaceId", SqlDbType.Int),
      new SqlMetaData("ItemId", SqlDbType.Int)
    };
    protected static readonly SqlMetaData[] typ_LocalPendingChange4 = new SqlMetaData[14]
    {
      new SqlMetaData("TargetItemDataspaceId", SqlDbType.Int),
      new SqlMetaData("TargetParentPath", SqlDbType.NVarChar, 435L),
      new SqlMetaData("TargetChildItem", SqlDbType.NVarChar, 400L),
      new SqlMetaData("CommittedItemDataspaceId", SqlDbType.Int),
      new SqlMetaData("CommittedServerItem", SqlDbType.NVarChar, 435L),
      new SqlMetaData("BranchFromItem", SqlDbType.NVarChar, 435L),
      new SqlMetaData("BranchFromVersion", SqlDbType.Int),
      new SqlMetaData("VersionFrom", SqlDbType.Int),
      new SqlMetaData("PendingCommand", SqlDbType.Int),
      new SqlMetaData("ItemType", SqlDbType.TinyInt),
      new SqlMetaData("Encoding", SqlDbType.Int),
      new SqlMetaData("LockStatus", SqlDbType.TinyInt),
      new SqlMetaData("CreationDate", SqlDbType.DateTime),
      new SqlMetaData("HasMergeConflict", SqlDbType.Bit)
    };
    protected static readonly SqlMetaData[] typ_LocalVersion4 = new SqlMetaData[8]
    {
      new SqlMetaData("ItemIndex", SqlDbType.Int),
      new SqlMetaData("ItemDataspaceId", SqlDbType.Int),
      new SqlMetaData("ItemId", SqlDbType.Int),
      new SqlMetaData("VersionFrom", SqlDbType.Int),
      new SqlMetaData("ServerItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("LocalItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("MappingLength", SqlDbType.SmallInt),
      new SqlMetaData("MappingLocalItemLength", SqlDbType.SmallInt)
    };
    protected static readonly SqlMetaData[] typ_LockObject2 = new SqlMetaData[3]
    {
      new SqlMetaData("ServerItem", SqlDbType.NVarChar, 435L),
      new SqlMetaData("LockType", SqlDbType.TinyInt),
      new SqlMetaData("RequiredLockType", SqlDbType.TinyInt)
    };
    private readonly SqlMetaData[] typ_Mapping3 = new SqlMetaData[6]
    {
      new SqlMetaData("ItemDataspaceId", SqlDbType.Int),
      new SqlMetaData("ProjectName", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ServerItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("LocalItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("MappingType", SqlDbType.Bit),
      new SqlMetaData("Depth", SqlDbType.TinyInt)
    };
    protected static readonly SqlMetaData[] typ_PendingAdd3 = new SqlMetaData[6]
    {
      new SqlMetaData("ItemDataspaceId", SqlDbType.Int),
      new SqlMetaData("ServerItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ItemType", SqlDbType.TinyInt),
      new SqlMetaData("Encoding", SqlDbType.Int),
      new SqlMetaData("LocalItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("LockStatus", SqlDbType.TinyInt)
    };
    private static readonly SqlMetaData[] typ_PendingChangeSecurity2 = new SqlMetaData[8]
    {
      new SqlMetaData("SourceItemDataspaceId", SqlDbType.Int),
      new SqlMetaData("SourceItemId", SqlDbType.Int),
      new SqlMetaData("TargetItemDatasapceId", SqlDbType.Int),
      new SqlMetaData("TargetItemId", SqlDbType.Int),
      new SqlMetaData("FailedSecurity", SqlDbType.Bit),
      new SqlMetaData("LockStatus", SqlDbType.TinyInt),
      new SqlMetaData("FailedPatternMatch", SqlDbType.Bit),
      new SqlMetaData("FailedRestrictions", SqlDbType.Bit)
    };
    protected static readonly SqlMetaData[] typ_PendingPropertyChange3 = new SqlMetaData[4]
    {
      new SqlMetaData("ServerItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("PropertyDataspaceId", SqlDbType.Int),
      new SqlMetaData("PropertyId", SqlDbType.Int),
      new SqlMetaData("LockStatus", SqlDbType.TinyInt)
    };
    private static readonly SqlMetaData[] typ_ServerItem = new SqlMetaData[2]
    {
      new SqlMetaData("ItemDataspaceId", SqlDbType.Int),
      new SqlMetaData("ServerItem", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_ItemPathPair = new SqlMetaData[3]
    {
      new SqlMetaData("ItemDataspaceId", SqlDbType.Int),
      new SqlMetaData("ItemNamePath", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ItemGuidPath", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_VersionRevertTo = new SqlMetaData[2]
    {
      new SqlMetaData("ItemId", SqlDbType.Int),
      new SqlMetaData("VersionRevertTo", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_LocalConflicts = new SqlMetaData[8]
    {
      new SqlMetaData("ConflictType", SqlDbType.Int),
      new SqlMetaData("ItemId", SqlDbType.Int),
      new SqlMetaData("VersionFrom", SqlDbType.Int),
      new SqlMetaData("PendingChangeId", SqlDbType.Int),
      new SqlMetaData("SourceLocalItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("TargetLocalItem", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Reason", SqlDbType.Int),
      new SqlMetaData("VersionRevertTo", SqlDbType.Int)
    };

    protected SqlParameter BindPolicyOverrideToTable(
      string parameterName,
      PolicyOverrideInfo policyOverride,
      out string overrideComment)
    {
      overrideComment = (string) null;
      if (policyOverride == null || policyOverride.Comment == null || policyOverride.Comment.Length <= 0)
        return this.BindPolicyFailureInfoTable(parameterName, (IEnumerable<PolicyFailureInfo>) null);
      overrideComment = policyOverride.Comment;
      return this.BindPolicyFailureInfoTable(parameterName, (IEnumerable<PolicyFailureInfo>) policyOverride.PolicyFailures);
    }

    protected virtual SqlParameter BindPolicyFailureInfoTable(
      string parameterName,
      IEnumerable<PolicyFailureInfo> rows)
    {
      rows = rows ?? Enumerable.Empty<PolicyFailureInfo>();
      System.Func<PolicyFailureInfo, SqlDataRecord> selector = (System.Func<PolicyFailureInfo, SqlDataRecord>) (failure =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(VersionedItemComponent.typ_PolicyOverride);
        sqlDataRecord.SetString(0, failure.PolicyName);
        sqlDataRecord.SetString(1, failure.Message == null ? string.Empty : failure.Message);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_PolicyOverride", rows.Select<PolicyFailureInfo, SqlDataRecord>(selector));
    }

    protected SqlParameter BindVersionControlPropertyValueTable(
      string parameterName,
      IEnumerable<PropertyValue> rows)
    {
      rows = rows ?? Enumerable.Empty<PropertyValue>();
      System.Func<PropertyValue, SqlDataRecord> selector = (System.Func<PropertyValue, SqlDataRecord>) (property =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(VersionedItemComponent.typ_VersionControlPropertyValue);
        sqlDataRecord.SetString(0, property.PropertyName);
        object obj = property.Value;
        if (obj != null)
        {
          int typeCode = (int) Type.GetTypeCode(obj.GetType());
          if (typeCode == 1)
          {
            byte[] buffer = (byte[]) obj;
            sqlDataRecord.SetBytes(5, 0L, buffer, 0, buffer.Length);
          }
          else
            sqlDataRecord.SetDBNull(5);
          if (typeCode == 9)
            sqlDataRecord.SetInt32(1, (int) obj);
          else
            sqlDataRecord.SetDBNull(1);
          if (typeCode == 14)
            sqlDataRecord.SetDouble(4, (double) obj);
          else
            sqlDataRecord.SetDBNull(4);
          if (typeCode == 16)
            sqlDataRecord.SetDateTime(2, (DateTime) obj);
          else
            sqlDataRecord.SetDBNull(2);
          if (typeCode == 18)
            sqlDataRecord.SetString(3, obj.ToString());
          else
            sqlDataRecord.SetDBNull(3);
        }
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_VersionControlPropertyValue", (IEnumerable<SqlDataRecord>) rows.Select<PropertyValue, SqlDataRecord>(selector).ToList<SqlDataRecord>());
    }

    protected virtual SqlParameter BindVersionControlLinkTable(
      string parameterName,
      IEnumerable<VersionControlLink> rows)
    {
      rows = rows ?? Enumerable.Empty<VersionControlLink>();
      System.Func<VersionControlLink, SqlDataRecord> selector = (System.Func<VersionControlLink, SqlDataRecord>) (link =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(VersionedItemComponent.typ_WorkspaceLink);
        sqlDataRecord.SetInt32(0, link.LinkType);
        sqlDataRecord.SetString(1, link.Url);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_WorkspaceLink", rows.Select<VersionControlLink, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindCheckinNoteFieldValueTable(
      string parameterName,
      IEnumerable<CheckinNoteFieldValue> rows)
    {
      rows = rows ?? Enumerable.Empty<CheckinNoteFieldValue>();
      System.Func<CheckinNoteFieldValue, SqlDataRecord> selector = (System.Func<CheckinNoteFieldValue, SqlDataRecord>) (checkinNote =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(VersionedItemComponent.typ_ReleaseNoteDetail);
        sqlDataRecord.SetString(0, checkinNote.Name);
        sqlDataRecord.SetString(1, checkinNote.Value == null ? string.Empty : checkinNote.Value);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ReleaseNoteDetail", rows.Select<CheckinNoteFieldValue, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindItemIdWithIndexTable(
      string parameterName,
      IEnumerable<int> rows)
    {
      rows = rows ?? Enumerable.Empty<int>();
      Func<int, int, SqlDataRecord> selector = (Func<int, int, SqlDataRecord>) ((itemId, itemIdIndex) =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(VersionedItemComponent.typ_ItemIdWithIndex);
        sqlDataRecord.SetInt32(0, itemIdIndex);
        sqlDataRecord.SetInt32(1, itemId);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ItemIdWithIndex", rows.Select<int, SqlDataRecord>(selector));
    }

    protected virtual DetermineLocalItemTypeColumns CreateDetermineLocalItemTypeColumns() => new DetermineLocalItemTypeColumns();

    protected virtual QueryPendingChangesFailureColumns CreateQueryPendingChangesFailureColumns() => new QueryPendingChangesFailureColumns(this.ProcedureName);

    public int FindChangesetByDate(DateTime date)
    {
      this.PrepareStoredProcedure("prc_FindChangeSetByDate");
      this.BindDateTime("@pointInTime", date);
      SqlParameter sqlParameter = this.Command.Parameters.Add("@changeSetId", SqlDbType.Int);
      sqlParameter.Direction = ParameterDirection.Output;
      this.ExecuteNonQuery();
      return (int) sqlParameter.Value;
    }

    public ResultCollection QueryServerItemByItemIds(int[] itemIds, int version, int options)
    {
      this.PrepareStoredProcedure("prc_QueryServerItemsByItemIds", 3600);
      this.BindItemIdWithIndexTable("@itemIds", (IEnumerable<int>) itemIds);
      this.BindInt("@version", version);
      this.BindInt("@options", options);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<Item>((ObjectBinder<Item>) this.CreateQueryItemsColumns());
      return resultCollection;
    }

    public int FindLatestChangeset()
    {
      this.PrepareStoredProcedure("prc_FindLatestChangeSet");
      SqlParameter sqlParameter = this.Command.Parameters.Add("@changeSetId", SqlDbType.Int);
      sqlParameter.Direction = ParameterDirection.Output;
      this.ExecuteNonQuery();
      return (int) sqlParameter.Value;
    }

    public ResultCollection FindPendingChangeById(int pendingChangeId)
    {
      this.PrepareStoredProcedure("prc_FindPendingChangeById");
      this.BindInt("@pendingChangeId", pendingChangeId);
      ResultCollection pendingChangeById = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      pendingChangeById.AddBinder<PendingChange>((ObjectBinder<PendingChange>) this.CreateQueryPendingChangesColumns(this.RequestContext));
      return pendingChangeById;
    }

    public void UpdateChangeset(int changeset, string comment, CheckinNote checkInNote)
    {
      this.PrepareStoredProcedure("prc_UpdateChangeSet");
      this.BindInt("@changeSetId", changeset);
      CheckinNoteFieldValue[] rows = (CheckinNoteFieldValue[]) null;
      if (checkInNote != null && checkInNote.Values != null && checkInNote.Values.Length != 0)
        rows = checkInNote.Values;
      this.BindCheckinNoteFieldValueTable("@checkInNoteList", (IEnumerable<CheckinNoteFieldValue>) rows);
      if (comment != null)
        this.BindXml("@comment", comment);
      else
        this.BindNullValue("@comment", SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public ResultCollection QueryChangeset(
      int changeset,
      bool includeChanges,
      bool includeSourceRenames)
    {
      this.PrepareStoredProcedure("prc_QueryChangeSet", 3600);
      this.BindInt("@changeSetId", changeset);
      this.BindBoolean("@includeChanges", includeChanges);
      this.BindBoolean("@includeSourceRenames", includeSourceRenames);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<Changeset>((ObjectBinder<Changeset>) new ChangesetColumns());
      resultCollection.AddBinder<PolicyFailureInfo>((ObjectBinder<PolicyFailureInfo>) new PolicyFailureColumns());
      resultCollection.AddBinder<CheckinNoteFieldValue>((ObjectBinder<CheckinNoteFieldValue>) new CheckinNoteColumns());
      if (includeChanges)
        resultCollection.AddBinder<Change>((ObjectBinder<Change>) this.CreateChangeColumns());
      return resultCollection;
    }

    public virtual ResultCollection QueryMergesForPendingChanges(List<PendingChange> changes)
    {
      this.PrepareStoredProcedure("prc_QueryMergesForPendingChanges");
      this.BindPendingMergeTable("@itemList", (IEnumerable<PendingMerge>) new PendingMergeCollection(changes));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<MergeSource>((ObjectBinder<MergeSource>) this.CreateMergeSourceColumns());
      return resultCollection;
    }

    public static ItemType EncodingToItemType(int encoding) => encoding == -3 ? ItemType.Folder : ItemType.File;

    public void DeleteUnusedContent()
    {
      this.PrepareStoredProcedure("prc_DeleteUnusedContent", 3600);
      this.ExecuteNonQuery();
    }

    public virtual ResultCollection QueryExpandedChanges(
      IEnumerable<ExpandedChange> changes,
      bool isRecursive)
    {
      this.PrepareStoredProcedure("prc_QueryExpandedChanges", 3600);
      this.BindExpandedChangeTable("@items", changes);
      this.BindBoolean("@recursive", isRecursive);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<ExpandedChange>((ObjectBinder<ExpandedChange>) this.CreateExpandedChangeColumns());
      return resultCollection;
    }

    public void ResetCheckinDates(DateTime lastCheckinDate)
    {
      if (lastCheckinDate < SqlDateTime.MinValue.Value)
        throw new InvalidSqlDateException(lastCheckinDate);
      this.PrepareStoredProcedure("prc_ResetCheckinDates");
      this.BindDateTime("@lastCheckinDate", lastCheckinDate);
      this.BindBoolean("@disableEmptyValidation", this.VersionControlRequestContext.VersionControlService.GetDisableResetCheckinDateEmptyValidation(this.VersionControlRequestContext));
      this.ExecuteNonQuery();
    }

    public int AcquirePendingChangeSecurityLock()
    {
      this.PrepareStoredProcedure("prc_AcquirePendingChangeSecurityLock");
      return (int) this.ExecuteScalar();
    }

    public void ReleasePendingChangeSecurityLock(
      int transactionId,
      List<PendChangeSecurity> items,
      bool completed)
    {
      this.PrepareStoredProcedure("prc_ReleasePendingChangeSecurityLock");
      this.BindInt("@transactionId", transactionId);
      this.BindPendChangeSecurityTable("@items", (IEnumerable<PendChangeSecurity>) items);
      this.BindBoolean("@completed", completed);
      this.ExecuteNonQuery();
    }

    public virtual void ConvertCheckoutLocks()
    {
      this.PrepareStoredProcedure("prc_ConvertCheckoutLocks");
      this.ExecuteNonQuery();
    }

    public virtual int ReservePropertyIds(int numIdsToReserve)
    {
      this.PrepareStoredProcedure("prc_ReservePropertyIds");
      this.BindInt("@numIdsToReserve", numIdsToReserve);
      return (int) this.ExecuteScalar();
    }

    protected virtual ReconcileResultColumns CreateReconcileResultColumns() => (ReconcileResultColumns) new ReconcileResultColumns2();

    public VersionedItemComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual void UpdateShelvesetCreationTime(
      string shelvesetName,
      Guid ownerId,
      DateTime? cutOffDate)
    {
      this.PrepareStoredProcedure("prc_UpdateShelvesetCreationTime");
      this.BindString("@shelveSetName", shelvesetName, 64, true, SqlDbType.NVarChar);
      this.BindGuid("@ownerId", ownerId);
      if (!cutOffDate.HasValue)
        this.BindNullValue("@cutOffDate", SqlDbType.DateTime);
      else
        this.BindDateTime("@cutOffDate", cutOffDate.Value);
      this.ExecuteNonQuery();
    }

    public virtual ResultCollection QueryChangesets(int[] changesets)
    {
      this.PrepareStoredProcedure("prc_QueryChangeSets", 3600);
      this.BindUniqueInt32Table("@changesets", (IEnumerable<int>) changesets);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<Changeset>((ObjectBinder<Changeset>) new ChangesetColumns());
      return resultCollection;
    }

    public virtual ResultCollection QueryChangesetOwners(bool includeCounts)
    {
      this.PrepareStoredProcedure("prc_QueryChangeSetOwners");
      this.BindBoolean("@includeCounts", includeCounts);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<ChangeSetOwner>((ObjectBinder<ChangeSetOwner>) new QueryUserIdentitiesColumns());
      return resultCollection;
    }

    public virtual ChangesetChangeTypeSummary QuerySummaryForChangeset(int changeset)
    {
      this.PrepareStoredProcedure("prc_QueryChangesetSummary");
      this.BindInt("@changeSetId", changeset);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext))
      {
        resultCollection.AddBinder<ChangesetChangeTypeSummary>((ObjectBinder<ChangesetChangeTypeSummary>) new ChangesetChangeTypeSummaryColumns(changeset));
        ObjectBinder<ChangesetChangeTypeSummary> current = resultCollection.GetCurrent<ChangesetChangeTypeSummary>();
        current.MoveNext();
        return current.Current;
      }
    }

    public virtual ResultCollection QueryShelvesets(
      Guid ownerId,
      string shelvesetName,
      int shelvesetVersion)
    {
      this.PrepareStoredProcedure("prc_QueryShelvesets", 3600);
      this.BindNullableGuid("@ownerId", ownerId);
      this.BindString("@shelvesetName", shelvesetName, 64, true, SqlDbType.NVarChar);
      this.BindInt("@shelvesetVersion", shelvesetVersion);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<Shelveset>((ObjectBinder<Shelveset>) new QueryShelvesetsColumns2());
      resultCollection.AddBinder<CheckinNoteFieldValue>((ObjectBinder<CheckinNoteFieldValue>) new CheckinNoteColumns());
      resultCollection.AddBinder<VersionControlLink>((ObjectBinder<VersionControlLink>) new LinkColumns());
      return resultCollection;
    }

    protected virtual IEnumerable<SqlDataRecord> BindExpandedChangeRows(
      IEnumerable<ExpandedChange> rows)
    {
      VersionedItemComponent versionedItemComponent = this;
      rows = rows ?? Enumerable.Empty<ExpandedChange>();
      foreach (ExpandedChange row in rows)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(VersionedItemComponent.typ_ExpandedChange);
        sqlDataRecord.SetString(0, DBPath.ServerToDatabasePath(versionedItemComponent.ConvertPathPairToPathWithProjectGuid(row.itemPathPair)));
        if (!string.IsNullOrEmpty(row.targetServerItem))
          sqlDataRecord.SetString(1, DBPath.ServerToDatabasePath(versionedItemComponent.ConvertPathPairToPathWithProjectGuid(row.targetItemPathPair)));
        else
          sqlDataRecord.SetDBNull(1);
        if (!string.IsNullOrEmpty(row.localItem))
          sqlDataRecord.SetString(2, DBPath.LocalToDatabasePath(row.localItem));
        else
          sqlDataRecord.SetDBNull(2);
        sqlDataRecord.SetInt32(3, row.requestIndex);
        sqlDataRecord.SetInt32(4, row.itemId);
        sqlDataRecord.SetByte(5, (byte) row.requiredLockLevel);
        yield return sqlDataRecord;
      }
    }

    protected virtual SqlParameter BindExpandedChangeTable(
      string parameterName,
      IEnumerable<ExpandedChange> rows)
    {
      rows = rows ?? Enumerable.Empty<ExpandedChange>();
      return this.BindTable(parameterName, "typ_ExpandedChange3", this.BindExpandedChangeRows((IEnumerable<ExpandedChange>) new List<ExpandedChange>(rows)));
    }

    protected virtual SqlParameter BindItemIdWithIndexTable(
      string parameterName,
      IEnumerable<Tuple<int, int>> rows)
    {
      rows = rows ?? Enumerable.Empty<Tuple<int, int>>();
      Func<Tuple<int, int>, int, SqlDataRecord> selector = (Func<Tuple<int, int>, int, SqlDataRecord>) ((tuple, itemIdIndex) =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(VersionedItemComponent.typ_ItemIdWithIndexAndDataspaceId);
        sqlDataRecord.SetInt32(0, itemIdIndex);
        sqlDataRecord.SetInt32(1, tuple.Item1);
        sqlDataRecord.SetInt32(2, tuple.Item2);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ItemIdWithIndexAndDataspaceId", rows.Select<Tuple<int, int>, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindItemSpecTable(
      string parameterName,
      IEnumerable<ItemSpec> rows)
    {
      rows = rows ?? Enumerable.Empty<ItemSpec>();
      Func<ItemSpec, int, SqlDataRecord> selector = (Func<ItemSpec, int, SqlDataRecord>) ((item, index) =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(VersionedItemComponent.typ_ItemSpec);
        sqlDataRecord.SetString(0, item.isServerItem ? DBPath.ServerToDatabasePath(this.ConvertPathPairToPathWithProjectGuid(item.ItemPathPair)) : DBPath.LocalToDatabasePath(item.Item));
        sqlDataRecord.SetBoolean(1, item.isServerItem);
        sqlDataRecord.SetByte(2, (byte) item.RecursionType);
        sqlDataRecord.SetInt32(3, index);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ItemSpec3", rows.Select<ItemSpec, SqlDataRecord>(selector));
    }

    protected virtual IEnumerable<SqlDataRecord> BindLocalPendingChangeRows(
      IEnumerable<LocalPendingChange> rows)
    {
      VersionedItemComponent versionedItemComponent = this;
      rows = rows ?? Enumerable.Empty<LocalPendingChange>();
      foreach (LocalPendingChange row in rows)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(VersionedItemComponent.typ_LocalPendingChange4);
        if (VersionControlPath.IsRootFolder(row.TargetServerItem))
        {
          sqlDataRecord.SetInt32(0, versionedItemComponent.GetDataspaceIdFromPath("$/"));
          sqlDataRecord.SetString(1, string.Empty);
          sqlDataRecord.SetString(2, DBPath.ServerToDatabasePath("$/"));
        }
        else
        {
          string convertedPath;
          Guid projectId;
          bool pathWithProjectId;
          if (!(pathWithProjectId = ProjectUtility.TryConvertToPathWithProjectId(versionedItemComponent.RequestContext, row.TargetServerItem, out convertedPath, out projectId, out string _)))
            projectId = Guid.Empty;
          sqlDataRecord.SetInt32(0, versionedItemComponent.GetDataspaceIdDebug(projectId, convertedPath));
          sqlDataRecord.SetString(1, DBPath.ServerToDatabasePath(VersionControlPath.GetFolderName(convertedPath)));
          sqlDataRecord.SetString(2, DBPath.ServerToDatabasePath(VersionControlPath.GetFileName(convertedPath, pathWithProjectId)));
        }
        if (!string.IsNullOrEmpty(row.CommittedServerItem))
        {
          string convertedPath;
          Guid projectId;
          string projectName;
          if (!ProjectUtility.TryConvertToPathWithProjectId(versionedItemComponent.RequestContext, row.CommittedServerItem, out convertedPath, out projectId, out projectName))
            throw new TeamProjectHasBeenDeletedException(projectName);
          int dataspaceIdDebug = versionedItemComponent.GetDataspaceIdDebug(projectId, convertedPath);
          sqlDataRecord.SetInt32(3, dataspaceIdDebug);
          sqlDataRecord.SetString(4, DBPath.ServerToDatabasePath(convertedPath));
        }
        else
        {
          sqlDataRecord.SetDBNull(3);
          sqlDataRecord.SetDBNull(4);
        }
        if (!string.IsNullOrEmpty(row.BranchFromItem) && row.BranchFromVersion > 0)
        {
          string convertedPath;
          Guid projectId;
          string projectName;
          if (!ProjectUtility.TryConvertToPathWithProjectId(versionedItemComponent.RequestContext, row.BranchFromItem, out convertedPath, out projectId, out projectName))
            throw new TeamProjectHasBeenDeletedException(projectName);
          versionedItemComponent.GetDataspaceIdDebug(projectId, convertedPath);
          sqlDataRecord.SetString(5, DBPath.ServerToDatabasePath(convertedPath));
          sqlDataRecord.SetInt32(6, row.BranchFromVersion);
        }
        else
        {
          sqlDataRecord.SetDBNull(5);
          sqlDataRecord.SetDBNull(6);
        }
        sqlDataRecord.SetInt32(7, row.Version);
        sqlDataRecord.SetInt32(8, row.PendingCommand);
        sqlDataRecord.SetByte(9, row.ItemType);
        sqlDataRecord.SetInt32(10, row.Encoding);
        if (row.LockStatus > (byte) 0)
          sqlDataRecord.SetByte(11, row.LockStatus);
        else
          sqlDataRecord.SetDBNull(11);
        sqlDataRecord.SetDateTime(12, row.CreationDate);
        sqlDataRecord.SetBoolean(13, row.HasMergeConflict);
        yield return sqlDataRecord;
      }
    }

    protected virtual SqlParameter BindLocalPendingChangeTable(
      string parameterName,
      IEnumerable<LocalPendingChange> rows)
    {
      return this.BindTable(parameterName, "typ_LocalPendingChange4", this.BindLocalPendingChangeRows(rows));
    }

    protected virtual SqlParameter BindLocalVersionTable(
      string parameterName,
      IEnumerable<BaseLocalVersionUpdate> rows)
    {
      rows = rows ?? Enumerable.Empty<BaseLocalVersionUpdate>();
      Func<BaseLocalVersionUpdate, int, SqlDataRecord> selector = (Func<BaseLocalVersionUpdate, int, SqlDataRecord>) ((update, index) =>
      {
        SqlDataRecord record = new SqlDataRecord(VersionedItemComponent.typ_LocalVersion4);
        record.SetInt32(0, index);
        if (update is ServerItemLocalVersionUpdate)
        {
          ServerItemLocalVersionUpdate localVersionUpdate1 = (ServerItemLocalVersionUpdate) update;
          ServerItemLocalVersionUpdate localVersionUpdate2 = new ServerItemLocalVersionUpdate();
          localVersionUpdate2.TargetLocalItem = localVersionUpdate1.TargetLocalItem;
          localVersionUpdate2.LocalVersion = localVersionUpdate1.LocalVersion;
          localVersionUpdate2.ItemId = localVersionUpdate1.ItemId;
          string convertedPath;
          if (localVersionUpdate1.LocalVersion == 0)
          {
            Guid projectId;
            if (!ProjectUtility.TryConvertToPathWithProjectId(this.RequestContext, localVersionUpdate1.SourceServerItem, out convertedPath, out projectId, out string _))
              return (SqlDataRecord) null;
            record.SetInt32(1, this.GetDataspaceIdDebug(projectId, convertedPath));
          }
          else
          {
            int dataspaceId;
            convertedPath = this.BestEffortConvertToPathWithProjectGuid(localVersionUpdate1.SourceItemPathPair, out dataspaceId);
            if (dataspaceId == 0)
              record.SetDBNull(1);
            else
              record.SetInt32(1, dataspaceId);
          }
          localVersionUpdate2.SourceItemPathPair = ItemPathPair.FromServerItem(convertedPath);
          localVersionUpdate2.SetRecord4(record);
        }
        else
          update.SetRecord4(record);
        return record;
      });
      return this.BindTable(parameterName, "typ_LocalVersion4", rows.Select<BaseLocalVersionUpdate, SqlDataRecord>(selector).Where<SqlDataRecord>((System.Func<SqlDataRecord, bool>) (s => s != null)));
    }

    protected virtual SqlParameter BindLockRequestTable(
      string parameterName,
      IEnumerable<LockRequest> rows)
    {
      rows = rows ?? Enumerable.Empty<LockRequest>();
      System.Func<LockRequest, SqlDataRecord> selector = (System.Func<LockRequest, SqlDataRecord>) (request =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(VersionedItemComponent.typ_LockObject2);
        sqlDataRecord.SetString(0, DBPath.ServerToDatabasePath(this.ConvertToPathWithProjectGuid(request.TargetServerItem)));
        sqlDataRecord.SetByte(1, (byte) request.LockLevel);
        sqlDataRecord.SetByte(2, (byte) request.RequiredLockLevel);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_LockObject2", rows.Select<LockRequest, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindMappingTable(string parameterName, IEnumerable<Mapping> rows)
    {
      rows = rows ?? Enumerable.Empty<Mapping>();
      System.Func<Mapping, SqlDataRecord> selector = (System.Func<Mapping, SqlDataRecord>) (mapping =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(this.typ_Mapping3);
        int dataspaceId;
        string pathWithProjectGuid = this.ConvertPathPairToPathWithProjectGuid(mapping.ItemPathPair, out dataspaceId);
        sqlDataRecord.SetInt32(0, dataspaceId);
        sqlDataRecord.SetDBNull(1);
        sqlDataRecord.SetString(2, DBPath.ServerToDatabasePath(pathWithProjectGuid));
        if (mapping is WorkingFolder workingFolder2 && workingFolder2.Type == WorkingFolderType.Map && workingFolder2.LocalItem != null)
          sqlDataRecord.SetString(3, DBPath.LocalToDatabasePath(workingFolder2.LocalItem));
        else
          sqlDataRecord.SetDBNull(3);
        sqlDataRecord.SetBoolean(4, mapping.Type != WorkingFolderType.Cloak);
        sqlDataRecord.SetByte(5, (byte) mapping.Depth);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_Mapping3", rows.Select<Mapping, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindPendAddTable(
      string parameterName,
      IEnumerable<ExpandedChange> rows,
      ChangeRequest[] requests)
    {
      rows = rows ?? Enumerable.Empty<ExpandedChange>();
      int dataspaceId;
      System.Func<ExpandedChange, SqlDataRecord> selector = (System.Func<ExpandedChange, SqlDataRecord>) (change =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(VersionedItemComponent.typ_PendingAdd3);
        ChangeRequest request = requests[change.requestIndex];
        string pathWithProjectGuid = this.ConvertPathPairToPathWithProjectGuid(change.itemPathPair, out dataspaceId);
        sqlDataRecord.SetInt32(0, dataspaceId);
        sqlDataRecord.SetString(1, DBPath.ServerToDatabasePath(pathWithProjectGuid));
        sqlDataRecord.SetByte(2, (byte) request.ItemType);
        sqlDataRecord.SetInt32(3, request.Encoding);
        if (!string.IsNullOrEmpty(change.localItem))
          sqlDataRecord.SetString(4, DBPath.LocalToDatabasePath(change.localItem));
        else
          sqlDataRecord.SetDBNull(4);
        if (change.requiredLockLevel != LockLevel.Unchanged)
          sqlDataRecord.SetByte(5, (byte) change.requiredLockLevel);
        else
          sqlDataRecord.SetDBNull(5);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_PendingAdd3", rows.Select<ExpandedChange, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindPendChangeSecurityTable(
      string parameterName,
      IEnumerable<PendChangeSecurity> rows)
    {
      rows = rows ?? Enumerable.Empty<PendChangeSecurity>();
      System.Func<PendChangeSecurity, SqlDataRecord> selector = (System.Func<PendChangeSecurity, SqlDataRecord>) (item =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(VersionedItemComponent.typ_PendingChangeSecurity2);
        sqlDataRecord.SetInt32(0, item.SourceItemDataspaceId);
        sqlDataRecord.SetInt32(1, item.SourceItemId);
        sqlDataRecord.SetInt32(2, item.TargetItemDataspaceId);
        sqlDataRecord.SetInt32(3, item.TargetItemId);
        sqlDataRecord.SetBoolean(4, item.FailedSecurity);
        if (item.LockLevel != LockLevel.Unchanged)
          sqlDataRecord.SetByte(5, (byte) item.LockLevel);
        else
          sqlDataRecord.SetDBNull(5);
        sqlDataRecord.SetBoolean(6, item.FailedPatternMatch);
        sqlDataRecord.SetBoolean(7, item.FailedRestrictions);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_PendingChangeSecurity2", rows.Select<PendChangeSecurity, SqlDataRecord>(selector));
    }

    protected virtual IEnumerable<SqlDataRecord> BindPendingMergeRows(IEnumerable<PendingMerge> rows)
    {
      VersionedItemComponent versionedItemComponent = this;
      int index = 0;
      foreach (PendingMerge row in rows)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(VersionedItemComponent.typ_PendingMerge);
        sqlDataRecord.SetInt32(0, row.WorkspaceId);
        sqlDataRecord.SetInt32(1, index);
        sqlDataRecord.SetString(2, DBPath.ServerToDatabasePath(versionedItemComponent.ConvertToPathWithProjectGuid(row.TargetServerItem)));
        sqlDataRecord.SetBoolean(3, row.IsRenameSource);
        ++index;
        if (!row.IsCommitted)
          ++index;
        yield return sqlDataRecord;
      }
    }

    protected virtual SqlParameter BindPendingMergeTable(
      string parameterName,
      IEnumerable<PendingMerge> rows)
    {
      rows = rows ?? Enumerable.Empty<PendingMerge>();
      return this.BindTable(parameterName, "typ_PendingMerge3", this.BindPendingMergeRows((IEnumerable<PendingMerge>) new List<PendingMerge>(rows)));
    }

    protected virtual SqlParameter BindPendPropertyChangeTable(
      string parameterName,
      IEnumerable<ExpandedChange> rows)
    {
      rows = rows ?? Enumerable.Empty<ExpandedChange>();
      System.Func<ExpandedChange, SqlDataRecord> selector = (System.Func<ExpandedChange, SqlDataRecord>) (change =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(VersionedItemComponent.typ_PendingPropertyChange3);
        sqlDataRecord.SetString(0, DBPath.ServerToDatabasePath(this.ConvertPathPairToPathWithProjectGuid(change.itemPathPair)));
        sqlDataRecord.SetInt32(1, this.GetDataspaceIdDebug(change.dataspaceId, change.serverItem));
        sqlDataRecord.SetInt32(2, change.propertyId);
        if (change.requiredLockLevel != LockLevel.Unchanged)
          sqlDataRecord.SetByte(3, (byte) change.requiredLockLevel);
        else
          sqlDataRecord.SetDBNull(3);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_PendingPropertyChange3", rows.Select<ExpandedChange, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindServerItemTable(
      string parameterName,
      IEnumerable<string> rows,
      bool bestEffort = false)
    {
      rows = rows ?? Enumerable.Empty<string>();
      List<string> source = new List<string>(rows);
      return bestEffort ? this.BindStringTable(parameterName, source.Select<string, string>((System.Func<string, string>) (x => DBPath.ServerToDatabasePath(this.BestEffortConvertToPathWithProjectGuid(x))))) : this.BindStringTable(parameterName, source.Select<string, string>((System.Func<string, string>) (x => DBPath.ServerToDatabasePath(this.ConvertToPathWithProjectGuid(x)))));
    }

    protected virtual SqlParameter BindServerItemPairTable(
      string parameterName,
      IEnumerable<ItemPathPair> rows)
    {
      rows = rows ?? Enumerable.Empty<ItemPathPair>();
      return this.BindStringTable(parameterName, new List<ItemPathPair>(rows).Select<ItemPathPair, string>((System.Func<ItemPathPair, string>) (x => DBPath.ServerToDatabasePath(this.ConvertPathPairToPathWithProjectGuid(x)))));
    }

    protected virtual IEnumerable<SqlDataRecord> BindServerItemTypeRows(IEnumerable<string> rows)
    {
      VersionedItemComponent versionedItemComponent = this;
      foreach (string row in rows)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(VersionedItemComponent.typ_ServerItem);
        int dataspaceId;
        string pathWithProjectGuid = versionedItemComponent.ConvertToPathWithProjectGuid(row, out dataspaceId);
        sqlDataRecord.SetInt32(0, dataspaceId);
        sqlDataRecord.SetString(1, DBPath.ServerToDatabasePath(pathWithProjectGuid));
        yield return sqlDataRecord;
      }
    }

    protected virtual SqlParameter BindServerItemTypeTable(
      string parameterName,
      IEnumerable<string> rows)
    {
      rows = rows ?? Enumerable.Empty<string>();
      return this.BindTable(parameterName, "typ_ServerItem", this.BindServerItemTypeRows((IEnumerable<string>) rows.ToList<string>()));
    }

    protected virtual SqlParameter BindServerItemPairTypeTable(
      string parameterName,
      IEnumerable<ItemPathPair> rows)
    {
      rows = rows ?? Enumerable.Empty<ItemPathPair>();
      return this.BindTable(parameterName, "typ_ServerItem", this.BindServerItemTypeRows((IEnumerable<string>) rows.Select<ItemPathPair, string>((System.Func<ItemPathPair, string>) (x => x.ProjectGuidPath ?? x.ProjectNamePath)).ToList<string>()));
    }

    protected virtual IEnumerable<SqlDataRecord> BindItemPathPairTypeRows(
      IEnumerable<ItemPathPair> rows)
    {
      VersionedItemComponent versionedItemComponent = this;
      foreach (ItemPathPair row in rows)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(VersionedItemComponent.typ_ItemPathPair);
        if (row.ProjectGuidPath == null)
        {
          int dataspaceId;
          string pathWithProjectGuid = versionedItemComponent.ConvertToPathWithProjectGuid(row.ProjectNamePath, out dataspaceId);
          sqlDataRecord.SetInt32(0, dataspaceId);
          sqlDataRecord.SetString(1, DBPath.ServerToDatabasePath(row.ProjectNamePath));
          sqlDataRecord.SetString(2, DBPath.ServerToDatabasePath(pathWithProjectGuid));
        }
        else
        {
          sqlDataRecord.SetInt32(0, versionedItemComponent.GetDataspaceIdFromPath(row.ProjectGuidPath));
          sqlDataRecord.SetString(1, DBPath.ServerToDatabasePath(row.ProjectNamePath));
          sqlDataRecord.SetString(2, DBPath.ServerToDatabasePath(row.ProjectGuidPath));
        }
        yield return sqlDataRecord;
      }
    }

    protected virtual SqlParameter BindItemPathPairTypeTable(
      string parameterName,
      IEnumerable<ItemPathPair> rows)
    {
      rows = rows ?? Enumerable.Empty<ItemPathPair>();
      return this.BindTable(parameterName, "typ_ItemPathPair", this.BindItemPathPairTypeRows((IEnumerable<ItemPathPair>) rows.ToList<ItemPathPair>()));
    }

    protected virtual BasicFailureColumns CreateBasicFailureColumns(string sqlProcName) => (BasicFailureColumns) new BasicFailureColumns15(sqlProcName, (VersionControlSqlResourceComponent) this);

    protected virtual ChangeColumns CreateChangeColumns() => (ChangeColumns) new ChangeColumns15((VersionControlSqlResourceComponent) this);

    protected virtual ConflictDetailsColumns CreateConflictDetailsColumns() => (ConflictDetailsColumns) new ConflictDetailsColumns15((VersionControlSqlResourceComponent) this);

    protected virtual DetermineItemTypeColumns CreateDetermineItemTypeColumns() => (DetermineItemTypeColumns) new DetermineItemTypeColumns15((VersionControlSqlResourceComponent) this);

    protected virtual DestroyedItemBinder CreateDestroyedItemColumns() => (DestroyedItemBinder) new DestroyedItemBinder15((VersionControlSqlResourceComponent) this);

    protected virtual ExpandedChangeBinder CreateExpandedChangeColumns() => (ExpandedChangeBinder) new ExpandedChangeBinder15((VersionControlSqlResourceComponent) this);

    protected virtual GetOperationColumns CreateGetOperationColumns() => (GetOperationColumns) new GetOperationColumns15b((VersionControlSqlResourceComponent) this);

    protected virtual ExtendedItemColumns CreateExtendedItemColumns() => (ExtendedItemColumns) new ExtendedItemColumns15((VersionControlSqlResourceComponent) this);

    protected virtual MergeSourceBinder CreateMergeSourceColumns() => (MergeSourceBinder) new MergeSourceBinder15((VersionControlSqlResourceComponent) this);

    protected virtual PendChangeFailureBinder CreatePendChangeFailureColumns(
      PendChangeFailureBinder.Caller caller)
    {
      return (PendChangeFailureBinder) new PendChangeFailureBinder15(caller, this.RequestContext, (VersionControlSqlResourceComponent) this);
    }

    protected virtual PendChangeFailureBinder CreatePendChangeFailureColumns(
      PendChangeFailureBinder.Caller caller,
      SeverityType severityType)
    {
      return (PendChangeFailureBinder) new PendChangeFailureBinder15(caller, this.RequestContext, severityType, (VersionControlSqlResourceComponent) this);
    }

    protected virtual PendWarningColumns CreatePendWarningColumns() => (PendWarningColumns) new PendWarningColumns15((VersionControlSqlResourceComponent) this);

    protected virtual QueryItemsColumns CreateQueryItemsColumns() => (QueryItemsColumns) new QueryItemsColumns15b((VersionControlSqlResourceComponent) this);

    protected virtual QueryItemsByChangesetColumns CreateQueryItemsByChangesetColumns() => new QueryItemsByChangesetColumns((VersionControlSqlResourceComponent) this);

    protected virtual QueryPendingChangesColumns CreateQueryPendingChangesColumns(
      IVssRequestContext requestContext)
    {
      return (QueryPendingChangesColumns) new QueryPendingChangesColumns15(requestContext, (VersionControlSqlResourceComponent) this);
    }

    protected virtual QueryPendingChangesForCheckinBinder CreateQueryPendingChangesForCheckinColumns() => (QueryPendingChangesForCheckinBinder) new QueryPendingChangesForCheckinBinder15((VersionControlSqlResourceComponent) this);

    protected virtual ServerItemBinder CreateServerItemColumns() => (ServerItemBinder) new ServerItemBinder15((VersionControlSqlResourceComponent) this);

    protected virtual QueryWorkspaceItemsColumns CreateQueryWorkspaceItemsColumns() => (QueryWorkspaceItemsColumns) new QueryWorkspaceItemsColumns15((VersionControlSqlResourceComponent) this);

    protected virtual QueryTfvcFileStatsColumns CreateTfvcFileStatsColumns() => new QueryTfvcFileStatsColumns((VersionControlSqlResourceComponent) this);

    internal virtual ResultCollection Checkin(
      Guid workspaceOwnerId,
      string workspaceName,
      PendingSetType workspaceType,
      IEnumerable<ItemPathPair> itemManager,
      string comment,
      DateTime creationDate,
      Guid ownerIdentifier,
      CheckinNote checkInNote,
      PolicyOverrideInfo policyOverride,
      Guid committerIdentifier,
      bool allowUnchangedContent,
      bool readyToCommit,
      bool returnLocalVersionUpdates,
      int itemCount,
      PathLength maxServerPathLength)
    {
      return this.Checkin(workspaceOwnerId, workspaceName, workspaceType, itemManager.Select<ItemPathPair, string>((System.Func<ItemPathPair, string>) (x => x.ProjectGuidPath ?? x.ProjectNamePath)), comment, creationDate, ownerIdentifier, checkInNote, policyOverride, committerIdentifier, allowUnchangedContent, readyToCommit, returnLocalVersionUpdates, itemCount, maxServerPathLength);
    }

    public virtual ResultCollection Checkin(
      Guid workspaceOwnerId,
      string workspaceName,
      PendingSetType workspaceType,
      IEnumerable<string> itemManager,
      string comment,
      DateTime creationDate,
      Guid ownerIdentifier,
      CheckinNote checkInNote,
      PolicyOverrideInfo policyOverride,
      Guid committerIdentifier,
      bool allowUnchangedContent,
      bool readyToCommit,
      bool returnLocalVersionUpdates,
      int itemCount,
      PathLength maxServerPathLength)
    {
      this.VersionControlRequestContext.LatestChangeset = 0;
      string sqlStatement = "EXEC prc_CheckIn @partitionId, @serviceDataspaceId, @teamFoundationId, @workspaceName, @workspaceType, @comment, @creationDate, @ownerIdentifier, @checkInNoteList, \r\n                                                @overrideComment, @policyOverrides, @itemList, @committerIdentifier, @allowUnchangedContent, @readyToCommit, \r\n                                                @returnLocalVersionUpdates, @maxServerPathLength, @dataspaceToPathMapping WITH RECOMPILE";
      if (itemCount < this.VersionControlRequestContext.VersionControlService.GetSprocRecompileThreshold(this.VersionControlRequestContext))
      {
        this.PrepareStoredProcedure("prc_CheckIn", 3600);
      }
      else
      {
        this.PrepareSqlBatch(sqlStatement.Length, 3600);
        this.AddStatement(sqlStatement);
      }
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@teamFoundationId", workspaceOwnerId);
      this.BindString("@workspaceName", workspaceName, 64, true, SqlDbType.NVarChar);
      this.BindInt("@workspaceType", (int) workspaceType);
      this.BindXml("@comment", comment);
      if (!VersionControlSqlResourceComponent.IsDateNull(creationDate))
      {
        if (creationDate < SqlDateTime.MinValue.Value)
          throw new InvalidSqlDateException(creationDate);
        this.BindDateTime("@creationDate", creationDate);
      }
      else
        this.BindNullValue("@creationDate", SqlDbType.DateTime);
      this.BindNullableGuid("@ownerIdentifier", ownerIdentifier);
      CheckinNoteFieldValue[] rows = (CheckinNoteFieldValue[]) null;
      if (checkInNote != null && checkInNote.Values != null && checkInNote.Values.Length != 0)
        rows = checkInNote.Values;
      this.BindCheckinNoteFieldValueTable("@checkInNoteList", (IEnumerable<CheckinNoteFieldValue>) rows);
      string overrideComment;
      this.BindPolicyOverrideToTable("@policyOverrides", policyOverride, out overrideComment);
      this.BindString("@overrideComment", overrideComment, 2048, true, SqlDbType.NVarChar);
      this.BindServerItemTable("@itemList", itemManager);
      this.BindGuid("@committerIdentifier", committerIdentifier);
      this.BindBoolean("@allowUnchangedContent", allowUnchangedContent);
      this.BindBoolean("@readyToCommit", readyToCommit);
      this.BindBoolean("@returnLocalVersionUpdates", returnLocalVersionUpdates);
      this.BindPathWithGuidLength("@maxServerPathLength", maxServerPathLength);
      Dictionary<string, KeyValuePair<string, int>> dictionary = new Dictionary<string, KeyValuePair<string, int>>((IEqualityComparer<string>) TFStringComparer.VersionControlPath);
      if (this.VersionControlRequestContext.VersionControlService.DebugDataspace(this.VersionControlRequestContext) != Guid.Empty)
      {
        foreach (string str1 in itemManager)
        {
          for (string str2 = str1; VersionControlPath.GetFolderDepth(str2) > 0; str2 = VersionControlPath.GetFolderName(str2))
          {
            if (!dictionary.ContainsKey(str2))
            {
              int dataspaceId;
              string pathWithProjectGuid = this.ConvertToPathWithProjectGuid(str2, out dataspaceId);
              dictionary[str2] = new KeyValuePair<string, int>(DBPath.ServerToDatabasePath(pathWithProjectGuid), dataspaceId);
            }
          }
        }
      }
      this.BindKeyValuePairStringInt32Table("@dataspaceToPathMapping", (IEnumerable<KeyValuePair<string, int>>) dictionary.Values);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<int>((ObjectBinder<int>) new CheckinColumns());
      resultCollection.AddBinder<string>((ObjectBinder<string>) new UndoneChangesColumns15((VersionControlSqlResourceComponent) this));
      resultCollection.AddBinder<DateTime>((ObjectBinder<DateTime>) new CreationDateColumns());
      resultCollection.AddBinder<PendingChangeConflict>((ObjectBinder<PendingChangeConflict>) new PendingChangeConflictColumns15(this.RequestContext, (VersionControlSqlResourceComponent) this));
      if (returnLocalVersionUpdates)
        resultCollection.AddBinder<GetOperation>((ObjectBinder<GetOperation>) this.CreateGetOperationColumns());
      return resultCollection;
    }

    public virtual ResultCollection CreateBranch(
      Guid ownerId,
      Guid committerId,
      string source,
      string target,
      VersionSpec to,
      Changeset info,
      CheckinNote checkInNote,
      List<Mapping> mappings,
      bool returnFailures,
      PathLength maxServerPathLength)
    {
      this.VersionControlRequestContext.LatestChangeset = 0;
      this.PrepareStoredProcedure("prc_CreateBranch", 3600);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", ownerId);
      this.BindGuid("@committerId", committerId);
      this.BindDataspaceIdAndServerItem("@sourceItemDataspaceId", "@sourceServerItem", source, false);
      this.BindDataspaceIdAndServerItem("@targetItemDataspaceId", "@targetServerItem", target, false);
      this.PrepareAndBindVersionSpec("@versionSpec", to, false);
      this.BindString("@comment", info.Comment, -1, true, SqlDbType.NVarChar);
      this.BindBoolean("@returnFailures", returnFailures);
      this.BindPathWithGuidLength("@maxServerPathLength", maxServerPathLength);
      if (!VersionControlSqlResourceComponent.IsDateNull(info.CreationDate))
        this.BindDateTime("@creationDate", info.CreationDate);
      else
        this.BindNullValue("@creationDate", SqlDbType.DateTime);
      CheckinNoteFieldValue[] rows = (CheckinNoteFieldValue[]) null;
      if (checkInNote != null && checkInNote.Values != null && checkInNote.Values.Length != 0)
        rows = checkInNote.Values;
      this.BindCheckinNoteFieldValueTable("@checkInNoteList", (IEnumerable<CheckinNoteFieldValue>) rows);
      string overrideComment;
      this.BindPolicyOverrideToTable("@policyOverrides", info.PolicyOverride, out overrideComment);
      this.BindString("@overrideComment", overrideComment, 2048, true, SqlDbType.NVarChar);
      this.BindMappingTable("@mappingList", (IEnumerable<Mapping>) mappings);
      ResultCollection branch = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      if (returnFailures)
        branch.AddBinder<Failure>((ObjectBinder<Failure>) new PendChangeFailureBinder15(PendChangeFailureBinder.Caller.Merge, this.RequestContext, (VersionControlSqlResourceComponent) this));
      branch.AddBinder<int>((ObjectBinder<int>) new CheckinColumns());
      return branch;
    }

    public virtual void DeleteBranchObject(ItemIdentifier item)
    {
      this.PrepareStoredProcedure("prc_DeleteBranchObject");
      this.BindServerItemPathPair("@rootServerItem", item.ItemPathPair, false);
      this.ExecuteNonQuery();
    }

    public virtual void DeleteShelveset(Guid ownerId, string shelvesetName, int shelvesetVersion)
    {
      this.PrepareStoredProcedure("prc_DeleteShelveset");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", ownerId);
      this.BindString("@shelvesetName", shelvesetName, 64, false, SqlDbType.NVarChar);
      this.BindInt("@shelvesetVersion", shelvesetVersion);
      this.ExecuteNonQuery();
    }

    public virtual ResultCollection Destroy(
      Item item,
      int stopAt,
      bool keepHistory,
      bool preview,
      bool silent,
      bool affectedChanges,
      string changesetComment,
      bool deleteWorkspaceState)
    {
      this.PrepareStoredProcedure("Tfvc.prc_Destroy", 0);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindDataspaceIdAndServerItemPathPair("@serverItemDataspaceId", "@serverItem", item.ItemPathPair, false);
      this.BindInt("@versionFrom", item.ChangesetId);
      this.BindInt("@stopAtVersion", stopAt);
      this.BindBoolean("@keepHistory", keepHistory);
      this.BindBoolean("@preview", preview);
      this.BindBoolean("@silent", silent);
      this.BindString("@comment", changesetComment, 2048, false, SqlDbType.NVarChar);
      this.BindBoolean("@affectedChanges", affectedChanges);
      this.BindGuid("@author", this.Author);
      this.BindBoolean("@deleteWorkspaceState", deleteWorkspaceState);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      if (silent)
        resultCollection.AddBinder<int>((ObjectBinder<int>) new NumberOfDestroyedItemsColumns());
      else
        resultCollection.AddBinder<Item>((ObjectBinder<Item>) this.CreateQueryItemsColumns());
      resultCollection.AddBinder<Failure>((ObjectBinder<Failure>) new DestroyFailureColumns15(this.RequestContext, (VersionControlSqlResourceComponent) this));
      if (affectedChanges)
      {
        resultCollection.AddBinder<PendingChange>((ObjectBinder<PendingChange>) this.CreateQueryPendingChangesColumns(this.RequestContext));
        resultCollection.AddBinder<PendingChange>((ObjectBinder<PendingChange>) this.CreateQueryPendingChangesColumns(this.RequestContext));
      }
      return resultCollection;
    }

    public virtual ResultCollection DetermineServerItemType(
      Workspace workspace,
      ItemPathPair itemPathPair)
    {
      this.PrepareStoredProcedure("prc_DetermineServerItemType");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindServerItemPathPair("@targetServerItem", itemPathPair, false);
      ResultCollection serverItemType = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      serverItemType.AddBinder<DeterminedItem>((ObjectBinder<DeterminedItem>) this.CreateDetermineItemTypeColumns());
      return serverItemType;
    }

    public virtual ResultCollection DetermineLocalItemType(
      Workspace workspace,
      string localItem,
      PathLength maxServerPathLength)
    {
      this.PrepareStoredProcedure("prc_DetermineLocalItemType");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindLocalItem("@localItem", localItem, false);
      ResultCollection localItemType = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      localItemType.AddBinder<DeterminedItem>((ObjectBinder<DeterminedItem>) new DetermineItemTypeColumns());
      return localItemType;
    }

    public virtual void AddConflict(
      Workspace workspace,
      ConflictType conflictType,
      int itemId,
      int versionFrom,
      int pendingChangeId,
      string sourceLocalItem,
      string targetLocalItem,
      int reason,
      int versionRevertTo,
      PathLength maxServerPathLength)
    {
      this.PrepareStoredProcedure("prc_AddLocalConflict");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindInt("@conflictType", (int) conflictType);
      this.BindInt("@itemId", itemId);
      this.BindInt("@versionFrom", versionFrom);
      this.BindInt("@pendingChangeId", pendingChangeId);
      this.BindLocalItem("@sourceLocalItem", sourceLocalItem, true);
      this.BindLocalItem("@targetLocalItem", targetLocalItem, true);
      this.BindInt("@reason", reason);
      this.BindInt("@versionRevertTo", versionRevertTo);
      this.ExecuteNonQuery();
    }

    public virtual ResultCollection GenerateExtantItemIds(
      Workspace workspace,
      IEnumerable<ItemPathPair> pathPairs)
    {
      this.PrepareStoredProcedure("prc_GenerateExtantItemIds");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindItemPathPairTypeTable("@itemList", pathPairs);
      ResultCollection extantItemIds = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      extantItemIds.AddBinder<Item>((ObjectBinder<Item>) this.CreateDestroyedItemColumns());
      return extantItemIds;
    }

    public virtual ResultCollection Get(
      Workspace workspace,
      string localItem,
      VersionSpec version,
      RecursionType recursive,
      GetOptions options,
      PathLength maxServerPathLength)
    {
      this.PrepareStoredProcedure("prc_Get", 3600);
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindServiceDataspaceId("@rootItemDataspaceId");
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindLocalItem("@localItem", localItem, true);
      this.PrepareAndBindVersionSpec("@versionSpec", version, false);
      this.BindByte("@depth", (byte) recursive);
      this.BindBoolean("@force", (options & GetOptions.GetAll) == GetOptions.GetAll);
      this.BindBoolean("@noGet", (options & GetOptions.Preview) == GetOptions.Preview);
      this.BindBoolean("@remap", (options & GetOptions.Remap) == GetOptions.Remap);
      this.BindInt("@maxRowsEvaluate", this.VersionControlRequestContext.VersionControlService.GetMaxRowsEvaluated(this.VersionControlRequestContext));
      this.BindPathWithGuidLength("@maxServerPathLength", maxServerPathLength);
      try
      {
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
        resultCollection.AddBinder<GetOperation>((ObjectBinder<GetOperation>) this.CreateGetOperationColumns());
        return resultCollection;
      }
      catch (DateVersionSpecBeforeBeginningOfRepositoryException ex)
      {
        this.RequestContext.TraceException(700205, TraceLevel.Info, Microsoft.TeamFoundation.VersionControl.Server.TraceArea.Get, TraceLayer.Component, (Exception) ex);
        throw new DateVersionSpecBeforeBeginningOfRepositoryException(((DateVersionSpec) version).OriginalText, (Exception) ex);
      }
    }

    public virtual ResultCollection LockItems(
      Workspace workspace,
      LockRequest[] lockRequests,
      bool silent,
      bool ignoreUnlockingNotLockedFiles)
    {
      this.PrepareStoredProcedure("prc_LockItem");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindLockRequestTable("@targetServerItemsIn", (IEnumerable<LockRequest>) lockRequests);
      this.BindBoolean("@silent", silent);
      this.BindBoolean("@ignoreUnlockingNotLockedFiles", ignoreUnlockingNotLockedFiles);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      if (!silent)
        resultCollection.AddBinder<GetOperation>((ObjectBinder<GetOperation>) this.CreateGetOperationColumns());
      return resultCollection;
    }

    public virtual ItemPathPair MapLocalToServerItemWithoutMappingRenames(
      WorkspaceInternal w,
      string localItem,
      bool honorCloaks,
      PathLength maxServerPathLength)
    {
      this.PrepareStoredProcedure("prc_MapLocalToServerItem");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", w.OwnerId);
      this.BindString("@workspaceName", w.Name, 64, false, SqlDbType.NVarChar);
      this.BindLocalItem("@localItem", localItem, false);
      this.BindBoolean("@honorCloaks", honorCloaks);
      this.BindPathWithGuidLength("@maxServerPathLength", maxServerPathLength);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<ItemPathPair>((ObjectBinder<ItemPathPair>) new ServerItemBinder15((VersionControlSqlResourceComponent) this));
      List<ItemPathPair> items = resultCollection.GetCurrent<ItemPathPair>().Items;
      return items.Count != 0 ? items[0] : throw new UnexpectedDatabaseResultException(this.ProcedureName);
    }

    public virtual ResultCollection PendAdd(
      Workspace workspace,
      List<ExpandedChange> expandedChanges,
      ChangeRequest[] changeRequests,
      bool silent,
      PathLength maxServerPathLength)
    {
      this.PrepareStoredProcedure("prc_PendAdd", 3600);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindBoolean("@silent", silent);
      this.BindPendAddTable("@itemList", (IEnumerable<ExpandedChange>) expandedChanges, changeRequests);
      this.BindPathWithGuidLength("@maxServerPathLength", maxServerPathLength);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      if (!silent)
      {
        resultCollection.AddBinder<GetOperation>((ObjectBinder<GetOperation>) this.CreateGetOperationColumns());
        resultCollection.AddBinder<Warning>((ObjectBinder<Warning>) new PendWarningColumns15((VersionControlSqlResourceComponent) this));
      }
      resultCollection.AddBinder<Failure>((ObjectBinder<Failure>) this.CreateBasicFailureColumns(this.ProcedureName));
      return resultCollection;
    }

    public virtual ResultCollection PendDelete(
      Workspace workspace,
      ItemPathPair targetItemPathPair,
      LockLevel lockLevel,
      bool silent)
    {
      this.PrepareStoredProcedure("prc_PendDelete", 3600);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindDataspaceIdAndServerItemPathPair("@targetServerItemDataspaceId", "@targetServerItem", targetItemPathPair, false);
      this.BindLockLevel("@lockStatus", lockLevel);
      this.BindBoolean("@silent", silent);
      this.BindInt("@maxRowsEvaluate", this.VersionControlRequestContext.VersionControlService.GetMaxRowsEvaluated(this.VersionControlRequestContext));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      if (!silent)
      {
        resultCollection.AddBinder<GetOperation>((ObjectBinder<GetOperation>) this.CreateGetOperationColumns());
        resultCollection.AddBinder<Warning>((ObjectBinder<Warning>) new PendWarningColumns15((VersionControlSqlResourceComponent) this));
      }
      return resultCollection;
    }

    public virtual ResultCollection PendEdit(
      Workspace workspace,
      ItemPathPair targetItemPathPair,
      int Encoding,
      LockLevel lockLevel,
      bool silent,
      bool getLatest)
    {
      this.PrepareStoredProcedure("prc_PendEdit");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindDataspaceIdAndServerItemPathPair("@targetServerItemDataspaceId", "@targetServerItem", targetItemPathPair, false);
      this.BindInt("@encoding", Encoding);
      this.BindLockLevel("@lockStatus", lockLevel);
      this.BindBoolean("@silent", silent);
      this.BindBoolean("@getLatest", getLatest);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      if (getLatest || !silent)
      {
        resultCollection.AddBinder<GetOperation>((ObjectBinder<GetOperation>) this.CreateGetOperationColumns());
        resultCollection.AddBinder<Warning>((ObjectBinder<Warning>) new PendWarningColumns15((VersionControlSqlResourceComponent) this));
      }
      return resultCollection;
    }

    public virtual ResultCollection PendMerge(
      Workspace workspace,
      ItemPathPair sourcePathPair,
      int sourceDeletionId,
      RecursionType recursion,
      ItemPathPair targetPathPair,
      VersionSpec versionSpecFrom,
      VersionSpec versionSpecTo,
      MergeOptionsEx options,
      bool branch,
      int transactionId,
      PathLength maxServerPathLength,
      bool useRecompileVersion)
    {
      if (this.Version >= 31 & useRecompileVersion)
        this.PrepareStoredProcedure("prc_PendMergeWithRecompile", 3600);
      else
        this.PrepareStoredProcedure("prc_PendMerge", 3600);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindDataspaceIdAndServerItemPathPair("@sourceServerItemDataspaceId", "@sourceServerItem", sourcePathPair, false);
      this.BindDataspaceIdAndServerItemPathPair("@targetServerItemDataspaceId", "@targetServerItem", targetPathPair, false);
      this.PrepareAndBindVersionSpec("@versionSpecFrom", versionSpecFrom, false);
      this.PrepareAndBindVersionSpec("@versionSpecTo", versionSpecTo, false);
      this.BindByte("@candidatesOnly", (byte) 0);
      this.BindBoolean("@forceBaseless", (options & MergeOptionsEx.Baseless) != 0);
      this.BindBoolean("@forceMerge", (options & MergeOptionsEx.ForceMerge) != 0);
      this.BindBoolean("@discard", (options & MergeOptionsEx.AlwaysAcceptMine) != 0);
      this.BindInt("@depth", (int) recursion);
      this.BindBoolean("@branch", branch);
      this.BindBoolean("@noMerge", (options & MergeOptionsEx.NoMerge) != 0);
      this.BindBoolean("@silent", (options & MergeOptionsEx.Silent) != 0);
      this.BindBoolean("@conservative", (options & MergeOptionsEx.Conservative) != 0);
      this.BindInt("@transactionId", transactionId);
      this.BindInt("@deadlockRetries", this.MaxDeadlockRetries);
      this.BindInt("@deadlockWait", this.DeadlockPause);
      this.BindBoolean("@disableMultipleRenames", this.VersionControlRequestContext.VersionControlService.GetDisableMultipleRenames(this.VersionControlRequestContext));
      this.BindPathWithGuidLength("@maxServerPathLength", maxServerPathLength);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<PendChangeSecurity>((ObjectBinder<PendChangeSecurity>) new PendChangeSecurityColumns15((VersionControlSqlResourceComponent) this));
      resultCollection.AddBinder<Failure>((ObjectBinder<Failure>) new PendChangeFailureBinder15(PendChangeFailureBinder.Caller.Merge, this.RequestContext, (VersionControlSqlResourceComponent) this));
      if ((options & MergeOptionsEx.Silent) == MergeOptionsEx.None)
        resultCollection.AddBinder<GetOperation>((ObjectBinder<GetOperation>) this.CreateGetOperationColumns());
      resultCollection.AddBinder<Failure>((ObjectBinder<Failure>) new MergeWarningColumns15((VersionControlSqlResourceComponent) this));
      if ((options & MergeOptionsEx.Silent) == MergeOptionsEx.None)
        resultCollection.AddBinder<Conflict>((ObjectBinder<Conflict>) this.CreateConflictDetailsColumns());
      resultCollection.AddBinder<ChangePendedFlags>((ObjectBinder<ChangePendedFlags>) new ChangePendedFlagsBinder());
      return resultCollection;
    }

    public virtual ResultCollection QueryMergeCandidates(
      Workspace workspace,
      ItemPathPair sourceItemPathPair,
      ItemPathPair targetItemPathPair,
      RecursionType recursive,
      int sourceDeletionId,
      MergeOptionsEx options,
      PathLength maxServerPathLength,
      bool useRecompileVersion)
    {
      if (this.Version >= 31 & useRecompileVersion)
        this.PrepareStoredProcedure("prc_PendMergeWithRecompile", 3600);
      else
        this.PrepareStoredProcedure("prc_PendMerge", 3600);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      if (workspace != null)
      {
        this.BindGuid("@ownerId", workspace.OwnerId);
        this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      }
      else
      {
        this.BindGuid("@ownerId", Guid.Empty);
        this.BindString("@workspaceName", string.Empty, 64, true, SqlDbType.NVarChar);
      }
      this.BindDataspaceIdAndServerItemPathPair("@sourceServerItemDataspaceId", "@sourceServerItem", sourceItemPathPair, false);
      this.BindDataspaceIdAndServerItemPathPair("@targetServerItemDataspaceId", "@targetServerItem", targetItemPathPair, false);
      this.PrepareAndBindVersionSpec("@versionSpecFrom", (VersionSpec) null, true);
      this.PrepareAndBindVersionSpec("@versionSpecTo", (VersionSpec) null, true);
      this.BindByte("@candidatesOnly", (byte) 1);
      this.BindBoolean("@forceBaseless", (options & MergeOptionsEx.Baseless) != 0);
      this.BindBoolean("@forceMerge", false);
      this.BindBoolean("@discard", false);
      this.BindInt("@depth", (int) recursive);
      this.BindBoolean("@branch", false);
      this.BindBoolean("@noMerge", true);
      this.BindBoolean("@silent", false);
      this.BindBoolean("@conservative", false);
      this.BindPathWithGuidLength("@maxServerPathLength", maxServerPathLength);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<Changeset>((ObjectBinder<Changeset>) new ChangesetColumns());
      resultCollection.AddBinder<ItemMerge>((ObjectBinder<ItemMerge>) new QueryMergesColumns15((VersionControlSqlResourceComponent) this));
      resultCollection.AddBinder<ItemMerge>((ObjectBinder<ItemMerge>) new QueryMergesColumns15((VersionControlSqlResourceComponent) this));
      return resultCollection;
    }

    public virtual ResultCollection PendRename(
      Workspace workspace,
      ItemPathPair sourceServerItemPathPair,
      ItemPathPair targetServerItemPathPair,
      LockLevel lockLevel,
      bool silent,
      PathLength maxServerPathLength)
    {
      this.PrepareStoredProcedure("prc_PendRename");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindDataspaceIdAndServerItemPathPair("@sourceServerItemDataspaceId", "@sourceServerItem", sourceServerItemPathPair, false);
      this.BindDataspaceIdAndServerItemPathPair("@targetServerItemDataspaceId", "@targetServerItem", targetServerItemPathPair, false);
      this.BindLockLevel("@lockStatus", lockLevel);
      this.BindBoolean("@silent", silent);
      this.BindInt("@maxRowsEvaluate", this.VersionControlRequestContext.VersionControlService.GetMaxRowsEvaluated(this.VersionControlRequestContext));
      this.BindPathWithGuidLength("@maxServerPathLength", maxServerPathLength);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      if (!silent)
      {
        resultCollection.AddBinder<GetOperation>((ObjectBinder<GetOperation>) this.CreateGetOperationColumns());
        resultCollection.AddBinder<Warning>((ObjectBinder<Warning>) this.CreatePendWarningColumns());
      }
      resultCollection.AddBinder<ChangePendedFlags>((ObjectBinder<ChangePendedFlags>) new ChangePendedFlagsBinder());
      return resultCollection;
    }

    public virtual ResultCollection PendRollback(
      Workspace workspace,
      ItemSpec[] items,
      VersionSpec itemVersion,
      VersionSpec from,
      VersionSpec to,
      RollbackOptions options,
      int transactionId,
      PathLength maxServerPathLength)
    {
      this.PrepareStoredProcedure("prc_PendRollback", 3600);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      if (items.Length != 0)
      {
        this.BindItemSpecTable("@items", (IEnumerable<ItemSpec>) items);
        this.PrepareAndBindVersionSpec("@itemSpecVersion", itemVersion, false);
      }
      this.PrepareAndBindVersionSpec("@versionSpecFrom", from, false);
      this.PrepareAndBindVersionSpec("@versionSpecTo", to, false);
      this.BindInt("@rollbackOptions", (int) options);
      this.BindInt("@maxRowsEvaluate", this.VersionControlRequestContext.VersionControlService.GetMaxRowsEvaluated(this.VersionControlRequestContext));
      this.BindInt("@transactionId", transactionId);
      this.BindInt("@deadlockRetries", this.MaxDeadlockRetries);
      this.BindInt("@deadlockWait", this.DeadlockPause);
      this.BindPathWithGuidLength("@maxServerPathLength", maxServerPathLength);
      SqlDataReader reader;
      try
      {
        reader = this.ExecuteReader();
      }
      catch (DateVersionSpecBeforeBeginningOfRepositoryException ex)
      {
        if (to is DateVersionSpec)
          to.ToChangeset(this.RequestContext);
        if (from is DateVersionSpec)
          from.ToChangeset(this.RequestContext);
        if (itemVersion is DateVersionSpec)
          itemVersion.ToChangeset(this.RequestContext);
        throw;
      }
      ResultCollection resultCollection = new ResultCollection((IDataReader) reader, this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<PendChangeSecurity>((ObjectBinder<PendChangeSecurity>) new PendChangeSecurityColumns15((VersionControlSqlResourceComponent) this));
      resultCollection.AddBinder<Failure>((ObjectBinder<Failure>) new RollbackFailureBinder15(this.RequestContext, (VersionControlSqlResourceComponent) this));
      if ((options & RollbackOptions.Silent) != RollbackOptions.Silent)
      {
        resultCollection.AddBinder<GetOperation>((ObjectBinder<GetOperation>) this.CreateGetOperationColumns());
        resultCollection.AddBinder<Conflict>((ObjectBinder<Conflict>) this.CreateConflictDetailsColumns());
      }
      resultCollection.AddBinder<Failure>((ObjectBinder<Failure>) new RollbackFailureBinder15(this.RequestContext, (VersionControlSqlResourceComponent) this));
      resultCollection.AddBinder<ChangePendedFlags>((ObjectBinder<ChangePendedFlags>) new ChangePendedFlagsBinder());
      return resultCollection;
    }

    public virtual ResultCollection PendUndelete(
      Workspace workspace,
      ItemPathPair sourceItemPathPair,
      ItemPathPair targetItemPathPair,
      LockLevel lockLevel,
      bool silent,
      PathLength maxServerPathLength)
    {
      this.PrepareStoredProcedure("prc_PendUndelete");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindDataspaceIdAndServerItemPathPair("@sourceServerItemDataspaceId", "@sourceServerItem", sourceItemPathPair, false);
      this.BindServerItemPathPair("@targetServerItem", targetItemPathPair, true);
      this.BindLockLevel("@lockStatus", lockLevel);
      this.BindBoolean("@silent", silent);
      this.BindPathWithGuidLength("@maxServerPathLength", maxServerPathLength);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      if (!silent)
      {
        resultCollection.AddBinder<GetOperation>((ObjectBinder<GetOperation>) this.CreateGetOperationColumns());
        resultCollection.AddBinder<Warning>((ObjectBinder<Warning>) this.CreatePendWarningColumns());
      }
      return resultCollection;
    }

    public virtual ResultCollection PendUnshelve(
      string shelveSetName,
      Guid shelvesetOwnerId,
      int shelvesetVersion,
      string workspaceName,
      Guid workspaceOwnerId,
      bool merge,
      IEnumerable<ItemPathPair> itemsToUnshelve,
      int transactionId,
      PathLength maxServerPathLength)
    {
      this.PrepareStoredProcedure("prc_PendUnshelve");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindNullableGuid("@workspaceOwnerId", workspaceOwnerId);
      this.BindString("@workspaceName", workspaceName, 64, true, SqlDbType.NVarChar);
      this.BindNullableGuid("@shelveSetOwnerId", shelvesetOwnerId);
      this.BindString("@shelveSetName", shelveSetName, 64, true, SqlDbType.NVarChar);
      this.BindInt("@shelvesetVersion", shelvesetVersion);
      this.BindBoolean("@merge", merge);
      this.BindInt("@transactionId", transactionId);
      this.BindInt("@deadlockRetries", this.MaxDeadlockRetries);
      this.BindInt("@deadlockWait", this.DeadlockPause);
      this.BindServerItemPairTypeTable("@itemList", itemsToUnshelve);
      this.BindPathWithGuidLength("@maxServerPathLength", maxServerPathLength);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<Failure>((ObjectBinder<Failure>) this.CreateBasicFailureColumns(this.ProcedureName));
      resultCollection.AddBinder<PendChangeSecurity>((ObjectBinder<PendChangeSecurity>) new PendChangeSecurityColumns15((VersionControlSqlResourceComponent) this));
      resultCollection.AddBinder<Shelveset>((ObjectBinder<Shelveset>) new QueryShelvesetsColumns2());
      resultCollection.AddBinder<CheckinNoteFieldValue>((ObjectBinder<CheckinNoteFieldValue>) new CheckinNoteColumns());
      resultCollection.AddBinder<VersionControlLink>((ObjectBinder<VersionControlLink>) new LinkColumns());
      resultCollection.AddBinder<Failure>((ObjectBinder<Failure>) new PendChangeFailureBinder15(PendChangeFailureBinder.Caller.Unshelve, this.RequestContext, (VersionControlSqlResourceComponent) this));
      resultCollection.AddBinder<Failure>((ObjectBinder<Failure>) new PendChangeFailureBinder15(PendChangeFailureBinder.Caller.Unshelve, this.RequestContext, SeverityType.Warning, (VersionControlSqlResourceComponent) this));
      resultCollection.AddBinder<Warning>((ObjectBinder<Warning>) this.CreatePendWarningColumns());
      resultCollection.AddBinder<AffectedItems>((ObjectBinder<AffectedItems>) new AffectedItemsColumns());
      resultCollection.AddBinder<GetOperation>((ObjectBinder<GetOperation>) this.CreateGetOperationColumns());
      if (merge)
        resultCollection.AddBinder<Conflict>((ObjectBinder<Conflict>) this.CreateConflictDetailsColumns());
      resultCollection.AddBinder<ChangePendedFlags>((ObjectBinder<ChangePendedFlags>) new ChangePendedFlagsBinder());
      return resultCollection;
    }

    public virtual ResultCollection QueryBranches(
      Workspace workspace,
      ItemPathPair itemPathPair,
      int changeset)
    {
      this.PrepareStoredProcedure("prc_QueryBranches");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindNullableGuid("@ownerId", workspace != null ? workspace.OwnerId : Guid.Empty);
      this.BindString("@workspaceName", workspace?.Name, 64, true, SqlDbType.NVarChar);
      this.BindServerItem("@targetRootServerItem", this.BestEffortConvertPathPairToPathWithProjectGuid(itemPathPair), false);
      this.BindInt("@version", changeset);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<BranchRelative>((ObjectBinder<BranchRelative>) new QueryBranchesColumns15((VersionControlSqlResourceComponent) this));
      return resultCollection;
    }

    public virtual ResultCollection QueryBranchObjectOwnership(
      int[] changesets,
      ItemSpec pathFilter)
    {
      this.PrepareStoredProcedure("prc_QueryBranchObjectOwnership");
      this.BindUniqueInt32Table("@changesets", (IEnumerable<int>) changesets);
      if (pathFilter != null)
      {
        this.BindServerItem("@filterItem", this.BestEffortConvertPathPairToPathWithProjectGuid(pathFilter.ItemPathPair), false);
        this.BindInt("@filterDepth", (int) pathFilter.RecursionType);
      }
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<BranchObjectOwnership>((ObjectBinder<BranchObjectOwnership>) new BranchObjectOwnershipBinder15((VersionControlSqlResourceComponent) this));
      return resultCollection;
    }

    public virtual ResultCollection QueryBranchObjects(ItemIdentifier item, RecursionType recursion)
    {
      this.PrepareStoredProcedure("prc_QueryBranchObjects");
      if (item != null)
        this.BindServerItem("@rootServerItem", this.BestEffortConvertPathPairToPathWithProjectGuid(item.ItemPathPair), false);
      this.BindInt("@depth", (int) recursion);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<BranchObject>((ObjectBinder<BranchObject>) new BranchObjectBinder15((VersionControlSqlResourceComponent) this));
      return resultCollection;
    }

    public virtual ResultCollection QueryBranchObjectsByPath(
      ItemPathPair rootItemPathPair,
      VersionSpec version)
    {
      this.PrepareStoredProcedure("prc_QueryBranchObjectsByPath");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindServerItem("@rootServerItem", this.BestEffortConvertPathPairToPathWithProjectGuid(rootItemPathPair), false);
      this.PrepareAndBindVersionSpec("@versionSpec", version, false);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<BranchObject>((ObjectBinder<BranchObject>) new BranchObjectBinder15((VersionControlSqlResourceComponent) this));
      return resultCollection;
    }

    public virtual ResultCollection QueryConflicts(Workspace workspace)
    {
      this.PrepareStoredProcedure("prc_QueryConflicts", 3600);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<Conflict>((ObjectBinder<Conflict>) this.CreateConflictDetailsColumns());
      return resultCollection;
    }

    public virtual ResultCollection QueryChangesForChangeset(int changeset, ItemSpec lastItem)
    {
      this.PrepareStoredProcedure("prc_QueryChangesForChangeset");
      this.BindInt("@changeSetId", changeset);
      if (lastItem != null)
        this.BindServerItem("@lastServerItem", this.BestEffortConvertPathPairToPathWithProjectGuid(lastItem.ItemPathPair), false);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<DateTime>((ObjectBinder<DateTime>) new CreationDateColumns());
      resultCollection.AddBinder<Change>((ObjectBinder<Change>) this.CreateChangeColumns());
      return resultCollection;
    }

    public virtual ResultCollection QueryDestroyableItems(
      string serverFolderPattern,
      int version,
      DeletedState deleted)
    {
      this.PrepareStoredProcedure("prc_QueryDestroyableItems", 3600);
      this.BindServerItem("@targetServerFolder", this.BestEffortConvertToPathWithProjectGuid(serverFolderPattern), false);
      this.BindInt("@version", version);
      this.BindByte("@deleted", (byte) deleted, (byte) 2);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryDestroyableItems", this.RequestContext);
      resultCollection.AddBinder<Item>((ObjectBinder<Item>) new QueryItemsColumns15a((VersionControlSqlResourceComponent) this));
      return resultCollection;
    }

    public virtual ResultCollection QueryHistory(
      ItemPathPair targetItemPathPair,
      VersionSpec versionItem,
      VersionSpec versionFrom,
      VersionSpec versionTo,
      Guid teamFoundationId,
      RecursionType recursive,
      bool includeFiles,
      bool slotMode,
      int maxChangesets,
      bool sortAscending,
      int maxChangesPerChangeset)
    {
      this.PrepareStoredProcedure("prc_QueryHistory", 3600);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindServerItem("@targetServerItem", this.BestEffortConvertPathPairToPathWithProjectGuid(targetItemPathPair), false);
      this.PrepareAndBindVersionSpec("@versionSpecItem", versionItem, false);
      this.PrepareAndBindVersionSpec("@versionSpecFrom", versionFrom, false);
      this.PrepareAndBindVersionSpec("@versionSpecTo", versionTo, false);
      this.BindNullableGuid("@teamFoundationId", teamFoundationId);
      this.BindInt("@depth", (int) recursive);
      this.BindBoolean("@includeFiles", includeFiles);
      this.BindBoolean("@slotMode", slotMode);
      this.BindInt("@maxChangeSets", maxChangesets);
      this.BindBoolean("@sortAscending", sortAscending);
      this.BindInt("@maxChangesPerChangeSet", maxChangesPerChangeset);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<DeterminedItem>((ObjectBinder<DeterminedItem>) this.CreateDetermineItemTypeColumns());
      resultCollection.AddBinder<Changeset>((ObjectBinder<Changeset>) new ChangesetColumns());
      resultCollection.AddBinder<PolicyFailureInfo>((ObjectBinder<PolicyFailureInfo>) new PolicyFailureColumns());
      resultCollection.AddBinder<CheckinNoteFieldValue>((ObjectBinder<CheckinNoteFieldValue>) new CheckinNoteColumns());
      if (includeFiles)
        resultCollection.AddBinder<Change>((ObjectBinder<Change>) this.CreateChangeColumns());
      return resultCollection;
    }

    public virtual ResultCollection QueryItemHistory(
      ItemPathPair targetItemPathPair,
      VersionSpec versionItem,
      VersionSpec versionFrom,
      VersionSpec versionTo,
      Guid teamFoundationId,
      bool includeFiles,
      int maxChangesets,
      bool sortAscending)
    {
      this.PrepareStoredProcedure("prc_QueryItemHistory", 3600);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindServerItem("@targetServerItem", this.BestEffortConvertPathPairToPathWithProjectGuid(targetItemPathPair), false);
      this.PrepareAndBindVersionSpec("@versionSpecItem", versionItem, false);
      this.PrepareAndBindVersionSpec("@versionSpecFrom", versionFrom, false);
      this.PrepareAndBindVersionSpec("@versionSpecTo", versionTo, false);
      this.BindNullableGuid("@teamFoundationId", teamFoundationId);
      this.BindBoolean("@includeFiles", includeFiles);
      this.BindInt("@maxChangeSets", maxChangesets);
      this.BindBoolean("@sortAscending", sortAscending);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.VersionControlRequestContext.VersionControlService.GetMaxItemsPerRequest(this.VersionControlRequestContext), this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<Changeset>((ObjectBinder<Changeset>) new ChangesetColumns());
      resultCollection.AddBinder<PolicyFailureInfo>((ObjectBinder<PolicyFailureInfo>) new PolicyFailureColumns());
      resultCollection.AddBinder<CheckinNoteFieldValue>((ObjectBinder<CheckinNoteFieldValue>) new CheckinNoteColumns());
      if (includeFiles)
        resultCollection.AddBinder<Change>((ObjectBinder<Change>) this.CreateChangeColumns());
      return resultCollection;
    }

    public virtual ResultCollection QueryItems(
      ItemPathPair itemPathPair,
      int changesetId,
      RecursionType recursive,
      DeletedState deleted,
      ItemType itemType,
      int options)
    {
      this.PrepareStoredProcedure("prc_QueryItems", 3600);
      this.BindServerItem("@targetServerItem", this.BestEffortConvertPathPairToPathWithProjectGuid(itemPathPair), true);
      this.BindInt("@version", changesetId);
      this.BindByte("@depth", (byte) recursive);
      this.BindByte("@deleted", (byte) deleted, (byte) 2);
      this.BindByte("@itemType", (byte) itemType, (byte) 0);
      this.BindInt("@options", options);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<DeterminedItem>((ObjectBinder<DeterminedItem>) this.CreateDetermineItemTypeColumns());
      resultCollection.AddBinder<Item>((ObjectBinder<Item>) this.CreateQueryItemsColumns());
      return resultCollection;
    }

    public virtual ResultCollection QueryItemsExtended(
      Workspace workspace,
      ItemPathPair itemPathPair,
      RecursionType recursive,
      DeletedState deleted,
      ItemType itemType,
      int options,
      int timeoutMinutes)
    {
      this.PrepareStoredProcedure("prc_QueryItemsExtended", timeoutMinutes * 60);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      if (workspace != null)
      {
        this.BindGuid("@ownerId", workspace.OwnerId);
        this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      }
      else
      {
        this.BindNullValue("@ownerId", SqlDbType.UniqueIdentifier);
        this.BindNullValue("@workspaceName", SqlDbType.NVarChar);
      }
      this.BindServerItem("@serverItem", this.BestEffortConvertPathPairToPathWithProjectGuid(itemPathPair), true);
      this.BindByte("@depth", (byte) recursive);
      this.BindByte("@itemType", (byte) itemType, (byte) 0);
      this.BindByte("@deleted", (byte) deleted, (byte) 2);
      this.BindInt("@options", options);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<ExtendedItem>((ObjectBinder<ExtendedItem>) this.CreateExtendedItemColumns());
      return resultCollection;
    }

    public virtual ResultCollection QueryItemsExtendedLocal(
      Workspace workspace,
      string localItem,
      RecursionType recursive,
      DeletedState deleted,
      ItemType itemType,
      int options,
      PathLength maxServerPathLength,
      int timeoutMinutes)
    {
      this.PrepareStoredProcedure("prc_QueryItemsExtendedLocal", timeoutMinutes * 60);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      if (workspace != null)
      {
        this.BindGuid("@ownerId", workspace.OwnerId);
        this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      }
      else
      {
        this.BindNullValue("@ownerId", SqlDbType.UniqueIdentifier);
        this.BindNullValue("@workspaceName", SqlDbType.NVarChar);
      }
      this.BindLocalItem("@localItem", localItem, true);
      this.BindByte("@depth", (byte) recursive);
      this.BindByte("@deleted", (byte) deleted, (byte) 2);
      this.BindByte("@itemType", (byte) itemType, (byte) 0);
      this.BindInt("@options", options);
      this.BindPathWithGuidLength("@maxServerPathLength", maxServerPathLength);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<ExtendedItem>((ObjectBinder<ExtendedItem>) this.CreateExtendedItemColumns());
      return resultCollection;
    }

    public virtual ResultCollection QueryItemsLocal(
      Workspace localWorkspace,
      string localItem,
      int changesetId,
      RecursionType recursive,
      DeletedState deleted,
      ItemType itemType,
      int options)
    {
      this.PrepareStoredProcedure("prc_QueryItemsLocal", 3600);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", localWorkspace.OwnerId);
      this.BindString("@workspaceName", localWorkspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindLocalItem("@localItem", localItem, true);
      this.BindInt("@version", changesetId);
      this.BindByte("@depth", (byte) recursive);
      this.BindByte("@deleted", (byte) deleted, (byte) 2);
      this.BindByte("@itemType", (byte) itemType, (byte) 0);
      this.BindInt("@options", options);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<DeterminedItem>((ObjectBinder<DeterminedItem>) this.CreateDetermineItemTypeColumns());
      resultCollection.AddBinder<Item>((ObjectBinder<Item>) this.CreateQueryItemsColumns());
      return resultCollection;
    }

    public virtual ResultCollection QueryLabelItems(
      string labelName,
      ItemPathPair labelScopePair,
      ItemPathPair itemPathPair,
      RecursionType recursive,
      ItemType itemType,
      DeletedState deletedState,
      int options)
    {
      this.PrepareStoredProcedure("prc_QueryLabelItems", 3600);
      this.BindString("@labelName", labelName, 64, false, SqlDbType.NVarChar);
      this.BindServerItem("@labelScope", this.BestEffortConvertPathPairToPathWithProjectGuid(labelScopePair), true);
      this.BindServerItem("@targetServerItem", this.BestEffortConvertPathPairToPathWithProjectGuid(itemPathPair), true);
      this.BindByte("@depth", (byte) recursive);
      this.BindByte("@itemType", (byte) itemType, (byte) 0);
      if (DeletedState.Any == deletedState)
        this.BindNullValue("@deleted", SqlDbType.Bit);
      else
        this.BindBoolean("@deleted", DeletedState.Deleted == deletedState);
      this.BindInt("@options", options);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<DeterminedItem>((ObjectBinder<DeterminedItem>) this.CreateDetermineItemTypeColumns());
      resultCollection.AddBinder<Item>((ObjectBinder<Item>) this.CreateQueryItemsColumns());
      return resultCollection;
    }

    public virtual ResultCollection QueryLabelItemsLocal(
      Workspace w,
      string labelName,
      string labelScope,
      string localItem,
      RecursionType recursive,
      ItemType itemType,
      DeletedState deletedState,
      int options,
      PathLength maxServerPathLength)
    {
      this.PrepareStoredProcedure("prc_QueryLabelItemsLocal", 3600);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@workspaceOwner", w.OwnerId);
      this.BindString("@workspaceName", w.Name, 64, false, SqlDbType.NVarChar);
      this.BindString("@labelName", labelName, 64, false, SqlDbType.NVarChar);
      this.BindServerItem("@labelScope", this.BestEffortConvertToPathWithProjectGuid(labelScope), true);
      this.BindLocalItem("@targetLocalItem", localItem, true);
      this.BindByte("@depth", (byte) recursive);
      this.BindByte("@itemType", (byte) itemType, (byte) 0);
      this.BindPathWithGuidLength("@maxServerPathLength", maxServerPathLength);
      if (DeletedState.Any == deletedState)
        this.BindNullValue("@deleted", SqlDbType.Bit);
      else
        this.BindBoolean("@deleted", DeletedState.Deleted == deletedState);
      this.BindInt("@options", options);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<DeterminedItem>((ObjectBinder<DeterminedItem>) new DetermineItemTypeColumns());
      resultCollection.AddBinder<Item>((ObjectBinder<Item>) this.CreateQueryItemsColumns());
      return resultCollection;
    }

    public virtual ResultCollection QueryLocalVersions(
      Workspace workspace,
      ItemSpec itemSpec,
      PathLength maxServerPathLength)
    {
      this.PrepareStoredProcedure("prc_QueryLocalVersions");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@teamFoundationId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindLocalItem("@localItem", DBPath.LocalToDatabasePath(itemSpec.Item), false);
      this.BindByte("@depth", (byte) itemSpec.RecursionType);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.VersionControlRequestContext.VersionControlService.GetMaxItemsPerRequest(this.VersionControlRequestContext), this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<LocalVersion>((ObjectBinder<LocalVersion>) new LocalVersionBinder());
      return resultCollection;
    }

    public virtual ResultCollection QueryMappedItems(
      Workspace workspace,
      ItemPathPair itemPathPair,
      RecursionType recursive,
      int options)
    {
      this.PrepareStoredProcedure("prc_QueryMappedItems");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindServerItem("@serverItem", this.BestEffortConvertPathPairToPathWithProjectGuid(itemPathPair), false);
      this.BindInt("@depth", (int) (byte) recursive);
      this.BindInt("@options", options);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<DeterminedItem>((ObjectBinder<DeterminedItem>) this.CreateDetermineItemTypeColumns());
      resultCollection.AddBinder<MappedItem>((ObjectBinder<MappedItem>) new QueryMappedItemsColumns15((VersionControlSqlResourceComponent) this));
      return resultCollection;
    }

    public virtual ResultCollection QueryMergesForChangeset(int changeset, List<Change> changes)
    {
      this.PrepareStoredProcedure("prc_QueryMergesForChangeset");
      this.BindInt("@changeSetId", changeset);
      List<Tuple<int, int>> rows = new List<Tuple<int, int>>(changes.Count);
      foreach (Change change in changes)
      {
        int dataspaceIdFromPathPair = this.GetDataspaceIdFromPathPair(change.Item.ItemPathPair);
        rows.Add(new Tuple<int, int>(dataspaceIdFromPathPair, change.Item.ItemId));
      }
      this.BindItemIdWithIndexTable("@itemList", (IEnumerable<Tuple<int, int>>) rows);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<MergeSource>((ObjectBinder<MergeSource>) this.CreateMergeSourceColumns());
      return resultCollection;
    }

    public virtual ResultCollection QueryMergeRelationships(string serverItem)
    {
      this.PrepareStoredProcedure("prc_QueryMergeRelationships");
      this.BindServerItem("@serverItem", this.BestEffortConvertToPathWithProjectGuid(serverItem), false);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.VersionControlRequestContext.VersionControlService.GetMaxItemsPerRequest(this.VersionControlRequestContext), this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<ItemIdentifier>((ObjectBinder<ItemIdentifier>) new ItemIdentifierBinder15((VersionControlSqlResourceComponent) this));
      return resultCollection;
    }

    public virtual ResultCollection QueryMerges(
      ItemPathPair sourceItemPathPair,
      VersionSpec versionSpecSource,
      int sourceDeletionId,
      ItemPathPair targetItemPathPair,
      VersionSpec versionSpecTarget,
      int targetDeletionId,
      RecursionType recursive,
      int versionFrom,
      int versionTo,
      bool showAll)
    {
      this.PrepareStoredProcedure("prc_QueryMerges", 3600);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindServerItem("@sourceServerItem", this.BestEffortConvertPathPairToPathWithProjectGuid(sourceItemPathPair), true);
      this.PrepareAndBindVersionSpec("@versionSpecSource", versionSpecSource, false);
      this.BindServerItem("@targetServerItem", this.BestEffortConvertPathPairToPathWithProjectGuid(targetItemPathPair), true);
      this.PrepareAndBindVersionSpec("@versionSpecTarget", versionSpecTarget, false);
      this.BindByte("@depth", (byte) recursive);
      this.BindInt("@versionFrom", versionFrom);
      this.BindInt("@versionTo", versionTo);
      this.BindInt("@sourceDeletionId", sourceDeletionId);
      this.BindInt("@targetDeletionId", targetDeletionId);
      this.BindInt("@maxRowsEvaluate", this.VersionControlRequestContext.VersionControlService.GetMaxRowsEvaluated(this.VersionControlRequestContext));
      this.BindBoolean("@showAll", showAll);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<Changeset>((ObjectBinder<Changeset>) new ChangesetColumns());
      resultCollection.AddBinder<ItemMerge>((ObjectBinder<ItemMerge>) new QueryMergesColumns15((VersionControlSqlResourceComponent) this));
      resultCollection.AddBinder<ItemMerge>((ObjectBinder<ItemMerge>) new QueryMergesColumns15((VersionControlSqlResourceComponent) this));
      return resultCollection;
    }

    public virtual ResultCollection QueryMergesExtended(
      ItemPathPair targetItemPathPair,
      int changesetTarget,
      int deletionId,
      int versionFrom,
      int versionTo,
      QueryMergesExtendedOptions options)
    {
      this.PrepareStoredProcedure("prc_QueryMergesExtended", 3600);
      this.BindServerItem("@targetServerItem", this.BestEffortConvertPathPairToPathWithProjectGuid(targetItemPathPair), true);
      this.BindInt("@versionTarget", changesetTarget);
      this.BindInt("@targetDeletionId", deletionId);
      this.BindInt("@versionFrom", versionFrom);
      this.BindInt("@versionTo", versionTo);
      this.BindInt("@options", (int) options);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<ExtendedMerge>((ObjectBinder<ExtendedMerge>) new ExtendedMergeBinder15((VersionControlSqlResourceComponent) this));
      return resultCollection;
    }

    public virtual ResultCollection QueryPendingChanges(
      Workspace localWorkspace,
      string filterWorkspaceName,
      Guid filterWorkspaceOwner,
      int filterWorkspaceVersion,
      PendingSetType filterPendingSetType,
      ItemSpec[] items,
      string lastChange,
      bool maskLocalWorkspaces,
      PathLength maxServerPathLength)
    {
      this.PrepareStoredProcedure("prc_QueryPendingChanges", 3600);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindNullableGuid("@localUserId", localWorkspace != null ? localWorkspace.OwnerId : Guid.Empty);
      this.BindString("@localWorkspaceName", localWorkspace?.Name, 64, true, SqlDbType.NVarChar);
      this.BindNullableGuid("@filterUserId", filterWorkspaceOwner);
      this.BindString("@filterWorkspaceName", filterWorkspaceName, 64, true, SqlDbType.NVarChar);
      this.BindByte("@filterWorkspaceType", (byte) filterPendingSetType);
      this.BindInt("@filterWorkspaceVersion", filterWorkspaceVersion);
      if (lastChange != null)
        this.BindServerItem("@lastServerItem", this.BestEffortConvertToPathWithProjectGuid(lastChange), false);
      this.BindItemSpecTable("@itemList", (IEnumerable<ItemSpec>) items);
      this.BindBoolean("@maskLocalWorkspaces", maskLocalWorkspaces);
      this.BindPathWithGuidLength("@maxServerPathLength", maxServerPathLength);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      UseMappingsOverridesColumns binder1 = new UseMappingsOverridesColumns();
      resultCollection.AddBinder<int>((ObjectBinder<int>) binder1);
      binder1.MoveNext();
      resultCollection.NextResult();
      if (binder1.Current > 0)
      {
        ProjectNameMappingsColumns binder2 = new ProjectNameMappingsColumns((VersionControlSqlResourceComponent) this);
        resultCollection.AddBinder<KeyValuePair<Guid, string>>((ObjectBinder<KeyValuePair<Guid, string>>) binder2);
        while (binder2.MoveNext())
        {
          IVssRequestContext requestContext = this.RequestContext;
          KeyValuePair<Guid, string> current = binder2.Current;
          Guid key = current.Key;
          current = binder2.Current;
          string projectName = current.Value;
          ProjectUtility.AddProjectMappingsToRequestContext(requestContext, key, projectName);
        }
        resultCollection.NextResult();
      }
      resultCollection.AddBinder<PendingChange>((ObjectBinder<PendingChange>) this.CreateQueryPendingChangesColumns(this.RequestContext));
      resultCollection.AddBinder<Failure>((ObjectBinder<Failure>) this.CreateQueryPendingChangesFailureColumns());
      return resultCollection;
    }

    public virtual ResultCollection QueryPendingChangesForCheckin(
      Guid workspaceOwner,
      string workspaceName,
      int workspaceVersion,
      PendingSetType workspaceType)
    {
      this.PrepareStoredProcedure("prc_QueryPendingChangesForCheckin", 3600);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspaceOwner);
      this.BindString("@workspaceName", workspaceName, 64, false, SqlDbType.NVarChar);
      this.BindInt("@workspaceVersion", workspaceVersion);
      this.BindByte("@workspaceType", (byte) workspaceType);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<PendingChangeLight>((ObjectBinder<PendingChangeLight>) this.CreateQueryPendingChangesForCheckinColumns());
      resultCollection.AddBinder<PendingChangeLight>((ObjectBinder<PendingChangeLight>) this.CreateQueryPendingChangesForCheckinColumns());
      return resultCollection;
    }

    public virtual ResultCollection QueryWorkspaceItems(
      Workspace w,
      ItemPathPair itemPathPair,
      RecursionType recursive,
      ItemType itemType,
      bool deleted,
      int options)
    {
      this.PrepareStoredProcedure("prc_QueryWorkspaceItems", 3600);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", w.OwnerId);
      this.BindString("@workspaceName", w.Name, 64, false, SqlDbType.NVarChar);
      this.BindServerItem("@targetServerItem", this.BestEffortConvertPathPairToPathWithProjectGuid(itemPathPair), true);
      this.BindByte("@depth", (byte) recursive);
      this.BindByte("@itemType", (byte) itemType, (byte) 0);
      this.BindBoolean("@deleted", deleted);
      this.BindInt("@options", options);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<DeterminedItem>((ObjectBinder<DeterminedItem>) this.CreateDetermineItemTypeColumns());
      resultCollection.AddBinder<WorkspaceItem>((ObjectBinder<WorkspaceItem>) this.CreateQueryWorkspaceItemsColumns());
      return resultCollection;
    }

    public virtual ResultCollection QueryWorkspaceItemsLocal(
      Workspace w,
      string localItem,
      RecursionType recursive,
      ItemType itemType,
      bool deleted,
      int options,
      PathLength maxServerPathLength)
    {
      this.PrepareStoredProcedure("prc_QueryWorkspaceItemsLocal", 3600);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", w.OwnerId);
      this.BindString("@workspaceName", w.Name, 64, false, SqlDbType.NVarChar);
      this.BindLocalItem("@localItem", localItem, true);
      this.BindByte("@depth", (byte) recursive);
      this.BindByte("@itemType", (byte) itemType, (byte) 0);
      this.BindBoolean("@deleted", deleted);
      this.BindInt("@options", options);
      this.BindPathWithGuidLength("@maxServerPathLength", maxServerPathLength);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<DeterminedItem>((ObjectBinder<DeterminedItem>) this.CreateDetermineLocalItemTypeColumns());
      resultCollection.AddBinder<WorkspaceItem>((ObjectBinder<WorkspaceItem>) this.CreateQueryWorkspaceItemsColumns());
      return resultCollection;
    }

    public virtual void PromotePendingWorkspaceMappings(
      Workspace workspace,
      int projectNotificationId)
    {
      this.PrepareStoredProcedure("prc_PromotePendingWorkspaceMappings");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindInt("@projectNotificationId", projectNotificationId);
      this.ExecuteNonQuery();
    }

    public virtual ResultCollection ReconcileLocalWorkspace(
      Workspace workspace,
      Guid pendingChangeSignature,
      LocalPendingChange[] changes,
      ServerItemLocalVersionUpdate[] localVersionUpdates,
      bool clearLocalVersionTable,
      bool throwOnProjectRenamed,
      PathLength maxServerPathLength)
    {
      this.PrepareStoredProcedure("prc_ReconcileLocalWorkspace", 3600);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindString("@workspaceName", workspace.Name, 64, true, SqlDbType.NVarChar);
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindGuid("@clientPendingChangeSignature", pendingChangeSignature);
      this.BindLocalPendingChangeTable("@pendingChangesIn", (IEnumerable<LocalPendingChange>) changes);
      this.BindLocalVersionTable("@localUpdates", (IEnumerable<BaseLocalVersionUpdate>) localVersionUpdates);
      this.BindBoolean("@clearLocalVersionTable", clearLocalVersionTable);
      this.BindBoolean("@throwOnProjectRenamed", throwOnProjectRenamed);
      this.BindPathWithGuidLength("@maxServerPathLength", maxServerPathLength);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      AnyProjectRenamesColumns binder1 = new AnyProjectRenamesColumns();
      resultCollection.AddBinder<int>((ObjectBinder<int>) binder1);
      binder1.MoveNext();
      if (binder1.Current > 0)
      {
        ReconcileBlockedByProjectRenameException projectRenameException = new ReconcileBlockedByProjectRenameException();
        resultCollection.NextResult();
        ProjectRevisionIdColumns binder2 = new ProjectRevisionIdColumns();
        resultCollection.AddBinder<int>((ObjectBinder<int>) binder2);
        binder2.MoveNext();
        projectRenameException.NewProjectRevisionId = binder2.Current;
        resultCollection.NextResult();
        ReconcileProjectRenamesColumns binder3 = new ReconcileProjectRenamesColumns();
        resultCollection.AddBinder<ReconcileProjectRename>((ObjectBinder<ReconcileProjectRename>) binder3);
        projectRenameException.OldProjectNames = binder3.Items.Select<ReconcileProjectRename, string>((System.Func<ReconcileProjectRename, string>) (s => s.OldProjectName)).ToArray<string>();
        projectRenameException.NewProjectNames = binder3.Items.Select<ReconcileProjectRename, string>((System.Func<ReconcileProjectRename, string>) (s => s.NewProjectName)).ToArray<string>();
        throw projectRenameException;
      }
      resultCollection.AddBinder<ReconcileResult>((ObjectBinder<ReconcileResult>) this.CreateReconcileResultColumns());
      resultCollection.AddBinder<Failure>((ObjectBinder<Failure>) this.CreatePendChangeFailureColumns(PendChangeFailureBinder.Caller.ReconcileLocalWorkspace));
      resultCollection.AddBinder<PendingChange>((ObjectBinder<PendingChange>) this.CreateQueryPendingChangesColumns(this.RequestContext));
      resultCollection.NextResult();
      return resultCollection;
    }

    public virtual void RemoveLocalConflict(Workspace workspace, int conflictId)
    {
      this.PrepareStoredProcedure("prc_RemoveLocalConflict");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindInt("@conflictId", conflictId);
      this.ExecuteNonQuery();
    }

    public virtual ResultCollection ReservePropertyIds(int numIdsToReserve, int conflictId = 0)
    {
      this.PrepareStoredProcedure("prc_ReservePropertyIds");
      this.BindInt("@conflictId", conflictId);
      this.BindInt("@numIdsToReserve", numIdsToReserve);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<PropertyDataspaceIdPair>((ObjectBinder<PropertyDataspaceIdPair>) new ReservePropertyIdsBinder15((VersionControlSqlResourceComponent) this));
      return resultCollection;
    }

    public virtual bool Shelve(
      Guid workspaceOwnerId,
      string workspaceName,
      Guid shelveSetOwnerId,
      string shelveSetName,
      int shelvesetVersion,
      string comment,
      string policyOverrideComment,
      IEnumerable<string> serverItems,
      CheckinNote checkInNote,
      VersionControlLink[] links,
      bool replace,
      StreamingCollection<PropertyValue> properties)
    {
      this.PrepareStoredProcedure("prc_Shelve");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindNullableGuid("@workspaceOwnerId", workspaceOwnerId);
      this.BindString("@workspaceName", workspaceName, 64, true, SqlDbType.NVarChar);
      this.BindNullableGuid("@shelveSetOwnerId", shelveSetOwnerId);
      this.BindString("@shelveSetName", shelveSetName, 64, true, SqlDbType.NVarChar);
      this.BindString("@comment", comment, -1, true, SqlDbType.NVarChar);
      this.BindString("@policyOverrideComment", policyOverrideComment, 2048, true, SqlDbType.NVarChar);
      this.BindInt("@shelvesetVersion", shelvesetVersion);
      CheckinNoteFieldValue[] rows = (CheckinNoteFieldValue[]) null;
      if (checkInNote != null && checkInNote.Values != null && checkInNote.Values.Length != 0)
        rows = checkInNote.Values;
      this.BindCheckinNoteFieldValueTable("@checkInNoteList", (IEnumerable<CheckinNoteFieldValue>) rows);
      this.BindBoolean("@replace", replace);
      this.BindVersionControlLinkTable("@linkList", (IEnumerable<VersionControlLink>) links);
      this.BindServerItemTable("@itemList", serverItems);
      this.BindVersionControlPropertyValueTable("@properties", (IEnumerable<PropertyValue>) properties);
      return 1 == (int) this.ExecuteScalar();
    }

    public virtual ResultCollection TrackMerges(
      int[] sourceChangesets,
      ItemIdentifier sourceItem,
      List<ItemIdentifier> targetItems,
      ItemSpec pathFilter)
    {
      this.PrepareStoredProcedure("prc_TrackMerges");
      this.BindUniqueInt32Table("@changesets", (IEnumerable<int>) sourceChangesets);
      if (pathFilter != null)
      {
        this.BindServerItem("@filterPath", this.BestEffortConvertPathPairToPathWithProjectGuid(pathFilter.ItemPathPair), false);
        this.BindInt("@filterDepth", (int) pathFilter.RecursionType);
      }
      this.BindServerItem("@sourceServerItem", this.BestEffortConvertPathPairToPathWithProjectGuid(sourceItem.ItemPathPair), false);
      List<string> rows = new List<string>(targetItems.Count);
      foreach (ItemIdentifier targetItem in targetItems)
        rows.Add(DBPath.ServerToDatabasePath(this.BestEffortConvertPathPairToPathWithProjectGuid(targetItem.ItemPathPair)));
      this.BindStringTable("@targetItemList", (IEnumerable<string>) rows);
      this.BindInt("@maxRowsEvaluate", this.VersionControlRequestContext.VersionControlService.GetMaxRowsEvaluated(this.VersionControlRequestContext));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<ExtendedMerge>((ObjectBinder<ExtendedMerge>) new ExtendedMergeBinder15((VersionControlSqlResourceComponent) this));
      resultCollection.AddBinder<ItemPathPair>((ObjectBinder<ItemPathPair>) this.CreateServerItemColumns());
      return resultCollection;
    }

    public virtual ResultCollection UndoPendingChange(
      Workspace workspace,
      IEnumerable<ItemPathPair> items,
      PathLength maxServerPathLength)
    {
      this.PrepareStoredProcedure("prc_UndoPendingChange", 3600);
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, true, SqlDbType.NVarChar);
      this.BindServerItemPairTypeTable("@itemList", items);
      this.BindPathWithGuidLength("@maxServerPathLength", maxServerPathLength);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<Failure>((ObjectBinder<Failure>) this.CreateBasicFailureColumns(this.ProcedureName));
      resultCollection.AddBinder<GetOperation>((ObjectBinder<GetOperation>) this.CreateGetOperationColumns());
      resultCollection.AddBinder<ChangePendedFlags>((ObjectBinder<ChangePendedFlags>) new ChangePendedFlagsBinder());
      return resultCollection;
    }

    public virtual void UpdateBranchObject(
      BranchProperties branchProperties,
      bool updateExisting,
      PathLength maxServerPathLength)
    {
      this.PrepareStoredProcedure("prc_UpdateBranchObject");
      this.BindServerItemPathPair("@rootServerItem", branchProperties.RootItem.ItemPathPair, false);
      if (!string.IsNullOrEmpty(branchProperties.Description))
        this.BindString("@description", branchProperties.Description, -1, false, SqlDbType.NVarChar);
      this.BindGuid("@ownerId", branchProperties.OwnerId);
      this.BindMappingTable("@mappingList", (IEnumerable<Mapping>) branchProperties.BranchMappings);
      if (branchProperties.ParentBranch != null)
        this.BindServerItemPathPair("@parentServerItem", branchProperties.ParentBranch.ItemPathPair, false);
      this.BindBoolean("@updateExisting", updateExisting);
      this.BindPathLength("@maxServerPathLength", maxServerPathLength);
      this.ExecuteNonQuery();
    }

    public virtual void UpdateLocalVersion(
      Workspace workspace,
      BaseLocalVersionUpdate[] updates,
      PathLength maxServerPathLength)
    {
      if (updates.Length < 2500)
      {
        this.PrepareStoredProcedure("prc_UpdateLocalVersion", 3600);
      }
      else
      {
        string sqlStatement = "EXEC prc_UpdateLocalVersion @partitionId, @serviceDataspaceId, @ownerId, @workspaceName, @localUpdates WITH RECOMPILE";
        this.PrepareSqlBatch(sqlStatement.Length, 3600);
        this.AddStatement(sqlStatement);
      }
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, true, SqlDbType.NVarChar);
      this.BindLocalVersionTable("@localUpdates", (IEnumerable<BaseLocalVersionUpdate>) updates);
      this.ExecuteNonQuery();
    }

    public virtual void UpdateRevertTo(Workspace workspace, int itemId, int revertToVersion)
    {
      this.PrepareStoredProcedure("prc_UpdateRevertTo");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, true, SqlDbType.NVarChar);
      this.BindInt("@itemId", itemId);
      this.BindInt("@versionRevertTo", revertToVersion);
      this.ExecuteNonQuery();
    }

    public virtual ResultCollection PendPropertyChange(
      Workspace workspace,
      IEnumerable<ExpandedChange> expandedChanges,
      bool silent)
    {
      this.PrepareStoredProcedure("prc_PendPropertyChange");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindBoolean("@silent", silent);
      this.BindPendPropertyChangeTable("@changeList", expandedChanges);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      if (!silent)
      {
        resultCollection.AddBinder<GetOperation>((ObjectBinder<GetOperation>) this.CreateGetOperationColumns());
        resultCollection.AddBinder<Warning>((ObjectBinder<Warning>) this.CreatePendWarningColumns());
      }
      resultCollection.AddBinder<Failure>((ObjectBinder<Failure>) this.CreateBasicFailureColumns(this.ProcedureName));
      return resultCollection;
    }

    public virtual ResultCollection Resolve(
      Workspace workspace,
      int conflictId,
      Resolution resolution,
      ItemPathPair name,
      int encoding,
      int propertyId,
      Guid? propertyDataspaceId,
      LockLevel lockLevel,
      bool disallowPropertyChangesOnAutoMerge,
      PathLength maxServerPathLength)
    {
      MergeFlags parameterValue = MergeFlags.None;
      switch (resolution)
      {
        case Resolution.AcceptMerge:
          parameterValue = MergeFlags.AcceptMerged;
          break;
        case Resolution.AcceptYours:
          parameterValue = MergeFlags.AcceptMine;
          break;
        case Resolution.AcceptTheirs:
          parameterValue = MergeFlags.AcceptTheirs;
          break;
        case Resolution.DeleteConflict:
          parameterValue = MergeFlags.DeleteConflict;
          break;
        case Resolution.AcceptYoursRenameTheirs:
          parameterValue = MergeFlags.AcceptYoursRenameTheirs;
          break;
      }
      this.PrepareStoredProcedure("prc_Resolve");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindInt("@conflictId", conflictId);
      this.BindInt("@resolution", (int) parameterValue);
      if (parameterValue == MergeFlags.DeleteConflict)
      {
        this.BindNullValue("@propertyDataspaceId", SqlDbType.Int);
        this.BindNullValue("@targetItemDataspaceId", SqlDbType.Int);
        this.BindNullValue("@targetServerItem", SqlDbType.NVarChar);
      }
      else
      {
        this.BindDataspaceIdAndServerItemPathPair("@targetItemDataspaceId", "@targetServerItem", name, true);
        this.BindNullableInt("@propertyDataspaceId", this.GetDataspaceIdDebug(propertyDataspaceId.Value, string.Empty), -1);
      }
      this.BindNullableInt("@encoding", encoding, -2);
      this.BindNullableInt("@propertyId", propertyId, -2);
      this.BindLockLevel("@lockStatus", lockLevel);
      this.BindBoolean("@disallowPropertyChangesOnAutoMerge", disallowPropertyChangesOnAutoMerge);
      this.BindPathWithGuidLength("@maxServerPathLength", maxServerPathLength);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<ServerException>((ObjectBinder<ServerException>) new PendChangeExceptionBinder15(this.RequestContext, (VersionControlSqlResourceComponent) this));
      resultCollection.AddBinder<GetOperation>((ObjectBinder<GetOperation>) this.CreateGetOperationColumns());
      resultCollection.AddBinder<GetOperation>((ObjectBinder<GetOperation>) this.CreateGetOperationColumns());
      resultCollection.AddBinder<ChangePendedFlags>((ObjectBinder<ChangePendedFlags>) new ChangePendedFlagsBinder());
      return resultCollection;
    }

    protected virtual SqlParameter BindRevertToVersion(
      string parameterName,
      IEnumerable<Tuple<int, int>> rows)
    {
      rows = rows ?? Enumerable.Empty<Tuple<int, int>>();
      System.Func<Tuple<int, int>, SqlDataRecord> selector = (System.Func<Tuple<int, int>, SqlDataRecord>) (entry =>
      {
        SqlDataRecord version = new SqlDataRecord(VersionedItemComponent.typ_VersionRevertTo);
        version.SetInt32(0, entry.Item1);
        version.SetInt32(1, entry.Item2);
        return version;
      });
      return this.BindTable(parameterName, "typ_VersionRevertTo", rows.Select<Tuple<int, int>, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindLocalConflictsTable(
      string parameterName,
      IEnumerable<PendingState> rows)
    {
      rows = rows ?? Enumerable.Empty<PendingState>();
      System.Func<PendingState, SqlDataRecord> selector = (System.Func<PendingState, SqlDataRecord>) (pendingState =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(VersionedItemComponent.typ_LocalConflicts);
        sqlDataRecord.SetInt32(0, (int) pendingState.ConflictInfo.ConflictType);
        sqlDataRecord.SetInt32(1, pendingState.ItemId);
        sqlDataRecord.SetInt32(2, pendingState.ConflictInfo.VersionFrom);
        sqlDataRecord.SetInt32(3, pendingState.ConflictInfo.PendingChangeId);
        if (pendingState.ConflictInfo.SourceLocalItem == null)
          sqlDataRecord.SetDBNull(4);
        else
          sqlDataRecord.SetString(4, DBPath.LocalToDatabasePath(pendingState.ConflictInfo.SourceLocalItem));
        if (pendingState.ConflictInfo.TargetLocalItem == null)
          sqlDataRecord.SetDBNull(5);
        else
          sqlDataRecord.SetString(5, DBPath.LocalToDatabasePath(pendingState.ConflictInfo.TargetLocalItem));
        sqlDataRecord.SetInt32(6, pendingState.ConflictInfo.Reason);
        sqlDataRecord.SetInt32(7, pendingState.RevertToVersion);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_LocalConflicts2", rows.Select<PendingState, SqlDataRecord>(selector));
    }

    public virtual void AddLocalConflictsBatch(
      Workspace workspace,
      IEnumerable<PendingState> localConflicts)
    {
      this.PrepareStoredProcedure("prc_AddLocalConflictsBatch");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, false, SqlDbType.NVarChar);
      this.BindLocalConflictsTable("@conflicts", localConflicts);
      this.ExecuteNonQuery();
    }

    public virtual void UpdateRevertToBatch(
      Workspace workspace,
      IEnumerable<Tuple<int, int>> revertToList)
    {
      this.PrepareStoredProcedure("prc_UpdateRevertToBatch");
      this.BindServiceDataspaceId("@serviceDataspaceId");
      this.BindGuid("@ownerId", workspace.OwnerId);
      this.BindString("@workspaceName", workspace.Name, 64, true, SqlDbType.NVarChar);
      this.BindRevertToVersion("@revertToVersions", revertToList);
      this.ExecuteNonQuery();
    }

    public virtual ResultCollection FixCorruption()
    {
      this.PrepareStoredProcedure("prc_FixTfvcCorruption", 0);
      return new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
    }

    public virtual List<Changeset> QueryChangesetRange(
      IEnumerable<Mapping> mappings,
      VersionSpec versionFrom,
      VersionSpec versionTo,
      bool includeFiles,
      int maxChangesets)
    {
      this.PrepareStoredProcedure("prc_QueryChangesetRange", 3600);
      List<string> rows1 = new List<string>();
      List<string> rows2 = new List<string>();
      foreach (Mapping mapping in mappings)
      {
        if (mapping.Type == WorkingFolderType.Cloak)
          rows2.Add(mapping.ServerItem);
        else
          rows1.Add(mapping.ServerItem);
      }
      this.BindServiceDataspaceId("@rootItemDataspaceId");
      this.BindServerItemTable("@includePaths", (IEnumerable<string>) rows1, true);
      this.BindServerItemTable("@excludePaths", (IEnumerable<string>) rows2, true);
      this.BindVersionSpec("@versionSpecFrom", versionFrom, false);
      this.BindVersionSpec("@versionSpecTo", versionTo, false);
      this.BindInt("@depth", 120);
      this.BindBoolean("@includeFiles", includeFiles);
      this.BindInt("@maxChangeSets", maxChangesets);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<Changeset>((ObjectBinder<Changeset>) new ChangesetColumns());
      return resultCollection.GetCurrent<Changeset>().Items;
    }

    public virtual ResultCollection InspectWorkspaces()
    {
      this.PrepareStoredProcedure("Tfvc.prc_InspectWorkspaces", 3600);
      this.BindInt("@includeWorkspaces", 1);
      this.BindInt("@includeShelvesets", 0);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<InspectWorkspaceInfo>((ObjectBinder<InspectWorkspaceInfo>) new InspectWorkspacesBinder());
      return resultCollection;
    }

    public virtual ResultCollection InspectShelvesets()
    {
      this.PrepareStoredProcedure("Tfvc.prc_InspectWorkspaces", 3600);
      this.BindInt("@includeWorkspaces", 0);
      this.BindInt("@includeShelvesets", 1);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<InspectShelvesetInfo>((ObjectBinder<InspectShelvesetInfo>) new InspectShelvesetsBinder());
      return resultCollection;
    }

    public virtual ResultCollection InspectLabels()
    {
      this.PrepareStoredProcedure("Tfvc.prc_InspectLabels", 3600);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<InspectLabelsInfo>((ObjectBinder<InspectLabelsInfo>) new InspectLabelsBinder());
      return resultCollection;
    }

    public virtual ResultCollection InspectBigFiles(ItemPathPair scopePath, int limit)
    {
      this.PrepareStoredProcedure("Tfvc.prc_InspectBigFiles", 3600);
      this.BindDataspaceIdAndServerItemPathPair("@dataspaceId", "@scopePath", scopePath, true);
      this.BindInt("@limit", limit);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<InspectBigFilesInfo>((ObjectBinder<InspectBigFilesInfo>) new InspectBigFilesBinder((VersionControlSqlResourceComponent) this));
      return resultCollection;
    }

    public virtual ResultCollection InspectOldFiles(ItemPathPair scopePath, int limit)
    {
      this.PrepareStoredProcedure("Tfvc.prc_InspectOldFiles", 3600);
      this.BindServerItemPathPair("@scopePath", scopePath, false);
      this.BindInt("@limit", limit);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<InspectOldFilesInfo>((ObjectBinder<InspectOldFilesInfo>) new InspectOldFilesBinder((VersionControlSqlResourceComponent) this));
      return resultCollection;
    }

    public virtual CodeMetrics QueryCodeMetrics(
      Guid projectId,
      int startingTimeBucket,
      int endTimeBucket)
    {
      throw new ServiceVersionNotSupportedException();
    }

    public virtual void DeleteCodeMetrics(int timePeriodsToRetain) => throw new ServiceVersionNotSupportedException();

    public virtual ResultCollection QueryItemsPaged(
      ItemSpec scopePath,
      int changesetId,
      ItemSpec lastItem,
      int options)
    {
      throw new ServiceVersionNotSupportedException();
    }

    public virtual ResultCollection QueryItemsPaged(
      ItemSpec scopePath,
      int changesetId,
      ItemSpec lastItem,
      int top,
      int options)
    {
      throw new ServiceVersionNotSupportedException();
    }

    public virtual ResultCollection QueryItemsByChangesetPaged(
      ItemSpec scopePath,
      int baseChangesetId,
      int targetChangesetId,
      ItemSpec lastItem,
      int top)
    {
      throw new ServiceVersionNotSupportedException();
    }

    public virtual ResultCollection QueryHistoryScore(
      ItemPathPair targetItemPathPair,
      VersionSpec versionItem,
      VersionSpec versionFrom,
      VersionSpec versionTo,
      Guid teamFoundationId,
      RecursionType recursive,
      bool includeFiles,
      bool slotMode,
      int maxChangesets,
      bool sortAscending,
      int maxChangesPerChangeset,
      int acceptableSeconds)
    {
      throw new ServiceVersionNotSupportedException();
    }

    public virtual ResultCollection QueryHistoryScore(
      ItemPathPair targetItemPathPair,
      VersionSpec versionItem,
      VersionSpec versionFrom,
      VersionSpec versionTo,
      Guid teamFoundationId,
      RecursionType recursive,
      bool includeFiles,
      bool slotMode,
      int maxChangesets,
      bool sortAscending,
      int maxChangesPerChangeset,
      int acceptableSeconds,
      bool isWildCard)
    {
      throw new ServiceVersionNotSupportedException();
    }

    public virtual TfvcFileStats QueryTfvcFileStats(ItemSpec scopePath) => throw new ServiceVersionNotSupportedException();

    public virtual void TfvcRemapObjectOwner(Guid oldVsid, Guid newVsid) => throw new ServiceVersionNotSupportedException();
  }
}
