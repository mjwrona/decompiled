// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.OAuth.VssOAuthErrorCodes
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

namespace Microsoft.VisualStudio.Services.OAuth
{
  public static class VssOAuthErrorCodes
  {
    public static readonly string AccessDenied = "access_denied";
    public static readonly string InvalidClient = "invalid_client";
    public static readonly string InvalidGrant = "invalid_grant";
    public static readonly string InvalidRequest = "invalid_request";
    public static readonly string InvalidScope = "invalid_scope";
    public static readonly string ServerError = "server_error";
    public static readonly string TemporarilyUnavailable = "temporarily_unavailable";
    public static readonly string UnauthorizedClient = "unauthorized_client";
    public static readonly string UnsupportedGrantType = "unsupported_grant_type";
    public static readonly string UnsupportedResponseType = "unsupported_response_type";
  }
}
