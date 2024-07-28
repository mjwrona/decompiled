// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.ODataQueryParameterBindingAttribute
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Query;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;

namespace Microsoft.AspNet.OData
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
  public sealed class ODataQueryParameterBindingAttribute : ParameterBindingAttribute
  {
    public override HttpParameterBinding GetBinding(HttpParameterDescriptor parameter) => (HttpParameterBinding) new ODataQueryParameterBindingAttribute.ODataQueryParameterBinding(parameter);

    internal static Type GetEntityClrTypeFromParameterType(Type parameterType) => parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof (ODataQueryOptions<>) ? ((IEnumerable<Type>) parameterType.GetGenericArguments()).Single<Type>() : (Type) null;

    internal class ODataQueryParameterBinding : HttpParameterBinding
    {
      private static MethodInfo _createODataQueryOptions = typeof (ODataQueryParameterBindingAttribute.ODataQueryParameterBinding).GetMethod("CreateODataQueryOptions");
      private const string CreateODataQueryOptionsCtorKey = "MS_CreateODataQueryOptionsOfT";

      public ODataQueryParameterBinding(HttpParameterDescriptor parameterDescriptor)
        : base(parameterDescriptor)
      {
      }

      public override Task ExecuteBindingAsync(
        ModelMetadataProvider metadataProvider,
        HttpActionContext actionContext,
        CancellationToken cancellationToken)
      {
        HttpRequestMessage request = actionContext != null ? actionContext.Request : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (actionContext));
        if (request == null)
          throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (actionContext), SRResources.ActionContextMustHaveRequest);
        HttpActionDescriptor actionDescriptor = actionContext.ActionDescriptor;
        if (actionDescriptor == null)
          throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (actionContext), SRResources.ActionContextMustHaveDescriptor);
        if (request.GetConfiguration() == null)
          throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (actionContext), SRResources.RequestMustContainConfiguration);
        Type type = ODataQueryParameterBindingAttribute.GetEntityClrTypeFromParameterType(this.Descriptor.ParameterType);
        if ((object) type == null)
          type = ODataQueryParameterBindingAttribute.ODataQueryParameterBinding.GetEntityClrTypeFromActionReturnType(actionDescriptor);
        Type entityClrType = type;
        IEdmModel model = request.GetModel();
        ODataQueryContext odataQueryContext = new ODataQueryContext(model != EdmCoreModel.Instance ? model : actionDescriptor.GetEdmModel(entityClrType), entityClrType, request.ODataProperties().Path);
        ODataQueryOptions odataQueryOptions = ((Func<ODataQueryContext, HttpRequestMessage, ODataQueryOptions>) this.Descriptor.Properties.GetOrAdd((object) "MS_CreateODataQueryOptionsOfT", (Func<object, object>) (_ => (object) Delegate.CreateDelegate(typeof (Func<ODataQueryContext, HttpRequestMessage, ODataQueryOptions>), ODataQueryParameterBindingAttribute.ODataQueryParameterBinding._createODataQueryOptions.MakeGenericMethod(entityClrType)))))(odataQueryContext, request);
        this.SetValue(actionContext, (object) odataQueryOptions);
        return Microsoft.AspNet.OData.Common.TaskHelpers.Completed();
      }

      public static ODataQueryOptions<T> CreateODataQueryOptions<T>(
        ODataQueryContext context,
        HttpRequestMessage request)
      {
        return new ODataQueryOptions<T>(context, request);
      }

      internal static Type GetEntityClrTypeFromActionReturnType(
        HttpActionDescriptor actionDescriptor)
      {
        Type type = !(actionDescriptor.ReturnType == (Type) null) ? TypeHelper.GetImplementedIEnumerableType(actionDescriptor.ReturnType) : throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.FailedToBuildEdmModelBecauseReturnTypeIsNull, (object) actionDescriptor.ActionName, (object) actionDescriptor.ControllerDescriptor.ControllerName);
        return !(type == (Type) null) ? type : throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.FailedToRetrieveTypeToBuildEdmModel, (object) actionDescriptor.ActionName, (object) actionDescriptor.ControllerDescriptor.ControllerName, (object) actionDescriptor.ReturnType.FullName);
      }
    }
  }
}
