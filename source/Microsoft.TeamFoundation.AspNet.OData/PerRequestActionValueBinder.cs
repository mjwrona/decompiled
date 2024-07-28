// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.PerRequestActionValueBinder
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace Microsoft.AspNet.OData
{
  internal class PerRequestActionValueBinder : IActionValueBinder
  {
    private IActionValueBinder _innerActionValueBinder;

    public PerRequestActionValueBinder(IActionValueBinder innerActionValueBinder) => this._innerActionValueBinder = innerActionValueBinder != null ? innerActionValueBinder : throw Error.ArgumentNull(nameof (innerActionValueBinder));

    public HttpActionBinding GetBinding(HttpActionDescriptor actionDescriptor)
    {
      HttpActionBinding binding = actionDescriptor != null ? this._innerActionValueBinder.GetBinding(actionDescriptor) : throw Error.ArgumentNull(nameof (actionDescriptor));
      if (binding == null)
        return (HttpActionBinding) null;
      if (binding.ParameterBindings != null)
      {
        for (int index = 0; index < binding.ParameterBindings.Length; ++index)
        {
          HttpParameterBinding parameterBinding = binding.ParameterBindings[index];
          if (parameterBinding != null && parameterBinding is FormatterParameterBinding)
            binding.ParameterBindings[index] = (HttpParameterBinding) new PerRequestParameterBinding(parameterBinding.Descriptor, (IEnumerable<MediaTypeFormatter>) actionDescriptor.Configuration.Formatters);
        }
      }
      return binding;
    }
  }
}
