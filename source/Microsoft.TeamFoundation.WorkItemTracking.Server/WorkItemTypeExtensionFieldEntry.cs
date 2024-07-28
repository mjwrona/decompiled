// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeExtensionFieldEntry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class WorkItemTypeExtensionFieldEntry
  {
    internal WorkItemTypeExtensionFieldEntry(Guid extensionId, FieldEntry fieldEntry)
    {
      if (fieldEntry.Usage == InternalFieldUsages.WorkItemTypeExtension)
      {
        this.ExtensionScoped = true;
        string extensionFieldPrefix = WorkItemTypeExtensionFieldDeclaration.GetExtensionFieldPrefix(extensionId);
        this.LocalName = fieldEntry.Name.Substring(extensionFieldPrefix.Length);
        this.LocalReferenceName = fieldEntry.ReferenceName.Substring(extensionFieldPrefix.Length);
      }
      else
      {
        this.LocalName = fieldEntry.Name;
        this.LocalReferenceName = fieldEntry.ReferenceName;
      }
      this.Field = fieldEntry;
    }

    public bool ExtensionScoped { get; set; }

    public string LocalName { get; private set; }

    public string LocalReferenceName { get; private set; }

    public string ReportingName { get; private set; }

    public string ReportingReferenceName { get; private set; }

    public FieldEntry Field { get; private set; }

    public WorkItemTypeExtensionFieldDeclaration ToDeclaration() => new WorkItemTypeExtensionFieldDeclaration(this);
  }
}
