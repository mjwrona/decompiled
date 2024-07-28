// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.MustacheParsingUtil
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal static class MustacheParsingUtil
  {
    internal static char SafeCharAt(string value, int index) => index >= 0 && index < value.Length ? value[index] : char.MinValue;

    internal static string SafeSubstring(string value, int index, int length) => index >= 0 && length > 0 && index + length <= value.Length ? value.Substring(index, length) : "";

    internal static bool IsConstantAt(string value, int index, string constantValue)
    {
      if (string.Equals(MustacheParsingUtil.SafeSubstring(value, index, constantValue.Length), constantValue, StringComparison.OrdinalIgnoreCase))
      {
        switch (MustacheParsingUtil.SafeCharAt(value, index + constantValue.Length))
        {
          case char.MinValue:
          case ' ':
            return true;
        }
      }
      return false;
    }

    internal static void EvaluateJToken(
      MustacheTextWriter writer,
      JArray token,
      MustacheEvaluationContext context,
      MustacheTemplatedExpression expression)
    {
      for (int index = 0; index < token.Count; ++index)
      {
        MustacheEvaluationContext context1 = new MustacheEvaluationContext()
        {
          ReplacementObject = (object) token[index],
          ParentContext = context,
          CurrentIndex = index,
          ParentItemsCount = token.Count,
          PartialExpressions = context.PartialExpressions,
          AdditionalEvaluationData = context.AdditionalEvaluationData,
          Options = context.Options
        };
        MustacheEvaluationContext.CombinePartialsDictionaries(context1, (MustacheAggregateExpression) expression);
        expression.EvaluateChildExpressions(context1, writer);
      }
    }

    internal static void AssertDepth(MustacheOptions options, int depth)
    {
      if (options.MaxDepth > 0 && depth > options.MaxDepth)
        throw new MustacheExpressionInvalidException(WebApiResources.MustacheTemplateMaxDepthExceeded((object) options.MaxDepth));
    }
  }
}
