// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Transform.TransformComponent19
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.Transform
{
  internal class TransformComponent19 : TransformComponent11
  {
    public override CleanupDeletedTableResult CleanupDeletedTable(
      string tableName,
      bool continueToNextTable,
      int retainHistoryDays)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_CleanupDeletedTable");
      this.BindString("@tableName", tableName, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindBoolean("@continueToNextTable", continueToNextTable);
      this.BindInt("@retainHistoryDays", retainHistoryDays);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<CleanupDeletedTableResult>((ObjectBinder<CleanupDeletedTableResult>) new CleanupDeletedTableColumns());
        return resultCollection.GetCurrent<CleanupDeletedTableResult>().Items.Single<CleanupDeletedTableResult>();
      }
    }
  }
}
