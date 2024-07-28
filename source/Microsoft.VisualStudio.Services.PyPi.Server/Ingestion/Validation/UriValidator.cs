// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation.UriValidator
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation
{
  public class UriValidator
  {
    private static readonly List<string> ValidSchemes = new List<string>()
    {
      Uri.UriSchemeFtp,
      Uri.UriSchemeGopher,
      Uri.UriSchemeHttp,
      Uri.UriSchemeHttps,
      Uri.UriSchemeMailto,
      Uri.UriSchemeNews,
      Uri.UriSchemeNntp,
      Uri.UriSchemeNetTcp,
      Uri.UriSchemeNetPipe
    };

    public static bool IsValidUri(string uri)
    {
      if (!uri.IsNullOrEmpty<char>() && uri.Equals("UNKNOWN"))
        return true;
      Uri result;
      return Uri.TryCreate(uri, UriKind.Absolute, out result) && UriValidator.ValidSchemes.Any<string>(new Func<string, bool>(result.Scheme.Contains));
    }
  }
}
