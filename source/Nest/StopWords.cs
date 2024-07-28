// Decompiled with JetBrains decompiler
// Type: Nest.StopWords
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;

namespace Nest
{
  [JsonFormatter(typeof (StopWordsFormatter))]
  public class StopWords : Union<string, IEnumerable<string>>
  {
    public StopWords(string item)
      : base(item)
    {
    }

    public StopWords(IEnumerable<string> item)
      : base(item)
    {
    }

    public static implicit operator StopWords(string first) => new StopWords(first);

    public static implicit operator StopWords(List<string> second) => new StopWords((IEnumerable<string>) second);

    public static implicit operator StopWords(string[] second) => new StopWords((IEnumerable<string>) second);
  }
}
