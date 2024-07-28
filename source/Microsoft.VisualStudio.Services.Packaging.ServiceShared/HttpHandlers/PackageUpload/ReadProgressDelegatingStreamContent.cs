// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers.PackageUpload.ReadProgressDelegatingStreamContent
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers.PackageUpload
{
  internal class ReadProgressDelegatingStreamContent : StreamContent
  {
    private readonly bool leaveOpen;
    private readonly IStopwatch stopWatch;
    private readonly ITimeProvider timeProvider;
    private readonly long bytesInterval;
    private ReadProgressStream readProgressStream;

    public ReadProgressDelegatingStreamContent(
      HttpContent originalContent,
      Stream originalStream,
      bool leaveOpen,
      IStopwatch stopWatch,
      ITimeProvider timeProvider,
      long bytesInterval)
      : base(originalStream)
    {
      foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) originalContent.Headers)
        this.Headers.TryAddWithoutValidation(header.Key, header.Value);
      this.leaveOpen = leaveOpen;
      this.stopWatch = stopWatch;
      this.timeProvider = timeProvider;
      this.bytesInterval = bytesInterval;
    }

    public void RecordEntry()
    {
      if (this.readProgressStream == null)
        throw new InvalidOperationException(Resources.Error_MustCallInOrder((object) "CreateContentReadStreamAsync", (object) nameof (RecordEntry)));
      this.readProgressStream.RecordProgressEntry();
    }

    public TransferRateResults GetTransferRateResults() => this.readProgressStream != null ? this.readProgressStream.GetTransferRateResults() : throw new InvalidOperationException(Resources.Error_MustCallInOrder((object) "CreateContentReadStreamAsync", (object) nameof (GetTransferRateResults)));

    protected override async Task<Stream> CreateContentReadStreamAsync()
    {
      this.readProgressStream = new ReadProgressStream(await base.CreateContentReadStreamAsync(), this.leaveOpen, this.stopWatch, this.timeProvider, this.bytesInterval);
      return (Stream) this.readProgressStream;
    }
  }
}
