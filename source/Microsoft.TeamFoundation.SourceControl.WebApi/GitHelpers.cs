// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitHelpers
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  internal static class GitHelpers
  {
    internal static byte[] ObjectIdFromString(string objectId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(objectId, nameof (objectId));
      if (objectId.Length != 40)
        throw new ArgumentException("An object ID must be 40 characters in length.", nameof (objectId));
      byte[] numArray = new byte[20];
      byte num1 = 0;
      for (int index = 0; index < 40; ++index)
      {
        short num2 = (short) objectId[index];
        int num3 = 1 == (index & 1) ? 1 : 0;
        byte num4;
        if (num2 >= (short) 48 && num2 <= (short) 57)
          num4 = (byte) 48;
        else if (num2 >= (short) 65 && num2 <= (short) 70)
        {
          num4 = (byte) 55;
        }
        else
        {
          if (num2 < (short) 97 || num2 > (short) 102)
            throw new ArgumentException(string.Format("While decoding an object ID, expected to find a hex digit in ASCII form; found {0} instead.", (object) num2.ToString("x2")), nameof (objectId));
          num4 = (byte) 87;
        }
        if (num3 != 0)
        {
          num1 |= (byte) ((uint) num2 - (uint) num4);
          numArray[index >> 1] = num1;
        }
        else
          num1 = (byte) ((int) num2 - (int) num4 << 4);
      }
      return numArray;
    }
  }
}
