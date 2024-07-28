// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.WorkItemTypeCategoryDeclaration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations
{
  public class WorkItemTypeCategoryDeclaration
  {
    public WorkItemTypeCategoryDeclaration(XElement xCategory)
    {
      this.Name = xCategory.Attribute((XName) "name")?.Value;
      this.ReferenceName = xCategory.Attribute((XName) "refname")?.Value;
      this.DefaultWorkItemTypeName = xCategory.Element((XName) "DEFAULTWORKITEMTYPE")?.Attribute((XName) "name")?.Value;
      this.WorkItemTypeNames.UnionWith(xCategory.Elements((XName) "WORKITEMTYPE").Select<XElement, string>((Func<XElement, string>) (e => e.Attribute((XName) "name")?.Value)));
      if (string.IsNullOrEmpty(this.DefaultWorkItemTypeName))
        return;
      this.WorkItemTypeNames.Add(this.DefaultWorkItemTypeName);
    }

    public string Name { get; set; }

    public string ReferenceName { get; set; }

    public string DefaultWorkItemTypeName { get; set; }

    public ISet<string> WorkItemTypeNames { get; } = (ISet<string>) new HashSet<string>();
  }
}
