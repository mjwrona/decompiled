// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.JsonHelpers.JsonHelperExtensions
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json.Linq;

namespace Microsoft.VisualStudio.Telemetry.JsonHelpers
{
  internal static class JsonHelperExtensions
  {
    public static bool FieldExists(this JObject jObject, string fieldName) => jObject != null && jObject[fieldName] != null;

    public static ITelemetryManifestMatch CreateTelemetryManifestMatch(this JObject jObject)
    {
      if (jObject.FieldExists("event"))
        return (ITelemetryManifestMatch) new TelemetryManifestMatchEventName();
      if (jObject.FieldExists("property"))
        return (ITelemetryManifestMatch) new TelemetryManifestMatchPropertyValue();
      if (jObject.FieldExists("and"))
        return (ITelemetryManifestMatch) new TelemetryManifestMatchAnd();
      if (jObject.FieldExists("or"))
        return (ITelemetryManifestMatch) new TelemetryManifestMatchOr();
      if (jObject.FieldExists("not"))
        return (ITelemetryManifestMatch) new TelemetryManifestMatchNot();
      return jObject.FieldExists("samplingRate") ? (ITelemetryManifestMatch) new TelemetryManifestMatchSampling() : (ITelemetryManifestMatch) new TelemetryManifestInvalidMatchItem();
    }
  }
}
