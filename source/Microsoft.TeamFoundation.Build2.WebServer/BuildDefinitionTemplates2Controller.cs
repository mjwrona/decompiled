// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildDefinitionTemplates2Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi.Internals;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "templates", ResourceVersion = 2)]
  public class BuildDefinitionTemplates2Controller : BuildDefinitionTemplatesController
  {
    [HttpGet]
    public override IList<BuildDefinitionTemplate3_2> GetTemplates() => this.GetTemplatesInternal(true);

    [HttpGet]
    public override BuildDefinitionTemplate3_2 GetTemplate(string templateId) => this.GetTemplateInternal(templateId, true);
  }
}
