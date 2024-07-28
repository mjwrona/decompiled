// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.JsonTelemetryManifestActionConverter
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class JsonTelemetryManifestActionConverter : 
    JsonCreationConverter<ITelemetryManifestAction>
  {
    internal override ITelemetryManifestAction Create(Type objectType, JObject jsonObject)
    {
      if (this.FieldExists("excludeForChannels", jsonObject))
        return (ITelemetryManifestAction) new TelemetryManifestActionExclude();
      if (this.FieldExists("route", jsonObject))
        return (ITelemetryManifestAction) new TelemetryManifestActionRoute();
      if (this.FieldExists("optOutIncludeEvents", jsonObject))
        return (ITelemetryManifestAction) new TelemetryManifestActionOptOutIncludeEvents();
      if (this.FieldExists("optOutExcludeEvents", jsonObject))
        return (ITelemetryManifestAction) new TelemetryManifestActionOptOutExcludeEvents();
      if (this.FieldExists("optOutIncludeProperties", jsonObject))
        return (ITelemetryManifestAction) new TelemetryManifestActionOptOutIncludeProperties();
      if (this.FieldExists("optOutExcludeProperties", jsonObject))
        return (ITelemetryManifestAction) new TelemetryManifestActionOptOutExcludeProperties();
      if (this.FieldExists("throttle", jsonObject))
        return (ITelemetryManifestAction) new TelemetryManifestActionThrottle();
      if (this.FieldExists("doNotThrottle", jsonObject))
        return (ITelemetryManifestAction) new TelemetryManifestActionDoNotThrottle();
      if (this.FieldExists("piiProperties", jsonObject))
        return (ITelemetryManifestAction) new TelemetryManifestActionPii();
      if (this.FieldExists("hashProperties", jsonObject))
        return (ITelemetryManifestAction) new TelemetryManifestActionHashed();
      if (this.FieldExists("credScanProperties", jsonObject))
        return (ITelemetryManifestAction) new TelemetryManifestActionCredScan();
      return this.FieldExists("excludeProperty", jsonObject) ? (ITelemetryManifestAction) new TelemetryManifestActionExcludeProperty() : (ITelemetryManifestAction) new TelemetryManifestInvalidAction();
    }
  }
}
