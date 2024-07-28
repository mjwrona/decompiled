// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RntbdTokenValue
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Documents
{
  [StructLayout(LayoutKind.Explicit)]
  internal struct RntbdTokenValue
  {
    [FieldOffset(0)]
    public byte valueByte;
    [FieldOffset(0)]
    public ushort valueUShort;
    [FieldOffset(0)]
    public uint valueULong;
    [FieldOffset(0)]
    public ulong valueULongLong;
    [FieldOffset(0)]
    public int valueLong;
    [FieldOffset(0)]
    public float valueFloat;
    [FieldOffset(0)]
    public double valueDouble;
    [FieldOffset(0)]
    public long valueLongLong;
    [FieldOffset(8)]
    public Guid valueGuid;
    [FieldOffset(24)]
    public ReadOnlyMemory<byte> valueBytes;
  }
}
