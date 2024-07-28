// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Validators.ComputeQueryValidator
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace Microsoft.AspNet.OData.Query.Validators
{
  public class ComputeQueryValidator : ExpressionQueryValidator
  {
    public ComputeQueryValidator(DefaultQuerySettings defaultQuerySettings)
      : base(defaultQuerySettings)
    {
    }

    public virtual void Validate(
      ComputeQueryOption computeQueryOption,
      ODataValidationSettings settings)
    {
      if (computeQueryOption == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (computeQueryOption));
      if (settings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (settings));
      if (computeQueryOption.Context.Path != null)
      {
        this.Property = computeQueryOption.Context.TargetProperty;
        this.StructuredType = computeQueryOption.Context.TargetStructuredType;
      }
      this.Validate(computeQueryOption.ComputeClause, settings, computeQueryOption.Context.Model);
    }

    public virtual void Validate(
      ComputeClause computeClause,
      ODataValidationSettings settings,
      IEdmModel model)
    {
      this.CurrentAnyAllExpressionDepth = 0;
      this.CurrentNodeCount = 0;
      this.Model = model;
      foreach (ComputeExpression computedItem in computeClause.ComputedItems)
        this.ValidateQueryNode(computedItem.Expression, settings);
    }

    internal static ComputeQueryValidator GetComputeQueryValidator(ODataQueryContext context)
    {
      if (context == null)
        return new ComputeQueryValidator(new DefaultQuerySettings());
      return context.RequestContainer != null ? context.RequestContainer.GetRequiredService<ComputeQueryValidator>() : new ComputeQueryValidator(context.DefaultQuerySettings);
    }
  }
}
