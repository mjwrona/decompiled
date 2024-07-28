// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Validators.SkipQueryValidator
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;

namespace Microsoft.AspNet.OData.Query.Validators
{
  public class SkipQueryValidator
  {
    public virtual void Validate(
      SkipQueryOption skipQueryOption,
      ODataValidationSettings validationSettings)
    {
      if (skipQueryOption == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (skipQueryOption));
      if (validationSettings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (validationSettings));
      int num = skipQueryOption.Value;
      int? maxSkip = validationSettings.MaxSkip;
      int valueOrDefault = maxSkip.GetValueOrDefault();
      if (num > valueOrDefault & maxSkip.HasValue)
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.SkipTopLimitExceeded, (object) validationSettings.MaxSkip, (object) AllowedQueryOptions.Skip, (object) skipQueryOption.Value));
    }

    internal static SkipQueryValidator GetSkipQueryValidator(ODataQueryContext context) => context == null || context.RequestContainer == null ? new SkipQueryValidator() : ServiceProviderServiceExtensions.GetRequiredService<SkipQueryValidator>(context.RequestContainer);
  }
}
