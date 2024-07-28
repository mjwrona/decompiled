// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsController
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Analytics.ML;
using Microsoft.VisualStudio.Services.Analytics.Model;
using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using Microsoft.VisualStudio.Services.Analytics.Telemetry;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Routing;

namespace Microsoft.VisualStudio.Services.Analytics
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "Analytics", ResourceName = "Analytics")]
  [ValidateViewAnalyticsPermission]
  [ValidateAnalyticsEnabledAndModelReadyAttribute]
  [AnalyticsODataFormatting]
  [ODataRouting]
  [ApiExplorerSettings(IgnoreApi = true)]
  public class AnalyticsController : TfsProjectApiController, IOverrideLoggingMethodNames
  {
    internal const int DefaultPageSize = 10000;
    internal const string PageSizeConfigPathInRegistry = "/Service/Analytics/Settings/PageSize/";
    internal const int DefaultMaxSize = 300000;
    internal const int MaxMaxSize = 10000000;
    internal const string DefaultMaxSizePathInRegistry = "/Service/Analytics/Settings/DefaultMaxSize";
    internal const string MaxMaxSizePathInRegistry = "/Service/Analytics/Settings/MaxMaxSize";
    internal static readonly RegistryQuery s_oDataQueryElapsedTimeThresholdInMsKey = new RegistryQuery("/Service/Analytics/CleanupService/ODataQueryElapsedTimeThresholdInMs");
    internal const string PreferHeaderName = "Prefer";
    internal const string AnnotationsHeader = "odata.include-annotations=\"vsts.*\"";
    internal const string TfsPreferHeaderName = "X-TFS-Prefer";
    internal const string MaxSizeHeaderName = "VSTS.Analytics.MaxSize";
    internal const string ExpectSinglePageHeaderName = "VSTS.Analytics.ExpectSinglePage";
    private static readonly Regex s_widgetMilestoneRegex = new Regex("(-(m|M)\\d{3})?", RegexOptions.Compiled);
    private ODataQueryOptions _odataQueryOptions;
    private IEdmEntitySet _entitySet;
    private AnalyticsODataRequestMessage _odataRequest;
    private const string AnalyticsPrefix = "Analytics";
    private static readonly string s_layer = "ODataController";

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ODataException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<EntitySetNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<AnalyticsAccessCheckException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<InvalidMaxSizeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<AnalyticsNotEnabledException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<QueryExceedsPreferedMaxSizeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ModelNotReadyException>(HttpStatusCode.ServiceUnavailable);
      exceptionMap.AddStatusCode<ModelSyncingException>(HttpStatusCode.ServiceUnavailable);
      exceptionMap.AddStatusCode<ODataIntegerOverflowException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ODataUnsupportedFeatureException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<QueryExceedsMaxWidthException>(HttpStatusCode.RequestEntityTooLarge);
      exceptionMap.AddStatusCode<QueryExceedsThresholdTimeException>(HttpStatusCode.ServiceUnavailable);
    }

    [ClientResponseType(typeof (object), null, null)]
    [HttpGet]
    [ClientAdditionalRouteParameter(typeof (string), "entityType", 0, "The type of entities to query with OData.")]
    [ClientInclude(RestClientLanguages.TypeScript)]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage Get(
      [ClientQueryParameter] string apply = null,
      [ClientQueryParameter] string count = null,
      [ClientQueryParameter] string expand = null,
      [ClientQueryParameter] string filter = null,
      [ClientQueryParameter] string format = null,
      [ClientQueryParameter] string orderby = null,
      [ClientQueryParameter] string search = null,
      [ClientQueryParameter] string select = null,
      [ClientQueryParameter] string skip = null,
      [ClientQueryParameter] string top = null)
    {
      this.TfsRequestContext.TraceEnter(12013003, this.Area, this.Layer, nameof (Get));
      try
      {
        this.TfsRequestContext.ValidateAnalyticsEnabled();
        AnalyticsService service = this.TfsRequestContext.GetService<AnalyticsService>();
        this.TfsRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>();
        this.EnsureQueryOptions();
        IEnumerable<string> values;
        if (this.Request.Headers.TryGetValues("Prefer", out values) && values.Any<string>((Func<string, bool>) (v => !string.IsNullOrWhiteSpace(v))))
        {
          this.Request.Headers.Remove("Prefer");
          this.Request.Headers.Add("Prefer", string.Join(",", values.Where<string>((Func<string, bool>) (v => !string.IsNullOrWhiteSpace(v)))) + ",odata.include-annotations=\"vsts.*\"");
        }
        else
          this.Request.Headers.Add("Prefer", "odata.include-annotations=\"vsts.*\"");
        this.TfsRequestContext.EmbedProjectInfoToRequestContext(this.ProjectInfo);
        IEdmEntityType edmEntityType = this._entitySet.EntityType();
        ODataQuerySizeOptions querySizeOptions1 = this.GetQuerySizeOptions(edmEntityType.Name);
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        HttpRequestMessage request = this.Request;
        MediaTypeFormatterCollection formatters = this.Configuration.Formatters;
        IEdmEntityType entityType = edmEntityType;
        ODataQueryOptions odataQueryOptions = this._odataQueryOptions;
        ODataQuerySizeOptions querySizeOptions2 = querySizeOptions1;
        ProjectInfo projectInfo = this.ProjectInfo;
        return service.QueryEntitySet(tfsRequestContext, request, formatters, entityType, odataQueryOptions, querySizeOptions2, projectInfo) ?? throw new EntitySetNotFoundException(edmEntityType.Name);
      }
      catch (ArgumentException ex) when (ex.Message.StartsWith("DbArithmeticExpression arguments must have a numeric common type."))
      {
        throw new ODataUnsupportedFeatureException(AnalyticsResources.ARITHMETIC_EXPRESSIONS_WITHOUT_COMMON_NUMERIC_TYPES(), (Exception) ex);
      }
      catch (Exception ex) when (ex.Message.StartsWith("Cannot continue the execution because the session is in the kill state."))
      {
        Exception innerException = AnalyticsController.GetInnerException(ex);
        throw new QueryExceedsThresholdTimeException(AnalyticsResources.SESSION_TERMINATED_THRESHOLD_ELAPSED_TIME_EXCEEDED((object) this.GetTotalElapsedTimeThresholdInMinutes(), (object) "https://go.microsoft.com/fwlink/?linkid=861060"), innerException);
      }
      catch (Exception ex) when (AnalyticsController.IsWrapperException(ex))
      {
        Exception exception = AnalyticsController.GetInnerException(ex);
        if (this.TfsRequestContext.IsCanceled)
          throw new RequestCanceledException(AnalyticsResources.WRAPPER_EXCEPTION_WHEN_REQUEST_CANCELLED((object) ex.GetType()), exception);
        if (exception is SqlException sqlException)
        {
          if (sqlException.Number == 8115)
            exception = (Exception) new ODataIntegerOverflowException(sqlException.Message, exception);
          if (sqlException.Number == 511)
            exception = (Exception) new QueryExceedsMaxWidthException(AnalyticsResources.QUERY_EXCEEDS_MAX_WIDTH(), exception);
        }
        ExceptionDispatchInfo.Capture(exception).Throw();
        throw;
      }
      finally
      {
        this.LogCustomerIntelligence();
        this.TfsRequestContext.TraceLeave(12013004, this.Area, this.Layer, nameof (Get));
      }
    }

    [HttpGet]
    public HttpResponseMessage GetByPath()
    {
      this.TfsRequestContext.TraceEnter(12013038, this.Area, this.Layer, nameof (GetByPath));
      try
      {
        this.TfsRequestContext.ValidateAnalyticsEnabled();
        this.EnsureQueryOptions();
        IEdmEntityType entityType = this._entitySet.EntityType();
        return this.TfsRequestContext.GetService<AnalyticsService>().QueryEntity(this.TfsRequestContext, this.Request, this.Configuration.Formatters, entityType, this.ProjectInfo);
      }
      catch (ArgumentException ex) when (ex.Message.StartsWith("DbArithmeticExpression arguments must have a numeric common type."))
      {
        throw new ODataUnsupportedFeatureException(AnalyticsResources.ARITHMETIC_EXPRESSIONS_WITHOUT_COMMON_NUMERIC_TYPES(), (Exception) ex);
      }
      catch (Exception ex) when (ex.Message.StartsWith("Cannot continue the execution because the session is in the kill state."))
      {
        Exception innerException = AnalyticsController.GetInnerException(ex);
        throw new QueryExceedsThresholdTimeException(AnalyticsResources.SESSION_TERMINATED_THRESHOLD_ELAPSED_TIME_EXCEEDED((object) this.GetTotalElapsedTimeThresholdInMinutes(), (object) "https://go.microsoft.com/fwlink/?linkid=861060"), innerException);
      }
      catch (Exception ex) when (AnalyticsController.IsWrapperException(ex))
      {
        Exception exception = AnalyticsController.GetInnerException(ex);
        if (this.TfsRequestContext.IsCanceled)
          throw new RequestCanceledException(AnalyticsResources.WRAPPER_EXCEPTION_WHEN_REQUEST_CANCELLED((object) ex.GetType()), exception);
        if (exception is SqlException sqlException)
        {
          if (sqlException.Number == 8115)
            exception = (Exception) new ODataIntegerOverflowException(sqlException.Message, exception);
          if (sqlException.Number == 511)
            exception = (Exception) new QueryExceedsMaxWidthException(AnalyticsResources.QUERY_EXCEEDS_MAX_WIDTH(), exception);
        }
        ExceptionDispatchInfo.Capture(exception).Throw();
        throw;
      }
      finally
      {
        this.LogCustomerIntelligence();
        this.TfsRequestContext.TraceLeave(12013039, this.Area, this.Layer, nameof (GetByPath));
      }
    }

    [HttpGet]
    [ClientResponseType(typeof (object), null, null)]
    [ClientInclude(RestClientLanguages.TypeScript)]
    [PublicProjectRequestRestrictions]
    public IEnumerable<Tag> RecommendTags(int WorkItemId)
    {
      if (this.ProjectInfo == null)
        throw new ODataException(AnalyticsResources.PROJECT_SCOPE_REQUIRED());
      return this.TfsRequestContext.IsFeatureEnabled("Analytics.Transform.WorkItemTagsPredict") ? (IEnumerable<Tag>) this.TfsRequestContext.GetService<IMLService>().PredictTagsForWorkItem(this.TfsRequestContext, this.ProjectInfo.Id, WorkItemId) : (IEnumerable<Tag>) new List<Tag>();
    }

    private void LogCustomerIntelligence()
    {
      MashupFlavor powerBiFlavor = UserAgentClassifier.GetPowerBIFlavor(this.TfsRequestContext, this.Request);
      string name = this._entitySet.EntityType().Name;
      if (AnalyticsController.TryGetSupportedWidgetName(this.TfsRequestContext.Command(), out string _))
      {
        if (AnalyticsController.IsWebAutomationCommand(this.TfsRequestContext.Command()))
          return;
        ODataEngagementPublisher.PublishInProductExperienceQueryEvent(this.TfsRequestContext, name);
      }
      else if (powerBiFlavor == MashupFlavor.Views)
        ViewsEngagementPublisher.PublishLoadViewEvent(this.TfsRequestContext, this.TfsRequestContext.RequestUri().ParseQueryString());
      else if (powerBiFlavor != MashupFlavor.None)
        ODataEngagementPublisher.PublishMashupQueryEvent(this.TfsRequestContext, name, powerBiFlavor);
      else
        ODataEngagementPublisher.PublishODataQueryEvent(this.TfsRequestContext, name);
    }

    private void EnsureQueryOptions()
    {
      this._entitySet = this._entitySet ?? this.Request.EntitySet();
      this._odataQueryOptions = this._odataQueryOptions ?? this.Request.CreateODataQueryOptions(this._entitySet.EntityType());
      this._odataRequest = new AnalyticsODataRequestMessage(this.Request);
      this.Request.GetRequestContext().Url = (UrlHelper) new AnalyticsUrlHelper(this.Request);
    }

    private ODataQuerySizeOptions GetQuerySizeOptions(string typeName)
    {
      int? nullable1 = this.GetPageSizeForEntityType(typeName);
      int? maxPageSize = this._odataRequest.PreferHeader().MaxPageSize;
      if (maxPageSize.HasValue)
      {
        int? nullable2 = maxPageSize;
        int? nullable3 = nullable1;
        if (nullable2.GetValueOrDefault() < nullable3.GetValueOrDefault() & nullable2.HasValue & nullable3.HasValue)
          nullable1 = maxPageSize;
      }
      return new ODataQuerySizeOptions()
      {
        MaxSize = this.GetMaxSizeCheck(),
        ExpectSinglePage = this.IsExpectForSinglePageSet(),
        PageSize = nullable1
      };
    }

    private bool TryGetPreferOption(string optionName, out string value)
    {
      value = (string) null;
      IEnumerable<string> values;
      if (this.Request.Headers.TryGetValues("X-TFS-Prefer", out values) || this.Request.Headers.TryGetValues("Prefer", out values))
      {
        string str1 = values.SelectMany<string, string>((Func<string, IEnumerable<string>>) (v => (IEnumerable<string>) v.Split(new string[3]
        {
          ";",
          " ",
          ","
        }, StringSplitOptions.RemoveEmptyEntries))).FirstOrDefault<string>((Func<string, bool>) (h => h.StartsWith(optionName, StringComparison.OrdinalIgnoreCase)));
        if (str1 != null)
        {
          string str2 = str1.Replace(optionName, string.Empty).Replace("=", string.Empty).Trim().Trim(';');
          if (!string.IsNullOrWhiteSpace(str2))
          {
            value = str2;
            return true;
          }
        }
      }
      return false;
    }

    private bool IsExpectForSinglePageSet()
    {
      string str;
      if (!this.TryGetPreferOption("VSTS.Analytics.ExpectSinglePage", out str))
        return false;
      bool result = false;
      return bool.TryParse(str, out result) & result;
    }

    private int? GetMaxSizeCheck()
    {
      string s;
      if (this.TryGetPreferOption("VSTS.Analytics.MaxSize", out s))
      {
        int maxMaxSize = this.GetMaxMaxSize();
        int result;
        if (int.TryParse(s, out result))
        {
          if (result == 0)
            return new int?(this.GetDefaultMaxSize());
          if (result >= 0 && result <= maxMaxSize)
            return new int?(result);
        }
        throw new InvalidMaxSizeException(maxMaxSize);
      }
      return new int?();
    }

    internal static Exception GetInnerException(Exception exception) => AnalyticsController.IsWrapperException(exception) && exception.InnerException != null ? AnalyticsController.GetInnerException(exception.InnerException) : exception;

    private static bool IsWrapperException(Exception exception) => exception is EntityCommandExecutionException || exception is TargetInvocationException;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.Request != null)
        this.Request.DeleteRequestContainer(true);
      base.Dispose(disposing);
    }

    internal int? GetPageSizeForEntityType(string typeName)
    {
      IVssRegistryService service = this.TfsRequestContext.GetService<IVssRegistryService>();
      string str = Path.Combine("/Service/Analytics/Settings/PageSize/", typeName);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) str;
      int num = service.GetValue<int>(tfsRequestContext, in local, true, 10000);
      return num != 0 ? new int?(num) : new int?();
    }

    internal int GetDefaultMaxSize() => this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<int>(this.TfsRequestContext, (RegistryQuery) "/Service/Analytics/Settings/DefaultMaxSize", 300000);

    internal int GetMaxMaxSize() => this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<int>(this.TfsRequestContext, (RegistryQuery) "/Service/Analytics/Settings/MaxMaxSize", 10000000);

    internal double GetTotalElapsedTimeThresholdInMinutes() => Math.Round(TimeSpan.FromMilliseconds((double) this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<int>(this.TfsRequestContext, in AnalyticsController.s_oDataQueryElapsedTimeThresholdInMsKey, true, 600000)).TotalMinutes, 2);

    internal static bool TryGetSupportedWidgetName(string command, out string supportedWidget)
    {
      supportedWidget = (string) null;
      if (string.IsNullOrWhiteSpace(command) || !command.StartsWith("vsts-widget-", StringComparison.OrdinalIgnoreCase))
        return false;
      supportedWidget = command.Substring("vsts-widget-".Length);
      supportedWidget = AnalyticsController.s_widgetMilestoneRegex.Replace(supportedWidget, string.Empty);
      supportedWidget = supportedWidget.Replace("-untracked", string.Empty);
      return true;
    }

    internal static bool IsWebAutomationCommand(string command) => !string.IsNullOrWhiteSpace(command) && command.ToLower().Contains("-untracked");

    [ClientIgnore]
    string IOverrideLoggingMethodNames.GetLoggingMethodName(
      string methodName,
      HttpActionContext actionContext)
    {
      if (methodName.EndsWith("Get"))
      {
        try
        {
          this.EnsureQueryOptions();
          string supportedWidget;
          if (AnalyticsController.TryGetSupportedWidgetName(this.TfsRequestContext.Command(), out supportedWidget))
            return "Analytics.Widget." + supportedWidget + "." + this._entitySet.Name;
          MashupFlavor powerBiFlavor = UserAgentClassifier.GetPowerBIFlavor(this.TfsRequestContext, this.Request);
          switch (powerBiFlavor)
          {
            case MashupFlavor.None:
              if (new ODataQueryClassifier(this._odataQueryOptions).IsWidgetLike())
                return "Analytics.Widget.External." + this._entitySet.Name;
              break;
            case MashupFlavor.Generic:
              return "Analytics.Mashup." + this._entitySet.Name;
            default:
              return string.Format("{0}.PBI.{1}.{2}", (object) "Analytics", (object) powerBiFlavor, (object) this._entitySet.Name);
          }
        }
        catch (Exception ex)
        {
          this.TfsRequestContext.TraceException(12016005, this.Area, this.Layer, ex);
        }
      }
      return methodName;
    }

    protected string Area => "AnalyticsModel";

    protected string Layer => AnalyticsController.s_layer;
  }
}
