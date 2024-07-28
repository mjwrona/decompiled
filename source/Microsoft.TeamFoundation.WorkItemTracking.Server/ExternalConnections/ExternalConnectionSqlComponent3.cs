// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.ExternalConnectionSqlComponent3
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  internal class ExternalConnectionSqlComponent3 : ExternalConnectionSqlComponent2
  {
    public override ExternalConnectionTelemetryData CollectectExternalConnectionTelemetry(
      int dataCollectionTimeFrameInDays)
    {
      this.PrepareStoredProcedure("prc_CollectExternalConnectionTelemetry");
      this.BindInt("@dataCollectionTimeFrameInDays", dataCollectionTimeFrameInDays);
      return this.ExecuteUnknown<ExternalConnectionTelemetryData>((System.Func<IDataReader, ExternalConnectionTelemetryData>) (reader => reader.Read() ? this.GetTelemetryDataBinder().Bind(reader) : (ExternalConnectionTelemetryData) null));
    }

    protected virtual ExternalConnectionSqlComponent2.TelemetryDataBinder GetTelemetryDataBinder() => new ExternalConnectionSqlComponent2.TelemetryDataBinder();
  }
}
