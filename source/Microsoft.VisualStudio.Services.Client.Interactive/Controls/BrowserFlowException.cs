// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Controls.BrowserFlowException
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Contracts;
using System;
using System.Net;

namespace Microsoft.VisualStudio.Services.Client.Controls
{
  [ExceptionMapping("0.0", "3.0", "BrowserFlowException", "Microsoft.VisualStudio.Services.Client.Controls.BrowserFlowException, Microsoft.VisualStudio.Services.Client, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  public class BrowserFlowException : VssException
  {
    internal const int BrowserScriptDisabledHelpLinkId = 324081;
    internal const int NavigationServiceUnavailableHelpLinkId = 324082;
    internal const int NavigationUnauthorizedHelpLinkId = 324083;
    internal const int NavigationForbiddenHelpLinkId = 324084;
    internal const int NavigationNotFoundHelpLinkId = 324085;
    internal const int NavigationBadRequestHelpLinkId = 324086;
    internal const int NavigationBadGatewayHelpLinkId = 324087;
    internal const int NavigationGatewayTimeoutHelpLinkId = 324088;
    internal const int NavigationInternalServerErrorHelpLinkId = 324089;
    internal const int ServerServiceUnavailableHelpLinkId = 324090;
    internal const int ServerUnauthorizedHelpLinkId = 324091;
    internal const int ServerForbiddenHelpLinkId = 324092;
    internal const int ServerNotFoundHelpLinkId = 324093;
    internal const int ServerBadRequestHelpLinkId = 324094;
    internal const int ServerInternalServerErrorHelpLinkId = 324095;
    internal const int ServerUnknownErrorHelpLinkId = 324096;
    internal const int UnknownClientErrorHelpLinkId = 324098;
    internal const int UnknownErrorHelpLinkId = 324099;

    public BrowserFlowException(BrowserFlowLayer layer, ErrorData error, string message)
      : this(layer, error, message, (Exception) null)
    {
    }

    public BrowserFlowException(
      BrowserFlowLayer layer,
      ErrorData error,
      string message,
      Exception innerException)
      : base(message, innerException)
    {
      this.Layer = layer;
      this.Error = error;
    }

    public BrowserFlowLayer Layer { get; private set; }

    public ErrorData Error { get; private set; }

    internal static string FormatHelpLink(int helpLinkId) => "https://go.microsoft.com/fwlink/?LinkID=" + helpLinkId.ToString();

    internal static int GetNavigationHelpLinkId(HttpStatusCode code)
    {
      switch (code)
      {
        case HttpStatusCode.BadRequest:
          return 324086;
        case HttpStatusCode.Unauthorized:
          return 324083;
        case HttpStatusCode.Forbidden:
          return 324084;
        case HttpStatusCode.NotFound:
          return 324085;
        case HttpStatusCode.InternalServerError:
          return 324089;
        case HttpStatusCode.BadGateway:
          return 324087;
        case HttpStatusCode.ServiceUnavailable:
          return 324082;
        case HttpStatusCode.GatewayTimeout:
          return 324088;
        default:
          return 324099;
      }
    }

    internal static string GetNavigationExceptionMessage(HttpStatusCode code)
    {
      switch (code)
      {
        case HttpStatusCode.BadRequest:
          return ClientResources.NavigationBadRequest();
        case HttpStatusCode.Unauthorized:
          return ClientResources.NavigationUnauthorized();
        case HttpStatusCode.Forbidden:
          return ClientResources.NavigationForbidden();
        case HttpStatusCode.NotFound:
          return ClientResources.NavigationNotFound();
        case HttpStatusCode.InternalServerError:
          return ClientResources.NavigationInternalServerError();
        case HttpStatusCode.BadGateway:
          return ClientResources.NavigationBadGateway();
        case HttpStatusCode.ServiceUnavailable:
          return ClientResources.NavigationServiceUnavailable();
        case HttpStatusCode.GatewayTimeout:
          return ClientResources.NavigationGatewayTimeout();
        default:
          return ClientResources.UnknownError();
      }
    }

    internal static int GetServerHelpLinkId(HttpStatusCode code)
    {
      switch (code)
      {
        case HttpStatusCode.BadRequest:
          return 324094;
        case HttpStatusCode.Unauthorized:
          return 324091;
        case HttpStatusCode.Forbidden:
          return 324092;
        case HttpStatusCode.NotFound:
          return 324093;
        case HttpStatusCode.InternalServerError:
          return 324095;
        case HttpStatusCode.ServiceUnavailable:
          return 324090;
        default:
          return 324099;
      }
    }

    internal static string GetServerExceptionMessage(HttpStatusCode code)
    {
      switch (code)
      {
        case HttpStatusCode.BadRequest:
          return ClientResources.ServerBadRequest();
        case HttpStatusCode.Unauthorized:
          return ClientResources.ServerUnauthorized();
        case HttpStatusCode.Forbidden:
          return ClientResources.ServerForbidden();
        case HttpStatusCode.NotFound:
          return ClientResources.ServerNotFound();
        case HttpStatusCode.InternalServerError:
          return ClientResources.ServerInternalServerError();
        case HttpStatusCode.ServiceUnavailable:
          return ClientResources.ServerServiceUnavailable();
        default:
          return ClientResources.UnknownError();
      }
    }
  }
}
