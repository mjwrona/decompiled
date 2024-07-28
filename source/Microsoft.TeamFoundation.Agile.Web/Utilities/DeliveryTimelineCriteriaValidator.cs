// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.DeliveryTimelineCriteriaValidator
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  public class DeliveryTimelineCriteriaValidator
  {
    private static readonly HashSet<InternalFieldType> c_supportedFieldTypes = new HashSet<InternalFieldType>()
    {
      InternalFieldType.String,
      InternalFieldType.Integer,
      InternalFieldType.PlainText,
      InternalFieldType.Guid,
      InternalFieldType.Double,
      InternalFieldType.Boolean
    };
    private static readonly HashSet<string> c_notSupportedFieldReferenceNames = new HashSet<string>()
    {
      "System.Id",
      "System.Title",
      "System.AreaId",
      "System.IterationId",
      "System.Rev",
      "System.RevisedDate",
      "System.NodeName",
      "System.TeamProject",
      "System.Watermark",
      "System.ExternalLinkCount",
      "System.HyperLinkCount",
      "System.RelatedLinkCount",
      "System.AttachedFileCount",
      "System.AuthorizedAs",
      "System.AuthorizedDate",
      "System.Links.LinkType"
    };
    private const int c_maxNumberofConditionsInsideCriteria = 5;
    private List<string> m_errors;
    private FilterModel m_criteria;
    private FilterClauseValidator m_clauseValidator;

    public DeliveryTimelineCriteriaValidator(IVssRequestContext context, FilterModel criteria)
    {
      this.m_clauseValidator = new FilterClauseValidator(context);
      this.m_criteria = criteria;
      this.m_errors = new List<string>();
    }

    public bool HasErrors => this.m_errors.Count > 0;

    public bool Validate() => this.ExecuteValidator((Action) (() => this.ValidateHelper()));

    public FilterModel Sanitize()
    {
      List<FilterClause> filterClauseList = new List<FilterClause>();
      foreach (FilterClause clause in (IEnumerable<FilterClause>) this.m_criteria.Clauses)
      {
        this.ValidateClause(clause);
        if (!this.HasErrors)
        {
          filterClauseList.Add(clause);
          this.m_errors = new List<string>();
        }
      }
      return new FilterModel()
      {
        Clauses = (ICollection<FilterClause>) filterClauseList,
        Groups = this.m_criteria.Groups,
        MaxGroupLevel = this.m_criteria.MaxGroupLevel
      };
    }

    private void ValidateHelper()
    {
      string filterCriteria = JsonConvert.SerializeObject((object) this.m_criteria);
      if (string.IsNullOrEmpty(filterCriteria))
        return;
      this.Assert((Func<bool>) (() => filterCriteria.Length < 2000), Microsoft.TeamFoundation.Agile.Web.Resources.FilterCriteriaTooLongMessage((object) 2000));
      this.Assert((Func<bool>) (() => this.m_criteria.Clauses.Count <= 5), Microsoft.TeamFoundation.Agile.Web.Resources.DTLCriteriaMaxLimitForConditions((object) 5));
      this.Assert((Func<bool>) (() => this.m_criteria.Groups.Count == 0), Microsoft.TeamFoundation.Agile.Web.Resources.DTLCriteriaWIQLGroups());
      foreach (FilterClause clause in (IEnumerable<FilterClause>) this.m_criteria.Clauses)
        this.ValidateClause(clause);
    }

    public string GetUserFriendlyErrorMessage()
    {
      StringBuilder sb = new StringBuilder();
      this.m_errors.ForEach((Action<string>) (e => sb.AppendLine(e)));
      return sb.ToString();
    }

    private void ValidateClause(FilterClause clause)
    {
      this.Assert((Func<bool>) (() => !string.IsNullOrEmpty(clause.FieldName)), Microsoft.TeamFoundation.Agile.Web.Resources.DTLCriteriaMissingField((object) clause.Index));
      this.Assert((Func<bool>) (() => this.m_clauseValidator.isFieldTypeValid(clause)), Microsoft.TeamFoundation.Agile.Web.Resources.DTLCriteriaInvalidField((object) clause.FieldName));
      FieldDefinition fieldDefinition = this.m_clauseValidator.GetFieldDefinition(clause);
      if (fieldDefinition == null)
        return;
      this.Assert((Func<bool>) (() => this.IsFieldTypeSupported(clause)), Microsoft.TeamFoundation.Agile.Web.Resources.DTLCriteriaNotSupportedField((object) fieldDefinition.ReferenceName));
      this.Assert((Func<bool>) (() => this.m_clauseValidator.isOperatorSupportedForType(clause)), Microsoft.TeamFoundation.Agile.Web.Resources.DTLCriteriaInvalidOperator((object) clause.Operator, (object) fieldDefinition.FieldType));
      this.Assert((Func<bool>) (() => this.m_clauseValidator.isValueSupportedForType(clause)), Microsoft.TeamFoundation.Agile.Web.Resources.DTLCriteriaNotSupportedFieldValue((object) clause.Value, (object) fieldDefinition.ReferenceName));
      if (string.IsNullOrEmpty(clause.LogicalOperator))
        return;
      this.Assert((Func<bool>) (() => this.m_clauseValidator.isLogicalOperatorAnd(clause)), Microsoft.TeamFoundation.Agile.Web.Resources.DTLCriteriaInvalidLogicalOperator((object) clause.LogicalOperator));
    }

    private bool IsFieldTypeSupported(FilterClause filterClause)
    {
      ArgumentUtility.CheckForNull<FilterClause>(filterClause, nameof (filterClause));
      FieldDefinition fieldDefinition = this.m_clauseValidator.GetFieldDefinition(filterClause);
      return fieldDefinition != null && DeliveryTimelineCriteriaValidator.c_supportedFieldTypes.Contains(fieldDefinition.FieldType) && !DeliveryTimelineCriteriaValidator.c_notSupportedFieldReferenceNames.Contains(fieldDefinition.ReferenceName) && fieldDefinition.IsQueryable && !fieldDefinition.IsIdentity && fieldDefinition.Usages != InternalFieldUsages.WorkItemTypeExtension;
    }

    private void Assert(Func<bool> evaluator, string errorFormat, params object[] args)
    {
      if (evaluator())
        return;
      this.AddError(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, errorFormat, args));
    }

    private void AddError(string message) => this.m_errors.Add(message);

    private bool ExecuteValidator(Action validator)
    {
      validator();
      return !this.HasErrors;
    }
  }
}
