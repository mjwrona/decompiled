// Decompiled with JetBrains decompiler
// Type: Nest.IFuzziness
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  [JsonFormatter(typeof (FuzzinessInterfaceFormatter))]
  public interface IFuzziness
  {
    bool Auto { get; }

    int? Low { get; }

    int? High { get; }

    int? EditDistance { get; }

    double? Ratio { get; }
  }
}
