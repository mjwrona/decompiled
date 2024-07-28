// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.MediaResponse
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents.Collections;
using System.Collections.Specialized;
using System.IO;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class MediaResponse : IMediaResponse
  {
    private INameValueCollection responseHeaders;

    public string ActivityId { get; internal set; }

    public Stream Media { get; internal set; }

    public string Slug { get; internal set; }

    public string ContentType { get; internal set; }

    public long ContentLength { get; internal set; }

    public long MaxMediaStorageUsageInMB { get; internal set; }

    public long CurrentMediaStorageUsageInMB { get; internal set; }

    public NameValueCollection ResponseHeaders => this.responseHeaders.ToNameValueCollection();

    internal INameValueCollection Headers
    {
      get => this.responseHeaders;
      set => this.responseHeaders = value;
    }
  }
}
