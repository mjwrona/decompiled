// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.LiteralBinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  internal sealed class LiteralBinder
  {
    internal static QueryNode BindLiteral(LiteralToken literalToken)
    {
      ExceptionUtils.CheckArgumentNotNull<LiteralToken>(literalToken, nameof (literalToken));
      if (string.IsNullOrEmpty(literalToken.OriginalText))
        return (QueryNode) new ConstantNode(literalToken.Value);
      return literalToken.ExpectedEdmTypeReference != null ? (QueryNode) new ConstantNode(literalToken.Value, literalToken.OriginalText, literalToken.ExpectedEdmTypeReference) : (QueryNode) new ConstantNode(literalToken.Value, literalToken.OriginalText);
    }

    internal static QueryNode BindInLiteral(LiteralToken literalToken)
    {
      ExceptionUtils.CheckArgumentNotNull<LiteralToken>(literalToken, nameof (literalToken));
      if (string.IsNullOrEmpty(literalToken.OriginalText))
        return (QueryNode) new ConstantNode(literalToken.Value);
      if (literalToken.ExpectedEdmTypeReference == null)
        return (QueryNode) new ConstantNode(literalToken.Value, literalToken.OriginalText);
      return literalToken.ExpectedEdmTypeReference is IEdmCollectionTypeReference edmTypeReference && literalToken.Value is ODataCollectionValue odataCollectionValue ? (QueryNode) new CollectionConstantNode(odataCollectionValue.Items, literalToken.OriginalText, edmTypeReference) : (QueryNode) new ConstantNode(literalToken.Value, literalToken.OriginalText, literalToken.ExpectedEdmTypeReference);
    }
  }
}
