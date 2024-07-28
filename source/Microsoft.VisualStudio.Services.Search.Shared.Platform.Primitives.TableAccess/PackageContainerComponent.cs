// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.PackageContainerComponent
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  internal class PackageContainerComponent : SQLTable<PackageContainer>
  {
    private const string ServiceName = "Search_PackageContainer";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<PackageContainerComponent>(1, true)
    }, "Search_PackageContainer");
    private static readonly SqlMetaData[] s_packageContainerLookupTable = new SqlMetaData[6]
    {
      new SqlMetaData("ContainerId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ContainerName", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("ContainerType", SqlDbType.Int),
      new SqlMetaData("IsDeleted", SqlDbType.Bit),
      new SqlMetaData("Token", SqlDbType.VarChar, 512L),
      new SqlMetaData("SecurityHashCode", SqlDbType.Binary, 32L)
    };
    private static readonly SqlMetaData[] s_guidTable = new SqlMetaData[1]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier)
    };

    public PackageContainerComponent()
      : base()
    {
    }

    internal PackageContainerComponent(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public override List<PackageContainer> AddTableEntityBatch(
      List<PackageContainer> containerList,
      bool merge)
    {
      this.ValidateNotNullOrEmptyList<PackageContainer>(nameof (containerList), (IList<PackageContainer>) containerList);
      this.PrepareStoredProcedure("Search.prc_AddOrUpdatePackageContainers");
      this.BindPackageContainerLookupTable("@itemList", (IEnumerable<PackageContainer>) containerList);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PackageContainer>((ObjectBinder<PackageContainer>) new PackageContainerComponent.PackageContainerColumns());
        return resultCollection.GetCurrent<PackageContainer>().Items;
      }
    }

    public override List<PackageContainer> RetriveTableEntityList(
      int count,
      TableEntityFilterList filterList)
    {
      this.ValidateNotNull<TableEntityFilterList>(nameof (filterList), filterList);
      this.PrepareStoredProcedure("Search.prc_QueryPackageContainers");
      this.BindInt("@count", count);
      TableEntityFilter propertyFilter;
      if (filterList.TryRetrieveFilter("ContainerType", out propertyFilter))
        this.BindInt("@containerType", Convert.ToInt32(propertyFilter.Value));
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PackageContainer>((ObjectBinder<PackageContainer>) new PackageContainerComponent.PackageContainerColumns());
        ObjectBinder<PackageContainer> current = resultCollection.GetCurrent<PackageContainer>();
        return current?.Items != null && current.Items.Count > 0 ? current.Items : new List<PackageContainer>();
      }
    }

    public virtual List<PackageContainer> GetPackageContainers(List<Guid> containerIds)
    {
      this.ValidateNotNullOrEmptyList<Guid>(nameof (containerIds), (IList<Guid>) containerIds);
      this.PrepareStoredProcedure("Search.prc_QueryPackageContainersById");
      this.BindPackageContainerIdTable("@containerIdList", (IEnumerable<Guid>) containerIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PackageContainer>((ObjectBinder<PackageContainer>) new PackageContainerComponent.PackageContainerColumns());
        ObjectBinder<PackageContainer> current = resultCollection.GetCurrent<PackageContainer>();
        return current?.Items != null && current.Items.Count > 0 ? current.Items : new List<PackageContainer>();
      }
    }

    public virtual PackageContainer GetPackageContainer(Guid containerId)
    {
      List<PackageContainer> packageContainers = this.GetPackageContainers(new List<Guid>()
      {
        containerId
      });
      return packageContainers != null && packageContainers.Count > 0 ? packageContainers[0] : (PackageContainer) null;
    }

    public virtual int DeletePackageContainers(List<Guid> packageContainerIds)
    {
      this.ValidateNotNullOrEmptyList<Guid>(nameof (packageContainerIds), (IList<Guid>) packageContainerIds);
      this.PrepareStoredProcedure("Search.prc_DeletePackageContainers");
      this.BindPackageContainerIdTable("@containerIdList", (IEnumerable<Guid>) packageContainerIds);
      return this.ExecuteNonQuery();
    }

    protected SqlParameter BindPackageContainerLookupTable(
      string parameterName,
      IEnumerable<PackageContainer> rows)
    {
      rows = rows ?? Enumerable.Empty<PackageContainer>();
      System.Func<PackageContainer, SqlDataRecord> selector = (System.Func<PackageContainer, SqlDataRecord>) (container =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(PackageContainerComponent.s_packageContainerLookupTable);
        sqlDataRecord.SetGuid(0, container.ContainerId);
        sqlDataRecord.SetString(1, container.Name);
        sqlDataRecord.SetInt32(2, (int) container.Type);
        sqlDataRecord.SetBoolean(3, container.IsDeleted);
        if (!string.IsNullOrEmpty(container.Token))
          sqlDataRecord.SetString(4, container.Token);
        else
          sqlDataRecord.SetDBNull(4);
        if (container.SecurityHashCode != null)
          sqlDataRecord.SetBytes(5, 0L, container.SecurityHashCode, 0, 20);
        else
          sqlDataRecord.SetDBNull(5);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Search.typ_PackageContainerDescriptor", rows.Select<PackageContainer, SqlDataRecord>(selector));
    }

    protected SqlParameter BindPackageContainerIdTable(
      string parameterName,
      IEnumerable<Guid> containerIds)
    {
      containerIds = containerIds ?? Enumerable.Empty<Guid>();
      System.Func<Guid, SqlDataRecord> selector = (System.Func<Guid, SqlDataRecord>) (containerId =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(PackageContainerComponent.s_guidTable);
        sqlDataRecord.SetGuid(0, containerId);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_GuidTable", containerIds.Select<Guid, SqlDataRecord>(selector));
    }

    protected class PackageContainerColumns : ObjectBinder<PackageContainer>
    {
      private SqlColumnBinder m_containerId = new SqlColumnBinder("ContainerId");
      private SqlColumnBinder m_containerName = new SqlColumnBinder("ContainerName");
      private SqlColumnBinder m_containerType = new SqlColumnBinder("ContainerType");
      private SqlColumnBinder m_isDeleted = new SqlColumnBinder("IsDeleted");
      private SqlColumnBinder m_token = new SqlColumnBinder("Token");
      private SqlColumnBinder m_securityHashcode = new SqlColumnBinder("SecurityHashCode");

      protected override PackageContainer Bind()
      {
        if (this.m_containerId.IsNull((IDataReader) this.Reader))
          return (PackageContainer) null;
        return new PackageContainer()
        {
          ContainerId = this.m_containerId.GetGuid((IDataReader) this.Reader),
          Name = this.m_containerName.GetString((IDataReader) this.Reader, false),
          Type = (PackageContainerType) this.m_containerType.GetInt32((IDataReader) this.Reader),
          IsDeleted = this.m_isDeleted.GetBoolean((IDataReader) this.Reader),
          Token = this.m_token.GetString((IDataReader) this.Reader, true),
          SecurityHashCode = this.m_securityHashcode.GetBytes((IDataReader) this.Reader, true)
        };
      }
    }
  }
}
