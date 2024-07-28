// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.EachExpressionToken
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens
{
  [DataContract]
  internal sealed class EachExpressionToken : ExpressionToken
  {
    [DataMember(Name = "id", EmitDefaultValue = false)]
    private string m_identifier;
    [DataMember(Name = "each", EmitDefaultValue = false)]
    private string m_expression;

    internal EachExpressionToken(
      int? fileId,
      int? line,
      int? column,
      string identifier,
      string expression)
      : base(8, fileId, line, column, "each")
    {
      this.m_identifier = identifier;
      this.m_expression = expression;
    }

    public override string ToString() => "${{ each " + this.m_identifier + " in " + this.m_expression + " }}";

    internal string Identifier
    {
      get
      {
        if (this.m_identifier == null)
          this.m_identifier = string.Empty;
        return this.m_identifier;
      }
    }

    internal string Expression
    {
      get
      {
        if (this.m_expression == null)
          this.m_expression = string.Empty;
        return this.m_expression;
      }
    }

    internal IList Evaluate(TemplateContext context, out int bytes)
    {
      if (context.ExpressionValues.ContainsKey(this.Identifier))
        throw new InvalidOperationException(YamlStrings.IdentifierAlreadyDefined((object) this.Identifier));
      TemplateToken templateToken1 = this.EvaluateTemplateToken(context, this.Expression, false, out bytes);
      if (templateToken1 == null)
        return (IList) null;
      if (templateToken1 is LiteralToken literalToken)
      {
        context.Error((TemplateToken) this, YamlStrings.ExpectedSequenceOrMappingActual((object) literalToken.Value));
        return (IList) null;
      }
      List<object> objectList = new List<object>();
      if (templateToken1 is SequenceToken sequenceToken)
      {
        foreach (TemplateToken templateToken2 in sequenceToken)
          objectList.Add((object) templateToken2);
      }
      else
      {
        if (!(templateToken1 is MappingToken mappingToken))
          throw new NotSupportedException("Expected each-expression to result in a sequence or a mapping. Actual '" + templateToken1?.GetType().FullName + "'");
        foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mappingToken)
          objectList.Add((object) new EachExpressionToken.Item()
          {
            Key = (keyValuePair.Key as LiteralToken),
            Value = keyValuePair.Value
          });
      }
      return (IList) objectList;
    }

    [DataContract]
    private struct Item
    {
      [DataMember]
      public LiteralToken Key;
      [DataMember]
      public TemplateToken Value;
    }
  }
}
