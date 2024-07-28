// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.WorkItemFieldRuleDeclarations
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations
{
  public class WorkItemFieldRuleDeclarations
  {
    public List<WorkItemRuleDeclaration> Children { get; } = new List<WorkItemRuleDeclaration>();

    private WorkItemFieldRuleDeclarations(string field, IEnumerable<WorkItemRuleDeclaration> rules)
    {
      this.Field = field;
      if (rules == null)
        return;
      this.Children.AddRange(rules);
    }

    public WorkItemFieldRuleDeclarations(XElement xmlFieldElement, Action<string> logError)
    {
      this.Field = Utilities.RequireAttribute(xmlFieldElement, (XName) "refname", logError);
      this.Children.AddRange((IEnumerable<WorkItemRuleDeclaration>) WorkItemRuleDeclaration.Parse(xmlFieldElement.Elements(), logError));
    }

    public string Field { get; set; }

    public static IReadOnlyCollection<WorkItemFieldRuleDeclarations> Parse(
      IEnumerable<XElement> elements,
      Action<string> logError)
    {
      return elements == null ? (IReadOnlyCollection<WorkItemFieldRuleDeclarations>) Array.Empty<WorkItemFieldRuleDeclarations>() : (IReadOnlyCollection<WorkItemFieldRuleDeclarations>) elements.Select<XElement, WorkItemFieldRuleDeclarations>((Func<XElement, WorkItemFieldRuleDeclarations>) (e => new WorkItemFieldRuleDeclarations(e, logError))).GroupBy<WorkItemFieldRuleDeclarations, string>((Func<WorkItemFieldRuleDeclarations, string>) (r => r.Field)).Select<IGrouping<string, WorkItemFieldRuleDeclarations>, WorkItemFieldRuleDeclarations>((Func<IGrouping<string, WorkItemFieldRuleDeclarations>, WorkItemFieldRuleDeclarations>) (g => g.Count<WorkItemFieldRuleDeclarations>() == 1 ? g.First<WorkItemFieldRuleDeclarations>() : new WorkItemFieldRuleDeclarations(g.Key, g.SelectMany<WorkItemFieldRuleDeclarations, WorkItemRuleDeclaration>((Func<WorkItemFieldRuleDeclarations, IEnumerable<WorkItemRuleDeclaration>>) (rd => (IEnumerable<WorkItemRuleDeclaration>) rd.Children))))).ToArray<WorkItemFieldRuleDeclarations>();
    }

    public bool Equals(WorkItemFieldRuleDeclarations other) => this.Equals(other, true, (Func<WorkItemRuleDeclaration, bool>) null);

    public bool Equals(
      WorkItemFieldRuleDeclarations other,
      bool deep,
      Func<WorkItemRuleDeclaration, bool> applicable)
    {
      if (other == null || !StringComparer.OrdinalIgnoreCase.Equals(this.Field, other.Field))
        return false;
      return !deep || BlockRuleDeclaration.Equals((IEnumerable<WorkItemRuleDeclaration>) this.Children, (IEnumerable<WorkItemRuleDeclaration>) other.Children, deep, applicable);
    }
  }
}
