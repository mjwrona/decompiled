// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.FilterValidation
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class FilterValidation
  {
    public static void ValidateFilters(List<string> filters, string stringVarName, bool allowEmpty = false)
    {
      if (allowEmpty)
        ArgumentUtility.CheckForNull<List<string>>(filters, stringVarName);
      else
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) filters, stringVarName);
      foreach (string filter in filters)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(filter.Substring(1), stringVarName);
        ArgumentUtility.CheckStringForInvalidCharacters(filter, stringVarName);
      }
    }
  }
}
