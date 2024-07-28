// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalSqlResourceComponent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalSqlResourceComponent : SqlAccess
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[47]
    {
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent>(1, true),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent2>(2),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent3>(3),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent4>(4),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent5>(5),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent6>(6),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent6>(7),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent7>(8),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent8>(9),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent8>(10),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent9>(11),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent9>(12),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent9>(13),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent10>(14),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent10>(15),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent10>(16),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent10>(17),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent11>(18),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent11>(19),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent11>(20),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent11>(21),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent11>(22),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent11>(23),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent11>(24),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent11>(25),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent11>(26),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent11>(27),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent12>(28),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent12>(29),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent12>(30),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent12>(31),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent13>(32),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent13>(33),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent14>(34),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent15>(35),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent16>(36),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent17>(37),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent18>(38),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent18>(39),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent18>(40),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent19>(41),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent20>(42),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent21>(43),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent22>(44),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent23>(45),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent24>(46),
      (IComponentCreator) new ComponentCreator<DalSqlResourceComponent25>(47)
    }, "WorkItemTracking");
    protected const int MAX_URI_CHUNK_SIZE = 16000;

    internal SqlParameter BindSyncIdentityTable(
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      return this.BindBasicTvp<Microsoft.VisualStudio.Services.Identity.Identity>((WorkItemTrackingResourceComponent.TvpRecordBinder<Microsoft.VisualStudio.Services.Identity.Identity>) new DalSqlResourceComponent.WitSyncIdentityTableRecordBinder(), parameterName, identities);
    }

    protected SqlParameter BindSyncMembershipTable(
      string parameterName,
      IEnumerable<SyncBase.Membership> memberships)
    {
      return this.BindBasicTvp<SyncBase.Membership>((WorkItemTrackingResourceComponent.TvpRecordBinder<SyncBase.Membership>) new DalSqlResourceComponent.WitSyncMembershipTableRecordBinder(), parameterName, memberships);
    }

    protected SqlParameter BindIdentityMembershipTable(
      string parameterName,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      return this.BindBasicTvp<Microsoft.VisualStudio.Services.Identity.Identity>((WorkItemTrackingResourceComponent.TvpRecordBinder<Microsoft.VisualStudio.Services.Identity.Identity>) new DalSqlResourceComponent.WitIdentityMembershipTableRecordBinder(), parameterName, identities);
    }

    public static DalSqlResourceComponent CreateComponent(
      IVssRequestContext requestContext,
      DatabaseConnectionType connectionType = DatabaseConnectionType.Default)
    {
      return requestContext.CreateComponent<DalSqlResourceComponent>("WorkItem", new DatabaseConnectionType?(connectionType));
    }

    public DalSqlResourceComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.None;

    public virtual IEnumerable<int> GetOrphanAttachments() => (IEnumerable<int>) new List<int>();

    public virtual void DeleteOrphanAttachmentsMetadata(IEnumerable<int> tfsFileIds)
    {
    }

    public virtual void DestroyAttachments(
      IVssIdentity userIdentity,
      IEnumerable<int> workItemIds,
      string comment,
      bool dualSave)
    {
    }

    public virtual void AddNewBuild(
      string buildName,
      string project,
      string buildDefinitionName,
      int maxBuildListSize)
    {
    }

    public virtual void GetWorkItemIds(
      long rowVersion,
      bool fDestroyed,
      int batchSize,
      long offset,
      out ResultCollection rc)
    {
      try
      {
        if (fDestroyed)
          this.PrepareStoredProcedure("GetDestroyedWorkItemIds");
        else
          this.PrepareStoredProcedure("GetChangedWorkItemIds");
        this.BindLong("@rowVersion", rowVersion);
        IDataReader reader = this.ExecuteReader();
        rc = new ResultCollection(reader, this.ProcedureName, this.RequestContext);
        rc.AddBinder<WorkItemId>((ObjectBinder<WorkItemId>) new WorkItemIdBinder(0L));
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    public virtual void GetWorkItemLinkChanges(
      long rowVersion,
      bool bypassPermissions,
      int batchSize,
      out ResultCollection rc)
    {
      try
      {
        if (!this.RequestContext.IsSystemContext)
          this.RequestContext.GetExtension<IAuthorizationProviderFactory>().EnsurePermitted(this.RequestContext, PermissionNamespaces.Global, "SYNCHRONIZE_READ");
        this.PrepareStoredProcedure(nameof (GetWorkItemLinkChanges), 3600);
        this.BindLong("@rowVersion", rowVersion);
        IDataReader reader = this.ExecuteReader();
        rc = new ResultCollection(reader, this.ProcedureName, this.RequestContext);
        rc.AddBinder<WorkItemLinkChange>((ObjectBinder<WorkItemLinkChange>) this.GetWorkItemLinkChangeBinder((PermissionCheckHelper) null));
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    public virtual long GetWorkItemLinkTimestampForDate(DateTime dateTime) => 0;

    public virtual Tuple<DateTime, DateTime> GetWorkItemLinkDateForTimestamp(long timestamp) => new Tuple<DateTime, DateTime>(DateTime.MinValue, DateTime.MinValue);

    public virtual void GetWorkItemLinkChanges(
      Guid? projectId,
      IEnumerable<string> types,
      IEnumerable<string> linkTypes,
      long rowVersion,
      bool bypassPermissions,
      int batchSize,
      DateTime? createdDateWatermark,
      DateTime? removedDateWatermark,
      int uncommittedChangesLookbackWindowInSeconds,
      out ResultCollection rc)
    {
      throw new NotImplementedException();
    }

    public virtual void ResyncIdentities1(
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      IEnumerable<SyncBase.Membership> members,
      Tuple<int, int> seqId)
    {
      throw new NotImplementedException();
    }

    public virtual void UpdateQueryTexts(IEnumerable<KeyValuePair<Guid, string>> queries)
    {
      string sqlStatement = "\r\nUPDATE  QI\r\nSET     QI.Text = UpdatedQueryItem.[Value]\r\nFROM    dbo.QueryItems AS QI \r\nJOIN    @queries AS UpdatedQueryItem\r\nON      QI.ID = UpdatedQueryItem.[Key]\r\n";
      this.PrepareSqlBatch(sqlStatement.Length, false);
      this.AddStatement(sqlStatement);
      this.BindKeyValuePairGuidStringTable("@queries", queries);
      this.ExecuteNonQuery();
    }

    public virtual IEnumerable<KeyValuePair<Guid, string>> GetAllQueryTexts()
    {
      string sqlStatement = "\r\nSELECT ID, Text \r\nFROM dbo.QueryItems\r\nWHERE fFolder = 0 AND fDeleted = 0\r\n";
      this.PrepareSqlBatch(sqlStatement.Length, false);
      this.AddStatement(sqlStatement);
      return this.ReadQueryTexts(this.ExecuteReader());
    }

    protected virtual IEnumerable<KeyValuePair<Guid, string>> ReadQueryTexts(IDataReader reader)
    {
      List<KeyValuePair<Guid, string>> keyValuePairList = new List<KeyValuePair<Guid, string>>();
      while (reader.Read())
        keyValuePairList.Add(new KeyValuePair<Guid, string>(reader.GetGuid(0), reader.GetString(1)));
      return (IEnumerable<KeyValuePair<Guid, string>>) keyValuePairList;
    }

    public virtual void DeletePrivateWorkItemTrackingData(Guid teamFoundationId)
    {
    }

    protected virtual void BindCollectionAndAccountHostIds()
    {
    }

    protected virtual void BindIdentityCategory(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
    }

    public virtual void InsertDomainAccountsSVRulesAndUpdateCacheStamp()
    {
    }

    public virtual void DeleteDomainAccountsSVRulesAndUpdateCacheStamp()
    {
    }

    public virtual IEnumerable<string> GetMissingIdentities(IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> groups) => Enumerable.Empty<string>();

    protected virtual void BindIdentityColumn(IVssIdentity caller) => this.BindString("@userSID", caller.Descriptor.Identifier, 256, false, SqlDbType.NVarChar);

    public virtual AdminMetadata GetMinimumAdminMetaData(
      int clientVersion,
      long treeCacheStamp,
      long fieldsCacheStamp,
      long fieldUsagesCacheStamp)
    {
      return (AdminMetadata) null;
    }

    protected virtual WorkItemLinkChangeBinder GetWorkItemLinkChangeBinder(
      PermissionCheckHelper helper)
    {
      return new WorkItemLinkChangeBinder((IPermissionCheckHelper) helper);
    }

    private class WitSyncIdentityTableRecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<Microsoft.VisualStudio.Services.Identity.Identity>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[8]
      {
        new SqlMetaData("D", SqlDbType.NVarChar, 256L),
        new SqlMetaData("U", SqlDbType.NVarChar, 256L),
        new SqlMetaData("C", SqlDbType.TinyInt),
        new SqlMetaData("T", SqlDbType.TinyInt),
        new SqlMetaData("S", SqlDbType.NVarChar, 256L),
        new SqlMetaData("N", SqlDbType.NVarChar, 256L),
        new SqlMetaData("I", SqlDbType.UniqueIdentifier),
        new SqlMetaData("F", SqlDbType.Bit)
      };
      public static readonly DalSqlResourceComponent.WitSyncIdentityTableRecordBinder Binder = new DalSqlResourceComponent.WitSyncIdentityTableRecordBinder();

      public override string TypeName => "typ_WitSyncIdentityTable";

      protected override SqlMetaData[] TvpMetadata => DalSqlResourceComponent.WitSyncIdentityTableRecordBinder.s_metadata;

      public override void SetRecordValues(WorkItemTrackingSqlDataRecord record, Microsoft.VisualStudio.Services.Identity.Identity identity)
      {
        string property1 = identity.GetProperty<string>("Domain", string.Empty);
        record.SetString(0, property1);
        string property2 = identity.GetProperty<string>("Account", string.Empty);
        record.SetString(1, property2);
        record.SetByte(2, (byte) IdentityConstantsNormalizer.GetBisIdentityType((IVssIdentity) identity));
        string property3 = identity.GetProperty<string>("SpecialType", string.Empty);
        record.SetByte(3, (byte) (GroupSpecialType) Enum.Parse(typeof (GroupSpecialType), property3));
        record.SetString(4, identity.Descriptor.Identifier);
        string str = identity.DisplayName;
        if (str.Length > 256)
          str = str.Substring(0, 256);
        record.SetString(5, str);
        record.SetGuid(6, identity.Id);
        record.SetBoolean(7, !identity.IsActive);
      }
    }

    private class WitSyncMembershipTableRecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<SyncBase.Membership>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[2]
      {
        new SqlMetaData("O", SqlDbType.NVarChar, 256L),
        new SqlMetaData("M", SqlDbType.NVarChar, 256L)
      };

      public override string TypeName => "typ_WitSyncMembershipTable";

      protected override SqlMetaData[] TvpMetadata => DalSqlResourceComponent.WitSyncMembershipTableRecordBinder.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        SyncBase.Membership entry)
      {
        record.SetString(0, entry.Owner);
        record.SetString(1, entry.Member);
      }
    }

    private class WitIdentityMembershipTableRecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<Microsoft.VisualStudio.Services.Identity.Identity>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[1]
      {
        new SqlMetaData("sid", SqlDbType.VarChar, 256L)
      };

      public override string TypeName => "typ_WitIdentityMembershipTable";

      protected override SqlMetaData[] TvpMetadata => DalSqlResourceComponent.WitIdentityMembershipTableRecordBinder.s_metadata;

      public override void SetRecordValues(WorkItemTrackingSqlDataRecord record, Microsoft.VisualStudio.Services.Identity.Identity identity) => record.SetString(0, identity.Descriptor.Identifier);
    }
  }
}
