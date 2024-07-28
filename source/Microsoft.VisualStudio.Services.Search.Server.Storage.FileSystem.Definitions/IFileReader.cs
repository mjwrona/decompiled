// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions.IFileReader
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 21076807-BD87-4A02-A068-A20A32678060
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions
{
  public interface IFileReader : IDisposable
  {
    uint ReadUInt32();

    int ReadInt32();

    long ReadInt64();

    bool ReadBoolean();

    byte ReadByte();

    byte[] ReadBytes(int count);

    string ReadString();

    bool CanRead();

    long GetContentLength();
  }
}
