// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.AadQueryUtils
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Aad
{
  internal class AadQueryUtils
  {
    internal static int CountTerms(params IEnumerable<string>[] enumerables)
    {
      int num = 0;
      foreach (IEnumerable<string> enumerable in enumerables)
        num += enumerable != null ? enumerable.Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x))).Count<string>() : 0;
      return num;
    }

    internal static string SanitizeInput(string input) => input.Replace("'", "''");

    internal static string ToString(IEnumerable<string> enumerable) => enumerable != null ? "[" + string.Join(",", enumerable) + "]" : "[]";
  }
}
