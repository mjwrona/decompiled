// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.XmlModelBinder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  internal class XmlModelBinder : IModelBinder
  {
    public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      ArgumentUtility.CheckForNull<ModelBindingContext>(bindingContext, nameof (bindingContext));
      string attemptedValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).AttemptedValue;
      return XmlSerializationHelper.Deserialize(bindingContext.ModelType, attemptedValue);
    }
  }
}
