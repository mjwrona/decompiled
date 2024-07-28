// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.OrderByBinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  internal sealed class OrderByBinder
  {
    private readonly MetadataBinder.QueryTokenVisitor bindMethod;

    internal OrderByBinder(MetadataBinder.QueryTokenVisitor bindMethod)
    {
      ExceptionUtils.CheckArgumentNotNull<MetadataBinder.QueryTokenVisitor>(bindMethod, nameof (bindMethod));
      this.bindMethod = bindMethod;
    }

    internal OrderByClause BindOrderBy(BindingState state, IEnumerable<OrderByToken> orderByTokens)
    {
      ExceptionUtils.CheckArgumentNotNull<BindingState>(state, nameof (state));
      ExceptionUtils.CheckArgumentNotNull<IEnumerable<OrderByToken>>(orderByTokens, nameof (orderByTokens));
      OrderByClause thenBy = (OrderByClause) null;
      foreach (OrderByToken orderByToken in orderByTokens.Reverse<OrderByToken>())
        thenBy = this.ProcessSingleOrderBy(state, thenBy, orderByToken);
      return thenBy;
    }

    private OrderByClause ProcessSingleOrderBy(
      BindingState state,
      OrderByClause thenBy,
      OrderByToken orderByToken)
    {
      ExceptionUtils.CheckArgumentNotNull<BindingState>(state, nameof (state));
      ExceptionUtils.CheckArgumentNotNull<OrderByToken>(orderByToken, nameof (orderByToken));
      if (!(this.bindMethod(orderByToken.Expression) is SingleValueNode expression) || expression.TypeReference != null && !expression.TypeReference.IsODataPrimitiveTypeKind() && !expression.TypeReference.IsODataEnumTypeKind() && !expression.TypeReference.IsODataTypeDefinitionTypeKind())
        throw new ODataException(Strings.MetadataBinder_OrderByExpressionNotSingleValue);
      return new OrderByClause(thenBy, expression, orderByToken.Direction, state.ImplicitRangeVariable);
    }
  }
}
