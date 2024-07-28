// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Builders.IReleaseBuilder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Builders
{
  public interface IReleaseBuilder
  {
    Release Build(
      ReleaseDefinition definition,
      CreateReleaseParameters createReleaseParameters,
      IVssRequestContext requestContext,
      ReleaseProjectInfo projectInfo);
  }
}
