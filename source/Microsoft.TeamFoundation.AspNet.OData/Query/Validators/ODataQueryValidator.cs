// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Validators.ODataQueryValidator
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;

namespace Microsoft.AspNet.OData.Query.Validators
{
  public class ODataQueryValidator
  {
    public virtual void Validate(
      ODataQueryOptions options,
      ODataValidationSettings validationSettings)
    {
      if (options == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (options));
      if (validationSettings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (validationSettings));
      if (options.Apply != null && options.Apply.ApplyClause != null)
        ODataQueryValidator.ValidateQueryOptionAllowed(AllowedQueryOptions.Apply, validationSettings.AllowedQueryOptions);
      if (options.Compute != null)
      {
        ODataQueryValidator.ValidateQueryOptionAllowed(AllowedQueryOptions.Compute, validationSettings.AllowedQueryOptions);
        options.Compute.Validate(validationSettings);
      }
      if (options.Skip != null)
      {
        ODataQueryValidator.ValidateQueryOptionAllowed(AllowedQueryOptions.Skip, validationSettings.AllowedQueryOptions);
        options.Skip.Validate(validationSettings);
      }
      if (options.Top != null)
      {
        ODataQueryValidator.ValidateQueryOptionAllowed(AllowedQueryOptions.Top, validationSettings.AllowedQueryOptions);
        options.Top.Validate(validationSettings);
      }
      if (options.OrderBy != null)
      {
        ODataQueryValidator.ValidateQueryOptionAllowed(AllowedQueryOptions.OrderBy, validationSettings.AllowedQueryOptions);
        options.OrderBy.Validate(validationSettings);
      }
      if (options.Filter != null)
      {
        ODataQueryValidator.ValidateQueryOptionAllowed(AllowedQueryOptions.Filter, validationSettings.AllowedQueryOptions);
        options.Filter.Validate(validationSettings);
      }
      if (options.Count != null || options.InternalRequest.IsCountRequest())
      {
        ODataQueryValidator.ValidateQueryOptionAllowed(AllowedQueryOptions.Count, validationSettings.AllowedQueryOptions);
        if (options.Count != null)
          options.Count.Validate(validationSettings);
      }
      if (options.SkipToken != null)
      {
        ODataQueryValidator.ValidateQueryOptionAllowed(AllowedQueryOptions.SkipToken, validationSettings.AllowedQueryOptions);
        options.SkipToken.Validate(validationSettings);
      }
      if (options.RawValues.Expand != null)
        ODataQueryValidator.ValidateQueryOptionAllowed(AllowedQueryOptions.Expand, validationSettings.AllowedQueryOptions);
      if (options.RawValues.Select != null)
        ODataQueryValidator.ValidateQueryOptionAllowed(AllowedQueryOptions.Select, validationSettings.AllowedQueryOptions);
      if (options.SelectExpand != null)
        options.SelectExpand.Validate(validationSettings);
      if (options.RawValues.Format != null)
        ODataQueryValidator.ValidateQueryOptionAllowed(AllowedQueryOptions.Format, validationSettings.AllowedQueryOptions);
      if (options.RawValues.SkipToken != null)
        ODataQueryValidator.ValidateQueryOptionAllowed(AllowedQueryOptions.SkipToken, validationSettings.AllowedQueryOptions);
      if (options.RawValues.DeltaToken == null)
        return;
      ODataQueryValidator.ValidateQueryOptionAllowed(AllowedQueryOptions.DeltaToken, validationSettings.AllowedQueryOptions);
    }

    internal static ODataQueryValidator GetODataQueryValidator(ODataQueryContext context) => context == null || context.RequestContainer == null ? new ODataQueryValidator() : ServiceProviderServiceExtensions.GetRequiredService<ODataQueryValidator>(context.RequestContainer);

    private static void ValidateQueryOptionAllowed(
      AllowedQueryOptions queryOption,
      AllowedQueryOptions allowed)
    {
      if ((queryOption & allowed) == AllowedQueryOptions.None)
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotAllowedQueryOption, (object) queryOption, (object) "AllowedQueryOptions"));
    }
  }
}
