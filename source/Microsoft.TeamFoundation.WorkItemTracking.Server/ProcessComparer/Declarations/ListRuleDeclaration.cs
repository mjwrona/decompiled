// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.ListRuleDeclaration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations
{
  public abstract class ListRuleDeclaration : WorkItemRuleDeclaration
  {
    public ListRuleDeclaration(XElement xmlRuleElement, Action<string> logError)
      : base(xmlRuleElement)
    {
      this.ExpandItems = xmlRuleElement.Attribute((XName) "expanditems")?.Value != "false";
      this.ExcludeGroups = xmlRuleElement.Attribute((XName) "filteritems")?.Value == "excludegroups";
      foreach (XElement element in xmlRuleElement.Elements())
      {
        switch (element.Name.LocalName)
        {
          case "LISTITEM":
            string str1 = Utilities.RequireAttribute(element, (XName) "value", logError);
            if (str1 != null)
            {
              this.Items.Add(str1);
              continue;
            }
            continue;
          case "GLOBALLIST":
            string str2 = Utilities.RequireAttribute(element, (XName) "name", logError);
            if (str2 != null)
            {
              this.Sets.Add(str2);
              continue;
            }
            continue;
          default:
            logError(string.Format("Unrecognized list element '{0}'", (object) element.Name));
            continue;
        }
      }
    }

    public bool ExpandItems { get; set; }

    public bool ExcludeGroups { get; set; }

    public ISet<string> Items { get; } = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public ISet<string> Sets { get; } = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public override bool Equals(
      WorkItemRuleDeclaration other,
      bool deep,
      Func<WorkItemRuleDeclaration, bool> applicable)
    {
      if (base.Equals(other, deep, applicable))
      {
        ListRuleDeclaration listRuleDeclaration = other as ListRuleDeclaration;
        if (this.ExpandItems == listRuleDeclaration.ExpandItems && this.ExcludeGroups == listRuleDeclaration.ExcludeGroups && this.Items.SetEquals((IEnumerable<string>) listRuleDeclaration.Items))
          return this.Sets.SetEquals((IEnumerable<string>) listRuleDeclaration.Sets);
      }
      return false;
    }

    public override int CompareTo(WorkItemRuleDeclaration other)
    {
      int num = base.CompareTo(other);
      if (num == 0 && other is ListRuleDeclaration listRuleDeclaration)
      {
        bool flag = this.ExpandItems;
        num = flag.CompareTo(listRuleDeclaration.ExpandItems);
        if (num == 0)
        {
          flag = this.ExcludeGroups;
          num = flag.CompareTo(listRuleDeclaration.ExcludeGroups);
        }
      }
      return num;
    }
  }
}
