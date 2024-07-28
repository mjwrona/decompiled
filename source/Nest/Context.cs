// Decompiled with JetBrains decompiler
// Type: Nest.Context
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  [JsonFormatter(typeof (ContextFormatter))]
  public class Context : Union<string, GeoLocation>
  {
    public Context(string category)
      : base(category)
    {
    }

    public Context(GeoLocation geo)
      : base(geo)
    {
    }

    public string Category => this.Item1;

    public GeoLocation Geo => this.Item2;

    public static implicit operator Context(string context) => new Context(context);

    public static implicit operator Context(GeoLocation context) => new Context(context);
  }
}
