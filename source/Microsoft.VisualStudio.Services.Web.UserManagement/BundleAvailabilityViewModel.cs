// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Web.UserManagement.BundleAvailabilityViewModel
// Assembly: Microsoft.VisualStudio.Services.Web.UserManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7BDE82F4-5081-4A92-A83F-EE78FF05B171
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Web.UserManagement.dll

using Microsoft.TeamFoundation.Server.WebAccess;

namespace Microsoft.VisualStudio.Services.Web.UserManagement
{
  public class BundleAvailabilityViewModel
  {
    public int BundlesAvailable { get; set; }

    public ExtensionViewModel ExtensionViewModel { get; set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["BundlesAvailable"] = (object) this.BundlesAvailable;
      return json;
    }
  }
}
