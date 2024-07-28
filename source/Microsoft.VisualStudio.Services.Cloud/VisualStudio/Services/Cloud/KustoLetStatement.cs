// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.KustoLetStatement
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class KustoLetStatement : KustoStatement
  {
    private readonly string m_name;
    private readonly string m_expression;
    private readonly FormattableString m_statement;

    public KustoLetStatement(string name, string expression)
    {
      this.m_name = this.CheckAndNormalize(name);
      this.m_expression = this.CheckAndNormalize(expression);
      this.m_statement = FormattableStringFactory.Create("let {0}={1}", (object) this.m_name, (object) this.m_expression);
    }

    public string Name => this.m_name;

    protected override FormattableString Statement => this.m_statement;
  }
}
