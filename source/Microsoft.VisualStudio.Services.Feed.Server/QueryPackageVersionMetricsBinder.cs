// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.QueryPackageVersionMetricsBinder
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class QueryPackageVersionMetricsBinder : ObjectBinder<QueryPackageVersionMetricsRow>
  {
    private SqlColumnBinder packageId = new SqlColumnBinder("PackageId");
    private SqlColumnBinder packageVersionId = new SqlColumnBinder("PackageVersionId");
    private SqlColumnBinder metrics = new SqlColumnBinder("Metrics");

    protected override QueryPackageVersionMetricsRow Bind() => new QueryPackageVersionMetricsRow()
    {
      PackageId = this.packageId.GetGuid((IDataReader) this.Reader, false),
      PackageVersionId = this.packageVersionId.GetGuid((IDataReader) this.Reader, false),
      Metrics = this.metrics.GetString((IDataReader) this.Reader, true)
    };
  }
}
