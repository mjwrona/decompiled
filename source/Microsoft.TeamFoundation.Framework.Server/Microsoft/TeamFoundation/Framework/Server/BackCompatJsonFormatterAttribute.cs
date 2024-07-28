// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BackCompatJsonFormatterAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Net.Http.Formatting;
using System.Web.Http.Controllers;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [AttributeUsage(AttributeTargets.Class)]
  public class BackCompatJsonFormatterAttribute : Attribute, IControllerConfiguration
  {
    public void Initialize(
      HttpControllerSettings controllerSettings,
      HttpControllerDescriptor controllerDescriptor)
    {
      if (controllerSettings == null || controllerSettings.Formatters == null)
        return;
      controllerSettings.Formatters.Remove((MediaTypeFormatter) controllerSettings.Formatters.JsonFormatter);
      controllerSettings.Formatters.Add((MediaTypeFormatter) new JsonMediaTypeFormatter());
    }
  }
}
