// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Query.SelectionClause
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

namespace Microsoft.Cloud.Metrics.Client.Query
{
  public sealed class SelectionClause
  {
    public static readonly SelectionClause AllResults = new SelectionClause(SelectionType.Undefined, 0, OrderBy.Undefined);

    public SelectionClause(SelectionType selectionType, int quantityToSelect, OrderBy orderBy)
    {
      this.SelectionType = selectionType;
      this.QuantityToSelect = quantityToSelect;
      this.OrderBy = orderBy;
    }

    public SelectionType SelectionType { get; private set; }

    public int QuantityToSelect { get; private set; }

    public OrderBy OrderBy { get; private set; }

    public override bool Equals(object obj) => this.Equals(obj as SelectionClause);

    public override int GetHashCode() => this.SelectionType.GetHashCode() ^ this.QuantityToSelect.GetHashCode() ^ this.OrderBy.GetHashCode();

    private bool Equals(SelectionClause otherClause) => otherClause != null && this.SelectionType.Equals((object) otherClause.SelectionType) && this.QuantityToSelect.Equals(otherClause.QuantityToSelect) && this.OrderBy == otherClause.OrderBy;
  }
}
