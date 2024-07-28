// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.ODataErrorCodeMessage
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [JsonObject(MemberSerialization.OptIn)]
  public class ODataErrorCodeMessage
  {
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("message")]
    public ODataErrorMessage Message { get; set; }

    [JsonProperty("values")]
    public List<ExtendedErrorValue> Values { get; set; }
  }
}
