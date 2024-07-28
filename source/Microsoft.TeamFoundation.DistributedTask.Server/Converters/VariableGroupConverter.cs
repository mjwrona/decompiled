// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Converters.VariableGroupConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Converters
{
  public static class VariableGroupConverter
  {
    public static VariableGroup ToVariableGroup(
      this VariableGroupParameters variableGroupParameters,
      IVssRequestContext requestContext,
      int variableGroupId,
      Guid projectId)
    {
      IList<VariableGroupProjectReference> projectReferenceList = (IList<VariableGroupProjectReference>) new List<VariableGroupProjectReference>();
      IList<VariableGroupProjectReference> projectReferences = variableGroupParameters.VariableGroupProjectReferences;
      if ((projectReferences != null ? (projectReferences.Count > 0 ? 1 : 0) : 0) != 0 && projectId == Guid.Empty)
        projectReferenceList = variableGroupParameters.VariableGroupProjectReferences;
      else
        projectReferenceList.Add(new VariableGroupProjectReference()
        {
          Description = variableGroupParameters.Description,
          Name = variableGroupParameters.Name,
          ProjectReference = new ProjectReference()
          {
            Id = projectId,
            Name = string.Empty
          }
        });
      VariableGroup variableGroup = new VariableGroup()
      {
        Type = variableGroupParameters.Type,
        Name = variableGroupParameters.Name,
        Description = variableGroupParameters.Description,
        ProviderData = variableGroupParameters.ProviderData,
        Variables = variableGroupParameters.Variables,
        CreatedBy = new IdentityRef(),
        Id = variableGroupId,
        VariableGroupProjectReferences = projectReferenceList
      };
      VariableGroupHelper.FillVariableGroupProjectReferencesProjectDetail(requestContext, variableGroup.VariableGroupProjectReferences);
      return variableGroup;
    }
  }
}
