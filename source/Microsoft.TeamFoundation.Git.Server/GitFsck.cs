// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitFsck
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class GitFsck
  {
    private const char c_optionIndicator = '-';
    private const char c_gitComment = '#';
    private const char c_gitSectionStart = '[';
    private const string c_gitUrlKey = "url";
    private static readonly byte[] s_gitmodulesNameNoDot = GitEncodingUtil.SafeAscii.GetBytes("gitmodules");
    private static readonly byte[] s_gitmodulesHashedShortNameBeforeTilde = GitEncodingUtil.SafeAscii.GetBytes("gi7eba");
    private static readonly byte[] s_gitNameNoDot = GitEncodingUtil.SafeAscii.GetBytes("git");

    public static IEnumerable<string> Fsck(
      IVssRequestContext rc,
      TfsGitCommit commit,
      GitFsckMode fsckMode)
    {
      List<string> warnings = new List<string>();
      foreach (TreeEntryAndPath treeEntryAndPath in commit.GetTree().GetTreeEntriesRecursive())
      {
        GitFsck.FsckTreeEntry(rc, commit, treeEntryAndPath.Entry, fsckMode, (IList<string>) warnings);
        GitFsck.FsckGitmodule(rc, commit, treeEntryAndPath.Entry, fsckMode);
      }
      return (IEnumerable<string>) warnings;
    }

    private static void FsckGitmodule(
      IVssRequestContext rc,
      TfsGitCommit commit,
      TfsGitTreeEntry entry,
      GitFsckMode fsckMode)
    {
      if ((fsckMode & GitFsckMode.Submodules) != GitFsckMode.Submodules || !GitFsck.NameMatchesGitmodules(entry.NameBytes))
        return;
      if (entry.PackType != GitPackObjectType.Blob)
        throw new UnsafeGitmodulesException(commit.ObjectId, entry.Name, Resources.Format("UnsafeGitmodules_Reason_InvalidObjectType", (object) entry.PackType));
      TfsGitBlob gitModuleFileBlob = !entry.Mode.IsSymlink() ? (TfsGitBlob) entry.Object : throw new UnsafeGitmodulesException(commit.ObjectId, entry.Name, Resources.Get("UnsafeGitmodules_Reason_IsSymlink"));
      foreach (TfsGitSubmodule gitmodule in (IEnumerable<TfsGitSubmodule>) new TfsGitSubmodule().ParseGitmodules(rc, gitModuleFileBlob))
      {
        if (!GitFsck.IsNameSafe(gitmodule.Name))
          throw new UnsafeGitmodulesException(commit.ObjectId, entry.Name, Resources.Format("UnsafeGitmodules_Reason_UnsafeEntryName", (object) gitmodule.Name));
        if (GitFsck.LooksLikeCommandOption(gitmodule.Path))
          throw new UnsafeGitmodulesException(commit.ObjectId, entry.Name, Resources.Format("UnsafeGitmodules_Reason_UnsafeEntryPath", (object) gitmodule.Path));
        GitFsck.UrlSafeResult urlSafeResult = GitFsck.IsUrlSafe(rc, gitmodule.RepositoryUrl);
        switch (urlSafeResult)
        {
          case GitFsck.UrlSafeResult.Safe:
          case GitFsck.UrlSafeResult.UnableToDetermine:
            continue;
          case GitFsck.UrlSafeResult.Unsafe:
            throw new UnsafeGitmodulesException(commit.ObjectId, entry.Name, Resources.Format("UnsafeGitmodules_Reason_UnsafeEntryUrl", (object) gitmodule.RepositoryUrl));
          default:
            throw new NotImplementedException(urlSafeResult.ToString());
        }
      }
    }

    private static void FsckTreeEntry(
      IVssRequestContext rc,
      TfsGitCommit commit,
      TfsGitTreeEntry entry,
      GitFsckMode fsckMode,
      IList<string> warnings)
    {
      if ((fsckMode & GitFsckMode.AlternateDataStreams) == GitFsckMode.AlternateDataStreams && GitFsck.LooksLikeMaliciousDotGit(entry))
        throw new UnsafeDotGitException(commit.ObjectId, entry.Name);
      if ((fsckMode & GitFsckMode.ItemPathBackslash) != GitFsckMode.ItemPathBackslash || !GitFsck.PathContainsBackslash(entry))
        return;
      string str = Resources.Format("GitRefUpdateWarningUnsafeEntryPath", (object) commit.ObjectId.ToAbbreviatedString(), (object) entry.Name);
      warnings.Add(str);
    }

    internal static bool NameMatchesGitmodules(ArraySegment<byte> nameBytes) => GitFsck.NameMatchesNtfsLongOrHfsPlus(GitFsck.s_gitmodulesNameNoDot, nameBytes) || GitFsck.NameMatchesNtfs1To4Short(GitFsck.s_gitmodulesNameNoDot, nameBytes) || GitFsck.NameMatchesNtfsHashedShort(GitFsck.s_gitmodulesHashedShortNameBeforeTilde, nameBytes);

    internal static bool StartsWithDotGitSynonym(
      ArraySegment<byte> nameBytes,
      int startIndex,
      out int endIndex)
    {
      return GitFsck.StartsWithDotName(GitFsck.s_gitNameNoDot, nameBytes, startIndex, out endIndex) || GitFsck.StartsWithNameNtfs1To4Short(GitFsck.s_gitNameNoDot, nameBytes, startIndex, out endIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool StartsWithDotName(
      byte[] expectedNoDot,
      ArraySegment<byte> nameBytes,
      int startIndex,
      out int endIndex)
    {
      endIndex = -1;
      int index = startIndex;
      int num = index + nameBytes.Count;
      if (nameBytes.Count < expectedNoDot.Length + 1 || index >= num || nameBytes.Array[index] != (byte) 46 || !GitFsck.RangeMatchesExpectedAZLower(expectedNoDot, nameBytes.Array, index + 1))
        return false;
      endIndex = index + expectedNoDot.Length;
      return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool StartsWithNameNtfs1To4Short(
      byte[] expectedNoDot,
      ArraySegment<byte> nameBytes,
      int startIndex,
      out int endIndex)
    {
      endIndex = -1;
      int iActual = startIndex;
      int num = startIndex + nameBytes.Count;
      if (nameBytes.Count < expectedNoDot.Length + 2 || iActual >= num || !GitFsck.MatchesExpectedAZLower(expectedNoDot[0], nameBytes.Array[iActual]) || !GitFsck.RangeMatchesExpectedAZLower(expectedNoDot, nameBytes.Array, iActual) || nameBytes.Array[iActual + expectedNoDot.Length] != (byte) 126 || nameBytes.Array[iActual + expectedNoDot.Length + 1] < (byte) 49 || nameBytes.Array[iActual + expectedNoDot.Length + 1] > (byte) 52)
        return false;
      endIndex = iActual + expectedNoDot.Length + 1;
      return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool NameMatchesNtfsLongOrHfsPlus(
      byte[] expectedNoDot,
      ArraySegment<byte> actual)
    {
      if (actual.Count < expectedNoDot.Length + 1)
        return false;
      bool flag1 = false;
      int offset = actual.Offset;
      int iAfter = offset + actual.Count;
      bool flag2 = flag1 | GitFsck.TrySkipIgnoredHfsPlus(actual.Array, ref offset, iAfter);
      if (offset >= iAfter || actual.Array[offset] != (byte) 46)
        return false;
      int iActual = offset + 1;
      int index = 0;
      while (index < expectedNoDot.Length)
      {
        flag2 |= GitFsck.TrySkipIgnoredHfsPlus(actual.Array, ref iActual, iAfter);
        if (iActual >= iAfter || !GitFsck.MatchesExpectedAZLower(expectedNoDot[index], actual.Array[iActual]))
          return false;
        ++index;
        ++iActual;
      }
      return !(flag2 | GitFsck.TrySkipIgnoredHfsPlus(actual.Array, ref iActual, iAfter)) ? GitFsck.MatchesNtfsTrailingDotOrSpace(actual.Array, iActual, actual.Offset + actual.Count) : iActual == iAfter;
    }

    internal static bool TEST_TrySkipIgnoredHfsPlus(byte[] actual, ref int iActual, int iAfter) => GitFsck.TrySkipIgnoredHfsPlus(actual, ref iActual, iAfter);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TrySkipIgnoredHfsPlus(byte[] actual, ref int iActual, int iAfter)
    {
      bool flag = false;
      while (iActual + 2 < iAfter)
      {
        if (actual[iActual] == (byte) 226)
        {
          if (actual[iActual + 1] == (byte) 128)
          {
            if (actual[iActual + 2] >= (byte) 140 && actual[iActual + 2] <= (byte) 143)
              flag = true;
            else if (actual[iActual + 2] >= (byte) 170 && actual[iActual + 2] <= (byte) 174)
              flag = true;
            else
              break;
          }
          else if (actual[iActual + 1] == (byte) 129 && actual[iActual + 2] >= (byte) 170 && actual[iActual + 2] <= (byte) 175)
            flag = true;
          else
            break;
        }
        else if (actual[iActual] == (byte) 239 && actual[iActual + 1] == (byte) 187 && actual[iActual + 2] == (byte) 191)
          flag = true;
        else
          break;
        iActual += 3;
      }
      return flag;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool NameMatchesNtfs1To4Short(byte[] expectedNoDot, ArraySegment<byte> actual)
    {
      int num = Math.Min(6, expectedNoDot.Length);
      if (actual.Count < num + 2)
        return false;
      int offset = actual.Offset;
      int index1 = 0;
      while (index1 < num)
      {
        if (!GitFsck.MatchesExpectedAZLower(expectedNoDot[index1], actual.Array[offset]))
          return false;
        ++index1;
        ++offset;
      }
      if (actual.Array[offset] != (byte) 126)
        return false;
      int index2 = offset + 1;
      if (actual.Array[index2] < (byte) 49 || actual.Array[index2] > (byte) 52)
        return false;
      int iActual = index2 + 1;
      return GitFsck.MatchesNtfsTrailingDotOrSpace(actual.Array, iActual, actual.Offset + actual.Count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool NameMatchesNtfsHashedShort(
      byte[] expectedBeforeTilde,
      ArraySegment<byte> actual)
    {
      if (actual.Count < 8)
        return false;
      int offset = actual.Offset;
      for (int index = 0; index < 6 && GitFsck.MatchesExpectedAsciiLower(expectedBeforeTilde[index], actual.Array[offset]); ++offset)
        ++index;
      if (actual.Array[offset] != (byte) 126)
        return false;
      int index1 = offset + 1;
      if (actual.Array[index1] < (byte) 49 || actual.Array[index1] > (byte) 57)
        return false;
      int iActual;
      for (iActual = index1 + 1; iActual < 8; ++iActual)
      {
        if (actual.Array[iActual] < (byte) 48 || actual.Array[iActual] > (byte) 57)
          return false;
      }
      return GitFsck.MatchesNtfsTrailingDotOrSpace(actual.Array, iActual, actual.Offset + actual.Count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool MatchesNtfsTrailingDotOrSpace(byte[] actual, int iActual, int iAfterActual)
    {
      for (; iActual < iAfterActual && actual[iActual] != (byte) 58 && actual[iActual] != (byte) 92 && actual[iActual] != (byte) 0; ++iActual)
      {
        if (actual[iActual] != (byte) 32 && actual[iActual] != (byte) 46)
          return false;
      }
      return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool MatchesExpectedAsciiLower(byte expected, byte actual) => (int) expected == (actual < (byte) 65 || actual > (byte) 90 ? (int) actual : (int) actual | 32);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool MatchesExpectedAZLower(byte expected, byte actual) => (int) expected == ((int) actual | 32);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool RangeMatchesExpectedAZLower(byte[] expected, byte[] actual, int iActual)
    {
      for (int index = 0; index < expected.Length; ++index)
      {
        if (!GitFsck.MatchesExpectedAZLower(expected[index], actual[iActual]))
          return false;
        ++iActual;
      }
      return true;
    }

    internal static bool IsNameSafe(string name)
    {
      if (string.IsNullOrEmpty(name))
        return false;
      int iBefore = -1;
      for (int index = 0; index < name.Length; ++index)
      {
        if (GitFsck.IsForwardOrBackSlash(name[index]))
        {
          if (GitFsck.IsDotDotBetween(name, iBefore, index))
            return false;
          iBefore = index;
        }
      }
      return !GitFsck.IsDotDotBetween(name, iBefore, name.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsForwardOrBackSlash(char c) => c == '/' || c == '\\';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsDotDotBetween(string name, int iBefore, int iAfter) => iAfter - iBefore == 3 && name[iBefore + 1] == '.' && name[iBefore + 2] == '.';

    internal static GitFsck.UrlSafeResult IsUrlSafe(IVssRequestContext rc, string url)
    {
      try
      {
        if (GitFsck.LooksLikeCommandOption(url))
          return GitFsck.UrlSafeResult.Unsafe;
        GitFsck.GitUrl gitUrl = GitFsck.GitUrl.Parse(url);
        if (gitUrl != null)
          return GitFsck.LooksLikeCommandOption(gitUrl.Host) || GitFsck.LooksLikeCommandOption(gitUrl.Port) ? GitFsck.UrlSafeResult.Unsafe : GitFsck.UrlSafeResult.Safe;
        Uri result;
        if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out result))
          return result.IsAbsoluteUri && !string.IsNullOrEmpty(result.Host) ? (result.Host[0] == '-' ? GitFsck.UrlSafeResult.Unsafe : GitFsck.UrlSafeResult.UnableToDetermine) : GitFsck.UrlSafeResult.Safe;
      }
      catch (Exception ex)
      {
        rc.TraceException(1013763, GitServerUtils.TraceArea, nameof (GitFsck), ex);
      }
      return GitFsck.UrlSafeResult.UnableToDetermine;
    }

    private static bool LooksLikeCommandOption(string value) => !string.IsNullOrEmpty(value) && value[0] == '-';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool LooksLikeMaliciousDotGit(TfsGitTreeEntry entry)
    {
      int offset = entry.NameBytes.Offset;
      int num = offset + entry.NameBytes.Count;
      GitFsck.TrySkipIgnoredHfsPlus(entry.NameBytes.Array, ref offset, num);
      int endIndex;
      if (GitFsck.StartsWithDotGitSynonym(entry.NameBytes, offset, out endIndex) && GitFsck.MatchesNtfsTrailingDotOrSpace(entry.NameBytes.Array, endIndex + 1, num))
        return true;
      for (; offset < num; ++offset)
      {
        if (entry.NameBytes.Array[offset] == (byte) 92 && GitFsck.StartsWithDotGitSynonym(entry.NameBytes, offset + 1, out endIndex) && GitFsck.MatchesNtfsTrailingDotOrSpace(entry.NameBytes.Array, endIndex + 1, num))
          return true;
      }
      return false;
    }

    private static bool PathContainsBackslash(TfsGitTreeEntry entry) => entry.Name.IndexOf("\\") > -1;

    public enum UrlSafeResult
    {
      Invalid,
      Safe,
      Unsafe,
      UnableToDetermine,
    }

    public class GitUrl
    {
      private const string c_schemeDelimiter = "://";
      private const string c_segmentDelimiter = "/";
      private const string c_portDelimiter = ":";
      private const string c_authDelimiter = "@";
      private const string c_endOfStringDelimiter = "%%";

      private GitUrl(string host, string port)
      {
        this.Host = host;
        this.Port = port;
      }

      public string Host { get; }

      public string Port { get; }

      public static GitFsck.GitUrl Parse(string url)
      {
        try
        {
          url += "%%";
          string tokenFound;
          if (!GitFsck.GitUrl.TryConsume(ref url, "://", out tokenFound))
            return (GitFsck.GitUrl) null;
          GitFsck.GitUrl.TryConsume(ref url, "@", out tokenFound);
          string delimiterUsed;
          string host = GitFsck.GitUrl.Consume(ref url, new string[3]
          {
            ":",
            "/",
            "%%"
          }, out delimiterUsed);
          if (string.IsNullOrEmpty(host))
            return (GitFsck.GitUrl) null;
          string port = (string) null;
          if (delimiterUsed == ":")
            port = GitFsck.GitUrl.Consume(ref url, new string[2]
            {
              "/",
              "%%"
            }, out delimiterUsed);
          return new GitFsck.GitUrl(host, port);
        }
        catch (Exception ex)
        {
          return (GitFsck.GitUrl) null;
        }
      }

      private static string Consume(ref string url, string[] delimiters, out string delimiterUsed)
      {
        foreach (string delimiter in delimiters)
        {
          string tokenFound;
          if (GitFsck.GitUrl.TryConsume(ref url, delimiter, out tokenFound))
          {
            delimiterUsed = delimiter;
            return tokenFound;
          }
        }
        delimiterUsed = (string) null;
        return (string) null;
      }

      private static bool TryConsume(ref string url, string delimiter, out string tokenFound)
      {
        int length = url.IndexOf(delimiter);
        if (length > 0)
        {
          tokenFound = url.Substring(0, length);
          url = url.Substring(length + delimiter.Length);
          return true;
        }
        tokenFound = (string) null;
        return false;
      }
    }
  }
}
