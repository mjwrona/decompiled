// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.BlockRuleDeclaration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations
{
  public abstract class BlockRuleDeclaration : WorkItemRuleDeclaration
  {
    public List<WorkItemRuleDeclaration> Children { get; } = new List<WorkItemRuleDeclaration>();

    public BlockRuleDeclaration()
    {
    }

    public BlockRuleDeclaration(XElement xmlRuleElement, Action<string> logError)
      : base(xmlRuleElement)
    {
      this.Children.AddRange((IEnumerable<WorkItemRuleDeclaration>) WorkItemRuleDeclaration.Parse(xmlRuleElement.Elements(), logError));
    }

    public override bool Equals(
      WorkItemRuleDeclaration other,
      bool deep,
      Func<WorkItemRuleDeclaration, bool> applicable)
    {
      if (base.Equals(other, deep, applicable))
      {
        if (!deep)
          return true;
        if (other is BlockRuleDeclaration blockRuleDeclaration)
          return BlockRuleDeclaration.Equals((IEnumerable<WorkItemRuleDeclaration>) this.Children, (IEnumerable<WorkItemRuleDeclaration>) blockRuleDeclaration.Children, deep, applicable);
      }
      return false;
    }

    public static bool Equals(
      IEnumerable<WorkItemRuleDeclaration> left,
      IEnumerable<WorkItemRuleDeclaration> right,
      bool deep,
      Func<WorkItemRuleDeclaration, bool> applicable)
    {
      if (applicable != null)
      {
        left = (IEnumerable<WorkItemRuleDeclaration>) left.Where<WorkItemRuleDeclaration>((Func<WorkItemRuleDeclaration, bool>) (r => applicable(r))).OrderBy<WorkItemRuleDeclaration, WorkItemRuleDeclaration>((Func<WorkItemRuleDeclaration, WorkItemRuleDeclaration>) (r => r));
        right = (IEnumerable<WorkItemRuleDeclaration>) right.Where<WorkItemRuleDeclaration>((Func<WorkItemRuleDeclaration, bool>) (r => applicable(r))).OrderBy<WorkItemRuleDeclaration, WorkItemRuleDeclaration>((Func<WorkItemRuleDeclaration, WorkItemRuleDeclaration>) (r => r));
      }
      return left.Zip<WorkItemRuleDeclaration, WorkItemRuleDeclaration, bool>(right, (Func<WorkItemRuleDeclaration, WorkItemRuleDeclaration, bool>) ((l, r) => l.Equals(r, deep, applicable))).All<bool>((Func<bool, bool>) (equals => equals));
    }
  }
}
