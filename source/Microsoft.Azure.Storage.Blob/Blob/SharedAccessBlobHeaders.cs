// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.SharedAccessBlobHeaders
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core.Util;

namespace Microsoft.Azure.Storage.Blob
{
  public sealed class SharedAccessBlobHeaders
  {
    public SharedAccessBlobHeaders()
    {
    }

    public SharedAccessBlobHeaders(SharedAccessBlobHeaders sharedAccessBlobHeaders)
    {
      CommonUtility.AssertNotNull(nameof (sharedAccessBlobHeaders), (object) sharedAccessBlobHeaders);
      this.ContentType = sharedAccessBlobHeaders.ContentType;
      this.ContentDisposition = sharedAccessBlobHeaders.ContentDisposition;
      this.ContentEncoding = sharedAccessBlobHeaders.ContentEncoding;
      this.ContentLanguage = sharedAccessBlobHeaders.ContentLanguage;
      this.CacheControl = sharedAccessBlobHeaders.CacheControl;
    }

    public string CacheControl { get; set; }

    public string ContentDisposition { get; set; }

    public string ContentEncoding { get; set; }

    public string ContentLanguage { get; set; }

    public string ContentType { get; set; }
  }
}
