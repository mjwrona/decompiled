// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.ConnectionInfo
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Logging;
using Microsoft.Cloud.Metrics.Client.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client
{
  public sealed class ConnectionInfo
  {
    public const string CertApiFirstSegment = "/api/";
    public const string UserApiFirstSegment = "/user-api/";
    internal static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(100.0);
    private const int NumberOfMdmGlobalEnvironments = 6;
    private static readonly TimeSpan MaxTimeout = TimeSpan.FromSeconds(300.0);
    private static readonly object LogId = Logger.CreateCustomLogId(nameof (ConnectionInfo));
    private static readonly ConcurrentDictionary<string, ConnectionInfo.StampInformation>[] AccountToUriMaps = new ConcurrentDictionary<string, ConnectionInfo.StampInformation>[6];
    private static readonly ConcurrentDictionary<string, Uri>[] GslbToUris = new ConcurrentDictionary<string, Uri>[6];
    private static readonly ConcurrentDictionary<string, string> HostToIpAddressMap = new ConcurrentDictionary<string, string>();
    private static readonly object GlobalEnvironmentInitializationLock = new object();
    private static readonly System.Threading.Timer TimerToRefreshHomeStamp;
    private static readonly System.Threading.Timer TimerToRefreshIpAddress;
    private static readonly HttpClient HttpClientWithoutAuthentication = HttpClientHelper.CreateHttpClient(ConnectionInfo.DefaultTimeout);
    private static readonly SemaphoreSlim[] Semaphores;
    private static volatile string[] globalEnvironments;
    private readonly int mdmEnvironmentMapIndex;
    private X509Certificate2 certificate;
    private volatile bool isCertificateValidated;

    static ConnectionInfo()
    {
      ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Ssl3;
      ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
      for (int index = 0; index < 6; ++index)
      {
        ConnectionInfo.GslbToUris[index] = new ConcurrentDictionary<string, Uri>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        ConnectionInfo.AccountToUriMaps[index] = new ConcurrentDictionary<string, ConnectionInfo.StampInformation>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }
      ConnectionInfo.Semaphores = new SemaphoreSlim[100];
      for (int index = 0; index < ConnectionInfo.Semaphores.Length; ++index)
        ConnectionInfo.Semaphores[index] = new SemaphoreSlim(1);
      ConnectionInfo.TimerToRefreshIpAddress = new System.Threading.Timer((TimerCallback) (state => ConnectionInfo.RefreshIpAddresses()), (object) null, ConnectionInfo.DnsResolutionUpdateFrequency, System.Threading.Timeout.InfiniteTimeSpan);
      ConnectionInfo.TimerToRefreshHomeStamp = new System.Threading.Timer((TimerCallback) (state => ConnectionInfo.RefreshAccountHomeStamp()), (object) null, ConnectionInfo.HomeStampAutomaticUpdateFrequency, System.Threading.Timeout.InfiniteTimeSpan);
    }

    public ConnectionInfo(Uri endpoint, string certificateThumbprintOrSubjectName)
      : this(endpoint, certificateThumbprintOrSubjectName, StoreLocation.LocalMachine)
    {
    }

    public ConnectionInfo(string certificateThumbprintOrSubjectName, MdmEnvironment mdmEnvironment = MdmEnvironment.Production)
      : this((Uri) null, certificateThumbprintOrSubjectName, StoreLocation.LocalMachine, (X509Certificate2) null, ConnectionInfo.DefaultTimeout, mdmEnvironment)
    {
    }

    public ConnectionInfo(Uri endpoint, X509Certificate2 certificate)
      : this(endpoint, (string) null, StoreLocation.LocalMachine, certificate, ConnectionInfo.DefaultTimeout)
    {
    }

    public ConnectionInfo(Uri endpoint, X509Certificate2 certificate, TimeSpan timeout)
      : this(endpoint, (string) null, StoreLocation.LocalMachine, certificate, timeout)
    {
    }

    public ConnectionInfo(Uri endpoint)
      : this(endpoint, (string) null, StoreLocation.LocalMachine, (X509Certificate2) null, ConnectionInfo.DefaultTimeout)
    {
    }

    public ConnectionInfo(TimeSpan timeout, Uri endpoint)
      : this(endpoint, (string) null, StoreLocation.LocalMachine, (X509Certificate2) null, timeout)
    {
    }

    public ConnectionInfo(X509Certificate2 certificate, MdmEnvironment mdmEnvironment = MdmEnvironment.Production)
      : this((Uri) null, (string) null, StoreLocation.LocalMachine, certificate, ConnectionInfo.DefaultTimeout, mdmEnvironment)
    {
    }

    public ConnectionInfo(MdmEnvironment mdmEnvironment = MdmEnvironment.Production)
      : this((Uri) null, (string) null, StoreLocation.LocalMachine, (X509Certificate2) null, ConnectionInfo.DefaultTimeout, mdmEnvironment)
    {
    }

    public ConnectionInfo(TimeSpan timeout, MdmEnvironment mdmEnvironment = MdmEnvironment.Production)
      : this((Uri) null, (string) null, StoreLocation.LocalMachine, (X509Certificate2) null, timeout, mdmEnvironment)
    {
    }

    public ConnectionInfo(
      Uri endpoint,
      string certificateThumbprintOrSubjectName,
      StoreLocation certificateStoreLocation)
      : this(endpoint, certificateThumbprintOrSubjectName, certificateStoreLocation, ConnectionInfo.DefaultTimeout)
    {
    }

    public ConnectionInfo(
      string certificateThumbprintOrSubjectName,
      StoreLocation certificateStoreLocation,
      MdmEnvironment mdmEnvironment = MdmEnvironment.Production)
      : this((Uri) null, certificateThumbprintOrSubjectName, certificateStoreLocation, (X509Certificate2) null, ConnectionInfo.DefaultTimeout, mdmEnvironment)
    {
    }

    public ConnectionInfo(
      Uri endpoint,
      string certificateThumbprintOrSubjectName,
      StoreLocation certificateStoreLocation,
      TimeSpan timeout)
      : this(endpoint, certificateThumbprintOrSubjectName, certificateStoreLocation, (X509Certificate2) null, timeout)
    {
    }

    public ConnectionInfo(
      string certificateThumbprintOrSubjectName,
      StoreLocation certificateStoreLocation,
      TimeSpan timeout,
      MdmEnvironment mdmEnvironment = MdmEnvironment.Production)
      : this((Uri) null, certificateThumbprintOrSubjectName, certificateStoreLocation, (X509Certificate2) null, timeout, mdmEnvironment)
    {
    }

    public ConnectionInfo(
      X509Certificate2 certificate,
      TimeSpan timeout,
      MdmEnvironment mdmEnvironment = MdmEnvironment.Production)
      : this((Uri) null, (string) null, StoreLocation.LocalMachine, certificate, timeout, mdmEnvironment)
    {
    }

    private ConnectionInfo(
      Uri endpoint,
      string certificateThumbprintOrSubjectName,
      StoreLocation certificateStoreLocation,
      X509Certificate2 certificate,
      TimeSpan timeout)
      : this(endpoint, certificateThumbprintOrSubjectName, certificateStoreLocation, certificate, timeout, MdmEnvironment.Production)
    {
      if (endpoint == (Uri) null)
        throw new ArgumentNullException(nameof (endpoint));
    }

    private ConnectionInfo(
      Uri endpoint,
      string certificateThumbprintOrSubjectName,
      StoreLocation certificateStoreLocation,
      X509Certificate2 certificate,
      TimeSpan timeout,
      MdmEnvironment mdmEnvironment)
    {
      if (certificate != null && !string.IsNullOrWhiteSpace(certificateThumbprintOrSubjectName))
        throw new ArgumentException("Either certificate or certificateThumbprintOrSubjectName can be specified, but not both.");
      if (timeout > ConnectionInfo.MaxTimeout)
        throw new ArgumentException(string.Format("The timeout value for a request must be less than {0}.", (object) ConnectionInfo.MaxTimeout));
      if (certificate == null && string.IsNullOrWhiteSpace(certificateThumbprintOrSubjectName))
        this.UseAadUserAuthentication = true;
      Logger.Log(LoggerLevel.Info, ConnectionInfo.LogId, "Created", "A new connection was created. Endpoint:{0}, CertThumbprint:{1}, CertStore:{2}, TimeoutMs:{3}", (object) endpoint, certificate != null ? (object) certificate.Thumbprint : (object) certificateThumbprintOrSubjectName, (object) certificateStoreLocation, (object) timeout.TotalMilliseconds);
      this.Endpoint = endpoint;
      this.CertificateThumbprintOrSubjectName = certificateThumbprintOrSubjectName;
      this.CertificateStoreLocation = certificateStoreLocation;
      this.Certificate = certificate;
      this.Timeout = timeout;
      this.mdmEnvironmentMapIndex = (int) mdmEnvironment;
    }

    public static TimeSpan HomeStampAutomaticUpdateFrequency { get; set; } = TimeSpan.FromMinutes(10.0);

    public static TimeSpan DnsResolutionUpdateFrequency { get; set; } = TimeSpan.FromSeconds(4.0);

    public static bool DisableDnsResolution { get; set; }

    public bool UseAadUserAuthentication { get; }

    public bool IsGlobalEndpoint => !(this.Endpoint == (Uri) null) && this.Endpoint.Host.StartsWith("global", StringComparison.OrdinalIgnoreCase);

    public Dictionary<string, string> AdditionalDefaultRequestHeaders { get; set; }

    public Uri Endpoint { get; }

    public string CertificateThumbprint => this.Certificate?.Thumbprint;

    public X509Certificate2 Certificate
    {
      get
      {
        if (this.certificate == null)
          this.certificate = CertificateHelper.FindX509Certificate(this.CertificateThumbprintOrSubjectName, this.CertificateStoreLocation);
        if (!this.isCertificateValidated)
        {
          CertificateHelper.ValidateCertificate(this.certificate);
          this.isCertificateValidated = true;
        }
        return this.certificate;
      }
      private set => this.certificate = value;
    }

    public string CertificateThumbprintOrSubjectName { get; private set; }

    public MdmEnvironment MdmEnvironment
    {
      get
      {
        if (this.Endpoint != (Uri) null)
          throw new InvalidOperationException(string.Format("Endpoint was specified during construction of instance, this instance operates only against that given endpoint {0}.", (object) this.Endpoint));
        return (MdmEnvironment) this.mdmEnvironmentMapIndex;
      }
    }

    public StoreLocation CertificateStoreLocation { get; private set; }

    public TimeSpan Timeout { get; private set; }

    public Uri GetEndpoint(string monitoringAccount) => this.GetEndpointAsync(monitoringAccount).GetAwaiter().GetResult();

    public async Task<Uri> GetEndpointAsync(string monitoringAccount) => this.Endpoint != (Uri) null ? this.Endpoint : (await this.GetAndUpdateIfRequiredStampInformation(monitoringAccount).ConfigureAwait(false)).StampMainUri;

    public async Task<Uri> GetMetricsDataQueryEndpoint(string monitoringAccount) => this.Endpoint != (Uri) null ? this.Endpoint : (await this.GetAndUpdateIfRequiredStampInformation(monitoringAccount).ConfigureAwait(false)).StampQueryUri;

    public Uri GetGlobalEndpoint() => new Uri(ConnectionInfo.ResolveGlobalEnvironments()[this.mdmEnvironmentMapIndex]);

    internal static string[] ResolveGlobalEnvironments()
    {
      if (ConnectionInfo.globalEnvironments != null)
      {
        if (!ConnectionInfo.DisableDnsResolution && !ConnectionInfo.globalEnvironments[0].EndsWith(ProductionGlobalEnvironmentResolver.PotentialProductionGlobalEnvironments[MdmEnvironment.Production], StringComparison.OrdinalIgnoreCase))
          ConnectionInfo.globalEnvironments[0] = "https://" + ProductionGlobalEnvironmentResolver.ResolveGlobalStampHostName();
        return ConnectionInfo.globalEnvironments;
      }
      lock (ConnectionInfo.GlobalEnvironmentInitializationLock)
      {
        ConnectionInfo.globalEnvironments = new string[6]
        {
          ConnectionInfo.DisableDnsResolution ? "https://" + ProductionGlobalEnvironmentResolver.PotentialProductionGlobalEnvironments[MdmEnvironment.Production] : "https://" + ProductionGlobalEnvironmentResolver.ResolveGlobalStampHostName(),
          "https://global.int.microsoftmetrics.com",
          "https://" + ProductionGlobalEnvironmentResolver.PotentialProductionGlobalEnvironments[MdmEnvironment.UsSec],
          "https://" + ProductionGlobalEnvironmentResolver.PotentialProductionGlobalEnvironments[MdmEnvironment.UsNat],
          "https://global.ppe.microsoftmetrics.com",
          "https://global.test.microsoftmetrics.com"
        };
        for (int index = 0; index < ConnectionInfo.globalEnvironments.Length; ++index)
        {
          Uri uri = new Uri(ConnectionInfo.globalEnvironments[index]);
          ConnectionInfo.GslbToUris[index].TryAdd(string.Empty, uri);
        }
        return ConnectionInfo.globalEnvironments;
      }
    }

    internal static async Task<string> ResolveIp(string hostname, bool throwOnFailure)
    {
      Exception innerException = (Exception) null;
      for (int i = 0; i < 3; ++i)
      {
        if (i != 0)
          await Task.Delay(TimeSpan.FromMilliseconds((double) (10 * i)));
        try
        {
          return await ConnectionInfo.ResolveIpNoRetry(hostname).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
          innerException = ex;
        }
      }
      Logger.Log(LoggerLevel.Error, ConnectionInfo.LogId, nameof (ResolveIp), string.Format("Resolving hostname to IP got an exception. HostName:{0}, Exception:{1}", (object) hostname, (object) innerException));
      if (throwOnFailure)
        throw new MetricsClientException("Resolving " + hostname + " to IP got an exception.", innerException);
      return (string) null;
    }

    internal static async Task<string> GetCachedIpAddress(string hostname)
    {
      if (ConnectionInfo.DisableDnsResolution || hostname.Equals("metrics-ob.dc.ad.msft.net", StringComparison.OrdinalIgnoreCase))
        return hostname;
      string cachedIpAddress1;
      if (ConnectionInfo.HostToIpAddressMap.TryGetValue(hostname, out cachedIpAddress1))
        return cachedIpAddress1;
      string cachedIpAddress2 = await ConnectionInfo.ResolveIp(hostname, true).ConfigureAwait(false);
      ConnectionInfo.HostToIpAddressMap.TryAdd(hostname, cachedIpAddress2);
      return cachedIpAddress2;
    }

    internal string GetAuthRelativeUrl(string relativeUrl) => !this.UseAadUserAuthentication ? "/api/" + relativeUrl : "/user-api/" + relativeUrl;

    private static async Task<string> ResolveIpNoRetry(string hostname)
    {
      IPAddress[] source = await Dns.GetHostAddressesAsync(hostname).ConfigureAwait(false);
      string str = source != null ? ((IEnumerable<IPAddress>) source).FirstOrDefault<IPAddress>((Func<IPAddress, bool>) (a => a.AddressFamily == AddressFamily.InterNetwork))?.ToString() : (string) null;
      return !string.IsNullOrWhiteSpace(str) ? str : throw new Exception("Resolved IP is null or empty. AddressList:" + JsonConvert.SerializeObject((object) source) + ".");
    }

    private static async void RefreshIpAddresses()
    {
      try
      {
        Logger.Log(LoggerLevel.Info, ConnectionInfo.LogId, nameof (RefreshIpAddresses), "Initiated the automatic refresh of IP addresses of home stamp endpoints.");
        ConnectionInfo.ResolveGlobalEnvironments();
        foreach (KeyValuePair<string, string> hostToIpAddress in ConnectionInfo.HostToIpAddressMap)
        {
          KeyValuePair<string, string> kvp = hostToIpAddress;
          string newValue = await ConnectionInfo.ResolveIp(kvp.Key, false).ConfigureAwait(false);
          if (!string.IsNullOrWhiteSpace(newValue))
          {
            string str = kvp.Value;
            if (newValue != str)
              ConnectionInfo.HostToIpAddressMap.TryUpdate(kvp.Key, newValue, kvp.Value);
          }
          kvp = new KeyValuePair<string, string>();
        }
      }
      catch (Exception ex)
      {
        Logger.Log(LoggerLevel.Error, ConnectionInfo.LogId, nameof (RefreshIpAddresses), string.Format("Hit exception: {0}", (object) ex));
      }
      finally
      {
        if (ConnectionInfo.DnsResolutionUpdateFrequency == TimeSpan.Zero)
          Logger.Log(LoggerLevel.Warning, ConnectionInfo.LogId, nameof (RefreshIpAddresses), "Terminate the refresh task since the refresh frequency is 0");
        else if (ConnectionInfo.DisableDnsResolution)
          Logger.Log(LoggerLevel.Warning, ConnectionInfo.LogId, nameof (RefreshIpAddresses), "Terminate the refresh task since DNS resolution is disabled");
        else
          ConnectionInfo.TimerToRefreshIpAddress.Change(ConnectionInfo.DnsResolutionUpdateFrequency, System.Threading.Timeout.InfiniteTimeSpan);
      }
    }

    private static async void RefreshAccountHomeStamp()
    {
      if (ConnectionInfo.globalEnvironments == null)
      {
        Logger.Log(LoggerLevel.Error, ConnectionInfo.LogId, nameof (RefreshAccountHomeStamp), "The global environments haven't been resolved yet.");
      }
      else
      {
        for (int index = 0; index < 6; ++index)
        {
          try
          {
            await ConnectionInfo.UpdateAccountToUriMapAsync(ConnectionInfo.HttpClientWithoutAuthentication, ConnectionInfo.globalEnvironments[index], ConnectionInfo.GslbToUris[index], ConnectionInfo.AccountToUriMaps[index]).ConfigureAwait(false);
          }
          catch (Exception ex)
          {
            Logger.Log(LoggerLevel.Error, ConnectionInfo.LogId, nameof (RefreshAccountHomeStamp), string.Format("The periodic background task that updates the account to URI map got an exception. Environment:{0}, Exception:{1}", (object) ConnectionInfo.globalEnvironments[index], (object) ex));
          }
        }
        if (ConnectionInfo.HomeStampAutomaticUpdateFrequency == TimeSpan.Zero)
          Logger.Log(LoggerLevel.Error, ConnectionInfo.LogId, nameof (RefreshAccountHomeStamp), "Terminate the home stamp refresh task since the refresh frequency is 0");
        else
          ConnectionInfo.TimerToRefreshHomeStamp.Change(ConnectionInfo.HomeStampAutomaticUpdateFrequency, System.Threading.Timeout.InfiniteTimeSpan);
      }
    }

    private static async Task UpdateAccountToUriMapAsync(
      HttpClient httpClient,
      string globalEnvironmentUrl,
      ConcurrentDictionary<string, Uri> gslbToUris,
      ConcurrentDictionary<string, ConnectionInfo.StampInformation> targetMap)
    {
      Logger.Log(LoggerLevel.Info, ConnectionInfo.LogId, nameof (UpdateAccountToUriMapAsync), "Initiated the automatic refresh of the account home stamp endpoints.");
      foreach (string account in (IEnumerable<string>) targetMap.Keys)
      {
        try
        {
          ConnectionInfo.StampInformation stampInformation = await ConnectionInfo.GetAndUpdateStampInfoAsync(httpClient, globalEnvironmentUrl, gslbToUris, targetMap, account).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
          Logger.Log(LoggerLevel.Error, ConnectionInfo.LogId, "AccountToUriUpdate", "The periodic background task that updates the account to URI map got an exception for account {0}. URL:{1}, Exception:{2}", (object) account, (object) globalEnvironmentUrl, (object) ex);
        }
      }
    }

    private static async Task<ConnectionInfo.StampInformation> GetAndUpdateStampInfoAsync(
      HttpClient httpClient,
      string globalStampUrl,
      ConcurrentDictionary<string, Uri> gslbToUris,
      ConcurrentDictionary<string, ConnectionInfo.StampInformation> targetMap,
      string account)
    {
      string homeStampGslbHostname = JsonConvert.DeserializeObject<string>(await HttpClientHelper.GetJsonResponse(new Uri(globalStampUrl + "/public/monitoringAccount/" + account + "/homeStamp"), HttpMethod.Get, httpClient, account, "GetHomeStamp").ConfigureAwait(false));
      string str;
      if (!string.IsNullOrEmpty(homeStampGslbHostname))
        str = await ConnectionInfo.SafeGetStampMetricDataQueryEndpointHostNameAsync(httpClient, "https://" + homeStampGslbHostname).ConfigureAwait(false);
      string key = string.IsNullOrWhiteSpace(homeStampGslbHostname) ? string.Empty : homeStampGslbHostname;
      Uri homeGslbUri;
      if (!gslbToUris.TryGetValue(key, out homeGslbUri))
      {
        homeGslbUri = new Uri("https://" + homeStampGslbHostname);
        gslbToUris.AddOrUpdate(key, homeGslbUri, (Func<string, Uri, Uri>) ((_, uri) => homeGslbUri));
      }
      ConnectionInfo.StampInformation stampInfo = new ConnectionInfo.StampInformation(homeGslbUri, string.IsNullOrEmpty(str) ? homeGslbUri : new Uri("https://" + str));
      targetMap.AddOrUpdate(account, stampInfo, (Func<string, ConnectionInfo.StampInformation, ConnectionInfo.StampInformation>) ((_, uri) => stampInfo));
      ConnectionInfo.StampInformation updateStampInfoAsync = stampInfo;
      homeStampGslbHostname = (string) null;
      return updateStampInfoAsync;
    }

    private static async Task<string> SafeGetStampMetricDataQueryEndpointHostNameAsync(
      HttpClient httpClient,
      string stampUrl)
    {
      try
      {
        return JsonConvert.DeserializeObject<string>(await HttpClientHelper.GetJsonResponse(new Uri(stampUrl + "/public/metricsDataQueryEndpointHostName"), HttpMethod.Get, httpClient, (string) null, (string) null).ConfigureAwait(false));
      }
      catch (Exception ex)
      {
        int num;
        if (!(ex is MetricsClientException metricsClientException))
        {
          num = 0;
        }
        else
        {
          HttpStatusCode? responseStatusCode = metricsClientException.ResponseStatusCode;
          HttpStatusCode httpStatusCode = HttpStatusCode.NotFound;
          num = responseStatusCode.GetValueOrDefault() == httpStatusCode & responseStatusCode.HasValue ? 1 : 0;
        }
        if (num != 0)
          Logger.Log(LoggerLevel.Info, ConnectionInfo.LogId, nameof (SafeGetStampMetricDataQueryEndpointHostNameAsync), "Failed to resolve query endpoint for the mdm stamp due to query resolution endpoint not found, will use main endpoint.");
        else
          Logger.Log(LoggerLevel.Warning, ConnectionInfo.LogId, nameof (SafeGetStampMetricDataQueryEndpointHostNameAsync), "Failed to resolve query endpoint for the mdm stamp due to unexpected exception, will use main endpoint.", (object) ex);
        return string.Empty;
      }
    }

    private async Task<ConnectionInfo.StampInformation> GetAndUpdateIfRequiredStampInformation(
      string monitoringAccount)
    {
      SemaphoreSlim semaphore = ConnectionInfo.Semaphores[Math.Abs(monitoringAccount.GetHashCode() % ConnectionInfo.Semaphores.Length)];
      ConnectionInfo.StampInformation stampInformation1;
      if (!ConnectionInfo.AccountToUriMaps[this.mdmEnvironmentMapIndex].TryGetValue(monitoringAccount, out stampInformation1))
      {
        TimeSpan timeout = TimeSpan.FromMinutes(2.0);
        if (!await semaphore.WaitAsync(timeout).ConfigureAwait(false))
          throw new MetricsClientException(string.Format("Failed to acquire a semaphore to get stamp info in {0}. MdmEnvironmentMapIndex:{1}, AccountToUriMapSize:{2}.", (object) timeout, (object) this.mdmEnvironmentMapIndex, (object) ConnectionInfo.AccountToUriMaps[this.mdmEnvironmentMapIndex].Count));
        try
        {
          if (!ConnectionInfo.AccountToUriMaps[this.mdmEnvironmentMapIndex].TryGetValue(monitoringAccount, out stampInformation1))
            stampInformation1 = await ConnectionInfo.GetAndUpdateStampInfoAsync(ConnectionInfo.HttpClientWithoutAuthentication, ConnectionInfo.ResolveGlobalEnvironments()[this.mdmEnvironmentMapIndex], ConnectionInfo.GslbToUris[this.mdmEnvironmentMapIndex], ConnectionInfo.AccountToUriMaps[this.mdmEnvironmentMapIndex], monitoringAccount).ConfigureAwait(false);
        }
        finally
        {
          semaphore.Release();
        }
        timeout = new TimeSpan();
      }
      ConnectionInfo.StampInformation stampInformation2 = stampInformation1;
      semaphore = (SemaphoreSlim) null;
      return stampInformation2;
    }

    private struct StampInformation
    {
      public StampInformation(Uri stampMainUri, Uri stampQueryUri)
      {
        this.StampMainUri = stampMainUri;
        this.StampQueryUri = stampQueryUri;
      }

      public Uri StampMainUri { get; }

      public Uri StampQueryUri { get; }
    }
  }
}
