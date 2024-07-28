// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.InBinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.OData.UriParser
{
  internal sealed class InBinder
  {
    private readonly Func<QueryToken, QueryNode> bindMethod;

    internal InBinder(Func<QueryToken, QueryNode> bindMethod) => this.bindMethod = bindMethod;

    internal QueryNode BindInOperator(InToken inToken, BindingState state)
    {
      ExceptionUtils.CheckArgumentNotNull<InToken>(inToken, nameof (inToken));
      SingleValueNode operandFromToken1 = this.GetSingleValueOperandFromToken(inToken.Left);
      CollectionNode operandFromToken2 = this.GetCollectionOperandFromToken(inToken.Right, (IEdmTypeReference) new EdmCollectionTypeReference((IEdmCollectionType) new EdmCollectionType(operandFromToken1.TypeReference)), state.Model);
      return (QueryNode) new InNode(operandFromToken1, operandFromToken2);
    }

    private SingleValueNode GetSingleValueOperandFromToken(QueryToken queryToken)
    {
      if (!(this.bindMethod(queryToken) is SingleValueNode operandFromToken))
        throw new ODataException(Microsoft.OData.Strings.MetadataBinder_LeftOperandNotSingleValue);
      return operandFromToken;
    }

    private CollectionNode GetCollectionOperandFromToken(
      QueryToken queryToken,
      IEdmTypeReference expectedType,
      IEdmModel model)
    {
      CollectionNode collectionNode;
      if (queryToken is LiteralToken literalToken)
      {
        string originalText = literalToken.OriginalText;
        string bracketLiteralText = originalText;
        if (bracketLiteralText[0] == '(')
        {
          StringBuilder stringBuilder = !(bracketLiteralText.Replace(" ", string.Empty) == "()") ? new StringBuilder(bracketLiteralText)
          {
            [0] = '['
          } : throw new ODataException(Microsoft.OData.Strings.MetadataBinder_RightOperandNotCollectionValue);
          stringBuilder[stringBuilder.Length - 1] = ']';
          bracketLiteralText = stringBuilder.ToString();
          switch (expectedType.Definition.AsElementType().FullTypeName())
          {
            case "Edm.String":
              bracketLiteralText = InBinder.NormalizeCollectionItems(bracketLiteralText, new InBinder.NormalizeFunction(InBinder.NormalizeStringItem));
              break;
            case "Edm.Guid":
              bracketLiteralText = InBinder.NormalizeCollectionItems(bracketLiteralText, new InBinder.NormalizeFunction(InBinder.NormalizeGuidItem));
              break;
          }
        }
        collectionNode = (CollectionNode) (this.bindMethod((QueryToken) new LiteralToken(ODataUriConversionUtils.ConvertFromCollectionValue(bracketLiteralText, model, expectedType), originalText, expectedType)) as CollectionConstantNode);
      }
      else
        collectionNode = this.bindMethod(queryToken) as CollectionNode;
      return collectionNode != null ? collectionNode : throw new ODataException(Microsoft.OData.Strings.MetadataBinder_RightOperandNotCollectionValue);
    }

    private static string NormalizeCollectionItems(
      string bracketLiteralText,
      InBinder.NormalizeFunction normalizeFunc)
    {
      string[] array = ((IEnumerable<string>) bracketLiteralText.Substring(1, bracketLiteralText.Length - 2).Split(',')).Select<string, string>((Func<string, string>) (s => s.Trim())).ToArray<string>();
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < array.Length; ++index)
      {
        string str = normalizeFunc(array[index]);
        if (index != array.Length - 1)
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0},", new object[1]
          {
            (object) str
          });
        else
          stringBuilder.Append(str);
      }
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]", new object[1]
      {
        (object) stringBuilder.ToString()
      });
    }

    private static string NormalizeStringItem(string str)
    {
      string str1 = str[0] == '\'' && str[str.Length - 1] == '\'' || str[0] == '"' && str[str.Length - 1] == '"' ? str : throw new ODataException(Microsoft.OData.Strings.StringItemShouldBeQuoted((object) str));
      if (str[0] == '\'')
        str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", new object[1]
        {
          (object) UriParserHelper.RemoveQuotes(str)
        });
      return str1;
    }

    private static string NormalizeGuidItem(string guid)
    {
      if (guid[0] == '\'' || guid[0] == '"')
        return guid;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "'{0}'", new object[1]
      {
        (object) guid
      });
    }

    private delegate string NormalizeFunction(string item);
  }
}
