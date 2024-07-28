// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.BuildDefinitionVariableExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class BuildDefinitionVariableExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable ToWebApiBuildDefinitionVariable(
      this Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable srvBuildDefinitionVariable,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvBuildDefinitionVariable == null)
        return (Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable) null;
      return new Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable(securedObject)
      {
        Value = srvBuildDefinitionVariable.Value,
        AllowOverride = srvBuildDefinitionVariable.AllowOverride,
        IsSecret = srvBuildDefinitionVariable.IsSecret
      };
    }

    public static Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable ToServerBuildDefinitionVariable(
      this Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable webApiBuilDefinitionVariable)
    {
      if (webApiBuilDefinitionVariable == null)
        return (Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable) null;
      return new Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable()
      {
        Value = webApiBuilDefinitionVariable.Value,
        AllowOverride = webApiBuilDefinitionVariable.AllowOverride,
        IsSecret = webApiBuilDefinitionVariable.IsSecret
      };
    }

    public static Dictionary<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable> ToWebApiVariables(
      this IDictionary<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable> srvVariables,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvVariables == null)
        return (Dictionary<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable>) null;
      Dictionary<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable> webApiVariables = new Dictionary<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable> srvVariable in (IEnumerable<KeyValuePair<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable>>) srvVariables)
        webApiVariables.Add(srvVariable.Key, srvVariable.Value.ToWebApiBuildDefinitionVariable(securedObject));
      return webApiVariables;
    }

    public static Dictionary<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable> ToServerVariables(
      this IDictionary<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable> srvRepositoriesDictionary)
    {
      if (srvRepositoriesDictionary == null)
        return (Dictionary<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable>) null;
      Dictionary<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable> serverVariables = new Dictionary<string, Microsoft.TeamFoundation.Build2.Server.BuildDefinitionVariable>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable> srvRepositories in (IEnumerable<KeyValuePair<string, Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionVariable>>) srvRepositoriesDictionary)
        serverVariables.Add(srvRepositories.Key, srvRepositories.Value.ToServerBuildDefinitionVariable());
      return serverVariables;
    }
  }
}
