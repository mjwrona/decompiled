// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.UpdateStatisticsComponent2
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class UpdateStatisticsComponent2 : UpdateStatisticsComponent
  {
    public override int UpdateStatistics(int rowCountThreshold)
    {
      this.PrepareStoredProcedure("prc_UpdateStatistics", 14400);
      this.BindInt("@rowCountThreshold", rowCountThreshold);
      return (int) this.ExecuteScalar();
    }
  }
}
