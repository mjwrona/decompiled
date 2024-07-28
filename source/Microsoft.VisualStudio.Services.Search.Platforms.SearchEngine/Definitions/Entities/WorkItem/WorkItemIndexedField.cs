// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem.WorkItemIndexedField
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem
{
  public class WorkItemIndexedField
  {
    private static readonly IReadOnlyDictionary<string, WorkItemContract.FieldType> s_stringTypeToPlatformFieldTypeMap = (IReadOnlyDictionary<string, WorkItemContract.FieldType>) WorkItemContract.PlatformFieldTypeStringMap.ToDictionary<KeyValuePair<WorkItemContract.FieldType, string>, string, WorkItemContract.FieldType>((Func<KeyValuePair<WorkItemContract.FieldType, string>, string>) (kvp => kvp.Value), (Func<KeyValuePair<WorkItemContract.FieldType, string>, WorkItemContract.FieldType>) (kvp => kvp.Key));
    private static readonly Regex s_platformWitFieldNameRegex = new Regex("^fields\\.(?<type>[a-z]+)\\|(?<core>[^\\|]+)\\|(?<metadata>[^\\$]*)\\$(?<suffix>(\\.[a-z]+)?)$", RegexOptions.Compiled);
    private static readonly Regex s_platformNonAnalyzedWitFieldNameRegex = new Regex("^nonAnalyzedFields\\.(?<type>[a-z]+)\\|(?<core>[^\\|]+)\\|(?<metadata>[^\\$]*)\\$", RegexOptions.Compiled);
    private static readonly Regex s_platformInternalFieldNameRegex = new Regex("^(?<name>[a-zA-Z]+)(?<suffix>(\\.[a-z]+)?)$", RegexOptions.Compiled);
    private static readonly Regex s_platformCompositeFieldNameRegex = new Regex("^fields\\.(?<type>[a-z]+)(?<suffix>(\\.[a-z]+)?)$", RegexOptions.Compiled);
    internal static readonly IReadOnlyDictionary<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType, WorkItemContract.FieldType> TfsToSearchPlatformFieldTypeMapping = (IReadOnlyDictionary<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType, WorkItemContract.FieldType>) new Dictionary<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType, WorkItemContract.FieldType>()
    {
      [Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.Boolean] = WorkItemContract.FieldType.Boolean,
      [Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.DateTime] = WorkItemContract.FieldType.DateTime,
      [Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.PicklistDouble] = WorkItemContract.FieldType.Real,
      [Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.Double] = WorkItemContract.FieldType.Real,
      [Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.History] = WorkItemContract.FieldType.Html,
      [Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.Html] = WorkItemContract.FieldType.Html,
      [Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.Integer] = WorkItemContract.FieldType.Integer,
      [Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.PicklistInteger] = WorkItemContract.FieldType.Integer,
      [Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.TreePath] = WorkItemContract.FieldType.Path,
      [Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.Guid] = WorkItemContract.FieldType.String,
      [Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.PlainText] = WorkItemContract.FieldType.String,
      [Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.String] = WorkItemContract.FieldType.String,
      [Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.PicklistString] = WorkItemContract.FieldType.String,
      [Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.Identity] = WorkItemContract.FieldType.String
    };
    private string m_coreName;
    private string m_metadataName;
    private string m_contractFieldName;
    private string m_platformFieldName;
    private string m_compositeContractFieldName;
    private string m_compositePlatformFieldName;

    public static WorkItemIndexedField FromWitField(
      string referenceName,
      WorkItemContract.FieldType type)
    {
      return new WorkItemIndexedField(referenceName, type);
    }

    public static WorkItemIndexedField FromWitField(WorkItemField witField) => new WorkItemIndexedField(witField);

    public static WorkItemIndexedField FromPlatformFieldName(string platformFieldName) => new WorkItemIndexedField(platformFieldName);

    private WorkItemIndexedField(WorkItemField workItemField)
      : this(workItemField.ReferenceName, WorkItemIndexedField.TfsToSearchPlatformFieldTypeMapping[workItemField.Type])
    {
    }

    private WorkItemIndexedField(string referenceName, WorkItemContract.FieldType type)
    {
      this.IsWitField = true;
      this.IsCompositeField = false;
      this.IsInternalField = false;
      this.Type = type;
      this.ReferenceName = referenceName.ToLowerInvariant();
    }

    private WorkItemIndexedField(string platformFieldName)
    {
      Match match1 = !string.IsNullOrWhiteSpace(platformFieldName) ? WorkItemIndexedField.s_platformWitFieldNameRegex.Match(platformFieldName) : throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Platform field name cannot be null or whitespace")), nameof (platformFieldName));
      Match match2 = WorkItemIndexedField.s_platformNonAnalyzedWitFieldNameRegex.Match(platformFieldName);
      if (match1.Success)
      {
        this.IsWitField = true;
        this.IsCompositeField = false;
        this.IsInternalField = false;
        string key = match1.Groups["type"].Value;
        WorkItemContract.FieldType fieldType;
        if (!WorkItemIndexedField.s_stringTypeToPlatformFieldTypeMap.TryGetValue(key, out fieldType))
          throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Platform field name [{0}] has invalid field type [{1}].", (object) platformFieldName, (object) key)));
        this.Type = fieldType;
        this.CoreName = match1.Groups["core"].Value;
        this.MetadataName = match1.Groups["metadata"].Value.Replace('>', '.');
        this.Suffix = match1.Groups["suffix"].Value;
        string str;
        if (string.IsNullOrEmpty(this.MetadataName))
          str = this.CoreName;
        else
          str = FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) this.MetadataName, (object) this.CoreName));
        this.ReferenceName = str;
      }
      else if (match2.Success)
      {
        this.IsWitField = true;
        this.IsCompositeField = false;
        this.IsInternalField = false;
        string key = match2.Groups["type"].Value;
        WorkItemContract.FieldType fieldType;
        if (!WorkItemIndexedField.s_stringTypeToPlatformFieldTypeMap.TryGetValue(key, out fieldType))
          throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Non-analyzed platform field name [{0}] has invalid field type [{1}].", (object) platformFieldName, (object) key)));
        this.Type = fieldType;
        this.CoreName = match2.Groups["core"].Value;
        this.MetadataName = match2.Groups["metadata"].Value.Replace('>', '.');
        string str;
        if (string.IsNullOrEmpty(this.MetadataName))
          str = this.CoreName;
        else
          str = FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) this.MetadataName, (object) this.CoreName));
        this.ReferenceName = str;
      }
      else
      {
        Match match3 = WorkItemIndexedField.s_platformCompositeFieldNameRegex.Match(platformFieldName);
        if (match3.Success)
        {
          this.IsWitField = false;
          this.IsCompositeField = true;
          this.IsInternalField = false;
          this.m_compositePlatformFieldName = platformFieldName;
          string key = match3.Groups["type"].Value;
          WorkItemContract.FieldType fieldType;
          if (!WorkItemIndexedField.s_stringTypeToPlatformFieldTypeMap.TryGetValue(key, out fieldType))
            throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Platform field name [{0}] has invalid field type [{1}].", (object) platformFieldName, (object) key)));
          this.Type = fieldType;
          this.Suffix = match3.Groups["suffix"].Value;
        }
        else
        {
          Match match4 = WorkItemIndexedField.s_platformInternalFieldNameRegex.Match(platformFieldName);
          if (match4.Success)
          {
            this.IsWitField = false;
            this.IsCompositeField = false;
            this.IsInternalField = true;
            this.Suffix = match4.Groups["suffix"].Value;
          }
          else
            throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Platform field name [{0}] is invalid.", (object) platformFieldName)));
        }
      }
      this.m_platformFieldName = platformFieldName;
    }

    public WorkItemContract.FieldType Type { get; }

    public bool IsWitField { get; }

    public bool IsInternalField { get; }

    public bool IsCompositeField { get; }

    public bool IsCompositeFieldEligible => this.IsWitField && !this.ReferenceName.Equals("System.History", StringComparison.OrdinalIgnoreCase) && this.Type != WorkItemContract.FieldType.IntegerAsString && this.Type != WorkItemContract.FieldType.AllTypes && this.Type != 0;

    public bool ShouldBeIndexedAsString => this.IsWitField && this.Type == WorkItemContract.FieldType.Integer;

    public bool IsEligibleForNonAnalyzedIndex
    {
      get
      {
        if (!this.IsWitField)
          return false;
        if (this.Type.Equals((object) WorkItemContract.FieldType.String) && (this.ReferenceName.Equals("System.WorkItemType", StringComparison.OrdinalIgnoreCase) || this.ReferenceName.Equals("System.State", StringComparison.OrdinalIgnoreCase) || this.ReferenceName.Equals("System.AssignedTo", StringComparison.OrdinalIgnoreCase) || this.ReferenceName.Equals("System.Title", StringComparison.OrdinalIgnoreCase) || this.ReferenceName.Equals("System.Tags", StringComparison.OrdinalIgnoreCase)) || this.Type.Equals((object) WorkItemContract.FieldType.Identity) && this.ReferenceName.Equals("System.AssignedTo", StringComparison.OrdinalIgnoreCase))
          return true;
        return this.Type.Equals((object) WorkItemContract.FieldType.Name) && this.ReferenceName.Equals("System.AssignedTo", StringComparison.OrdinalIgnoreCase);
      }
    }

    public string CoreName
    {
      get
      {
        if (this.IsWitField && this.m_coreName == null)
        {
          int length = this.ReferenceName.LastIndexOf('.');
          if (length < 0)
          {
            this.m_coreName = this.ReferenceName;
            this.m_metadataName = string.Empty;
          }
          else
          {
            this.m_coreName = this.ReferenceName.Substring(length + 1);
            this.m_metadataName = this.ReferenceName.Substring(0, length);
          }
        }
        return this.m_coreName;
      }
      private set => this.m_coreName = value;
    }

    public string MetadataName
    {
      get
      {
        if (this.IsWitField && this.m_metadataName == null)
        {
          int length = this.ReferenceName.LastIndexOf('.');
          if (length < 0)
          {
            this.m_coreName = this.ReferenceName;
            this.m_metadataName = string.Empty;
          }
          else
          {
            this.m_coreName = this.ReferenceName.Substring(length + 1);
            this.m_metadataName = this.ReferenceName.Substring(0, length);
          }
        }
        return this.m_metadataName;
      }
      private set => this.m_metadataName = value;
    }

    public string Suffix { get; } = string.Empty;

    public string CompositeContractFieldName
    {
      get
      {
        if ((this.IsCompositeFieldEligible || this.IsCompositeField) && this.m_compositeContractFieldName == null)
          this.m_compositeContractFieldName = FormattableString.Invariant(FormattableStringFactory.Create("{0}{1}", (object) WorkItemContract.PlatformFieldTypeStringMap[this.Type], (object) this.Suffix));
        return this.m_compositeContractFieldName;
      }
    }

    public string CompositePlatformFieldName
    {
      get
      {
        if ((this.IsCompositeFieldEligible || this.IsCompositeField) && this.m_compositePlatformFieldName == null)
          this.m_compositePlatformFieldName = FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) "fields", (object) this.CompositeContractFieldName));
        return this.m_compositePlatformFieldName;
      }
    }

    public string ContractFieldName
    {
      get
      {
        if (this.m_contractFieldName == null)
        {
          if (this.IsWitField)
            this.m_contractFieldName = FormattableString.Invariant(FormattableStringFactory.Create("{0}|{1}|{2}${3}", (object) WorkItemContract.PlatformFieldTypeStringMap[this.Type], (object) this.CoreName, (object) this.MetadataName.Replace('.', '>'), (object) this.Suffix));
          else if (this.IsCompositeField)
            this.m_contractFieldName = this.CompositeContractFieldName;
          else if (this.IsInternalField)
            this.m_contractFieldName = this.PlatformFieldName;
        }
        return this.m_contractFieldName;
      }
    }

    public string PlatformFieldName
    {
      get
      {
        if (this.m_platformFieldName == null)
        {
          string str;
          if (!this.IsWitField)
            str = this.ContractFieldName;
          else
            str = FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) "fields", (object) this.ContractFieldName));
          this.m_platformFieldName = str;
        }
        return this.m_platformFieldName;
      }
    }

    public string NonAnalyzedPlatformFieldName
    {
      get
      {
        if (this.m_platformFieldName == null)
        {
          string str;
          if (!this.IsWitField)
            str = this.ContractFieldName;
          else
            str = FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) "nonAnalyzedFields", (object) this.ContractFieldName));
          this.m_platformFieldName = str;
        }
        return this.m_platformFieldName;
      }
    }

    public WorkItemIndexedField AsStringIndexedField
    {
      get
      {
        if (!this.ShouldBeIndexedAsString)
          return (WorkItemIndexedField) null;
        if (this.Type == WorkItemContract.FieldType.Integer)
          return WorkItemIndexedField.FromWitField(this.ReferenceName, WorkItemContract.FieldType.IntegerAsString);
        throw new NotSupportedException("Fields of type non-integer are not supported to be indexed as string.");
      }
    }

    public string ReferenceName { get; }
  }
}
