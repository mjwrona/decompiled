// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.CollectionSettingsAdminViewDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Html;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class CollectionSettingsAdminViewDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.CollectionSettings";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      TfsWebContext webContext = (TfsWebContext) WebContextFactory.GetWebContext(requestContext);
      CollectionSettingsAdminViewData data = new CollectionSettingsAdminViewData();
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer"))
      {
        data.AccountTrialInformationDataJson = JsonConvert.SerializeObject((object) AccountTrialExtensions.GetAccountTrialInformation(webContext));
        data.AccountAadInformationDataJson = JsonConvert.SerializeObject((object) AccountAadInformationExtensions.GetAccountAadInformation(webContext));
      }
      else
      {
        JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
        data.AccountTrialInformationDataJson = scriptSerializer.Serialize((object) AccountTrialExtensions.GetAccountTrialInformation(webContext));
        data.AccountAadInformationDataJson = scriptSerializer.Serialize((object) AccountAadInformationExtensions.GetAccountAadInformation(webContext));
      }
      return (object) data;
    }
  }
}
