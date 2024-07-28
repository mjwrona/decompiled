// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.JsonTelemetryManifestParser
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class JsonTelemetryManifestParser : ITelemetryManifestParser, IStreamParser
  {
    public async Task<object> ParseAsync(TextReader stream) => (object) this.Parse(await stream.ReadToEndAsync().ConfigureAwait(false));

    public TelemetryManifest Parse(string jsonString)
    {
      try
      {
        return JsonConvert.DeserializeObject<TelemetryManifest>(jsonString, (JsonConverter) new JsonTelemetryManifestMatchConverter(), (JsonConverter) new JsonTelemetryManifestMatchValueConverter(), (JsonConverter) new JsonTelemetryManifestActionConverter(), (JsonConverter) new JsonTelemetryManifestRouteArgsConverter());
      }
      catch (Exception ex)
      {
        throw new TelemetryManifestParserException("there was error in parsing manifest file", ex);
      }
    }
  }
}
