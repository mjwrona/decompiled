// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.EvaluationMemory
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  internal sealed class EvaluationMemory
  {
    private const int c_minObjectSize = 24;
    private const int c_stringBaseOverhead = 26;
    private readonly List<int> m_depths = new List<int>();
    private readonly int m_maxAmount;
    private readonly ExpressionNode m_node;
    private int m_maxActiveDepth = -1;
    private int m_totalAmount;

    internal EvaluationMemory(int maxBytes, ExpressionNode node)
    {
      this.m_maxAmount = maxBytes;
      this.m_node = node;
    }

    internal void AddAmount(int depth, int bytes, bool trimDepth = false)
    {
      if (trimDepth)
      {
        for (; this.m_maxActiveDepth > depth; --this.m_maxActiveDepth)
        {
          int depth1 = this.m_depths[this.m_maxActiveDepth];
          if (depth1 > 0)
          {
            if (depth1 > this.m_totalAmount)
              throw new InvalidOperationException("Bytes to subtract exceeds total bytes");
            checked { this.m_totalAmount -= depth1; }
            this.m_depths[this.m_maxActiveDepth] = 0;
          }
        }
      }
      if (depth > this.m_maxActiveDepth)
      {
        while (this.m_depths.Count <= depth)
          this.m_depths.Add(0);
        this.m_maxActiveDepth = depth;
      }
      checked { this.m_depths[depth] += bytes; }
      checked { this.m_totalAmount += bytes; }
      if (this.m_totalAmount > this.m_maxAmount)
        throw new InvalidOperationException(ExpressionResources.ExceededAllowedMemory((object) this.m_node?.ConvertToExpression()));
    }

    internal static int CalculateBytes(object obj) => obj is string str ? checked (26 + (unchecked (str != null) ? str.Length : 0) * 2) : 24;
  }
}
