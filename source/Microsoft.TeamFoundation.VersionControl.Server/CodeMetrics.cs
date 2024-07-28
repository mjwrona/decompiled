// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CodeMetrics
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class CodeMetrics
  {
    public CodeMetrics(
      int changesetsCount,
      int authorsCount,
      List<ChangesetsTrendItem> changesetsTrend)
    {
      this.Changesets = changesetsCount;
      this.Authors = authorsCount;
      this.ChangesetsTrend = changesetsTrend;
    }

    public int Changesets { get; set; }

    public int Authors { get; set; }

    public List<ChangesetsTrendItem> ChangesetsTrend { get; }
  }
}
