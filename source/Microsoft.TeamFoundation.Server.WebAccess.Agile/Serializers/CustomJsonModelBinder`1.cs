// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Serializers.CustomJsonModelBinder`1
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using System;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Serializers
{
  public class CustomJsonModelBinder<T> : JsonModelBinder where T : ArgumentException
  {
    public override void HandleArgumentException(
      ArgumentException e,
      ITfsController tfsController,
      ModelBindingContext bindingContext)
    {
      base.HandleArgumentException((ArgumentException) Activator.CreateInstance(typeof (T), (object) e.Message, (object) e.ParamName, (object) e.InnerException), tfsController, bindingContext);
    }
  }
}
