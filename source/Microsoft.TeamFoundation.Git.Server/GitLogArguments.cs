// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitLogArguments
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitLogArguments
  {
    public GitLogArguments()
    {
      this.Order = CommitOrder.DateOrder;
      this.Path = (string) null;
      this.HistoryMode = GitLogHistoryMode.Simplified;
      this.StopAtAdds = false;
      this.FromDate = new DateTime?();
      this.ToDate = new DateTime?();
      this.Author = (string) null;
      this.Committer = (string) null;
      this.Skip = new int?();
      this.MaxCount = new int?();
    }

    public CommitOrder Order { get; set; }

    public string Path { get; set; }

    public GitLogHistoryMode HistoryMode { get; set; }

    public bool RewriteParents { get; set; }

    public bool StopAtAdds { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public string Author { get; set; }

    public string Committer { get; set; }

    public int? Skip { get; set; }

    public int? MaxCount { get; set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.Order == CommitOrder.TopoOrder)
      {
        if (this.RewriteParents)
          stringBuilder.Append("--graph ");
        else
          stringBuilder.Append("--topo-order ");
      }
      else
      {
        stringBuilder.Append("--date-order ");
        if (this.RewriteParents)
          stringBuilder.Append("--parents ");
      }
      switch (this.HistoryMode)
      {
        case GitLogHistoryMode.FirstParent:
          stringBuilder.Append("--first-parent ");
          break;
        case GitLogHistoryMode.FullHistory:
          stringBuilder.Append("--full-history ");
          break;
        case GitLogHistoryMode.FullHistorySimplifyMerges:
          stringBuilder.Append("--full-history --simplify-merges ");
          break;
      }
      if (this.StopAtAdds)
        stringBuilder.Append("--remove-empty ");
      if (this.FromDate.HasValue)
        stringBuilder.Append("--since=" + this.MakeSafe(this.FromDate.ToString()) + " ");
      if (this.ToDate.HasValue)
        stringBuilder.Append("--until=" + this.MakeSafe(this.ToDate.ToString()) + " ");
      if (this.Author != null)
        stringBuilder.Append("--author=******* ");
      if (this.Committer != null)
        stringBuilder.Append("--committer=******* ");
      if (this.Skip.HasValue)
        stringBuilder.Append(string.Format("--skip={0} ", (object) this.Skip));
      if (this.MaxCount.HasValue)
        stringBuilder.Append(string.Format("--max-count={0} ", (object) this.MaxCount));
      if (this.Path != null)
        stringBuilder.Append("-- " + this.MakeSafe(this.Path));
      return stringBuilder.ToString();
    }

    private string MakeSafe(string arg) => arg.Replace(" ", "\\ ");
  }
}
