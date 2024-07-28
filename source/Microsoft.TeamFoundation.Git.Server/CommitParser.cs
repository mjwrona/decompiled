// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.CommitParser
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class CommitParser
  {
    private static readonly string c_pgpArmorDashes = "-----";
    private static readonly string c_beginPgpArmorLead = CommitParser.c_pgpArmorDashes + "BEGIN PGP ";
    private static readonly string c_endPgpArmorLead = CommitParser.c_pgpArmorDashes + "END PGP ";
    private const string c_layer = "TfsGitCommit";
    private const int c_maxCommentLength = 85000;
    private static readonly Encoding s_parserEncoding = Encoding.UTF8;

    public static void ParseMetadata(
      Stream stream,
      long length,
      CommitParserOptions options,
      ICommitParserHandler handler)
    {
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      bool flag4 = true;
      ArraySegment<byte> buffer = GitStreamUtil.GetBuffer(stream, length);
      int offset = buffer.Offset;
      int iAfter = offset + buffer.Count;
      string str1 = CommitParser.ReadLine(buffer, ref offset, iAfter);
      while (!string.IsNullOrEmpty(str1))
      {
        bool flag5 = false;
        string str2 = string.Empty;
        int num = str1.IndexOf(' ');
        if (!flag2)
        {
          if (num < 0)
            throw new CommitObjectFailedToParseException("Commit parse failed due to lack of a space on the line: " + str1);
          string str3 = str1;
          int length1 = num;
          int startIndex = length1 + 1;
          str2 = str3.Substring(0, length1);
          str1 = startIndex != str1.Length ? str1.Substring(startIndex) : string.Empty;
        }
        switch (str2)
        {
          case "tree":
            if (!flag4)
              throw new CommitObjectFailedToParseException("Commit parse failed due to a tree header not on the first line");
            Sha1Id tree;
            try
            {
              tree = new Sha1Id(str1);
            }
            catch (ArgumentException ex)
            {
              throw new CommitObjectFailedToParseException("Commit parse failed due to a bad object id on tree header line: " + str1);
            }
            handler.OnTree(tree);
            flag1 = true;
            break;
          case "parent":
            if (!flag1 | flag3)
              throw new CommitObjectFailedToParseException("Commit parse failed due to parent being out of order. It should already have a line and not have an author.");
            Sha1Id parent;
            try
            {
              parent = new Sha1Id(str1);
            }
            catch (ArgumentException ex)
            {
              throw new CommitObjectFailedToParseException("Commit parse failed due to a bad object id on parent header line: " + str1);
            }
            handler.OnParent(parent);
            break;
          case "author":
            if (!flag1)
              throw new CommitObjectFailedToParseException("Commit parse failed due to author being out of order. It should already have a tree.");
            IdentityAndDate result1;
            if (!IdentityAndDate.TryParse(str1, out result1))
              throw new CommitObjectFailedToParseException("Commit parse failed due to author identity failed to parse: " + str1);
            handler.OnAuthor(result1);
            flag3 = true;
            break;
          case "committer":
            if (!flag3)
              throw new CommitObjectFailedToParseException("Commit parse failed due to committer being out of order. It should already have an author.");
            IdentityAndDate result2;
            if (!IdentityAndDate.TryParse(str1, out result2))
              throw new CommitObjectFailedToParseException("Commit parse failed due to committer identity failed to parse: " + str1);
            handler.OnCommitter(result2);
            flag2 = true;
            break;
          default:
            if (!flag2)
              throw new CommitObjectFailedToParseException("We must have a committer before having custom headers: " + str1);
            do
            {
              str1 = CommitParser.ReadLine(buffer, ref offset, iAfter);
            }
            while (str1 != null && str1.Length > 0 && str1[0] == ' ');
            flag5 = true;
            break;
        }
        if (!flag5)
          str1 = CommitParser.ReadLine(buffer, ref offset, iAfter);
        flag4 = false;
      }
      if (!flag1 || !flag3 || !flag2)
        throw new CommitObjectFailedToParseException("Commit parse failed due to missing a tree, author, or committer, all of which are required.");
      if ((options & CommitParserOptions.RejectInvalidComments) == CommitParserOptions.RejectInvalidComments)
      {
        for (int index = offset; index < iAfter; ++index)
        {
          if (buffer.Array[index] == (byte) 0)
            throw new CommitObjectFailedToParseException("Commit comment contained one or more null bytes");
        }
      }
      handler.OnCommentByteOffset(offset);
    }

    public static string ParseComment(Stream stream, int byteOffset, int maxCommentLength = 85000)
    {
      GitStreamUtil.SafeForwardSeek(stream, (long) byteOffset, false);
      string empty = string.Empty;
      int num = (int) stream.Length;
      if (num < 0)
        num = int.MaxValue;
      return (Math.Min(num - byteOffset, maxCommentLength) >= maxCommentLength ? CommitParser.ParseCommentLimited(stream, maxCommentLength) : CommitParser.ParseFullComment(stream)).Replace(char.MinValue, '�').Trim();
    }

    public static string ParseFullComment(Stream stream)
    {
      using (StreamReader streamReader = new StreamReader(stream, CommitParser.s_parserEncoding, false, GitStreamUtil.StreamReaderDefaultBufferSize, true))
        return streamReader.ReadToEnd();
    }

    private static string ParseCommentLimited(Stream stream, int limit)
    {
      StringBuilder stringBuilder = new StringBuilder(limit);
      Decoder decoder = CommitParser.s_parserEncoding.GetDecoder();
      int defaultBufferSize = GitStreamUtil.StreamReaderDefaultBufferSize;
      char[] chars1 = new char[CommitParser.s_parserEncoding.GetMaxCharCount(defaultBufferSize) - 1];
      byte[] numArray = new byte[defaultBufferSize];
      int byteCount;
      for (int index = 0; index < limit; index += byteCount)
      {
        int count = Math.Min(numArray.Length, limit - index);
        byteCount = stream.Read(numArray, 0, count);
        int chars2 = decoder.GetChars(numArray, 0, byteCount, chars1, 0);
        stringBuilder.Append(chars1, 0, chars2);
        if (byteCount == 0)
          break;
      }
      return stringBuilder.ToString();
    }

    private static string ReadLine(ArraySegment<byte> commit, ref int offset, int iAfter)
    {
      string str = (string) null;
      if (offset < iAfter)
      {
        int num = Array.IndexOf<byte>(commit.Array, (byte) 10, offset, iAfter - offset);
        if (num != -1)
        {
          int count = num - offset;
          str = CommitParser.s_parserEncoding.GetString(commit.Array, offset, count);
          offset = num + 1;
        }
      }
      return str;
    }
  }
}
