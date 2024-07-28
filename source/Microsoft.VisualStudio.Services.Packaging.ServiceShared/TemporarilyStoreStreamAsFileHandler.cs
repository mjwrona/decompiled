// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.TemporarilyStoreStreamAsFileHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class TemporarilyStoreStreamAsFileHandler : 
    IAsyncHandler<FeedRequest<Stream>, FileStream>,
    IHaveInputType<FeedRequest<Stream>>,
    IHaveOutputType<FileStream>
  {
    public async Task<FileStream> Handle(FeedRequest<Stream> request)
    {
      string str = Path.Combine(Path.GetTempPath(), request.Feed.Id.ToString());
      Directory.CreateDirectory(str);
      FileStream fs = File.Create(Path.Combine(str, Guid.NewGuid().ToString()), 4096, FileOptions.DeleteOnClose);
      await request.AdditionalData.CopyToAsync((Stream) fs);
      fs.Position = 0L;
      FileStream fileStream = fs;
      fs = (FileStream) null;
      return fileStream;
    }
  }
}
