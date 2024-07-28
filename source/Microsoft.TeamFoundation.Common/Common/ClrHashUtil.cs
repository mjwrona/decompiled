// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.ClrHashUtil
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ClrHashUtil
  {
    public static unsafe int GetStringHashOrcas32(string str)
    {
      fixed (char* chPtr = str)
      {
        int num1 = 352654597;
        int num2 = num1;
        int* numPtr = (int*) chPtr;
        for (int length = str.Length; length > 0; length -= 4)
        {
          num1 = (num1 << 5) + num1 + (num1 >> 27) ^ *numPtr;
          if (length > 2)
          {
            num2 = (num2 << 5) + num2 + (num2 >> 27) ^ numPtr[1];
            numPtr += 2;
          }
          else
            break;
        }
        return num1 + num2 * 1566083941;
      }
    }

    public static unsafe int GetStringHashOrcas64(string str)
    {
      fixed (char* chPtr1 = str)
      {
        int num1 = 5381;
        int num2 = num1;
        int num3;
        for (char* chPtr2 = chPtr1; (num3 = (int) *chPtr2) != 0; chPtr2 += 2)
        {
          num1 = (num1 << 5) + num1 ^ num3;
          int num4 = (int) chPtr2[1];
          if (num4 != 0)
            num2 = (num2 << 5) + num2 ^ num4;
          else
            break;
        }
        return num1 + num2 * 1566083941;
      }
    }
  }
}
