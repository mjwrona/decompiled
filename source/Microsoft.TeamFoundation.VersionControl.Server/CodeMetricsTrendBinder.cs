// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CodeMetricsTrendBinder
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CodeMetricsTrendBinder : VersionControlObjectBinder<ChangesetsTrendItem>
  {
    private SqlColumnBinder timeBucket = new SqlColumnBinder("TimeBucket");
    private SqlColumnBinder metricCount = new SqlColumnBinder("MetricValue");

    protected override ChangesetsTrendItem Bind() => new ChangesetsTrendItem(this.timeBucket.GetInt32((IDataReader) this.Reader), this.metricCount.GetInt32((IDataReader) this.Reader));
  }
}
