// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.UndefinedReference
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

namespace Microsoft.Ajax.Utilities
{
  public sealed class UndefinedReference
  {
    private Context m_context;
    private Lookup m_lookup;
    private string m_name;
    private ReferenceType m_type;

    public AstNode LookupNode => (AstNode) this.m_lookup;

    public string Name => this.m_name;

    public ReferenceType ReferenceType => this.m_type;

    public int Column => this.m_context != null ? this.m_context.StartColumn + 1 : 0;

    public int Line => this.m_context != null ? this.m_context.StartLineNumber : 0;

    internal UndefinedReference(Lookup lookup, Context context)
    {
      this.m_lookup = lookup;
      this.m_name = lookup.Name;
      this.m_type = lookup.RefType;
      this.m_context = context;
    }

    public override string ToString() => this.m_name;
  }
}
