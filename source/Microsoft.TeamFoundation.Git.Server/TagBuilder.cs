// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TagBuilder
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Server.Core.Security;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class TagBuilder
  {
    public static Sha1Id CreateAnnotatedTag(
      Sha1Id objectId,
      GitPackObjectType objectType,
      string name,
      string tagger,
      string comment,
      out byte[] tagBytes)
    {
      if (!IdentityAndDate.TryParse(tagger, out IdentityAndDate _))
        throw new TagObjectFailedToParseException();
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("object {0}\n", (object) objectId);
      stringBuilder.AppendFormat("type {0}\n", (object) objectType.ToGitString());
      stringBuilder.AppendFormat("tag {0}\n", (object) name);
      stringBuilder.AppendFormat("tagger {0}\n", (object) tagger);
      if (comment != null)
        stringBuilder.AppendFormat("\n{0}", (object) comment);
      else
        stringBuilder.Append("\n");
      tagBytes = GitEncodingUtil.SafeUtf8NoBom.GetBytes(stringBuilder.ToString());
      using (HashingStream<SHA1Cng2> hashingStream = new HashingStream<SHA1Cng2>())
      {
        hashingStream.Setup(Stream.Null, FileAccess.Write, leaveOpen: true);
        byte[] objectHeader = GitServerUtils.CreateObjectHeader(GitPackObjectType.Tag, (long) tagBytes.Length);
        hashingStream.Write(objectHeader, 0, objectHeader.Length);
        hashingStream.Write(tagBytes, 0, tagBytes.Length);
        return new Sha1Id(hashingStream.Hash);
      }
    }
  }
}
