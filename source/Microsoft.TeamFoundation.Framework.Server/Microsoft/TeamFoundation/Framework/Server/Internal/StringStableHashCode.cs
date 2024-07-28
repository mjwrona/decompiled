// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Internal.StringStableHashCode
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.Internal
{
  internal struct StringStableHashCode : IStableHashCode
  {
    private string m_string;

    public StringStableHashCode(string str) => this.m_string = str.ToUpperInvariant();

    public unsafe int GetStableHashCode()
    {
      fixed (char* chPtr = this.m_string)
      {
        int num1 = 352654597;
        int num2 = num1;
        int* numPtr = (int*) chPtr;
        int length;
        for (length = this.m_string.Length; length > 2; length -= 4)
        {
          num1 = (num1 << 5) + num1 + (num1 >> 27) ^ *numPtr;
          num2 = (num2 << 5) + num2 + (num2 >> 27) ^ numPtr[1];
          numPtr += 2;
        }
        if (length > 0)
          num1 = (num1 << 5) + num1 + (num1 >> 27) ^ *numPtr;
        uint num3 = (uint) (num1 + num2 * 1566083941);
        return (int) ((uint) (((int) num3 & (int) byte.MaxValue) << 24 | ((int) num3 & 65280) << 8) | (num3 & 16711680U) >> 8 | (num3 & 4278190080U) >> 24);
      }
    }

    public override string ToString() => this.m_string;

    public static implicit operator StringStableHashCode(string str) => new StringStableHashCode(str);
  }
}
