// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.FromODataUriAttribute
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using System;
using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;

namespace Microsoft.AspNet.OData
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
  public sealed class FromODataUriAttribute : ModelBinderAttribute
  {
    private static readonly ModelBinderProvider _provider = (ModelBinderProvider) new ODataModelBinderProvider();

    public override HttpParameterBinding GetBinding(HttpParameterDescriptor parameter)
    {
      if (parameter == null)
        throw Error.ArgumentNull(nameof (parameter));
      IModelBinder binder = FromODataUriAttribute._provider.GetBinder(parameter.Configuration, parameter.ParameterType);
      IEnumerable<ValueProviderFactory> providerFactories = this.GetValueProviderFactories(parameter.Configuration);
      return (HttpParameterBinding) new ModelBinderParameterBinding(parameter, binder, providerFactories);
    }
  }
}
