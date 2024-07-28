// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.JsonTelemetryManifestMatchValueConverter
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class JsonTelemetryManifestMatchValueConverter : 
    JsonCreationConverter<ITelemetryManifestMatchValue>
  {
    internal override ITelemetryManifestMatchValue Create(Type objectType, JObject jsonObject)
    {
      if (this.FieldExists("eq", jsonObject))
        return (ITelemetryManifestMatchValue) new TelemetryManifestMatchValueEq();
      if (this.FieldExists("lt", jsonObject))
        return (ITelemetryManifestMatchValue) new TelemetryManifestMatchValueLt();
      if (this.FieldExists("gt", jsonObject))
        return (ITelemetryManifestMatchValue) new TelemetryManifestMatchValueGt();
      if (this.FieldExists("exists", jsonObject))
        return (ITelemetryManifestMatchValue) new TelemetryManifestMatchValueExists();
      if (this.FieldExists("startsWith", jsonObject))
        return (ITelemetryManifestMatchValue) new TelemetryManifestMatchValueStartsWith();
      if (this.FieldExists("endsWith", jsonObject))
        return (ITelemetryManifestMatchValue) new TelemetryManifestMatchValueEndsWith();
      return this.FieldExists("contains", jsonObject) ? (ITelemetryManifestMatchValue) new TelemetryManifestMatchValueContains() : (ITelemetryManifestMatchValue) new TelemetryManifestInvalidMatchValueItem();
    }
  }
}
