// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controllers.ErrorController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Aad;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Framework.Server.DataImport;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Contracts;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Common.Utility;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.InstanceManagement.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Operations;
using Microsoft.VisualStudio.Services.Partitioning.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controllers
{
  [Microsoft.TeamFoundation.Server.WebAccess.Platform.Filters.ApplyRequestLanguage]
  public class ErrorController : Controller
  {
    private static readonly string s_traceLayer = nameof (ErrorController);
    private static readonly bool s_isHosted = false;
    private List<string> InformationalErrorMessages = new List<string>()
    {
      FrameworkResources.AadGuestUserNotAllowedInAccount(),
      FrameworkResources.MissingRequiredAadGroupMembership(),
      FrameworkResources.InvalidLicenseException(),
      PlatformResources.NoTenantExceptionMessage
    };
    private const string CreateMicrosoftAccountLink = "https://go.microsoft.com/fwlink/?linkid=860178";
    private const string WrongWorkOrPersonalSignoutTrackingParameter = "switch_out=1";
    private static readonly Guid s_serviceInstanceDIS = Guid.Parse("0000003E-0000-8888-8000-000000000000");
    private static readonly Guid s_operationPluginDataImportStatus = Guid.Parse("7a25d9d5-63db-462a-adee-7adbd76a1309");

    static ErrorController()
    {
      try
      {
        string appSetting = WebConfigurationManager.AppSettings["IsHosted"];
        ErrorController.s_isHosted = !string.IsNullOrEmpty(appSetting) && string.Equals(appSetting, "true", StringComparison.OrdinalIgnoreCase);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, nameof (ErrorController), ex);
      }
    }

    [ValidateInput(false)]
    public ActionResult Index()
    {
      int statusCode = 0;
      string message = string.Empty;
      string title = string.Empty;
      string str1 = string.Empty;
      bool flag1 = false;
      bool flag2 = true;
      bool? nullable = new bool?();
      string[] second = (string[]) null;
      Exception exception = (Exception) null;
      try
      {
        TeamFoundationTracingService.TraceEnterRaw(512000, "WebAccess", ErrorController.s_traceLayer, nameof (Index));
        this.ViewData["mode"] = (object) "error";
        bool debuggingEnabled = this.Request.RequestContext.HttpContext.IsDebuggingEnabled;
        RouteData routeData = this.ControllerContext.RouteData;
        object obj1;
        if (routeData != null && routeData.Values.TryGetValue("Exception", out obj1))
          exception = obj1 as Exception;
        object obj2;
        if (routeData != null && routeData.Values.TryGetValue("StatusCode", out obj2))
          statusCode = (int) obj2;
        object obj3;
        if (routeData != null && routeData.Values.TryGetValue("Description", out obj3))
          message = obj3 as string;
        if (exception != null)
        {
          if (exception is HttpException httpException)
            statusCode = !(exception is HttpRequestValidationException) ? httpException.GetHttpCode() : 400;
          if ((exception is HttpUnhandledException || exception is RequestFilterException) && exception.InnerException != null)
            exception = exception.InnerException;
          UserFriendlyError userFriendlyError = new UserFriendlyError(exception, ErrorController.s_isHosted);
          message = userFriendlyError.Message;
          title = userFriendlyError.Title;
          flag1 = userFriendlyError.ShowMessageToHostedUsers;
          if (exception is BrowserNotSupportedException)
          {
            str1 = ((BrowserNotSupportedException) exception).DetailedMessage;
            this.ViewData["mode"] = (object) "info";
            flag2 = false;
            statusCode = 400;
          }
          else
          {
            if (exception is MisconfiguredRouteException)
              statusCode = 404;
            if (debuggingEnabled)
              str1 = exception.ToReadableStackTrace();
          }
          if (exception is TooManyRequestsException)
          {
            statusCode = 429;
            if (!debuggingEnabled)
              flag1 = false;
          }
        }
        if (statusCode != 0)
        {
          try
          {
            this.Response.StatusCode = statusCode;
            this.Response.TrySkipIisCustomErrors = true;
          }
          catch (HttpException ex)
          {
          }
        }
        this.ViewData["title"] = (object) PlatformResources.PageTitleWithContent.FormatUI((object) title);
        this.ViewData["message"] = (object) message;
        this.ViewData["detail"] = (object) str1;
        TraceLevel traceLevel = exception == null || !(exception is UnauthorizedRequestException) ? TraceLevel.Error : TraceLevel.Warning;
        TraceEvent trace = new TraceEvent("{0}  Exception: {1}", (object) message, exception != null ? (object) exception.ToReadableStackTrace() : (object) "No exception specified");
        TeamFoundationTracingService.GetTraceEvent(ref trace, 512005, traceLevel, "WebAccess", TfsTraceLayers.Controller, (string[]) null, exception == null ? (string) null : exception.GetType().FullName);
        if (this.Request != null)
        {
          trace.Uri = this.Request.Url.ToString();
          trace.UserAgent = this.Request.UserAgent;
        }
        TeamFoundationTracingService.TraceRaw(ref trace);
        if (exception != null)
          TeamFoundationTracingService.TraceExceptionRaw(512005, traceLevel, "WebAccess", TfsTraceLayers.Controller, exception);
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(512010, "WebAccess", ErrorController.s_traceLayer, nameof (Index));
      }
      if (!ErrorController.s_isHosted)
        return (ActionResult) this.View();
      IVssRequestContext vssRequestContext = (IVssRequestContext) this.Request.RequestContext.HttpContext.Items[(object) "IVssRequestContext"];
      string userDisplayName;
      Microsoft.VisualStudio.Services.Identity.Identity identity;
      string str2 = this.ExtractUserNames(vssRequestContext, out userDisplayName, out identity);
      if (string.IsNullOrEmpty(str2) && exception is InvalidIdentityException identityException)
        str2 = identityException.UserName;
      if (statusCode != 401 && (exception is AadAuthorizationException || exception is InvalidIdentityException))
      {
        nullable = new bool?(false);
        second = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, PlatformResources.Tips_Forbidden, (object) SafeHtmlWrapper.MakeSafeWithHtmlEncode(str2)).Split(new string[1]
        {
          Environment.NewLine
        }, StringSplitOptions.RemoveEmptyEntries);
      }
      if (exception is AadUserStateException)
      {
        nullable = new bool?(false);
        Guid guid = Guid.Empty;
        string inputHtml = string.Empty;
        if (identity != null)
        {
          inputHtml = identity.GetProperty<string>("Account", string.Empty);
          guid = identity.GetProperty<Guid>("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.Empty);
        }
        second = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, PlatformResources.Tips_ForbiddenAadUserState, (object) SafeHtmlWrapper.MakeSafeWithHtmlEncode(inputHtml), (object) guid).Split(new string[1]
        {
          Environment.NewLine
        }, StringSplitOptions.RemoveEmptyEntries);
      }
      if (exception is HostInReadOnlyModeException)
      {
        nullable = new bool?(false);
        statusCode = 503;
        second = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, FrameworkResources.SpsHostInReadOnlyMode()).Split(new string[1]
        {
          Environment.NewLine
        }, StringSplitOptions.RemoveEmptyEntries);
      }
      string rootURL;
      string signoutUrl = this.ExtractSignoutURL(vssRequestContext, statusCode, out rootURL);
      if (!string.IsNullOrEmpty(message))
      {
        if (message.Contains("VS403201"))
          return this.ReparentingStatusPage(message, title);
        if (message.Contains("VS403166"))
          return this.DataImportStatusPage(message, title);
      }
      ErrorModel model = new ErrorModel(statusCode, str2, this.Url);
      model.ServiceCommunicationLink = "https://twitter.com/AzureDevOps";
      model.ServiceCommunicationLinkText = "@AzureDevOps";
      model.HeaderLogoBase64 = "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";
      model.SetActionLink(signoutUrl, rootURL, rootURL);
      model.Title = title;
      model.InnerMessage = message;
      model.EncodeStackTrace = flag2;
      model.StackTrace = str1;
      if (flag1)
        model.ShowInnerMessage = true;
      if (vssRequestContext != null && vssRequestContext.ActivityId != Guid.Empty)
      {
        model.ActivityId = vssRequestContext.ActivityId;
        model.ErrorDateTime = DateTime.UtcNow;
      }
      if (exception is HttpInvalidLicenseException)
      {
        model.ShowInnerMessage = true;
        model.Tips = (string[]) null;
      }
      if (exception is RequestBlockedException)
      {
        string messageHtml = ((RequestBlockedException) exception).MessageHtml;
        if (!string.IsNullOrEmpty(messageHtml))
          model.HtmlMessage = messageHtml;
        string redirectLocation = vssRequestContext.GetService<ITeamFoundationAuthenticationService>().GetSignInRedirectLocation(vssRequestContext);
        model.SetActionLink(signoutUrl, rootURL, redirectLocation);
        model.ShowInnerMessage = false;
      }
      if (exception is InvitationPendingException pendingException)
      {
        second = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, PlatformResources.Tips_InvitationPending, (object) pendingException.AccountName, (object) pendingException.OrganizationName).Split(new string[1]
        {
          Environment.NewLine
        }, StringSplitOptions.RemoveEmptyEntries);
        model.Message = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, PlatformResources.ErrorMessage_InvitationPending, (object) SafeHtmlWrapper.MakeSafeWithHtmlEncode(model.UserName), (object) SafeHtmlWrapper.MakeSafeWithHtmlEncode(pendingException.OrganizationName));
        model.Tips = (string[]) null;
      }
      if (exception is WrongWorkOrPersonalException wrongWorkOrPersonalException)
      {
        model.HtmlMessage = this.GetWrongWorkOrPersonalHtmlMessage(wrongWorkOrPersonalException);
        string empty1 = string.Empty;
        string empty2 = string.Empty;
        string actionText;
        string actionLink;
        if (wrongWorkOrPersonalException.ShouldCreatePersonal)
        {
          actionText = PlatformResources.ActionLink_CreatePersonal;
          actionLink = "https://go.microsoft.com/fwlink/?linkid=860178";
        }
        else
        {
          actionText = wrongWorkOrPersonalException.ShouldBePersonal ? PlatformResources.ActionLink_SwitchToPersonal : PlatformResources.ActionLink_SwitchToWork;
          actionLink = this.ExtractSwitchUrl(vssRequestContext, statusCode, wrongWorkOrPersonalException);
        }
        string signoutURL = this.AddSignoutTrackingParameter(signoutUrl, "switch_out=1");
        model.SetSwitchUserActionLinks(actionText, actionLink, signoutURL);
        string str3 = wrongWorkOrPersonalException.ShouldBePersonal ? PlatformResources.Tips_ShouldBePersonal : PlatformResources.Tips_ShouldBeWork;
        model.Tips = str3.Split(new string[1]
        {
          Environment.NewLine
        }, StringSplitOptions.RemoveEmptyEntries);
        model.EncodeTips = false;
        model.ShowInnerMessage = false;
      }
      if (statusCode == 403 && second == null && this.IsExceptionMessageInformational(model.InnerMessage))
        model.ShowInnerMessage = true;
      if (second != null)
      {
        if (model.Tips != null)
          second = ((IEnumerable<string>) model.Tips).Concat<string>((IEnumerable<string>) second).ToArray<string>();
        model.Tips = second;
      }
      if (nullable.HasValue)
        model.EncodeTips = nullable.Value;
      this.ViewData["errorData"] = (object) new ErrorData()
      {
        Uri = this.Request.Url,
        StatusCode = statusCode,
        Details = str1,
        Message = message,
        Identity = userDisplayName
      };
      // ISSUE: reference to a compiler-generated field
      if (ErrorController.\u003C\u003Eo__3.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        ErrorController.\u003C\u003Eo__3.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "jQueryVersion", typeof (ErrorController), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = ErrorController.\u003C\u003Eo__3.\u003C\u003Ep__0.Target((CallSite) ErrorController.\u003C\u003Eo__3.\u003C\u003Ep__0, this.ViewBag, CoreReferencesContext.SelectJQueryMinVersionForBrowser());
      return (ActionResult) this.View("HostedErrorNew", (object) model);
    }

    private ActionResult ReparentingStatusPage(string message, string title)
    {
      IDictionary<string, string> statusProperties = this.GetReparentingStatusProperties(this.ExtractRequestIdFromErrorMessage("Activity Id: ", message));
      string valueOrDefault1 = statusProperties.GetValueOrDefault<string, string>("CollectionName");
      string valueOrDefault2 = statusProperties.GetValueOrDefault<string, string>("SourceOrganizationName");
      string valueOrDefault3 = statusProperties.GetValueOrDefault<string, string>("TargetOrganizationName");
      return (ActionResult) this.View("ReparentingStatusView", !string.IsNullOrEmpty(valueOrDefault2) || string.IsNullOrEmpty(valueOrDefault3) ? (string.IsNullOrEmpty(valueOrDefault2) || !string.IsNullOrEmpty(valueOrDefault3) ? (object) new ReparentingStatusModel(title, valueOrDefault1, string.Empty, false) : (object) new ReparentingStatusModel(title, valueOrDefault1, valueOrDefault2, false)) : (object) new ReparentingStatusModel(title, valueOrDefault1, valueOrDefault3, true));
    }

    private ActionResult DataImportStatusPage(string message, string title)
    {
      DataImportStatusModel model = new DataImportStatusModel()
      {
        Title = title
      };
      try
      {
        using (IVssRequestContext systemContext = TeamFoundationApplicationCore.DeploymentServiceHost.CreateSystemContext())
        {
          Guid fromErrorMessage = this.ExtractRequestIdFromErrorMessage("DataImport Id: ", message);
          Operation operation = (Operation) null;
          model.Image = StaticResources.Versioned.Content.GetLocation("DataImportRunning.png", systemContext);
          model.LoadingImage = StaticResources.Versioned.Content.GetLocation("Loading.gif", systemContext);
          if (fromErrorMessage == Guid.Empty)
            systemContext.Trace(512011, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, "Cannot find a DataImportID in the error message: " + Environment.NewLine + message);
          else if (this.Request.RequestContext.HttpContext.Items[(object) HttpContextConstants.ServiceHostRouteContext] is HostRouteContext hostRouteContext)
          {
            Guid hostId = hostRouteContext.HostId;
            Guid instanceId = PartitionedClientHelper.GetSpsClientForHostId<InstanceManagementHttpClient>(systemContext, hostId).GetHostInstanceMappingsAsync(hostId, new Guid?(ErrorController.s_serviceInstanceDIS)).SyncResult<List<HostInstanceMapping>>()[0].ServiceInstance.InstanceId;
            operation = systemContext.GetClient<OperationsHttpClient>(instanceId).GetOperationAsync(fromErrorMessage, new Guid?(ErrorController.s_operationPluginDataImportStatus)).SyncResult<Operation>();
          }
          if (operation != null)
          {
            model.StepDescription = (IHtmlString) new HtmlString(operation.DetailedMessage);
            string data = (string) null;
            if (operation.ResultMessage.StartsWith("<?xml"))
            {
              data = operation.ResultMessage;
            }
            else
            {
              string str = string.Join(":", ((IEnumerable<string>) operation.ResultMessage.Split(new char[1]
              {
                ':'
              }, StringSplitOptions.RemoveEmptyEntries)).Skip<string>(3));
              if (str.StartsWith("<?xml"))
                data = str;
            }
            if (data != null)
            {
              PlatformDataImportStatusOperationResult statusOperationResult = PlatformDataImportStatusOperationResult.FromString(data);
              model.LastUpdate = statusOperationResult.StatusPageLastUpdated.ToString("O");
              model.StepTitle = statusOperationResult.StatusPageTitle;
              model.StepCounter = statusOperationResult.StatusPageSubtitle;
              List<DataImportFileTransferProgressMetricModel> progressMetricModelList = new List<DataImportFileTransferProgressMetricModel>();
              foreach (DataImportFileTransferProgress transferProgress in statusOperationResult.FileTransferProgress)
              {
                Guid result;
                if (Guid.TryParse(transferProgress.Service, out result))
                {
                  ServiceTypeMetadata metadataByServiceType = ServiceTypeMapper.GetServiceTypeMetadataByServiceType(result);
                  if (metadataByServiceType != null && !metadataByServiceType.DataImportStatusPageIgnore && transferProgress.BytesTotal != -1L && transferProgress.BytesTransferred != -1L)
                  {
                    double num = (double) transferProgress.BytesTotal != 0.0 ? (double) transferProgress.BytesTransferred / (double) transferProgress.BytesTotal * 100.0 : 100.0;
                    DataImportFileTransferProgressMetricModel progressMetricModel = new DataImportFileTransferProgressMetricModel()
                    {
                      ServiceName = metadataByServiceType.PublicDisplayName,
                      Progress = num,
                      Transferred = ByteConverterUtility.ConvertBytesToString(transferProgress.BytesTransferred, ByteConverterUtilityUnit.B, 3),
                      Total = ByteConverterUtility.ConvertBytesToString(transferProgress.BytesTotal, ByteConverterUtilityUnit.B, 3)
                    };
                    progressMetricModelList.Add(progressMetricModel);
                  }
                }
              }
              progressMetricModelList.Sort((Comparison<DataImportFileTransferProgressMetricModel>) ((a, b) => a.ServiceName.CompareTo(b.ServiceName)));
              model.FileTransferProgress = progressMetricModelList;
            }
            if (operation.Status == OperationStatus.Failed)
              model.Image = StaticResources.Versioned.Content.GetLocation("DataImportFailure.png", systemContext);
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceRawAlwaysOn(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DataImportStatusPage: Exception={0} CallerStack={1} ExceptionStack={2} OriginalMessage={3}", (object) ex.Message, (object) Environment.StackTrace, (object) ex.StackTrace, (object) message));
      }
      return (ActionResult) this.View("DataImportStatusView", (object) model);
    }

    private Guid ExtractRequestIdFromErrorMessage(string marker, string message)
    {
      Guid result = Guid.Empty;
      int num = message.IndexOf(marker);
      if (num != -1)
        Guid.TryParse(message.Substring(num + marker.Length, 36), out result);
      return result;
    }

    private string ExtractUserNames(
      IVssRequestContext tfsRequestContext,
      out string userDisplayName,
      out Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      userDisplayName = string.Empty;
      string userNames = string.Empty;
      identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      try
      {
        if (tfsRequestContext != null)
        {
          if (tfsRequestContext.UserContext != (IdentityDescriptor) null)
          {
            identity = tfsRequestContext.GetUserIdentity();
            if (identity != null)
            {
              userDisplayName = identity.DisplayName;
              string property = identity.GetProperty<string>("Account", string.Empty);
              userNames = string.IsNullOrEmpty(property) || property.Equals(userDisplayName) ? userDisplayName : string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} <{1}>", (object) userDisplayName, (object) property);
              string tenantName = this.ExtractTenantName(tfsRequestContext, identity);
              if (!string.IsNullOrEmpty(tenantName))
                userNames = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} ({1})", (object) userNames, (object) tenantName);
            }
          }
        }
      }
      catch (Exception ex)
      {
        tfsRequestContext.TraceException(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, ex);
      }
      return userNames;
    }

    private string ExtractTenantName(IVssRequestContext tfsRequestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (identity == null)
        return string.Empty;
      string tenantName = string.Empty;
      try
      {
        if (identity.IsExternalUser)
        {
          IAadTenantDetailProvider extension = tfsRequestContext.GetExtension<IAadTenantDetailProvider>((Func<IAadTenantDetailProvider, bool>) (x => x.CanHandleRequest(tfsRequestContext)));
          if (extension != null)
            tenantName = extension.GetDisplayName(tfsRequestContext.To(TeamFoundationHostType.Deployment));
        }
        else if (identity.GetProperty<string>("Domain", string.Empty).Equals("Windows Live ID"))
          tenantName = FrameworkResources.ErrorMessage_MsaTenantName();
      }
      catch (Exception ex)
      {
        tfsRequestContext.TraceException(599999, TraceLevel.Info, "WebAccess", TfsTraceLayers.Controller, ex);
      }
      return tenantName;
    }

    private string ExtractSignoutURL(
      IVssRequestContext tfsRequestContext,
      int statusCode,
      out string rootURL,
      string additionalRedirectQueryParameters = null)
    {
      rootURL = VirtualPathUtility.ToAbsolute("~/");
      string signoutUrl;
      if (tfsRequestContext != null)
      {
        if (tfsRequestContext.ServiceInstanceType() == ServiceInstanceTypes.SPS)
          tfsRequestContext = tfsRequestContext.To(TeamFoundationHostType.Deployment);
        AccessMapping accessMapping = tfsRequestContext.GetService<ILocationService>().DetermineAccessMapping(tfsRequestContext);
        rootURL = accessMapping.AccessPoint;
        if (tfsRequestContext.UserContext != (IdentityDescriptor) null && tfsRequestContext.UserContext.IsCspPartnerIdentityType())
          return ErrorController.BuildSignoutUrlForCspPartnerUser(tfsRequestContext);
        signoutUrl = !accessMapping.AccessPoint.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? accessMapping.AccessPoint + "/_signout" : accessMapping.AccessPoint + "_signout";
        if (statusCode == 401 || statusCode == 403 || statusCode == 404)
        {
          UriBuilder uriBuilder = new UriBuilder(this.Request.Url);
          uriBuilder.Port = new Uri(accessMapping.AccessPoint).Port;
          if (statusCode == 403)
          {
            if (tfsRequestContext.RequestRestrictions().HasAnyLabel("SignedInPage", "TenantPicker"))
            {
              string uri = HttpUtility.ParseQueryString(uriBuilder.Query)["reply_to"] ?? rootURL;
              NameValueCollection state = AadAuthUrlUtility.ParseState(tfsRequestContext);
              if (state != null)
                uri = state["reply_to"] ?? uri;
              uriBuilder = new UriBuilder(uri);
            }
          }
          if (additionalRedirectQueryParameters != null)
            uriBuilder.Query = uriBuilder.Query == null || uriBuilder.Query.Length <= 1 ? additionalRedirectQueryParameters : uriBuilder.Query.Substring(1) + "&" + additionalRedirectQueryParameters;
          signoutUrl += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "?redirectUrl={0}", (object) Uri.EscapeDataString(uriBuilder.Uri.AbsoluteUri));
        }
      }
      else
        signoutUrl = VirtualPathUtility.ToAbsolute("~/_signout");
      return signoutUrl;
    }

    private static string BuildSignoutUrlForCspPartnerUser(IVssRequestContext tfsRequestContext)
    {
      string spsAbsoluteUrl = ErrorController.GetSpsAbsoluteUrl(tfsRequestContext.To(TeamFoundationHostType.Deployment));
      if (string.IsNullOrWhiteSpace(spsAbsoluteUrl))
        return VirtualPathUtility.ToAbsolute("~/_signout");
      string str = spsAbsoluteUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? spsAbsoluteUrl + "_signout" : spsAbsoluteUrl + "/_signout";
      Dictionary<string, string> parameters = new Dictionary<string, string>()
      {
        {
          "tenantId",
          AadIdentityHelper.GetIdentityTenantId(tfsRequestContext.UserContext).ToString()
        }
      };
      string redirectLocation = tfsRequestContext.GetService<ITeamFoundationAuthenticationService>().GetSignInRedirectLocation(tfsRequestContext, true, (IDictionary<string, string>) parameters);
      return string.IsNullOrWhiteSpace(redirectLocation) ? str : str + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "?redirectUrl={0}", (object) UriUtility.UrlEncode(redirectLocation));
    }

    private static string GetSpsAbsoluteUrl(IVssRequestContext context)
    {
      ILocationService service = context.GetService<ILocationService>();
      try
      {
        return new Uri(service.GetLocationServiceUrl(context, ServiceInstanceTypes.SPS, AccessMappingConstants.ClientAccessMappingMoniker)).AbsoluteUri;
      }
      catch (Exception ex)
      {
        context.Trace(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, "Failed to get SPS AbsoluteUri. " + ex.Message);
        return string.Empty;
      }
    }

    private string ExtractSwitchUrl(
      IVssRequestContext tfsRequestContext,
      int statusCode,
      WrongWorkOrPersonalException wrongWorkOrPersonalException)
    {
      string str = (wrongWorkOrPersonalException.ShouldBePersonal ? 'P' : 'W').ToString() + "," + wrongWorkOrPersonalException.AccountName;
      return this.ExtractSignoutURL(tfsRequestContext, statusCode, out string _, "switch_hint=" + HttpUtility.UrlEncode(str));
    }

    private string AddSignoutTrackingParameter(string signoutUrl, string parameter) => signoutUrl.Replace("?redirectUrl=", "?" + parameter + "&redirectUrl=");

    private string GetWrongWorkOrPersonalHtmlMessage(
      WrongWorkOrPersonalException wrongWorkOrPersonalException)
    {
      string empty = string.Empty;
      string format = !wrongWorkOrPersonalException.ShouldBePersonal ? PlatformResources.ErrorMessage_SwitchToWork : (!wrongWorkOrPersonalException.ShouldCreatePersonal ? PlatformResources.ErrorMessage_SwitchToPersonal : PlatformResources.ErrorMessage_CreatePersonal);
      string inputHtml = this.Request?.Url?.Host ?? string.Empty;
      return string.Format((IFormatProvider) CultureInfo.CurrentUICulture, format, (object) SafeHtmlWrapper.MakeSafeWithHtmlEncode(wrongWorkOrPersonalException.AccountName), (object) SafeHtmlWrapper.MakeSafeWithHtmlEncode(inputHtml));
    }

    private IDictionary<string, string> GetReparentingStatusProperties(Guid reparentingRequestId)
    {
      IDictionary<string, string> statusProperties = (IDictionary<string, string>) new Dictionary<string, string>();
      using (IVssRequestContext systemContext = TeamFoundationApplicationCore.DeploymentServiceHost.CreateSystemContext())
      {
        using (IDisposableReadOnlyList<IReparentingStatusProvider> extensions = systemContext.GetExtensions<IReparentingStatusProvider>())
        {
          if (extensions != null)
          {
            if (extensions.Count == 1)
            {
              IReparentingStatusProvider reparentingStatusProvider = extensions.First<IReparentingStatusProvider>();
              if (this.Request.RequestContext.HttpContext.Items[(object) HttpContextConstants.ServiceHostRouteContext] is HostRouteContext hostRouteContext)
              {
                Guid hostId = hostRouteContext.HostId;
                statusProperties = reparentingStatusProvider.GetReparentingStatusProperties(systemContext, reparentingRequestId, hostId);
              }
            }
            else if (extensions.Count > 1)
              systemContext.TraceException(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, (Exception) new ErrorController.MultipleReparentingStatusProviderException(string.Format(ErrorController.MultipleReparentingStatusProviderException.ExceptionMessage, (object) extensions.Count)));
          }
        }
      }
      return statusProperties;
    }

    private bool IsExceptionMessageInformational(string exceptionInnerMsg)
    {
      foreach (string informationalErrorMessage in this.InformationalErrorMessages)
      {
        if (string.Equals(exceptionInnerMsg, informationalErrorMessage, StringComparison.InvariantCultureIgnoreCase))
          return true;
      }
      return false;
    }

    public ActionResult NotFound() => throw new HttpException(404, PlatformResources.PageNotFound);

    private class MultipleReparentingStatusProviderException : Exception
    {
      public static string ExceptionMessage = "Found {0} implementations of the Reparenting Status Provider extension. Expected a single implementation";

      public MultipleReparentingStatusProviderException(string message)
        : base(message)
      {
      }
    }
  }
}
