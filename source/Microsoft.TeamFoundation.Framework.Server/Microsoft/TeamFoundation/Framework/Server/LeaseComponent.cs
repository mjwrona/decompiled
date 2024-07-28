// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LeaseComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LeaseComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<LeaseComponent>(1, false),
      (IComponentCreator) new ComponentCreator<LeaseComponent2>(2, false)
    }, "Lease");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static LeaseComponent() => LeaseComponent.s_sqlExceptionFactories.Add(800304, new SqlExceptionFactory(typeof (LeaseLostException)));

    public LeaseComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual bool AcquireLease(
      string leaseName,
      TimeSpan leaseTime,
      Guid processId,
      Guid leaseOwner)
    {
      this.PrepareStoredProcedure("prc_AcquireLease");
      this.BindString("@leaseName", leaseName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("@expirySeconds", (int) leaseTime.TotalSeconds);
      this.BindGuid("@processId", processId);
      this.BindGuid("@leaseOwner", leaseOwner);
      return (bool) this.ExecuteScalar();
    }

    public virtual void ReleaseLease(string leaseName, Guid leaseOwner)
    {
      this.PrepareStoredProcedure("prc_ReleaseLease");
      this.BindString("@leaseName", leaseName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("@leaseOwner", leaseOwner);
      this.ExecuteScalar();
    }

    public virtual void RenewLease(string leaseName, Guid leaseOwner, TimeSpan leaseTime) => throw new LeaseLostException(leaseName, leaseOwner);

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) LeaseComponent.s_sqlExceptionFactories;
  }
}
