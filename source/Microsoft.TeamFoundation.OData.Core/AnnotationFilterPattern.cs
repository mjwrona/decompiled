// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.AnnotationFilterPattern
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;

namespace Microsoft.OData
{
  internal abstract class AnnotationFilterPattern : IComparable<AnnotationFilterPattern>
  {
    internal static readonly AnnotationFilterPattern IncludeAllPattern = (AnnotationFilterPattern) new AnnotationFilterPattern.WildCardPattern(false);
    internal static readonly AnnotationFilterPattern ExcludeAllPattern = (AnnotationFilterPattern) new AnnotationFilterPattern.WildCardPattern(true);
    protected readonly string Pattern;
    private const char NamespaceSeparator = '.';
    private const char ExcludeOperator = '-';
    private const string WildCard = "*";
    private const string DotStar = ".*";
    private readonly bool isExclude;

    private AnnotationFilterPattern(string pattern, bool isExclude)
    {
      this.isExclude = isExclude;
      this.Pattern = pattern;
    }

    internal virtual bool IsExclude => this.isExclude;

    public int CompareTo(AnnotationFilterPattern other)
    {
      ExceptionUtils.CheckArgumentNotNull<AnnotationFilterPattern>(other, nameof (other));
      int num = AnnotationFilterPattern.ComparePatternPriority(this.Pattern, other.Pattern);
      if (num != 0)
        return num;
      if (this.IsExclude == other.IsExclude)
        return 0;
      return !this.IsExclude ? 1 : -1;
    }

    internal static AnnotationFilterPattern Create(string pattern)
    {
      AnnotationFilterPattern.ValidatePattern(pattern);
      bool isExclude = AnnotationFilterPattern.RemoveExcludeOperator(ref pattern);
      return pattern == "*" ? (!isExclude ? AnnotationFilterPattern.IncludeAllPattern : AnnotationFilterPattern.ExcludeAllPattern) : (pattern.EndsWith(".*", StringComparison.Ordinal) ? (AnnotationFilterPattern) new AnnotationFilterPattern.StartsWithPattern(pattern.Substring(0, pattern.Length - 1), isExclude) : (AnnotationFilterPattern) new AnnotationFilterPattern.ExactMatchPattern(pattern, isExclude));
    }

    internal static void Sort(AnnotationFilterPattern[] pattersToSort) => Array.Sort<AnnotationFilterPattern>(pattersToSort);

    internal abstract bool Matches(string annotationName);

    private static int ComparePatternPriority(string pattern1, string pattern2)
    {
      if (pattern1 == pattern2)
        return 0;
      if (pattern1 == "*")
        return 1;
      if (pattern2 == "*" || pattern1.StartsWith(pattern2, StringComparison.Ordinal))
        return -1;
      return pattern2.StartsWith(pattern1, StringComparison.Ordinal) ? 1 : 0;
    }

    private static bool RemoveExcludeOperator(ref string pattern)
    {
      if (pattern[0] != '-')
        return false;
      pattern = pattern.Substring(1);
      return true;
    }

    private static void ValidatePattern(string pattern)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(pattern, nameof (pattern));
      string pattern1 = pattern;
      AnnotationFilterPattern.RemoveExcludeOperator(ref pattern1);
      if (pattern1 == "*")
        return;
      string[] strArray = pattern1.Split('.');
      int length = strArray.Length;
      if (length == 1)
        throw new ArgumentException(Strings.AnnotationFilterPattern_InvalidPatternMissingDot((object) pattern));
      for (int index = 0; index < length; ++index)
      {
        string str = strArray[index];
        if (string.IsNullOrEmpty(str))
          throw new ArgumentException(Strings.AnnotationFilterPattern_InvalidPatternEmptySegment((object) pattern));
        if (str != "*" && str.Contains("*"))
          throw new ArgumentException(Strings.AnnotationFilterPattern_InvalidPatternWildCardInSegment((object) pattern));
        bool flag = index + 1 == length;
        if (str == "*" && !flag)
          throw new ArgumentException(Strings.AnnotationFilterPattern_InvalidPatternWildCardMustBeInLastSegment((object) pattern));
      }
    }

    private sealed class WildCardPattern : AnnotationFilterPattern
    {
      internal WildCardPattern(bool isExclude)
        : base("*", isExclude)
      {
      }

      internal override bool Matches(string annotationName) => true;
    }

    private sealed class StartsWithPattern : AnnotationFilterPattern
    {
      internal StartsWithPattern(string pattern, bool isExclude)
        : base(pattern, isExclude)
      {
      }

      internal override bool Matches(string annotationName) => annotationName.StartsWith(this.Pattern, StringComparison.Ordinal);
    }

    private sealed class ExactMatchPattern : AnnotationFilterPattern
    {
      internal ExactMatchPattern(string pattern, bool isExclude)
        : base(pattern, isExclude)
      {
      }

      internal override bool Matches(string annotationName) => annotationName.Equals(this.Pattern, StringComparison.Ordinal);
    }
  }
}
