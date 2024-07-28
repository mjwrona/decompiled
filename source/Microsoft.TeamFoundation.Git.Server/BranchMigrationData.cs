// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.BranchMigrationData
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class BranchMigrationData
  {
    internal BranchMigrationData(string source, string target, string[] parents)
    {
      this.SourceName = this.PrependRefsHeadsIfNeeded(source);
      this.TargetName = this.PrependRefsHeadsIfNeeded(target);
      this.ParentNames = ((IEnumerable<string>) parents).Select<string, string>((Func<string, string>) (x => this.PrependRefsHeadsIfNeeded(x))).ToArray<string>();
    }

    private string PrependRefsHeadsIfNeeded(string refName) => !string.IsNullOrEmpty(refName) && !refName.StartsWith("refs/heads/") ? "refs/heads/" + refName : refName;

    internal string SourceName { get; }

    internal string TargetName { get; }

    internal string[] ParentNames { get; }
  }
}
