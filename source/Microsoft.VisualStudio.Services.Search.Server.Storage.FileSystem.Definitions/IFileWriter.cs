// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions.IFileWriter
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 21076807-BD87-4A02-A068-A20A32678060
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions
{
  public interface IFileWriter : IDisposable
  {
    void Write(uint number);

    void Write(int i);

    void Write(long i);

    void Write(bool flag);

    void Write(byte value);

    void Write(byte[] array, int index, int count);

    void WriteString(string value);

    long GetPosition();

    void Flush();
  }
}
