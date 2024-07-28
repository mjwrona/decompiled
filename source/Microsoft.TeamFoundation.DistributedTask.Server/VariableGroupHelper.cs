// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.VariableGroupHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class VariableGroupHelper
  {
    public static void FillVariableGroupProjectReferencesProjectDetail(
      IVssRequestContext requestContext,
      IList<VariableGroupProjectReference> variableGroupProjectReferences)
    {
      if (variableGroupProjectReferences == null || variableGroupProjectReferences.Count == 0)
        return;
      IEnumerable<ProjectInfo> projects = requestContext.GetService<IProjectService>().GetProjects(requestContext.Elevate());
      IDictionary<string, Guid> dictionary1 = (IDictionary<string, Guid>) new Dictionary<string, Guid>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      IDictionary<Guid, string> dictionary2 = (IDictionary<Guid, string>) new Dictionary<Guid, string>();
      foreach (ProjectInfo projectInfo in projects)
      {
        if (!dictionary1.ContainsKey(projectInfo.Name))
          dictionary1.Add(projectInfo.Name, projectInfo.Id);
        if (!dictionary2.ContainsKey(projectInfo.Id))
          dictionary2.Add(projectInfo.Id, projectInfo.Name);
      }
      foreach (VariableGroupProjectReference projectReference1 in (IEnumerable<VariableGroupProjectReference>) (variableGroupProjectReferences ?? (IList<VariableGroupProjectReference>) new List<VariableGroupProjectReference>()))
      {
        ProjectReference projectReference2 = projectReference1.ProjectReference;
        Guid guid = projectReference2 != null ? projectReference2.Id : throw new ArgumentException(TaskResources.VariableGroupProjectReferenceInformationNotProvided()).Expected("DistributedTask");
        Guid id = projectReference2.Id;
        if (projectReference2.Id == Guid.Empty)
        {
          if (!dictionary1.ContainsKey(projectReference2.Name))
            throw new ArgumentException(TaskResources.VariableGroupProjectReferenceInformationNameInvalid()).Expected("DistributedTask");
          projectReference2.Id = dictionary1[projectReference2.Name];
          projectReference2.Name = dictionary2[projectReference2.Id];
        }
        if (string.IsNullOrEmpty(projectReference2.Name))
        {
          if (!dictionary2.ContainsKey(projectReference2.Id))
            throw new ArgumentException(TaskResources.VariableGroupProjectReferenceInformationIdInvalid()).Expected("DistributedTask");
          projectReference2.Name = dictionary2[projectReference2.Id];
        }
      }
    }
  }
}
