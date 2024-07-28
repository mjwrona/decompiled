// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.TestSignalUrlDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Contracts;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public class TestSignalUrlDataProvider : IExtensionDataProvider
  {
    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      ProjectInfo project = requestContext.GetService<IRequestProjectService>().GetProject(requestContext);
      if (project == null)
        return (object) null;
      IProjectService service = requestContext.GetService<IProjectService>();
      requestContext.RootContext.Items["RequestProject"] = (object) service.GetProject(requestContext, project.Id);
      return (object) new SignalRUrlData(project.Id)
      {
        ConnectionUrl = Utils.GetConnectionUrl(requestContext)
      };
    }

    public string Name => "TestManagement.Provider.SignalRUrl";
  }
}
