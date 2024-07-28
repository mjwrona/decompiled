// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.SamplingUtf8StringComparer
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  internal class SamplingUtf8StringComparer : IEqualityComparer<Utf8String>
  {
    public static readonly SamplingUtf8StringComparer Default = new SamplingUtf8StringComparer();

    public bool Equals(Utf8String x, Utf8String y)
    {
      Utf8Span span = x.Span;
      return ((Utf8Span) ref span).Equals(y.Span);
    }

    public int GetHashCode(Utf8String obj)
    {
      uint num1 = 5381;
      uint num2 = num1;
      Utf8Span span1 = obj.Span;
      ReadOnlySpan<byte> span2 = ((Utf8Span) ref span1).Span;
      int num3 = Math.Min(span2.Length, 4);
      for (int index = 0; index < num3; ++index)
      {
        uint num4 = (uint) span2[index * 13 % span2.Length];
        if (index % 2 == 0)
          num1 = (num1 << 5) + num1 ^ num4;
        else
          num2 = (num2 << 5) + num2 ^ num4;
      }
      return (int) num1 + (int) num2 * 1566083941;
    }
  }
}
