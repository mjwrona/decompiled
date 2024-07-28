// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.TargetedNotificationsProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class TargetedNotificationsProvider : TargetedNotificationsProviderBase
  {
    internal const string DefaultUrl = "https://go.microsoft.com/fwlink/?LinkId=690387";
    private const string DefaultContentType = "application/json";
    private const int DefaultRequestTimeout = 10000;
    private readonly IHttpWebRequestFactory webRequestFactory;
    private readonly ITargetedNotificationsParser notificationsParser;
    private readonly RemoteSettingsFilterProvider remoteSettingsFilterProvider;

    public TargetedNotificationsProvider(RemoteSettingsInitializer initializer)
      : base(initializer.CacheableRemoteSettingsStorageHandler, initializer)
    {
      this.webRequestFactory = initializer.HttpWebRequestFactory;
      this.notificationsParser = initializer.TargetedNotificationsParser;
      this.remoteSettingsFilterProvider = initializer.FilterProvider;
    }

    public override string Name => "TargetedNotifications";

    protected override async Task<ActionResponseBag> GetTargetedNotificationActionsAsync()
    {
      TargetedNotificationsProvider notificationsProvider = this;
      using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(notificationsProvider.cancellationTokenSource.Token))
      {
        cts.CancelAfter(10000);
        IEnumerable<string> previouslyCachedRuleIds = notificationsProvider.useCache ? notificationsProvider.notificationAndCourtesyCache.GetAllCachedRuleIds(new int?(notificationsProvider.cacheTimeoutMs)) : Enumerable.Empty<string>();
        Stream stream = await notificationsProvider.SendTargetedNotificationsRequestAsync(previouslyCachedRuleIds, cts.Token).ConfigureAwait(false);
        if (stream != null)
        {
          try
          {
            ActionResponseBag newResponse = await notificationsProvider.notificationsParser.ParseStreamAsync(stream, cts.Token).ConfigureAwait(false);
            if (notificationsProvider.useCache)
              notificationsProvider.notificationAndCourtesyCache.MergeNewResponse(newResponse, previouslyCachedRuleIds, new int?(notificationsProvider.cacheTimeoutMs));
            return newResponse;
          }
          catch (TargetedNotificationsException ex)
          {
            string eventName = "VS/Core/TargetedNotifications/ApiResponseParseFailure";
            Dictionary<string, object> additionalProperties = new Dictionary<string, object>()
            {
              {
                "VS.Core.TargetedNotifications.ApiResponseMs",
                (object) notificationsProvider.apiTimer?.ElapsedMilliseconds
              },
              {
                "VS.Core.TargetedNotifications.Iteration",
                (object) notificationsProvider.queryIteration
              }
            };
            notificationsProvider.targetedNotificationsTelemetry.PostDiagnosticFault(eventName, "Failed to parse TN API response", (Exception) ex, additionalProperties);
          }
        }
        return (ActionResponseBag) null;
      }
    }

    private async Task<string> ResolveUrlRedirectAsync(string url, CancellationToken token)
    {
      IHttpWebRequest httpWebRequest = this.webRequestFactory.Create(url);
      httpWebRequest.Method = "HEAD";
      httpWebRequest.AllowAutoRedirect = false;
      IHttpWebResponse httpWebResponse = await httpWebRequest.GetResponseAsync(token).ConfigureAwait(false);
      if (httpWebResponse.ErrorCode != ErrorCode.NoError || httpWebResponse.Headers == null)
        throw new TargetedNotificationsException(string.Format("Could not resolve url redirect ({0}-{1}-{2})", (object) httpWebResponse.ErrorCode, (object) httpWebResponse.StatusCode, (object) httpWebResponse.ExceptionCode));
      return httpWebResponse.Headers["Location"] ?? throw new TargetedNotificationsException("Could not resolve url redirect (no location header was received)");
    }

    private async Task<Stream> SendTargetedNotificationsRequestAsync(
      IEnumerable<string> previouslyCachedRuleIds,
      CancellationToken cancelToken)
    {
      TargetedNotificationsProvider notificationsProvider = this;
      Stream stream = (Stream) null;
      notificationsProvider.apiTimer = Stopwatch.StartNew();
      try
      {
        string parameters = await notificationsProvider.BuildRequestParametersAsync(previouslyCachedRuleIds, cancelToken).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(parameters))
          throw new TargetedNotificationsException("Invalid parameters configuration");
        IHttpWebRequestFactory webRequestFactory = notificationsProvider.webRequestFactory;
        IHttpWebRequest request = webRequestFactory.Create(await notificationsProvider.ResolveUrlRedirectAsync("https://go.microsoft.com/fwlink/?LinkId=690387", cancelToken).ConfigureAwait(false));
        webRequestFactory = (IHttpWebRequestFactory) null;
        request.ContentType = "application/json";
        request.Method = "POST";
        notificationsProvider.logger.LogVerbose("Sending request to TN backend", (object) parameters);
        byte[] postData = Encoding.UTF8.GetBytes(parameters);
        request.ContentLength = (long) postData.Length;
        Stream postStream = await request.GetRequestStreamAsync(cancelToken).ConfigureAwait(false);
        await postStream.WriteAsync(postData, 0, postData.Length, cancelToken).ConfigureAwait(false);
        postStream.Close();
        IHttpWebResponse httpWebResponse = await request.GetResponseAsync(cancelToken).ConfigureAwait(false);
        notificationsProvider.apiTimer.Stop();
        if (httpWebResponse.ErrorCode == ErrorCode.NoError)
        {
          stream = httpWebResponse.GetResponseStream();
        }
        else
        {
          string eventName = "VS/Core/TargetedNotifications/ApiRequestFailed";
          string description = "Request failed.";
          Dictionary<string, object> additionalProperties = new Dictionary<string, object>()
          {
            {
              "VS.Core.TargetedNotifications.ErrorCode",
              (object) httpWebResponse.ErrorCode
            },
            {
              "VS.Core.TargetedNotifications.ApiResponseMs",
              (object) notificationsProvider.apiTimer?.ElapsedMilliseconds
            },
            {
              "VS.Core.TargetedNotifications.Iteration",
              (object) notificationsProvider.queryIteration
            },
            {
              "VS.Core.TargetedNotifications.ShutdownRequested",
              (object) notificationsProvider.cancellationTokenSource.IsCancellationRequested
            },
            {
              "VS.Core.TargetedNotifications.TimeoutRequested",
              (object) (bool) (notificationsProvider.cancellationTokenSource.IsCancellationRequested ? 0 : (cancelToken.IsCancellationRequested ? 1 : 0))
            }
          };
          if (httpWebResponse.ErrorCode == ErrorCode.WebExceptionThrown)
          {
            additionalProperties.Add("VS.Core.TargetedNotifications.WebExceptionCode", (object) httpWebResponse.ExceptionCode);
            additionalProperties.Add("VS.Core.TargetedNotifications.StatusCode", (object) httpWebResponse.StatusCode);
          }
          notificationsProvider.targetedNotificationsTelemetry.PostDiagnosticFault(eventName, description, additionalProperties: additionalProperties);
        }
        parameters = (string) null;
        request = (IHttpWebRequest) null;
        postData = (byte[]) null;
        postStream = (Stream) null;
      }
      catch (Exception ex)
      {
        notificationsProvider.apiTimer.Stop();
        string eventName = "VS/Core/TargetedNotifications/ApiRequestException";
        string str = "Sending request to TN backend failed";
        Dictionary<string, object> additionalProperties = new Dictionary<string, object>()
        {
          {
            "VS.Core.TargetedNotifications.ApiResponseMs",
            (object) notificationsProvider.apiTimer?.ElapsedMilliseconds
          },
          {
            "VS.Core.TargetedNotifications.Iteration",
            (object) notificationsProvider.queryIteration
          },
          {
            "VS.Core.TargetedNotifications.ShutdownRequested",
            (object) notificationsProvider.cancellationTokenSource.IsCancellationRequested
          },
          {
            "VS.Core.TargetedNotifications.TimeoutRequested",
            (object) (bool) (notificationsProvider.cancellationTokenSource.IsCancellationRequested ? 0 : (cancelToken.IsCancellationRequested ? 1 : 0))
          }
        };
        notificationsProvider.logger.LogError(str, ex);
        notificationsProvider.targetedNotificationsTelemetry.PostDiagnosticFault(eventName, str, ex, additionalProperties);
      }
      Stream stream1 = stream;
      stream = (Stream) null;
      return stream1;
    }

    private async Task<string> BuildRequestParametersAsync(
      IEnumerable<string> previouslyCachedRuleIds,
      CancellationToken cancellationToken)
    {
      TargetedNotificationsProvider notificationsProvider = this;
      string channelId = notificationsProvider.remoteSettingsFilterProvider.GetChannelId();
      if (string.IsNullOrWhiteSpace(channelId))
        return string.Empty;
      ActionRequestParameters requestParameters1 = new ActionRequestParameters();
      requestParameters1.MachineId = TargetedNotificationsProvider.HandleGuidParameter(notificationsProvider.remoteSettingsFilterProvider.GetMachineId());
      requestParameters1.UserId = TargetedNotificationsProvider.HandleGuidParameter(notificationsProvider.remoteSettingsFilterProvider.GetUserId());
      ActionRequestParameters requestParameters2 = requestParameters1;
      requestParameters2.VsoId = await notificationsProvider.remoteSettingsFilterProvider.GetVsIdAsync().WithCancellation<string>(cancellationToken).ConfigureAwait(false);
      requestParameters1.Culture = notificationsProvider.remoteSettingsFilterProvider.GetCulture();
      requestParameters1.Version = notificationsProvider.remoteSettingsFilterProvider.GetApplicationVersion();
      requestParameters1.VsSku = notificationsProvider.remoteSettingsFilterProvider.GetVsSku();
      requestParameters1.NotificationsCount = notificationsProvider.remoteSettingsFilterProvider.GetNotificationsCount();
      requestParameters1.AppIdPackage = TargetedNotificationsProvider.HandleGuidParameter(notificationsProvider.remoteSettingsFilterProvider.GetAppIdPackageGuid());
      requestParameters1.MacAddressHash = notificationsProvider.remoteSettingsFilterProvider.GetMacAddressHash();
      requestParameters1.ChannelId = channelId;
      requestParameters1.ChannelManifestId = notificationsProvider.remoteSettingsFilterProvider.GetChannelManifestId();
      requestParameters1.ManifestId = notificationsProvider.remoteSettingsFilterProvider.GetManifestId();
      requestParameters1.OsType = notificationsProvider.remoteSettingsFilterProvider.GetOsType();
      requestParameters1.OsVersion = notificationsProvider.remoteSettingsFilterProvider.GetOsVersion();
      requestParameters1.ExeName = notificationsProvider.remoteSettingsFilterProvider.GetApplicationName();
      requestParameters1.IsInternal = TargetedNotificationsProvider.HandleBoolParameter(notificationsProvider.remoteSettingsFilterProvider.GetIsUserInternal());
      requestParameters1.CachedRuleIds = previouslyCachedRuleIds;
      requestParameters1.SessionId = notificationsProvider.targetedNotificationsTelemetry.SessionId;
      requestParameters1.SessionRole = notificationsProvider.remoteSettingsFilterProvider.GetSessionRole();
      requestParameters1.ClrVersion = notificationsProvider.remoteSettingsFilterProvider.GetClrVersion();
      requestParameters1.ProcessArchitecture = notificationsProvider.remoteSettingsFilterProvider.GetProcessArchitecture();
      requestParameters1.ClientSourceType = notificationsProvider.remoteSettingsFilterProvider.GetClientSourceType();
      ActionRequestParameters requestParameters = requestParameters1;
      requestParameters2 = (ActionRequestParameters) null;
      requestParameters1 = (ActionRequestParameters) null;
      return JsonConvert.SerializeObject((object) requestParameters);
    }

    private static string HandleGuidParameter(Guid guid) => guid == new Guid() ? string.Empty : guid.ToString();

    private static int HandleBoolParameter(bool value) => value ? 1 : 0;
  }
}
