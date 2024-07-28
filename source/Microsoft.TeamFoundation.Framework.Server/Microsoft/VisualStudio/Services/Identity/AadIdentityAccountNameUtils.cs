// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.AadIdentityAccountNameUtils
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Identity
{
  public static class AadIdentityAccountNameUtils
  {
    public static string GetSignInAddressFromEncodedUpn(string upn)
    {
      if (string.IsNullOrEmpty(upn) || upn.Length >= 128)
        return (string) null;
      string[] array = ((IEnumerable<string>) Regex.Split(upn, "#EXT#", RegexOptions.IgnoreCase)).Where<string>((Func<string, bool>) (aToken => !string.IsNullOrEmpty(aToken))).ToArray<string>();
      if (array.Length != 2)
        return (string) null;
      string str = array[0];
      int length = str.LastIndexOf('_');
      return length < 0 ? (string) null : str.Substring(0, length) + "@" + str.Substring(length + 1);
    }

    public static string CreateGuestUserPrincipalNamePrefixFromUpn(string upn) => !string.IsNullOrEmpty(upn) ? upn.Replace("@", "_") + "#EXT#" : string.Empty;
  }
}
