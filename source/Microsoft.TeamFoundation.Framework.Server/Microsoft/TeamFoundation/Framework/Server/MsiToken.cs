// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MsiToken
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi.Jwt;
using Newtonsoft.Json;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [JsonObject(MemberSerialization.OptIn)]
  public class MsiToken
  {
    [JsonProperty(PropertyName = "access_token")]
    public string AccessToken { get; set; }

    [JsonProperty(PropertyName = "expires_on")]
    [JsonConverter(typeof (UnixEpochDateTimeConverter))]
    public DateTime ExpiresOn { get; set; }

    [JsonProperty(PropertyName = "expires_in")]
    public double ExpiresIn { get; set; }

    [JsonProperty]
    public string Resource { get; set; }

    public static MsiToken Parse(string input)
    {
      try
      {
        return JsonUtilities.Deserialize<MsiToken>(input);
      }
      catch (Exception ex)
      {
        throw new FormatException("Could not parse response token", ex);
      }
    }
  }
}
