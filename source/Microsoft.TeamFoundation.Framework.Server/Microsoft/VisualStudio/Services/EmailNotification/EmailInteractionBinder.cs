// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.EmailNotification.EmailInteractionBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;

namespace Microsoft.VisualStudio.Services.EmailNotification
{
  public class EmailInteractionBinder : IModelBinder
  {
    public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
    {
      try
      {
        if (bindingContext.ModelType != typeof (EmailInteraction))
          return false;
        ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == null)
          return false;
        if (!(valueProviderResult.RawValue is string rawValue))
        {
          bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Wrong value type");
          return false;
        }
        EmailInteraction emailInteraction = EmailInteraction.FromBase64Context(rawValue);
        bindingContext.Model = (object) emailInteraction;
        return true;
      }
      catch (Exception ex)
      {
        bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex.Message);
        return false;
      }
    }
  }
}
