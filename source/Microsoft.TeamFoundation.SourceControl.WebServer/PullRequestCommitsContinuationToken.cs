// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.PullRequestCommitsContinuationToken
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Git.Server;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public sealed class PullRequestCommitsContinuationToken
  {
    internal PullRequestCommitsContinuationToken(Sha1Id rootCommitId, Sha1Id nextCommitId)
    {
      this.RootCommitId = rootCommitId;
      this.NextCommitId = nextCommitId;
    }

    public static bool TryParseContinuationToken(
      string rawToken,
      out PullRequestCommitsContinuationToken token)
    {
      token = (PullRequestCommitsContinuationToken) null;
      if (string.IsNullOrWhiteSpace(rawToken))
        return false;
      string[] strArray = rawToken.Split(':');
      Sha1Id id1;
      Sha1Id id2;
      if (strArray.Length != 2 || !Sha1Id.TryParse(strArray[0], out id1) || !Sha1Id.TryParse(strArray[1], out id2))
        return false;
      token = new PullRequestCommitsContinuationToken(id1, id2);
      return true;
    }

    public override string ToString()
    {
      Sha1Id sha1Id = this.RootCommitId;
      string str1 = sha1Id.ToString();
      sha1Id = this.NextCommitId;
      string str2 = sha1Id.ToString();
      return str1 + ":" + str2;
    }

    internal Sha1Id RootCommitId { get; }

    internal Sha1Id NextCommitId { get; }
  }
}
