// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpressionOperatorExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal static class QueryExpressionOperatorExtensions
  {
    internal static bool IsEverable(this QueryExpressionOperator op) => op == QueryExpressionOperator.Ever || op == QueryExpressionOperator.EverContains || op == QueryExpressionOperator.EverContainsWords || op == QueryExpressionOperator.EverFTContains || op == QueryExpressionOperator.NeverContains || op == QueryExpressionOperator.NeverContainsWords || op == QueryExpressionOperator.NeverFTContains;

    internal static bool IsNegated(this QueryExpressionOperator op) => op == QueryExpressionOperator.NeverContains || op == QueryExpressionOperator.NeverContainsWords || op == QueryExpressionOperator.NeverFTContains || op == QueryExpressionOperator.NotContains || op == QueryExpressionOperator.NotContainsWords || op == QueryExpressionOperator.NotEquals || op == QueryExpressionOperator.NotFTContains || op == QueryExpressionOperator.NotUnder || op == QueryExpressionOperator.NotIn || op == QueryExpressionOperator.IsNotEmpty;

    internal static bool IsContains(this QueryExpressionOperator op) => op == QueryExpressionOperator.Contains || op == QueryExpressionOperator.EverContains || op == QueryExpressionOperator.NeverContains || op == QueryExpressionOperator.NotContains || op.IsContainsWords() || op.IsFullTextContains();

    internal static bool IsContainsWords(this QueryExpressionOperator op) => op == QueryExpressionOperator.ContainsWords || op == QueryExpressionOperator.EverContainsWords || op == QueryExpressionOperator.NeverContainsWords || op == QueryExpressionOperator.NotContainsWords;

    internal static bool IsFullTextContains(this QueryExpressionOperator op) => op == QueryExpressionOperator.EverFTContains || op == QueryExpressionOperator.FTContains || op == QueryExpressionOperator.NeverFTContains || op == QueryExpressionOperator.NotFTContains;

    internal static bool IsUnder(this QueryExpressionOperator op) => op == QueryExpressionOperator.Under || op == QueryExpressionOperator.NotUnder;

    internal static bool IsEquals(this QueryExpressionOperator op) => op == QueryExpressionOperator.Equals || op == QueryExpressionOperator.NotEquals;

    internal static bool UsesIsEmpty(this QueryExpressionOperator op) => op == QueryExpressionOperator.IsEmpty || op == QueryExpressionOperator.IsNotEmpty;
  }
}
