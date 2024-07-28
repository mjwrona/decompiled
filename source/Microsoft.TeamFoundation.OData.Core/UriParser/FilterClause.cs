// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.FilterClause
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class FilterClause
  {
    private readonly SingleValueNode expression;
    private readonly RangeVariable rangeVariable;

    public FilterClause(SingleValueNode expression, RangeVariable rangeVariable)
    {
      ExceptionUtils.CheckArgumentNotNull<SingleValueNode>(expression, nameof (expression));
      ExceptionUtils.CheckArgumentNotNull<RangeVariable>(rangeVariable, "parameter");
      this.expression = expression;
      this.rangeVariable = rangeVariable;
    }

    public SingleValueNode Expression => this.expression;

    public RangeVariable RangeVariable => this.rangeVariable;

    public IEdmTypeReference ItemType => this.RangeVariable.TypeReference;
  }
}
