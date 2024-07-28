// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.DetachReferences
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

namespace Microsoft.Ajax.Utilities
{
  public class DetachReferences : TreeVisitor
  {
    private static readonly DetachReferences s_instance = new DetachReferences();

    private DetachReferences()
    {
    }

    public static void Apply(AstNode node) => node?.Accept((IVisitor) DetachReferences.s_instance);

    public static void Apply(params AstNode[] nodes)
    {
      if (nodes == null)
        return;
      foreach (AstNode node in nodes)
        node.Accept((IVisitor) DetachReferences.s_instance);
    }

    public override void Visit(Lookup node) => node?.VariableField?.References.Remove((INameReference) node);
  }
}
