// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.HttpHeaders
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class HttpHeaders
  {
    public const string ClientRequestId = "x-ms-client-request-id";
    public const string ETag = "ETag";
    public const string Upn = "x-ms-client-principal-name";
    public const string TenantId = "x-ms-client-tenant-id";
    public const string CsmPassthrough = "x-ms-csmpassthrough";
    public const string Location = "Location";
    public const string RetryAfter = "Retry-After";
    public const string MarketplacePartNumber = "x-ms-marketplace-part";
    public const string MarketplaceOrderNumber = "x-ms-marketplace-order";
    public const string ArmCorrelationRequestId = "x-ms-correlation-request-id";
  }
}
