// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.ServiceBusResourceOperations
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using Microsoft.Azure.NotificationHubs.Tracing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  internal sealed class ServiceBusResourceOperations
  {
    internal const string FeedContentType = "application/atom+xml;type=feed;charset=utf-8";
    private static string ConflictOperationInProgressSubCode = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SubCode={0}", new object[1]
    {
      (object) ExceptionErrorCodes.ConflictOperationInProgress.ToString("D")
    });

    public static IAsyncResult BeginGetInformation(
      TrackingContext trackingContext,
      IEnumerable<Uri> addresses,
      NamespaceManagerSettings settings,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      return (IAsyncResult) new ServiceBusResourceOperations.GetInformationAsyncResult(trackingContext, addresses, settings, timeout, callback, state);
    }

    public static IDictionary<string, string> EndGetInformation(IAsyncResult result) => AsyncResult<ServiceBusResourceOperations.GetInformationAsyncResult>.End(result).Information;

    public static IAsyncResult BeginCreate<TEntityDescription>(
      TEntityDescription resourceDescription,
      string[] resourceNames,
      NamespaceManager namespaceManager,
      AsyncCallback callback,
      object state)
      where TEntityDescription : EntityDescription, IResourceDescription
    {
      return ServiceBusResourceOperations.BeginCreateOrUpdate<TEntityDescription>(TrackingContext.GetInstance(Guid.NewGuid(), resourceNames[0]), resourceDescription, (IResourceDescription[]) null, resourceNames, namespaceManager.addresses, namespaceManager.Settings.InternalOperationTimeout, false, false, (IDictionary<string, string>) null, namespaceManager.Settings, callback, state);
    }

    public static Task<TEntityDescription> CreateAsync<TEntityDescription>(
      TEntityDescription resourceDescription,
      IResourceDescription[] descriptions,
      string[] resourceNames,
      NamespaceManager namespaceManager)
      where TEntityDescription : EntityDescription, IResourceDescription
    {
      TrackingContext trackingContext = TrackingContext.GetInstance(Guid.NewGuid(), resourceNames[0]);
      return Task.Factory.FromAsync<TEntityDescription>((Func<AsyncCallback, object, IAsyncResult>) ((callback, state) => ServiceBusResourceOperations.BeginCreateOrUpdate<TEntityDescription>(trackingContext, resourceDescription, descriptions, resourceNames, namespaceManager.addresses, namespaceManager.Settings.InternalOperationTimeout, false, false, (IDictionary<string, string>) null, namespaceManager.Settings, callback, state)), new Func<IAsyncResult, TEntityDescription>(ServiceBusResourceOperations.EndCreate<TEntityDescription>), (object) null);
    }

    public static Task<TEntityDescription> CreateAsync<TEntityDescription>(
      TEntityDescription resourceDescription,
      string[] resourceNames,
      NamespaceManager manager)
      where TEntityDescription : EntityDescription, IResourceDescription
    {
      return Task.Factory.FromAsync<TEntityDescription, string[], NamespaceManager, TEntityDescription>(new Func<TEntityDescription, string[], NamespaceManager, AsyncCallback, object, IAsyncResult>(ServiceBusResourceOperations.BeginCreate<TEntityDescription>), new Func<IAsyncResult, TEntityDescription>(ServiceBusResourceOperations.EndCreate<TEntityDescription>), resourceDescription, resourceNames, manager, (object) null);
    }

    public static IAsyncResult BeginCreate<TEntityDescription>(
      TEntityDescription resourceDescription,
      IResourceDescription[] descriptions,
      string[] resourceNames,
      NamespaceManager namespaceManager,
      AsyncCallback callback,
      object state)
      where TEntityDescription : EntityDescription, IResourceDescription
    {
      return ServiceBusResourceOperations.BeginCreateOrUpdate<TEntityDescription>(TrackingContext.GetInstance(Guid.NewGuid(), resourceNames[0]), resourceDescription, descriptions, resourceNames, namespaceManager.addresses, namespaceManager.Settings.InternalOperationTimeout, false, false, (IDictionary<string, string>) null, namespaceManager.Settings, callback, state);
    }

    public static IAsyncResult BeginCreateRegistrationId(
      string[] resourceNames,
      NamespaceManager namespaceManager,
      AsyncCallback callback,
      object state)
    {
      return (IAsyncResult) new ServiceBusResourceOperations.CreateRegistrationIdAsyncResult(TrackingContext.GetInstance(Guid.NewGuid(), resourceNames[0]), (IResourceDescription[]) null, resourceNames, namespaceManager.addresses, namespaceManager.Settings.InternalOperationTimeout, false, false, (IDictionary<string, string>) null, namespaceManager.Settings, callback, state);
    }

    public static string EndCreateRegistrationId(IAsyncResult asyncResult) => AsyncResult<ServiceBusResourceOperations.CreateRegistrationIdAsyncResult>.End(asyncResult).Result;

    public static IAsyncResult BeginGetInstallation(
      TrackingContext trackingContext,
      string installationId,
      string hubPath,
      IEnumerable<Uri> addresses,
      TimeSpan timeout,
      NamespaceManagerSettings settings,
      AsyncCallback callback,
      object state)
    {
      return (IAsyncResult) new ServiceBusResourceOperations.GetInstallationAsyncResult(installationId, hubPath, trackingContext, addresses, settings, timeout, callback, state);
    }

    public static string EndGetInstallation(IAsyncResult asyncResult) => AsyncResult<ServiceBusResourceOperations.GetInstallationAsyncResult>.End(asyncResult).Result;

    public static IAsyncResult BeginCreateOrUpdateInstallation(
      TrackingContext trackingContext,
      string jsonPayload,
      string installationId,
      string method,
      string hubPath,
      IEnumerable<Uri> addresses,
      TimeSpan timeout,
      bool isAnonymousAccessible,
      IDictionary<string, string> queryParametersAndValues,
      NamespaceManagerSettings settings,
      AsyncCallback callback,
      object state)
    {
      return (IAsyncResult) new ServiceBusResourceOperations.CreateOrUpdateInstallationAsyncResult(trackingContext, jsonPayload, installationId, method, hubPath, addresses, timeout, isAnonymousAccessible, queryParametersAndValues, settings, callback, state);
    }

    public static string EndCreateOrUpdateInstallation(IAsyncResult asyncResult) => AsyncResult<ServiceBusResourceOperations.CreateOrUpdateInstallationAsyncResult>.End(asyncResult).Result;

    public static IAsyncResult BeginDeleteInstallation(
      TrackingContext trackingContext,
      string installationId,
      string hubPath,
      IEnumerable<Uri> addresses,
      NamespaceManagerSettings settings,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      return (IAsyncResult) new ServiceBusResourceOperations.DeleteInstallationAsyncResult(trackingContext, installationId, hubPath, addresses, (Dictionary<string, string>) null, timeout, settings, callback, state);
    }

    public static void EndDeleteInstallation(IAsyncResult asyncResult) => AsyncResult<ServiceBusResourceOperations.DeleteInstallationAsyncResult>.End(asyncResult);

    public static Task<TEntityDescription> UpdateAsync<TEntityDescription>(
      TEntityDescription resourceDescription,
      string[] resourceNames,
      NamespaceManager manager)
      where TEntityDescription : EntityDescription, IResourceDescription
    {
      return Task.Factory.FromAsync<TEntityDescription, string[], NamespaceManager, TEntityDescription>(new Func<TEntityDescription, string[], NamespaceManager, AsyncCallback, object, IAsyncResult>(ServiceBusResourceOperations.BeginUpdate<TEntityDescription>), new Func<IAsyncResult, TEntityDescription>(ServiceBusResourceOperations.EndUpdate<TEntityDescription>), resourceDescription, resourceNames, manager, (object) null);
    }

    public static IAsyncResult BeginUpdate<TEntityDescription>(
      TEntityDescription resourceDescription,
      string[] resourceNames,
      NamespaceManager namespaceManager,
      AsyncCallback callback,
      object state)
      where TEntityDescription : EntityDescription, IResourceDescription
    {
      return ServiceBusResourceOperations.BeginCreateOrUpdate<TEntityDescription>(TrackingContext.GetInstance(Guid.NewGuid(), resourceNames[0]), resourceDescription, (IResourceDescription[]) null, resourceNames, namespaceManager.addresses, namespaceManager.Settings.InternalOperationTimeout, false, true, (IDictionary<string, string>) null, namespaceManager.Settings, callback, state);
    }

    public static T EndUpdate<T>(IAsyncResult asyncResult) where T : EntityDescription, IResourceDescription => ServiceBusResourceOperations.EndCreate<T>(asyncResult);

    public static IAsyncResult BeginCreateOrUpdate<TEntityDescription>(
      TrackingContext trackingContext,
      TEntityDescription resourceDescription,
      IResourceDescription[] descriptions,
      string[] resourceNames,
      IEnumerable<Uri> addresses,
      TimeSpan timeout,
      bool isAnonymousAccessible,
      bool isUpdate,
      IDictionary<string, string> queryParametersAndValues,
      NamespaceManagerSettings settings,
      AsyncCallback callback,
      object state)
      where TEntityDescription : EntityDescription, IResourceDescription
    {
      return (IAsyncResult) new ServiceBusResourceOperations.CreateOrUpdateAsyncResult<TEntityDescription>(trackingContext, resourceDescription, descriptions, resourceNames, addresses, timeout, isAnonymousAccessible, isUpdate, queryParametersAndValues, settings, callback, state);
    }

    public static T EndCreate<T>(IAsyncResult asyncResult) where T : EntityDescription, IResourceDescription => AsyncResult<ServiceBusResourceOperations.CreateOrUpdateAsyncResult<T>>.End(asyncResult).Result;

    public static Task DeleteAsync(string[] resourceNames, NamespaceManager manager) => Task.Factory.FromAsync<string[], NamespaceManager>(new Func<string[], NamespaceManager, AsyncCallback, object, IAsyncResult>(ServiceBusResourceOperations.BeginDelete), new Action<IAsyncResult>(ServiceBusResourceOperations.EndDelete), resourceNames, manager, (object) null);

    public static IAsyncResult BeginDelete(
      string[] resourceNames,
      NamespaceManager namespaceManager,
      AsyncCallback callback,
      object state)
    {
      return ServiceBusResourceOperations.BeginDelete(TrackingContext.GetInstance(Guid.NewGuid(), resourceNames[0]), (IResourceDescription[]) null, resourceNames, namespaceManager.addresses, namespaceManager.Settings, namespaceManager.Settings.InternalOperationTimeout, callback, state);
    }

    public static IAsyncResult BeginDelete(
      TrackingContext trackingContext,
      IResourceDescription[] descriptions,
      string[] resourceNames,
      IEnumerable<Uri> addresses,
      NamespaceManagerSettings settings,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      return (IAsyncResult) new ServiceBusResourceOperations.DeleteAsyncResult(trackingContext, descriptions, resourceNames, addresses, settings, timeout, callback, state);
    }

    public static IAsyncResult BeginDelete(
      TrackingContext trackingContext,
      IResourceDescription[] descriptions,
      string[] resourceNames,
      IEnumerable<Uri> addresses,
      Dictionary<string, string> additionalHeaders,
      NamespaceManagerSettings settings,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
    {
      return (IAsyncResult) new ServiceBusResourceOperations.DeleteAsyncResult(trackingContext, descriptions, resourceNames, addresses, additionalHeaders, settings, timeout, callback, state);
    }

    public static void EndDelete(IAsyncResult asyncResult) => AsyncResult<ServiceBusResourceOperations.DeleteAsyncResult>.End(asyncResult);

    public static Task<TEntityDescription> GetAsync<TEntityDescription>(
      string[] resourceNames,
      NamespaceManager manager)
      where TEntityDescription : EntityDescription, IResourceDescription
    {
      return Task.Factory.FromAsync<string[], NamespaceManager, TEntityDescription>(new Func<string[], NamespaceManager, AsyncCallback, object, IAsyncResult>(ServiceBusResourceOperations.BeginGet<TEntityDescription>), new Func<IAsyncResult, TEntityDescription>(ServiceBusResourceOperations.EndGet<TEntityDescription>), resourceNames, manager, (object) null);
    }

    public static Task<TEntityDescription> GetAsync<TEntityDescription>(
      IResourceDescription[] descriptions,
      string[] resourceNames,
      NamespaceManager manager)
      where TEntityDescription : EntityDescription, IResourceDescription
    {
      TrackingContext trackingContext = TrackingContext.GetInstance(Guid.NewGuid(), resourceNames[0]);
      return Task.Factory.FromAsync<TEntityDescription>((Func<AsyncCallback, object, IAsyncResult>) ((callback, state) => ServiceBusResourceOperations.BeginGet<TEntityDescription>(trackingContext, descriptions, resourceNames, manager.addresses, manager.Settings, manager.Settings.InternalOperationTimeout, callback, state)), new Func<IAsyncResult, TEntityDescription>(ServiceBusResourceOperations.EndGet<TEntityDescription>), (object) null);
    }

    public static IAsyncResult BeginGet<TEntityDescription>(
      string[] resourceNames,
      NamespaceManager namespaceManager,
      AsyncCallback callback,
      object state)
      where TEntityDescription : EntityDescription, IResourceDescription
    {
      return ServiceBusResourceOperations.BeginGet<TEntityDescription>(TrackingContext.GetInstance(Guid.NewGuid(), resourceNames[0]), (IResourceDescription[]) null, resourceNames, namespaceManager.addresses, namespaceManager.Settings, namespaceManager.Settings.InternalOperationTimeout, callback, state);
    }

    public static IAsyncResult BeginGet<TEntityDescription>(
      TrackingContext trackingContext,
      IResourceDescription[] descriptions,
      string[] resourceNames,
      IEnumerable<Uri> addresses,
      NamespaceManagerSettings settings,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
      where TEntityDescription : EntityDescription, IResourceDescription
    {
      return (IAsyncResult) new ServiceBusResourceOperations.GetAsyncResult<TEntityDescription>(trackingContext, descriptions, resourceNames, addresses, settings, timeout, callback, state);
    }

    public static TEntityDescription EndGet<TEntityDescription>(IAsyncResult asyncResult) where TEntityDescription : EntityDescription, IResourceDescription => AsyncResult<ServiceBusResourceOperations.GetAsyncResult<TEntityDescription>>.End(asyncResult).Result;

    public static TEntityDescription EndGet<TEntityDescription>(
      IAsyncResult asyncResult,
      out string[] resourceNames)
      where TEntityDescription : EntityDescription, IResourceDescription
    {
      ServiceBusResourceOperations.GetAsyncResult<TEntityDescription> getAsyncResult = AsyncResult<ServiceBusResourceOperations.GetAsyncResult<TEntityDescription>>.End(asyncResult);
      resourceNames = getAsyncResult.ResourceNames;
      return getAsyncResult.Result;
    }

    public static Task<SyndicationFeed> GetAllAsync(
      IResourceDescription[] resourceDescriptions,
      NamespaceManager manager)
    {
      return Task.Factory.FromAsync<IResourceDescription[], NamespaceManager, SyndicationFeed>(new Func<IResourceDescription[], NamespaceManager, AsyncCallback, object, IAsyncResult>(ServiceBusResourceOperations.BeginGetAll), new Func<IAsyncResult, SyndicationFeed>(ServiceBusResourceOperations.EndGetAll), resourceDescriptions, manager, (object) null);
    }

    public static Task<SyndicationFeed> GetAllAsync(
      IResourceDescription[] resourceDescriptions,
      string[] resourceNames,
      NamespaceManager manager)
    {
      TrackingContext trackingContext = TrackingContext.GetInstance(Guid.NewGuid());
      return Task.Factory.FromAsync<SyndicationFeed>((Func<AsyncCallback, object, IAsyncResult>) ((callback, state) => ServiceBusResourceOperations.BeginGetAll(trackingContext, resourceDescriptions, resourceNames, manager.addresses, manager.Settings, callback, state)), new Func<IAsyncResult, SyndicationFeed>(ServiceBusResourceOperations.EndGetAll), (object) null);
    }

    public static IAsyncResult BeginGetAll(
      IResourceDescription[] resourceDescriptions,
      NamespaceManager namespaceManager,
      AsyncCallback callback,
      object state)
    {
      return ServiceBusResourceOperations.BeginGetAll(TrackingContext.GetInstance(Guid.NewGuid()), resourceDescriptions, (string[]) null, namespaceManager.addresses, namespaceManager.Settings, callback, state);
    }

    public static IAsyncResult BeginGetAll(
      TrackingContext trackingContext,
      IResourceDescription[] descriptions,
      string[] resourceNames,
      IEnumerable<Uri> addresses,
      NamespaceManagerSettings settings,
      AsyncCallback callback,
      object state)
    {
      return (IAsyncResult) new ServiceBusResourceOperations.GetAllAsyncResult(trackingContext, descriptions, resourceNames, addresses, settings, callback, state);
    }

    public static IAsyncResult BeginGetAll(
      TrackingContext trackingContext,
      string filter,
      IResourceDescription[] descriptions,
      string[] resourceNames,
      IEnumerable<Uri> addresses,
      NamespaceManagerSettings settings,
      AsyncCallback callback,
      object state)
    {
      return (IAsyncResult) new ServiceBusResourceOperations.GetAllAsyncResult(trackingContext, filter, descriptions, resourceNames, addresses, settings, callback, state);
    }

    public static IAsyncResult BeginGetAll(
      TrackingContext trackingContext,
      string filter,
      IResourceDescription[] descriptions,
      string[] resourceNames,
      IEnumerable<Uri> addresses,
      NamespaceManagerSettings settings,
      int skip,
      int top,
      AsyncCallback callback,
      object state)
    {
      return (IAsyncResult) new ServiceBusResourceOperations.GetAllAsyncResult(trackingContext, filter, descriptions, resourceNames, addresses, settings, skip, top, true, callback, state);
    }

    public static IAsyncResult BeginGetAll(
      TrackingContext trackingContext,
      string filter,
      IResourceDescription[] descriptions,
      string[] resourceNames,
      IEnumerable<Uri> addresses,
      NamespaceManagerSettings settings,
      string continuationToken,
      int top,
      AsyncCallback callback,
      object state)
    {
      return (IAsyncResult) new ServiceBusResourceOperations.GetAllAsyncResult(trackingContext, filter, descriptions, resourceNames, addresses, settings, continuationToken, top, true, callback, state);
    }

    public static SyndicationFeed EndGetAll(IAsyncResult asyncResult) => AsyncResult<ServiceBusResourceOperations.GetAllAsyncResult>.End(asyncResult).Feed;

    public static SyndicationFeed EndGetAll(IAsyncResult asyncResult, out string continuationToken)
    {
      ServiceBusResourceOperations.GetAllAsyncResult getAllAsyncResult = AsyncResult<ServiceBusResourceOperations.GetAllAsyncResult>.End(asyncResult);
      continuationToken = getAllAsyncResult.NewContinuationToken;
      return getAllAsyncResult.Feed;
    }

    private static bool IsRetriableException(Exception exception)
    {
      switch (exception)
      {
        case MessagingCommunicationException _:
          return true;
        case WebException webException:
          return webException.Status == WebExceptionStatus.ConnectFailure || webException.Status == WebExceptionStatus.RequestCanceled || webException.Status == WebExceptionStatus.NameResolutionFailure || webException.Status == WebExceptionStatus.Timeout;
        default:
          return false;
      }
    }

    internal static Exception ConvertIOException(
      TrackingContext trackingContext,
      IOException ioException,
      int timeoutInMilliseconds,
      bool isRequestAborted)
    {
      return isRequestAborted ? (Exception) new TimeoutException(SRClient.TrackableExceptionMessageFormat((object) SRClient.OperationRequestTimedOut((object) timeoutInMilliseconds), (object) trackingContext.CreateClientTrackingExceptionInfo()), (Exception) ioException) : (Exception) new MessagingException(SRClient.TrackableExceptionMessageFormat((object) ioException.Message, (object) trackingContext.CreateClientTrackingExceptionInfo()), (Exception) ioException);
    }

    internal static Exception ConvertWebException(
      TrackingContext trackingContext,
      WebException webException,
      int timeoutInMilliseconds,
      bool isUpdate = false)
    {
      HttpWebResponse response = (HttpWebResponse) webException.Response;
      string message = webException.Message;
      try
      {
        if (response == null)
        {
          if (webException.Status == WebExceptionStatus.RequestCanceled || webException.Status == WebExceptionStatus.Timeout)
            return (Exception) new TimeoutException(SRClient.TrackableExceptionMessageFormat((object) SRClient.OperationRequestTimedOut((object) timeoutInMilliseconds), (object) trackingContext.CreateClientTrackingExceptionInfo()), (Exception) webException);
          if (webException.Status == WebExceptionStatus.ConnectFailure || webException.Status == WebExceptionStatus.NameResolutionFailure)
            return (Exception) new MessagingCommunicationException(SRClient.TrackableExceptionMessageFormat((object) message, (object) trackingContext.CreateClientTrackingExceptionInfo()), (Exception) webException);
        }
        else
        {
          ServiceBusErrorData serviceBusErrorData = ServiceBusErrorData.GetServiceBusErrorData(response);
          if (!string.IsNullOrEmpty(serviceBusErrorData.Detail) && !serviceBusErrorData.Detail.Equals(response.StatusDescription, StringComparison.Ordinal))
            message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", new object[2]
            {
              (object) message,
              (object) serviceBusErrorData.Detail
            });
          else
            message = SRClient.TrackableExceptionMessageFormat((object) message, (object) trackingContext.CreateClientTrackingExceptionInfo());
          if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.NoContent)
            return (Exception) new MessagingEntityNotFoundException(message, (Exception) webException);
          if (response.StatusCode == HttpStatusCode.Conflict)
          {
            if (response.Method.Equals("DELETE"))
              return (Exception) new MessagingException(message, (Exception) webException);
            if (response.Method.Equals("PUT") & isUpdate)
              return (Exception) new MessagingException(message, (Exception) webException);
            return message.Contains(ServiceBusResourceOperations.ConflictOperationInProgressSubCode) ? (Exception) new MessagingException(message, (Exception) webException) : (Exception) new MessagingEntityAlreadyExistsException(message, (TrackingContext) null, (Exception) webException);
          }
          if (response.StatusCode == HttpStatusCode.Unauthorized)
            return (Exception) new UnauthorizedAccessException(message, (Exception) webException);
          if (response.StatusCode == HttpStatusCode.Forbidden)
            return (Exception) new QuotaExceededException(message, (Exception) webException);
          if (response.StatusCode == HttpStatusCode.BadRequest)
            return (Exception) new ArgumentException(message, (Exception) webException);
          if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            return (Exception) new ServerBusyException(message, (Exception) webException);
          if (response.StatusCode == HttpStatusCode.GatewayTimeout)
            return (Exception) new MessagingCommunicationException(message, (Exception) webException);
        }
        return (Exception) new MessagingException(message, (Exception) webException);
      }
      finally
      {
        response?.Close();
      }
    }

    private static Uri CreateCollectionUri<T>(
      Uri baseUri,
      T[] resourceDescriptions,
      string[] resourceNames,
      ContinuationToken continuationToken,
      int skip,
      int top,
      string filter)
      where T : class, IResourceDescription
    {
      if (resourceDescriptions == null || resourceDescriptions.Length == 0)
        throw new ArgumentException(SRClient.NullResourceDescription);
      UriBuilder uriBuilder = new UriBuilder(baseUri);
      if (uriBuilder.Port == -1)
        uriBuilder.Port = RelayEnvironment.RelayHttpsPort;
      uriBuilder.Scheme = Uri.UriSchemeHttps;
      MessagingUtilities.EnsureTrailingSlash(uriBuilder);
      if (resourceNames == null || resourceNames.Length == 0)
      {
        uriBuilder.Path = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}/{2}", new object[3]
        {
          (object) uriBuilder.Path,
          (object) "$Resources",
          (object) resourceDescriptions[0].CollectionName
        });
      }
      else
      {
        StringBuilder stringBuilder = new StringBuilder(uriBuilder.Path);
        for (int index = 0; index < resourceDescriptions.Length; ++index)
        {
          if (resourceNames != null && index < resourceNames.Length)
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/", new object[2]
            {
              (object) resourceNames[index],
              (object) resourceDescriptions[index].CollectionName
            });
        }
        uriBuilder.Path = stringBuilder.ToString();
      }
      MessagingUtilities.EnsureTrailingSlash(uriBuilder);
      string empty = string.Empty;
      string currentQuery;
      if (continuationToken != null)
      {
        if (!string.IsNullOrWhiteSpace(continuationToken.Token))
          currentQuery = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}&$top={2}", new object[3]
          {
            (object) "continuationtoken",
            (object) continuationToken.Token,
            (object) top
          });
        else
          currentQuery = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "$top={0}", new object[1]
          {
            (object) top
          });
      }
      else
        currentQuery = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "$skip={0}&$top={1}", new object[2]
        {
          (object) skip,
          (object) top
        });
      if (!string.IsNullOrWhiteSpace(filter))
        currentQuery = ServiceBusResourceOperations.AddQueryParameter(currentQuery, "$filter", HttpUtility.UrlEncode(filter));
      uriBuilder.Query = ServiceBusResourceOperations.AddQueryParameter(currentQuery, "api-version", "2016-07");
      return uriBuilder.Uri;
    }

    private static Uri CreateInformationUri(Uri baseUri)
    {
      UriBuilder httpsSchemeAndPort = MessagingUtilities.CreateUriBuilderWithHttpsSchemeAndPort(baseUri);
      MessagingUtilities.EnsureTrailingSlash(httpsSchemeAndPort);
      httpsSchemeAndPort.Path = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}/", new object[2]
      {
        (object) httpsSchemeAndPort.Path,
        (object) "$protocol-version"
      });
      return httpsSchemeAndPort.Uri;
    }

    private static Uri CreateResourceUri(
      Uri baseUri,
      IResourceDescription[] resourceDescriptions,
      string[] resourceNames,
      IDictionary<string, string> queryParametersAndValues)
    {
      if (resourceNames == null || resourceNames.Length == 0)
        throw new ArgumentException(SRClient.NullResourceName);
      UriBuilder httpsSchemeAndPort = MessagingUtilities.CreateUriBuilderWithHttpsSchemeAndPort(baseUri);
      MessagingUtilities.EnsureTrailingSlash(httpsSchemeAndPort);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}/", new object[2]
      {
        (object) httpsSchemeAndPort.Path,
        (object) resourceNames[0]
      });
      for (int index = 1; index < resourceNames.Length; ++index)
      {
        if (resourceDescriptions != null && index <= resourceDescriptions.Length)
        {
          if (resourceNames[index] != string.Empty)
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/", new object[2]
            {
              (object) resourceDescriptions[index - 1].CollectionName,
              (object) resourceNames[index]
            });
          else
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}/", new object[1]
            {
              (object) resourceDescriptions[index - 1].CollectionName
            });
        }
      }
      httpsSchemeAndPort.Path = stringBuilder.ToString();
      string currentQuery = ServiceBusResourceOperations.AddQueryParameter(string.Empty, "api-version", "2016-07");
      if (queryParametersAndValues != null)
      {
        foreach (KeyValuePair<string, string> parametersAndValue in (IEnumerable<KeyValuePair<string, string>>) queryParametersAndValues)
          currentQuery = ServiceBusResourceOperations.AddQueryParameter(currentQuery, parametersAndValue.Key, parametersAndValue.Value);
      }
      httpsSchemeAndPort.Query = currentQuery;
      return httpsSchemeAndPort.Uri;
    }

    private static string AddQueryParameter(string currentQuery, string name, string value)
    {
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", new object[2]
      {
        (object) name,
        (object) value
      });
      if (!string.IsNullOrEmpty(currentQuery))
        currentQuery += "&";
      return currentQuery + str;
    }

    private sealed class GetInformationAsyncResult : 
      RetryAsyncResult<ServiceBusResourceOperations.GetInformationAsyncResult>
    {
      private readonly EventTraceActivity relatedActivity;
      private readonly NamespaceManagerSettings settings;
      private readonly TokenProvider tokenProvider;
      private readonly ServiceBusUriManager uriManager;
      private readonly RetryPolicy retryPolicy;
      private readonly TrackingContext trackingContext;
      private HttpWebRequest request;
      private HttpWebResponse response;
      private IOThreadTimer requestCancelTimer;
      private volatile bool isRequestAborted;

      public GetInformationAsyncResult(
        TrackingContext trackingContext,
        IEnumerable<Uri> managementAddresses,
        NamespaceManagerSettings settings,
        TimeSpan timeout,
        AsyncCallback callback,
        object state)
        : base(timeout, callback, state)
      {
        if (trackingContext == null)
          throw new ArgumentNullException(nameof (trackingContext));
        if (settings == null)
          throw FxTrace.Exception.ArgumentNull(nameof (settings));
        this.trackingContext = trackingContext;
        this.settings = settings;
        this.tokenProvider = settings.TokenProvider;
        this.retryPolicy = settings.RetryPolicy;
        this.Information = (IDictionary<string, string>) new ConcurrentDictionary<string, string>();
        this.uriManager = new ServiceBusUriManager(managementAddresses.ToList<Uri>(), true);
        this.relatedActivity = EventTraceActivity.CreateFromThread();
        this.Start();
      }

      public IDictionary<string, string> Information { get; private set; }

      protected override IEnumerator<IteratorAsyncResult<ServiceBusResourceOperations.GetInformationAsyncResult>.AsyncStep> GetAsyncSteps()
      {
        int iteration = 0;
        TimeSpan iterationSleep = this.retryPolicy.IsServerBusy ? RetryPolicy.ServerBusyBaseSleepTime : TimeSpan.Zero;
        if (this.retryPolicy.IsServerBusy && RetryPolicy.ServerBusyBaseSleepTime >= this.OriginalTimeout)
        {
          string serverBusyExceptionMessage = this.retryPolicy.ServerBusyExceptionMessage;
          yield return this.CallAsyncSleep(this.RemainingTime());
          this.Complete((Exception) new ServerBusyException(serverBusyExceptionMessage));
        }
        else
        {
          bool shouldRetry;
          do
          {
            shouldRetry = false;
            if (iterationSleep != TimeSpan.Zero)
              yield return this.CallAsyncSleep(iterationSleep);
            this.LastAsyncStepException = (Exception) null;
            this.request = (HttpWebRequest) WebRequest.Create(ServiceBusResourceOperations.CreateInformationUri(this.uriManager.Current));
            if (ServiceBusEnvironment.Proxy != null)
              this.request.Proxy = ServiceBusEnvironment.Proxy;
            this.request.ServicePoint.MaxIdleTime = Constants.ServicePointMaxIdleTimeMilliSeconds;
            this.request.Method = "GET";
            this.request.ContentType = "application/atom+xml;type=entry;charset=utf-8";
            HttpWebRequest request = this.request;
            TimeSpan originalTimeout = this.OriginalTimeout;
            int num;
            if (originalTimeout.TotalMilliseconds <= (double) int.MaxValue)
            {
              originalTimeout = this.OriginalTimeout;
              num = (int) originalTimeout.TotalMilliseconds;
            }
            else
              num = int.MaxValue;
            request.Timeout = num;
            this.request.SetUserAgentHeader();
            this.request.AddAuthorizationHeader(this.tokenProvider, this.uriManager.Current, "Manage");
            this.request.AddTrackingIdHeader(this.trackingContext);
            this.request.AddCorrelationHeader(this.relatedActivity);
            this.request.AddXProcessAtHeader();
            this.requestCancelTimer = new IOThreadTimer(new Action<object>(this.CancelRequest), (object) this.request, true);
            try
            {
              TimeSpan timeToCancelRequest = this.RemainingTime();
              if (timeToCancelRequest <= TimeSpan.Zero)
              {
                this.Complete((Exception) new TimeoutException(SRClient.OperationRequestTimedOut((object) this.OriginalTimeout)));
                yield break;
              }
              else
              {
                yield return this.CallAsync((IteratorAsyncResult<ServiceBusResourceOperations.GetInformationAsyncResult>.BeginCall) ((thisPtr, t, c, s) =>
                {
                  IAsyncResult response = thisPtr.request.BeginGetResponse(c, s);
                  thisPtr.requestCancelTimer.SetIfValid(timeToCancelRequest);
                  return response;
                }), (IteratorAsyncResult<ServiceBusResourceOperations.GetInformationAsyncResult>.EndCall) ((thisPtr, r) =>
                {
                  ServiceBusResourceOperations.GetInformationAsyncResult informationAsyncResult = thisPtr;
                  informationAsyncResult.response = (HttpWebResponse) informationAsyncResult.request.EndGetResponse(r);
                }), IteratorAsyncResult<ServiceBusResourceOperations.GetInformationAsyncResult>.ExceptionPolicy.Continue);
                if (this.LastAsyncStepException != null)
                {
                  WebException asyncStepException1 = this.LastAsyncStepException as WebException;
                  IOException asyncStepException2 = this.LastAsyncStepException as IOException;
                  if (asyncStepException1 != null)
                    this.LastAsyncStepException = ServiceBusResourceOperations.ConvertWebException(this.trackingContext, asyncStepException1, this.request.Timeout);
                  if (asyncStepException2 != null)
                    this.LastAsyncStepException = ServiceBusResourceOperations.ConvertIOException(this.trackingContext, asyncStepException2, this.request.Timeout, this.isRequestAborted);
                  shouldRetry = !this.TransactionExists && this.retryPolicy.ShouldRetry(this.RemainingTime(), iteration, this.LastAsyncStepException, out iterationSleep);
                  if (shouldRetry)
                  {
                    MessagingClientEtwProvider.TraceClient((Action) (() => MessagingClientEtwProvider.Provider.EventWriteRetryPolicyIteration((EventTraceActivity) null, string.Empty, this.retryPolicy.GetType().Name, "GetInfo", iteration, iterationSleep.ToString(), this.LastAsyncStepException.GetType().FullName, this.LastAsyncStepException.Message)));
                    ++iteration;
                    goto label_37;
                  }
                }
                else
                {
                  try
                  {
                    using (this.response)
                    {
                      if (this.response.StatusCode != HttpStatusCode.OK)
                      {
                        this.LastAsyncStepException = (Exception) new MessagingException(this.response.StatusDescription, (Exception) new WebException(this.response.StatusDescription, (Exception) null, WebExceptionStatus.ProtocolError, (WebResponse) this.response));
                      }
                      else
                      {
                        foreach (string allKey in this.response.Headers.AllKeys)
                          this.Information[allKey] = this.response.Headers[allKey];
                      }
                    }
                  }
                  catch (XmlException ex)
                  {
                    this.LastAsyncStepException = (Exception) new MessagingException(SRClient.InvalidXmlFormat, (Exception) ex);
                  }
                  catch (WebException ex)
                  {
                    this.LastAsyncStepException = ServiceBusResourceOperations.ConvertWebException(this.trackingContext, ex, this.request.Timeout);
                  }
                  catch (IOException ex)
                  {
                    this.LastAsyncStepException = ServiceBusResourceOperations.ConvertIOException(this.trackingContext, ex, this.request.Timeout, this.isRequestAborted);
                  }
                  if (this.LastAsyncStepException != null)
                  {
                    shouldRetry = !this.TransactionExists && this.retryPolicy.ShouldRetry(this.RemainingTime(), iteration, this.LastAsyncStepException, out iterationSleep);
                    if (shouldRetry)
                    {
                      MessagingClientEtwProvider.TraceClient((Action) (() => MessagingClientEtwProvider.Provider.EventWriteRetryPolicyIteration((EventTraceActivity) null, string.Empty, this.retryPolicy.GetType().Name, "GetInfo", iteration, iterationSleep.ToString(), this.LastAsyncStepException.GetType().FullName, this.LastAsyncStepException.Message)));
                      ++iteration;
                      goto label_37;
                    }
                  }
                  else
                    this.retryPolicy.ResetServerBusy();
                }
                goto label_38;
label_37:;
              }
            }
            finally
            {
              this.requestCancelTimer.Cancel();
            }
label_38:;
          }
          while (this.uriManager.MoveNextUri() & shouldRetry);
          this.Complete(this.LastAsyncStepException);
        }
      }

      private void CancelRequest(object state)
      {
        this.isRequestAborted = true;
        ((WebRequest) state).Abort();
      }
    }

    private sealed class GetInstallationAsyncResult : 
      RetryAsyncResult<ServiceBusResourceOperations.GetInstallationAsyncResult>
    {
      private readonly TrackingContext trackingContext;
      private readonly EventTraceActivity relatedActivity;
      private readonly NamespaceManagerSettings settings;
      private readonly TokenProvider tokenProvider;
      private readonly IResourceDescription[] collectionDescriptions;
      private readonly string[] resourceNames;
      private readonly ServiceBusUriManager uriManager;
      private readonly RetryPolicy retryPolicy;
      private HttpWebRequest request;
      private HttpWebResponse response;
      private IOThreadTimer requestCancelTimer;
      private volatile bool isRequestAborted;

      public GetInstallationAsyncResult(
        string installationId,
        string hubPath,
        TrackingContext trackingContext,
        IEnumerable<Uri> baseAddresses,
        NamespaceManagerSettings settings,
        TimeSpan timeout,
        AsyncCallback callback,
        object state)
        : base(timeout, callback, state)
      {
        if (trackingContext == null)
          throw new ArgumentNullException(nameof (trackingContext));
        if (settings == null)
          throw FxTrace.Exception.ArgumentNull(nameof (settings));
        this.trackingContext = trackingContext;
        this.settings = settings;
        this.tokenProvider = settings.TokenProvider;
        this.collectionDescriptions = new IResourceDescription[1]
        {
          (IResourceDescription) new ServiceBusResourceOperations.GetInstallationAsyncResult.InstallationResourceDescription()
        };
        this.resourceNames = new string[2]
        {
          hubPath,
          installationId
        };
        this.uriManager = new ServiceBusUriManager(baseAddresses.ToList<Uri>());
        this.relatedActivity = EventTraceActivity.CreateFromThread();
        this.retryPolicy = settings.RetryPolicy;
        this.Start();
      }

      public string Result { get; private set; }

      protected override IEnumerator<IteratorAsyncResult<ServiceBusResourceOperations.GetInstallationAsyncResult>.AsyncStep> GetAsyncSteps()
      {
        int iteration = 0;
        TimeSpan iterationSleep = this.retryPolicy.IsServerBusy ? RetryPolicy.ServerBusyBaseSleepTime : TimeSpan.Zero;
        if (this.retryPolicy.IsServerBusy && RetryPolicy.ServerBusyBaseSleepTime >= this.OriginalTimeout)
        {
          string serverBusyExceptionMessage = this.retryPolicy.ServerBusyExceptionMessage;
          yield return this.CallAsyncSleep(this.RemainingTime());
          this.Complete((Exception) new ServerBusyException(serverBusyExceptionMessage));
        }
        else
        {
          bool shouldRetry;
          do
          {
            shouldRetry = false;
            if (iterationSleep != TimeSpan.Zero)
              yield return this.CallAsyncSleep(iterationSleep);
            this.LastAsyncStepException = (Exception) null;
            Uri resourceUri = ServiceBusResourceOperations.CreateResourceUri(this.uriManager.Current, this.collectionDescriptions, this.resourceNames, (IDictionary<string, string>) null);
            this.request = (HttpWebRequest) WebRequest.Create(resourceUri);
            if (ServiceBusEnvironment.Proxy != null)
              this.request.Proxy = ServiceBusEnvironment.Proxy;
            this.request.ServicePoint.MaxIdleTime = Constants.ServicePointMaxIdleTimeMilliSeconds;
            this.request.Method = "GET";
            this.request.ContentType = "application/json";
            HttpWebRequest request = this.request;
            TimeSpan originalTimeout = this.OriginalTimeout;
            int num;
            if (originalTimeout.TotalMilliseconds <= (double) int.MaxValue)
            {
              originalTimeout = this.OriginalTimeout;
              num = (int) originalTimeout.TotalMilliseconds;
            }
            else
              num = int.MaxValue;
            request.Timeout = num;
            this.request.SetUserAgentHeader();
            this.request.AddXProcessAtHeader();
            this.request.AddAuthorizationHeader(this.tokenProvider, this.uriManager.Current, "Manage");
            this.request.AddTrackingIdHeader(this.trackingContext);
            this.request.AddCorrelationHeader(this.relatedActivity);
            this.requestCancelTimer = new IOThreadTimer(new Action<object>(this.CancelRequest), (object) this.request, true);
            try
            {
              TimeSpan timeToCancelRequest = this.RemainingTime();
              if (timeToCancelRequest <= TimeSpan.Zero)
              {
                this.Complete((Exception) new TimeoutException(SRClient.OperationRequestTimedOut((object) this.OriginalTimeout)));
                yield break;
              }
              else
              {
                yield return this.CallAsync((IteratorAsyncResult<ServiceBusResourceOperations.GetInstallationAsyncResult>.BeginCall) ((thisPtr, t, c, s) =>
                {
                  IAsyncResult response = thisPtr.request.BeginGetResponse(c, s);
                  thisPtr.requestCancelTimer.SetIfValid(timeToCancelRequest);
                  return response;
                }), (IteratorAsyncResult<ServiceBusResourceOperations.GetInstallationAsyncResult>.EndCall) ((thisPtr, r) =>
                {
                  ServiceBusResourceOperations.GetInstallationAsyncResult installationAsyncResult = thisPtr;
                  installationAsyncResult.response = (HttpWebResponse) installationAsyncResult.request.EndGetResponse(r);
                }), IteratorAsyncResult<ServiceBusResourceOperations.GetInstallationAsyncResult>.ExceptionPolicy.Continue);
                if (this.LastAsyncStepException != null)
                {
                  WebException asyncStepException1 = this.LastAsyncStepException as WebException;
                  IOException asyncStepException2 = this.LastAsyncStepException as IOException;
                  if (asyncStepException1 != null)
                    this.LastAsyncStepException = ServiceBusResourceOperations.ConvertWebException(this.trackingContext, asyncStepException1, this.request.Timeout);
                  if (asyncStepException2 != null)
                    this.LastAsyncStepException = ServiceBusResourceOperations.ConvertIOException(this.trackingContext, asyncStepException2, this.request.Timeout, this.isRequestAborted);
                  TimeSpan remainingTime = this.RemainingTime();
                  shouldRetry = !this.TransactionExists && this.retryPolicy.ShouldRetry(remainingTime, iteration, this.LastAsyncStepException, out iterationSleep);
                  if (shouldRetry)
                  {
                    string operation = string.Format("Get:{0}", (object) resourceUri.AbsoluteUri);
                    MessagingClientEtwProvider.TraceClient((Action) (() => MessagingClientEtwProvider.Provider.EventWriteRetryPolicyIteration((EventTraceActivity) null, string.Empty, this.retryPolicy.GetType().Name, operation, iteration, iterationSleep.ToString(), this.LastAsyncStepException.GetType().FullName, this.LastAsyncStepException.Message)));
                    ++iteration;
                    goto label_40;
                  }
                }
                else
                {
                  try
                  {
                    using (this.response)
                    {
                      if (this.response.StatusCode != HttpStatusCode.OK)
                      {
                        this.LastAsyncStepException = (Exception) new MessagingException(this.response.StatusDescription, false, (Exception) new WebException(this.response.StatusDescription, (Exception) null, WebExceptionStatus.ProtocolError, (WebResponse) this.response));
                      }
                      else
                      {
                        using (Stream responseStream = this.response.GetResponseStream())
                          this.Result = new StreamReader(responseStream).ReadToEnd();
                      }
                    }
                  }
                  catch (XmlException ex)
                  {
                    this.LastAsyncStepException = (Exception) new MessagingException(SRClient.InvalidXmlFormat, false, (Exception) ex);
                  }
                  catch (WebException ex)
                  {
                    this.LastAsyncStepException = ServiceBusResourceOperations.ConvertWebException(this.trackingContext, ex, this.request.Timeout);
                  }
                  catch (IOException ex)
                  {
                    this.LastAsyncStepException = ServiceBusResourceOperations.ConvertIOException(this.trackingContext, ex, this.request.Timeout, this.isRequestAborted);
                  }
                  if (this.LastAsyncStepException != null)
                  {
                    TimeSpan remainingTime = this.RemainingTime();
                    shouldRetry = !this.TransactionExists && this.retryPolicy.ShouldRetry(remainingTime, iteration, this.LastAsyncStepException, out iterationSleep);
                    if (shouldRetry)
                    {
                      string operation = string.Format("Get:{0}", (object) resourceUri.AbsoluteUri);
                      MessagingClientEtwProvider.TraceClient((Action) (() => MessagingClientEtwProvider.Provider.EventWriteRetryPolicyIteration((EventTraceActivity) null, string.Empty, this.retryPolicy.GetType().Name, operation, iteration, iterationSleep.ToString(), this.LastAsyncStepException.GetType().FullName, this.LastAsyncStepException.Message)));
                      ++iteration;
                      goto label_40;
                    }
                  }
                  else
                    this.retryPolicy.ResetServerBusy();
                }
                goto label_19;
label_40:
                goto label_41;
              }
            }
            finally
            {
              this.requestCancelTimer.Cancel();
            }
label_19:
            resourceUri = (Uri) null;
label_41:;
          }
          while (this.uriManager.MoveNextUri() & shouldRetry);
          this.Complete(this.LastAsyncStepException);
        }
      }

      private void CancelRequest(object state)
      {
        this.isRequestAborted = true;
        ((WebRequest) state).Abort();
      }

      private class InstallationResourceDescription : IResourceDescription
      {
        public string CollectionName => "installations";
      }
    }

    private sealed class CreateOrUpdateInstallationAsyncResult : 
      IteratorAsyncResult<ServiceBusResourceOperations.CreateOrUpdateInstallationAsyncResult>
    {
      private readonly TrackingContext trackingContext;
      private readonly EventTraceActivity relatedActivity;
      private readonly string jsonPayload;
      private readonly NamespaceManagerSettings settings;
      private readonly TokenProvider tokenProvider;
      private readonly bool isAnonymousAccessible;
      private readonly ServiceBusUriManager uriManager;
      private readonly IDictionary<string, string> queryParametersAndValues;
      private readonly string method;
      private HttpWebRequest request;
      private Uri currentResourceUri;
      private Stream requestStream;
      private HttpWebResponse response;
      private IResourceDescription[] collectionDescriptions;
      private string[] resourceNames;
      private IOThreadTimer requestCancelTimer;
      private volatile bool isRequestAborted;

      public CreateOrUpdateInstallationAsyncResult(
        TrackingContext trackingContext,
        string jsonPayload,
        string installationId,
        string method,
        string hubPath,
        IEnumerable<Uri> baseAddresses,
        TimeSpan timeout,
        bool isAnonymousAccessible,
        IDictionary<string, string> queryParametersAndValues,
        NamespaceManagerSettings settings,
        AsyncCallback callback,
        object state)
        : base(timeout, callback, state)
      {
        this.trackingContext = trackingContext != null ? trackingContext : throw new ArgumentNullException(nameof (trackingContext));
        this.settings = settings;
        this.tokenProvider = settings.TokenProvider;
        this.collectionDescriptions = new IResourceDescription[1]
        {
          (IResourceDescription) new ServiceBusResourceOperations.CreateOrUpdateInstallationAsyncResult.InstallationResourceDescription()
        };
        this.isAnonymousAccessible = isAnonymousAccessible;
        this.resourceNames = new string[2]
        {
          hubPath,
          installationId
        };
        this.method = method;
        this.jsonPayload = jsonPayload;
        this.uriManager = new ServiceBusUriManager(baseAddresses.ToList<Uri>());
        this.relatedActivity = EventTraceActivity.CreateFromThread();
        this.queryParametersAndValues = queryParametersAndValues;
        this.Start();
      }

      public string Result { get; private set; }

      protected override IEnumerator<IteratorAsyncResult<ServiceBusResourceOperations.CreateOrUpdateInstallationAsyncResult>.AsyncStep> GetAsyncSteps()
      {
        while (this.uriManager.MoveNextUri())
        {
          this.LastAsyncStepException = (Exception) null;
          this.currentResourceUri = ServiceBusResourceOperations.CreateResourceUri(this.uriManager.Current, this.collectionDescriptions, this.resourceNames, this.queryParametersAndValues);
          this.request = (HttpWebRequest) WebRequest.Create(this.currentResourceUri);
          if (ServiceBusEnvironment.Proxy != null)
            this.request.Proxy = ServiceBusEnvironment.Proxy;
          this.request.Headers.Add("X-MS-ISANONYMOUSACCESSIBLE", this.isAnonymousAccessible.ToString());
          this.request.Method = this.method;
          this.request.ServicePoint.MaxIdleTime = Constants.ServicePointMaxIdleTimeMilliSeconds;
          this.request.ContentType = "application/json";
          this.request.Timeout = this.OriginalTimeout.TotalMilliseconds > (double) int.MaxValue ? int.MaxValue : (int) this.OriginalTimeout.TotalMilliseconds;
          this.request.SetUserAgentHeader();
          this.request.AddXProcessAtHeader();
          this.request.AddAuthorizationHeader(this.tokenProvider, this.uriManager.Current, "Manage");
          this.request.AddTrackingIdHeader(this.trackingContext);
          this.request.AddCorrelationHeader(this.relatedActivity);
          this.requestCancelTimer = new IOThreadTimer(new Action<object>(this.CancelRequest), (object) this.request, true);
          bool asyncSteps;
          try
          {
            TimeSpan timeToCancelRequest = this.RemainingTime();
            if (timeToCancelRequest <= TimeSpan.Zero)
            {
              this.Complete((Exception) new TimeoutException(SRClient.OperationRequestTimedOut((object) this.OriginalTimeout)));
              asyncSteps = false;
            }
            else
            {
              try
              {
                yield return this.CallAsync((IteratorAsyncResult<ServiceBusResourceOperations.CreateOrUpdateInstallationAsyncResult>.BeginCall) ((thisPtr, t, c, s) =>
                {
                  IAsyncResult requestStream = thisPtr.request.BeginGetRequestStream(c, s);
                  thisPtr.requestCancelTimer.SetIfValid(timeToCancelRequest);
                  return requestStream;
                }), (IteratorAsyncResult<ServiceBusResourceOperations.CreateOrUpdateInstallationAsyncResult>.EndCall) ((thisPtr, r) =>
                {
                  ServiceBusResourceOperations.CreateOrUpdateInstallationAsyncResult installationAsyncResult = thisPtr;
                  installationAsyncResult.requestStream = installationAsyncResult.request.EndGetRequestStream(r);
                }), IteratorAsyncResult<ServiceBusResourceOperations.CreateOrUpdateInstallationAsyncResult>.ExceptionPolicy.Continue);
                if (this.LastAsyncStepException != null)
                {
                  if (!this.uriManager.CanRetry() || !ServiceBusResourceOperations.IsRetriableException(this.LastAsyncStepException))
                  {
                    Exception operationException = this.LastAsyncStepException;
                    if (operationException is WebException webException)
                      operationException = ServiceBusResourceOperations.ConvertWebException(this.trackingContext, webException, this.request.Timeout);
                    if (operationException is IOException ioException)
                      operationException = ServiceBusResourceOperations.ConvertIOException(this.trackingContext, ioException, this.request.Timeout, this.isRequestAborted);
                    this.Complete(operationException);
                    asyncSteps = false;
                    goto label_25;
                  }
                }
                else
                {
                  try
                  {
                    using (StreamWriter streamWriter = new StreamWriter(this.requestStream, Encoding.UTF8))
                    {
                      streamWriter.Write(this.jsonPayload);
                      goto label_8;
                    }
                  }
                  catch (WebException ex)
                  {
                    if (this.uriManager.CanRetry())
                    {
                      if (ServiceBusResourceOperations.IsRetriableException((Exception) ex))
                        goto label_24;
                    }
                    this.Complete(ServiceBusResourceOperations.ConvertWebException(this.trackingContext, ex, this.request.Timeout));
                    asyncSteps = false;
                    goto label_25;
                  }
                  catch (IOException ex)
                  {
                    this.Complete(ServiceBusResourceOperations.ConvertIOException(this.trackingContext, ex, this.request.Timeout, this.isRequestAborted));
                    asyncSteps = false;
                    goto label_25;
                  }
                }
label_24:
                goto label_53;
label_25:
                goto label_52;
              }
              finally
              {
                this.requestCancelTimer.Cancel();
              }
label_8:
              TimeSpan timeToCancelResponse = this.RemainingTime();
              if (timeToCancelResponse <= TimeSpan.Zero)
              {
                this.Complete((Exception) new TimeoutException(SRClient.OperationRequestTimedOut((object) this.OriginalTimeout)));
                asyncSteps = false;
                goto label_52;
              }
              else
              {
                yield return this.CallAsync((IteratorAsyncResult<ServiceBusResourceOperations.CreateOrUpdateInstallationAsyncResult>.BeginCall) ((thisPtr, t, c, s) =>
                {
                  IAsyncResult response = thisPtr.request.BeginGetResponse(c, s);
                  thisPtr.requestCancelTimer.Set(timeToCancelResponse);
                  return response;
                }), (IteratorAsyncResult<ServiceBusResourceOperations.CreateOrUpdateInstallationAsyncResult>.EndCall) ((thisPtr, r) =>
                {
                  ServiceBusResourceOperations.CreateOrUpdateInstallationAsyncResult installationAsyncResult = thisPtr;
                  installationAsyncResult.response = (HttpWebResponse) installationAsyncResult.request.EndGetResponse(r);
                }), IteratorAsyncResult<ServiceBusResourceOperations.CreateOrUpdateInstallationAsyncResult>.ExceptionPolicy.Continue);
                if (this.LastAsyncStepException != null)
                {
                  if (!this.uriManager.CanRetry() || !ServiceBusResourceOperations.IsRetriableException(this.LastAsyncStepException))
                  {
                    Exception operationException = this.LastAsyncStepException;
                    if (operationException is WebException webException)
                      operationException = ServiceBusResourceOperations.ConvertWebException(this.trackingContext, webException, this.request.Timeout);
                    if (operationException is IOException ioException)
                      operationException = ServiceBusResourceOperations.ConvertIOException(this.trackingContext, ioException, this.request.Timeout, this.isRequestAborted);
                    this.Complete(operationException);
                    asyncSteps = false;
                    goto label_52;
                  }
                }
                else
                {
                  try
                  {
                    using (this.response)
                    {
                      if (this.response.StatusCode != HttpStatusCode.Created && this.response.StatusCode != HttpStatusCode.OK)
                      {
                        if (!this.uriManager.CanRetry())
                        {
                          this.Complete(ServiceBusResourceOperations.ConvertWebException(this.trackingContext, new WebException(this.response.StatusDescription, (Exception) null, WebExceptionStatus.ProtocolError, (WebResponse) this.response), this.request.Timeout));
                          asyncSteps = false;
                          goto label_52;
                        }
                        else
                          goto label_53;
                      }
                      else
                      {
                        using (StreamReader streamReader = new StreamReader(this.response.GetResponseStream(), Encoding.UTF8))
                          this.Result = streamReader.ReadToEnd();
                      }
                    }
                  }
                  catch (XmlException ex)
                  {
                    this.Complete((Exception) new MessagingException(SRClient.InvalidXmlFormat, false, (Exception) ex));
                    asyncSteps = false;
                    goto label_52;
                  }
                  catch (WebException ex)
                  {
                    if (this.uriManager.CanRetry())
                    {
                      if (ServiceBusResourceOperations.IsRetriableException((Exception) ex))
                        goto label_53;
                    }
                    this.Complete(ServiceBusResourceOperations.ConvertWebException(this.trackingContext, ex, this.request.Timeout));
                    asyncSteps = false;
                    goto label_52;
                  }
                  catch (IOException ex)
                  {
                    this.Complete(ServiceBusResourceOperations.ConvertIOException(this.trackingContext, ex, this.request.Timeout, this.isRequestAborted));
                  }
                  break;
                }
              }
label_53:
              continue;
            }
label_52:;
          }
          finally
          {
            this.requestCancelTimer.Cancel();
          }
          return asyncSteps;
        }
      }

      private void CancelRequest(object state)
      {
        this.isRequestAborted = true;
        ((WebRequest) state).Abort();
      }

      private class InstallationResourceDescription : IResourceDescription
      {
        public string CollectionName => "installations";
      }
    }

    private sealed class DeleteInstallationAsyncResult : 
      RetryAsyncResult<ServiceBusResourceOperations.DeleteInstallationAsyncResult>
    {
      private readonly TrackingContext trackingContext;
      private readonly EventTraceActivity relatedActivity;
      private readonly NamespaceManagerSettings settings;
      private readonly TokenProvider tokenProvider;
      private readonly ServiceBusUriManager uriManager;
      private readonly RetryPolicy retryPolicy;
      private readonly string[] resourceNames;
      private readonly Dictionary<string, string> additionalHeaders;
      private volatile bool isRequestAborted;
      private HttpWebRequest request;
      private HttpWebResponse response;
      private IResourceDescription[] collectionDescriptions;
      private IOThreadTimer requestCancelTimer;
      private Uri resourceUri;

      public DeleteInstallationAsyncResult(
        TrackingContext trackingContext,
        string installationId,
        string hubPath,
        IEnumerable<Uri> baseAddresses,
        Dictionary<string, string> additionalHeaders,
        TimeSpan timeout,
        NamespaceManagerSettings settings,
        AsyncCallback callback,
        object state)
        : base(timeout, callback, state)
      {
        if (trackingContext == null)
          throw new ArgumentNullException(nameof (trackingContext));
        this.additionalHeaders = additionalHeaders;
        this.trackingContext = trackingContext;
        this.settings = settings;
        this.retryPolicy = settings.RetryPolicy;
        this.tokenProvider = settings.TokenProvider;
        this.collectionDescriptions = new IResourceDescription[1]
        {
          (IResourceDescription) new ServiceBusResourceOperations.DeleteInstallationAsyncResult.InstallationResourceDescription()
        };
        this.resourceNames = new string[2]
        {
          hubPath,
          installationId
        };
        this.uriManager = new ServiceBusUriManager(baseAddresses.ToList<Uri>());
        this.relatedActivity = EventTraceActivity.CreateFromThread();
        this.Start();
      }

      public string Result { get; private set; }

      protected override IEnumerator<IteratorAsyncResult<ServiceBusResourceOperations.DeleteInstallationAsyncResult>.AsyncStep> GetAsyncSteps()
      {
        int iteration = 0;
        TimeSpan iterationSleep = this.retryPolicy.IsServerBusy ? RetryPolicy.ServerBusyBaseSleepTime : TimeSpan.Zero;
        if (this.retryPolicy.IsServerBusy && RetryPolicy.ServerBusyBaseSleepTime >= this.OriginalTimeout)
        {
          string serverBusyExceptionMessage = this.retryPolicy.ServerBusyExceptionMessage;
          yield return this.CallAsyncSleep(this.RemainingTime());
          this.Complete((Exception) new ServerBusyException(serverBusyExceptionMessage));
        }
        else
        {
          bool shouldRetry;
          bool asyncSteps;
          do
          {
            shouldRetry = false;
            if (iterationSleep != TimeSpan.Zero)
              yield return this.CallAsyncSleep(iterationSleep);
            this.LastAsyncStepException = (Exception) null;
            this.resourceUri = ServiceBusResourceOperations.CreateResourceUri(this.uriManager.Current, this.collectionDescriptions, this.resourceNames, (IDictionary<string, string>) null);
            this.request = (HttpWebRequest) WebRequest.Create(this.resourceUri);
            if (ServiceBusEnvironment.Proxy != null)
              this.request.Proxy = ServiceBusEnvironment.Proxy;
            this.request.ServicePoint.MaxIdleTime = Constants.ServicePointMaxIdleTimeMilliSeconds;
            this.request.ContentType = "application/json";
            this.request.Method = "DELETE";
            HttpWebRequest request = this.request;
            TimeSpan originalTimeout = this.OriginalTimeout;
            int num;
            if (originalTimeout.TotalMilliseconds <= (double) int.MaxValue)
            {
              originalTimeout = this.OriginalTimeout;
              num = (int) originalTimeout.TotalMilliseconds;
            }
            else
              num = int.MaxValue;
            request.Timeout = num;
            this.request.SetUserAgentHeader();
            this.request.AddXProcessAtHeader();
            this.request.AddAuthorizationHeader(this.tokenProvider, this.uriManager.Current, "Manage");
            this.request.AddTrackingIdHeader(this.trackingContext);
            this.request.AddCorrelationHeader(this.relatedActivity);
            this.requestCancelTimer = new IOThreadTimer(new Action<object>(this.CancelRequest), (object) this.request, true);
            if (this.additionalHeaders != null)
            {
              foreach (KeyValuePair<string, string> additionalHeader in this.additionalHeaders)
                this.request.Headers.Add(additionalHeader.Key, additionalHeader.Value);
            }
            try
            {
              TimeSpan timeToCancelRequest = this.RemainingTime();
              if (timeToCancelRequest <= TimeSpan.Zero)
              {
                this.Complete((Exception) new TimeoutException(SRClient.OperationRequestTimedOut((object) this.OriginalTimeout)));
                asyncSteps = false;
              }
              else
              {
                yield return this.CallAsync((IteratorAsyncResult<ServiceBusResourceOperations.DeleteInstallationAsyncResult>.BeginCall) ((thisPtr, t, c, s) =>
                {
                  IAsyncResult response = thisPtr.request.BeginGetResponse(c, s);
                  thisPtr.requestCancelTimer.SetIfValid(timeToCancelRequest);
                  return response;
                }), (IteratorAsyncResult<ServiceBusResourceOperations.DeleteInstallationAsyncResult>.EndCall) ((thisPtr, r) =>
                {
                  ServiceBusResourceOperations.DeleteInstallationAsyncResult installationAsyncResult = thisPtr;
                  installationAsyncResult.response = (HttpWebResponse) installationAsyncResult.request.EndGetResponse(r);
                }), IteratorAsyncResult<ServiceBusResourceOperations.DeleteInstallationAsyncResult>.ExceptionPolicy.Continue);
                if (this.LastAsyncStepException != null)
                {
                  WebException asyncStepException1 = this.LastAsyncStepException as WebException;
                  IOException asyncStepException2 = this.LastAsyncStepException as IOException;
                  if (asyncStepException1 != null)
                    this.LastAsyncStepException = ServiceBusResourceOperations.ConvertWebException(this.trackingContext, asyncStepException1, this.request.Timeout);
                  if (asyncStepException2 != null)
                    this.LastAsyncStepException = ServiceBusResourceOperations.ConvertIOException(this.trackingContext, asyncStepException2, this.request.Timeout, this.isRequestAborted);
                  if (this.LastAsyncStepException is MessagingEntityNotFoundException)
                  {
                    this.LastAsyncStepException = (Exception) null;
                    this.retryPolicy.ResetServerBusy();
                    asyncSteps = false;
                    goto label_42;
                  }
                  else
                  {
                    shouldRetry = !this.TransactionExists && this.retryPolicy.ShouldRetry(this.RemainingTime(), iteration, this.LastAsyncStepException, out iterationSleep);
                    if (shouldRetry)
                    {
                      string operation = string.Format("Delete:{0}", (object) this.resourceUri.AbsoluteUri);
                      MessagingClientEtwProvider.TraceClient((Action) (() => MessagingClientEtwProvider.Provider.EventWriteRetryPolicyIteration((EventTraceActivity) null, string.Empty, this.retryPolicy.GetType().Name, operation, iteration, iterationSleep.ToString(), this.LastAsyncStepException.GetType().FullName, this.LastAsyncStepException.Message)));
                      ++iteration;
                      goto label_43;
                    }
                  }
                }
                else
                {
                  try
                  {
                    using (this.response)
                    {
                      if (this.response.StatusCode != HttpStatusCode.NoContent)
                        this.LastAsyncStepException = (Exception) new MessagingException(this.response.StatusDescription, (Exception) new WebException(this.response.StatusDescription, (Exception) null, WebExceptionStatus.ProtocolError, (WebResponse) this.response));
                    }
                  }
                  catch (WebException ex)
                  {
                    this.LastAsyncStepException = ServiceBusResourceOperations.ConvertWebException(this.trackingContext, ex, this.request.Timeout);
                  }
                  catch (IOException ex)
                  {
                    this.LastAsyncStepException = ServiceBusResourceOperations.ConvertIOException(this.trackingContext, ex, this.request.Timeout, this.isRequestAborted);
                  }
                  if (this.LastAsyncStepException != null)
                  {
                    shouldRetry = !this.TransactionExists && this.retryPolicy.ShouldRetry(this.RemainingTime(), iteration, this.LastAsyncStepException, out iterationSleep);
                    if (shouldRetry)
                    {
                      string operation = string.Format("Delete:{0}", (object) this.resourceUri.AbsoluteUri);
                      MessagingClientEtwProvider.TraceClient((Action) (() => MessagingClientEtwProvider.Provider.EventWriteRetryPolicyIteration((EventTraceActivity) null, string.Empty, this.retryPolicy.GetType().Name, operation, iteration, iterationSleep.ToString(), this.LastAsyncStepException.GetType().FullName, this.LastAsyncStepException.Message)));
                      ++iteration;
                      goto label_43;
                    }
                  }
                  else
                    this.retryPolicy.ResetServerBusy();
                }
                goto label_44;
label_43:
                goto label_44;
              }
label_42:
              goto label_27;
            }
            finally
            {
              this.requestCancelTimer.Cancel();
            }
label_44:;
          }
          while (this.uriManager.MoveNextUri() & shouldRetry);
          goto label_45;
label_27:
          return asyncSteps;
label_45:
          this.Complete(this.LastAsyncStepException);
        }
      }

      private void CancelRequest(object state)
      {
        this.isRequestAborted = true;
        ((WebRequest) state).Abort();
      }

      private class InstallationResourceDescription : IResourceDescription
      {
        public string CollectionName => "installations";
      }
    }

    private sealed class CreateOrUpdateAsyncResult<TEntityDescription> : 
      IteratorAsyncResult<ServiceBusResourceOperations.CreateOrUpdateAsyncResult<TEntityDescription>>
      where TEntityDescription : EntityDescription, IResourceDescription
    {
      private readonly TrackingContext trackingContext;
      private readonly EventTraceActivity relatedActivity;
      private readonly SyndicationItem feedItem;
      private readonly NamespaceManagerSettings settings;
      private readonly TokenProvider tokenProvider;
      private readonly bool isAnonymousAccessible;
      private readonly ServiceBusUriManager uriManager;
      private readonly EntityDescription entityDescription;
      private readonly IDictionary<string, string> queryParametersAndValues;
      private HttpWebRequest request;
      private Uri currentResourceUri;
      private Stream requestStream;
      private HttpWebResponse response;
      private IResourceDescription[] collectionDescriptions;
      private string[] resourceNames;
      private bool isUpdate;
      private IOThreadTimer requestCancelTimer;
      private volatile bool isRequestAborted;

      public CreateOrUpdateAsyncResult(
        TrackingContext trackingContext,
        TEntityDescription resourceDescription,
        IResourceDescription[] collectionDescriptions,
        string[] resourceNames,
        IEnumerable<Uri> baseAddresses,
        TimeSpan timeout,
        bool isAnonymousAccessible,
        bool isUpdate,
        IDictionary<string, string> queryParametersAndValues,
        NamespaceManagerSettings settings,
        AsyncCallback callback,
        object state)
        : base(timeout, callback, state)
      {
        this.trackingContext = trackingContext != null ? trackingContext : throw new ArgumentNullException(nameof (trackingContext));
        this.settings = settings;
        this.tokenProvider = settings.TokenProvider;
        this.collectionDescriptions = collectionDescriptions;
        this.isAnonymousAccessible = isAnonymousAccessible;
        this.resourceNames = resourceNames;
        this.feedItem = new SyndicationItem();
        this.feedItem.Content = (SyndicationContent) new XmlSyndicationContent("application/atom+xml;type=entry;charset=utf-8", new SyndicationElementExtension((object) resourceDescription));
        this.uriManager = new ServiceBusUriManager(baseAddresses.ToList<Uri>());
        this.isUpdate = isUpdate;
        this.entityDescription = (EntityDescription) resourceDescription;
        this.relatedActivity = EventTraceActivity.CreateFromThread();
        this.queryParametersAndValues = queryParametersAndValues;
        this.Start();
      }

      public TEntityDescription Result { get; private set; }

      private string getMethod() => typeof (TEntityDescription).FullName.Contains("RegistrationDescription") ? (!this.isUpdate ? "POST" : "PUT") : (typeof (TEntityDescription).FullName.Contains("NotificationHubJob") ? "POST" : "PUT");

      protected override IEnumerator<IteratorAsyncResult<ServiceBusResourceOperations.CreateOrUpdateAsyncResult<TEntityDescription>>.AsyncStep> GetAsyncSteps()
      {
        while (this.uriManager.MoveNextUri())
        {
          this.LastAsyncStepException = (Exception) null;
          this.currentResourceUri = ServiceBusResourceOperations.CreateResourceUri(this.uriManager.Current, this.collectionDescriptions, this.resourceNames, this.queryParametersAndValues);
          this.request = (HttpWebRequest) WebRequest.Create(this.currentResourceUri);
          if (ServiceBusEnvironment.Proxy != null)
            this.request.Proxy = ServiceBusEnvironment.Proxy;
          this.request.Headers.Add("X-MS-ISANONYMOUSACCESSIBLE", this.isAnonymousAccessible.ToString());
          this.request.Method = this.getMethod();
          this.request.ServicePoint.MaxIdleTime = Constants.ServicePointMaxIdleTimeMilliSeconds;
          this.request.ContentType = "application/atom+xml;type=entry;charset=utf-8";
          this.request.Timeout = this.OriginalTimeout.TotalMilliseconds > (double) int.MaxValue ? int.MaxValue : (int) this.OriginalTimeout.TotalMilliseconds;
          if (this.isUpdate)
            this.request.Headers.Add(HttpRequestHeader.IfMatch, "*");
          this.request.SetUserAgentHeader();
          this.request.AddXProcessAtHeader();
          this.request.AddAuthorizationHeader(this.tokenProvider, this.uriManager.Current, "Manage");
          this.request.AddTrackingIdHeader(this.trackingContext);
          this.request.AddCorrelationHeader(this.relatedActivity);
          if (!(this.entityDescription is NotificationHubDescription) && this.entityDescription is RegistrationDescription && this.isUpdate)
            this.request.Headers[HttpRequestHeader.IfMatch] = ((RegistrationDescription) this.entityDescription).ETag;
          this.requestCancelTimer = new IOThreadTimer(new Action<object>(this.CancelRequest), (object) this.request, true);
          bool asyncSteps;
          try
          {
            TimeSpan timeToCancelRequest = this.RemainingTime();
            if (timeToCancelRequest <= TimeSpan.Zero)
            {
              this.Complete((Exception) new TimeoutException(SRClient.OperationRequestTimedOut((object) this.OriginalTimeout)));
              asyncSteps = false;
            }
            else
            {
              try
              {
                yield return this.CallAsync((IteratorAsyncResult<ServiceBusResourceOperations.CreateOrUpdateAsyncResult<TEntityDescription>>.BeginCall) ((thisPtr, t, c, s) =>
                {
                  IAsyncResult requestStream = thisPtr.request.BeginGetRequestStream(c, s);
                  thisPtr.requestCancelTimer.SetIfValid(timeToCancelRequest);
                  return requestStream;
                }), (IteratorAsyncResult<ServiceBusResourceOperations.CreateOrUpdateAsyncResult<TEntityDescription>>.EndCall) ((thisPtr, r) =>
                {
                  ServiceBusResourceOperations.CreateOrUpdateAsyncResult<TEntityDescription> updateAsyncResult = thisPtr;
                  updateAsyncResult.requestStream = updateAsyncResult.request.EndGetRequestStream(r);
                }), IteratorAsyncResult<ServiceBusResourceOperations.CreateOrUpdateAsyncResult<TEntityDescription>>.ExceptionPolicy.Continue);
                if (this.LastAsyncStepException != null)
                {
                  if (!this.uriManager.CanRetry() || !ServiceBusResourceOperations.IsRetriableException(this.LastAsyncStepException))
                  {
                    Exception operationException = this.LastAsyncStepException;
                    if (operationException is WebException webException)
                      operationException = ServiceBusResourceOperations.ConvertWebException(this.trackingContext, webException, this.request.Timeout, this.isUpdate);
                    if (operationException is IOException ioException)
                      operationException = ServiceBusResourceOperations.ConvertIOException(this.trackingContext, ioException, this.request.Timeout, this.isRequestAborted);
                    this.Complete(operationException);
                    asyncSteps = false;
                    goto label_30;
                  }
                }
                else
                {
                  try
                  {
                    Stream requestStream = this.requestStream;
                    using (XmlWriter writer = XmlWriter.Create(requestStream, new XmlWriterSettings()
                    {
                      Encoding = Encoding.UTF8,
                      CloseOutput = true
                    }))
                    {
                      this.feedItem.GetAtom10Formatter().WriteTo(writer);
                      goto label_13;
                    }
                  }
                  catch (WebException ex)
                  {
                    if (this.uriManager.CanRetry())
                    {
                      if (ServiceBusResourceOperations.IsRetriableException((Exception) ex))
                        goto label_29;
                    }
                    this.Complete(ServiceBusResourceOperations.ConvertWebException(this.trackingContext, ex, this.request.Timeout, this.isUpdate));
                    asyncSteps = false;
                    goto label_30;
                  }
                  catch (IOException ex)
                  {
                    this.Complete(ServiceBusResourceOperations.ConvertIOException(this.trackingContext, ex, this.request.Timeout, this.isRequestAborted));
                    asyncSteps = false;
                    goto label_30;
                  }
                }
label_29:
                goto label_58;
label_30:
                goto label_57;
              }
              finally
              {
                this.requestCancelTimer.Cancel();
              }
label_13:
              TimeSpan timeToCancelResponse = this.RemainingTime();
              if (timeToCancelResponse <= TimeSpan.Zero)
              {
                this.Complete((Exception) new TimeoutException(SRClient.OperationRequestTimedOut((object) this.OriginalTimeout)));
                asyncSteps = false;
                goto label_57;
              }
              else
              {
                yield return this.CallAsync((IteratorAsyncResult<ServiceBusResourceOperations.CreateOrUpdateAsyncResult<TEntityDescription>>.BeginCall) ((thisPtr, t, c, s) =>
                {
                  IAsyncResult response = thisPtr.request.BeginGetResponse(c, s);
                  thisPtr.requestCancelTimer.Set(timeToCancelResponse);
                  return response;
                }), (IteratorAsyncResult<ServiceBusResourceOperations.CreateOrUpdateAsyncResult<TEntityDescription>>.EndCall) ((thisPtr, r) =>
                {
                  ServiceBusResourceOperations.CreateOrUpdateAsyncResult<TEntityDescription> updateAsyncResult = thisPtr;
                  updateAsyncResult.response = (HttpWebResponse) updateAsyncResult.request.EndGetResponse(r);
                }), IteratorAsyncResult<ServiceBusResourceOperations.CreateOrUpdateAsyncResult<TEntityDescription>>.ExceptionPolicy.Continue);
                if (this.LastAsyncStepException != null)
                {
                  if (!this.uriManager.CanRetry() || !ServiceBusResourceOperations.IsRetriableException(this.LastAsyncStepException))
                  {
                    Exception operationException = this.LastAsyncStepException;
                    if (operationException is WebException webException)
                      operationException = ServiceBusResourceOperations.ConvertWebException(this.trackingContext, webException, this.request.Timeout, this.isUpdate);
                    if (operationException is IOException ioException)
                      operationException = ServiceBusResourceOperations.ConvertIOException(this.trackingContext, ioException, this.request.Timeout, this.isRequestAborted);
                    this.Complete(operationException);
                    asyncSteps = false;
                    goto label_57;
                  }
                }
                else
                {
                  try
                  {
                    using (this.response)
                    {
                      if (this.response.StatusCode != HttpStatusCode.Created && this.response.StatusCode != HttpStatusCode.OK)
                      {
                        if (!this.uriManager.CanRetry())
                        {
                          this.Complete(ServiceBusResourceOperations.ConvertWebException(this.trackingContext, new WebException(this.response.StatusDescription, (Exception) null, WebExceptionStatus.ProtocolError, (WebResponse) this.response), this.request.Timeout, this.isUpdate));
                          asyncSteps = false;
                          goto label_57;
                        }
                        else
                          goto label_58;
                      }
                      else
                      {
                        Stream responseStream = this.response.GetResponseStream();
                        using (XmlReader reader = XmlReader.Create(responseStream, new XmlReaderSettings()
                        {
                          CloseInput = true
                        }))
                        {
                          Atom10ItemFormatter atom10ItemFormatter = new Atom10ItemFormatter();
                          atom10ItemFormatter.ReadFrom(reader);
                          this.Result = ((XmlSyndicationContent) atom10ItemFormatter.Item.Content).ReadContent<TEntityDescription>();
                        }
                      }
                    }
                  }
                  catch (XmlException ex)
                  {
                    this.Complete((Exception) new MessagingException(SRClient.InvalidXmlFormat, false, (Exception) ex));
                    asyncSteps = false;
                    goto label_57;
                  }
                  catch (WebException ex)
                  {
                    if (this.uriManager.CanRetry())
                    {
                      if (ServiceBusResourceOperations.IsRetriableException((Exception) ex))
                        goto label_58;
                    }
                    this.Complete(ServiceBusResourceOperations.ConvertWebException(this.trackingContext, ex, this.request.Timeout, this.isUpdate));
                    asyncSteps = false;
                    goto label_57;
                  }
                  catch (IOException ex)
                  {
                    this.Complete(ServiceBusResourceOperations.ConvertIOException(this.trackingContext, ex, this.request.Timeout, this.isRequestAborted));
                  }
                  break;
                }
              }
label_58:
              continue;
            }
label_57:;
          }
          finally
          {
            this.requestCancelTimer.Cancel();
          }
          return asyncSteps;
        }
      }

      private void CancelRequest(object state)
      {
        this.isRequestAborted = true;
        ((WebRequest) state).Abort();
      }
    }

    private sealed class DeleteAsyncResult : 
      RetryAsyncResult<ServiceBusResourceOperations.DeleteAsyncResult>
    {
      private readonly TrackingContext trackingContext;
      private readonly EventTraceActivity relatedActivity;
      private readonly NamespaceManagerSettings settings;
      private readonly TokenProvider tokenProvider;
      private readonly ServiceBusUriManager uriManager;
      private readonly IResourceDescription[] collectionDescriptions;
      private readonly string[] collectionResourceNames;
      private readonly RetryPolicy retryPolicy;
      private Uri resourceUri;
      private HttpWebResponse response;
      private HttpWebRequest request;
      private IOThreadTimer requestCancelTimer;
      private volatile bool isRequestAborted;
      private Dictionary<string, string> additionalHeaders;

      public DeleteAsyncResult(
        TrackingContext trackingContext,
        IResourceDescription[] collectionDescriptions,
        string[] collectionResourceNames,
        IEnumerable<Uri> baseAddresses,
        Dictionary<string, string> additionalHeaders,
        NamespaceManagerSettings settings,
        TimeSpan timeout,
        AsyncCallback callback,
        object state)
        : base(timeout, callback, state)
      {
        this.trackingContext = trackingContext != null ? trackingContext : throw new ArgumentNullException(nameof (trackingContext));
        this.settings = settings;
        this.tokenProvider = settings.TokenProvider;
        this.retryPolicy = settings.RetryPolicy;
        this.collectionDescriptions = collectionDescriptions;
        this.collectionResourceNames = collectionResourceNames;
        this.uriManager = new ServiceBusUriManager(baseAddresses.ToList<Uri>(), true);
        this.relatedActivity = EventTraceActivity.CreateFromThread();
        this.additionalHeaders = additionalHeaders;
        this.Start();
      }

      public DeleteAsyncResult(
        TrackingContext trackingContext,
        IResourceDescription[] collectionDescriptions,
        string[] collectionResourceNames,
        IEnumerable<Uri> baseAddresses,
        NamespaceManagerSettings settings,
        TimeSpan timeout,
        AsyncCallback callback,
        object state)
        : this(trackingContext, collectionDescriptions, collectionResourceNames, baseAddresses, (Dictionary<string, string>) null, settings, timeout, callback, state)
      {
      }

      protected override IEnumerator<IteratorAsyncResult<ServiceBusResourceOperations.DeleteAsyncResult>.AsyncStep> GetAsyncSteps()
      {
        int iteration = 0;
        TimeSpan iterationSleep = this.retryPolicy.IsServerBusy ? RetryPolicy.ServerBusyBaseSleepTime : TimeSpan.Zero;
        if (this.retryPolicy.IsServerBusy && RetryPolicy.ServerBusyBaseSleepTime >= this.OriginalTimeout)
        {
          string serverBusyExceptionMessage = this.retryPolicy.ServerBusyExceptionMessage;
          yield return this.CallAsyncSleep(this.RemainingTime());
          this.Complete((Exception) new ServerBusyException(serverBusyExceptionMessage));
        }
        else
        {
          bool shouldRetry;
          bool asyncSteps;
          do
          {
            shouldRetry = false;
            if (iterationSleep != TimeSpan.Zero)
              yield return this.CallAsyncSleep(iterationSleep);
            this.LastAsyncStepException = (Exception) null;
            this.resourceUri = ServiceBusResourceOperations.CreateResourceUri(this.uriManager.Current, this.collectionDescriptions, this.collectionResourceNames, (IDictionary<string, string>) null);
            this.request = (HttpWebRequest) WebRequest.Create(this.resourceUri);
            if (ServiceBusEnvironment.Proxy != null)
              this.request.Proxy = ServiceBusEnvironment.Proxy;
            this.request.ServicePoint.MaxIdleTime = Constants.ServicePointMaxIdleTimeMilliSeconds;
            this.request.ContentType = "application/atom+xml;type=entry;charset=utf-8";
            this.request.Method = "DELETE";
            HttpWebRequest request = this.request;
            TimeSpan originalTimeout = this.OriginalTimeout;
            int num;
            if (originalTimeout.TotalMilliseconds <= (double) int.MaxValue)
            {
              originalTimeout = this.OriginalTimeout;
              num = (int) originalTimeout.TotalMilliseconds;
            }
            else
              num = int.MaxValue;
            request.Timeout = num;
            this.request.SetUserAgentHeader();
            this.request.AddXProcessAtHeader();
            this.request.AddAuthorizationHeader(this.tokenProvider, this.uriManager.Current, "Manage");
            this.request.AddTrackingIdHeader(this.trackingContext);
            this.request.AddCorrelationHeader(this.relatedActivity);
            this.requestCancelTimer = new IOThreadTimer(new Action<object>(this.CancelRequest), (object) this.request, true);
            if (this.additionalHeaders != null)
            {
              foreach (KeyValuePair<string, string> additionalHeader in this.additionalHeaders)
                this.request.Headers.Add(additionalHeader.Key, additionalHeader.Value);
            }
            try
            {
              TimeSpan timeToCancelRequest = this.RemainingTime();
              if (timeToCancelRequest <= TimeSpan.Zero)
              {
                this.Complete((Exception) new TimeoutException(SRClient.OperationRequestTimedOut((object) this.OriginalTimeout)));
                asyncSteps = false;
              }
              else
              {
                yield return this.CallAsync((IteratorAsyncResult<ServiceBusResourceOperations.DeleteAsyncResult>.BeginCall) ((thisPtr, t, c, s) =>
                {
                  IAsyncResult response = thisPtr.request.BeginGetResponse(c, s);
                  thisPtr.requestCancelTimer.SetIfValid(timeToCancelRequest);
                  return response;
                }), (IteratorAsyncResult<ServiceBusResourceOperations.DeleteAsyncResult>.EndCall) ((thisPtr, r) =>
                {
                  ServiceBusResourceOperations.DeleteAsyncResult deleteAsyncResult = thisPtr;
                  deleteAsyncResult.response = (HttpWebResponse) deleteAsyncResult.request.EndGetResponse(r);
                }), IteratorAsyncResult<ServiceBusResourceOperations.DeleteAsyncResult>.ExceptionPolicy.Continue);
                if (this.LastAsyncStepException != null)
                {
                  WebException asyncStepException1 = this.LastAsyncStepException as WebException;
                  IOException asyncStepException2 = this.LastAsyncStepException as IOException;
                  if (asyncStepException1 != null)
                    this.LastAsyncStepException = ServiceBusResourceOperations.ConvertWebException(this.trackingContext, asyncStepException1, this.request.Timeout);
                  if (asyncStepException2 != null)
                    this.LastAsyncStepException = ServiceBusResourceOperations.ConvertIOException(this.trackingContext, asyncStepException2, this.request.Timeout, this.isRequestAborted);
                  if (this.LastAsyncStepException is MessagingEntityNotFoundException)
                  {
                    this.LastAsyncStepException = (Exception) null;
                    this.retryPolicy.ResetServerBusy();
                    asyncSteps = false;
                    goto label_42;
                  }
                  else
                  {
                    shouldRetry = !this.TransactionExists && this.retryPolicy.ShouldRetry(this.RemainingTime(), iteration, this.LastAsyncStepException, out iterationSleep);
                    if (shouldRetry)
                    {
                      string operation = string.Format("Delete:{0}", (object) this.resourceUri.AbsoluteUri);
                      MessagingClientEtwProvider.TraceClient((Action) (() => MessagingClientEtwProvider.Provider.EventWriteRetryPolicyIteration((EventTraceActivity) null, string.Empty, this.retryPolicy.GetType().Name, operation, iteration, iterationSleep.ToString(), this.LastAsyncStepException.GetType().FullName, this.LastAsyncStepException.Message)));
                      ++iteration;
                      goto label_43;
                    }
                  }
                }
                else
                {
                  try
                  {
                    using (this.response)
                    {
                      if (this.response.StatusCode != HttpStatusCode.OK)
                        this.LastAsyncStepException = (Exception) new MessagingException(this.response.StatusDescription, (Exception) new WebException(this.response.StatusDescription, (Exception) null, WebExceptionStatus.ProtocolError, (WebResponse) this.response));
                    }
                  }
                  catch (WebException ex)
                  {
                    this.LastAsyncStepException = ServiceBusResourceOperations.ConvertWebException(this.trackingContext, ex, this.request.Timeout);
                  }
                  catch (IOException ex)
                  {
                    this.LastAsyncStepException = ServiceBusResourceOperations.ConvertIOException(this.trackingContext, ex, this.request.Timeout, this.isRequestAborted);
                  }
                  if (this.LastAsyncStepException != null)
                  {
                    shouldRetry = !this.TransactionExists && this.retryPolicy.ShouldRetry(this.RemainingTime(), iteration, this.LastAsyncStepException, out iterationSleep);
                    if (shouldRetry)
                    {
                      string operation = string.Format("Delete:{0}", (object) this.resourceUri.AbsoluteUri);
                      MessagingClientEtwProvider.TraceClient((Action) (() => MessagingClientEtwProvider.Provider.EventWriteRetryPolicyIteration((EventTraceActivity) null, string.Empty, this.retryPolicy.GetType().Name, operation, iteration, iterationSleep.ToString(), this.LastAsyncStepException.GetType().FullName, this.LastAsyncStepException.Message)));
                      ++iteration;
                      goto label_43;
                    }
                  }
                  else
                    this.retryPolicy.ResetServerBusy();
                }
                goto label_44;
label_43:
                goto label_44;
              }
label_42:
              goto label_27;
            }
            finally
            {
              this.requestCancelTimer.Cancel();
            }
label_44:;
          }
          while (this.uriManager.MoveNextUri() & shouldRetry);
          goto label_45;
label_27:
          return asyncSteps;
label_45:
          this.Complete(this.LastAsyncStepException);
        }
      }

      private void CancelRequest(object state)
      {
        this.isRequestAborted = true;
        ((WebRequest) state).Abort();
      }
    }

    private sealed class GetAsyncResult<TEntityDescription> : 
      RetryAsyncResult<ServiceBusResourceOperations.GetAsyncResult<TEntityDescription>>
      where TEntityDescription : EntityDescription, IResourceDescription
    {
      private readonly TrackingContext trackingContext;
      private readonly EventTraceActivity relatedActivity;
      private readonly NamespaceManagerSettings settings;
      private readonly TokenProvider tokenProvider;
      private readonly MessagingDescriptionSerializer<TEntityDescription> serializer;
      private readonly IResourceDescription[] collectionDescriptions;
      private readonly string[] collectionResourceNames;
      private readonly ServiceBusUriManager uriManager;
      private readonly RetryPolicy retryPolicy;
      private HttpWebRequest request;
      private HttpWebResponse response;
      private IOThreadTimer requestCancelTimer;
      private volatile bool isRequestAborted;

      public GetAsyncResult(
        TrackingContext trackingContext,
        IResourceDescription[] collectionDescriptions,
        string[] collectionResourceNames,
        IEnumerable<Uri> managementAddresses,
        NamespaceManagerSettings settings,
        TimeSpan timeout,
        AsyncCallback callback,
        object state)
        : base(timeout, callback, state)
      {
        if (trackingContext == null)
          throw new ArgumentNullException(nameof (trackingContext));
        if (collectionResourceNames == null)
          throw FxTrace.Exception.ArgumentNull(nameof (collectionResourceNames));
        if (settings == null)
          throw FxTrace.Exception.ArgumentNull(nameof (settings));
        this.trackingContext = trackingContext;
        this.ResourceNames = collectionResourceNames;
        this.settings = settings;
        this.tokenProvider = settings.TokenProvider;
        this.retryPolicy = settings.RetryPolicy;
        this.collectionDescriptions = collectionDescriptions;
        this.collectionResourceNames = collectionResourceNames;
        this.uriManager = new ServiceBusUriManager(managementAddresses.ToList<Uri>(), true);
        this.serializer = new MessagingDescriptionSerializer<TEntityDescription>();
        this.relatedActivity = EventTraceActivity.CreateFromThread();
        this.Start();
      }

      public TEntityDescription Result { get; private set; }

      public string[] ResourceNames { get; private set; }

      protected override IEnumerator<IteratorAsyncResult<ServiceBusResourceOperations.GetAsyncResult<TEntityDescription>>.AsyncStep> GetAsyncSteps()
      {
        int iteration = 0;
        TimeSpan iterationSleep = this.retryPolicy.IsServerBusy ? RetryPolicy.ServerBusyBaseSleepTime : TimeSpan.Zero;
        if (this.retryPolicy.IsServerBusy && RetryPolicy.ServerBusyBaseSleepTime >= this.OriginalTimeout)
        {
          string serverBusyExceptionMessage = this.retryPolicy.ServerBusyExceptionMessage;
          yield return this.CallAsyncSleep(this.RemainingTime());
          this.Complete((Exception) new ServerBusyException(serverBusyExceptionMessage));
        }
        else
        {
          bool shouldRetry;
          do
          {
            shouldRetry = false;
            if (iterationSleep != TimeSpan.Zero)
              yield return this.CallAsyncSleep(iterationSleep);
            this.LastAsyncStepException = (Exception) null;
            Uri resourceUri = ServiceBusResourceOperations.CreateResourceUri(this.uriManager.Current, this.collectionDescriptions, this.collectionResourceNames, (IDictionary<string, string>) null);
            this.request = (HttpWebRequest) WebRequest.Create(resourceUri);
            if (ServiceBusEnvironment.Proxy != null)
              this.request.Proxy = ServiceBusEnvironment.Proxy;
            this.request.ServicePoint.MaxIdleTime = Constants.ServicePointMaxIdleTimeMilliSeconds;
            this.request.Method = "GET";
            this.request.ContentType = "application/atom+xml;type=entry;charset=utf-8";
            HttpWebRequest request = this.request;
            TimeSpan originalTimeout = this.OriginalTimeout;
            int num;
            if (originalTimeout.TotalMilliseconds <= (double) int.MaxValue)
            {
              originalTimeout = this.OriginalTimeout;
              num = (int) originalTimeout.TotalMilliseconds;
            }
            else
              num = int.MaxValue;
            request.Timeout = num;
            this.request.SetUserAgentHeader();
            this.request.AddXProcessAtHeader();
            this.request.AddAuthorizationHeader(this.tokenProvider, this.uriManager.Current, "Manage");
            this.request.AddTrackingIdHeader(this.trackingContext);
            this.request.AddCorrelationHeader(this.relatedActivity);
            this.requestCancelTimer = new IOThreadTimer(new Action<object>(this.CancelRequest), (object) this.request, true);
            try
            {
              TimeSpan timeToCancelRequest = this.RemainingTime();
              if (timeToCancelRequest <= TimeSpan.Zero)
              {
                this.Complete((Exception) new TimeoutException(SRClient.OperationRequestTimedOut((object) this.OriginalTimeout)));
                yield break;
              }
              else
              {
                yield return this.CallAsync((IteratorAsyncResult<ServiceBusResourceOperations.GetAsyncResult<TEntityDescription>>.BeginCall) ((thisPtr, t, c, s) =>
                {
                  IAsyncResult response = thisPtr.request.BeginGetResponse(c, s);
                  thisPtr.requestCancelTimer.SetIfValid(timeToCancelRequest);
                  return response;
                }), (IteratorAsyncResult<ServiceBusResourceOperations.GetAsyncResult<TEntityDescription>>.EndCall) ((thisPtr, r) =>
                {
                  ServiceBusResourceOperations.GetAsyncResult<TEntityDescription> getAsyncResult = thisPtr;
                  getAsyncResult.response = (HttpWebResponse) getAsyncResult.request.EndGetResponse(r);
                }), IteratorAsyncResult<ServiceBusResourceOperations.GetAsyncResult<TEntityDescription>>.ExceptionPolicy.Continue);
                if (this.LastAsyncStepException != null)
                {
                  WebException asyncStepException1 = this.LastAsyncStepException as WebException;
                  IOException asyncStepException2 = this.LastAsyncStepException as IOException;
                  if (asyncStepException1 != null)
                    this.LastAsyncStepException = ServiceBusResourceOperations.ConvertWebException(this.trackingContext, asyncStepException1, this.request.Timeout);
                  if (asyncStepException2 != null)
                    this.LastAsyncStepException = ServiceBusResourceOperations.ConvertIOException(this.trackingContext, asyncStepException2, this.request.Timeout, this.isRequestAborted);
                  TimeSpan remainingTime = this.RemainingTime();
                  shouldRetry = !this.TransactionExists && this.retryPolicy.ShouldRetry(remainingTime, iteration, this.LastAsyncStepException, out iterationSleep);
                  if (shouldRetry)
                  {
                    string operation = string.Format("Get:{0}", (object) resourceUri.AbsoluteUri);
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    MessagingClientEtwProvider.TraceClient((Action) (() => MessagingClientEtwProvider.Provider.EventWriteRetryPolicyIteration((EventTraceActivity) null, string.Empty, this.CS\u0024\u003C\u003E8__locals2.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.retryPolicy.GetType().Name, operation, iteration, iterationSleep.ToString(), this.CS\u0024\u003C\u003E8__locals2.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.LastAsyncStepException.GetType().FullName, this.CS\u0024\u003C\u003E8__locals2.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.LastAsyncStepException.Message)));
                    ++iteration;
                    goto label_44;
                  }
                }
                else
                {
                  try
                  {
                    using (this.response)
                    {
                      bool flag = this.response.ContentType.Replace(" ", "").Equals("application/atom+xml;type=feed;charset=utf-8", StringComparison.OrdinalIgnoreCase);
                      string entityName = this.ResourceNames.Length == 0 || string.IsNullOrEmpty(this.ResourceNames[0]) ? "Null" : this.ResourceNames[0];
                      if (this.response.StatusCode != HttpStatusCode.OK)
                        this.LastAsyncStepException = (Exception) new MessagingException(this.response.StatusDescription, false, (Exception) new WebException(this.response.StatusDescription, (Exception) null, WebExceptionStatus.ProtocolError, (WebResponse) this.response));
                      else if (flag && (typeof (TEntityDescription).FullName.Contains("TopicDescription") || typeof (TEntityDescription).FullName.Contains("QueueDescription") || typeof (TEntityDescription).FullName.Contains("SubscriptionDescription") || typeof (TEntityDescription).FullName.Contains("RelayDescription") || typeof (TEntityDescription).FullName.Contains("NotificationHubDescription") || typeof (TEntityDescription).FullName.Contains("ConsumerGroupDescription") || typeof (TEntityDescription).FullName.Contains("PartitionDescription") || typeof (TEntityDescription).FullName.Contains("EventHubDescription")))
                      {
                        this.LastAsyncStepException = (Exception) new MessagingEntityNotFoundException(entityName);
                      }
                      else
                      {
                        using (Stream responseStream = this.response.GetResponseStream())
                        {
                          try
                          {
                            this.Result = this.serializer.DeserializeFromAtomFeed(responseStream);
                          }
                          catch (InvalidCastException ex)
                          {
                            this.LastAsyncStepException = (Exception) new MessagingException(SRClient.InvalidManagementEntityType((object) entityName, (object) typeof (TEntityDescription).Name), false, (Exception) ex);
                          }
                          catch (SerializationException ex)
                          {
                            this.LastAsyncStepException = (Exception) new MessagingException(SRClient.InvalidManagementEntityType((object) entityName, (object) typeof (TEntityDescription).Name), false, (Exception) ex);
                          }
                        }
                      }
                    }
                  }
                  catch (XmlException ex)
                  {
                    this.LastAsyncStepException = (Exception) new MessagingException(SRClient.InvalidXmlFormat, false, (Exception) ex);
                  }
                  catch (WebException ex)
                  {
                    this.LastAsyncStepException = ServiceBusResourceOperations.ConvertWebException(this.trackingContext, ex, this.request.Timeout);
                  }
                  catch (IOException ex)
                  {
                    this.LastAsyncStepException = ServiceBusResourceOperations.ConvertIOException(this.trackingContext, ex, this.request.Timeout, this.isRequestAborted);
                  }
                  if (this.LastAsyncStepException != null)
                  {
                    TimeSpan remainingTime = this.RemainingTime();
                    shouldRetry = !this.TransactionExists && this.retryPolicy.ShouldRetry(remainingTime, iteration, this.LastAsyncStepException, out iterationSleep);
                    if (shouldRetry)
                    {
                      string operation = string.Format("Get:{0}", (object) resourceUri.AbsoluteUri);
                      // ISSUE: reference to a compiler-generated field
                      // ISSUE: reference to a compiler-generated field
                      // ISSUE: reference to a compiler-generated field
                      // ISSUE: reference to a compiler-generated field
                      // ISSUE: reference to a compiler-generated field
                      // ISSUE: reference to a compiler-generated field
                      // ISSUE: reference to a compiler-generated field
                      // ISSUE: reference to a compiler-generated field
                      // ISSUE: reference to a compiler-generated field
                      MessagingClientEtwProvider.TraceClient((Action) (() => MessagingClientEtwProvider.Provider.EventWriteRetryPolicyIteration((EventTraceActivity) null, string.Empty, this.CS\u0024\u003C\u003E8__locals3.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.retryPolicy.GetType().Name, operation, iteration, iterationSleep.ToString(), this.CS\u0024\u003C\u003E8__locals3.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.LastAsyncStepException.GetType().FullName, this.CS\u0024\u003C\u003E8__locals3.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.LastAsyncStepException.Message)));
                      ++iteration;
                      goto label_44;
                    }
                  }
                  else
                    this.retryPolicy.ResetServerBusy();
                }
                goto label_40;
label_44:
                goto label_45;
              }
            }
            finally
            {
              this.requestCancelTimer.Cancel();
            }
label_40:
            resourceUri = (Uri) null;
label_45:;
          }
          while (this.uriManager.MoveNextUri() & shouldRetry);
          this.Complete(this.LastAsyncStepException);
        }
      }

      private void CancelRequest(object state)
      {
        this.isRequestAborted = true;
        ((WebRequest) state).Abort();
      }
    }

    private sealed class GetAllAsyncResult : 
      RetryAsyncResult<ServiceBusResourceOperations.GetAllAsyncResult>
    {
      private static readonly Func<SyndicationLink, bool> nextLinkPredicate = (Func<SyndicationLink, bool>) (link => string.Equals(link.RelationshipType, "next", StringComparison.Ordinal));
      private readonly TrackingContext trackingContext;
      private readonly EventTraceActivity relatedActivity;
      private readonly IResourceDescription[] descriptions;
      private readonly TokenProvider tokenProvider;
      private readonly int timeoutInMilliseconds;
      private readonly string[] resourceNames;
      private readonly NamespaceManagerSettings settings;
      private readonly ServiceBusUriManager uriManager;
      private readonly string filter;
      private readonly int skip;
      private readonly int top;
      private readonly ContinuationToken continuationToken;
      private readonly bool stopAfterOnePage;
      private HttpWebRequest request;
      private HttpWebResponse response;
      private IOThreadTimer requestCancelTimer;
      private TimeoutHelper operationTimer;
      private volatile bool isRequestAborted;

      public GetAllAsyncResult(
        TrackingContext trackingContext,
        IResourceDescription[] descriptions,
        string[] resourceNames,
        IEnumerable<Uri> addresses,
        NamespaceManagerSettings settings,
        AsyncCallback callback,
        object state)
        : this(trackingContext, string.Empty, descriptions, resourceNames, addresses, settings, -1, -1, false, callback, state)
      {
      }

      public GetAllAsyncResult(
        TrackingContext trackingContext,
        string filter,
        IResourceDescription[] descriptions,
        string[] resourceNames,
        IEnumerable<Uri> addresses,
        NamespaceManagerSettings settings,
        AsyncCallback callback,
        object state)
        : this(trackingContext, filter, descriptions, resourceNames, addresses, settings, -1, -1, false, callback, state)
      {
      }

      public GetAllAsyncResult(
        TrackingContext trackingContext,
        string filter,
        IResourceDescription[] descriptions,
        string[] resourceNames,
        IEnumerable<Uri> addresses,
        NamespaceManagerSettings settings,
        int skip,
        int top,
        bool stopAfterTopEntities,
        AsyncCallback callback,
        object state)
        : base(TimeSpan.MaxValue, callback, state)
      {
        this.trackingContext = trackingContext != null ? trackingContext : throw new ArgumentNullException(nameof (trackingContext));
        this.filter = filter;
        this.tokenProvider = settings.TokenProvider;
        this.descriptions = descriptions;
        this.skip = skip >= 0 ? skip : 0;
        this.top = top > 0 ? top : settings.GetEntitiesPageSize;
        TimeSpan operationTimeout = settings.InternalOperationTimeout;
        this.timeoutInMilliseconds = operationTimeout.TotalMilliseconds > (double) int.MaxValue ? int.MaxValue : (int) operationTimeout.TotalMilliseconds;
        this.uriManager = new ServiceBusUriManager(addresses.ToList<Uri>(), true);
        this.resourceNames = resourceNames;
        this.settings = settings;
        this.relatedActivity = EventTraceActivity.CreateFromThread();
        this.stopAfterOnePage = stopAfterTopEntities;
        this.Start();
      }

      public GetAllAsyncResult(
        TrackingContext trackingContext,
        string filter,
        IResourceDescription[] descriptions,
        string[] resourceNames,
        IEnumerable<Uri> addresses,
        NamespaceManagerSettings settings,
        string continuationTokenString,
        int top,
        bool stopAfterTopEntities,
        AsyncCallback callback,
        object state)
        : base(TimeSpan.MaxValue, callback, state)
      {
        this.trackingContext = trackingContext != null ? trackingContext : throw new ArgumentNullException(nameof (trackingContext));
        this.filter = filter;
        this.tokenProvider = settings.TokenProvider;
        this.descriptions = descriptions;
        this.skip = this.skip >= 0 ? this.skip : 0;
        this.top = top > 0 ? top : settings.GetEntitiesPageSize;
        TimeSpan operationTimeout = settings.InternalOperationTimeout;
        this.timeoutInMilliseconds = operationTimeout.TotalMilliseconds > (double) int.MaxValue ? int.MaxValue : (int) operationTimeout.TotalMilliseconds;
        this.uriManager = new ServiceBusUriManager(addresses.ToList<Uri>(), true);
        this.resourceNames = resourceNames;
        this.settings = settings;
        this.relatedActivity = EventTraceActivity.CreateFromThread();
        this.stopAfterOnePage = stopAfterTopEntities;
        this.continuationToken = new ContinuationToken(continuationTokenString);
        this.Start();
      }

      public SyndicationFeed Feed { get; private set; }

      public string NewContinuationToken { get; private set; }

      protected override IEnumerator<IteratorAsyncResult<ServiceBusResourceOperations.GetAllAsyncResult>.AsyncStep> GetAsyncSteps()
      {
        int iteration = 0;
        TimeSpan iterationSleep = this.settings.RetryPolicy.IsServerBusy ? RetryPolicy.ServerBusyBaseSleepTime : TimeSpan.Zero;
        if (this.continuationToken != null && !this.continuationToken.IsValid)
        {
          this.Complete((Exception) new ArgumentException("continuationToken"));
        }
        else
        {
          Uri resourceUri = ServiceBusResourceOperations.CreateCollectionUri<IResourceDescription>(this.uriManager.Current, this.descriptions, this.resourceNames, this.continuationToken, this.skip, this.top, this.filter);
          if (this.settings.RetryPolicy.IsServerBusy && RetryPolicy.ServerBusyBaseSleepTime >= this.settings.InternalOperationTimeout)
          {
            string serverBusyExceptionMessage = this.settings.RetryPolicy.ServerBusyExceptionMessage;
            yield return this.CallAsyncSleep(this.RemainingTime());
            this.Complete((Exception) new ServerBusyException(serverBusyExceptionMessage));
          }
          else
          {
            this.operationTimer = new TimeoutHelper(this.settings.InternalOperationTimeout, true);
            bool shouldRetry;
            do
            {
              shouldRetry = false;
              if (iterationSleep != TimeSpan.Zero)
                yield return this.CallAsyncSleep(iterationSleep);
              this.LastAsyncStepException = (Exception) null;
              this.InitializeRequest(resourceUri);
              this.requestCancelTimer = new IOThreadTimer(new Action<object>(this.CancelRequest), (object) this.request, true);
              try
              {
                TimeSpan timeToCancelRequest = this.operationTimer.RemainingTime();
                if (timeToCancelRequest <= TimeSpan.Zero)
                {
                  this.Complete((Exception) new TimeoutException(SRClient.OperationRequestTimedOut((object) this.OriginalTimeout)));
                  yield break;
                }
                else
                {
                  yield return this.CallAsync((IteratorAsyncResult<ServiceBusResourceOperations.GetAllAsyncResult>.BeginCall) ((thisPtr, t, c, s) =>
                  {
                    IAsyncResult response = thisPtr.request.BeginGetResponse(c, s);
                    thisPtr.requestCancelTimer.SetIfValid(timeToCancelRequest);
                    return response;
                  }), (IteratorAsyncResult<ServiceBusResourceOperations.GetAllAsyncResult>.EndCall) ((thisPtr, r) =>
                  {
                    ServiceBusResourceOperations.GetAllAsyncResult getAllAsyncResult = thisPtr;
                    getAllAsyncResult.response = (HttpWebResponse) getAllAsyncResult.request.EndGetResponse(r);
                  }), IteratorAsyncResult<ServiceBusResourceOperations.GetAllAsyncResult>.ExceptionPolicy.Continue);
                  if (this.LastAsyncStepException != null)
                  {
                    WebException asyncStepException1 = this.LastAsyncStepException as WebException;
                    IOException asyncStepException2 = this.LastAsyncStepException as IOException;
                    if (asyncStepException1 != null)
                      this.LastAsyncStepException = ServiceBusResourceOperations.ConvertWebException(this.trackingContext, asyncStepException1, this.request.Timeout);
                    if (asyncStepException2 != null)
                      this.LastAsyncStepException = ServiceBusResourceOperations.ConvertIOException(this.trackingContext, asyncStepException2, this.request.Timeout, this.isRequestAborted);
                    TimeSpan remainingTime = this.operationTimer.RemainingTime();
                    shouldRetry = !this.TransactionExists && this.settings.RetryPolicy.ShouldRetry(remainingTime, iteration, this.LastAsyncStepException, out iterationSleep);
                    if (shouldRetry)
                    {
                      string operation = string.Format("GetAll:{0}", (object) resourceUri.AbsoluteUri);
                      MessagingClientEtwProvider.TraceClient((Action) (() => MessagingClientEtwProvider.Provider.EventWriteRetryPolicyIteration((EventTraceActivity) null, string.Empty, this.settings.RetryPolicy.GetType().Name, operation, iteration, iterationSleep.ToString(), this.LastAsyncStepException.GetType().FullName, this.LastAsyncStepException.Message)));
                      ++iteration;
                    }
                  }
                  else
                  {
                    Uri nextPageLink = (Uri) null;
                    try
                    {
                      using (this.response)
                      {
                        if (this.response.StatusCode != HttpStatusCode.OK)
                          this.LastAsyncStepException = (Exception) new MessagingException(this.response.StatusDescription, false, (Exception) new WebException(this.response.StatusDescription, (Exception) null, WebExceptionStatus.ProtocolError, (WebResponse) this.response));
                        else
                          this.ProcessResponse(out nextPageLink);
                      }
                      this.response = (HttpWebResponse) null;
                    }
                    catch (XmlException ex)
                    {
                      this.LastAsyncStepException = (Exception) new MessagingException(SRClient.InvalidXmlFormat, false, (Exception) ex);
                    }
                    catch (WebException ex)
                    {
                      this.LastAsyncStepException = ServiceBusResourceOperations.ConvertWebException(this.trackingContext, ex, this.request.Timeout);
                    }
                    catch (IOException ex)
                    {
                      this.LastAsyncStepException = ServiceBusResourceOperations.ConvertIOException(this.trackingContext, ex, this.request.Timeout, this.isRequestAborted);
                    }
                    if (this.LastAsyncStepException != null)
                    {
                      TimeSpan remainingTime = this.operationTimer.RemainingTime();
                      shouldRetry = !this.TransactionExists && this.settings.RetryPolicy.ShouldRetry(remainingTime, iteration, this.LastAsyncStepException, out iterationSleep);
                      if (shouldRetry)
                      {
                        string operation = string.Format("GetAll:{0}", (object) resourceUri.AbsoluteUri);
                        MessagingClientEtwProvider.TraceClient((Action) (() => MessagingClientEtwProvider.Provider.EventWriteRetryPolicyIteration((EventTraceActivity) null, string.Empty, this.settings.RetryPolicy.GetType().Name, operation, iteration, iterationSleep.ToString(), this.LastAsyncStepException.GetType().FullName, this.LastAsyncStepException.Message)));
                        ++iteration;
                      }
                    }
                    else
                    {
                      this.requestCancelTimer.Cancel();
                      this.settings.RetryPolicy.ResetServerBusy();
                      if (!this.stopAfterOnePage && nextPageLink != (Uri) null)
                      {
                        this.operationTimer = new TimeoutHelper(this.settings.InternalOperationTimeout, true);
                        resourceUri = nextPageLink;
                        shouldRetry = true;
                        goto label_36;
                      }
                      else
                        break;
                    }
                  }
                }
              }
              finally
              {
                this.requestCancelTimer.Cancel();
              }
label_36:;
            }
            while (this.uriManager.MoveNextUri() & shouldRetry);
            this.Complete(this.LastAsyncStepException);
          }
        }
      }

      private void CancelRequest(object state)
      {
        this.isRequestAborted = true;
        ((WebRequest) state).Abort();
      }

      private void InitializeRequest(Uri resourceUri)
      {
        this.request = (HttpWebRequest) WebRequest.Create(resourceUri);
        if (ServiceBusEnvironment.Proxy != null)
          this.request.Proxy = ServiceBusEnvironment.Proxy;
        this.request.ServicePoint.MaxIdleTime = Constants.ServicePointMaxIdleTimeMilliSeconds;
        this.request.Method = "GET";
        this.request.ContentType = "application/atom+xml;type=entry;charset=utf-8";
        this.request.Timeout = this.timeoutInMilliseconds;
        this.request.SetUserAgentHeader();
        this.request.AddXProcessAtHeader();
        this.request.AddAuthorizationHeader(this.tokenProvider, this.uriManager.Current, "Manage");
        this.request.AddTrackingIdHeader(this.trackingContext);
        this.request.AddCorrelationHeader(this.relatedActivity);
      }

      private void ProcessResponse(out Uri nextPageLink)
      {
        nextPageLink = (Uri) null;
        Stream responseStream = this.response.GetResponseStream();
        using (XmlReader reader = XmlReader.Create(responseStream, new XmlReaderSettings()
        {
          CloseInput = true
        }))
        {
          SyndicationFeed syndicationFeed = SyndicationFeed.Load<SyndicationFeed>(reader);
          if (syndicationFeed != null && syndicationFeed.Links != null)
          {
            SyndicationLink syndicationLink = syndicationFeed.Links.FirstOrDefault<SyndicationLink>(ServiceBusResourceOperations.GetAllAsyncResult.nextLinkPredicate);
            nextPageLink = syndicationLink == null ? (Uri) null : syndicationLink.Uri;
          }
          if (this.Feed == null)
          {
            this.Feed = syndicationFeed;
          }
          else
          {
            SyndicationFeed feed = this.Feed;
            if (syndicationFeed != null)
            {
              if (syndicationFeed.Items != null)
                this.Feed = new SyndicationFeed(feed.Items.Union<SyndicationItem>(syndicationFeed.Items));
            }
          }
        }
        this.NewContinuationToken = this.response.Headers["x-ms-continuationtoken"];
      }
    }

    private sealed class CreateRegistrationIdAsyncResult : 
      RetryAsyncResult<ServiceBusResourceOperations.CreateRegistrationIdAsyncResult>
    {
      private static UriTemplate NotificationHubRegistrationIdsUriTemplate = new UriTemplate("registrationids/{registration}", true);
      private readonly TrackingContext trackingContext;
      private readonly EventTraceActivity relatedActivity;
      private readonly NamespaceManagerSettings settings;
      private readonly TokenProvider tokenProvider;
      private readonly bool isAnonymousAccessible;
      private readonly ServiceBusUriManager uriManager;
      private readonly IDictionary<string, string> queryParametersAndValues;
      private HttpWebRequest request;
      private Uri currentResourceUri;
      private HttpWebResponse response;
      private IResourceDescription[] collectionDescriptions;
      private string[] resourceNames;
      private IOThreadTimer requestCancelTimer;
      private volatile bool isRequestAborted;

      public CreateRegistrationIdAsyncResult(
        TrackingContext trackingContext,
        IResourceDescription[] collectionDescriptions,
        string[] resourceNames,
        IEnumerable<Uri> baseAddresses,
        TimeSpan timeout,
        bool isAnonymousAccessible,
        bool isUpdate,
        IDictionary<string, string> queryParametersAndValues,
        NamespaceManagerSettings settings,
        AsyncCallback callback,
        object state)
        : base(timeout, callback, state)
      {
        this.trackingContext = trackingContext != null ? trackingContext : throw new ArgumentNullException(nameof (trackingContext));
        this.settings = settings;
        this.tokenProvider = settings.TokenProvider;
        this.collectionDescriptions = collectionDescriptions;
        this.isAnonymousAccessible = isAnonymousAccessible;
        this.resourceNames = resourceNames;
        this.uriManager = new ServiceBusUriManager(baseAddresses.ToList<Uri>());
        this.relatedActivity = EventTraceActivity.CreateFromThread();
        this.queryParametersAndValues = queryParametersAndValues;
        this.Start();
      }

      public string Result { get; private set; }

      protected override IEnumerator<IteratorAsyncResult<ServiceBusResourceOperations.CreateRegistrationIdAsyncResult>.AsyncStep> GetAsyncSteps()
      {
        while (this.uriManager.MoveNextUri())
        {
          this.LastAsyncStepException = (Exception) null;
          Uri notificationHubUri = ServiceBusResourceOperations.CreateResourceUri(this.uriManager.Current, this.collectionDescriptions, this.resourceNames, this.queryParametersAndValues);
          UriBuilder uriBuilder1 = new UriBuilder(notificationHubUri);
          UriBuilder uriBuilder2 = uriBuilder1;
          uriBuilder2.Path = uriBuilder2.Path.TrimEnd('/');
          uriBuilder1.Path = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", new object[2]
          {
            (object) uriBuilder1.Path,
            (object) "registrationids"
          });
          this.currentResourceUri = uriBuilder1.Uri;
          this.request = (HttpWebRequest) WebRequest.Create(this.currentResourceUri);
          if (ServiceBusEnvironment.Proxy != null)
            this.request.Proxy = ServiceBusEnvironment.Proxy;
          this.request.Headers.Add("X-MS-ISANONYMOUSACCESSIBLE", this.isAnonymousAccessible.ToString());
          this.request.Method = "POST";
          this.request.ServicePoint.MaxIdleTime = Constants.ServicePointMaxIdleTimeMilliSeconds;
          this.request.ContentType = "application/atom+xml;type=entry;charset=utf-8";
          this.request.Timeout = this.OriginalTimeout.TotalMilliseconds > (double) int.MaxValue ? int.MaxValue : (int) this.OriginalTimeout.TotalMilliseconds;
          this.request.ContentLength = 0L;
          this.request.SetUserAgentHeader();
          this.request.AddXProcessAtHeader();
          this.request.AddAuthorizationHeader(this.tokenProvider, this.uriManager.Current, "Manage");
          this.request.AddTrackingIdHeader(this.trackingContext);
          this.request.AddCorrelationHeader(this.relatedActivity);
          this.requestCancelTimer = new IOThreadTimer(new Action<object>(this.CancelRequest), (object) this.request, true);
          bool asyncSteps;
          try
          {
            TimeSpan timeToCancelRequest = this.RemainingTime();
            if (timeToCancelRequest <= TimeSpan.Zero)
            {
              this.Complete((Exception) new TimeoutException(SRClient.OperationRequestTimedOut((object) this.OriginalTimeout)));
              asyncSteps = false;
            }
            else
            {
              yield return this.CallAsync((IteratorAsyncResult<ServiceBusResourceOperations.CreateRegistrationIdAsyncResult>.BeginCall) ((thisPtr, t, c, s) =>
              {
                IAsyncResult response = thisPtr.request.BeginGetResponse(c, s);
                thisPtr.requestCancelTimer.SetIfValid(timeToCancelRequest);
                return response;
              }), (IteratorAsyncResult<ServiceBusResourceOperations.CreateRegistrationIdAsyncResult>.EndCall) ((thisPtr, r) =>
              {
                ServiceBusResourceOperations.CreateRegistrationIdAsyncResult registrationIdAsyncResult = thisPtr;
                registrationIdAsyncResult.response = (HttpWebResponse) registrationIdAsyncResult.request.EndGetResponse(r);
              }), IteratorAsyncResult<ServiceBusResourceOperations.CreateRegistrationIdAsyncResult>.ExceptionPolicy.Continue);
              if (this.LastAsyncStepException != null)
              {
                if (!this.uriManager.CanRetry() || !ServiceBusResourceOperations.IsRetriableException(this.LastAsyncStepException))
                {
                  Exception operationException = this.LastAsyncStepException;
                  if (operationException is WebException webException)
                    operationException = ServiceBusResourceOperations.ConvertWebException(this.trackingContext, webException, this.request.Timeout);
                  if (operationException is IOException ioException)
                    operationException = ServiceBusResourceOperations.ConvertIOException(this.trackingContext, ioException, this.request.Timeout, this.isRequestAborted);
                  this.Complete(operationException);
                  asyncSteps = false;
                  goto label_28;
                }
              }
              else
              {
                try
                {
                  using (this.response)
                  {
                    if (this.response.StatusCode != HttpStatusCode.Created && this.response.StatusCode != HttpStatusCode.OK)
                    {
                      if (!this.uriManager.CanRetry())
                      {
                        this.Complete(ServiceBusResourceOperations.ConvertWebException(this.trackingContext, new WebException(this.response.StatusDescription, (Exception) null, WebExceptionStatus.ProtocolError, (WebResponse) this.response), this.request.Timeout));
                        asyncSteps = false;
                        goto label_28;
                      }
                      else
                        goto label_29;
                    }
                    else
                    {
                      Uri candidate = new Uri(this.response.Headers.Get("Location"));
                      this.Result = ServiceBusResourceOperations.CreateRegistrationIdAsyncResult.NotificationHubRegistrationIdsUriTemplate.Match(notificationHubUri, candidate).BoundVariables["registration"];
                    }
                  }
                }
                catch (WebException ex)
                {
                  if (this.uriManager.CanRetry())
                  {
                    if (ServiceBusResourceOperations.IsRetriableException((Exception) ex))
                      goto label_29;
                  }
                  this.Complete(ServiceBusResourceOperations.ConvertWebException(this.trackingContext, ex, this.request.Timeout));
                  asyncSteps = false;
                  goto label_28;
                }
                catch (IOException ex)
                {
                  this.Complete(ServiceBusResourceOperations.ConvertIOException(this.trackingContext, ex, this.request.Timeout, this.isRequestAborted));
                }
                break;
              }
label_29:
              continue;
            }
label_28:;
          }
          finally
          {
            this.requestCancelTimer.Cancel();
          }
          return asyncSteps;
        }
      }

      private void CancelRequest(object state)
      {
        this.isRequestAborted = true;
        ((WebRequest) state).Abort();
      }
    }
  }
}
