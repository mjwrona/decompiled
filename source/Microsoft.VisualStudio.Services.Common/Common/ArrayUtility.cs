// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.ArrayUtility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System.Text;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class ArrayUtility
  {
    public static bool Equals(byte[] a1, byte[] a2)
    {
      if (a1.Length != a2.Length)
        return false;
      return a1.Length == 0 || ArrayUtility.Equals(a1, a2, a1.Length);
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

    public static string StringFromByteArray(byte[] bytes)
    {
      if (bytes == null || bytes.Length == 0)
        return "null";
      StringBuilder stringBuilder = new StringBuilder(bytes.Length * 2);
      for (int index = 0; index < bytes.Length; ++index)
      {
        int num = (int) bytes[index];
        char ch1 = (char) ((num >> 4 & 15) + 48);
        char ch2 = (char) ((num & 15) + 48);
        stringBuilder.Append(ch1 >= ':' ? (char) ((uint) ch1 + 39U) : ch1);
        stringBuilder.Append(ch2 >= ':' ? (char) ((uint) ch2 + 39U) : ch2);
      }
      return stringBuilder.ToString();
    }
  }
}
