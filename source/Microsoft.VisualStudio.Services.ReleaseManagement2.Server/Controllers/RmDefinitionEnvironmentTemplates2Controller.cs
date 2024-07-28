// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmDefinitionEnvironmentTemplates2Controller
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "environmenttemplates", ResourceVersion = 2)]
  public class RmDefinitionEnvironmentTemplates2Controller : 
    RmDefinitionEnvironmentTemplatesController
  {
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "Right term")]
    protected override ReleaseDefinitionEnvironmentTemplate LatestToIncoming(
      ReleaseDefinitionEnvironmentTemplate environmentTemplate)
    {
      if (environmentTemplate == null)
        throw new InvalidRequestException(Resources.EnvironmentTemplateCannotBeNull);
      if (environmentTemplate.Environment != null && !environmentTemplate.Environment.DeployPhases.Any<DeployPhase>())
        environmentTemplate.Environment.ToDeployPhasesFormat();
      return environmentTemplate;
    }

    protected override ReleaseDefinitionEnvironmentTemplate IncomingToLatest(
      ReleaseDefinitionEnvironmentTemplate environmentTemplate)
    {
      return environmentTemplate;
    }
  }
}
