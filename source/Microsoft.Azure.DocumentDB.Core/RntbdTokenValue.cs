// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RntbdTokenValue
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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
    public byte[] valueBytes;
  }
}
