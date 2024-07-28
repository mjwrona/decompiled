// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemField
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WorkItemField
  {
    private int? m_reportingType;

    public string Name { get; private set; }

    public string ReferenceName { get; private set; }

    public InternalFieldType Type { get; private set; }

    internal WorkItemField(string referenceName, string name, InternalFieldType type)
    {
      this.Name = name;
      this.ReferenceName = referenceName;
      this.Type = type;
    }

    internal WorkItemField(string referenceName, string name, string type)
      : this(referenceName, name, WorkItemField.TranslateFieldType(type))
    {
    }

    internal bool IsRenameSafe { get; set; }

    internal int ReportingType
    {
      get => this.m_reportingType.HasValue ? this.m_reportingType.Value : -1;
      set => this.m_reportingType = new int?(value);
    }

    internal bool IsReportable => this.m_reportingType.HasValue;

    internal static int TranslateReportability(string value)
    {
      if (VssStringComparer.XmlElement.Equals(value, ProvisionValues.ReportingDimension))
        return 2;
      if (VssStringComparer.XmlElement.Equals(value, ProvisionValues.ReportingMeasure))
        return 1;
      if (VssStringComparer.XmlElement.Equals(value, ProvisionValues.ReportingDetail))
        return 3;
      throw new ArgumentException("Unknown reporting type");
    }

    internal static InternalFieldType TranslateFieldType(string fieldType)
    {
      if (VssStringComparer.XmlElement.Equals(fieldType, ProvisionValues.FieldTypeString))
        return InternalFieldType.String;
      if (VssStringComparer.XmlElement.Equals(fieldType, ProvisionValues.FieldTypeInteger))
        return InternalFieldType.Integer;
      if (VssStringComparer.XmlElement.Equals(fieldType, ProvisionValues.FieldTypeDateTime))
        return InternalFieldType.DateTime;
      if (VssStringComparer.XmlElement.Equals(fieldType, ProvisionValues.FieldTypePlainText))
        return InternalFieldType.PlainText;
      if (VssStringComparer.XmlElement.Equals(fieldType, ProvisionValues.FieldTypeHtml))
        return InternalFieldType.Html;
      if (VssStringComparer.XmlElement.Equals(fieldType, ProvisionValues.FieldTypeTreePath))
        return InternalFieldType.TreePath;
      if (VssStringComparer.XmlElement.Equals(fieldType, ProvisionValues.FieldTypeHistory))
        return InternalFieldType.History;
      if (VssStringComparer.XmlElement.Equals(fieldType, ProvisionValues.FieldTypeDouble))
        return InternalFieldType.Double;
      if (VssStringComparer.XmlElement.Equals(fieldType, ProvisionValues.FieldTypeGuid))
        return InternalFieldType.Guid;
      return VssStringComparer.XmlElement.Equals(fieldType, ProvisionValues.FieldTypeBoolean) ? InternalFieldType.Boolean : InternalFieldType.String;
    }
  }
}
