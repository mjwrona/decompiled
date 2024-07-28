// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Utility.UserAccessTokenRefresher
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Logging;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client.Utility
{
  public sealed class UserAccessTokenRefresher
  {
    public const string BearerTokenAuthScheme = "Bearer";
    private const string ClientId = "e64181a2-56a9-4fac-b4c7-efa5252b4d32";
    private const string ClientRedirectUri = "https://GenevaMDMClient";
    private static readonly string[] Scopes = new string[1]
    {
      "https://microsoftmetrics.com/user_impersonation"
    };
    private static readonly object LogId = Microsoft.Cloud.Metrics.Client.Logging.Logger.CreateCustomLogId(nameof (UserAccessTokenRefresher));
    private static Lazy<UserAccessTokenRefresher> instance = new Lazy<UserAccessTokenRefresher>((Func<UserAccessTokenRefresher>) (() => new UserAccessTokenRefresher()));
    private static Func<DeviceCodeResult, Task> deviceCodeResultCallback = (Func<DeviceCodeResult, Task>) (codeResult =>
    {
      Console.WriteLine(codeResult.Message);
      return (Task) Task.FromResult<int>(0);
    });
    private readonly SemaphoreSlim accessTokenRefreshLock = new SemaphoreSlim(1);
    private string userAccessToken;
    private DateTime lastAccessTokenRefreshTime;

    public static string UserPrincipalName { get; set; }

    public static Uri Authority { get; set; } = new Uri("https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47");

    public static Func<DeviceCodeResult, Task> DeviceCodeResultCallback
    {
      get => UserAccessTokenRefresher.deviceCodeResultCallback;
      set => UserAccessTokenRefresher.deviceCodeResultCallback = value;
    }

    internal static UserAccessTokenRefresher Instance => UserAccessTokenRefresher.instance.Value;

    internal string UserAccessToken
    {
      get
      {
        if (this.userAccessToken == null)
        {
          this.RefreshAccessToken().Wait();
          if (this.userAccessToken == null)
            throw new MetricsClientException("Failed to obtain an AAD user access token.");
        }
        return this.userAccessToken;
      }
    }

    internal async Task RefreshAccessToken()
    {
      this.accessTokenRefreshLock.WaitAsync();
      try
      {
        if (!(this.lastAccessTokenRefreshTime < DateTime.UtcNow.AddMinutes(-10.0)))
          return;
        this.userAccessToken = await UserAccessTokenRefresher.AcquireAccessToken().ConfigureAwait(false);
        this.lastAccessTokenRefreshTime = DateTime.UtcNow;
        Microsoft.Cloud.Metrics.Client.Logging.Logger.Log(LoggerLevel.Info, UserAccessTokenRefresher.LogId, nameof (RefreshAccessToken), "Succeeded");
      }
      catch (Exception ex)
      {
        Microsoft.Cloud.Metrics.Client.Logging.Logger.Log(LoggerLevel.Error, UserAccessTokenRefresher.LogId, nameof (RefreshAccessToken), ex.ToString());
        throw;
      }
      finally
      {
        this.accessTokenRefreshLock.Release();
      }
    }

    private static async Task<string> AcquireAccessToken()
    {
      IPublicClientApplication app = PublicClientApplicationBuilder.Create("e64181a2-56a9-4fac-b4c7-efa5252b4d32").WithAuthority(UserAccessTokenRefresher.Authority).WithRedirectUri("https://GenevaMDMClient").Build();
      try
      {
        return (await app.AcquireTokenByIntegratedWindowsAuth((IEnumerable<string>) UserAccessTokenRefresher.Scopes).WithUsername(UserAccessTokenRefresher.UserPrincipalName).ExecuteAsync(CancellationToken.None).ConfigureAwait(false)).AccessToken;
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case MsalException _:
          case PlatformNotSupportedException _:
            Microsoft.Cloud.Metrics.Client.Logging.Logger.Log(LoggerLevel.Info, UserAccessTokenRefresher.LogId, "AcquireTokenByIntegratedWindowsAuth - Failed to acquire an AAD token", ex.ToString());
            break;
          default:
            throw;
        }
      }
      try
      {
        return (await app.AcquireTokenInteractive((IEnumerable<string>) UserAccessTokenRefresher.Scopes).ExecuteAsync().ConfigureAwait(false)).AccessToken;
      }
      catch (MsalException ex)
      {
        throw new MetricsClientException("AcquireTokenWithDeviceCode/AcquireTokenInteractive - Failed to acquire an AAD token", (Exception) ex);
      }
    }
  }
}
