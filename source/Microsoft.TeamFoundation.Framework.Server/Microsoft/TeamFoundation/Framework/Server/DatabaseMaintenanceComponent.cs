// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseMaintenanceComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabaseMaintenanceComponent : TeamFoundationSqlResourceComponent
  {
    protected const double ReorganizeIndexesSoftTimeoutPercent = 0.8;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<DatabaseMaintenanceComponent>(1),
      (IComponentCreator) new ComponentCreator<DatabaseMaintenanceComponent2>(2),
      (IComponentCreator) new ComponentCreator<DatabaseMaintenanceComponent3>(3)
    }, "DatabaseMaintenance");

    public virtual void OptimizeDatabase(
      TimeSpan timeout,
      bool checkLogSize,
      bool changeCompression)
    {
      this.CheckTimeout(timeout);
      this.PrepareStoredProcedure("prc_OptimizeDatabase", (int) timeout.TotalSeconds);
      this.ExecuteNonQuery();
    }

    public virtual void OptimizeDatabase2(
      TimeSpan timeout,
      bool checkLogSize,
      bool changeCompression,
      bool reorganizeOnly)
    {
      this.OptimizeDatabase(timeout, checkLogSize, changeCompression);
    }

    public virtual void ReorganizeIndexes(
      TimeSpan timeout,
      double fragmentationThreshold,
      bool checkLogSize = true)
    {
    }

    protected void CheckTimeout(TimeSpan timeout)
    {
      if (timeout.TotalSeconds < 0.0 || timeout.TotalSeconds > (double) int.MaxValue)
        throw new ArgumentOutOfRangeException(nameof (timeout));
    }
  }
}
