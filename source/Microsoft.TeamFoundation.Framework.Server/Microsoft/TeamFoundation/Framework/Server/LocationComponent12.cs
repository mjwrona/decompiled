// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LocationComponent12
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LocationComponent12 : LocationComponent11
  {
    public override void ConfigureAccessMapping(
      AccessMapping accessMapping,
      bool makeDefault,
      bool allowOverlapping)
    {
      this.PrepareStoredProcedure("prc_ConfigureAccessMapping");
      this.BindServiceHostId(true);
      this.BindString("@moniker", accessMapping.Moniker, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@displayName", accessMapping.DisplayName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@accessPoint", accessMapping.AccessPoint, -1, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindBoolean("@makeDefault", makeDefault);
      this.BindBoolean("@allowOverlapping", allowOverlapping);
      this.BindGuid("@writerIdentifier", this.Author);
      this.ExecuteNonQuery();
    }
  }
}
