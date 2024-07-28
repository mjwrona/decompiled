// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.XmlFormatterAttribute
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Net.Http.Formatting;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class XmlFormatterAttribute : Attribute, IControllerConfiguration
  {
    public void Initialize(
      HttpControllerSettings controllerSettings,
      HttpControllerDescriptor controllerDescriptor)
    {
      XmlMediaTypeFormatter mediaTypeFormatter = new XmlMediaTypeFormatter();
      controllerSettings.Formatters.Clear();
      controllerSettings.Formatters.Add((MediaTypeFormatter) mediaTypeFormatter);
    }
  }
}
