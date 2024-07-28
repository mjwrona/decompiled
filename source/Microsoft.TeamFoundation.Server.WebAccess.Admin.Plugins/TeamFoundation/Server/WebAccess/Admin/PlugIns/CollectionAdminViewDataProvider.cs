// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.CollectionAdminViewDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class CollectionAdminViewDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.CollectionAdminView";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      CollectionAdminViewData data = new CollectionAdminViewData();
      CollectionViewModel collectionViewModel = new CollectionViewModel((TfsWebContext) WebContextFactory.GetWebContext(requestContext));
      collectionViewModel.CanCreateProjects = requestContext.ExecutionEnvironment.IsHostedDeployment;
      data.DisplayName = collectionViewModel.DisplayName;
      data.Description = collectionViewModel.Description;
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer"))
      {
        data.CollectionOverviewOptionsJson = JsonConvert.SerializeObject((object) new
        {
          projects = collectionViewModel.TeamProjects,
          canCreateProjects = collectionViewModel.CanCreateProjects,
          projectRenameIsEnabled = true
        });
      }
      else
      {
        JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
        data.CollectionOverviewOptionsJson = scriptSerializer.Serialize((object) new
        {
          projects = collectionViewModel.TeamProjects,
          canCreateProjects = collectionViewModel.CanCreateProjects,
          projectRenameIsEnabled = true
        });
      }
      return (object) data;
    }
  }
}
