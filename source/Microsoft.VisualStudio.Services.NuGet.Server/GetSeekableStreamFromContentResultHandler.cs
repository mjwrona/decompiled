// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.GetSeekableStreamFromContentResultHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.HttpStreams;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server
{
  public class GetSeekableStreamFromContentResultHandler : 
    IAsyncHandler<ContentResult, Stream>,
    IHaveInputType<ContentResult>,
    IHaveOutputType<Stream>
  {
    public static IAsyncHandler<ContentResult, Stream> Instance { get; } = (IAsyncHandler<ContentResult, Stream>) new GetSeekableStreamFromContentResultHandler();

    public async Task<Stream> Handle(ContentResult contentResult)
    {
      Stream stream = contentResult.Stream ?? (Stream) new HttpSeekableStream(contentResult.RedirectToUri);
      if (!stream.CanSeek)
      {
        FileStream fileStream = File.Create(Path.GetTempFileName(), 1000, FileOptions.DeleteOnClose);
        await stream.CopyToAsync((Stream) fileStream);
        stream = (Stream) fileStream;
        fileStream = (FileStream) null;
      }
      return stream;
    }
  }
}
