// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.GalleryCustomHeaders
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public static class GalleryCustomHeaders
  {
    public const string ClientIdHeader = "X-Market-Client-Id";
    public const string UserIdHeader = "X-Market-User-Id";
    public const string PayloadFileName = "X-Market-UploadFileName";
    public const string PayloadProduct = "X-Market-UploadFileProduct";
    public const string ReviewApiKeyName = "X-VSGallery-ApiKey";
    public const string ReviewApiKeyValue = "48CFF4A6-39D2-476B-9530-45459FD6279A";
    public const string AccountToken = "X-Market-AccountToken";
    public const string SearchActivityId = "X-Market-Search-Activity-Id";
    public const string VSMarketplaceUserAgent = "VSMarketplace";
  }
}
