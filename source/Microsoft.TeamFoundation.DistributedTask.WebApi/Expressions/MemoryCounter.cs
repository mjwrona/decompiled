// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.MemoryCounter
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MemoryCounter
  {
    internal const int MinObjectSize = 24;
    internal const int StringBaseOverhead = 26;
    private readonly int m_maxBytes;
    private readonly ExpressionNode m_node;
    private int m_currentBytes;

    internal MemoryCounter(ExpressionNode node, int? maxBytes)
    {
      this.m_node = node;
      this.m_maxBytes = maxBytes.GetValueOrDefault() > 0 ? maxBytes.Value : int.MaxValue;
    }

    public int CurrentBytes => this.m_currentBytes;

    public void Add(int amount)
    {
      if (!this.TryAdd(amount))
        throw new InvalidOperationException(ExpressionResources.ExceededAllowedMemory((object) this.m_node?.ConvertToExpression()));
    }

    public void Add(string value) => this.Add(MemoryCounter.CalculateSize(value));

    public void Add(JToken value, bool traverse)
    {
      if (value == null)
        this.Add(24);
      if (!traverse)
      {
        switch (value.Type)
        {
          case JTokenType.Property:
            this.Add((value as JProperty).Name);
            break;
          case JTokenType.String:
          case JTokenType.Bytes:
          case JTokenType.Uri:
            this.Add(value.ToObject<string>());
            break;
          default:
            this.Add(24);
            break;
        }
      }
      else
      {
        do
        {
          this.Add(value, false);
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
      }
    }

    public void AddMinObjectSize() => this.Add(24);

    public void Remove(string value) => this.m_currentBytes -= MemoryCounter.CalculateSize(value);

    public static int CalculateSize(string value) => checked (26 + (unchecked (value != null) ? value.Length : 0) * 2);

    internal bool TryAdd(int amount)
    {
      try
      {
        checked { amount += this.m_currentBytes; }
        if (amount > this.m_maxBytes)
          return false;
        this.m_currentBytes = amount;
        return true;
      }
      catch (OverflowException ex)
      {
        return false;
      }
    }

    internal bool TryAdd(string value) => this.TryAdd(MemoryCounter.CalculateSize(value));
  }
}
