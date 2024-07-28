// Decompiled with JetBrains decompiler
// Type: Nest.Fuzziness
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  [JsonFormatter(typeof (FuzzinessFormatter))]
  public class Fuzziness : IFuzziness
  {
    private bool _auto;
    private int? _low;
    private int? _high;
    private int? _editDistance;
    private double? _ratio;

    public static Fuzziness Auto => new Fuzziness()
    {
      _auto = true
    };

    public static Fuzziness AutoLength(int low, int high) => new Fuzziness()
    {
      _auto = true,
      _low = new int?(low),
      _high = new int?(high)
    };

    bool IFuzziness.Auto => this._auto;

    int? IFuzziness.Low => this._low;

    int? IFuzziness.High => this._high;

    int? IFuzziness.EditDistance => this._editDistance;

    double? IFuzziness.Ratio => this._ratio;

    public static Fuzziness EditDistance(int distance) => new Fuzziness()
    {
      _editDistance = new int?(distance)
    };

    public static Fuzziness Ratio(double ratio) => new Fuzziness()
    {
      _ratio = new double?(ratio)
    };
  }
}
