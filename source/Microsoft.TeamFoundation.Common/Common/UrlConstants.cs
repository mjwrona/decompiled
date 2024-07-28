// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.UrlConstants
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Common
{
  [GenerateSpecificConstants(null)]
  public class UrlConstants
  {
    [GenerateConstant(null)]
    public static readonly string[] SafeUriSchemes = new string[24]
    {
      "http",
      "https",
      "ftp",
      "gopher",
      "mailto",
      "news",
      "telnet",
      "wais",
      "vstfs",
      "tfs",
      "alm",
      "mtm",
      "mtms",
      "mtr",
      "mtrs",
      "mfbclient",
      "mfbclients",
      "test-runner",
      "x-mvwit",
      "onenote",
      "codeflow",
      "file",
      "tel",
      "skype"
    };
    public static readonly HashSet<string> SafeUriSchemesSet = new HashSet<string>((IEnumerable<string>) UrlConstants.SafeUriSchemes, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  }
}
