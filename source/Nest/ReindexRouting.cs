// Decompiled with JetBrains decompiler
// Type: Nest.ReindexRouting
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  [JsonFormatter(typeof (ReindexRoutingFormatter))]
  public class ReindexRouting
  {
    public static ReindexRouting Discard = new ReindexRouting("discard", true);
    public static ReindexRouting Keep = new ReindexRouting("keep", true);
    private readonly string _newRoutingValue;

    public ReindexRouting(string newRoutingValue)
      : this(newRoutingValue, false)
    {
    }

    private ReindexRouting(string newRoutingValue, bool noPrefix)
    {
      string str = newRoutingValue.TrimStart('=');
      this._newRoutingValue = (noPrefix ? "" : "=") + str;
    }

    public static implicit operator ReindexRouting(string routing) => new ReindexRouting(routing);

    public override string ToString() => this._newRoutingValue;
  }
}
