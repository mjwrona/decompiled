// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.LockManagerComponent
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

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class LockManagerComponent : SQLTable<IndexingUnitChangeEvent>
  {
    private const string ServiceName = "Search_ResourceLockManager";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<LockManagerComponent>(1, true),
      (IComponentCreator) new ComponentCreator<LockManagerComponent2>(2)
    }, "Search_ResourceLockManager");
    private static readonly SqlMetaData[] s_lockDetailsType = new SqlMetaData[3]
    {
      new SqlMetaData("ResourceName", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("LockMode", SqlDbType.TinyInt),
      new SqlMetaData("LockOwner", SqlDbType.NVarChar, 128L)
    };
    private static readonly SqlMetaData[] s_typStringTable = new SqlMetaData[1]
    {
      new SqlMetaData("Data", SqlDbType.NVarChar, 1024L)
    };

    public LockManagerComponent()
      : base()
    {
      this.SelectedFeatures = SqlResourceComponentFeatures.None;
    }

    internal LockManagerComponent(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    internal virtual LockStatus AcquireLocks(IList<LockDetails> lockingRequirements)
    {
      if (lockingRequirements == null || lockingRequirements.Count == 0)
        throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentException("lockingRequirements is null or empty"));
      LockDetails.ValidateLockingRequirements(lockingRequirements);
      try
      {
        SqlCommand sqlCommand = this.PrepareStoredProcedure("Search.prc_AcquireLocks");
        this.BindLockDetails("@lockList", (IEnumerable<LockDetails>) lockingRequirements);
        SqlParameter sqlParameter1 = new SqlParameter("@acquired", SqlDbType.Bit);
        sqlParameter1.Direction = ParameterDirection.Output;
        sqlCommand.Parameters.Add(sqlParameter1);
        SqlParameter sqlParameter2 = new SqlParameter("@leaseId", SqlDbType.NVarChar, (int) byte.MaxValue);
        sqlParameter2.Direction = ParameterDirection.Output;
        sqlCommand.Parameters.Add(sqlParameter2);
        this.ExecuteNonQuery();
        return new LockStatus((bool) sqlParameter1.Value, sqlParameter2.Value is DBNull ? string.Empty : (string) sqlParameter2.Value);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to Acquire Locks");
      }
    }

    internal virtual bool ReleaseLocks(IList<LockDetails> lockingRequirements)
    {
      if (lockingRequirements == null || lockingRequirements.Count == 0)
        throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentException("lockingRequirements is null or empty"));
      LockDetails.ValidateLockingRequirements(lockingRequirements);
      try
      {
        this.PrepareStoredProcedure("Search.prc_ReleaseLocks");
        this.BindLockDetails("@lockList", (IEnumerable<LockDetails>) lockingRequirements);
        return (int) this.ExecuteNonQuery(true) == 0;
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to Release Locks");
      }
    }

    public virtual void ReleaseLocksWithLeaseId(string leaseId)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_ReleaseAllLocksWithLeaseId");
        this.BindString("@leaseId", leaseId, (int) byte.MaxValue, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to Release Locks with lease Id : " + leaseId));
      }
    }

    public virtual void ReleaseLocksWithLeaseIds(IEnumerable<string> leaseIds)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_ReleaseLocksWithLeaseIds");
        System.Func<string, SqlDataRecord> selector = (System.Func<string, SqlDataRecord>) (row =>
        {
          SqlDataRecord record = new SqlDataRecord(LockManagerComponent.s_typStringTable);
          record.SetNullableString(0, row);
          return record;
        });
        this.BindTable("@leaseIds", "typ_StringTable", leaseIds.Select<string, SqlDataRecord>(selector));
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to Release Locks with lease Id : (" + string.Join(",", leaseIds) + ")"));
      }
    }

    internal void BindLockDetails(string parameterName, IEnumerable<LockDetails> rows)
    {
      rows = rows ?? Enumerable.Empty<LockDetails>();
      System.Func<LockDetails, SqlDataRecord> selector = (System.Func<LockDetails, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(LockManagerComponent.s_lockDetailsType);
        sqlDataRecord.SetString(0, row.ResourceName);
        sqlDataRecord.SetByte(1, (byte) row.LockMode);
        sqlDataRecord.SetString(2, row.LockOwner);
        return sqlDataRecord;
      });
      this.BindTable(parameterName, "Search.typ_LockDetails", rows.Select<LockDetails, SqlDataRecord>(selector));
    }
  }
}
