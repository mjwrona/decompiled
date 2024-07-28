// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ContentSecurityPolicy.CspReport
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Newtonsoft.Json;

namespace Microsoft.TeamFoundation.Server.WebAccess.ContentSecurityPolicy
{
  internal class CspReport
  {
    [JsonProperty("document-uri")]
    public string DocumentUri { get; set; }

    [JsonProperty("referrer")]
    public string Referrer { get; set; }

    [JsonProperty("effective-directive")]
    public string EffectiveDirective { get; set; }

    [JsonProperty("violated-directive")]
    public string ViolatedDirective { get; set; }

    [JsonProperty("original-policy")]
    public string OriginalPolicy { get; set; }

    [JsonProperty("blocked-uri")]
    public string BlockedUri { get; set; }

    [JsonProperty("source-file")]
    public string SourceFile { get; set; }

    [JsonProperty("line-number")]
    public int LineNumber { get; set; }

    [JsonProperty("column-number")]
    public int ColumnNumber { get; set; }

    [JsonProperty("status-code")]
    public string StatusCode { get; set; }

    public override string ToString() => JsonConvert.SerializeObject((object) this, Formatting.Indented);
  }
}
