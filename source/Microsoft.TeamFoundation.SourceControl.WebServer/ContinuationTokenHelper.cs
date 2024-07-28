// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.ContinuationTokenHelper
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public sealed class ContinuationTokenHelper
  {
    public static bool TryParse(string encodedToken, out JObject rawTokenAsJObject)
    {
      rawTokenAsJObject = (JObject) null;
      if (string.IsNullOrWhiteSpace(encodedToken))
        return false;
      try
      {
        rawTokenAsJObject = JObject.Parse(ContinuationTokenHelper.DecodeToken(encodedToken));
      }
      catch (Exception ex)
      {
        return false;
      }
      return true;
    }

    public static string DecodeToken(string encodedToken) => Encoding.UTF8.GetString(Convert.FromBase64String(encodedToken));

    public static string EncodeToken(JObject token) => ContinuationTokenHelper.EncodeTokenString(token.ToString(Formatting.None));

    internal static string EncodeTokenString(string tokenString) => Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenString));
  }
}
