// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.FilterBinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;

namespace Microsoft.OData.UriParser
{
  internal sealed class FilterBinder
  {
    private readonly MetadataBinder.QueryTokenVisitor bindMethod;
    private readonly BindingState state;

    internal FilterBinder(MetadataBinder.QueryTokenVisitor bindMethod, BindingState state)
    {
      this.bindMethod = bindMethod;
      this.state = state;
    }

    internal FilterClause BindFilter(QueryToken filter)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryToken>(filter, nameof (filter));
      if (!(this.bindMethod(filter) is SingleValueNode expression) || expression.TypeReference != null && !expression.TypeReference.IsODataPrimitiveTypeKind())
        throw new ODataException(Microsoft.OData.Strings.MetadataBinder_FilterExpressionNotSingleValue);
      IEdmTypeReference typeReference = expression.TypeReference;
      if (typeReference != null)
      {
        IEdmPrimitiveTypeReference type = typeReference.AsPrimitiveOrNull();
        if (type == null || ExtensionMethods.PrimitiveKind(type) != EdmPrimitiveTypeKind.Boolean)
          throw new ODataException(Microsoft.OData.Strings.MetadataBinder_FilterExpressionNotSingleValue);
      }
      return new FilterClause(expression, this.state.ImplicitRangeVariable);
    }
  }
}
