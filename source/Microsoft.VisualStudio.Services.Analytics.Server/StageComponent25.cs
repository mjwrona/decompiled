// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.StageComponent25
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class StageComponent25 : StageComponent23
  {
    public override CleanupStreamResult CleanupStream(
      string table,
      int providerShardId,
      int streamId,
      int commandTimeoutSeconds)
    {
      string streamStoredProcedure = this.GetCleanupStreamStoredProcedure(table);
      if (streamStoredProcedure == null)
        return this.CleanupStreamGeneric(table, providerShardId, streamId, commandTimeoutSeconds);
      this.PrepareStoredProcedure(streamStoredProcedure, commandTimeoutSeconds);
      this.BindString("@tableName", table, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindInt("@providerShardId", providerShardId);
      this.BindInt("@streamId", streamId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<CleanupStreamResult>((ObjectBinder<CleanupStreamResult>) new CleanupStreamColumns());
        return resultCollection.GetCurrent<CleanupStreamResult>().Items.Single<CleanupStreamResult>();
      }
    }
  }
}
