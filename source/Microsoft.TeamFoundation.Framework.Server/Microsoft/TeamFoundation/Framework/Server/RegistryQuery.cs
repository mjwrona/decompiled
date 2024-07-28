// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RegistryQuery
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public readonly struct RegistryQuery
  {
    public readonly string Path;
    public readonly string Pattern;
    public readonly int Depth;
    private static readonly bool[] s_validCharsTable = new bool[128]
    {
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      false,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      false,
      false,
      true,
      true,
      true,
      false,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      false,
      true,
      false,
      true,
      false,
      false,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      false,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      true,
      false,
      true,
      true,
      true
    };

    public RegistryQuery(string registryPathPattern, bool allowPatterns = true)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(registryPathPattern, nameof (registryPathPattern));
      RegistryQuery.CheckOrParseHelper(registryPathPattern, allowPatterns, true, out this.Path, out this.Pattern, out this.Depth);
      if (this.Path.Length != 0)
        return;
      this.Path = "/";
    }

    internal RegistryQuery(string path, string pattern, int depth)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      pattern = pattern ?? string.Empty;
      ArgumentUtility.CheckForOutOfRange(depth, nameof (depth), 0);
      this.Path = path;
      this.Pattern = pattern;
      this.Depth = depth;
    }

    public static implicit operator RegistryQuery(string path) => new RegistryQuery(path);

    public override string ToString() => !string.IsNullOrEmpty(this.Pattern) ? string.Format("Path: {0} Pattern: {1} Depth: {2}", (object) this.Path, (object) this.Pattern, (object) this.Depth) : string.Format("Path: {0} Depth: {1}", (object) this.Path, (object) this.Depth);

    public bool Matches(string entryPath)
    {
      if (!RegistryHelpers.IsSubItem(entryPath, this.Path) || this.Depth < int.MaxValue && RegistryQuery.PathDepth(entryPath, this.Path) > this.Depth)
        return false;
      if (string.IsNullOrEmpty(this.Pattern) || this.Pattern.Length == 1 && this.Pattern[0] == '*')
        return true;
      if (entryPath[entryPath.Length - 1] == '/')
        return false;
      int num = entryPath.LastIndexOf('/') + 1;
      if (Wildcard.IsWildcard(this.Pattern))
        return Wildcard.Match(entryPath.Substring(num), this.Pattern);
      return entryPath.Length == this.Pattern.Length + num && string.Compare(this.Pattern, 0, entryPath, num, this.Pattern.Length, StringComparison.OrdinalIgnoreCase) == 0;
    }

    private static int PathDepth(string childPath, string rootPath)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(childPath, nameof (childPath));
      ArgumentUtility.CheckStringForNullOrEmpty(rootPath, nameof (rootPath));
      int num = 0;
      for (int length = rootPath.Length; length < childPath.Length && num < 2; ++length)
      {
        if (childPath[length] == '/')
          ++num;
      }
      return num;
    }

    internal static void CheckOrParseHelper(
      string path,
      bool allowPatterns,
      bool parse,
      out string queryPath,
      out string queryPattern,
      out int queryDepth)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      if (parse)
      {
        if (path[0] != '/')
          path = RegistryHelpers.CombinePath("/", path);
        while (path.Length > 1 && path[path.Length - 1] == '/')
          path = path.Substring(0, path.Length - 1);
      }
      if (path[0] != '/')
        throw new RegistryPathException(path, FrameworkResources.RegistryPathRequired());
      if (path.Length > 1 && path[path.Length - 1] == '/')
        throw new RegistryPathException(path, FrameworkResources.RegistryPathBadEnding());
      RegistryQuery.PathPartData data1 = new RegistryQuery.PathPartData();
      RegistryQuery.PathPartData data2 = new RegistryQuery.PathPartData();
      for (int index1 = 1; index1 < path.Length; ++index1)
      {
        char index2 = path[index1];
        if (index2 == '/')
        {
          if (data1.IsValid)
          {
            int num = (int) RegistryQuery.CheckPathPart(path, ref data1, allowPatterns, (ushort) 2);
          }
          data2.IndexOfNextPathPart = index1;
          data1 = data2;
          data2 = new RegistryQuery.PathPartData();
        }
        else
        {
          if (index2 < '\u0080' && !RegistryQuery.s_validCharsTable[(int) index2])
            throw new RegistryPathException(path, FrameworkResources.InvalidRegistryPathInvalidCharactersAndWildcards());
          switch (index2)
          {
            case '*':
              ++data2.AsteriskCount;
              break;
            case '.':
              ++data2.DotCount;
              break;
          }
          ++data2.PathPartLength;
        }
      }
      queryPath = "/";
      queryPattern = string.Empty;
      queryDepth = int.MaxValue;
      if (path.Length <= 1)
        return;
      data2.IndexOfNextPathPart = path.Length;
      RegistryQuery.PathPartType pathPartType1 = RegistryQuery.PathPartType.Literal;
      if (data1.IsValid)
        pathPartType1 = RegistryQuery.CheckPathPart(path, ref data1, allowPatterns, (ushort) 1);
      RegistryQuery.PathPartType pathPartType2 = RegistryQuery.CheckPathPart(path, ref data2, allowPatterns, (ushort) 0);
      int length;
      int startIndex;
      if (pathPartType1 == RegistryQuery.PathPartType.Literal)
      {
        switch (pathPartType2)
        {
          case RegistryQuery.PathPartType.RecursionOperator:
            length = data1.IndexOfNextPathPart;
            startIndex = -1;
            queryDepth = int.MaxValue;
            break;
          case RegistryQuery.PathPartType.Pattern:
            length = data1.IndexOfNextPathPart;
            startIndex = data2.IndexOfNextPathPart - data2.PathPartLength;
            queryDepth = 1;
            break;
          case RegistryQuery.PathPartType.Literal:
            length = path.Length;
            startIndex = -1;
            queryDepth = 0;
            break;
          default:
            throw new InvalidOperationException();
        }
      }
      else
      {
        if (pathPartType1 != RegistryQuery.PathPartType.RecursionOperator)
          throw new InvalidOperationException();
        if (pathPartType2 == RegistryQuery.PathPartType.Literal || pathPartType2 == RegistryQuery.PathPartType.Pattern)
        {
          length = data1.IndexOfNextPathPart - data1.PathPartLength - 1;
          startIndex = data2.IndexOfNextPathPart - data2.PathPartLength;
          queryDepth = int.MaxValue;
        }
        else
        {
          if (pathPartType2 == RegistryQuery.PathPartType.RecursionOperator)
            throw new RegistryPathPatternException(path, FrameworkResources.RegistryBadPatternMatch());
          throw new InvalidOperationException();
        }
      }
      if (length > 259)
        throw new InvalidPathException(TFCommonResources.InvalidServerPathTooLong((object) path.Substring(0, length)));
      if (!parse)
        return;
      queryPath = path.Substring(0, length);
      if (startIndex <= 0)
        return;
      queryPattern = path.Substring(startIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static RegistryQuery.PathPartType CheckPathPart(
      string path,
      ref RegistryQuery.PathPartData data,
      bool allowPatterns,
      ushort remainingPathParts)
    {
      RegistryQuery.PathPartType pathPartType = RegistryQuery.PathPartType.Literal;
      if (data.PathPartLength == 0)
        throw new RegistryPathException(path, FrameworkResources.RegistryPathEmptySegment());
      if (data.AsteriskCount > 0)
      {
        if (!allowPatterns)
          throw new RegistryPathException(path, FrameworkResources.RegistryPathNoLiterals());
        if (remainingPathParts > (ushort) 1)
          throw new RegistryPathPatternException(path, FrameworkResources.RegistryBadPatternMatch());
        if (data.PathPartLength == 2 && data.AsteriskCount == 2)
        {
          pathPartType = RegistryQuery.PathPartType.RecursionOperator;
        }
        else
        {
          if (remainingPathParts == (ushort) 1)
            throw new RegistryPathPatternException(path, FrameworkResources.RegistryBadPatternMatch());
          pathPartType = RegistryQuery.PathPartType.Pattern;
        }
      }
      else if (data.DotCount == data.PathPartLength)
      {
        if (!allowPatterns)
          throw new RegistryPathException(path, FrameworkResources.RegistryPathNoLiterals());
        if (data.DotCount != 3)
          throw new RegistryPathPatternException(path, FrameworkResources.RegistryPathInvalidSegment((object) path.Substring(data.IndexOfNextPathPart - data.PathPartLength, data.PathPartLength)));
        if (remainingPathParts > (ushort) 1)
          throw new RegistryPathPatternException(path, FrameworkResources.RegistryBadPatternMatch());
        pathPartType = RegistryQuery.PathPartType.RecursionOperator;
      }
      return pathPartType;
    }

    private struct PathPartData
    {
      public int IndexOfNextPathPart;
      public int PathPartLength;
      public int AsteriskCount;
      public int DotCount;

      public bool IsValid => this.IndexOfNextPathPart != 0;
    }

    private enum PathPartType
    {
      RecursionOperator,
      Pattern,
      Literal,
    }
  }
}
