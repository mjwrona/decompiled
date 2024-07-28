// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseStartMetadataExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public static class ReleaseStartMetadataExtensions
  {
    public static CreateReleaseParameters ToCreateReleaseParameters(
      this ReleaseStartMetadata releaseStartMetadata,
      IVssRequestContext requestContext)
    {
      IList<ArtifactMetadata> artifactsMetadata = releaseStartMetadata != null ? ReleaseStartMetadataExtensions.ValidateArtifact(releaseStartMetadata) : throw new ArgumentNullException(nameof (releaseStartMetadata));
      string str = string.IsNullOrEmpty(releaseStartMetadata.Description) ? string.Empty : releaseStartMetadata.Description;
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason releaseReason = releaseStartMetadata.Reason == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReason.None ? Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseReason.Manual : releaseStartMetadata.Reason.FromWebApi();
      CreateReleaseParameters releaseParameters = new CreateReleaseParameters()
      {
        DefinitionId = releaseStartMetadata.DefinitionId,
        Description = str,
        IsDraft = releaseStartMetadata.IsDraft,
        CreatedBy = requestContext.GetUserId(true),
        Reason = releaseReason
      };
      releaseParameters.CreatedFor = releaseStartMetadata.CreatedFor;
      releaseParameters.PopulateArtifactVersions(artifactsMetadata);
      ReleaseManagementArtifactPropertyKinds.CopyProperties(releaseParameters.Properties, releaseStartMetadata.Properties);
      if (releaseStartMetadata.Variables != null)
      {
        foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variable in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) releaseStartMetadata.Variables)
          releaseParameters.Variables[variable.Key] = variable.Value.ToServerConfigurationVariableValue();
      }
      if (releaseStartMetadata.EnvironmentsMetadata != null)
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStartEnvironmentMetadata environmentMetadata in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseStartEnvironmentMetadata>) releaseStartMetadata.EnvironmentsMetadata)
        {
          Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseStartEnvironmentMetadata serverEnvironmentMetadata = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseStartEnvironmentMetadata()
          {
            DefinitionEnvironmentId = environmentMetadata.DefinitionEnvironmentId
          };
          IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variables = environmentMetadata.Variables;
          if (variables != null)
            variables.ForEach<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>((Action<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) (v => serverEnvironmentMetadata.Variables.Add(v.Key, v.Value.ToServerConfigurationVariableValue())));
          releaseParameters.EnvironmentsMetadata.Add(serverEnvironmentMetadata);
        }
      }
      if (releaseStartMetadata.ManualEnvironments != null)
      {
        foreach (string manualEnvironment in (IEnumerable<string>) releaseStartMetadata.ManualEnvironments)
          releaseParameters.ManualEnvironments.Add(manualEnvironment);
      }
      return releaseParameters;
    }

    private static IList<ArtifactMetadata> ValidateArtifact(
      ReleaseStartMetadata releaseStartMetadata)
    {
      IList<ArtifactMetadata> artifacts = releaseStartMetadata.Artifacts;
      if (artifacts != null)
      {
        foreach (ArtifactMetadata artifactMetadata in (IEnumerable<ArtifactMetadata>) artifacts)
        {
          if (artifactMetadata.Alias.IsNullOrEmpty<char>())
            throw new InvalidRequestException(Resources.ArtifactSourceAliasCannotBeEmpty);
          ReleaseStartMetadataExtensions.ValidateVersion(artifactMetadata.Alias, artifactMetadata.InstanceReference);
        }
        if (!releaseStartMetadata.IsDraft)
          ReleaseStartMetadataExtensions.ValidateArtifactForUniqueAlias(artifacts);
      }
      return artifacts;
    }

    private static void ValidateArtifactForUniqueAlias(IList<ArtifactMetadata> artifactsMetadata)
    {
      Dictionary<string, int> dictionary = artifactsMetadata.GroupBy<ArtifactMetadata, string>((Func<ArtifactMetadata, string>) (artifact => artifact.Alias), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Where<IGrouping<string, ArtifactMetadata>>((Func<IGrouping<string, ArtifactMetadata>, bool>) (g => g.Count<ArtifactMetadata>() > 1)).ToDictionary<IGrouping<string, ArtifactMetadata>, string, int>((Func<IGrouping<string, ArtifactMetadata>, string>) (x => x.Key), (Func<IGrouping<string, ArtifactMetadata>, int>) (y => y.Count<ArtifactMetadata>()));
      if (dictionary.Count > 0)
      {
        string enumerable = string.Empty;
        foreach (KeyValuePair<string, int> keyValuePair in dictionary)
          enumerable = !enumerable.IsNullOrEmpty<char>() ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0},{1}", (object) enumerable, (object) keyValuePair.Key) : keyValuePair.Key;
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DuplicateArtifactSourceAliasFound, (object) enumerable));
      }
    }

    private static void ValidateVersion(string alias, BuildVersion artifactVersionInfo)
    {
      if (artifactVersionInfo == null || string.IsNullOrWhiteSpace(artifactVersionInfo.Id))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseArtifactVersionIdCannotBeEmpty, (object) alias));
    }
  }
}
