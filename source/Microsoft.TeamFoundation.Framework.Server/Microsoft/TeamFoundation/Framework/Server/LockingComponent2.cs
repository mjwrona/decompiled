// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LockingComponent2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LockingComponent2 : LockingComponent
  {
    private static readonly string s_area = "Locking";
    private static readonly string s_layer = nameof (LockingComponent2);

    public override bool AcquireLocks(
      TeamFoundationLockMode lockMode,
      int lockTimeout,
      string[] resources,
      out string timedoutLockName)
    {
      resources = this.AppendPartitionIds(resources);
      this.PrepareStoredProcedure("prc_AcquireLocks", lockTimeout != -1 ? Math.Max(lockTimeout, this.CommandTimeout) : 0);
      this.BindString("@lockMode", lockMode.ToString(), 32, false, SqlDbType.NVarChar);
      this.BindInt("@lockTimeout", lockTimeout);
      this.BindOrderedStringTable("@resources", (IEnumerable<string>) resources);
      SqlDataReader reader = this.ExecuteReader();
      if (!reader.Read())
        throw new UnexpectedDatabaseResultException(this.ProcedureName);
      LockingComponent2.AcquireLocksColumns acquireLocksColumns = new LockingComponent2.AcquireLocksColumns();
      int int32 = acquireLocksColumns.StatusColumn.GetInt32((IDataReader) reader);
      timedoutLockName = acquireLocksColumns.ResourceColumn.GetString((IDataReader) reader, true);
      this.ReleaseVerificationLock();
      if (int32 >= 0)
        this.IncrementLocksAcquiredCount(resources.Length);
      return int32 >= 0;
    }

    public override void ReleaseLocks(string[] resources)
    {
      try
      {
        resources = this.AppendPartitionIds(resources);
        this.PrepareStoredProcedure("prc_ReleaseLocks");
        this.BindOrderedStringTable("@resources", (IEnumerable<string>) resources);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(98002, LockingComponent2.s_area, LockingComponent2.s_layer, ex);
        throw;
      }
      finally
      {
        this.DecrementLocksAcquiredCount(resources.Length);
      }
    }

    private string[] AppendPartitionIds(string[] resources)
    {
      string[] strArray = new string[resources.Length];
      for (int index = 0; index < resources.Length; ++index)
        strArray[index] = this.AppendPartitionId(resources[index]);
      return strArray;
    }

    internal class AcquireLocksColumns
    {
      public SqlColumnBinder StatusColumn = new SqlColumnBinder("Status");
      public SqlColumnBinder LockIdColumn = new SqlColumnBinder("LockId");
      public SqlColumnBinder ResourceColumn = new SqlColumnBinder("Resource");
    }
  }
}
