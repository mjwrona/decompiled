// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.WhenRuleDeclaration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations
{
  public class WhenRuleDeclaration : EqualityRuleDeclaration
  {
    public WhenRuleDeclaration(string field, string value)
      : base(field, value)
    {
    }

    public WhenRuleDeclaration(XElement xmlRuleElement, Action<string> logError)
      : base(xmlRuleElement, logError)
    {
    }

    public override WorkItemRuleName Name => WorkItemRuleName.When;
  }
}
