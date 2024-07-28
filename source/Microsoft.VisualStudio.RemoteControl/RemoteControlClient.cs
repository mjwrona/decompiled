// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteControl.RemoteControlClient
// Assembly: Microsoft.VisualStudio.RemoteControl, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4D9D0761-3208-49DD-A9E2-BF705DBE6B5D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.RemoteControl.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.RemoteControl
{
  public class RemoteControlClient : IRemoteControlClient, IDisposable
  {
    private static Action<string, IDictionary<string, object>> telemetryLogger = (Action<string, IDictionary<string, object>>) ((eventName, properties) => { });
    private static Action<string, IDictionary<string, object>, IDictionary<string, object>> telemetryLogger2 = (Action<string, IDictionary<string, object>, IDictionary<string, object>>) ((eventName, properties, piiProperties) => RemoteControlClient.telemetryLogger(eventName, properties));
    private const int DefaultHTTPRequestTimeoutSeconds = 60;
    private const int DefaultPollingIntervalMins = 1380;
    private const int DefaultReadFileTelemetry = 1;
    private const int MinPollingIntervalMins = 5;
    private const int MaxHTTPRequestTimeoutSeconds = 60;
    private const int RemoteControlExplicitlyDisabled = 1;
    private readonly int maxRandomDownloadDelaySeconds = 15000;
    private readonly int httpRequestTimeoutSeconds;
    private readonly System.Threading.Timer cacheUpdateTimer;
    private readonly Random rand = new Random();
    private readonly IRemoteControlHTTPRequestor requestor;
    private readonly SemaphoreSlim updateMutex = new SemaphoreSlim(1, 1);
    private readonly CancellationTokenSource cancellationToken = new CancellationTokenSource();
    private readonly IFileReader fileReader;
    private readonly bool isDisabled;
    private bool isDisposed;

    public RemoteControlClient(
      string hostId,
      string baseUrl,
      string relativePath,
      int pollingIntervalMins = 1380,
      int theHttpRequestTimeoutSeconds = 60,
      int overrideReadFileTelemetryFrequency = 1)
      : this(Platform.IsWindows ? (IRegistryTools) new RegistryTools() : (IRegistryTools) new FileBasedRegistryTools(), hostId, baseUrl, relativePath, pollingIntervalMins, theHttpRequestTimeoutSeconds, overrideReadFileTelemetryFrequency)
    {
    }

    internal RemoteControlClient(
      IRegistryTools registryTools,
      string hostId,
      string baseUrl,
      string relativePath,
      int pollingIntervalMins = 1380,
      int theHttpRequestTimeoutSeconds = 60,
      int overrideReadFileTelemetryFrequency = 1)
      : this(registryTools, hostId, baseUrl, relativePath, pollingIntervalMins)
    {
      if (!this.Uri.IsLocalFile)
      {
        this.httpRequestTimeoutSeconds = Math.Min(60, theHttpRequestTimeoutSeconds);
        if (this.isDisabled)
          return;
        this.requestor = (IRemoteControlHTTPRequestor) new RemoteControlHTTPRequestor(this.FullUrl, this.httpRequestTimeoutSeconds * 1000);
        this.cacheUpdateTimer = new System.Threading.Timer((TimerCallback) (o => this.CacheUpdateTimerCallback(o, this.cancellationToken.Token).SwallowException()));
        this.cacheUpdateTimer.Change(0, -1);
      }
      else
        this.fileReader = (IFileReader) new FileReader(this.FullUrl);
    }

    internal RemoteControlClient(
      IRemoteControlHTTPRequestor requestor,
      IRegistryTools theRegistryTools,
      string hostId,
      string baseUrl,
      string relativePath,
      int pollingIntervalMins = 1380,
      int httpRequestTimeoutSeconds = 60,
      int maxRandomDownloadDelaySeconds = 0)
      : this(theRegistryTools, hostId, baseUrl, relativePath, pollingIntervalMins)
    {
      this.maxRandomDownloadDelaySeconds = maxRandomDownloadDelaySeconds;
      this.requestor = requestor;
      this.cacheUpdateTimer = new System.Threading.Timer((TimerCallback) (o => this.CacheUpdateTimerCallback(o, this.cancellationToken.Token).SwallowException()), (object) null, -1, -1);
    }

    internal RemoteControlClient(
      IRegistryTools theRegistryTools,
      IFileReader theFileReader,
      string hostId,
      string baseUrl,
      string relativePath)
      : this(theRegistryTools, hostId, baseUrl, relativePath, 1380)
    {
      this.fileReader = theFileReader;
    }

    private RemoteControlClient(
      IRegistryTools registryTools,
      string hostId,
      string baseUrl,
      string relativePath,
      int pollingIntervalMins)
    {
      this.Uri = RemoteControlUri.Create(registryTools, hostId, baseUrl, relativePath);
      this.PollingIntervalMins = Math.Max(5, pollingIntervalMins);
      int num = 0;
      try
      {
        num = Convert.ToInt32(registryTools.GetRegistryValueFromCurrentUserRoot("Software\\Microsoft\\VisualStudio\\RemoteControl", "TurnOffSwitch", (object) 0));
      }
      catch
      {
      }
      this.isDisabled = num == 1;
    }

    public static Action<string, IDictionary<string, object>> TelemetryLogger
    {
      get => RemoteControlClient.telemetryLogger;
      set
      {
        value.RequiresArgumentNotNull<Action<string, IDictionary<string, object>>>(nameof (value));
        RemoteControlClient.telemetryLogger = value;
      }
    }

    public static Action<string, IDictionary<string, object>, IDictionary<string, object>> TelemetryLogger2
    {
      get => RemoteControlClient.telemetryLogger2;
      set
      {
        value.RequiresArgumentNotNull<Action<string, IDictionary<string, object>, IDictionary<string, object>>>(nameof (value));
        RemoteControlClient.telemetryLogger2 = value;
      }
    }

    public string FullUrl => this.Uri.FullUrl;

    public int PollingIntervalMins { get; }

    internal RemoteControlUri Uri { get; }

    public Stream ReadFile(BehaviorOnStale staleBehavior)
    {
      if (this.isDisabled)
        return (Stream) null;
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (RemoteControlClient));
      return this.ReadFileAsync(staleBehavior).Result;
    }

    public async Task<Stream> ReadFileAsync(BehaviorOnStale staleBehavior)
    {
      if (this.isDisabled)
        return (Stream) null;
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (RemoteControlClient));
      if (this.Uri.IsLocalFile)
        return (await this.ReadFileFromLocalAsync().ConfigureAwait(false)).RespStream;
      switch (staleBehavior)
      {
        case BehaviorOnStale.ReturnStale:
          return (await this.GetFileAndInstrumentAsync().ConfigureAwait(false)).RespStream;
        case BehaviorOnStale.ReturnNull:
          GetFileResult fileResult = await this.GetFileAndInstrumentAsync().ConfigureAwait(false);
          if (this.IsStale(fileResult))
            return fileResult.RespStream;
          fileResult.Dispose();
          return (Stream) null;
        case BehaviorOnStale.ForceDownload:
          return (await this.GetFileAndInstrumentAsync(true).ConfigureAwait(false)).RespStream;
        default:
          return (Stream) null;
      }
    }

    public void Dispose()
    {
      if (this.isDisposed)
        return;
      if (this.cacheUpdateTimer != null)
        this.cacheUpdateTimer.Dispose();
      this.cancellationToken.Cancel();
      if (this.requestor != null)
        this.requestor.Cancel();
      this.isDisposed = true;
    }

    internal bool RunUpdateFileMethod()
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (RemoteControlClient));
      return this.EnsureFileIsUpToDateAsync(new CancellationToken(false)).Result;
    }

    private async Task<GetFileResult> ReadFileFromLocalAsync() => await Task.Run<GetFileResult>((Func<GetFileResult>) (() =>
    {
      GetFileResult getFileResult = new GetFileResult()
      {
        Code = HttpStatusCode.Unused
      };
      try
      {
        getFileResult.RespStream = this.fileReader.ReadFile();
        getFileResult.Code = HttpStatusCode.OK;
      }
      catch (ArgumentException ex)
      {
        getFileResult.ErrorMessage = "File path contains invalid characters";
      }
      catch (IOException ex)
      {
        getFileResult.ErrorMessage = "IO exception reading file";
      }
      catch (UnauthorizedAccessException ex)
      {
        getFileResult.ErrorMessage = "Could not access file for reading";
      }
      return getFileResult;
    })).ConfigureAwait(false);

    private async Task CacheUpdateTimerCallback(object stateInfo, CancellationToken token)
    {
      try
      {
        int num = await this.EnsureFileIsUpToDateAsync(token).ConfigureAwait(false) ? 1 : 0;
      }
      finally
      {
        if (!this.isDisposed)
        {
          try
          {
            this.cacheUpdateTimer.Change(this.PollingIntervalMins * 60 * 1000, -1);
          }
          catch (ObjectDisposedException ex)
          {
          }
        }
      }
    }

    private async Task<bool> EnsureFileIsUpToDateAsync(CancellationToken cancellationToken)
    {
      if (this.isDisabled)
        return false;
      try
      {
        await this.updateMutex.WaitAsync(cancellationToken).ConfigureAwait(false);
        for (int i = 1; i <= 2; ++i)
        {
          using (GetFileResult fileResult = await this.GetFileAndInstrumentAsync().ConfigureAwait(false))
          {
            if (this.IsStale(fileResult))
              return fileResult.IsSuccessStatusCode;
          }
          if (await this.requestor.LastServerRequestErrorSecondsAgoAsync().ConfigureAwait(false) < this.PollingIntervalMins * 60)
            return false;
          if (i < 2)
            await Task.Delay(this.rand.Next(0, this.maxRandomDownloadDelaySeconds), cancellationToken).ConfigureAwait(false);
        }
        using (GetFileResult getFileResult = await this.GetFileAndInstrumentAsync(true).ConfigureAwait(false))
          return getFileResult.IsSuccessStatusCode;
      }
      finally
      {
        try
        {
          this.updateMutex.Release();
        }
        catch (SemaphoreFullException ex)
        {
        }
      }
    }

    private async Task<GetFileResult> GetFileAndInstrumentAsync(bool fromServer = false)
    {
      GetFileResult fileResult = await (fromServer ? this.requestor.GetFileFromServerAsync() : this.requestor.GetFileFromCacheAsync()).ConfigureAwait(false);
      if (fromServer && (!fileResult.IsFromCache || !fileResult.IsSuccessStatusCode))
        this.InstrumentGetFile(fileResult);
      return fileResult;
    }

    private void InstrumentGetFile(GetFileResult fileResult)
    {
      Dictionary<string, object> dictionary1 = new Dictionary<string, object>()
      {
        {
          "VS.RemoteControl.DownloadFile.FullUrl",
          (object) this.FullUrl
        },
        {
          "VS.RemoteControl.DownloadFile.IsSuccess",
          (object) fileResult.IsSuccessStatusCode
        },
        {
          "VS.RemoteControl.DownloadFile.HttpRequestTimeoutInSecs",
          (object) this.httpRequestTimeoutSeconds
        },
        {
          "VS.RemoteControl.DownloadFile.IsFromCache",
          (object) fileResult.IsFromCache
        },
        {
          "VS.RemoteControl.DownloadFile.PollingIntervalInMins",
          (object) this.PollingIntervalMins
        }
      };
      Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
      if (fileResult.IsSuccessStatusCode)
      {
        dictionary1.Add("VS.RemoteControl.DownloadFile.IsNotFound", (object) (fileResult.Code == HttpStatusCode.NotFound));
        if (fileResult.RespStream != null)
          dictionary1.Add("VS.RemoteControl.DownloadFile.StreamSize", (object) fileResult.RespStream.Length);
        if (fileResult.AgeSeconds.HasValue)
          dictionary1.Add("VS.RemoteControl.DownloadFile.AgeInSecs", (object) fileResult.AgeSeconds.Value);
      }
      else
      {
        if (fileResult.Code != HttpStatusCode.Unused)
          dictionary1.Add("VS.RemoteControl.DownloadFile.ErrorCode", (object) System.Enum.GetName(typeof (HttpStatusCode), (object) fileResult.Code));
        dictionary2.Add("VS.RemoteControl.DownloadFile.ErrorMessage", (object) fileResult.ErrorMessage);
      }
      RemoteControlClient.TelemetryLogger2("VS/RemoteControl/DownloadFile", (IDictionary<string, object>) dictionary1, (IDictionary<string, object>) dictionary2);
    }

    private bool IsStale(GetFileResult fileResult)
    {
      if (!fileResult.IsFromCache || !fileResult.AgeSeconds.HasValue)
        return false;
      int? ageSeconds = fileResult.AgeSeconds;
      int num = this.PollingIntervalMins * 60;
      return ageSeconds.GetValueOrDefault() <= num & ageSeconds.HasValue;
    }
  }
}
