// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectComponent14
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectComponent14 : ProjectComponent13
  {
    protected override void BindProjectVisibility(string parameterName, byte? projectVisibility) => this.BindNullableByte("@projectVisibility", projectVisibility);

    protected override ObjectBinder<ProjectInfo> CreateProjectInfoColumnsBinder() => (ObjectBinder<ProjectInfo>) new ProjectInfoColumns4();
  }
}
