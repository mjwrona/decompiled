// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectComponent13
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectComponent13 : ProjectComponent12
  {
    internal override sealed ProjectOperation UpdateProject(
      ProjectInfo project,
      Guid userIdentity,
      out ProjectInfo updatedProject,
      bool mergeProperties = false)
    {
      throw new NotSupportedException("Use the overload without mergeProperties instead.");
    }

    protected override sealed void AddProjectIdPropertyColumnsBinder(ResultCollection rc)
    {
    }

    protected override sealed void AddProjectPropertyColumnsBinder(ResultCollection rc)
    {
    }

    protected override ObjectBinder<ProjectInfo> CreateProjectHistoryColumnsBinder() => this.CreateProjectInfoColumnsBinder();

    protected override sealed List<Tuple<Guid, ProjectProperty>> GetPropertiesForProjects(
      ResultCollection rc)
    {
      return new List<Tuple<Guid, ProjectProperty>>();
    }

    protected override sealed List<ProjectProperty> GetPropertiesForProject(ResultCollection rc) => new List<ProjectProperty>();

    protected override sealed void ResolveProjectPropertyDeltas(
      IVssRequestContext requestContext,
      IList<ProjectInfo> projectHistory)
    {
    }

    protected override void BindProperties(ProjectInfo project)
    {
    }

    protected override sealed void BindMergeProperties(bool mergeProperties)
    {
    }
  }
}
