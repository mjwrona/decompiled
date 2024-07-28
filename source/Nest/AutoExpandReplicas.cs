// Decompiled with JetBrains decompiler
// Type: Nest.AutoExpandReplicas
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  [JsonFormatter(typeof (AutoExpandReplicasFormatter))]
  public class AutoExpandReplicas
  {
    private const string AllMaxReplicas = "all";
    private Union<int?, string> _maxReplicas;
    private int? _minReplicas;

    public static AutoExpandReplicas Disabled { get; } = new AutoExpandReplicas();

    public bool Enabled { get; private set; }

    public Union<int?, string> MaxReplicas
    {
      get => this._maxReplicas;
      private set
      {
        this.Enabled = value != null || this._minReplicas.HasValue;
        this._maxReplicas = value;
      }
    }

    public int? MinReplicas
    {
      get => this._minReplicas;
      private set
      {
        this.Enabled = value.HasValue || this._maxReplicas != null;
        this._minReplicas = value;
      }
    }

    public static AutoExpandReplicas Create(int minReplicas, int maxReplicas)
    {
      if (minReplicas < 0)
        throw new ArgumentException("minReplicas must be greater than or equal to 0", nameof (minReplicas));
      if (maxReplicas < 0)
        throw new ArgumentException("maxReplicas must be greater than or equal to 0", nameof (minReplicas));
      return minReplicas <= maxReplicas ? new AutoExpandReplicas()
      {
        Enabled = true,
        MinReplicas = new int?(minReplicas),
        MaxReplicas = (Union<int?, string>) new int?(maxReplicas)
      } : throw new ArgumentException("minReplicas must be less than or equal to maxReplicas", nameof (minReplicas));
    }

    public static AutoExpandReplicas Create(int minReplicas) => minReplicas >= 0 ? new AutoExpandReplicas()
    {
      Enabled = true,
      MinReplicas = new int?(minReplicas),
      MaxReplicas = (Union<int?, string>) "all"
    } : throw new ArgumentException("minReplicas must be greater than or equal to 0", nameof (minReplicas));

    public static AutoExpandReplicas Create(string value)
    {
      if (value.IsNullOrEmpty())
        throw new ArgumentException("cannot be null or empty", nameof (value));
      if (value.Equals("false", StringComparison.OrdinalIgnoreCase))
        return AutoExpandReplicas.Disabled;
      string[] strArray = value.Split('-');
      if (strArray.Length != 2)
        throw new ArgumentException("must contain a 'from' and 'to' value", nameof (value));
      int result1;
      if (!int.TryParse(strArray[0], out result1))
        throw new FormatException("minReplicas must be an integer");
      int result2 = 0;
      bool flag1 = false;
      bool flag2 = strArray[1] == "all";
      if (!flag2)
        flag1 = int.TryParse(strArray[1], out result2);
      if (!flag1 && !flag2)
        throw new FormatException("minReplicas must be an integer or 'all'");
      return !flag1 ? AutoExpandReplicas.Create(result1) : AutoExpandReplicas.Create(result1, result2);
    }

    public static implicit operator AutoExpandReplicas(string value) => !value.IsNullOrEmpty() ? AutoExpandReplicas.Create(value) : (AutoExpandReplicas) null;

    public override string ToString()
    {
      if (!this.Enabled)
        return "false";
      return string.Join("-", (object) this.MinReplicas, (object) this.MaxReplicas.Match<string>((Func<int?, string>) (i => i.ToString()), (Func<string, string>) (s => s)));
    }
  }
}
