// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryService
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.SessionChannel;
using Microsoft.VisualStudio.Utilities.Internal;
using System;

namespace Microsoft.VisualStudio.Telemetry
{
  [CLSCompliant(false)]
  public static class TelemetryService
  {
    private static object lockDefaultSessionCreation = new object();

    internal static TelemetrySession InternalDefaultSession { get; set; }

    public static TelemetrySession DefaultSession
    {
      get
      {
        if (TelemetryService.InternalDefaultSession == null)
        {
          lock (TelemetryService.lockDefaultSessionCreation)
          {
            if (TelemetryService.InternalDefaultSession == null)
              TelemetryService.InternalDefaultSession = TelemetrySession.Create();
          }
        }
        return TelemetryService.InternalDefaultSession;
      }
    }

    public static AssetService AssetService => AssetService.Instance;

    internal static ITelemetryEtwProvider TelemetryEventSource { get; set; }

    public static void SetDefaultSession(TelemetrySession telemetrySession)
    {
      if (!TelemetryService.TrySetDefaultSession(telemetrySession))
        throw new InvalidOperationException();
    }

    public static bool TrySetDefaultSession(TelemetrySession telemetrySession)
    {
      if (TelemetryService.InternalDefaultSession == null)
      {
        lock (TelemetryService.lockDefaultSessionCreation)
        {
          if (TelemetryService.InternalDefaultSession == null)
          {
            TelemetryService.InternalDefaultSession = telemetrySession;
            return true;
          }
        }
      }
      return false;
    }

    public static TelemetrySession CreateAndGetDefaultSession(
      string appInsightsIKey,
      string asimovIKey)
    {
      return TelemetryService.CreateAndGetDefaultSession(appInsightsIKey, asimovIKey, (string) null);
    }

    public static TelemetrySession CreateAndGetDefaultSession(string collectorApiKey) => TelemetryService.CreateAndGetDefaultSession((string) null, (string) null, collectorApiKey);

    public static TelemetrySession CreateAndGetDefaultSession(
      string appInsightsIKey,
      string asimovIKey,
      string collectorApiKey)
    {
      bool flag = true;
      if (TelemetryService.InternalDefaultSession == null)
      {
        lock (TelemetryService.lockDefaultSessionCreation)
        {
          if (TelemetryService.InternalDefaultSession == null)
          {
            if (string.IsNullOrWhiteSpace(asimovIKey) && string.IsNullOrWhiteSpace(collectorApiKey))
            {
              asimovIKey.RequiresArgumentNotNullAndNotWhiteSpace(nameof (asimovIKey));
              collectorApiKey.RequiresArgumentNotNullAndNotWhiteSpace(nameof (collectorApiKey));
            }
            TelemetrySessionInitializer initializerObject = TelemetrySessionInitializer.Default;
            initializerObject.AppInsightsInstrumentationKey = appInsightsIKey;
            initializerObject.AsimovInstrumentationKey = asimovIKey;
            initializerObject.CollectorApiKey = collectorApiKey;
            TelemetryService.InternalDefaultSession = TelemetrySession.Create(initializerObject);
            flag = false;
          }
        }
      }
      if (flag)
        throw new InvalidOperationException("Unable to create new default Telemetry Session with provided keys.");
      return TelemetryService.InternalDefaultSession;
    }

    public static void AttachTestChannel(ITelemetryTestChannel channel)
    {
      channel.RequiresArgumentNotNull<ITelemetryTestChannel>(nameof (channel));
      GlobalTelemetryTestChannel.Instance.EventPosted += new EventHandler<TelemetryTestChannelEventArgs>(channel.OnPostEvent);
    }

    public static void DetachTestChannel(ITelemetryTestChannel channel)
    {
      channel.RequiresArgumentNotNull<ITelemetryTestChannel>(nameof (channel));
      GlobalTelemetryTestChannel.Instance.EventPosted -= new EventHandler<TelemetryTestChannelEventArgs>(channel.OnPostEvent);
    }

    public static void InitializeEtwProvider(ITelemetryEtwProvider provider)
    {
      provider.RequiresArgumentNotNull<ITelemetryEtwProvider>(nameof (provider));
      TelemetryService.TelemetryEventSource = TelemetryService.TelemetryEventSource == null ? provider : throw new InvalidOperationException("Telemetry ETW provider can only be initialized once.");
    }

    internal static void EnsureEtwProviderInitialized()
    {
      if (TelemetryService.TelemetryEventSource != null)
        return;
      TelemetryService.TelemetryEventSource = (ITelemetryEtwProvider) new TelemetryNullEtwProvider();
    }
  }
}
