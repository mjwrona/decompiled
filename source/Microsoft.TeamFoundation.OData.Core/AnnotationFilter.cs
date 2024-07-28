// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.AnnotationFilter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData
{
  internal class AnnotationFilter
  {
    private static readonly AnnotationFilter IncludeAll = (AnnotationFilter) new AnnotationFilter.IncludeAllFilter();
    private static readonly AnnotationFilter ExcludeAll = (AnnotationFilter) new AnnotationFilter.ExcludeAllFilter();
    private static readonly char[] AnnotationFilterPatternSeparator = new char[1]
    {
      ','
    };
    private readonly AnnotationFilterPattern[] prioritizedPatternsToMatch;

    private AnnotationFilter(
      AnnotationFilterPattern[] prioritizedPatternsToMatch)
    {
      this.prioritizedPatternsToMatch = prioritizedPatternsToMatch;
    }

    internal static AnnotationFilter Create(string filter)
    {
      if (string.IsNullOrEmpty(filter))
        return AnnotationFilter.ExcludeAll;
      AnnotationFilterPattern[] array = ((IEnumerable<string>) filter.Split(AnnotationFilter.AnnotationFilterPatternSeparator)).Select<string, AnnotationFilterPattern>((Func<string, AnnotationFilterPattern>) (pattern => AnnotationFilterPattern.Create(pattern.Trim()))).ToArray<AnnotationFilterPattern>();
      AnnotationFilterPattern.Sort(array);
      if (array[0] == AnnotationFilterPattern.IncludeAllPattern)
        return AnnotationFilter.IncludeAll;
      return array[0] == AnnotationFilterPattern.ExcludeAllPattern ? AnnotationFilter.ExcludeAll : new AnnotationFilter(array);
    }

    internal static AnnotationFilter CreateIncludeAllFilter() => (AnnotationFilter) new AnnotationFilter.IncludeAllFilter();

    internal virtual bool Matches(string annotationName)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(annotationName, nameof (annotationName));
      foreach (AnnotationFilterPattern annotationFilterPattern in this.prioritizedPatternsToMatch)
      {
        if (annotationFilterPattern.Matches(annotationName))
          return !annotationFilterPattern.IsExclude;
      }
      return false;
    }

    private sealed class IncludeAllFilter : AnnotationFilter
    {
      internal IncludeAllFilter()
        : base(new AnnotationFilterPattern[0])
      {
      }

      internal override bool Matches(string annotationName)
      {
        ExceptionUtils.CheckArgumentStringNotNullOrEmpty(annotationName, nameof (annotationName));
        return true;
      }
    }

    private sealed class ExcludeAllFilter : AnnotationFilter
    {
      internal ExcludeAllFilter()
        : base(new AnnotationFilterPattern[0])
      {
      }

      internal override bool Matches(string annotationName)
      {
        ExceptionUtils.CheckArgumentStringNotNullOrEmpty(annotationName, nameof (annotationName));
        return false;
      }
    }
  }
}
