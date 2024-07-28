// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CloudConnected.CloudConnectedUtilities
// Assembly: Microsoft.VisualStudio.Services.CloudConnected.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6AAFC756-39E6-4247-9102-7DC33B035E4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CloudConnected.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.CloudConnected
{
  public class CloudConnectedUtilities
  {
    public static string EncodeToken(Dictionary<string, string> properties)
    {
      ArgumentUtility.CheckForNull<Dictionary<string, string>>(properties, nameof (properties));
      return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) properties)));
    }

    public static Dictionary<string, string> DecodeToken(string token)
    {
      Dictionary<string, string> dictionary1 = CloudConnectedUtilities.DecodeRawToken(token);
      Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
      foreach (KeyValuePair<string, string> keyValuePair in dictionary1)
      {
        if (CloudConstantsMappingHelper.ConstantsMap.ContainsKey(keyValuePair.Key))
          dictionary2[CloudConstantsMappingHelper.ConstantsMap[keyValuePair.Key]] = keyValuePair.Value;
        else
          dictionary2[keyValuePair.Key] = keyValuePair.Value;
      }
      return dictionary2;
    }

    public static Dictionary<string, string> DecodeRawToken(string token)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      return JsonConvert.DeserializeObject<Dictionary<string, string>>(Encoding.UTF8.GetString(Convert.FromBase64String(token)));
    }
  }
}
