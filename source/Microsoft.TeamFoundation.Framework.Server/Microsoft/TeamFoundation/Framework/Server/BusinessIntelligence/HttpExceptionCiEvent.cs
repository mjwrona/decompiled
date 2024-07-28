// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence.HttpExceptionCiEvent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence
{
  [ClientSupportedCiEvent]
  public class HttpExceptionCiEvent : ExceptionCiEvent
  {
    public const string HttpCodeParam = "HttpCode";

    public HttpExceptionCiEvent()
      : base(CustomerIntelligenceFeature.HttpException)
    {
    }

    public HttpExceptionCiEvent(int httpCode, string errorMessage)
      : base(CustomerIntelligenceFeature.HttpException, errorMessage)
    {
      this.HttpCode = httpCode;
    }

    public int HttpCode
    {
      get
      {
        int httpCode;
        this.Properties.TryGetValue<int>(nameof (HttpCode), out httpCode);
        return httpCode;
      }
      set => this.Properties[nameof (HttpCode)] = (object) value;
    }
  }
}
