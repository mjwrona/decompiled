// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Extensions.HttpActionDescriptorExtensions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.OData.Edm;
using System;
using System.ComponentModel;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.AspNet.OData.Extensions
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal static class HttpActionDescriptorExtensions
  {
    private const string ModelKeyPrefix = "Microsoft.AspNet.OData.Model+";

    internal static IEdmModel GetEdmModel(
      this HttpActionDescriptor actionDescriptor,
      Type entityClrType)
    {
      if (actionDescriptor == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (actionDescriptor));
      if (entityClrType == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (entityClrType));
      return actionDescriptor.Properties.GetOrAdd((object) ("Microsoft.AspNet.OData.Model+" + entityClrType.FullName), (Func<object, object>) (_ =>
      {
        ODataConventionModelBuilder conventionModelBuilder = new ODataConventionModelBuilder((IWebApiAssembliesResolver) new WebApiAssembliesResolver(actionDescriptor.Configuration.Services.GetAssembliesResolver()), true);
        conventionModelBuilder.AddEntitySet(entityClrType.Name, conventionModelBuilder.AddEntityType(entityClrType));
        return (object) conventionModelBuilder.GetEdmModel();
      })) as IEdmModel;
    }
  }
}
