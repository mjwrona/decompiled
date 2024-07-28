// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.UriTemplateParser
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.OData.UriParser
{
  internal sealed class UriTemplateParser
  {
    internal static bool IsValidTemplateLiteral(string literalText) => !string.IsNullOrEmpty(literalText) && literalText.StartsWith("{", StringComparison.Ordinal) && literalText.EndsWith("}", StringComparison.Ordinal);

    internal static bool TryParseLiteral(
      string literalText,
      IEdmTypeReference expectedType,
      out UriTemplateExpression expression)
    {
      if (UriTemplateParser.IsValidTemplateLiteral(literalText))
      {
        expression = new UriTemplateExpression()
        {
          LiteralText = literalText,
          ExpectedType = expectedType
        };
        return true;
      }
      expression = (UriTemplateExpression) null;
      return false;
    }
  }
}
