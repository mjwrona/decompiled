// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.SharedAccessFileHeaders
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Core.Util;

namespace Microsoft.Azure.Storage.File
{
  public sealed class SharedAccessFileHeaders
  {
    public SharedAccessFileHeaders()
    {
    }

    public SharedAccessFileHeaders(SharedAccessFileHeaders sharedAccessFileHeaders)
    {
      CommonUtility.AssertNotNull(nameof (sharedAccessFileHeaders), (object) sharedAccessFileHeaders);
      this.ContentType = sharedAccessFileHeaders.ContentType;
      this.ContentDisposition = sharedAccessFileHeaders.ContentDisposition;
      this.ContentEncoding = sharedAccessFileHeaders.ContentEncoding;
      this.ContentLanguage = sharedAccessFileHeaders.ContentLanguage;
      this.CacheControl = sharedAccessFileHeaders.CacheControl;
    }

    public string CacheControl { get; set; }

    public string ContentDisposition { get; set; }

    public string ContentEncoding { get; set; }

    public string ContentLanguage { get; set; }

    public string ContentType { get; set; }
  }
}
