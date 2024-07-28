// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.ODataQueryContextExtensions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Query.Validators;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Microsoft.AspNet.OData
{
  internal static class ODataQueryContextExtensions
  {
    public static ODataQuerySettings UpdateQuerySettings(
      this ODataQueryContext context,
      ODataQuerySettings querySettings,
      IQueryable query)
    {
      ODataQuerySettings odataQuerySettings = context == null || context.RequestContainer == null ? new ODataQuerySettings() : context.RequestContainer.GetRequiredService<ODataQuerySettings>();
      odataQuerySettings.CopyFrom(querySettings);
      if (odataQuerySettings.HandleNullPropagation == HandleNullPropagationOption.Default)
        odataQuerySettings.HandleNullPropagation = query != null ? HandleNullPropagationOptionHelper.GetDefaultHandleNullPropagationOption(query) : HandleNullPropagationOption.True;
      return odataQuerySettings;
    }

    public static SkipTokenHandler GetSkipTokenHandler(this ODataQueryContext context) => context == null || context.RequestContainer == null ? (SkipTokenHandler) DefaultSkipTokenHandler.Instance : context.RequestContainer.GetRequiredService<SkipTokenHandler>();

    public static SkipTokenQueryValidator GetSkipTokenQueryValidator(this ODataQueryContext context) => context == null || context.RequestContainer == null ? new SkipTokenQueryValidator() : context.RequestContainer.GetRequiredService<SkipTokenQueryValidator>();
  }
}
