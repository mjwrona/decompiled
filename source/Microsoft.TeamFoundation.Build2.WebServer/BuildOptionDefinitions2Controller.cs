// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildOptionDefinitions2Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "options", ResourceVersion = 2)]
  public class BuildOptionDefinitions2Controller : BuildApiController
  {
    [HttpGet]
    public virtual List<Microsoft.TeamFoundation.Build.WebApi.BuildOptionDefinition> GetBuildOptionDefinitions()
    {
      List<Microsoft.TeamFoundation.Build.WebApi.BuildOptionDefinition> source = new List<Microsoft.TeamFoundation.Build.WebApi.BuildOptionDefinition>();
      ApiResourceVersion apiResourceVersion = this.GetApiResourceVersion();
      using (IDisposableReadOnlyList<IBuildOption> extensions = this.TfsRequestContext.GetExtensions<IBuildOption>(throwOnError: true))
      {
        Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition = new Microsoft.TeamFoundation.Build2.Server.BuildDefinition();
        definition.Process = (Microsoft.TeamFoundation.Build2.Server.BuildProcess) new Microsoft.TeamFoundation.Build2.Server.DesignerProcess();
        TeamProjectReference projectReference = this.ProjectInfo.ToTeamProjectReference(this.TfsRequestContext);
        foreach (IBuildOption feature in extensions.Where<IBuildOption>((Func<IBuildOption, bool>) (bo => bo.IsSupported(this.TfsRequestContext, apiResourceVersion, definition.Process != null ? definition.Process.Type : 1))))
        {
          Microsoft.TeamFoundation.Build2.Server.BuildOptionDefinition definition1 = feature.GetDefinition(this.TfsRequestContext, this.ProjectId);
          if (definition1 != null)
          {
            definition1.Ordinal = feature.GetOrdinal();
            source.Add(definition1.ToWebApiBuildOptionDefinition((ISecuredObject) projectReference));
          }
        }
      }
      return source.OrderBy<Microsoft.TeamFoundation.Build.WebApi.BuildOptionDefinition, int>((Func<Microsoft.TeamFoundation.Build.WebApi.BuildOptionDefinition, int>) (d => d.Ordinal)).ToList<Microsoft.TeamFoundation.Build.WebApi.BuildOptionDefinition>();
    }
  }
}
