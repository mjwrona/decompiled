// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Query.QueryFilter
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

namespace Microsoft.Cloud.Metrics.Client.Query
{
  public sealed class QueryFilter
  {
    public static readonly QueryFilter NoFilter = new QueryFilter(Operator.Undefined, 0.0);

    public QueryFilter(Operator @operator, double operand)
    {
      this.Operator = @operator;
      this.Operand = operand;
    }

    public Operator Operator { get; private set; }

    public double Operand { get; private set; }

    public override string ToString() => string.Format("{0} {1}", (object) this.Operator, (object) this.Operand);

    public override bool Equals(object obj) => this.Equals(obj as QueryFilter);

    public override int GetHashCode() => this.Operator.GetHashCode() ^ this.Operand.GetHashCode();

    private bool Equals(QueryFilter otherFilter) => otherFilter != null && this.Operator.Equals((object) otherFilter.Operator) && this.Operand == otherFilter.Operand;
  }
}
