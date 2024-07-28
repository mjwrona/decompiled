// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql.LockManager
// Assembly: Microsoft.VisualStudio.Services.Search.Server.DataAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3B684226-797D-4C9F-9AC1-E10D39E316D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.DataAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql
{
  internal class LockManager : SqlAzureDataAccess, ILockManager
  {
    public LockStatus AcquireLocks(
      IVssRequestContext requestContext,
      IList<LockDetails> lockingRequirements)
    {
      this.ValidateNotNullOrEmptyList<LockDetails>(nameof (lockingRequirements), lockingRequirements);
      using (LockManagerComponent component = requestContext.CreateComponent<LockManagerComponent>())
        return this.InvokeTableOperation<LockStatus>((Func<LockStatus>) (() => component.AcquireLocks(lockingRequirements)));
    }

    public bool ReleaseLocks(IVssRequestContext requestContext, IList<LockDetails> acquiredLocks)
    {
      this.ValidateNotNullOrEmptyList<LockDetails>(nameof (acquiredLocks), acquiredLocks);
      using (LockManagerComponent component = requestContext.CreateComponent<LockManagerComponent>())
        return this.InvokeTableOperation<bool>((Func<bool>) (() => component.ReleaseLocks(acquiredLocks)));
    }

    public void ReleaseLocksWithLeaseId(IVssRequestContext requestContext, string leaseId)
    {
      using (LockManagerComponent component = requestContext.CreateComponent<LockManagerComponent>())
        this.InvokeTableOperation<string>((Func<string>) (() =>
        {
          component.ReleaseLocksWithLeaseId(leaseId);
          return (string) null;
        }));
    }
  }
}
