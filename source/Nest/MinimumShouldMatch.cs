// Decompiled with JetBrains decompiler
// Type: Nest.MinimumShouldMatch
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  [JsonFormatter(typeof (MinimumShouldMatchFormatter))]
  public class MinimumShouldMatch : Union<int?, string>
  {
    public MinimumShouldMatch(int count)
      : base(new int?(count))
    {
    }

    public MinimumShouldMatch(string percentage)
      : base(percentage)
    {
    }

    public static MinimumShouldMatch Fixed(int count) => (MinimumShouldMatch) count;

    public static MinimumShouldMatch Percentage(double percentage) => (MinimumShouldMatch) string.Format("{0}%", (object) percentage);

    public static implicit operator MinimumShouldMatch(string first) => new MinimumShouldMatch(first);

    public static implicit operator MinimumShouldMatch(int second) => new MinimumShouldMatch(second);

    public static implicit operator MinimumShouldMatch(double second) => MinimumShouldMatch.Percentage(second);
  }
}
