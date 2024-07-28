// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityManagementComponent : TeamFoundationSqlResourceComponent
  {
    private const string s_area = "IdentityManagementComponent";
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      [400040] = new SqlExceptionFactory(typeof (IdentityAccountNameAlreadyInUseException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new IdentityAccountNameAlreadyInUseException(sqEr.ExtractString("oneAccountName"), sqEr.ExtractInt("collisionCount")))),
      [400045] = new SqlExceptionFactory(typeof (IdentityAliasAlreadyInUseException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new IdentityAliasAlreadyInUseException(sqEr.ExtractString("conflictingAlias")))),
      [400047] = new SqlExceptionFactory(typeof (DynamicIdentityTypeCreationNotSupportedException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new DynamicIdentityTypeCreationNotSupportedException())),
      [400048] = new SqlExceptionFactory(typeof (TooManyIdentitiesReturnedException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new TooManyIdentitiesReturnedException())),
      [400100] = new SqlExceptionFactory(typeof (HistoricalIdentityNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new HistoricalIdentityNotFoundException(sqEr.ToString(), (Exception) sqEx)))
    };
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[35]
    {
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent>(1),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent2>(2),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent3>(3),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent4>(4),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent5>(5),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent6>(6),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent7>(7),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent8>(8),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent9>(9),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent10>(10),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent11>(11),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent12>(12),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent13>(13),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent14>(14),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent15>(15),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent16>(16),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent17>(17),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent18>(18),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent19>(19),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent20>(20),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent21>(21),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent22>(22),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent23>(23),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent24>(24),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent25>(25),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent26>(26),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent27>(27),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent28>(28),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent29>(29),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent30>(30),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent31>(31),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent32>(32),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent33>(33),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent34>(34),
      (IComponentCreator) new ComponentCreator<IdentityManagementComponent35>(35)
    }, "IdentityManagement");

    protected override void SetupComponentCircuitBreaker(
      IVssRequestContext requestContext,
      string databaseName,
      ApplicationIntent applicationIntent)
    {
      string key = "IdentityManagementComponent-" + databaseName;
      this.ComponentLevelCommandSetter.AndCommandKey((Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) key);
      this.ComponentLevelCircuitBreakerProperties = (ICommandProperties) new CommandPropertiesRegistry(requestContext, (Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) key, this.ComponentLevelCommandSetter.CommandPropertiesDefaults);
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) IdentityManagementComponent.s_sqlExceptionFactories;

    protected override string TraceArea => nameof (IdentityManagementComponent);

    public IdentityManagementComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public void Install()
    {
      this.PrepareStoredProcedure("prc_IdentityInstall");
      this.ExecuteNonQuery();
    }

    public virtual List<Microsoft.VisualStudio.Services.Identity.Identity> UpdateIdentities(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> updates,
      HashSet<string> propertiesToUpdate,
      bool favorCurrentlyActive,
      bool updateIdentityAudit,
      bool allowMetadataUpdates,
      out List<Guid> createdAndBoundIds,
      out List<Guid> deletedIds,
      out IdentityChangedData identityChangedData,
      out List<KeyValuePair<Guid, Guid>> identitiesToTransfer)
    {
      createdAndBoundIds = new List<Guid>();
      return this.UpdateIdentities(updates, propertiesToUpdate, favorCurrentlyActive, updateIdentityAudit, allowMetadataUpdates, out deletedIds, out identityChangedData, out identitiesToTransfer);
    }

    public virtual List<Microsoft.VisualStudio.Services.Identity.Identity> UpdateIdentities(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> updates,
      HashSet<string> propertiesToUpdate,
      bool favorCurrentlyActive,
      bool updateIdentityAudit,
      bool allowMetadataUpdates,
      out List<Guid> deletedIds,
      out IdentityChangedData identityChangedData,
      out List<KeyValuePair<Guid, Guid>> identitiesToTransfer)
    {
      try
      {
        this.TraceEnter(4701000, nameof (UpdateIdentities));
        this.PrepareUpdateIdentitiesStoredProcedure(updates);
        this.BindIdentityTable("@identities", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) updates, propertiesToUpdate);
        this.BindGuid("@eventAuthor", this.Author);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<IdentityChangedData>((ObjectBinder<IdentityChangedData>) new IdentityManagementComponent.UpdateIdentitySummaryColumns());
          resultCollection.AddBinder<Tuple<int, Guid, bool>>((ObjectBinder<Tuple<int, Guid, bool>>) new IdentityManagementComponent.UpdateIdentityColumns());
          resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new IdentityManagementComponent.IdentityIdColumns());
          resultCollection.AddBinder<DescriptorChange>((ObjectBinder<DescriptorChange>) new IdentityManagementComponent.DescriptorChangeColumns());
          identityChangedData = resultCollection.GetCurrent<IdentityChangedData>().FirstOrDefault<IdentityChangedData>();
          if (identityChangedData == null)
            throw new UnexpectedDatabaseResultException(this.ProcedureName);
          resultCollection.NextResult();
          List<Microsoft.VisualStudio.Services.Identity.Identity> identityList = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
          foreach (Tuple<int, Guid, bool> tuple in resultCollection.GetCurrent<Tuple<int, Guid, bool>>())
          {
            Microsoft.VisualStudio.Services.Identity.Identity update = updates[tuple.Item1];
            update.Id = tuple.Item2;
            update.MasterId = tuple.Item2;
            if (tuple.Item3)
              identityList.Add(update);
          }
          resultCollection.NextResult();
          deletedIds = resultCollection.GetCurrent<Guid>().Items;
          if (identityChangedData.DescriptorChangeType == DescriptorChangeType.Minor)
          {
            resultCollection.NextResult();
            identityChangedData.DescriptorChanges = resultCollection.GetCurrent<DescriptorChange>().ToArray<DescriptorChange>();
          }
          identitiesToTransfer = new List<KeyValuePair<Guid, Guid>>();
          return identityList;
        }
      }
      finally
      {
        this.TraceLeave(4701009, nameof (UpdateIdentities));
      }
    }

    public virtual IList<Guid> FetchIdentityIdsBatch(
      int batchSize,
      int maxSequenceId = -1,
      bool incudeOnlyClaimsAndBindPendingTypes = false)
    {
      return (IList<Guid>) null;
    }

    public virtual int UpgradeIdentitiesToTargetResourceVersion(
      int targetResourceVersion,
      int updateBatchSize,
      int maxNumberOfIdentitiesToUpdate)
    {
      return -1;
    }

    public virtual int DowngradeIdentitiesToTargetResourceVersion(
      int targetResourceVersion,
      int updateBatchSize,
      int maxNumberOfIdentitiesToUpdate)
    {
      return -1;
    }

    public virtual int UpdateIdentityVsid(Guid oldVsid, Guid newVsid) => -1;

    public virtual int FireMajorDescriptorChangeNotification() => -1;

    public virtual IList<Guid> ReadIdentityIdsWithNoDirectoryAliasByPage(
      Guid tenantId,
      Guid? pageIndex,
      int pageSize)
    {
      return (IList<Guid>) null;
    }

    public virtual IList<Guid> ReadIdsForIdentitiesWithNoStorageKeyCuidMapByPage(
      Guid? pageIndex,
      int pageSize,
      string identityType,
      Guid tenantId)
    {
      return (IList<Guid>) null;
    }

    public virtual IList<Guid> ReadIdsForIdentitiesWithNoStorageKeyCuidMapByPage(
      Guid? pageIndex,
      int pageSize,
      string identityType)
    {
      return (IList<Guid>) null;
    }

    public virtual IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesByDomainAndOid(
      string domain,
      Guid externalId)
    {
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) null;
    }

    public virtual Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityByDomainAndOidWithLargestSequenceId(
      string domain,
      Guid externalId)
    {
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    public virtual ResultCollection ReadIdentities(
      IEnumerable<IdentityDescriptor> descriptors,
      IEnumerable<Guid> ids)
    {
      try
      {
        this.TraceEnter(4701010, nameof (ReadIdentities));
        this.PrepareStoredProcedure("prc_ReadIdentities");
        this.BindOrderedDescriptorTable("@descriptors", descriptors, true);
        this.BindOrderedGuidTable("@ids", ids, true);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<IdentityManagementComponent.IdentityData>((ObjectBinder<IdentityManagementComponent.IdentityData>) new IdentityManagementComponent.IdentitiesColumns());
        return resultCollection;
      }
      finally
      {
        this.TraceLeave(4701019, nameof (ReadIdentities));
      }
    }

    public virtual Microsoft.VisualStudio.Services.Identity.Identity ReadSocialIdentity(
      SocialDescriptor socialDescriptor)
    {
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    public virtual ResultCollection ReadIdentity(
      IdentitySearchFilter searchFactor,
      string factorValue,
      string domain,
      string account,
      int uniqueUserId,
      bool? isGroup)
    {
      try
      {
        this.TraceEnter(4701020, nameof (ReadIdentity));
        this.PrepareStoredProcedure("prc_ReadIdentity");
        this.BindInt("@searchFactor", (int) searchFactor);
        this.BindString("@factorValue", factorValue, 515, false, SqlDbType.NVarChar);
        this.BindString("@domain", domain, 256, true, SqlDbType.NVarChar);
        this.BindString("@account", account, 256, true, SqlDbType.NVarChar);
        this.BindInt("@uniqueUserId", uniqueUserId);
        if (isGroup.HasValue)
          this.BindBoolean("@isGroup", isGroup.Value);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.Identity.Identity>((ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>) new IdentityManagementComponent.IdentityColumns());
        return resultCollection;
      }
      finally
      {
        this.TraceLeave(4701029, nameof (ReadIdentity));
      }
    }

    public virtual ResultCollection GetChanges(
      int sequenceId,
      ref int lastSequenceId,
      long firstAuditSequenceId,
      bool useIdentityAudit,
      IEnumerable<Guid> scopedIdentityIds = null,
      bool includeDescriptorChanges = false)
    {
      try
      {
        this.TraceEnter(4701030, nameof (GetChanges));
        this.PrepareStoredProcedure("prc_GetIdentityChanges");
        this.BindInt("@sequenceId", sequenceId);
        if (lastSequenceId > 0)
          this.BindInt("@lastSequenceId", lastSequenceId);
        SqlDataReader reader = this.ExecuteReader();
        if (!reader.Read())
          throw new UnexpectedDatabaseResultException(this.ProcedureName);
        lastSequenceId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException(this.ProcedureName);
        ResultCollection changes = new ResultCollection((IDataReader) reader, this.ProcedureName, this.RequestContext);
        changes.AddBinder<Guid>((ObjectBinder<Guid>) new IdentityManagementComponent.IdentityIdColumns());
        return changes;
      }
      finally
      {
        this.TraceLeave(4701039, nameof (GetChanges));
      }
    }

    public virtual long GetLatestIdentitySequenceId(bool useIdentityAudit)
    {
      int lastSequenceId = 0;
      using (this.GetChanges(int.MaxValue, ref lastSequenceId, 1L, useIdentityAudit))
        return (long) lastSequenceId;
    }

    public virtual ResultCollection GetScopedIdentityChanges(
      long sequenceId,
      Guid scopeId,
      bool useIdentityAudit,
      out long lastSequenceId)
    {
      throw new NotImplementedException();
    }

    public virtual ResultCollection GetScopedIdentityChanges(
      long sequenceId,
      Guid scopeId,
      bool useIdentityAudit,
      bool includeDescriptorChanges,
      out long lastSequenceId)
    {
      return this.GetScopedIdentityChanges(sequenceId, scopeId, useIdentityAudit, out lastSequenceId);
    }

    public virtual ResultCollection GetScopedPagedIdentityChanges(
      long sequenceId,
      Guid scopeId,
      bool useIdentityAudit,
      int pageSize,
      out long lastSequenceId)
    {
      return this.GetScopedIdentityChanges(sequenceId, scopeId, useIdentityAudit, out lastSequenceId);
    }

    public virtual int InvalidateIdentities(bool updateIdentityAudit, IList<Guid> identityIds = null) => -1;

    public ResultCollection ReadSyncQueue(int limit)
    {
      try
      {
        this.TraceEnter(4701040, nameof (ReadSyncQueue));
        this.PrepareStoredProcedure("prc_ReadSyncQueue");
        this.BindInt("@limit", limit);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<SyncQueueData>((ObjectBinder<SyncQueueData>) new IdentityManagementComponent.SyncQueueColumns());
        return resultCollection;
      }
      finally
      {
        this.TraceLeave(4701049, nameof (ReadSyncQueue));
      }
    }

    public void UpdateSyncQueue(
      IEnumerable<Guid> queueDeletes,
      IEnumerable<Tuple<Guid, bool, bool>> queueUpdates)
    {
      try
      {
        this.TraceEnter(4701050, nameof (UpdateSyncQueue));
        this.PrepareStoredProcedure("prc_UpdateSyncQueue");
        this.BindGuidTable("@queueDeletes", queueDeletes);
        this.BindIdentityQueueTable("@queueUpdates", queueUpdates);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.TraceLeave(4701059, nameof (UpdateSyncQueue));
      }
    }

    public virtual int CleanupDescriptorChangeQueue(int beforeDays) => -1;

    public virtual List<IdentityAuditRecord> GetIdentityAuditRecords(DateTime lastUpdatedOnOrBefore) => new List<IdentityAuditRecord>();

    public virtual void DeleteIdentityAuditRecords(long sequenceId)
    {
    }

    public virtual List<IdentityManagementComponent.GdprClaimsIdentity> FetchGdprClaimsIdentities() => (List<IdentityManagementComponent.GdprClaimsIdentity>) null;

    public virtual IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesByTypeId(
      byte typeId)
    {
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    public virtual IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesByTypeIdAndPartialSid(
      byte typeId,
      string partialSid)
    {
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    public virtual IList<Guid> GetIdentityIdsByTypeIdAndPartialSid(
      byte typeId,
      string partialSid,
      bool treatSidAsPrefix = true)
    {
      return (IList<Guid>) new List<Guid>();
    }

    public virtual void InsertIdentities(IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
    }

    public virtual IdentityDescriptor GetIdentityDescriptorById(Guid id) => (IdentityDescriptor) null;

    public virtual void InsertIdentities(IList<Microsoft.VisualStudio.Services.Identity.Identity> identities, bool ignoreDuplicates) => throw new NotSupportedException();

    public virtual bool DeleteIdentities(IList<Guid> identityIds, bool sleepIfBusy = false) => false;

    public virtual bool DeleteHistoricalIdentities(IList<Guid> identityIds) => false;

    public virtual void InsertIdentityRepairChanges(
      IReadOnlyList<IdentityRepairChange> identityRepairChanges)
    {
      throw new NotSupportedException();
    }

    public virtual void UpdateIdentityId(Guid identityId, Guid newIdentityId) => throw new NotSupportedException();

    protected virtual void PrepareUpdateIdentitiesStoredProcedure(IList<Microsoft.VisualStudio.Services.Identity.Identity> updates)
    {
      int commandTimeout = this.CommandTimeout;
      if (commandTimeout != 0)
      {
        if (updates.Count >= 100000 && commandTimeout < 21600)
          commandTimeout = 21600;
        else if (updates.Count >= 5000 && commandTimeout < 3600)
          commandTimeout = 3600;
      }
      this.PrepareStoredProcedure("prc_UpdateIdentities", commandTimeout);
    }

    public virtual bool SwapIdentity(string domain, string accountName, Guid id1, Guid id2) => throw new NotSupportedException();

    protected class UpdateIdentitySummaryColumns : ObjectBinder<IdentityChangedData>
    {
      private SqlColumnBinder DescriptorChangeType = new SqlColumnBinder(nameof (DescriptorChangeType));
      private SqlColumnBinder IdentitySequenceId = new SqlColumnBinder(nameof (IdentitySequenceId));
      private SqlColumnBinder GroupSequenceId = new SqlColumnBinder(nameof (GroupSequenceId));

      protected override IdentityChangedData Bind() => new IdentityChangedData()
      {
        IdentitySequenceId = this.IdentitySequenceId.GetInt32((IDataReader) this.Reader),
        GroupSequenceId = this.GroupSequenceId.GetInt32((IDataReader) this.Reader),
        DescriptorChangeType = (DescriptorChangeType) this.DescriptorChangeType.GetInt32((IDataReader) this.Reader)
      };
    }

    protected class IdentityAuditRecordColumns : ObjectBinder<IdentityAuditRecord>
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder SequenceId = new SqlColumnBinder(nameof (SequenceId));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));

      protected override IdentityAuditRecord Bind() => new IdentityAuditRecord()
      {
        IdentityId = this.Id.GetGuid((IDataReader) this.Reader),
        SequenceId = this.SequenceId.GetInt64((IDataReader) this.Reader),
        LastUpdated = this.LastUpdated.GetDateTimeOffset(this.Reader)
      };
    }

    protected class DescriptorChangeColumns : ObjectBinder<DescriptorChange>
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));

      protected override DescriptorChange Bind() => new DescriptorChange()
      {
        MasterId = this.Id.GetGuid((IDataReader) this.Reader)
      };
    }

    protected class UpdateIdentityColumns : ObjectBinder<Tuple<int, Guid, bool>>
    {
      private SqlColumnBinder OrderId = new SqlColumnBinder(nameof (OrderId));
      private SqlColumnBinder IdentityId = new SqlColumnBinder(nameof (IdentityId));
      private SqlColumnBinder Updated = new SqlColumnBinder(nameof (Updated));

      protected override Tuple<int, Guid, bool> Bind() => new Tuple<int, Guid, bool>(this.OrderId.GetInt32((IDataReader) this.Reader), this.IdentityId.GetGuid((IDataReader) this.Reader), this.Updated.GetBoolean((IDataReader) this.Reader));
    }

    protected class IdentityMembershipColumns : 
      ObjectBinder<IdentityManagementComponent.IdentityMembership>
    {
      private SqlColumnBinder ContainerId = new SqlColumnBinder(nameof (ContainerId));
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder Sid = new SqlColumnBinder(nameof (Sid));
      private SqlColumnBinder Type = new SqlColumnBinder(nameof (Type));

      protected override IdentityManagementComponent.IdentityMembership Bind()
      {
        Guid guid1 = this.ContainerId.GetGuid((IDataReader) this.Reader);
        Guid guid2 = this.Id.GetGuid((IDataReader) this.Reader);
        string identifier = this.Sid.GetString((IDataReader) this.Reader, false);
        string identityType = this.Type.GetString((IDataReader) this.Reader, false);
        return new IdentityManagementComponent.IdentityMembership()
        {
          ContainerId = guid1,
          Id = guid2,
          Descriptor = new IdentityDescriptor(identityType, identifier)
        };
      }
    }

    protected class IdentityColumns : ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>
    {
      protected SqlColumnBinder Sid = new SqlColumnBinder(nameof (Sid));
      private SqlColumnBinder Type = new SqlColumnBinder(nameof (Type));
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder ProviderDisplayName = new SqlColumnBinder(nameof (ProviderDisplayName));
      private SqlColumnBinder DisplayName = new SqlColumnBinder(nameof (DisplayName));
      private SqlColumnBinder Description = new SqlColumnBinder(nameof (Description));
      private SqlColumnBinder Domain = new SqlColumnBinder(nameof (Domain));
      private SqlColumnBinder AccountName = new SqlColumnBinder(nameof (AccountName));
      private SqlColumnBinder DistinguishedName = new SqlColumnBinder(nameof (DistinguishedName));
      private SqlColumnBinder MailAddress = new SqlColumnBinder(nameof (MailAddress));
      private SqlColumnBinder IsGroup = new SqlColumnBinder(nameof (IsGroup));
      private SqlColumnBinder UniqueUserId = new SqlColumnBinder(nameof (UniqueUserId));

      protected override Microsoft.VisualStudio.Services.Identity.Identity Bind() => this.Bind(this.Reader);

      internal virtual Microsoft.VisualStudio.Services.Identity.Identity Bind(SqlDataReader reader) => this.BindColumnsExceptDescriptor(reader, this.BindIdentityDescriptor(reader));

      protected virtual IdentityDescriptor BindIdentityDescriptor(SqlDataReader reader)
      {
        string identifier = this.Sid.GetString((IDataReader) reader, false);
        return new IdentityDescriptor(this.Type.GetString((IDataReader) reader, false), identifier);
      }

      protected virtual string BindCustomDisplayName(
        SqlDataReader reader,
        string providerDisplayName)
      {
        string y = DBPath.DatabaseToUserPath(this.DisplayName.GetString((IDataReader) reader, false));
        if (VssStringComparer.IdentityNameOrdinal.Equals(providerDisplayName, y))
          y = (string) null;
        return y;
      }

      private Microsoft.VisualStudio.Services.Identity.Identity BindColumnsExceptDescriptor(
        SqlDataReader reader,
        IdentityDescriptor descriptor)
      {
        Guid guid = this.Id.GetGuid((IDataReader) reader);
        string userPath = DBPath.DatabaseToUserPath(this.ProviderDisplayName.GetString((IDataReader) reader, false));
        string y = this.BindCustomDisplayName(reader, userPath);
        if (VssStringComparer.IdentityNameOrdinal.Equals(userPath, y))
          y = (string) null;
        string str1 = this.Description.GetString((IDataReader) reader, false);
        string str2 = this.Domain.GetString((IDataReader) reader, true);
        string str3 = this.AccountName.GetString((IDataReader) reader, false);
        string str4 = this.DistinguishedName.GetString((IDataReader) reader, false);
        string str5 = this.MailAddress.GetString((IDataReader) reader, false);
        bool boolean = this.IsGroup.GetBoolean((IDataReader) reader, false);
        int int32 = this.UniqueUserId.GetInt32((IDataReader) reader, 0);
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = new Microsoft.VisualStudio.Services.Identity.Identity();
        identity1.Id = guid;
        identity1.Descriptor = descriptor;
        identity1.ProviderDisplayName = userPath;
        identity1.CustomDisplayName = y;
        identity1.IsActive = true;
        identity1.UniqueUserId = int32;
        identity1.IsContainer = boolean;
        identity1.Members = (ICollection<IdentityDescriptor>) new List<IdentityDescriptor>();
        identity1.MemberOf = (ICollection<IdentityDescriptor>) new List<IdentityDescriptor>();
        identity1.MemberIds = (ICollection<Guid>) new List<Guid>();
        identity1.MemberOfIds = (ICollection<Guid>) new List<Guid>();
        identity1.MasterId = guid;
        identity1.ValidateProperties = false;
        Microsoft.VisualStudio.Services.Identity.Identity identity2 = identity1;
        if (boolean)
        {
          identity2.SetProperty("SchemaClassName", (object) "Group");
          identity2.SetProperty("SecurityGroup", (object) "SecurityGroup");
        }
        else
          identity2.SetProperty("SchemaClassName", (object) "User");
        identity2.SetProperty("Description", (object) str1);
        identity2.SetProperty("Domain", (object) str2);
        identity2.SetProperty("Account", (object) str3);
        identity2.SetProperty("DN", (object) str4);
        identity2.SetProperty("Mail", (object) str5);
        identity2.SetProperty("SpecialType", (object) "Generic");
        if (identity2.UniqueUserId != 0)
          identity2.IsActive = false;
        return identity2;
      }
    }

    protected class IdentitiesColumns : ObjectBinder<IdentityManagementComponent.IdentityData>
    {
      private readonly IdentityManagementComponent.IdentityColumns IdentityColumns = new IdentityManagementComponent.IdentityColumns();
      private SqlColumnBinder OrderId = new SqlColumnBinder(nameof (OrderId));

      protected override IdentityManagementComponent.IdentityData Bind() => new IdentityManagementComponent.IdentityData()
      {
        Identity = this.IdentityColumns.Bind(this.Reader),
        OrderId = this.OrderId.GetInt32((IDataReader) this.Reader)
      };
    }

    protected class IdentityIdColumns : ObjectBinder<Guid>
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));

      protected override Guid Bind() => this.Id.GetGuid((IDataReader) this.Reader);
    }

    protected class GdprClaimsIdentityColumns : 
      ObjectBinder<IdentityManagementComponent.GdprClaimsIdentity>
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder Sid = new SqlColumnBinder(nameof (Sid));
      private SqlColumnBinder TypeId = new SqlColumnBinder(nameof (TypeId));

      protected override IdentityManagementComponent.GdprClaimsIdentity Bind()
      {
        Guid guid = this.Id.GetGuid((IDataReader) this.Reader);
        string identifier = this.Sid.GetString((IDataReader) this.Reader, false);
        byte typeId = this.TypeId.GetByte((IDataReader) this.Reader);
        IdentityDescriptor identityDescriptor = new IdentityDescriptor(IdentityTypeMapper.Instance.GetTypeNameFromId(typeId), identifier);
        return new IdentityManagementComponent.GdprClaimsIdentity()
        {
          Id = guid,
          Descriptor = identityDescriptor
        };
      }
    }

    protected class SyncQueueColumns : ObjectBinder<SyncQueueData>
    {
      private SqlColumnBinder IdentityId = new SqlColumnBinder(nameof (IdentityId));
      private SqlColumnBinder Recursive = new SqlColumnBinder(nameof (Recursive));
      private SqlColumnBinder Log = new SqlColumnBinder(nameof (Log));

      protected override SyncQueueData Bind() => new SyncQueueData()
      {
        IdentityId = this.IdentityId.GetGuid((IDataReader) this.Reader),
        Recursive = this.Recursive.GetBoolean((IDataReader) this.Reader),
        Log = this.Log.GetBoolean((IDataReader) this.Reader)
      };
    }

    internal class IdentityMembership
    {
      public Guid ContainerId { get; set; }

      public Guid Id { get; set; }

      public IdentityDescriptor Descriptor { get; set; }
    }

    internal class IdentityData
    {
      public Microsoft.VisualStudio.Services.Identity.Identity Identity { get; set; }

      public int OrderId { get; set; }
    }

    public class GdprClaimsIdentity
    {
      public Guid Id { get; set; }

      public IdentityDescriptor Descriptor { get; set; }
    }

    internal struct ReferencedIdentity
    {
      public Guid IdentityId;
      public IdentityManagementComponent.ReferencedIdentityLocation Location;
    }

    internal enum ReferencedIdentityLocation : byte
    {
      Unknown,
      Local,
      Remote,
    }

    protected class ReferencedIdentityColumns2 : 
      ObjectBinder<IdentityManagementComponent.ReferencedIdentity>
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
      private SqlColumnBinder IsLocal = new SqlColumnBinder(nameof (IsLocal));

      protected override IdentityManagementComponent.ReferencedIdentity Bind() => new IdentityManagementComponent.ReferencedIdentity()
      {
        IdentityId = this.Id.GetGuid((IDataReader) this.Reader),
        Location = this.IsLocal.GetBoolean((IDataReader) this.Reader) ? IdentityManagementComponent.ReferencedIdentityLocation.Local : IdentityManagementComponent.ReferencedIdentityLocation.Remote
      };
    }
  }
}
