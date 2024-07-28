// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.VariableGroupExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class VariableGroupExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.VariableGroup ToWebApiVariableGroup(
      this Microsoft.TeamFoundation.Build2.Server.VariableGroup srvVariableGroup,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvVariableGroup == null)
        return (Microsoft.TeamFoundation.Build.WebApi.VariableGroup) null;
      Microsoft.TeamFoundation.Build.WebApi.VariableGroup variableGroup = new Microsoft.TeamFoundation.Build.WebApi.VariableGroup(securedObject);
      variableGroup.Id = srvVariableGroup.Id;
      variableGroup.Type = srvVariableGroup.Type;
      variableGroup.Name = srvVariableGroup.Name;
      variableGroup.Description = srvVariableGroup.Description;
      Microsoft.TeamFoundation.Build.WebApi.VariableGroup apiVariableGroup = variableGroup;
      if (srvVariableGroup.Variables != null)
        apiVariableGroup.Variables = (IDictionary<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable>) srvVariableGroup.Variables.ToWebApiVariables(securedObject);
      return apiVariableGroup;
    }

    public static Microsoft.TeamFoundation.Build2.Server.VariableGroup ToServerVariableGroup(
      this Microsoft.TeamFoundation.Build.WebApi.VariableGroup webApiVariableGroup)
    {
      if (webApiVariableGroup == null)
        return (Microsoft.TeamFoundation.Build2.Server.VariableGroup) null;
      Microsoft.TeamFoundation.Build2.Server.VariableGroup serverVariableGroup = new Microsoft.TeamFoundation.Build2.Server.VariableGroup()
      {
        Id = webApiVariableGroup.Id,
        Type = webApiVariableGroup.Type,
        Name = webApiVariableGroup.Name,
        Description = webApiVariableGroup.Description
      };
      if (webApiVariableGroup.Variables != null)
        serverVariableGroup.Variables = (IDictionary<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable>) webApiVariableGroup.Variables.ToServerVariables();
      return serverVariableGroup;
    }
  }
}
