// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.ByteArrayEqualityComparer
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Common
{
  public sealed class ByteArrayEqualityComparer : IEqualityComparer<byte[]>
  {
    public bool Equals(byte[] first, byte[] second)
    {
      if (first == second)
        return true;
      if (first == null || second == null || first.Length != second.Length)
        return false;
      for (int index = 0; index < first.Length; ++index)
      {
        if ((int) first[index] != (int) second[index])
          return false;
      }
      return true;
    }

    public int GetHashCode(byte[] array)
    {
      if (array == null)
        return 0;
      int hashCode = 17;
      foreach (byte num in array)
        hashCode = hashCode * 31 + (int) num;
      return hashCode;
    }
  }
}
