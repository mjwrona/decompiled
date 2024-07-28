// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.QuerySetting
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Newtonsoft.Json;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class QuerySetting
  {
    [JsonProperty("flat")]
    public string FlatQuery { get; set; }

    [JsonProperty("link")]
    public string LinkQuery { get; set; }

    [JsonProperty("tree ")]
    public string TreeQuery { get; set; }

    [JsonProperty("defaultMode")]
    public string DefaultMode { get; set; }
  }
}
