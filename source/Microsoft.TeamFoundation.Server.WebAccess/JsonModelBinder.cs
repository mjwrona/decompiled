// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.JsonModelBinder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class JsonModelBinder : IModelBinder
  {
    public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      ArgumentUtility.CheckForNull<ModelBindingContext>(bindingContext, nameof (bindingContext));
      ITfsController tfsController = (ITfsController) null;
      if (controllerContext != null)
        tfsController = controllerContext.Controller as ITfsController;
      if (tfsController != null)
        tfsController.TfsRequestContext.TraceEnter(520060, "WebAccess", TfsTraceLayers.Framework, "JsonModelBinder.BindModel");
      object obj = (object) null;
      ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
      if (valueProviderResult != null)
      {
        string attemptedValue = valueProviderResult.AttemptedValue;
        try
        {
          int maxJsonLength = this.MaxJsonLength;
          obj = maxJsonLength > 0 ? (!tfsController.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer") ? JsonExtensions.FromJson(attemptedValue, bindingContext.ModelType, maxJsonLength, this.GetJsConverters()) : JsonExtensions.FromJson(attemptedValue, bindingContext.ModelType, this.GetConverters())) : (!tfsController.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer") ? JsonExtensions.FromJson(attemptedValue, bindingContext.ModelType, this.GetJsConverters()) : JsonExtensions.FromJson(attemptedValue, bindingContext.ModelType, this.GetConverters()));
          if (obj is IValidatable validatable)
            validatable.Validate();
        }
        catch (ArgumentException ex)
        {
          this.HandleArgumentException(ex, tfsController, bindingContext);
        }
        catch (Exception ex)
        {
          bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex);
        }
      }
      if (tfsController != null)
        tfsController.TfsRequestContext.TraceLeave(520060, "WebAccess", TfsTraceLayers.Framework, "JsonModelBinder.BindModel");
      return obj;
    }

    public virtual int MaxJsonLength => -1;

    public virtual JsonConverter[] GetConverters() => Array.Empty<JsonConverter>();

    public virtual JavaScriptConverter[] GetJsConverters() => Array.Empty<JavaScriptConverter>();

    public virtual void HandleArgumentException(
      ArgumentException e,
      ITfsController tfsController,
      ModelBindingContext bindingContext)
    {
      if (tfsController != null)
        e.Expected(tfsController.TfsRequestContext.ServiceName);
      bindingContext.ModelState.AddModelError(bindingContext.ModelName, (Exception) e);
    }
  }
}
