// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.Protocol.BlobListingContext
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Shared.Protocol;

namespace Microsoft.Azure.Storage.Blob.Protocol
{
  public sealed class BlobListingContext : ListingContext
  {
    public BlobListingContext(
      string prefix,
      int? maxResults,
      string delimiter,
      BlobListingDetails details)
      : base(prefix, maxResults)
    {
      this.Delimiter = delimiter;
      this.Details = details;
    }

    public string Delimiter { get; set; }

    public BlobListingDetails Details { get; set; }
  }
}
