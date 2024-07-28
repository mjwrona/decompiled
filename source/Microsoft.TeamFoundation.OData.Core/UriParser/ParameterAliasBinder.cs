// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ParameterAliasBinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;

namespace Microsoft.OData.UriParser
{
  internal sealed class ParameterAliasBinder
  {
    private readonly MetadataBinder.QueryTokenVisitor bindMethod;

    internal ParameterAliasBinder(MetadataBinder.QueryTokenVisitor bindMethod)
    {
      ExceptionUtils.CheckArgumentNotNull<MetadataBinder.QueryTokenVisitor>(bindMethod, nameof (bindMethod));
      this.bindMethod = bindMethod;
    }

    internal ParameterAliasNode BindParameterAlias(
      BindingState bindingState,
      FunctionParameterAliasToken aliasToken)
    {
      ExceptionUtils.CheckArgumentNotNull<BindingState>(bindingState, nameof (bindingState));
      ExceptionUtils.CheckArgumentNotNull<FunctionParameterAliasToken>(aliasToken, nameof (aliasToken));
      string alias = aliasToken.Alias;
      ParameterAliasValueAccessor aliasValueAccessor = bindingState.Configuration.ParameterAliasValueAccessor;
      if (aliasValueAccessor == null)
        return new ParameterAliasNode(alias, (IEdmTypeReference) null);
      SingleValueNode segment = (SingleValueNode) null;
      if (!aliasValueAccessor.ParameterAliasValueNodesCached.TryGetValue(alias, out segment))
      {
        string aliasValueExpression = aliasValueAccessor.GetAliasValueExpression(alias);
        if (aliasValueExpression == null)
        {
          aliasValueAccessor.ParameterAliasValueNodesCached[alias] = (SingleValueNode) null;
        }
        else
        {
          segment = this.ParseAndBindParameterAliasValueExpression(bindingState, aliasValueExpression, aliasToken.ExpectedParameterType);
          aliasValueAccessor.ParameterAliasValueNodesCached[alias] = segment;
        }
      }
      return new ParameterAliasNode(alias, segment.GetEdmTypeReference());
    }

    private SingleValueNode ParseAndBindParameterAliasValueExpression(
      BindingState bindingState,
      string aliasValueExpression,
      IEdmTypeReference parameterType)
    {
      if (!(this.bindMethod(ParameterAliasBinder.ParseComplexOrCollectionAlias(new UriQueryExpressionParser(bindingState.Configuration.Settings.FilterLimit).ParseExpressionText(aliasValueExpression), parameterType, bindingState.Model)) is SingleValueNode aliasValueExpression1))
        throw new ODataException("ODataErrorStrings.MetadataBinder_ParameterAliasValueExpressionNotSingleValue");
      return aliasValueExpression1;
    }

    private static QueryToken ParseComplexOrCollectionAlias(
      QueryToken queryToken,
      IEdmTypeReference parameterType,
      IEdmModel model)
    {
      if (queryToken is LiteralToken literalToken && literalToken.Value is string str && !string.IsNullOrEmpty(literalToken.OriginalText))
      {
        ExpressionLexer expressionLexer = new ExpressionLexer(literalToken.OriginalText, true, false, true);
        if (expressionLexer.CurrentToken.Kind == ExpressionTokenKind.BracketedExpression || expressionLexer.CurrentToken.Kind == ExpressionTokenKind.BracedExpression)
        {
          object obj = (object) str;
          if (!parameterType.IsStructured() && !parameterType.IsStructuredCollectionType())
            obj = ODataUriUtils.ConvertFromUriLiteral(str, ODataVersion.V4, model, parameterType);
          return (QueryToken) new LiteralToken(obj, literalToken.OriginalText, parameterType);
        }
      }
      return queryToken;
    }
  }
}
