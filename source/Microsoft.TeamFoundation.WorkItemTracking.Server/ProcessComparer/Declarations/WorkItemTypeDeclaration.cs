// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.WorkItemTypeDeclaration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations
{
  public class WorkItemTypeDeclaration
  {
    private List<GlobalListDeclaration> globalLists = new List<GlobalListDeclaration>();
    private List<WorkItemFieldDeclaration> fields = new List<WorkItemFieldDeclaration>();
    private List<WorkItemStateDeclaration> states = new List<WorkItemStateDeclaration>();
    private List<WorkItemTransitionDeclaration> transitions = new List<WorkItemTransitionDeclaration>();

    protected WorkItemTypeDeclaration()
    {
    }

    public WorkItemTypeDeclaration(XElement xtype, Action<string> logError)
    {
      this.Name = xtype.Attribute((XName) "name").Value;
      this.ReferenceName = xtype.Attribute((XName) "refname")?.Value;
      this.Description = xtype.Element((XName) "DESCRIPTION")?.Value;
      this.FormXml = xtype.Element((XName) "FORM")?.ToString();
      XElement xelement1 = xtype.Element((XName) "FIELDS");
      if (xelement1 != null)
      {
        foreach (XElement element in xelement1.Elements((XName) "FIELD"))
          this.fields.Add(new WorkItemFieldDeclaration(element, logError));
      }
      XElement xelement2 = xtype.Element((XName) "WORKFLOW");
      if (xelement2 != null)
      {
        XElement xelement3 = xelement2.Element((XName) "STATES");
        if (xelement3 != null)
        {
          foreach (XElement element in xelement3.Elements((XName) "STATE"))
            this.states.Add(new WorkItemStateDeclaration(element, logError));
        }
        XElement xelement4 = xelement2.Element((XName) "TRANSITIONS");
        if (xelement4 != null)
        {
          foreach (XElement element in xelement4.Elements((XName) "TRANSITION"))
            this.transitions.Add(new WorkItemTransitionDeclaration(element, logError));
        }
      }
      this.globalLists = GlobalListDeclaration.ReadGlobalLists(xtype.Element((XName) "GLOBALLISTS"), logError);
    }

    public string Name { get; set; }

    public string ReferenceName { get; set; }

    public string Description { get; set; }

    public string FormXml { get; set; }

    public IReadOnlyCollection<WorkItemFieldDeclaration> Fields => (IReadOnlyCollection<WorkItemFieldDeclaration>) this.fields;

    public IReadOnlyCollection<WorkItemStateDeclaration> States => (IReadOnlyCollection<WorkItemStateDeclaration>) this.states;

    public IReadOnlyCollection<WorkItemTransitionDeclaration> Transitions => (IReadOnlyCollection<WorkItemTransitionDeclaration>) this.transitions;

    public IReadOnlyCollection<GlobalListDeclaration> GlobalLists => (IReadOnlyCollection<GlobalListDeclaration>) this.globalLists;
  }
}
