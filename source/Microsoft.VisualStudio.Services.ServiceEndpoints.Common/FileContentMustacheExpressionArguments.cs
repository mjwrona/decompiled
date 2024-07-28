// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.FileContentMustacheExpressionArguments
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  public class FileContentMustacheExpressionArguments
  {
    public const int MaxResponseSizeSupported = 524288;
    public const int RequestTimeoutInSeconds = 20;
    public string Url;
    public string AuthToken;
    public string ContentType;

    public string Method { get; private set; }

    public FileContentMustacheExpressionArguments()
    {
      this.ContentType = "application/json";
      this.Method = "GET";
    }
  }
}
