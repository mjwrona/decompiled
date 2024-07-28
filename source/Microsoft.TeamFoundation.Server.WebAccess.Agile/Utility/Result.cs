// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility.Result
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility
{
  internal class Result
  {
    [JsonProperty("listValues")]
    public IEnumerable<string> listValues { get; set; }

    [JsonProperty("teamField")]
    public TeamField teamField { get; set; }

    [JsonProperty("fieldName")]
    public string fieldName { get; set; }

    [JsonProperty("url")]
    public string url { get; set; }

    [JsonProperty("userHasTeamWritePermission")]
    public bool userHasTeamWritePermission { get; set; }
  }
}
