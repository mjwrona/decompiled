// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.IO.IWriter
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.IO
{
  public interface IWriter
  {
    ulong BytesWritten { get; }

    void Close();

    void WriteBool(bool value);

    void WriteInt8(sbyte value);

    void WriteInt16(short value);

    void WriteInt32(int value);

    void WriteInt64(long value);

    void WriteVInt32(int value);

    void WriteVInt64(long value);

    void WriteUInt8(byte value);

    void WriteUInt16(ushort value);

    void WriteUInt32(uint value);

    void WriteUInt64(ulong value);

    void WriteVUInt32(uint value);

    void WriteVUInt64(ulong value);

    [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bytes")]
    void WriteBytes(byte[] bytes);

    [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bytes")]
    void WriteBytes(byte[] bytes, uint length);

    [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bytes")]
    void WriteBytes(byte[] bytes, uint index, uint length);

    void WriteString(string str);

    [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte")]
    void WriteString(string str, uint maxByteLength);

    void WriteFloat(float value);

    void WriteDouble(double value);
  }
}
