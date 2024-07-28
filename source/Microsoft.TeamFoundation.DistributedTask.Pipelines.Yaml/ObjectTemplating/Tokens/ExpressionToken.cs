// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.ExpressionToken
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens
{
  [DataContract]
  internal abstract class ExpressionToken : ScalarToken
  {
    internal ExpressionToken(
      int templateType,
      int? fileId,
      int? line,
      int? column,
      string directive)
      : base(templateType, fileId, line, column)
    {
      this.Directive = directive;
    }

    internal string Directive { get; }

    internal static bool IsValidExpression(string expression, out Exception ex)
    {
      bool flag;
      try
      {
        new ExpressionParser().ValidateSyntax(expression, (Microsoft.TeamFoundation.DistributedTask.Expressions.ITraceWriter) null);
        flag = true;
        ex = (Exception) null;
      }
      catch (Exception ex1)
      {
        flag = false;
        ex = ex1;
      }
      return flag;
    }
  }
}
