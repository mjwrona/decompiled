// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.ReindexingStatusTable
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class ReindexingStatusTable : SQLTable<ReindexingStatusEntry>
  {
    private const string ServiceName = "Search_ReindexingStatus";
    protected const int BatchSizeForUpserting = 500;
    protected IEnumerable<IEntityType> m_entityTypes;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<ReindexingStatusTable>(1, true),
      (IComponentCreator) new ComponentCreator<ReindexingStatusTableV2>(2),
      (IComponentCreator) new ComponentCreator<ReindexingStatusTableV3>(3)
    }, "Search_ReindexingStatus");

    public ReindexingStatusTable()
      : base(false)
    {
    }

    internal ReindexingStatusTable(string connectionString, IVssRequestContext requestContext)
      : base(false)
    {
      ISqlConnectionInfo connectionInfo = SqlConnectionInfoFactory.Create(connectionString);
      this.m_entityTypes = requestContext.To(TeamFoundationHostType.Deployment).GetService<IEntityService>().GetEntityTypes();
      this.Initialize(connectionInfo, 3600, 20, 1, 1, this.GetDefaultLogger(), (CircuitBreakerDatabaseProperties) null);
    }

    protected override void Initialize(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int serviceVersion,
      DatabaseConnectionType connectionType,
      ITFLogger logger)
    {
      this.m_entityTypes = requestContext.To(TeamFoundationHostType.Deployment).GetService<IEntityService>().GetEntityTypes();
      base.Initialize(requestContext, dataspaceCategory, dataspaceIdentifier, serviceVersion, connectionType, logger);
    }

    public override ReindexingStatusEntry Insert(ReindexingStatusEntry reindexingStatus)
    {
      this.ValidateNotNull<ReindexingStatusEntry>(nameof (reindexingStatus), reindexingStatus);
      try
      {
        this.PrepareStoredProcedure("Search.prc_AddOrUpdateEntryForReindexingStatusTable");
        this.BindGuid("@collectionId", reindexingStatus.CollectionId);
        this.BindByte("@status", (byte) reindexingStatus.Status);
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format("Failed to Add entry with Collection ID {0}, Status {1} with SQL Azure platform.", (object) reindexingStatus.CollectionId, (object) reindexingStatus.Status));
      }
      return (ReindexingStatusEntry) null;
    }

    public override List<ReindexingStatusEntry> RetriveTableEntityList(
      int count,
      TableEntityFilterList filterList)
    {
      this.ValidateNotNull<TableEntityFilterList>(nameof (filterList), filterList);
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryOnReindexingStatusTable");
        this.BindInt("@count", count);
        TableEntityFilter propertyFilter;
        if (!filterList.TryRetrieveFilter("Status", out propertyFilter))
          throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentNullException(nameof (filterList)));
        this.BindByte("@status", (byte) (ReindexingStatus) Enum.Parse(typeof (ReindexingStatus), propertyFilter.Value));
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ReindexingStatusEntry>((ObjectBinder<ReindexingStatusEntry>) new ReindexingStatusTable.Columns(this.m_entityTypes));
          ObjectBinder<ReindexingStatusEntry> current = resultCollection.GetCurrent<ReindexingStatusEntry>();
          return current?.Items != null && current.Items.Count > 0 ? current.Items : new List<ReindexingStatusEntry>();
        }
      }
      catch (TableAccessException ex)
      {
        ExceptionDispatchInfo.Capture((Exception) ex).Throw();
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve ReindexingStatusSQLTableEntry list with SQL Azure Platform");
      }
      return (List<ReindexingStatusEntry>) null;
    }

    public override ReindexingStatusEntry RetriveTableEntity(TableEntityFilterList filterList)
    {
      this.ValidateNotNull<TableEntityFilterList>(nameof (filterList), filterList);
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryOnReindexingStatusTable");
        TableEntityFilter propertyFilter;
        if (!filterList.TryRetrieveFilter("CollectionId", out propertyFilter))
          throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentNullException(nameof (filterList)));
        this.BindGuid("@collectionId", new Guid(propertyFilter.Value));
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ReindexingStatusEntry>((ObjectBinder<ReindexingStatusEntry>) new ReindexingStatusTable.Columns(this.m_entityTypes));
          ObjectBinder<ReindexingStatusEntry> current = resultCollection.GetCurrent<ReindexingStatusEntry>();
          if (current?.Items == null || current.Items.Count <= 0)
            return (ReindexingStatusEntry) null;
          if (current.Items.Count == 1)
            return resultCollection.GetCurrent<ReindexingStatusEntry>().Items[0];
          throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, message: "More than one matching ReindexingStatus found for the input filter list");
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve ReindexingStatus with SQL Azure Platform");
      }
    }

    public virtual bool DeleteTableEntity(Guid collectionId, IEntityType entityType)
    {
      Stopwatch stopwatch = new Stopwatch();
      try
      {
        this.PrepareStoredProcedure("Search.prc_DeleteReindexingStatusEntry");
        this.BindGuid("@collectionId", collectionId);
        stopwatch.Start();
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, FormattableString.Invariant(FormattableStringFactory.Create("Failed to Delete Reindexing Status entry for Collection ID: {0}.", (object) collectionId)));
      }
      return true;
    }

    public virtual ReindexingStatusEntry GetReindexingStatusEntry(
      Guid collectionId,
      IEntityType entityType)
    {
      throw new NotImplementedException();
    }

    public virtual List<ReindexingStatusEntry> AddOrUpdateReindexingStatusEntries(
      List<ReindexingStatusEntry> reindexingStatusEntries)
    {
      throw new NotImplementedException();
    }

    protected class Columns : ObjectBinder<ReindexingStatusEntry>
    {
      public SqlColumnBinder CollectionId = new SqlColumnBinder(nameof (CollectionId));
      public SqlColumnBinder EntityType = new SqlColumnBinder(nameof (EntityType));
      public SqlColumnBinder Status = new SqlColumnBinder(nameof (Status));
      public SqlColumnBinder LastUpdatedTimeStamp = new SqlColumnBinder(nameof (LastUpdatedTimeStamp));
      public IVssRequestContext RequestContext;
      private IEnumerable<IEntityType> m_entityTypes;

      protected override ReindexingStatusEntry Bind() => new ReindexingStatusEntry(this.CollectionId.GetGuid((IDataReader) this.Reader), EntityPluginsFactory.GetEntityType(this.m_entityTypes, (int) this.EntityType.GetByte((IDataReader) this.Reader)))
      {
        Status = (ReindexingStatus) this.Status.GetByte((IDataReader) this.Reader),
        LastUpdatedTimeStamp = this.LastUpdatedTimeStamp.GetDateTime((IDataReader) this.Reader)
      };

      public Columns(IEnumerable<IEntityType> entityTypes) => this.m_entityTypes = entityTypes;
    }

    protected class ReindexingStatusColumns : ReindexingStatusTable.Columns
    {
      public SqlColumnBinder Priority = new SqlColumnBinder(nameof (Priority));

      protected override ReindexingStatusEntry Bind()
      {
        ReindexingStatusEntry reindexingStatusEntry = base.Bind();
        reindexingStatusEntry.Priority = this.Priority.GetInt16((IDataReader) this.Reader);
        return reindexingStatusEntry;
      }

      public ReindexingStatusColumns(IEnumerable<IEntityType> m_entityTypes)
        : base(m_entityTypes)
      {
      }
    }
  }
}
