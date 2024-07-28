// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.KustoTabularExpressionStatement
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class KustoTabularExpressionStatement : KustoStatement
  {
    private readonly string m_dataSource;
    private readonly List<string> m_operators = new List<string>();

    public KustoTabularExpressionStatement(string dataSource, params string[] operators)
    {
      this.m_dataSource = this.CheckAndNormalize(dataSource);
      foreach (string opr in operators)
        this.AppendOperator(opr);
    }

    protected override FormattableString Statement => this.m_operators.Any<string>() ? FormattableStringFactory.Create("{0}|{1}", (object) this.m_dataSource, (object) string.Join("|", (IEnumerable<string>) this.m_operators)) : FormattableStringFactory.Create("{0}", (object) this.m_dataSource);

    public void AppendOperator(string opr) => this.m_operators.Add(this.CheckAndNormalize(opr));
  }
}
