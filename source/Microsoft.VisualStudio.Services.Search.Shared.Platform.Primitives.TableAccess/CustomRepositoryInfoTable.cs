// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.CustomRepositoryInfoTable
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class CustomRepositoryInfoTable : SQLTable<CustomRepositoryEntity>
  {
    private const string ServiceName = "Search_CustomRepositoryInfo";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<CustomRepositoryInfoTable>(1, true),
      (IComponentCreator) new ComponentCreator<CustomRepositoryInfoTableV2>(2, true)
    }, "Search_CustomRepositoryInfo");
    private static readonly SqlMetaData[] s_customRepositorySqlTableEntityLookupTable = new SqlMetaData[4]
    {
      new SqlMetaData("CollectionId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ProjectName", SqlDbType.VarChar, 70L),
      new SqlMetaData("RepositoryName", SqlDbType.VarChar, 500L),
      new SqlMetaData("RepositoryId", SqlDbType.UniqueIdentifier)
    };

    public CustomRepositoryInfoTable()
      : base()
    {
    }

    internal CustomRepositoryInfoTable(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public override CustomRepositoryEntity Insert(CustomRepositoryEntity entity)
    {
      this.ValidateNotNull<CustomRepositoryEntity>(nameof (entity), entity);
      this.ValidateNotEmpty("entity.CollectionId", entity.CollectionId);
      this.ValidateNotNull<string>("entity.ProjectName", entity.ProjectName);
      this.ValidateNotNull<string>("entity.RepositoryName", entity.RepositoryName);
      this.ValidateNotEmpty("entity.RepositoryId", entity.RepositoryId);
      try
      {
        this.PrepareStoredProcedure("Search.prc_AddEntryForCustomRepositoryInfoTable");
        IList<CustomRepositoryEntity> rows = (IList<CustomRepositoryEntity>) new List<CustomRepositoryEntity>();
        rows.Add(entity);
        this.BindRepositoryEntityLookupTable("@itemList", (IEnumerable<CustomRepositoryEntity>) rows);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<CustomRepositoryEntity>((ObjectBinder<CustomRepositoryEntity>) new CustomRepositoryInfoTable.CustomRepositoryInfoTableColumns());
          ObjectBinder<CustomRepositoryEntity> current = resultCollection.GetCurrent<CustomRepositoryEntity>();
          if (current != null && current.Items != null && current.Items.Count == 1)
            return resultCollection.GetCurrent<CustomRepositoryEntity>().Items[0];
          throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, message: string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to Add entity with Repository name {0}, Collection ID {1} with SQL Azure platform", (object) entity.RepositoryName, (object) entity.CollectionId));
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to Add entity with Repository name {0}, Collection ID {1} with SQL Azure platform", (object) entity.RepositoryName, (object) entity.CollectionId));
      }
    }

    public override CustomRepositoryEntity Update(CustomRepositoryEntity entity)
    {
      this.ValidateNotNull<CustomRepositoryEntity>(nameof (entity), entity);
      this.ValidateNotEmpty("entity.CollectionId", entity.CollectionId);
      this.ValidateNotNull<string>("entity.ProjectName", entity.ProjectName);
      this.ValidateNotNull<string>("entity.RepositoryName", entity.RepositoryName);
      this.ValidateNotEmpty("entity.RepositoryId", entity.RepositoryId);
      try
      {
        this.PrepareStoredProcedure("Search.prc_UpdateEntryForCustomRepositoryInfoTable");
        IList<CustomRepositoryEntity> rows = (IList<CustomRepositoryEntity>) new List<CustomRepositoryEntity>();
        rows.Add(entity);
        this.BindRepositoryEntityLookupTable("@itemList", (IEnumerable<CustomRepositoryEntity>) rows);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<CustomRepositoryEntity>((ObjectBinder<CustomRepositoryEntity>) new CustomRepositoryInfoTable.CustomRepositoryInfoTableColumns());
          ObjectBinder<CustomRepositoryEntity> current = resultCollection.GetCurrent<CustomRepositoryEntity>();
          if (current != null && current.Items != null && current.Items.Count == 1)
            return resultCollection.GetCurrent<CustomRepositoryEntity>().Items[0];
          throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, message: string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to Update entity with Repository name {0}, Collection ID {1} with SQL Azure platform", (object) entity.RepositoryName, (object) entity.CollectionId));
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to Update entity with Repository name {0}, Collection ID {1} with SQL Azure platform", (object) entity.RepositoryName, (object) entity.CollectionId));
      }
    }

    public override void Delete(CustomRepositoryEntity entity)
    {
      this.ValidateNotNull<CustomRepositoryEntity>(nameof (entity), entity);
      try
      {
        this.PrepareStoredProcedure("Search.prc_RemoveRepositoriesFromCustomRepositoryInfoTable");
        this.BindGuid("@collectionId", entity.CollectionId);
        this.BindGuid("@repositoryId", entity.RepositoryId);
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to Delete entity with Repository Id {0}, Collection ID {1} with SQL Azure platform", (object) entity.RepositoryId, (object) entity.CollectionId));
      }
    }

    public override List<CustomRepositoryEntity> RetriveTableEntityList(
      int count,
      TableEntityFilterList filterList)
    {
      this.ValidateNotNullOrEmptyList<TableEntityFilter>(nameof (filterList), (IList<TableEntityFilter>) filterList);
      try
      {
        this.PrepareStoredProcedure("Search.prc_QueryOnCustomRepositoryInfoTable");
        this.BindInt("@count", count);
        this.BindGuid("@collectionId", new Guid(TableEntityHelper.RetrieveFilterValue(filterList, "CollectionId")));
        if (filterList.Count > 1)
        {
          TableEntityFilter propertyFilter;
          if (filterList.TryRetrieveFilter("ProjectName", out propertyFilter))
            this.BindString("@projectName", TableEntityHelper.RetrieveFilterValue(filterList, "ProjectName"), 70, BindStringBehavior.Unchanged, SqlDbType.VarChar);
          if (filterList.TryRetrieveFilter("RepositoryName", out propertyFilter))
            this.BindString("@repositoryName", TableEntityHelper.RetrieveFilterValue(filterList, "RepositoryName"), 500, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        }
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<CustomRepositoryEntity>((ObjectBinder<CustomRepositoryEntity>) new CustomRepositoryInfoTable.CustomRepositoryInfoTableColumns());
          ObjectBinder<CustomRepositoryEntity> current = resultCollection.GetCurrent<CustomRepositoryEntity>();
          return current != null && current.Items != null && current.Items.Count > 0 ? current.Items : new List<CustomRepositoryEntity>();
        }
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to retrieve Entity List with SQL Azure Platform");
      }
    }

    protected void ValidateNotEmpty(string propertyName, Guid value)
    {
      if (value == Guid.Empty)
        throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentNullException(propertyName));
    }

    private SqlParameter BindRepositoryEntityLookupTable(
      string parameterName,
      IEnumerable<CustomRepositoryEntity> rows)
    {
      rows = rows ?? Enumerable.Empty<CustomRepositoryEntity>();
      System.Func<CustomRepositoryEntity, SqlDataRecord> selector = (System.Func<CustomRepositoryEntity, SqlDataRecord>) (entity =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(CustomRepositoryInfoTable.s_customRepositorySqlTableEntityLookupTable);
        sqlDataRecord.SetGuid(0, entity.CollectionId);
        sqlDataRecord.SetString(1, entity.ProjectName);
        sqlDataRecord.SetString(2, entity.RepositoryName);
        sqlDataRecord.SetGuid(3, entity.RepositoryId);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_CustomRepositoryInfoDescriptor", rows.Select<CustomRepositoryEntity, SqlDataRecord>(selector));
    }

    private class CustomRepositoryInfoTableColumns : ObjectBinder<CustomRepositoryEntity>
    {
      private SqlColumnBinder m_collectionId = new SqlColumnBinder("CollectionId");
      private SqlColumnBinder m_projectName = new SqlColumnBinder("ProjectName");
      private SqlColumnBinder m_repositoryName = new SqlColumnBinder("RepositoryName");
      private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");

      protected override CustomRepositoryEntity Bind()
      {
        if (this.m_repositoryId.IsNull((IDataReader) this.Reader))
          return (CustomRepositoryEntity) null;
        return new CustomRepositoryEntity()
        {
          CollectionId = this.m_collectionId.GetGuid((IDataReader) this.Reader),
          ProjectName = this.m_projectName.GetString((IDataReader) this.Reader, false),
          RepositoryName = this.m_repositoryName.GetString((IDataReader) this.Reader, false),
          RepositoryId = this.m_repositoryId.GetGuid((IDataReader) this.Reader)
        };
      }
    }
  }
}
