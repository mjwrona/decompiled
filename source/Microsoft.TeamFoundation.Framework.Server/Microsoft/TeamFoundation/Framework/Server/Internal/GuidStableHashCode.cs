// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Internal.GuidStableHashCode
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server.Internal
{
  internal struct GuidStableHashCode : IStableHashCode
  {
    private Guid m_guid;

    public GuidStableHashCode(Guid guid) => this.m_guid = guid;

    public int GetStableHashCode()
    {
      byte[] byteArray = this.m_guid.ToByteArray();
      int num1 = (int) byteArray[3] << 24 | (int) byteArray[2] << 16 | (int) byteArray[1] << 8 | (int) byteArray[0];
      short num2 = (short) ((int) byteArray[5] << 8 | (int) byteArray[4]);
      short num3 = (short) ((int) byteArray[7] << 8 | (int) byteArray[6]);
      byte num4 = byteArray[10];
      byte num5 = byteArray[15];
      int num6 = (int) num2 << 16 | (int) (ushort) num3;
      return num1 ^ num6 ^ ((int) num4 << 24 | (int) num5);
    }

    public override string ToString() => this.m_guid.ToString("D");

    public static implicit operator GuidStableHashCode(Guid guid) => new GuidStableHashCode(guid);
  }
}
