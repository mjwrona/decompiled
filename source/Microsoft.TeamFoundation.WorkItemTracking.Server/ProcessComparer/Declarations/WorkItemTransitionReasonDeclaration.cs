// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.WorkItemTransitionReasonDeclaration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations
{
  public class WorkItemTransitionReasonDeclaration
  {
    public WorkItemTransitionReasonDeclaration(XElement xmlReason, Action<string> logError)
    {
      switch (xmlReason.Name.LocalName)
      {
        case "DEFAULTREASON":
          this.IsDefault = true;
          goto case "REASON";
        case "REASON":
          this.Name = Utilities.RequireAttribute(xmlReason, (XName) "value", logError);
          this.Rules = WorkItemFieldRuleDeclarations.Parse(xmlReason.Element((XName) "FIELDS")?.Elements((XName) "FIELD"), logError);
          break;
        default:
          logError("Unknown xml element '" + xmlReason.Name.LocalName + "'.");
          break;
      }
    }

    public string Name { get; set; }

    public bool IsDefault { get; set; }

    public IReadOnlyCollection<WorkItemFieldRuleDeclarations> Rules { get; private set; }
  }
}
