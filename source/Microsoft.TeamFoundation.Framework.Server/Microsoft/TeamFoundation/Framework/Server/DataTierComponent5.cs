// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataTierComponent5
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DataTierComponent5 : DataTierComponent3
  {
    public override void UpdateDataTierConnectionString(DataTierInfo dataTier, string newDataSource)
    {
      string parameterValue1 = SqlConnectionHelper.SanitizeConnectionString(dataTier.ConnectionInfo.ConnectionString);
      string parameterValue2 = SqlConnectionHelper.SanitizeConnectionString(dataTier.ConnectionInfo.CloneReplaceDataSource(newDataSource).ConnectionString);
      this.PrepareStoredProcedure("prc_UpdateDataTierConnectionString");
      this.BindString("@oldConnectionString", parameterValue1, 520, false, SqlDbType.NVarChar);
      this.BindString("@newConnectionString", parameterValue2, 520, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }
  }
}
