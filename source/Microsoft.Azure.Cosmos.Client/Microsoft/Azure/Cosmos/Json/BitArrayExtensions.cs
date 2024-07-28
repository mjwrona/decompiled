// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Json.BitArrayExtensions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections;

namespace Microsoft.Azure.Cosmos.Json
{
  internal static class BitArrayExtensions
  {
    public static bool SetEquals(this BitArray first, BitArray second)
    {
      if (first.Length != second.Length)
        return false;
      for (int index = 0; index < first.Length; ++index)
      {
        if (first[index] != second[index])
          return false;
      }
      return true;
    }

    public static bool IsSubset(this BitArray first, BitArray second)
    {
      if (first.Length != second.Length)
        return false;
      for (int index = 0; index < first.Length; ++index)
      {
        if (first[index] && !second[index])
          return false;
      }
      return true;
    }
  }
}
