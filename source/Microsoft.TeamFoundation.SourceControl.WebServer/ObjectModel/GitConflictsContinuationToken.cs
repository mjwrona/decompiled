// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.ObjectModel.GitConflictsContinuationToken
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.SourceControl.WebServer.ObjectModel
{
  public class GitConflictsContinuationToken
  {
    public GitConflictsContinuationToken(int conflictId) => this.ConflictId = conflictId;

    public GitConflictsContinuationToken(IEnumerable<GitConflict> conflicts)
    {
      conflicts = conflicts ?? throw new ArgumentNullException(nameof (conflicts));
      if (!conflicts.Any<GitConflict>())
        return;
      this.ConflictId = conflicts.Select<GitConflict, int>((Func<GitConflict, int>) (x => x.ConflictId)).Max() + 1;
    }

    public int ConflictId { get; private set; }

    public override string ToString() => this.ConflictId.ToString();

    public static bool TryParse(string value, out GitConflictsContinuationToken token)
    {
      token = (GitConflictsContinuationToken) null;
      int result;
      if (string.IsNullOrEmpty(value) || !int.TryParse(value, out result))
        return false;
      token = new GitConflictsContinuationToken(result);
      return true;
    }
  }
}
