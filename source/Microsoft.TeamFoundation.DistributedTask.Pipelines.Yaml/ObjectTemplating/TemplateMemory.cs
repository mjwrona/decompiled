// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.TemplateMemory
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating
{
  internal sealed class TemplateMemory
  {
    internal const int MinObjectSize = 24;
    internal const int StringBaseOverhead = 26;
    private readonly int m_maxDepth;
    private readonly int m_maxEvents;
    private readonly int m_maxBytes;
    private int m_depth;
    private int m_greatestDepth;
    private int m_events;
    private int m_currentBytes;
    private int m_greatestBytes;
    private int m_greatestFileSize;

    internal TemplateMemory(int maxDepth, int maxEvents, int maxBytes)
    {
      this.m_maxDepth = maxDepth;
      this.m_maxEvents = maxEvents;
      this.m_maxBytes = maxBytes;
    }

    public int CurrentBytes => this.m_currentBytes;

    public int GreatestBytes => this.m_greatestBytes;

    public int GreatestDepth => this.m_greatestDepth;

    public int GreatestFileSize => this.m_greatestFileSize;

    public int MaxBytes => this.m_maxBytes;

    public int EventCount => this.m_events;

    internal void AddBytes(int bytes)
    {
      checked { this.m_currentBytes += bytes; }
      if (this.m_currentBytes > this.m_greatestBytes)
        this.m_greatestBytes = this.m_currentBytes;
      if (this.m_currentBytes > this.m_maxBytes)
        throw new InvalidOperationException(YamlStrings.MaxObjectSizeExceeded());
    }

    internal void AddBytes(string value) => this.AddBytes(this.CalculateBytes(value));

    internal void AddBytes(JToken value, bool traverse) => this.AddBytes(this.CalculateBytes(value, traverse));

    internal void AddBytes(TemplateToken value, bool traverse = false) => this.AddBytes(this.CalculateBytes(value, traverse));

    internal void AddBytes(LiteralToken literal) => this.AddBytes(this.CalculateBytes(literal));

    internal void AddBytes(SequenceToken sequence) => this.AddBytes(this.CalculateBytes(sequence, false));

    internal void AddBytes(MappingToken mapping) => this.AddBytes(this.CalculateBytes(mapping, false));

    internal void AddBytes(BasicExpressionToken basicExpression) => this.AddBytes(this.CalculateBytes(basicExpression));

    internal void AddBytes(IfExpressionToken ifExpression) => this.AddBytes(this.CalculateBytes(ifExpression));

    internal void AddBytes(InsertExpressionToken insertExpression) => this.AddBytes(this.CalculateBytes(insertExpression));

    internal void AddBytes(EachExpressionToken eachExpression) => this.AddBytes(this.CalculateBytes(eachExpression));

    internal void AddFileSize(int length)
    {
      if (length <= this.m_greatestFileSize)
        return;
      this.m_greatestFileSize = length;
    }

    internal int CalculateBytes(string value) => checked (26 + (unchecked (value != null) ? value.Length : 0) * 2);

    internal int CalculateBytes(JToken value, bool traverse)
    {
      if (value == null)
        return 24;
      if (!traverse)
      {
        switch (value.Type)
        {
          case JTokenType.Object:
          case JTokenType.Array:
          case JTokenType.Integer:
          case JTokenType.Float:
          case JTokenType.Boolean:
          case JTokenType.Null:
            return 24;
          case JTokenType.Property:
            string name = (value as JProperty).Name;
            return checked (26 + (unchecked (name != null) ? name.Length : 0) * 2);
          case JTokenType.String:
            return checked (26 + value.ToObject<string>().Length * 2);
          default:
            throw new NotSupportedException(string.Format("Unexpected JToken type '{0}' when traversing object", (object) value.Type));
        }
      }
      else
      {
        int bytes1 = 0;
        do
        {
          int bytes2 = this.CalculateBytes(value, false);
          checked { bytes1 += bytes2; }
          if (value.HasValues)
          {
            value = value.First;
          }
          else
          {
            do
            {
              JToken next = value.Next;
              if (next != null)
              {
                value = next;
                break;
              }
              value = (JToken) value.Parent;
            }
            while (value != null);
          }
        }
        while (value != null);
        return bytes1;
      }
    }

    internal int CalculateBytes(TemplateToken value, bool traverse = false)
    {
      switch (value.Type)
      {
        case 0:
          return this.CalculateBytes(value as LiteralToken);
        case 1:
          return this.CalculateBytes(value as SequenceToken, traverse);
        case 2:
          return this.CalculateBytes(value as MappingToken, traverse);
        case 3:
          return this.CalculateBytes(value as BasicExpressionToken);
        case 4:
          return this.CalculateBytes(value as InsertExpressionToken);
        case 5:
          return this.CalculateBytes(value as IfExpressionToken);
        case 6:
          return this.CalculateBytes(value as ElseIfExpressionToken);
        case 7:
          return this.CalculateBytes(value as ElseExpressionToken);
        case 8:
          return this.CalculateBytes(value as EachExpressionToken);
        default:
          throw new NotSupportedException(string.Format("Unexpected template type '{0}'", (object) value.Type));
      }
    }

    internal int CalculateBytes(LiteralToken literal)
    {
      string str = literal.Value;
      return checked (50 + (unchecked (str != null) ? str.Length : 0) * 2);
    }

    internal int CalculateBytes(SequenceToken sequence, bool traverse = false)
    {
      int bytes1 = 24;
      if (traverse && sequence.Count > 0)
      {
        foreach (TemplateToken templateToken in sequence)
        {
          int bytes2 = this.CalculateBytes(templateToken, true);
          checked { bytes1 += bytes2; }
        }
      }
      return bytes1;
    }

    internal int CalculateBytes(MappingToken mapping, bool traverse = false)
    {
      int bytes1 = 24;
      if (traverse && mapping.Count > 0)
      {
        foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mapping)
        {
          int bytes2 = this.CalculateBytes((TemplateToken) keyValuePair.Key, true);
          int bytes3 = this.CalculateBytes(keyValuePair.Value, true);
          checked { bytes1 += bytes2 + bytes3; }
        }
      }
      return bytes1;
    }

    internal int CalculateBytes(BasicExpressionToken basicExpression)
    {
      string expression = basicExpression.Expression;
      return checked (50 + (unchecked (expression != null) ? expression.Length : 0) * 2);
    }

    internal int CalculateBytes(IfExpressionToken ifExpression)
    {
      string expression = ifExpression.Expression;
      return checked (50 + (unchecked (expression != null) ? expression.Length : 0) * 2);
    }

    internal int CalculateBytes(ElseExpressionToken elseExpression) => 24;

    internal int CalculateBytes(ElseIfExpressionToken elseIfExpression)
    {
      string expression = elseIfExpression.Expression;
      return checked (50 + (unchecked (expression != null) ? expression.Length : 0) * 2);
    }

    internal int CalculateBytes(InsertExpressionToken insertExpression) => 24;

    internal int CalculateBytes(EachExpressionToken eachExpression)
    {
      string identifier = eachExpression.Identifier;
      int num1 = checked (50 + (unchecked (identifier != null) ? identifier.Length : 0) * 2 + 26);
      string expression = eachExpression.Expression;
      int num2 = checked (unchecked (expression != null) ? expression.Length : 0 * 2);
      return checked (num1 + num2);
    }

    internal void SubtractBytes(int bytes)
    {
      if (bytes > this.m_currentBytes)
        throw new InvalidOperationException("Bytes to subtract exceeds total bytes");
      this.m_currentBytes -= bytes;
    }

    internal void SubtractBytes(JToken value, bool traverse) => this.SubtractBytes(this.CalculateBytes(value, traverse));

    internal void SubtractBytes(TemplateToken value, bool traverse = false) => this.SubtractBytes(this.CalculateBytes(value, traverse));

    internal void SubtractBytes(LiteralToken literal) => this.SubtractBytes(this.CalculateBytes(literal));

    internal void SubtractBytes(SequenceToken sequence, bool traverse = false) => this.SubtractBytes(this.CalculateBytes(sequence, traverse));

    internal void SubtractBytes(MappingToken mapping, bool traverse = false) => this.SubtractBytes(this.CalculateBytes(mapping, traverse));

    internal void SubtractBytes(BasicExpressionToken basicExpression) => this.SubtractBytes(this.CalculateBytes(basicExpression));

    internal void SubtractBytes(IfExpressionToken ifExpression) => this.SubtractBytes(this.CalculateBytes(ifExpression));

    internal void SubtractBytes(InsertExpressionToken insertExpression) => this.SubtractBytes(this.CalculateBytes(insertExpression));

    internal void SubtractBytes(EachExpressionToken eachExpression) => this.SubtractBytes(this.CalculateBytes(eachExpression));

    internal void IncrementDepth()
    {
      ++this.m_depth;
      if (this.m_depth > this.m_greatestDepth)
        this.m_greatestDepth = this.m_depth;
      if (this.m_depth > this.m_maxDepth)
        throw new InvalidOperationException(YamlStrings.MaxObjectDepthExceeded());
    }

    internal void DecrementDepth() => --this.m_depth;

    internal void IncrementEvents()
    {
      if (this.m_events++ >= this.m_maxEvents)
        throw new InvalidOperationException(YamlStrings.MaxTemplateEventsExceeded());
    }
  }
}
