// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CodeMetricsBinder
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CodeMetricsBinder : VersionControlObjectBinder<CodeMetrics>
  {
    private SqlColumnBinder authors = new SqlColumnBinder("Authors");
    private SqlColumnBinder changesets = new SqlColumnBinder("Changesets");

    protected override CodeMetrics Bind() => new CodeMetrics(this.changesets.GetInt32((IDataReader) this.Reader), this.authors.GetInt32((IDataReader) this.Reader), (List<ChangesetsTrendItem>) null);
  }
}
