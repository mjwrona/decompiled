// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DataQualityComponent10
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class DataQualityComponent10 : DataQualityComponent2
  {
    public override CleanupDataQualityResult CleanupDataQualityResults(int retainHistoryDays)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_CleanupDataQualityResults");
      this.BindInt("@retainHistoryDays", retainHistoryDays);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<CleanupDataQualityResult>((ObjectBinder<CleanupDataQualityResult>) new CleanupDataQualityColumns());
        return resultCollection.GetCurrent<CleanupDataQualityResult>().Items.Single<CleanupDataQualityResult>();
      }
    }
  }
}
