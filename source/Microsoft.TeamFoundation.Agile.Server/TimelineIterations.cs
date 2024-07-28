// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.TimelineIterations
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Agile.Server
{
  public class TimelineIterations
  {
    public IList<TreeNode> ValidIterations { get; set; }

    public IList<TreeNode> MissingDatesIterations { get; set; }

    public IList<TreeNode> OverlappedIterations { get; set; }
  }
}
