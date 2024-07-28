// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.FilterClauseValidator
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  public class FilterClauseValidator
  {
    private static readonly string[] s_allowedOperator_String = new string[4]
    {
      "=",
      "<>",
      "CONTAINS",
      "NOT CONTAINS"
    };
    private static readonly string[] s_allowedOperator_DateTime = new string[6]
    {
      "=",
      ">",
      ">=",
      "<",
      "<=",
      "<>"
    };
    private static readonly string[] s_allowedOperator_Integer = new string[6]
    {
      "=",
      ">",
      ">=",
      "<",
      "<=",
      "<>"
    };
    private static readonly string[] s_allowedOperator_Double = new string[6]
    {
      "=",
      ">",
      ">=",
      "<",
      "<=",
      "<>"
    };
    private static readonly string[] s_allowedOperator_TreePath = new string[6]
    {
      "=",
      "<>",
      "UNDER",
      "NOT UNDER",
      "CONTAINS",
      "NOT CONTAINS"
    };
    private static readonly string[] s_allowedOperator_PlainText_SupportTextQuery = new string[2]
    {
      "CONTAINS WORDS",
      "NOT CONTAINS WORDS"
    };
    private static readonly string[] s_allowedOperator_PlainText_NotSupportTextQuery = new string[2]
    {
      "CONTAINS",
      "NOT CONTAINS"
    };
    private static readonly string[] s_allowedOperator_Guid = new string[2]
    {
      "=",
      "<>"
    };
    private static readonly string[] s_allowedOperator_Boolean = new string[2]
    {
      "=",
      "<>"
    };
    private static readonly string[] s_allowedOperator_Identity = new string[2]
    {
      "=",
      "<>"
    };
    private static readonly IReadOnlyDictionary<InternalFieldType, HashSet<string>> s_allowedTypesAndOperator = (IReadOnlyDictionary<InternalFieldType, HashSet<string>>) new Dictionary<InternalFieldType, HashSet<string>>()
    {
      {
        InternalFieldType.String,
        new HashSet<string>((IEnumerable<string>) new List<string>((IEnumerable<string>) FilterClauseValidator.s_allowedOperator_String), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      },
      {
        InternalFieldType.DateTime,
        new HashSet<string>((IEnumerable<string>) new List<string>((IEnumerable<string>) FilterClauseValidator.s_allowedOperator_DateTime), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      },
      {
        InternalFieldType.Integer,
        new HashSet<string>((IEnumerable<string>) new List<string>((IEnumerable<string>) FilterClauseValidator.s_allowedOperator_Integer), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      },
      {
        InternalFieldType.TreePath,
        new HashSet<string>((IEnumerable<string>) new List<string>((IEnumerable<string>) FilterClauseValidator.s_allowedOperator_TreePath), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      },
      {
        InternalFieldType.PlainText,
        new HashSet<string>((IEnumerable<string>) new List<string>((IEnumerable<string>) FilterClauseValidator.s_allowedOperator_PlainText_SupportTextQuery), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      },
      {
        InternalFieldType.Guid,
        new HashSet<string>((IEnumerable<string>) new List<string>((IEnumerable<string>) FilterClauseValidator.s_allowedOperator_Guid), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      },
      {
        InternalFieldType.Boolean,
        new HashSet<string>((IEnumerable<string>) new List<string>((IEnumerable<string>) FilterClauseValidator.s_allowedOperator_Boolean), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      },
      {
        InternalFieldType.Identity,
        new HashSet<string>((IEnumerable<string>) new List<string>((IEnumerable<string>) FilterClauseValidator.s_allowedOperator_Identity), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      },
      {
        InternalFieldType.Double,
        new HashSet<string>((IEnumerable<string>) new List<string>((IEnumerable<string>) FilterClauseValidator.s_allowedOperator_Double), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      }
    };
    private static readonly string[] s_allowedLogicalOperator = new string[2]
    {
      "AND",
      "OR"
    };
    private static readonly HashSet<string> s_allowedLogicalOperators = new HashSet<string>((IEnumerable<string>) FilterClauseValidator.s_allowedLogicalOperator, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private IVssRequestContext m_requestContext;

    public FilterClauseValidator(IVssRequestContext context) => this.m_requestContext = context;

    public virtual FieldDefinition GetFieldDefinition(FilterClause filterClause)
    {
      ArgumentUtility.CheckForNull<FilterClause>(filterClause, nameof (filterClause));
      FieldDefinition field = (FieldDefinition) null;
      if (!string.IsNullOrEmpty(filterClause.FieldName))
        this.m_requestContext.GetService<WebAccessWorkItemService>().TryGetFieldDefinitionByName(this.m_requestContext, filterClause.FieldName, out field);
      return field;
    }

    public bool isFieldTypeValid(FilterClause filterClause)
    {
      ArgumentUtility.CheckForNull<FilterClause>(filterClause, nameof (filterClause));
      return this.GetFieldDefinition(filterClause) != null;
    }

    public bool isFieldTypeSupported(FilterClause filterClause)
    {
      ArgumentUtility.CheckForNull<FilterClause>(filterClause, nameof (filterClause));
      FieldDefinition fieldDefinition = this.GetFieldDefinition(filterClause);
      return fieldDefinition != null && FilterClauseValidator.s_allowedTypesAndOperator.ContainsKey(fieldDefinition.FieldType);
    }

    public bool isOperatorSupportedForType(FilterClause filterClause)
    {
      FieldDefinition fieldDefinition = this.GetFieldDefinition(filterClause);
      if (fieldDefinition == null)
        return false;
      if (fieldDefinition.FieldType == InternalFieldType.PlainText)
        return new HashSet<string>((IEnumerable<string>) new List<string>(fieldDefinition.SupportsTextQuery ? (IEnumerable<string>) FilterClauseValidator.s_allowedOperator_PlainText_SupportTextQuery : (IEnumerable<string>) FilterClauseValidator.s_allowedOperator_PlainText_NotSupportTextQuery), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Contains(filterClause.Operator);
      return FilterClauseValidator.s_allowedTypesAndOperator.ContainsKey(fieldDefinition.FieldType) && FilterClauseValidator.s_allowedTypesAndOperator[fieldDefinition.FieldType].Contains(filterClause.Operator);
    }

    public bool isValueSupportedForType(FilterClause filterClause)
    {
      FieldDefinition fieldDefinition = this.GetFieldDefinition(filterClause);
      if (fieldDefinition != null)
      {
        switch (fieldDefinition.FieldType)
        {
          case InternalFieldType.Integer:
            if (!int.TryParse(filterClause.Value, out int _))
              return false;
            break;
          case InternalFieldType.PlainText:
            if (string.IsNullOrWhiteSpace(filterClause.Value))
              return false;
            break;
          case InternalFieldType.Double:
            if (!double.TryParse(filterClause.Value, out double _))
              return false;
            break;
        }
      }
      return true;
    }

    public bool isLogicalOperatorSupported(FilterClause filterClause) => string.IsNullOrEmpty(filterClause.LogicalOperator) || FilterClauseValidator.s_allowedLogicalOperators.Contains(filterClause.LogicalOperator);

    public bool isLogicalOperatorAnd(FilterClause filterClause) => string.IsNullOrEmpty(filterClause.LogicalOperator) || filterClause.LogicalOperator.Equals("AND");
  }
}
