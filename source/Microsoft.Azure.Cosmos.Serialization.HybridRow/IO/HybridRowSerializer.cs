// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.IO.HybridRowSerializer
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.IO
{
  public static class HybridRowSerializer
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HybridRowSerializer.EqualityReferenceResult EqualityReferenceCheck<T>(T x, T y)
    {
      if (typeof (T).IsValueType)
        return HybridRowSerializer.EqualityReferenceResult.Unknown;
      if ((object) x == (object) y)
        return HybridRowSerializer.EqualityReferenceResult.Equal;
      return (object) x == null || (object) y == null || x.GetType() != y.GetType() ? HybridRowSerializer.EqualityReferenceResult.NotEqual : HybridRowSerializer.EqualityReferenceResult.Unknown;
    }

    public enum EqualityReferenceResult
    {
      Unknown = -1, // 0xFFFFFFFF
      NotEqual = 0,
      Equal = 1,
    }
  }
}
