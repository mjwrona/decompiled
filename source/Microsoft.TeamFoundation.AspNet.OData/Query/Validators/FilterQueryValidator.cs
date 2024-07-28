// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Validators.FilterQueryValidator
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace Microsoft.AspNet.OData.Query.Validators
{
  public class FilterQueryValidator : ExpressionQueryValidator
  {
    public FilterQueryValidator(DefaultQuerySettings defaultQuerySettings)
      : base(defaultQuerySettings)
    {
    }

    public virtual void Validate(
      FilterQueryOption filterQueryOption,
      ODataValidationSettings settings)
    {
      if (filterQueryOption == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (filterQueryOption));
      if (settings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (settings));
      if (filterQueryOption.Context.Path != null)
      {
        this.Property = filterQueryOption.Context.TargetProperty;
        this.StructuredType = filterQueryOption.Context.TargetStructuredType;
      }
      this.Validate(filterQueryOption.FilterClause, settings, filterQueryOption.Context.Model);
    }

    public virtual void Validate(
      FilterClause filterClause,
      ODataValidationSettings settings,
      IEdmModel model)
    {
      this.CurrentAnyAllExpressionDepth = 0;
      this.CurrentNodeCount = 0;
      this.Model = model;
      this.ValidateQueryNode((QueryNode) filterClause.Expression, settings);
    }

    internal virtual void Validate(
      IEdmProperty property,
      IEdmStructuredType structuredType,
      FilterClause filterClause,
      ODataValidationSettings settings,
      IEdmModel model)
    {
      this.Property = property;
      this.StructuredType = structuredType;
      this.Validate(filterClause, settings, model);
    }

    internal static FilterQueryValidator GetFilterQueryValidator(ODataQueryContext context)
    {
      if (context == null)
        return new FilterQueryValidator(new DefaultQuerySettings());
      return context.RequestContainer != null ? context.RequestContainer.GetRequiredService<FilterQueryValidator>() : new FilterQueryValidator(context.DefaultQuerySettings);
    }
  }
}
