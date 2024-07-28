// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldDefinition
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class FieldDefinition : IEquatable<FieldDefinition>
  {
    private List<FieldUsage> m_fieldUsages;

    internal FieldDefinition()
    {
    }

    public FieldDefinition(FieldDefinitionRecord record)
    {
      this.Id = record.Id;
      this.Name = record.Name;
      this.ReferenceName = record.ReferenceName;
      this.DBType = (FieldDBType) record.DBType;
      this.IsReportable = record.IsReportable;
      this.ReportingName = record.ReportingName;
      this.ReportingReferenceName = record.ReportingReferenceName;
      this.SupportsTextQuery = record.SupportsTextQuery;
      this.FieldType = FieldDefinition.ConvertFieldType(this.DBType);
      this.SystemType = FieldDefinition.ToSystemType(this.FieldType);
      this.IsIdentity = record.IsIdentity;
      this.IsHistoryEnabled = record.IsHistoryEnabled;
    }

    public FieldDefinition(FieldEntry fieldEntry)
    {
      this.Id = fieldEntry.FieldId;
      this.Name = fieldEntry.Name;
      this.ReferenceName = fieldEntry.ReferenceName;
      this.DBType = (FieldDBType) fieldEntry.FieldDataType;
      this.IsReportable = fieldEntry.IsReportable;
      this.ReportingName = fieldEntry.ReportingName;
      this.ReportingReferenceName = fieldEntry.ReportingReferenceName;
      this.SupportsTextQuery = fieldEntry.SupportsTextQuery;
      this.FieldType = fieldEntry.FieldType;
      this.SystemType = fieldEntry.SystemType;
      this.IsIdentity = fieldEntry.IsIdentity;
      this.IsHistoryEnabled = fieldEntry.IsHistoryEnabled;
      this.Usages = fieldEntry.Usage;
    }

    public virtual int Id { get; private set; }

    public virtual string Name { get; private set; }

    public virtual string ReferenceName { get; private set; }

    public virtual FieldDBType DBType { get; internal set; }

    public virtual InternalFieldType FieldType { get; private set; }

    public virtual Type SystemType { get; private set; }

    public virtual InternalFieldUsages Usages { get; set; }

    public virtual bool IsIdentity { get; private set; }

    public virtual bool IsHistoryEnabled { get; private set; }

    public virtual bool SupportsTextQuery { get; set; }

    public virtual bool CanSortBy
    {
      get
      {
        if (this.Id == -35)
          return false;
        switch (this.DBType & FieldDBType.Double)
        {
          case FieldDBType.Keyword:
          case FieldDBType.Integer:
          case FieldDBType.DateTime:
          case FieldDBType.TreeNode:
          case FieldDBType.Guid:
          case FieldDBType.Bit:
          case FieldDBType.Double:
            if (!this.IsIgnored)
              return true;
            break;
        }
        return false;
      }
    }

    public virtual bool IsComputed => (this.DBType & FieldDBType.Double) == FieldDBType.TreeNode || (this.DBType & FieldDBType.MaskReadOnly) != (FieldDBType) 0;

    public virtual bool IsEditable
    {
      get
      {
        switch ((CoreField) this.Id)
        {
          case CoreField.IterationPath:
          case CoreField.AreaPath:
            return true;
          case CoreField.Id:
          case CoreField.Rev:
          case CoreField.WorkItemType:
          case CoreField.CreatedBy:
            return false;
          default:
            return !this.IsComputed;
        }
      }
    }

    public virtual bool IsIgnored => (this.DBType & FieldDBType.MaskIgnore) != 0;

    public virtual bool IsQueryable
    {
      get
      {
        switch (this.DBType & FieldDBType.Double)
        {
          case FieldDBType.Keyword:
          case FieldDBType.Integer:
          case FieldDBType.DateTime:
          case FieldDBType.LongText:
          case FieldDBType.TreeNode:
          case FieldDBType.Guid:
          case FieldDBType.Bit:
          case FieldDBType.Double:
            if (!this.IsIgnored)
              return true;
            break;
        }
        return false;
      }
    }

    public virtual bool IsReportable { get; private set; }

    public virtual bool IsUserNameField => this.DBType == FieldDBType.Person;

    public virtual bool IsCloneable
    {
      get
      {
        int id = this.Id;
        if (id <= 2)
        {
          if (id <= -7)
          {
            if (id != -105 && id != -7)
              goto label_8;
          }
          else if ((uint) (id - -5) > 2U && id != 2)
            goto label_8;
        }
        else if (id <= 22)
        {
          if ((uint) (id - 8) > 1U && id != 22)
            goto label_8;
        }
        else if (id != 25 && (uint) (id - 32) > 1U && id != 54)
          goto label_8;
        return false;
label_8:
        return this.IsEditable;
      }
    }

    public virtual bool IsLongText
    {
      get
      {
        switch (this.FieldType)
        {
          case InternalFieldType.PlainText:
          case InternalFieldType.Html:
          case InternalFieldType.History:
            return true;
          default:
            return false;
        }
      }
    }

    public virtual FieldFlags Flags
    {
      get
      {
        FieldFlags flags = FieldFlags.None;
        if (this.CanSortBy)
          flags |= FieldFlags.Sortable;
        if (this.IsComputed)
          flags |= FieldFlags.Computed;
        if (this.IsIgnored)
          flags |= FieldFlags.Ignored;
        if (this.IsQueryable)
          flags |= FieldFlags.Queryable;
        if (this.IsReportable)
          flags |= FieldFlags.Reportable;
        if (this.IsUserNameField)
          flags |= FieldFlags.PersonField;
        if (this.IsCloneable)
          flags |= FieldFlags.Cloneable;
        if (this.IsLongText)
          flags |= FieldFlags.LongText;
        if (this.SupportsTextQuery)
          flags |= FieldFlags.SupportsTextQuery;
        return flags;
      }
    }

    public virtual string ReportingName { get; set; }

    public virtual string ReportingReferenceName { get; set; }

    public virtual List<FieldUsage> FieldUsages
    {
      get
      {
        if (this.m_fieldUsages == null)
          this.m_fieldUsages = new List<FieldUsage>();
        return this.m_fieldUsages;
      }
    }

    public virtual bool IsInternal => this.IsIgnored || this.IsInternalFieldType;

    public static bool IsSystemField(string refName)
    {
      ArgumentUtility.CheckForNull<string>(refName, nameof (refName));
      return TFStringComparer.WorkItemFieldReferenceName.StartsWith(refName.Trim(), CoreFieldReferenceNames.SystemFieldPrefix);
    }

    internal bool IsInternalFieldType
    {
      get
      {
        switch (this.DBType & FieldDBType.MaskFieldType)
        {
          case FieldDBType.Keyword:
          case FieldDBType.Person:
          case FieldDBType.Integer:
          case FieldDBType.DateTime:
          case FieldDBType.LongText:
          case FieldDBType.TreeNode:
          case FieldDBType.Guid:
          case FieldDBType.Bit:
          case FieldDBType.Double:
          case FieldDBType.TreePath:
          case FieldDBType.TreeId:
          case FieldDBType.History:
          case FieldDBType.TreeNodeName:
          case FieldDBType.Html:
          case FieldDBType.TreeNodeType:
            return false;
          default:
            return true;
        }
      }
    }

    internal static InternalFieldType ConvertFieldType(FieldDBType fieldDBType)
    {
      InternalFieldType internalFieldType = InternalFieldType.String;
      switch (fieldDBType & FieldDBType.MaskFieldType)
      {
        case FieldDBType.Keyword:
        case FieldDBType.Person:
        case FieldDBType.TreeNode:
        case FieldDBType.TreeNodeName:
        case FieldDBType.TreeNodeType:
          internalFieldType = InternalFieldType.String;
          break;
        case FieldDBType.Integer:
        case FieldDBType.TreeId:
          internalFieldType = InternalFieldType.Integer;
          break;
        case FieldDBType.DateTime:
          internalFieldType = InternalFieldType.DateTime;
          break;
        case FieldDBType.LongText:
          internalFieldType = InternalFieldType.PlainText;
          break;
        case FieldDBType.Guid:
          internalFieldType = InternalFieldType.Guid;
          break;
        case FieldDBType.Bit:
          internalFieldType = InternalFieldType.Boolean;
          break;
        case FieldDBType.Double:
          internalFieldType = InternalFieldType.Double;
          break;
        case FieldDBType.TreePath:
          internalFieldType = InternalFieldType.TreePath;
          break;
        case FieldDBType.History:
          internalFieldType = InternalFieldType.History;
          break;
        case FieldDBType.Html:
          internalFieldType = InternalFieldType.Html;
          break;
      }
      return internalFieldType;
    }

    private static Type ToSystemType(InternalFieldType type)
    {
      switch (type)
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

    public bool Equals(FieldDefinition other)
    {
      if (other == null)
        return false;
      return this == other || this.Id == other.Id;
    }

    public override int GetHashCode() => this.Id.GetHashCode();
  }
}
