// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.WorkItemTransitionDeclaration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations
{
  public class WorkItemTransitionDeclaration
  {
    private List<WorkItemTransitionReasonDeclaration> reasons = new List<WorkItemTransitionReasonDeclaration>();

    public WorkItemTransitionDeclaration(XElement xmlTransition, Action<string> logError)
    {
      this.From = xmlTransition.Attribute((XName) "from")?.Value ?? "";
      this.To = Utilities.RequireAttribute(xmlTransition, (XName) "to", logError);
      this.Rules = WorkItemFieldRuleDeclarations.Parse(xmlTransition.Element((XName) "FIELDS")?.Elements((XName) "FIELD"), logError);
      XElement xelement = xmlTransition.Element((XName) "REASONS");
      if (xelement == null)
        return;
      foreach (XElement element in xelement.Elements())
        this.reasons.Add(new WorkItemTransitionReasonDeclaration(element, logError));
    }

    public string From { get; set; }

    public string To { get; set; }

    public string DefaultReason { get; set; }

    public IReadOnlyCollection<WorkItemFieldRuleDeclarations> Rules { get; private set; }

    public IReadOnlyCollection<WorkItemTransitionReasonDeclaration> Reasons => (IReadOnlyCollection<WorkItemTransitionReasonDeclaration>) this.reasons;
  }
}
