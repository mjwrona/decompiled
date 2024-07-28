// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.ODataModelBinderProvider
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter.Deserialization;
using Microsoft.AspNet.OData.Routing;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;

namespace Microsoft.AspNet.OData.Formatter
{
  public class ODataModelBinderProvider : ModelBinderProvider
  {
    public override IModelBinder GetBinder(HttpConfiguration configuration, Type modelType)
    {
      if (configuration == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (configuration));
      if (modelType == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (modelType));
      return (IModelBinder) new ODataModelBinderProvider.ODataModelBinder();
    }

    internal class ODataModelBinder : IModelBinder
    {
      public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
      {
        if (bindingContext == null)
          throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (bindingContext));
        if (bindingContext.ModelMetadata == null)
          throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (bindingContext), SRResources.ModelBinderUtil_ModelMetadataCannotBeNull);
        string key = "DF908045-6922-46A0-82F2-2F6E7F43D1B1_" + bindingContext.ModelName;
        ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(key);
        if (valueProviderResult == null)
        {
          valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
          if (valueProviderResult == null)
            return false;
        }
        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
        try
        {
          if (valueProviderResult.RawValue is ODataParameterValue rawValue1)
          {
            bindingContext.Model = ODataModelBinderProvider.ODataModelBinder.ConvertTo(rawValue1, actionContext, bindingContext, actionContext.Request.GetRequestContainer());
            return true;
          }
          if (!(valueProviderResult.RawValue is string rawValue2))
            return false;
          bindingContext.Model = ODataModelBinderConverter.ConvertTo(rawValue2, bindingContext.ModelType);
          return true;
        }
        catch (ODataException ex)
        {
          bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex.Message);
          return false;
        }
        catch (ValidationException ex)
        {
          bindingContext.ModelState.AddModelError(bindingContext.ModelName, Microsoft.AspNet.OData.Common.Error.Format(SRResources.ValueIsInvalid, valueProviderResult.RawValue, (object) ex.Message));
          return false;
        }
        catch (FormatException ex)
        {
          bindingContext.ModelState.AddModelError(bindingContext.ModelName, Microsoft.AspNet.OData.Common.Error.Format(SRResources.ValueIsInvalid, valueProviderResult.RawValue, (object) ex.Message));
          return false;
        }
        catch (Exception ex)
        {
          bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex);
          return false;
        }
      }

      internal static object ConvertTo(
        ODataParameterValue parameterValue,
        HttpActionContext actionContext,
        ModelBindingContext bindingContext,
        IServiceProvider requestContainer)
      {
        object graph = parameterValue.Value;
        switch (graph)
        {
          case null:
          case ODataNullValue _:
            return (object) null;
          default:
            IEdmTypeReference edmType = parameterValue.EdmType;
            ODataDeserializerContext readContext = ODataModelBinderProvider.ODataModelBinder.BuildDeserializerContext(actionContext, bindingContext, edmType);
            return ODataModelBinderConverter.Convert(graph, edmType, bindingContext.ModelType, bindingContext.ModelName, readContext, requestContainer);
        }
      }

      internal static ODataDeserializerContext BuildDeserializerContext(
        HttpActionContext actionContext,
        ModelBindingContext bindingContext,
        IEdmTypeReference edmTypeReference)
      {
        HttpRequestMessage request = actionContext.Request;
        ODataPath path = request.ODataProperties().Path;
        IEdmModel model = request.GetModel();
        return new ODataDeserializerContext()
        {
          Path = path,
          Model = model,
          Request = request,
          ResourceType = bindingContext.ModelType,
          ResourceEdmType = edmTypeReference
        };
      }
    }
  }
}
