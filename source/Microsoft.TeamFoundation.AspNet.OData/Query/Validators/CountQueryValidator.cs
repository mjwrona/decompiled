// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Validators.CountQueryValidator
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using System;

namespace Microsoft.AspNet.OData.Query.Validators
{
  public class CountQueryValidator
  {
    private readonly DefaultQuerySettings _defaultQuerySettings;

    public CountQueryValidator(DefaultQuerySettings defaultQuerySettings) => this._defaultQuerySettings = defaultQuerySettings;

    public virtual void Validate(
      CountQueryOption countQueryOption,
      ODataValidationSettings validationSettings)
    {
      if (countQueryOption == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (countQueryOption));
      if (validationSettings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (validationSettings));
      ODataPath path = countQueryOption.Context.Path;
      if (path == null || path.Segments.Count <= 0)
        return;
      IEdmProperty targetProperty = countQueryOption.Context.TargetProperty;
      IEdmStructuredType targetStructuredType = countQueryOption.Context.TargetStructuredType;
      string targetName = countQueryOption.Context.TargetName;
      if (!EdmLibHelpers.IsNotCountable(targetProperty, targetStructuredType, countQueryOption.Context.Model, this._defaultQuerySettings.EnableCount))
        return;
      if (targetProperty == null)
        throw new InvalidOperationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotCountableEntitySetUsedForCount, (object) targetName));
      throw new InvalidOperationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotCountablePropertyUsedForCount, (object) targetName));
    }

    internal static CountQueryValidator GetCountQueryValidator(ODataQueryContext context)
    {
      if (context == null)
        return new CountQueryValidator(new DefaultQuerySettings());
      return context.RequestContainer != null ? context.RequestContainer.GetRequiredService<CountQueryValidator>() : new CountQueryValidator(context.DefaultQuerySettings);
    }
  }
}
