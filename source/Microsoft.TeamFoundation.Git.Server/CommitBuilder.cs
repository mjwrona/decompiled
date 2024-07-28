// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.CommitBuilder
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Server.Core.Security;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class CommitBuilder
  {
    public static Sha1Id CreateCommit(
      IEnumerable<Sha1Id> parents,
      Sha1Id tree,
      string author,
      string committer,
      string comment,
      out byte[] commitBytes)
    {
      IdentityAndDate result;
      if (!IdentityAndDate.TryParse(author, out result) || !IdentityAndDate.TryParse(committer, out result))
        throw new CommitObjectFailedToParseException();
      if (!string.IsNullOrEmpty(comment) && comment.Contains("\0"))
        throw new CommitObjectFailedToParseException("Commit comment contained one or more null bytes");
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("tree {0}\n", (object) tree);
      foreach (Sha1Id parent in parents)
        stringBuilder.AppendFormat("parent {0}\n", (object) parent);
      stringBuilder.AppendFormat("author {0}\n", (object) author);
      stringBuilder.AppendFormat("committer {0}\n", (object) committer);
      stringBuilder.Append("\n");
      stringBuilder.Append(comment);
      stringBuilder.Append("\n");
      commitBytes = GitEncodingUtil.SafeUtf8NoBom.GetBytes(stringBuilder.ToString());
      using (HashingStream<SHA1Cng2> hashingStream = new HashingStream<SHA1Cng2>())
      {
        hashingStream.Setup(Stream.Null, FileAccess.Write, leaveOpen: true);
        byte[] objectHeader = GitServerUtils.CreateObjectHeader(GitPackObjectType.Commit, (long) commitBytes.Length);
        hashingStream.Write(objectHeader, 0, objectHeader.Length);
        hashingStream.Write(commitBytes, 0, commitBytes.Length);
        return new Sha1Id(hashingStream.Hash);
      }
    }
  }
}
