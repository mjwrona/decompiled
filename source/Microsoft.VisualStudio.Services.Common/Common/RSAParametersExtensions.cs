// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.RSAParametersExtensions
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Security.Cryptography;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class RSAParametersExtensions
  {
    public static RSAParameters FromJsonString(string parameterString)
    {
      ArgumentUtility.CheckForNull<string>(parameterString, nameof (parameterString));
      JObject jobject = JObject.Parse(parameterString);
      return new RSAParameters()
      {
        D = jobject["D"].ToObject<byte[]>(),
        DP = jobject["DP"].ToObject<byte[]>(),
        DQ = jobject["DQ"].ToObject<byte[]>(),
        Exponent = jobject["Exponent"].ToObject<byte[]>(),
        InverseQ = jobject["InverseQ"].ToObject<byte[]>(),
        Modulus = jobject["Modulus"].ToObject<byte[]>(),
        P = jobject["P"].ToObject<byte[]>(),
        Q = jobject["Q"].ToObject<byte[]>()
      };
    }

    public static string ToJsonString(this RSAParameters rsaParameters) => new JObject()
    {
      ["D"] = ((JToken) rsaParameters.D),
      ["DP"] = ((JToken) rsaParameters.DP),
      ["DQ"] = ((JToken) rsaParameters.DQ),
      ["Exponent"] = ((JToken) rsaParameters.Exponent),
      ["InverseQ"] = ((JToken) rsaParameters.InverseQ),
      ["Modulus"] = ((JToken) rsaParameters.Modulus),
      ["P"] = ((JToken) rsaParameters.P),
      ["Q"] = ((JToken) rsaParameters.Q)
    }.ToString();
  }
}
