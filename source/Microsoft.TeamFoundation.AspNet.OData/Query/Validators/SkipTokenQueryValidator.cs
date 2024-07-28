// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Validators.SkipTokenQueryValidator
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;

namespace Microsoft.AspNet.OData.Query.Validators
{
  public class SkipTokenQueryValidator
  {
    public virtual void Validate(
      SkipTokenQueryOption skipToken,
      ODataValidationSettings validationSettings)
    {
      if (skipToken == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull("skipQueryOption");
      if (validationSettings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (validationSettings));
      if (skipToken.Context != null && !skipToken.Context.DefaultQuerySettings.EnableSkipToken)
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NotAllowedQueryOption, (object) AllowedQueryOptions.SkipToken, (object) "AllowedQueryOptions"));
    }
  }
}
