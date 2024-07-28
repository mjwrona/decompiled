// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.LockManagerComponent2
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  internal class LockManagerComponent2 : LockManagerComponent
  {
    public LockManagerComponent2()
    {
    }

    internal LockManagerComponent2(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    internal override bool ReleaseLocks(IList<LockDetails> lockingRequirements)
    {
      if (lockingRequirements == null || lockingRequirements.Count == 0)
        throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentException("lockingRequirements is null or empty"));
      LockDetails.ValidateLockingRequirements(lockingRequirements);
      try
      {
        this.PrepareStoredProcedure("Search.prc_ReleaseLocks");
        this.BindLockDetails("@lockList", (IEnumerable<LockDetails>) lockingRequirements);
        SqlParameter sqlParameter = this.BindBoolean("@failed", false);
        sqlParameter.Direction = ParameterDirection.Output;
        this.ExecuteNonQuery();
        return !(sqlParameter.Value is DBNull) && !(bool) sqlParameter.Value;
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to Release Locks");
      }
    }
  }
}
