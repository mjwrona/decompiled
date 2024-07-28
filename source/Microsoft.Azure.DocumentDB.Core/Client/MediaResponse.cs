// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Client.MediaResponse
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Collections;
using System.Collections.Specialized;
using System.IO;

namespace Microsoft.Azure.Documents.Client
{
  public sealed class MediaResponse : IMediaResponse
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
