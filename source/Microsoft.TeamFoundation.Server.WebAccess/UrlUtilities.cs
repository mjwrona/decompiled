// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.UrlUtilities
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  internal static class UrlUtilities
  {
    private const string ActionPrefix = "_a";

    public static string BuildFragmentAction(
      string actionName,
      params Tuple<string, string>[] nameValuePairs)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(actionName, nameof (actionName));
      StringBuilder stringBuilder = new StringBuilder("#");
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", (object) "_a", (object) actionName);
      if (nameValuePairs != null && nameValuePairs.Length != 0)
      {
        foreach (Tuple<string, string> nameValuePair in nameValuePairs)
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "&{0}={1}", (object) nameValuePair.Item1, (object) Uri.EscapeDataString(nameValuePair.Item2));
      }
      return stringBuilder.ToString();
    }
  }
}
