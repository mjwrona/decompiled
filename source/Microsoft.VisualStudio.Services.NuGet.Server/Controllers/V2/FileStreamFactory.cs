// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2.FileStreamFactory
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using System.IO;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2
{
  public class FileStreamFactory : IFileStreamFactory
  {
    public Stream CreateFileStream(string filename) => (Stream) new FileStream(filename, FileMode.Open);
  }
}
