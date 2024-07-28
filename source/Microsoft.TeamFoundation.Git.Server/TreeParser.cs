// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TreeParser
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class TreeParser
  {
    public static IEnumerable<TreeParser.Entry> Parse(
      Stream streamToParse,
      long length,
      TreeFsckOptions fsckOptions)
    {
      if (length > 16777216L)
        throw new TreeExceedsMaterializationLimitException(Resources.Format("TreeObjectFailedToParseException_TooBig", (object) 16777216L));
      return TreeParser.Parse(GitStreamUtil.GetBuffer(streamToParse, length), fsckOptions);
    }

    internal static IEnumerable<TreeParser.Entry> Parse(
      ArraySegment<byte> content,
      TreeFsckOptions fsckOptions)
    {
      ArraySegment<byte>? prevName = new ArraySegment<byte>?();
      GitPackObjectType prevPackType = GitPackObjectType.None;
      Dictionary<string, GitPackObjectType> caseInsensitiveNames = (Dictionary<string, GitPackObjectType>) null;
      if ((fsckOptions & TreeFsckOptions.EnforceConsistentCase) == TreeFsckOptions.EnforceConsistentCase)
        caseInsensitiveNames = new Dictionary<string, GitPackObjectType>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      int offset = content.Offset;
      int iAfter = offset + content.Count;
      while (offset < iAfter)
      {
        StatMode mode = TreeParser.ParseMode(content.Array, ref offset, iAfter);
        GitPackObjectType packType = mode.ToPackType();
        int length;
        int iNextNull;
        TreeParser.ReadEntryName(content.Array, offset, iAfter, fsckOptions, out length, out iNextNull);
        ArraySegment<byte> arraySegment = new ArraySegment<byte>(content.Array, offset, length);
        offset = iNextNull + 1 + 20;
        if (offset > iAfter)
          throw new TreeEndInsideObjectIdException(Resources.Get("TreeObjectFailedToParseException_EndInsideObjectId"));
        if ((fsckOptions & TreeFsckOptions.EnforceConsistentCase) == TreeFsckOptions.EnforceConsistentCase)
        {
          string nameStr = TreeParser.NameBytesToString(arraySegment);
          if (caseInsensitiveNames.ContainsKey(nameStr))
          {
            KeyValuePair<string, GitPackObjectType> keyValuePair = caseInsensitiveNames.First<KeyValuePair<string, GitPackObjectType>>((Func<KeyValuePair<string, GitPackObjectType>, bool>) (e => string.Equals(nameStr, e.Key, StringComparison.OrdinalIgnoreCase)));
            string str1 = packType == GitPackObjectType.Blob ? Resources.Get("BlobTreeEntryName") : Resources.Get("TreeTreeEntryName");
            string str2 = keyValuePair.Value == GitPackObjectType.Blob ? Resources.Get("BlobTreeEntryName") : Resources.Get("TreeTreeEntryName");
            throw new TreeCaseEnforcementException(Resources.Format("TreeCaseEnforcementError", (object) str1, (object) nameStr, (object) str2, (object) keyValuePair.Key));
          }
          caseInsensitiveNames.Add(nameStr, packType);
        }
        if (prevName.HasValue && TreeParser.GitCompareBytes(prevName.Value, prevPackType, arraySegment, packType) >= 0)
          throw new TreeBadSortOrderException(Resources.Get("TreeObjectFailedToParseException_BadSortOrder"));
        prevName = new ArraySegment<byte>?(arraySegment);
        prevPackType = packType;
        yield return new TreeParser.Entry(mode, arraySegment);
      }
    }

    private static void ReadEntryName(
      byte[] content,
      int offset,
      int iAfter,
      TreeFsckOptions fsckOptions,
      out int length,
      out int iNextNull)
    {
      bool flag = false;
      iNextNull = offset;
      while (iNextNull < iAfter)
      {
        int num = (int) content[iNextNull];
        if (num == 47)
          flag = true;
        if (num != 0)
          ++iNextNull;
        else
          break;
      }
      if (iNextNull == iAfter)
        throw new TreeMissingNullAfterEntryNameException(Resources.Get("TreeObjectFailedToParseException_MissingNullAfterEntryName"));
      length = iNextNull - offset;
      if (flag || TreeParser.DoIsInvalidEntryName(content, offset, length, fsckOptions))
        throw TreeParser.CreateInvalidEntryNameException(content, offset, length);
    }

    private static bool DoIsInvalidEntryName(
      byte[] content,
      int offset,
      int length,
      TreeFsckOptions fsckOptions)
    {
      if (length == 0)
        return true;
      if (content[offset] == (byte) 46)
      {
        switch (length)
        {
          case 1:
            return true;
          case 2:
            if (content[offset + 1] == (byte) 46)
              return true;
            break;
          case 4:
            if ((fsckOptions & TreeFsckOptions.RejectDotGit) == TreeFsckOptions.RejectDotGit && (content[offset + 1] == (byte) 103 || content[offset + 1] == (byte) 71) && (content[offset + 2] == (byte) 105 || content[offset + 2] == (byte) 73) && (content[offset + 3] == (byte) 116 || content[offset + 3] == (byte) 84))
              return true;
            break;
        }
      }
      return false;
    }

    private static Exception CreateInvalidEntryNameException(
      byte[] content,
      int offset,
      int length)
    {
      return (Exception) new TreeInvalidEntryNameException(Resources.Format("TreeObjectFailedToParseException_InvalidEntryName", length == 0 ? (object) "[empty]" : (object) Encoding.UTF8.GetString(content, offset, length)));
    }

    public static string NameBytesToString(ArraySegment<byte> nameBytes) => GitEncodingUtil.BestEffortUtf8NoBom.GetString(nameBytes.Array, nameBytes.Offset, nameBytes.Count);

    public static void CheckEntryName(ArraySegment<byte> nameBytes, TreeFsckOptions fsckOptions)
    {
      if (TreeParser.IsInvalidEntryName(nameBytes, fsckOptions))
        throw TreeParser.CreateInvalidEntryNameException(nameBytes.Array, nameBytes.Offset, nameBytes.Count);
    }

    public static bool IsInvalidEntryName(ArraySegment<byte> nameBytes, TreeFsckOptions fsckOptions)
    {
      ArgumentUtility.CheckForNull<byte[]>(nameBytes.Array, nameof (nameBytes));
      for (int offset = nameBytes.Offset; offset < nameBytes.Count; ++offset)
      {
        switch (nameBytes.Array[offset])
        {
          case 0:
          case 47:
            return true;
          default:
            continue;
        }
      }
      return TreeParser.DoIsInvalidEntryName(nameBytes.Array, nameBytes.Offset, nameBytes.Count, fsckOptions);
    }

    public static int GitCompareBytes(
      ArraySegment<byte> first,
      GitPackObjectType firstObjectType,
      ArraySegment<byte> second,
      GitPackObjectType secondObjectType)
    {
      for (int index = 0; index < first.Count && index < second.Count; ++index)
      {
        byte num1 = first.Array[first.Offset + index];
        byte num2 = second.Array[second.Offset + index];
        if ((int) num1 != (int) num2)
          return (int) num1 - (int) num2;
      }
      if (first.Count > second.Count && secondObjectType == GitPackObjectType.Tree)
        return (int) first.Array[first.Offset + second.Count] - 47;
      return first.Count < second.Count && firstObjectType == GitPackObjectType.Tree ? 47 - (int) second.Array[second.Offset + first.Count] : first.Count - second.Count;
    }

    internal static StatMode TEST_ParseMode(string octalMode)
    {
      string s = octalMode + " ";
      byte[] bytes = GitEncodingUtil.SafeAscii.GetBytes(s);
      int offset = 0;
      StatMode mode = TreeParser.ParseMode(bytes, ref offset, bytes.Length);
      if (offset != bytes.Length)
        throw new Exception(FormattableString.Invariant(FormattableStringFactory.Create("{0} failed to read all of {1}", (object) "ParseMode", (object) octalMode)));
      return mode;
    }

    private static StatMode ParseMode(byte[] buf, ref int offset, int iAfter)
    {
      int num1 = Math.Min(offset + 6 + 1, iAfter);
      int mode = 0;
      while (offset < num1)
      {
        byte num2 = buf[offset++];
        if (num2 >= (byte) 48 && num2 <= (byte) 55)
        {
          mode = (mode << 3) + ((int) num2 - 48);
        }
        else
        {
          if (num2 == (byte) 32)
            return (StatMode) mode;
          throw new TreeInvalidCharInModeException(Resources.Format("TreeObjectFailedToParseException_InvalidCharInMode", (object) string.Format("0x{0:x}", (object) (int) num2)));
        }
      }
      throw new TreeMissingSpaceBetweenModeAndNameException(Resources.Get("TreeObjectFailedToParseException_MissingSpaceBetweenModeAndName"));
    }

    internal struct Entry
    {
      internal Entry(StatMode mode, ArraySegment<byte> nameInFullContent)
      {
        this.Mode = mode;
        this.NameBytes = nameInFullContent;
        ArraySegment<byte> nameBytes = this.NameBytes;
        byte[] array = nameBytes.Array;
        nameBytes = this.NameBytes;
        int offset = nameBytes.Offset;
        nameBytes = this.NameBytes;
        int count = nameBytes.Count;
        int sourceIndex = offset + count + 1;
        this.ObjectId = new Sha1Id(array, sourceIndex);
      }

      public StatMode Mode { get; }

      public ArraySegment<byte> NameBytes { get; }

      public Sha1Id ObjectId { get; }

      public string GetNameString() => TreeParser.NameBytesToString(this.NameBytes);

      public GitPackObjectType PackType => this.Mode.ToPackType();
    }
  }
}
