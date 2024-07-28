// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ExpressionValue
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class ExpressionValue
  {
    public static bool IsExpression(string value) => !string.IsNullOrEmpty(value) && value.Length > 3 && value.StartsWith("$[", StringComparison.Ordinal) && value.EndsWith("]", StringComparison.Ordinal);

    public static bool TryParse<T>(string expression, out ExpressionValue<T> value)
    {
      value = !ExpressionValue.IsExpression(expression) ? (ExpressionValue<T>) null : new ExpressionValue<T>(expression, true);
      return value != (ExpressionValue<T>) null;
    }

    public static ExpressionValue<T> FromExpression<T>(string expression) => new ExpressionValue<T>(expression, true);

    public static ExpressionValue<T> FromLiteral<T>(T literal) => new ExpressionValue<T>(literal);

    public static ExpressionValue<string> FromToken(string token) => ExpressionValue.IsExpression(token) ? ExpressionValue.FromExpression<string>(token) : ExpressionValue.FromLiteral<string>(token);

    internal static string TrimExpression(string value)
    {
      string str = value.Substring(2, value.Length - 3).Trim();
      return !string.IsNullOrEmpty(str) ? str : throw new ArgumentException(PipelineStrings.ExpressionInvalid((object) value));
    }
  }
}
