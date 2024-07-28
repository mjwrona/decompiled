// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.ArrayUtil
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

namespace Microsoft.TeamFoundation.Common
{
  public static class ArrayUtil
  {
    public static bool Equals(byte[] a1, byte[] a2)
    {
      if (a1 == null && a2 == null)
        return true;
      if (a1 == null || a2 == null || a1.Length != a2.Length)
        return false;
      return a1.Length == 0 || ArrayUtil.Equals(a1, a2, a1.Length);
    }

    public static int GetHashCode(byte[] array)
    {
      int hashCode = 0;
      foreach (byte num in array)
        hashCode += (int) num;
      return hashCode;
    }

    public static unsafe bool Equals(byte[] a1, byte[] a2, int length)
    {
      fixed (byte* numPtr1 = &a1[0])
        fixed (byte* numPtr2 = &a2[0])
        {
          byte* numPtr3 = numPtr1;
          byte* numPtr4 = numPtr2;
          for (int index = length >> 2; index > 0; --index)
          {
            if (*(int*) numPtr3 != *(int*) numPtr4)
              return false;
            numPtr3 += 4;
            numPtr4 += 4;
          }
          for (int index = length & 3; index > 0; --index)
          {
            if ((int) *numPtr3 != (int) *numPtr4)
              return false;
            ++numPtr3;
            ++numPtr4;
          }
        }
      return true;
    }
  }
}
