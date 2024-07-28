// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.UInt128BinaryComparer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class UInt128BinaryComparer : IComparer<UInt128>
  {
    public static readonly UInt128BinaryComparer Singleton = new UInt128BinaryComparer();

    private UInt128BinaryComparer()
    {
    }

    public int Compare(UInt128 x, UInt128 y)
    {
      if ((long) x.GetLow() != (long) y.GetLow())
        return this.ReverseBytes(x.GetLow()) < this.ReverseBytes(y.GetLow()) ? -1 : 1;
      if ((long) x.GetHigh() == (long) y.GetHigh())
        return 0;
      return this.ReverseBytes(x.GetHigh()) < this.ReverseBytes(y.GetHigh()) ? -1 : 1;
    }

    private ulong ReverseBytes(ulong value) => (ulong) (((long) value & (long) byte.MaxValue) << 56 | ((long) value & 65280L) << 40 | ((long) value & 16711680L) << 24 | ((long) value & 4278190080L) << 8) | (value & 1095216660480UL) >> 8 | (value & 280375465082880UL) >> 24 | (value & 71776119061217280UL) >> 40 | (value & 18374686479671623680UL) >> 56;
  }
}
