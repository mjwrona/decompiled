// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ExtensionEventCallbackService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  internal class ExtensionEventCallbackService : IExtensionEventCallbackService, IVssFrameworkService
  {
    private const string c_callbackContribPreInstallTarget = "ms.vss-extmgmt-web.preinstall-callbacks";
    private const string c_callbackContribPreUninstallTarget = "ms.vss-extmgmt-web.preuninstall-callbacks";
    private const string c_callbackContribType = "ms.vss-extmgmt-web.extension-callback";
    private const string c_callbackContribServiceInstanceTypePropName = "serviceInstanceType";
    private const string c_callbackContribContributionTypePropName = "contributionType";
    private const string c_callbackContribFlagsPropName = "flags";
    private const string c_callbackContribUriPropName = "uri";
    private const string c_area = "ExtensionEventCallbackService";
    private const string c_layer = "Service";
    private static readonly Dictionary<ExtensionOperation, IEnumerable<string>> s_callbackFilterTargets = new Dictionary<ExtensionOperation, IEnumerable<string>>()
    {
      {
        ExtensionOperation.PreInstall,
        (IEnumerable<string>) new string[1]
        {
          "ms.vss-extmgmt-web.preinstall-callbacks"
        }
      },
      {
        ExtensionOperation.PreUninstall,
        (IEnumerable<string>) new string[1]
        {
          "ms.vss-extmgmt-web.preuninstall-callbacks"
        }
      }
    };
    private static readonly HashSet<string> s_callbackFilterTypes = new HashSet<string>((IEnumerable<string>) new string[1]
    {
      "ms.vss-extmgmt-web.extension-callback"
    }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void PerformEventCallbacks(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      ExtensionManifest extensionManifest,
      PublishedExtension publishedExtension,
      ExtensionOperation extensionOperation,
      Guid registrationId)
    {
      requestContext.TraceEnter(10013260, nameof (ExtensionEventCallbackService), "Service", nameof (PerformEventCallbacks));
      try
      {
        IEnumerable<string> contributionIds = (IEnumerable<string>) null;
        ExtensionEventCallbackService.s_callbackFilterTargets.TryGetValue(extensionOperation, out contributionIds);
        if (contributionIds != null)
        {
          IContributionService service = requestContext.GetService<IContributionService>();
          foreach (Contribution queryContribution in service.QueryContributions(requestContext, contributionIds, ExtensionEventCallbackService.s_callbackFilterTypes, ContributionQueryOptions.IncludeChildren))
          {
            bool flag = false;
            PublishedExtensionFlags result1;
            if (publishedExtension != null && Enum.TryParse<PublishedExtensionFlags>(queryContribution.GetProperty<string>("flags", string.Empty), true, out result1))
              flag = (publishedExtension.Flags & result1) == result1;
            if (!flag)
            {
              string contributionTypeFilter = queryContribution.GetProperty<string>("contributionType");
              flag = !string.IsNullOrEmpty(contributionTypeFilter) && extensionManifest.Contributions.Any<Contribution>((Func<Contribution, bool>) (c => contributionTypeFilter.Equals(c.Type, StringComparison.OrdinalIgnoreCase)));
            }
            if (flag)
            {
              ContributionProviderDetails contributionProviderDetails = service.QueryContributionProviderDetails(requestContext, queryContribution.Id);
              JObject replacementValues = new JObject();
              Guid result2 = Guid.Empty;
              string input;
              if (contributionProviderDetails.Properties.TryGetValue("::ServiceInstanceType", out input) && Guid.TryParse(input, out result2) && result2 != Guid.Empty)
                replacementValues["$ServiceInstanceType"] = (JToken) input;
              string templateUriProperty = queryContribution.GetTemplateUriProperty(requestContext, "uri", (object) replacementValues, PlatformMustacheExtensions.Parser);
              if (!string.IsNullOrEmpty(templateUriProperty))
              {
                Guid result3 = Guid.Empty;
                string property = queryContribution.GetProperty<string>("serviceInstanceType");
                if (string.IsNullOrEmpty(property) || !Guid.TryParse(property, out result3))
                  result3 = result2;
                this.PerformEventCallback(requestContext, publisherName, extensionName, version, extensionManifest, extensionOperation, (string) null, templateUriProperty, result3, this.IsEventCallbackDecisionPoint(extensionOperation));
              }
            }
          }
        }
        if (extensionManifest == null || extensionManifest.EventCallbacks == null)
          return;
        ExtensionEventCallback extensionEventCallback = (ExtensionEventCallback) null;
        switch (extensionOperation)
        {
          case ExtensionOperation.PreInstall:
            extensionEventCallback = extensionManifest.EventCallbacks.PreInstall;
            break;
          case ExtensionOperation.PostInstall:
            extensionEventCallback = extensionManifest.EventCallbacks.PostInstall;
            break;
          case ExtensionOperation.PostUninstall:
            extensionEventCallback = extensionManifest.EventCallbacks.PostUninstall;
            break;
          case ExtensionOperation.PostEnable:
            extensionEventCallback = extensionManifest.EventCallbacks.PostEnable;
            break;
          case ExtensionOperation.PostDisable:
            extensionEventCallback = extensionManifest.EventCallbacks.PostDisable;
            break;
          case ExtensionOperation.PostUpdate:
            extensionEventCallback = extensionManifest.EventCallbacks.PostUpdate;
            break;
        }
        if (extensionEventCallback == null)
          return;
        if (string.IsNullOrWhiteSpace(extensionEventCallback.Uri))
          return;
        string eventCallbackUrl;
        try
        {
          eventCallbackUrl = extensionManifest.GetTemplateUriProperty(extensionEventCallback.Uri, new object(), PlatformMustacheExtensions.Parser);
        }
        catch (InvalidMustacheTemplateContextException ex)
        {
          eventCallbackUrl = (string) null;
          if (requestContext.ExecutionEnvironment.IsHostedDeployment)
            requestContext.TraceException(10013259, nameof (ExtensionEventCallbackService), "Service", (Exception) ex);
        }
        if (string.IsNullOrEmpty(eventCallbackUrl))
          return;
        string applicationSessionToken = this.GetApplicationSessionToken(requestContext, registrationId);
        this.PerformEventCallback(requestContext, publisherName, extensionName, version, extensionManifest, extensionOperation, applicationSessionToken, eventCallbackUrl, Guid.Empty, this.IsEventCallbackDecisionPoint(extensionOperation));
      }
      finally
      {
        requestContext.TraceLeave(10013260, nameof (ExtensionEventCallbackService), "Service", nameof (PerformEventCallbacks));
      }
    }

    private void PerformEventCallback(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      ExtensionManifest extensionManifest,
      ExtensionOperation extensionOperation,
      string applicationToken,
      string eventCallbackUrl,
      Guid callbackServiceInstanceType,
      bool isDecisionPoint)
    {
      requestContext.TraceEnter(10013261, nameof (ExtensionEventCallbackService), "Service", nameof (PerformEventCallback));
      try
      {
        string locationServiceUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, LocationServiceConstants.SelfReferenceIdentifier, AccessMappingConstants.ClientAccessMappingMoniker);
        bool flag = false;
        ExtensionEventHttpClient extensionEventHttpClient;
        if (callbackServiceInstanceType != Guid.Empty)
        {
          flag = true;
          extensionEventHttpClient = (requestContext.ClientProvider as ICreateClient).CreateClient<ExtensionEventHttpClient>(requestContext, new Uri(eventCallbackUrl), "", (ApiResourceLocationCollection) null, false);
        }
        else
          extensionEventHttpClient = new ExtensionEventHttpClient(new Uri(eventCallbackUrl), new VssHttpRequestSettings()
          {
            SendTimeout = new TimeSpan(0, 0, 20)
          });
        ExtensionEventCallbackData data = new ExtensionEventCallbackData()
        {
          Operation = extensionOperation,
          PublisherName = publisherName,
          ExtensionName = extensionName,
          Version = version,
          Target = new ExtensionEventTarget()
          {
            Id = requestContext.ServiceHost.InstanceId,
            Name = requestContext.ServiceHost.Name,
            Uri = locationServiceUrl
          }
        };
        requestContext.Trace(10013263, TraceLevel.Info, nameof (ExtensionEventCallbackService), "Service", "Calling extension event callback: url={0}, op={1}, pub={2}, ext={3}, host={4}", (object) eventCallbackUrl, (object) data.Operation, (object) data.PublisherName, (object) data.ExtensionName, (object) data.Target.Id);
        Task<ExtensionEventCallbackResult> task = extensionEventHttpClient.SendExtensionEventCallbackAsync(eventCallbackUrl, applicationToken, data, flag ? (object) requestContext.Elevate() : (object) (IVssRequestContext) null);
        if (!isDecisionPoint)
          return;
        ExtensionEventCallbackResult result = task.Result;
        if (result == null || !result.Allow)
        {
          requestContext.Trace(10013264, TraceLevel.Info, nameof (ExtensionEventCallbackService), "Service", "Event callback decision point returned DENY");
          throw new ExtensionEventCallbackDeniedException(result != null ? (result.Message == null || string.IsNullOrWhiteSpace(result.Message) ? ExtensionResources.ExtensionEventCallbackDeniedMessageFormat((object) ExtensionResources.ExtensionEventCallbackDeniedNoReason()) : result.Message) : ExtensionResources.ExtensionEventCallbackDeniedMessageFormat((object) ExtensionResources.ExtensionEventCallbackDeniedNoResult()));
        }
        requestContext.Trace(10013264, TraceLevel.Info, nameof (ExtensionEventCallbackService), "Service", "Event callback decision point returned ALLOW");
      }
      catch (Exception ex)
      {
        requestContext.Trace(10013262, TraceLevel.Error, nameof (ExtensionEventCallbackService), "Service", "Failed to notify extension: {0}.{1} Template: {2} of operation {3}", (object) publisherName, (object) extensionName, (object) eventCallbackUrl, (object) extensionOperation.ToString());
        requestContext.TraceException(10013264, nameof (ExtensionEventCallbackService), "Service", ex);
        if (!isDecisionPoint)
          return;
        Exception exception = ex;
        if (exception is AggregateException)
          exception = exception.InnerException;
        if (exception is ExtensionEventCallbackDeniedException)
        {
          throw;
        }
        else
        {
          string str;
          if (exception is VssServiceResponseException)
          {
            VssServiceResponseException responseException = exception as VssServiceResponseException;
            str = string.Format("{0} ({1})", (object) responseException.Message, (object) responseException.HttpStatusCode);
          }
          else
            str = exception.Message;
          throw new ExtensionEventCallbackDeniedException(ExtensionResources.ExtensionEventCallbackDeniedMessageFormat((object) str));
        }
      }
      finally
      {
        requestContext.TraceLeave(10013261, nameof (ExtensionEventCallbackService), "Service", nameof (PerformEventCallback));
      }
    }

    private string GetApplicationSessionToken(
      IVssRequestContext requestContext,
      Guid registrationId)
    {
      string applicationSessionToken = (string) null;
      if (registrationId != Guid.Empty && !requestContext.IsSystemContext && IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) requestContext.GetUserIdentity()))
      {
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        AppSessionTokenResult sessionTokenResult = vssRequestContext.GetService<IDelegatedAuthorizationService>().IssueAppSessionToken(vssRequestContext, registrationId, new Guid?(requestContext.GetUserId()));
        applicationSessionToken = !sessionTokenResult.HasError ? sessionTokenResult.AppSessionToken : throw new AppSessionTokenException(sessionTokenResult.AppSessionTokenError);
      }
      return applicationSessionToken;
    }

    private bool IsEventCallbackDecisionPoint(ExtensionOperation operation) => operation == ExtensionOperation.PreInstall || operation == ExtensionOperation.PreUninstall;
  }
}
