// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.ScaledAgileCardSettingsValidator
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.Work.WebApi.Exceptions;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  internal class ScaledAgileCardSettingsValidator
  {
    private const int c_maxAdditionalFieldsCount = 10;
    private static IReadOnlyList<string> CoreFields = (IReadOnlyList<string>) new List<string>()
    {
      CoreFieldReferenceNames.Id,
      CoreFieldReferenceNames.AssignedTo,
      CoreFieldReferenceNames.State,
      CoreFieldReferenceNames.Tags
    };
    private IVssRequestContext m_requestContext;

    internal ScaledAgileCardSettingsValidator(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.m_requestContext = requestContext;
    }

    internal void ValidateAdditionalFields(IEnumerable<string> additionalFields) => this.ValidateAdditionalFieldsInternal(additionalFields, false);

    internal IEnumerable<FieldInfo> GetSanitizedAdditionalFields(
      IEnumerable<string> additionalFields)
    {
      return this.ValidateAdditionalFieldsInternal(additionalFields, true);
    }

    internal IEnumerable<FieldInfo> GetSanitizedCoreFields(IEnumerable<string> coreFields) => this.ValidateAndSanitizefields(coreFields.ToList<string>(), false);

    protected IEnumerable<FieldInfo> ValidateAdditionalFieldsInternal(
      IEnumerable<string> additionalFields,
      bool ignoreInvalidFields)
    {
      if (additionalFields == null || !additionalFields.Any<string>())
        return Enumerable.Empty<FieldInfo>();
      List<string> list = additionalFields.ToList<string>();
      this.CheckDuplicateCoreFields(list);
      this.CheckDuplicateAdditionalFields(list);
      this.ValidateAdditionalFieldCount(list);
      return this.ValidateAndSanitizefields(list, ignoreInvalidFields);
    }

    internal void CheckDuplicateCoreFields(List<string> additionalFields)
    {
      string fieldName = ScaledAgileCardSettingsValidator.CoreFields.Intersect<string>((IEnumerable<string>) additionalFields).FirstOrDefault<string>();
      if (!string.IsNullOrEmpty(fieldName))
        throw new CardSettingsInvalidAdditionalFieldException(fieldName);
    }

    internal void CheckDuplicateAdditionalFields(List<string> additionalFields)
    {
      IGrouping<string, string> grouping = additionalFields.GroupBy<string, string>((Func<string, string>) (x => x), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName).Where<IGrouping<string, string>>((Func<IGrouping<string, string>, bool>) (x => x.Count<string>() > 1)).FirstOrDefault<IGrouping<string, string>>();
      if (grouping != null)
        throw new CardSettingsDuplicateAdditionalFieldsException(grouping.Key);
    }

    internal void ValidateAdditionalFieldCount(List<string> additionalFields)
    {
      if (additionalFields.Count > 10)
        throw new CardSettingsMaxAdditionalFieldsExceededException();
    }

    internal IEnumerable<FieldInfo> ValidateAndSanitizefields(
      List<string> fields,
      bool ignoreInvalidFields)
    {
      IFieldTypeDictionary fieldDictionary = this.GetFieldDictionary();
      List<FieldInfo> fieldInfoList = new List<FieldInfo>();
      foreach (string field in fields)
      {
        FieldEntry validFieldDefinition;
        if (this.TryGetFieldDefinition(field, fieldDictionary, out validFieldDefinition))
        {
          fieldInfoList.Add(new FieldInfo()
          {
            ReferenceName = validFieldDefinition.ReferenceName,
            DisplayName = validFieldDefinition.Name,
            FieldType = this.MapAdditionalFieldType(field, validFieldDefinition.FieldType),
            IsIdentity = validFieldDefinition.IsIdentity
          });
        }
        else
        {
          if (!ignoreInvalidFields)
            throw new CardSettingsInvalidFieldIdentifierException(field);
          fieldInfoList.Add(new FieldInfo()
          {
            ReferenceName = field
          });
        }
      }
      return (IEnumerable<FieldInfo>) fieldInfoList;
    }

    private bool TryGetFieldDefinition(
      string fieldRefName,
      IFieldTypeDictionary validDefinitions,
      out FieldEntry validFieldDefinition)
    {
      return validDefinitions.TryGetFieldByNameOrId(fieldRefName, out validFieldDefinition) && this.isValidCardFieldType(validFieldDefinition.FieldType) && validFieldDefinition.IsQueryable;
    }

    private bool isValidCardFieldType(InternalFieldType fieldType)
    {
      switch (fieldType)
      {
        case InternalFieldType.String:
        case InternalFieldType.Integer:
        case InternalFieldType.DateTime:
        case InternalFieldType.PlainText:
        case InternalFieldType.TreePath:
        case InternalFieldType.Double:
        case InternalFieldType.Boolean:
          return true;
        default:
          return false;
      }
    }

    private FieldType MapAdditionalFieldType(string fieldId, InternalFieldType fieldType)
    {
      FieldType validFieldType;
      if (this.TryMapToAdditionalFieldType(fieldType, out validFieldType))
        return validFieldType;
      throw new CardSettingsInvalidFieldIdentifierException(fieldId);
    }

    private bool TryMapToAdditionalFieldType(
      InternalFieldType fieldType,
      out FieldType validFieldType)
    {
      switch (fieldType)
      {
        case InternalFieldType.String:
          validFieldType = FieldType.String;
          return true;
        case InternalFieldType.Integer:
          validFieldType = FieldType.Integer;
          return true;
        case InternalFieldType.DateTime:
          validFieldType = FieldType.DateTime;
          return true;
        case InternalFieldType.PlainText:
          validFieldType = FieldType.PlainText;
          return true;
        case InternalFieldType.TreePath:
          validFieldType = FieldType.TreePath;
          return true;
        case InternalFieldType.Double:
          validFieldType = FieldType.Double;
          return true;
        case InternalFieldType.Boolean:
          validFieldType = FieldType.Boolean;
          return true;
        default:
          validFieldType = FieldType.String;
          return false;
      }
    }

    private IFieldTypeDictionary GetFieldDictionary() => this.m_requestContext.WitContext().FieldDictionary;
  }
}
