// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.BuildOptionDefinitionExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class BuildOptionDefinitionExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.BuildOptionDefinition ToWebApiBuildOptionDefinition(
      this Microsoft.TeamFoundation.Build2.Server.BuildOptionDefinition srvBuildOptionDefinition,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvBuildOptionDefinition == null)
        return (Microsoft.TeamFoundation.Build.WebApi.BuildOptionDefinition) null;
      Microsoft.TeamFoundation.Build.WebApi.BuildOptionDefinition optionDefinition1 = new Microsoft.TeamFoundation.Build.WebApi.BuildOptionDefinition(securedObject);
      optionDefinition1.Id = srvBuildOptionDefinition.Id;
      optionDefinition1.Ordinal = srvBuildOptionDefinition.Ordinal;
      optionDefinition1.Name = srvBuildOptionDefinition.Name;
      optionDefinition1.Description = srvBuildOptionDefinition.Description;
      Microsoft.TeamFoundation.Build.WebApi.BuildOptionDefinition optionDefinition2 = optionDefinition1;
      if (srvBuildOptionDefinition.Inputs != null)
        optionDefinition2.Inputs = (IList<Microsoft.TeamFoundation.Build.WebApi.BuildOptionInputDefinition>) srvBuildOptionDefinition.Inputs.Select<Microsoft.TeamFoundation.Build2.Server.BuildOptionInputDefinition, Microsoft.TeamFoundation.Build.WebApi.BuildOptionInputDefinition>((Func<Microsoft.TeamFoundation.Build2.Server.BuildOptionInputDefinition, Microsoft.TeamFoundation.Build.WebApi.BuildOptionInputDefinition>) (x => x.ToWebApiBuildOptionInputDefinition(securedObject))).ToList<Microsoft.TeamFoundation.Build.WebApi.BuildOptionInputDefinition>();
      if (srvBuildOptionDefinition.Groups != null)
        optionDefinition2.Groups = (IList<Microsoft.TeamFoundation.Build.WebApi.BuildOptionGroupDefinition>) srvBuildOptionDefinition.Groups.Select<Microsoft.TeamFoundation.Build2.Server.BuildOptionGroupDefinition, Microsoft.TeamFoundation.Build.WebApi.BuildOptionGroupDefinition>((Func<Microsoft.TeamFoundation.Build2.Server.BuildOptionGroupDefinition, Microsoft.TeamFoundation.Build.WebApi.BuildOptionGroupDefinition>) (x => x.ToWebApiBuildOptionGroupDefinition(securedObject))).ToList<Microsoft.TeamFoundation.Build.WebApi.BuildOptionGroupDefinition>();
      return optionDefinition2;
    }

    public static Microsoft.TeamFoundation.Build.WebApi.BuildOptionInputDefinition ToWebApiBuildOptionInputDefinition(
      this Microsoft.TeamFoundation.Build2.Server.BuildOptionInputDefinition srvBuildOptionInputDefinition,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvBuildOptionInputDefinition == null)
        return (Microsoft.TeamFoundation.Build.WebApi.BuildOptionInputDefinition) null;
      return new Microsoft.TeamFoundation.Build.WebApi.BuildOptionInputDefinition(securedObject)
      {
        Name = srvBuildOptionInputDefinition.Name,
        Label = srvBuildOptionInputDefinition.Label,
        DefaultValue = srvBuildOptionInputDefinition.DefaultValue,
        Required = srvBuildOptionInputDefinition.Required,
        InputType = (Microsoft.TeamFoundation.Build.WebApi.BuildOptionInputType) srvBuildOptionInputDefinition.InputType,
        VisibleRule = srvBuildOptionInputDefinition.VisibleRule,
        GroupName = srvBuildOptionInputDefinition.GroupName,
        Options = srvBuildOptionInputDefinition.Options,
        HelpDocuments = srvBuildOptionInputDefinition.HelpDocuments
      };
    }

    public static Microsoft.TeamFoundation.Build.WebApi.BuildOptionGroupDefinition ToWebApiBuildOptionGroupDefinition(
      this Microsoft.TeamFoundation.Build2.Server.BuildOptionGroupDefinition srvBuildOptionGroupDefinition,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvBuildOptionGroupDefinition == null)
        return (Microsoft.TeamFoundation.Build.WebApi.BuildOptionGroupDefinition) null;
      return new Microsoft.TeamFoundation.Build.WebApi.BuildOptionGroupDefinition(securedObject)
      {
        Name = srvBuildOptionGroupDefinition.Name,
        DisplayName = srvBuildOptionGroupDefinition.DisplayName,
        IsExpanded = srvBuildOptionGroupDefinition.IsExpanded
      };
    }

    public static BuildOptionDefinitionReference ToWebApiBuildOptionDefinitionReference(
      this Microsoft.TeamFoundation.Build2.Server.BuildOptionDefinition srvBuildOptionDefinition,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvBuildOptionDefinition == null)
        return (BuildOptionDefinitionReference) null;
      return new BuildOptionDefinitionReference(securedObject)
      {
        Id = srvBuildOptionDefinition.Id
      };
    }

    public static Microsoft.TeamFoundation.Build2.Server.BuildOptionDefinition ToServerBuildOptionDefinition(
      this BuildOptionDefinitionReference webApiOptionDefReference)
    {
      if (webApiOptionDefReference == null)
        return (Microsoft.TeamFoundation.Build2.Server.BuildOptionDefinition) null;
      return new Microsoft.TeamFoundation.Build2.Server.BuildOptionDefinition()
      {
        Id = webApiOptionDefReference.Id
      };
    }
  }
}
