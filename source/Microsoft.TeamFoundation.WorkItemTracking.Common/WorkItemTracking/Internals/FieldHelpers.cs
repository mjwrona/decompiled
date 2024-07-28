// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.FieldHelpers
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  public static class FieldHelpers
  {
    public static Type GetFieldSystemType(InternalFieldType fieldType)
    {
      switch (fieldType)
      {
        case InternalFieldType.String:
        case InternalFieldType.PlainText:
        case InternalFieldType.Html:
        case InternalFieldType.TreePath:
        case InternalFieldType.History:
          return typeof (string);
        case InternalFieldType.Integer:
          return typeof (int);
        case InternalFieldType.DateTime:
          return typeof (DateTime);
        case InternalFieldType.Double:
          return typeof (double);
        case InternalFieldType.Guid:
          return typeof (Guid);
        case InternalFieldType.Boolean:
          return typeof (bool);
        default:
          return typeof (object);
      }
    }

    public static InternalFieldUsages GetFieldUsage(int objectId)
    {
      switch (objectId)
      {
        case -102:
          return InternalFieldUsages.Tree;
        case -101:
          return InternalFieldUsages.WorkItemLink;
        case -100:
          return InternalFieldUsages.WorkItem;
        case -99:
          return InternalFieldUsages.WorkItemTypeExtension;
        default:
          return InternalFieldUsages.None;
      }
    }

    public static InternalFieldType ConvertToFieldType(FieldDBType fieldDataType)
    {
      InternalFieldType fieldType = InternalFieldType.String;
      switch (fieldDataType & FieldDBType.MaskFieldType)
      {
        case FieldDBType.Keyword:
        case FieldDBType.Person:
        case FieldDBType.TreeNode:
        case FieldDBType.TreeNodeName:
        case FieldDBType.TreeNodeType:
          fieldType = InternalFieldType.String;
          break;
        case FieldDBType.Integer:
        case FieldDBType.TreeId:
          fieldType = InternalFieldType.Integer;
          break;
        case FieldDBType.DateTime:
          fieldType = InternalFieldType.DateTime;
          break;
        case FieldDBType.LongText:
          fieldType = InternalFieldType.PlainText;
          break;
        case FieldDBType.Guid:
          fieldType = InternalFieldType.Guid;
          break;
        case FieldDBType.Bit:
          fieldType = InternalFieldType.Boolean;
          break;
        case FieldDBType.Double:
          fieldType = InternalFieldType.Double;
          break;
        case FieldDBType.TreePath:
          fieldType = InternalFieldType.TreePath;
          break;
        case FieldDBType.History:
          fieldType = InternalFieldType.History;
          break;
        case FieldDBType.Html:
          fieldType = InternalFieldType.Html;
          break;
      }
      return fieldType;
    }

    public static FieldDBType ConvertToFieldDBType(InternalFieldType fieldType)
    {
      FieldDBType fieldDbType = FieldDBType.Keyword;
      switch (fieldType)
      {
        case InternalFieldType.Integer:
          fieldDbType = FieldDBType.Integer;
          break;
        case InternalFieldType.DateTime:
          fieldDbType = FieldDBType.DateTime;
          break;
        case InternalFieldType.PlainText:
          fieldDbType = FieldDBType.LongText;
          break;
        case InternalFieldType.Html:
          fieldDbType = FieldDBType.Html;
          break;
        case InternalFieldType.TreePath:
          fieldDbType = FieldDBType.TreePath;
          break;
        case InternalFieldType.History:
          fieldDbType = FieldDBType.History;
          break;
        case InternalFieldType.Double:
          fieldDbType = FieldDBType.Double;
          break;
        case InternalFieldType.Guid:
          fieldDbType = FieldDBType.Guid;
          break;
        case InternalFieldType.Boolean:
          fieldDbType = FieldDBType.Bit;
          break;
      }
      return fieldDbType;
    }

    public static FieldDBType StringToFieldDBType(string stringType) => FieldHelpers.StringToFieldDBType(stringType, false, false);

    public static FieldDBType StringToFieldDBType(string stringType, bool syncNameChanges) => FieldHelpers.StringToFieldDBType(stringType, syncNameChanges, false);

    public static FieldDBType StringToFieldDBType(
      string stringType,
      bool syncNameChanges,
      bool identityFieldsEnabled)
    {
      if (TFStringComparer.WorkItemFieldType.Equals(stringType, ProvisionValues.FieldTypeIdentity) || TFStringComparer.WorkItemFieldType.Equals(stringType, ProvisionValues.FieldTypeString) & syncNameChanges)
        return FieldDBType.Person;
      if (TFStringComparer.WorkItemFieldType.Equals(stringType, ProvisionValues.FieldTypeString))
        return FieldDBType.Keyword;
      if (TFStringComparer.WorkItemFieldType.Equals(stringType, ProvisionValues.FieldTypeInteger))
        return FieldDBType.Integer;
      if (TFStringComparer.WorkItemFieldType.Equals(stringType, ProvisionValues.FieldTypeDateTime))
        return FieldDBType.DateTime;
      if (TFStringComparer.WorkItemFieldType.Equals(stringType, ProvisionValues.FieldTypePlainText))
        return FieldDBType.LongText;
      if (TFStringComparer.WorkItemFieldType.Equals(stringType, ProvisionValues.FieldTypeHtml))
        return FieldDBType.Html;
      if (TFStringComparer.WorkItemFieldType.Equals(stringType, ProvisionValues.FieldTypeTreePath))
        return FieldDBType.TreePath;
      if (TFStringComparer.WorkItemFieldType.Equals(stringType, ProvisionValues.FieldTypeHistory))
        return FieldDBType.History;
      if (TFStringComparer.WorkItemFieldType.Equals(stringType, ProvisionValues.FieldTypeDouble))
        return FieldDBType.Double;
      if (TFStringComparer.WorkItemFieldType.Equals(stringType, ProvisionValues.FieldTypeGuid))
        return FieldDBType.Guid;
      if (TFStringComparer.WorkItemFieldType.Equals(stringType, ProvisionValues.FieldTypeBoolean))
        return FieldDBType.Bit;
      throw new ArgumentException("FieldTypeInvalid");
    }

    public static int ParseFormulaString(string value)
    {
      if (VssStringComparer.XmlElement.Equals(value, ProvisionValues.FormulaSum))
        return 1;
      if (VssStringComparer.XmlElement.Equals(value, ProvisionValues.FormulaCount))
        return 2;
      if (VssStringComparer.XmlElement.Equals(value, ProvisionValues.FormulaDistinctCount))
        return 3;
      if (VssStringComparer.XmlElement.Equals(value, ProvisionValues.FormulaAvg))
        return 4;
      if (VssStringComparer.XmlElement.Equals(value, ProvisionValues.FormulaMin))
        return 5;
      return VssStringComparer.XmlElement.Equals(value, ProvisionValues.FormulaMax) ? 6 : 0;
    }

    public static int ParseReportabilityString(string value)
    {
      if (VssStringComparer.XmlElement.Equals(value, ProvisionValues.ReportingDimension))
        return 2;
      if (VssStringComparer.XmlElement.Equals(value, ProvisionValues.ReportingMeasure))
        return 1;
      return VssStringComparer.XmlElement.Equals(value, ProvisionValues.ReportingDetail) ? 3 : 0;
    }

    public static bool IsValidFieldValue(Type fieldType, string value)
    {
      if (string.IsNullOrEmpty(value) || fieldType == typeof (string))
        return true;
      if (fieldType == typeof (int))
        return int.TryParse(value, out int _);
      if (fieldType == typeof (double))
        return double.TryParse(value, out double _);
      if (fieldType == typeof (DateTime))
        return DateTime.TryParse(value, out DateTime _);
      if (fieldType == typeof (Guid))
        return Guid.TryParse(value, out Guid _);
      return fieldType == typeof (bool) && bool.TryParse(value, out bool _);
    }
  }
}
