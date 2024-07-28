// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseMaintenanceComponent2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabaseMaintenanceComponent2 : DatabaseMaintenanceComponent
  {
    public override void OptimizeDatabase(
      TimeSpan timeout,
      bool checkLogSize,
      bool changeCompression)
    {
      this.CheckTimeout(timeout);
      this.PrepareStoredProcedure("prc_OptimizeDatabase", (int) timeout.TotalSeconds);
      this.BindBoolean("@checkLogSize", checkLogSize);
      this.BindBoolean("@changeCompression", changeCompression);
      this.ExecuteNonQuery();
    }

    public override void OptimizeDatabase2(
      TimeSpan timeout,
      bool checkLogSize,
      bool changeCompression,
      bool reorganizeOnly)
    {
      this.CheckTimeout(timeout);
      this.PrepareStoredProcedure("prc_OptimizeDatabase2", (int) timeout.TotalSeconds);
      this.BindBoolean("@checkLogSize", checkLogSize);
      this.BindBoolean("@changeCompression", changeCompression);
      this.BindBoolean("@reorganizeOnly", reorganizeOnly);
      this.ExecuteNonQuery();
    }
  }
}
