// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.DocumentContext
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public class DocumentContext
  {
    private Dictionary<string, string> m_reportedVariables;

    public string Source { get; private set; }

    public string FileContext { get; set; }

    public bool IsGenerated { get; private set; }

    public JSParser Parser { get; set; }

    public DocumentContext(string source) => this.Source = source;

    public DocumentContext Clone() => new DocumentContext(this.Source)
    {
      IsGenerated = this.IsGenerated,
      FileContext = this.FileContext,
      Parser = this.Parser,
      m_reportedVariables = this.m_reportedVariables
    };

    internal void HandleError(ContextError error)
    {
      if (this.Parser == null)
        return;
      this.Parser.OnCompilerError(error);
    }

    internal void ReportUndefined(UndefinedReference referernce)
    {
      if (this.Parser == null)
        return;
      this.Parser.OnUndefinedReference(referernce);
    }

    internal bool HasAlreadySeenErrorFor(string varName)
    {
      if (this.m_reportedVariables == null)
        this.m_reportedVariables = new Dictionary<string, string>();
      else if (this.m_reportedVariables.ContainsKey(varName))
        return true;
      this.m_reportedVariables.Add(varName, varName);
      return false;
    }
  }
}
