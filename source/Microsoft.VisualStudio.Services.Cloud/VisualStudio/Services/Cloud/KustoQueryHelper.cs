// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.KustoQueryHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class KustoQueryHelper
  {
    public static readonly string StatementDelimiter = ";";
    private static readonly char[] s_toBeTrimmed = new char[2]
    {
      ';',
      ' '
    };

    public static string Concat(params string[] statements) => string.Join(KustoQueryHelper.StatementDelimiter, ((IEnumerable<string>) statements).Where<string>((Func<string, bool>) (s => !string.IsNullOrWhiteSpace(s))).Select<string, string>((Func<string, string>) (s => s.Trim(KustoQueryHelper.s_toBeTrimmed))));
  }
}
