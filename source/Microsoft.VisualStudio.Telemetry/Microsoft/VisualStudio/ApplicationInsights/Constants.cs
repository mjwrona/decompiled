// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Constants
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

namespace Microsoft.VisualStudio.ApplicationInsights
{
  internal class Constants
  {
    internal const string TelemetryServiceEndpoint = "https://dc.services.visualstudio.com/v2/track";
    internal const string TelemetryNamePrefix = "Microsoft.ApplicationInsights.";
    internal const string DevModeTelemetryNamePrefix = "Microsoft.ApplicationInsights.Dev.";
    internal const string TelemetryGroup = "{0d943590-b235-5bdb-f854-89520f32fc0b}";
    internal const string DevModeTelemetryGroup = "{ba84f32b-8af2-5006-f147-5030cdd7f22d}";
    internal const string EventSourceGroupTraitKey = "ETW_GROUP";
    internal const int MaxExceptionCountToSave = 10;
    internal const string WebRequestContentType = "application/x-json-stream; charset=utf-8";
    internal const string AiasimovChannelName = "aiasimov";
    internal const string AiVortexChannelName = "aivortex";
    internal const string CollectorChannelName = "collector";
    internal const string CollectorClientVersion = "VSTelemetryAPI";
    internal const string CollectorEndpoint = "https://events.data.microsoft.com/OneCollector/1.0";
    internal const string NewCollectorEndpoint = "https://mobile.events.data.microsoft.com/OneCollector/1.0";
    internal const string VortexEndPoint = "https://vortex.data.microsoft.com/collect/v1";
    internal const string CollectorNoResponseBodyDiagnosticsEnvVar = "VSTelemetryAPI_NoResponseBody";
    internal const bool InitialDefaultUseCollector = false;
  }
}
