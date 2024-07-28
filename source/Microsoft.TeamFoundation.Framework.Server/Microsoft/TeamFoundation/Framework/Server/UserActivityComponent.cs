// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UserActivityComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class UserActivityComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<UserActivityComponent>(1)
    }, "UserActivity");
    private static readonly string s_area = nameof (UserActivityComponent);

    public UserActivityComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    protected override string TraceArea => UserActivityComponent.s_area;

    public virtual void UpdateUserActivity(DateTime startTime, DateTime endTime)
    {
      this.PrepareStoredProcedure("prc_UpdateUserActivity");
      this.BindDateTime("@startTime", startTime);
      this.BindDateTime("@endTime", endTime);
      this.ExecuteNonQuery();
    }

    public virtual void PruneUserActivity(int maxAgeDays)
    {
      this.PrepareStoredProcedure("prc_PruneUserActivity");
      this.BindInt("@maxAgeDays", maxAgeDays);
      this.ExecuteNonQuery();
    }

    public virtual List<UserActivityEntry> QueryUserActivity(int lastDays)
    {
      this.PrepareStoredProcedure("prc_QueryUserActivity");
      this.BindInt("@lastDays", lastDays);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<UserActivityEntry>((ObjectBinder<UserActivityEntry>) this.GetUserActivityBinder());
        return resultCollection.GetCurrent<UserActivityEntry>().Items;
      }
    }

    protected virtual UserActivityBinder GetUserActivityBinder() => new UserActivityBinder(this.RequestContext);
  }
}
