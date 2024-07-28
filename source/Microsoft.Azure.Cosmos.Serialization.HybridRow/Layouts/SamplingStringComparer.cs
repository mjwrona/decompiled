// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts.SamplingStringComparer
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts
{
  internal class SamplingStringComparer : IEqualityComparer<string>
  {
    public static readonly SamplingStringComparer Default = new SamplingStringComparer();

    public bool Equals(string x, string y) => x.Equals(y);

    public int GetHashCode(string obj)
    {
      uint num1 = 5381;
      uint num2 = num1;
      ReadOnlySpan<char> readOnlySpan = obj.AsSpan();
      int num3 = Math.Min(readOnlySpan.Length, 4);
      for (int index = 0; index < num3; ++index)
      {
        uint num4 = (uint) readOnlySpan[index * 13 % readOnlySpan.Length];
        if (index % 2 == 0)
          num1 = (num1 << 5) + num1 ^ num4;
        else
          num2 = (num2 << 5) + num2 ^ num4;
      }
      return (int) num1 + (int) num2 * 1566083941;
    }
  }
}
