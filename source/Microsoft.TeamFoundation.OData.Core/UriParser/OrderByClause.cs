// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.OrderByClause
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class OrderByClause
  {
    private readonly SingleValueNode expression;
    private readonly OrderByDirection direction;
    private readonly RangeVariable rangeVariable;
    private readonly OrderByClause thenBy;

    public OrderByClause(
      OrderByClause thenBy,
      SingleValueNode expression,
      OrderByDirection direction,
      RangeVariable rangeVariable)
    {
      ExceptionUtils.CheckArgumentNotNull<SingleValueNode>(expression, nameof (expression));
      ExceptionUtils.CheckArgumentNotNull<RangeVariable>(rangeVariable, "parameter");
      this.thenBy = thenBy;
      this.expression = expression;
      this.direction = direction;
      this.rangeVariable = rangeVariable;
    }

    public OrderByClause ThenBy => this.thenBy;

    public SingleValueNode Expression => this.expression;

    public OrderByDirection Direction => this.direction;

    public RangeVariable RangeVariable => this.rangeVariable;

    public IEdmTypeReference ItemType => this.RangeVariable.TypeReference;
  }
}
