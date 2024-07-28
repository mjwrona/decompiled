// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.TempFile
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public sealed class TempFile : IDisposable
  {
    private readonly IFileSystem fileSystem;

    public TempFile(IFileSystem fileSystem, string directory)
    {
      this.fileSystem = fileSystem;
      this.Path = System.IO.Path.Combine(directory, Guid.NewGuid().ToString("N") + ".tmp");
    }

    public string Path { get; }

    public void Dispose()
    {
      if (!this.fileSystem.FileExists(this.Path))
        return;
      this.fileSystem.DeleteFile(this.Path);
    }
  }
}
