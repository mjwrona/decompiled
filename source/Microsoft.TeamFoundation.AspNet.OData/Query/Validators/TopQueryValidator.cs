// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Validators.TopQueryValidator
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;

namespace Microsoft.AspNet.OData.Query.Validators
{
  public class TopQueryValidator
  {
    public virtual void Validate(
      TopQueryOption topQueryOption,
      ODataValidationSettings validationSettings)
    {
      if (topQueryOption == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (topQueryOption));
      if (validationSettings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (validationSettings));
      int num = topQueryOption.Value;
      int? maxTop1 = validationSettings.MaxTop;
      int valueOrDefault = maxTop1.GetValueOrDefault();
      if (num > valueOrDefault & maxTop1.HasValue)
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.SkipTopLimitExceeded, (object) validationSettings.MaxTop, (object) AllowedQueryOptions.Top, (object) topQueryOption.Value));
      int maxTop2;
      if (EdmLibHelpers.IsTopLimitExceeded(topQueryOption.Context.TargetProperty, topQueryOption.Context.TargetStructuredType, topQueryOption.Context.Model, topQueryOption.Value, topQueryOption.Context.DefaultQuerySettings, out maxTop2))
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.SkipTopLimitExceeded, (object) maxTop2, (object) AllowedQueryOptions.Top, (object) topQueryOption.Value));
    }

    internal static TopQueryValidator GetTopQueryValidator(ODataQueryContext context) => context == null || context.RequestContainer == null ? new TopQueryValidator() : ServiceProviderServiceExtensions.GetRequiredService<TopQueryValidator>(context.RequestContainer);
  }
}
