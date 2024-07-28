// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.KustoRestrictStatement
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class KustoRestrictStatement : KustoStatement
  {
    private readonly FormattableString m_statement;

    public KustoRestrictStatement(params string[] dataSources)
    {
      ArgumentUtility.CheckEnumerableForEmpty((IEnumerable) dataSources, nameof (dataSources));
      this.m_statement = FormattableStringFactory.Create("restrict access to ({0})", (object) string.Join(",", ((IEnumerable<string>) dataSources).Select<string, string>((Func<string, string>) (ds => this.CheckAndNormalize(ds)))));
    }

    protected override FormattableString Statement => this.m_statement;
  }
}
