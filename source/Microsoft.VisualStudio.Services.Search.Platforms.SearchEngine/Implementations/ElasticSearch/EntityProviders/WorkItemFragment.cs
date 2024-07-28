// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders.WorkItemFragment
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders
{
  internal sealed class WorkItemFragment : IComparable<WorkItemFragment>
  {
    private static readonly Regex s_hitRegex = new Regex("^<(?<score>\\d+\\.?\\d+)#(?<startOffset>\\d+)#(?<endOffset>\\d+)#(?<contentLength>\\d+)>(?<snippet>.+)$", RegexOptions.Compiled | RegexOptions.Singleline);

    public string FieldReferenceName { get; set; }

    public string Text { get; set; }

    public double Score { get; set; }

    public int StartOffset { get; set; }

    public int EndOffset { get; set; }

    public int ContentLength { get; set; }

    public Exception Exception { get; set; }

    public static WorkItemFragment Parse(string fieldReferenceName, string fragment)
    {
      Match match = WorkItemFragment.s_hitRegex.Match(fragment);
      try
      {
        return new WorkItemFragment()
        {
          FieldReferenceName = fieldReferenceName,
          Score = Convert.ToDouble(match.Groups["score"].Value, (IFormatProvider) CultureInfo.InvariantCulture),
          StartOffset = Convert.ToInt32(match.Groups["startOffset"].Value, (IFormatProvider) CultureInfo.InvariantCulture),
          EndOffset = Convert.ToInt32(match.Groups["endOffset"].Value, (IFormatProvider) CultureInfo.InvariantCulture),
          ContentLength = Convert.ToInt32(match.Groups["contentLength"].Value, (IFormatProvider) CultureInfo.InvariantCulture),
          Text = match.Groups["snippet"].Value
        };
      }
      catch (Exception ex)
      {
        return new WorkItemFragment()
        {
          FieldReferenceName = fieldReferenceName,
          Score = 0.0,
          StartOffset = 0,
          EndOffset = 50,
          ContentLength = 100,
          Text = fragment.Substring(fragment.IndexOf('>') + 1),
          Exception = ex
        };
      }
    }

    public int CompareTo(WorkItemFragment other)
    {
      double num1 = this.Score * 1E-06;
      if (Math.Abs(this.Score - other.Score) > num1)
        return other.Score.CompareTo(this.Score);
      int num2 = string.Compare(this.FieldReferenceName, other.FieldReferenceName, StringComparison.Ordinal);
      return num2 != 0 ? num2 : this.StartOffset.CompareTo(other.StartOffset);
    }
  }
}
