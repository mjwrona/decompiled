// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.IO.IReader
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.IO
{
  public interface IReader
  {
    bool ReadBool();

    sbyte ReadInt8();

    short ReadInt16();

    int ReadInt32();

    long ReadInt64();

    int ReadVInt32();

    long ReadVInt64();

    byte ReadUInt8();

    ushort ReadUInt16();

    uint ReadUInt32();

    ulong ReadUInt64();

    uint ReadVUInt32();

    ulong ReadVUInt64();

    byte[] ReadBytes(uint length);

    uint ReadBytes(byte[] data, uint offset, uint length);

    string ReadString();

    void SkipBytes(int length);

    void SkipString();

    void SkipVInt();
  }
}
