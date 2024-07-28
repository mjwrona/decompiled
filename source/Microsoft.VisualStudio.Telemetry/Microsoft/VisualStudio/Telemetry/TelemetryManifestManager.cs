// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryManifestManager
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.RemoteControl;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryManifestManager : 
    TelemetryDisposableObject,
    ITelemetryManifestManager,
    IDisposable
  {
    private const int RemoteControlReadFileTelemetryFrequency = 6;
    private static readonly TimeSpan DownloadInterval = TimeSpan.FromHours(6.0);
    private static readonly TimeSpan ReadInterval = TimeSpan.FromMinutes(5.0);
    private static readonly TimeSpan ForceReadDelay = TimeSpan.FromSeconds(10.0);
    private readonly ITelemetryManifestParser manifestParser;
    private readonly ITelemetryScheduler scheduler;
    private readonly TelemetrySession mainSession;
    private IRemoteControlClient remoteControlClient;
    private ITelemetryManifestManagerSettings settings;
    private CancellationTokenSource tokenSource = new CancellationTokenSource();
    private bool isStarted;

    public event EventHandler<TelemetryManifestEventArgs> UpdateTelemetryManifestStatusEvent;

    public bool ForcedReadManifest { get; private set; }

    internal TelemetryManifest CurrentManifest { get; private set; }

    public TelemetryManifestManager(
      IRemoteControlClient theRemoteControlClient,
      ITelemetryManifestManagerSettings theSettings,
      ITelemetryManifestParser theManifestParser,
      ITelemetryScheduler theScheduler,
      TelemetrySession theMainSession)
    {
      theManifestParser.RequiresArgumentNotNull<ITelemetryManifestParser>(nameof (theManifestParser));
      theScheduler.RequiresArgumentNotNull<ITelemetryScheduler>(nameof (theScheduler));
      theMainSession.RequiresArgumentNotNull<TelemetrySession>(nameof (theMainSession));
      this.manifestParser = theManifestParser;
      this.scheduler = theScheduler;
      this.scheduler.InitializeTimed(TelemetryManifestManager.ReadInterval);
      this.mainSession = theMainSession;
      this.remoteControlClient = theRemoteControlClient;
      this.settings = theSettings;
      RemoteControlClient.TelemetryLogger2 = (Action<string, IDictionary<string, object>, IDictionary<string, object>>) ((eventName, properties, piiProperties) =>
      {
        TelemetryEvent telemetryEvent = new TelemetryEvent(eventName);
        telemetryEvent.Properties.AddRange<string, object>(properties);
        telemetryEvent.Properties.AddRange<string, object>((IDictionary<string, object>) piiProperties.ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (p => p.Key), (Func<KeyValuePair<string, object>, object>) (p => (object) new TelemetryPiiProperty(p.Value))));
        this.mainSession.PostEvent(telemetryEvent);
      });
    }

    public void Start(string hostName, bool isDisposing)
    {
      if (this.isStarted)
        return;
      if (this.settings == null)
        this.settings = (ITelemetryManifestManagerSettings) new TelemetryManifestManagerSettings(hostName);
      if (this.remoteControlClient == null)
        this.remoteControlClient = (IRemoteControlClient) new RemoteControlClient(Platform.IsWindows ? (IRegistryTools) new RegistryTools() : (IRegistryTools) new FileBasedRegistryTools(), this.settings.HostId, this.settings.BaseUrl, this.settings.RelativePath, (int) TelemetryManifestManager.DownloadInterval.TotalMinutes, overrideReadFileTelemetryFrequency: 6);
      if (!isDisposing)
      {
        CancellationToken token = this.tokenSource.Token;
        this.scheduler.Schedule((Func<Task>) (async () =>
        {
          if (token.IsCancellationRequested)
            return;
          await this.Check(BehaviorOnStale.ReturnStale, token).ConfigureAwait(false);
          if (token.IsCancellationRequested || this.CurrentManifest != null)
            return;
          await this.Check(BehaviorOnStale.ForceDownload, token).ConfigureAwait(false);
        }), new CancellationToken?(token));
        this.scheduler.ScheduleTimed(new Func<Task>(this.Check), true);
      }
      this.isStarted = true;
    }

    public bool ForceReadManifest()
    {
      if (this.CurrentManifest == null)
      {
        this.tokenSource.Cancel();
        this.ForcedReadManifest = true;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        if (!this.Check(BehaviorOnStale.ReturnStale, cancellationTokenSource.Token).Wait(TelemetryManifestManager.ForceReadDelay))
          cancellationTokenSource.Cancel();
      }
      return this.CurrentManifest != null;
    }

    internal async Task Check() => await this.Check(BehaviorOnStale.ReturnStale).ConfigureAwait(false);

    protected override void DisposeManagedResources()
    {
      if (!this.isStarted)
        return;
      if (!this.ForcedReadManifest)
        this.tokenSource.Cancel();
      this.remoteControlClient.Dispose();
      this.scheduler.CancelTimed(true);
    }

    private async Task Check(BehaviorOnStale staleBehavior, CancellationToken token = default (CancellationToken))
    {
      try
      {
        await this.LoadManifest(staleBehavior, token).ConfigureAwait(false);
      }
      catch (TelemetryManifestParserException ex)
      {
        if (token.IsCancellationRequested)
          return;
        List<string> values = new List<string>();
        for (Exception innerException = ex.InnerException; innerException != null; innerException = innerException.InnerException)
          values.Add(innerException.Message);
        this.OnUpdateTelemetryManifestStatusEvent(new TelemetryManifestEventArgs((TelemetryManifest) null));
        this.InstrumentLoad((TelemetryManifest) null, 0L, ex.Message, values.Count > 0 ? values.Join(";") : (string) null, 0.0);
      }
      catch (Exception ex)
      {
        FaultEvent faultEvent = new FaultEvent("VS/Telemetry/InternalFault", string.Format("LoadManifest ManifestManager.Check"), ex)
        {
          PostThisEventToTelemetry = false
        };
        using (Process currentProcess = Process.GetCurrentProcess())
        {
          faultEvent.AddProcessDump(currentProcess.Id);
          this.mainSession.PostEvent((TelemetryEvent) faultEvent);
        }
      }
    }

    private async Task LoadManifest(BehaviorOnStale staleBehavior, CancellationToken token = default (CancellationToken))
    {
      Stopwatch watch = Stopwatch.StartNew();
      Tuple<TelemetryManifest, long> tuple = await this.ReadAndParseManifest(staleBehavior).ConfigureAwait(false);
      watch.Stop();
      if (token.IsCancellationRequested)
      {
        watch = (Stopwatch) null;
      }
      else
      {
        if (this.CurrentManifest != null)
        {
          if (tuple.Item1 == null)
          {
            watch = (Stopwatch) null;
            return;
          }
          if (this.CurrentManifest.Version == tuple.Item1.Version)
          {
            watch = (Stopwatch) null;
            return;
          }
        }
        string message = "Manifest is null";
        if (tuple.Item1 != null)
        {
          this.CurrentManifest = tuple.Item1;
          message = (string) null;
        }
        this.OnUpdateTelemetryManifestStatusEvent(new TelemetryManifestEventArgs(tuple.Item1));
        this.InstrumentLoad(tuple.Item1, tuple.Item2, message, (string) null, (double) watch.ElapsedMilliseconds);
        watch = (Stopwatch) null;
      }
    }

    private async Task<Tuple<TelemetryManifest, long>> ReadAndParseManifest(
      BehaviorOnStale staleBehavior)
    {
      Stream stream = await this.remoteControlClient.ReadFileAsync(staleBehavior).ConfigureAwait(false);
      if (stream == null)
        return new Tuple<TelemetryManifest, long>((TelemetryManifest) null, 0L);
      using (StreamReader streamReader = new StreamReader(stream))
      {
        try
        {
          return new Tuple<TelemetryManifest, long>((TelemetryManifest) await this.manifestParser.ParseAsync((TextReader) streamReader).ConfigureAwait(false), stream.Length);
        }
        catch (Exception ex)
        {
          switch (ex)
          {
            case IOException _:
            case ThreadAbortException _:
              return new Tuple<TelemetryManifest, long>((TelemetryManifest) null, 0L);
            default:
              throw;
          }
        }
      }
    }

    private void OnUpdateTelemetryManifestStatusEvent(TelemetryManifestEventArgs e)
    {
      EventHandler<TelemetryManifestEventArgs> manifestStatusEvent = this.UpdateTelemetryManifestStatusEvent;
      if (manifestStatusEvent == null)
        return;
      manifestStatusEvent((object) this, e);
    }

    private void InstrumentLoad(
      TelemetryManifest telemetryManifest,
      long streamSize,
      string message,
      string errorDetails,
      double duration)
    {
      if (this.mainSession.IsSessionCloned)
        return;
      bool flag = telemetryManifest != null;
      TelemetryEvent telemetryEvent = new TelemetryEvent("VS/TelemetryApi/Manifest/Load");
      telemetryEvent.Properties["VS.TelemetryApi.DynamicTelemetry.HostName"] = (object) this.mainSession.HostName;
      telemetryEvent.Properties["VS.TelemetryApi.Manifest.Load.IsLoadSuccess"] = (object) flag;
      if (streamSize > 0L)
        telemetryEvent.Properties["VS.TelemetryApi.Manifest.Load.StreamSize"] = (object) streamSize;
      if (flag)
      {
        if (telemetryManifest != null)
        {
          telemetryEvent.Properties["VS.TelemetryApi.DynamicTelemetry.Manifest.Version"] = (object) telemetryManifest.Version;
          telemetryEvent.Properties["VS.TelemetryApi.DynamicTelemetry.Manifest.FormatVersion"] = (object) 2U;
          if (telemetryManifest.InvalidRules.Any<string>())
            telemetryEvent.Properties["VS.TelemetryApi.DynamicTelemetry.Manifest.UnrecognizedRules"] = (object) telemetryManifest.InvalidRules.Join(",");
          telemetryEvent.Properties["VS.TelemetryApi.DynamicTelemetry.Manifest.UnrecognizedRules.Count"] = (object) telemetryManifest.InvalidRules.Count<string>();
          telemetryEvent.Properties["VS.TelemetryApi.DynamicTelemetry.Manifest.UnrecognizedActions.Count"] = (object) telemetryManifest.InvalidActionCount;
          string str = telemetryManifest.GetAllSamplings().Where<TelemetryManifestMatchSampling.Path>((Func<TelemetryManifestMatchSampling.Path, bool>) (path => path.Sampling.IsSampleActive)).Select<TelemetryManifestMatchSampling.Path, string>((Func<TelemetryManifestMatchSampling.Path, string>) (path => path.FullName)).Join(",");
          if (str != string.Empty)
            telemetryEvent.Properties["VS.TelemetryApi.DynamicTelemetry.Manifest.EnabledSamplings"] = (object) str;
        }
        telemetryEvent.Properties["VS.TelemetryApi.Manifest.Load.Duration"] = (object) duration;
      }
      else
      {
        telemetryEvent.Properties["VS.TelemetryApi.Manifest.Load.ErrorMessage"] = (object) message;
        if (errorDetails != null)
          telemetryEvent.Properties["VS.TelemetryApi.Manifest.Load.ErrorDetails"] = (object) new TelemetryPiiProperty((object) errorDetails);
      }
      this.mainSession.PostEvent(telemetryEvent);
    }
  }
}
