// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.FieldEntry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public abstract class FieldEntry : IEquatable<FieldEntry>
  {
    public FieldEntry() => this.IsHistoryEnabled = true;

    public string Name { get; protected set; }

    public int FieldDataType { get; protected set; }

    public int FieldId { get; protected set; }

    public int ParentFieldId { get; protected set; }

    public bool IsReportable { get; protected set; }

    public bool IsCore { get; protected set; }

    public bool OftenQueriedAsText { get; protected set; }

    public bool SupportsTextQuery { get; protected set; }

    public int ReportingFormula { get; protected set; }

    public int ReportingType { get; protected set; }

    public string ReportingReferenceName { get; protected set; }

    public string ReportingName { get; protected set; }

    public InternalFieldUsages Usage { get; protected set; }

    public Guid ProcessId { get; protected set; }

    public bool? IsInCurrentProject { get; set; }

    public string Description { get; protected set; }

    public Guid? PickListId { get; protected set; }

    public bool IsHistoryEnabled { get; protected set; }

    internal bool IsDeleted { get; set; }

    public bool IsLocked { get; set; }

    public virtual string ReferenceName { get; protected set; }

    public virtual bool IsStoredInWideSchema => this.Usage == InternalFieldUsages.WorkItem && !this.IsLongText;

    public virtual bool IsStoredInLongSchema => !this.IsLongText && !this.IsCore || this.FieldId == 1;

    public virtual bool IsUsedInTrendData => this.Usage == InternalFieldUsages.WorkItemTypeExtension && !this.IsPerson && this.ReportingType == 2;

    public virtual FieldStorageTarget StorageTarget
    {
      get
      {
        if (this.Usage != InternalFieldUsages.WorkItem && this.Usage != InternalFieldUsages.WorkItemTypeExtension)
          return FieldStorageTarget.Unknown;
        if (this.Usage == InternalFieldUsages.WorkItem && this.IsCore && !this.IsLongText)
        {
          if (this.FieldId == 1)
            return FieldStorageTarget.LongTable | FieldStorageTarget.LongTexts;
          return this.FieldId == 90 || this.FieldId == 92 || this.FieldId == 91 ? FieldStorageTarget.LongTable : FieldStorageTarget.WideTable;
        }
        return this.IsLongText ? FieldStorageTarget.LongTexts : FieldStorageTarget.LongTable;
      }
    }

    public virtual bool IsReadOnly => (this.FieldDataType & 1) != 0;

    public virtual bool IsIgnored => (this.FieldDataType & 2) != 0;

    public virtual bool IsPerson => (this.FieldDataType & 8) != 0;

    public virtual bool IsIdentity { get; internal set; }

    public bool IsAreaPath => this.FieldId == -7;

    public bool IsPortfolioProject => this.FieldId == -42;

    public bool IsIterationPath => this.FieldId == -105;

    public bool IsTreeNode => (this.FieldDataType & 240) == 160;

    public virtual bool CanSortBy
    {
      get
      {
        if (this.FieldId == -35)
          return false;
        switch (this.FieldDataType & 240)
        {
          case 16:
          case 32:
          case 48:
          case 208:
          case 224:
          case 240:
            if (!this.IsIgnored)
              return true;
            break;
          case 160:
            return true;
        }
        return false;
      }
    }

    public virtual bool IsQueryable
    {
      get
      {
        switch (this.FieldDataType & 240)
        {
          case 16:
          case 32:
          case 48:
          case 64:
          case 208:
          case 224:
          case 240:
            if (!this.IsIgnored)
              return true;
            break;
          case 160:
            return true;
        }
        return false;
      }
    }

    public virtual bool IsUpdatable
    {
      get
      {
        switch (this.FieldDataType & 240)
        {
          case 16:
          case 32:
          case 48:
          case 208:
          case 224:
          case 240:
            if (!this.IsIgnored && !this.IsReadOnly)
              return true;
            break;
        }
        return false;
      }
    }

    public virtual bool IsComputed => (this.FieldDataType & 240) == 160 || this.IsReadOnly;

    public virtual bool IsCloneable
    {
      get
      {
        switch (this.FieldId)
        {
          case -105:
          case -7:
          case -5:
          case -4:
          case -3:
          case 2:
          case 3:
          case 7:
          case 8:
          case 9:
          case 22:
          case 25:
          case 32:
          case 33:
          case 54:
            return false;
          default:
            return !this.IsComputed;
        }
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

    public virtual bool IsHtml
    {
      get
      {
        switch (this.FieldType)
        {
          case InternalFieldType.Html:
          case InternalFieldType.History:
            return true;
          default:
            return false;
        }
      }
    }

    public bool IsPicklist
    {
      get
      {
        if (FieldEntry.FieldSubTypeIsPicklist(this.FieldDataType))
        {
          Guid? pickListId = this.PickListId;
          if (pickListId.HasValue)
          {
            pickListId = this.PickListId;
            return pickListId.Value != Guid.Empty;
          }
        }
        return false;
      }
    }

    private static bool FieldSubTypeIsPicklist(int fieldDataType) => (fieldDataType & 3840) == 1280;

    public virtual InternalFieldType FieldType => FieldEntry.ConvertToFieldType(this.FieldDataType);

    internal static InternalFieldType ConvertToFieldType(int fieldDataType)
    {
      InternalFieldType fieldType = InternalFieldType.String;
      if (FieldEntry.FieldSubTypeIsPicklist(fieldDataType))
        fieldDataType &= (int) byte.MaxValue;
      switch (fieldDataType & 4088)
      {
        case 16:
        case 24:
        case 160:
        case 528:
        case 784:
          fieldType = InternalFieldType.String;
          break;
        case 32:
        case 288:
          fieldType = InternalFieldType.Integer;
          break;
        case 48:
          fieldType = InternalFieldType.DateTime;
          break;
        case 64:
          fieldType = InternalFieldType.PlainText;
          break;
        case 208:
          fieldType = InternalFieldType.Guid;
          break;
        case 224:
          fieldType = InternalFieldType.Boolean;
          break;
        case 240:
          fieldType = InternalFieldType.Double;
          break;
        case 272:
          fieldType = InternalFieldType.TreePath;
          break;
        case 320:
          fieldType = InternalFieldType.History;
          break;
        case 576:
          fieldType = InternalFieldType.Html;
          break;
      }
      return fieldType;
    }

    public Type SystemType => FieldHelpers.GetFieldSystemType(this.FieldType);

    public SqlDbType SqlType => FieldEntry.ConvertToSqlType(this.FieldType);

    private static SqlDbType ConvertToSqlType(InternalFieldType fieldType)
    {
      switch (fieldType)
      {
        case InternalFieldType.String:
        case InternalFieldType.PlainText:
        case InternalFieldType.Html:
        case InternalFieldType.TreePath:
        case InternalFieldType.History:
          return SqlDbType.NVarChar;
        case InternalFieldType.Integer:
          return SqlDbType.Int;
        case InternalFieldType.DateTime:
          return SqlDbType.DateTime;
        case InternalFieldType.Double:
          return SqlDbType.Float;
        case InternalFieldType.Guid:
          return SqlDbType.UniqueIdentifier;
        case InternalFieldType.Boolean:
          return SqlDbType.Bit;
        default:
          return SqlDbType.NVarChar;
      }
    }

    public FieldFlags Flags
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
        if (this.IsPerson)
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

    public PsFieldDefinitionTypeEnum PsFieldType => (PsFieldDefinitionTypeEnum) (this.FieldDataType & 4088);

    public bool Equals(FieldEntry other) => other != null && this.FieldId == other.FieldId;

    public override bool Equals(object obj) => this.Equals(obj as FieldEntry);

    public override int GetHashCode() => this.FieldId;
  }
}
