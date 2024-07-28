// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.WorkItemFieldDeclaration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations
{
  public class WorkItemFieldDeclaration
  {
    private static readonly HashSet<string> nameSyncedFields = new HashSet<string>()
    {
      "System.AuthorizedAs",
      "System.AssignedTo",
      "System.ChangedBy",
      "System.CreatedBy"
    };

    public WorkItemFieldDeclaration(XElement fieldElement, Action<string> logError)
    {
      this.Name = fieldElement.Attribute((XName) "name")?.Value;
      this.ReferenceName = fieldElement.Attribute((XName) "refname")?.Value;
      this.Type = fieldElement.Attribute((XName) "type")?.Value;
      string str = fieldElement.Attribute((XName) "syncnamechanges")?.Value;
      bool result;
      this.SyncNameChanges = string.IsNullOrEmpty(str) ? WorkItemFieldDeclaration.nameSyncedFields.Contains(this.ReferenceName) : bool.TryParse(str, out result) && result;
      this.Reportable = fieldElement.Attribute((XName) "reportable")?.Value;
      this.Formula = fieldElement.Attribute((XName) "formula")?.Value;
      this.Rules = new WorkItemFieldRuleDeclarations(fieldElement, logError);
    }

    public string Name { get; set; }

    public string ReferenceName { get; set; }

    public string Type { get; set; }

    public bool SyncNameChanges { get; set; }

    public string Reportable { get; set; }

    public string Formula { get; set; }

    public WorkItemFieldRuleDeclarations Rules { get; private set; }
  }
}
