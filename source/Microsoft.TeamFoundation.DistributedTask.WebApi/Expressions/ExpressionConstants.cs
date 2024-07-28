// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.ExpressionConstants
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Expressions;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  internal static class ExpressionConstants
  {
    internal static readonly string DateTimeFormat = "yyyy\\-MM\\-dd\\ HH\\:mm\\:sszzz";
    internal static readonly int MaxDepth = 50;
    internal static readonly int MaxLength = 21000;
    internal static readonly string NumberFormat = "0.#######";
    internal static readonly Dictionary<string, IFunctionInfo> WellKnownFunctions = new Dictionary<string, IFunctionInfo>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    internal const char StartIndex = '[';
    internal const char StartParameter = '(';
    internal const char EndIndex = ']';
    internal const char EndParameter = ')';
    internal const char Separator = ',';
    internal const char Dereference = '.';
    internal const char Wildcard = '*';

    static ExpressionConstants()
    {
      ExpressionConstants.AddFunction<AndNode>("and", 2, int.MaxValue);
      ExpressionConstants.AddFunction<CoalesceNode>("coalesce", 2, int.MaxValue);
      ExpressionConstants.AddFunction<ContainsNode>("contains", 2, 2);
      ExpressionConstants.AddFunction<ContainsValueNode>("containsValue", 2, 2);
      ExpressionConstants.AddFunction<ConvertToJsonNode>("convertToJson", 1, 1);
      ExpressionConstants.AddFunction<EndsWithNode>("endsWith", 2, 2);
      ExpressionConstants.AddFunction<EqualNode>("eq", 2, 2);
      ExpressionConstants.AddFunction<FormatNode>("format", 1, (int) byte.MaxValue);
      ExpressionConstants.AddFunction<GreaterThanNode>("gt", 2, 2);
      ExpressionConstants.AddFunction<GreaterThanOrEqualNode>("ge", 2, 2);
      ExpressionConstants.AddFunction<JoinNode>("join", 2, 2);
      ExpressionConstants.AddFunction<LengthNode>("length", LengthNode.minParameters, LengthNode.maxParameters);
      ExpressionConstants.AddFunction<LessThanNode>("lt", 2, 2);
      ExpressionConstants.AddFunction<LessThanOrEqualNode>("le", 2, 2);
      ExpressionConstants.AddFunction<LowerNode>("lower", 1, 1);
      ExpressionConstants.AddFunction<InNode>("in", 2, int.MaxValue);
      ExpressionConstants.AddFunction<NotNode>("not", 1, 1);
      ExpressionConstants.AddFunction<NotEqualNode>("ne", 2, 2);
      ExpressionConstants.AddFunction<NotInNode>("notIn", 2, int.MaxValue);
      ExpressionConstants.AddFunction<OrNode>("or", 2, int.MaxValue);
      ExpressionConstants.AddFunction<ReplaceNode>("replace", 2, 3);
      ExpressionConstants.AddFunction<SplitNode>("split", 2, 2);
      ExpressionConstants.AddFunction<StartsWithNode>("startsWith", 2, 2);
      ExpressionConstants.AddFunction<UpperNode>("upper", 1, 1);
      ExpressionConstants.AddFunction<XorNode>("xor", 2, 2);
    }

    private static void AddFunction<T>(string name, int minParameters, int maxParameters) where T : FunctionNode, new() => ExpressionConstants.WellKnownFunctions.Add(name, (IFunctionInfo) new FunctionInfo<T>(name, minParameters, maxParameters));
  }
}
