// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.NonValidatingParameterBindingAttribute
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Validation;

namespace Microsoft.AspNet.OData
{
  [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
  internal sealed class NonValidatingParameterBindingAttribute : ParameterBindingAttribute
  {
    public override HttpParameterBinding GetBinding(HttpParameterDescriptor parameter)
    {
      IEnumerable<MediaTypeFormatter> formatters = (IEnumerable<MediaTypeFormatter>) parameter.Configuration.Formatters;
      return (HttpParameterBinding) new NonValidatingParameterBindingAttribute.NonValidatingParameterBinding(parameter, formatters);
    }

    private sealed class NonValidatingParameterBinding : PerRequestParameterBinding
    {
      public NonValidatingParameterBinding(
        HttpParameterDescriptor descriptor,
        IEnumerable<MediaTypeFormatter> formatters)
        : base(descriptor, formatters)
      {
      }

      protected override HttpParameterBinding CreateInnerBinding(
        IEnumerable<MediaTypeFormatter> perRequestFormatters)
      {
        return this.Descriptor.BindWithFormatter(perRequestFormatters, (IBodyModelValidator) null);
      }
    }
  }
}
