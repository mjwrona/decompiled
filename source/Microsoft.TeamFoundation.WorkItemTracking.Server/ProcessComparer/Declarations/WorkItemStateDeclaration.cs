// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.WorkItemStateDeclaration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations
{
  public class WorkItemStateDeclaration
  {
    public WorkItemStateDeclaration(XElement xmlState, Action<string> logError)
    {
      this.Name = Utilities.RequireAttribute(xmlState, (XName) "value", logError);
      this.Rules = (IReadOnlyCollection<WorkItemFieldRuleDeclarations>) WorkItemFieldRuleDeclarations.Parse(xmlState.Element((XName) "FIELDS")?.Elements((XName) "FIELD"), logError).Where<WorkItemFieldRuleDeclarations>((Func<WorkItemFieldRuleDeclarations, bool>) (e => e != null)).ToList<WorkItemFieldRuleDeclarations>();
    }

    public string Name { get; set; }

    public IReadOnlyCollection<WorkItemFieldRuleDeclarations> Rules { get; private set; }
  }
}
