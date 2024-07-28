// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeExtensionFieldDeclaration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class WorkItemTypeExtensionFieldDeclaration
  {
    public WorkItemTypeExtensionFieldDeclaration() => this.ExtensionScoped = true;

    internal WorkItemTypeExtensionFieldDeclaration(WorkItemTypeExtensionFieldEntry fieldEntry)
    {
      this.ExtensionScoped = fieldEntry.ExtensionScoped;
      this.Name = fieldEntry.LocalName;
      this.ReferenceName = fieldEntry.LocalReferenceName;
      this.Type = (FieldDBType) fieldEntry.Field.FieldDataType;
    }

    public string Name { get; set; }

    public string ReferenceName { get; set; }

    public int ParentFieldId { get; set; }

    public FieldDBType Type { get; set; }

    public bool ExtensionScoped { get; set; }

    public FieldReportingType ReportingType { get; set; }

    internal CustomFieldEntry ToCustomField(Guid extensionId)
    {
      string str1;
      string str2;
      if (this.ExtensionScoped)
      {
        string extensionFieldPrefix = WorkItemTypeExtensionFieldDeclaration.GetExtensionFieldPrefix(extensionId);
        str1 = extensionFieldPrefix + this.Name;
        str2 = extensionFieldPrefix + this.ReferenceName;
      }
      else
      {
        str1 = this.Name;
        str2 = this.ReferenceName;
      }
      return new CustomFieldEntry()
      {
        Name = str1,
        ReferenceName = str2,
        Type = (int) this.Type,
        ReportingType = (int) this.ReportingType,
        ReportingEnabled = this.ReportingType != 0,
        Usage = -99,
        ParentFieldId = this.ParentFieldId
      };
    }

    internal static string GetExtensionFieldPrefix(Guid extensionId) => "WEF_" + extensionId.ToString("N").ToUpperInvariant() + "_";
  }
}
