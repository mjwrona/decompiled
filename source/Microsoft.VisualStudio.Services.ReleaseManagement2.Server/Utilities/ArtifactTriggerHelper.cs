// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.ArtifactTriggerHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public static class ArtifactTriggerHelper
  {
    private const string UniqueSourceIdentifierFormat = "{{{{{0}}}}}";

    public static IList<ReleaseTriggerBase> GetTriggers(
      ReleaseDefinition definition,
      string artifactType)
    {
      IList<ReleaseTriggerBase> triggers = (IList<ReleaseTriggerBase>) new List<ReleaseTriggerBase>();
      if (definition != null && definition.Triggers != null)
      {
        foreach (ReleaseTriggerBase trigger in (IEnumerable<ReleaseTriggerBase>) definition.Triggers)
        {
          switch (trigger.TriggerType)
          {
            case ReleaseTriggerType.ArtifactSource:
              ArtifactSourceTrigger artifactSourceTrigger = (ArtifactSourceTrigger) trigger;
              if (definition.LinkedArtifacts.FirstOrDefault<ArtifactSource>((Func<ArtifactSource, bool>) (x => x.Alias == artifactSourceTrigger.Alias && x.ArtifactTypeId.Equals(artifactType, StringComparison.OrdinalIgnoreCase))) != null)
              {
                triggers.Add((ReleaseTriggerBase) artifactSourceTrigger);
                continue;
              }
              continue;
            case ReleaseTriggerType.SourceRepo:
              SourceRepoTrigger sourceRepoTrigger = (SourceRepoTrigger) trigger;
              if (definition.LinkedArtifacts.FirstOrDefault<ArtifactSource>((Func<ArtifactSource, bool>) (x => x.Alias == sourceRepoTrigger.Alias && x.ArtifactTypeId.Equals(artifactType, StringComparison.OrdinalIgnoreCase))) != null)
              {
                triggers.Add((ReleaseTriggerBase) sourceRepoTrigger);
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
      return triggers;
    }

    public static ArtifactSource GetArtifactSource(
      ReleaseDefinition definition,
      ReleaseTriggerBase trigger)
    {
      ArtifactSource artifactSource1 = (ArtifactSource) null;
      ReleaseTriggerType? triggerType = trigger?.TriggerType;
      if (triggerType.HasValue)
      {
        switch (triggerType.GetValueOrDefault())
        {
          case ReleaseTriggerType.ArtifactSource:
            ArtifactSourceTrigger artifactSourceTrigger = (ArtifactSourceTrigger) trigger;
            if (artifactSourceTrigger != null)
            {
              ArtifactSource artifactSource2;
              if (definition == null)
              {
                artifactSource2 = (ArtifactSource) null;
              }
              else
              {
                IList<ArtifactSource> linkedArtifacts = definition.LinkedArtifacts;
                artifactSource2 = linkedArtifacts != null ? linkedArtifacts.Where<ArtifactSource>((Func<ArtifactSource, bool>) (x => x.Alias == artifactSourceTrigger.Alias)).SingleOrDefault<ArtifactSource>() : (ArtifactSource) null;
              }
              artifactSource1 = artifactSource2;
              break;
            }
            break;
          case ReleaseTriggerType.SourceRepo:
            SourceRepoTrigger sourceRepoTrigger = (SourceRepoTrigger) trigger;
            if (sourceRepoTrigger != null)
            {
              ArtifactSource artifactSource3;
              if (definition == null)
              {
                artifactSource3 = (ArtifactSource) null;
              }
              else
              {
                IList<ArtifactSource> linkedArtifacts = definition.LinkedArtifacts;
                artifactSource3 = linkedArtifacts != null ? linkedArtifacts.Where<ArtifactSource>((Func<ArtifactSource, bool>) (x => x.Alias == sourceRepoTrigger.Alias)).SingleOrDefault<ArtifactSource>() : (ArtifactSource) null;
              }
              artifactSource1 = artifactSource3;
              break;
            }
            break;
        }
      }
      return artifactSource1;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    public static string GetNormalizedArtifactDefinitionIdentifier(
      IVssRequestContext requestContext,
      Guid projectId,
      ArtifactSource artifactSource,
      IArtifactType artifactType)
    {
      if (artifactSource == null)
        throw new ArgumentNullException(nameof (artifactSource));
      if (artifactType == null)
        throw new ArgumentNullException(nameof (artifactType));
      InputValue inputValue;
      Guid result;
      if (artifactSource.SourceData.TryGetValue("connection", out inputValue) && Guid.TryParse(inputValue.Value, out result))
      {
        string normalizedEndpointUrl = ArtifactTriggerHelper.GetNormalizedEndpointUrl(requestContext, projectId, result);
        if (!string.IsNullOrEmpty(normalizedEndpointUrl))
        {
          if (artifactType.ArtifactTriggerConfiguration != null && artifactType.ArtifactTriggerConfiguration.IsWebhookSupportedAtServerLevel)
            return normalizedEndpointUrl;
          Dictionary<string, string> dictionary = artifactSource.SourceData.ToDictionary<KeyValuePair<string, InputValue>, string, string>((Func<KeyValuePair<string, InputValue>, string>) (pair => pair.Key), (Func<KeyValuePair<string, InputValue>, string>) (pair => pair.Value.Value));
          dictionary["connection"] = normalizedEndpointUrl;
          dictionary["project"] = projectId.ToString();
          JToken replacementContext = JToken.FromObject((object) dictionary);
          return new MustacheTemplateParser().ReplaceValues(artifactType.UniqueSourceIdentifier, (object) replacementContext);
        }
        requestContext.Trace(1976462, TraceLevel.Error, "ReleaseManagementService", "ArtifactTrigger", "Cannot get endpoint {0} from project {1}.", (object) result, (object) projectId);
      }
      else
        requestContext.Trace(1976463, TraceLevel.Error, "ReleaseManagementService", "ArtifactTrigger", "Cannot find connectionId from the artifactSource {0}.", (object) artifactSource.Id);
      return string.Empty;
    }

    public static void QueueJobToTriggerReleases(
      IVssRequestContext requestContext,
      CustomArtifactTriggerEvent customArtifactTriggerEvent)
    {
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) customArtifactTriggerEvent);
      TeamFoundationJobService service = requestContext.GetService<TeamFoundationJobService>();
      string extensionName = "Microsoft.VisualStudio.Services.ReleaseManagement.Server.Plugins.JobExtensions.TriggerReleaseForCustomArtifactJobExtension";
      string jobName = "TriggerReleaseForCustomArtifactJobExtension";
      using (requestContext.AllowAnonymousWrites())
        service.QueueOneTimeJob(requestContext, jobName, extensionName, xml, JobPriorityLevel.Normal);
    }

    public static IList<string> FindMatchingArtifactSources(
      IEnumerable<string> uniqueSourceIdentifiers,
      string uniqueSourceIdentifierTemplate,
      Dictionary<string, string> parameters)
    {
      IList<string> matchingArtifactSources = (IList<string>) new List<string>();
      if (!uniqueSourceIdentifierTemplate.IsNullOrEmpty<char>())
      {
        string enumerable = new MustacheTemplateParser().ReplaceValues(ArtifactTriggerHelper.TrimUniqueSourceIdentifierForConnection(uniqueSourceIdentifierTemplate), (object) JToken.FromObject((object) (parameters ?? new Dictionary<string, string>())));
        if (!enumerable.IsNullOrEmpty<char>())
        {
          foreach (string str in uniqueSourceIdentifiers ?? (IEnumerable<string>) new List<string>())
          {
            if (str.EndsWith(enumerable, StringComparison.OrdinalIgnoreCase))
              matchingArtifactSources.Add(str);
          }
        }
      }
      return matchingArtifactSources;
    }

    private static string TrimUniqueSourceIdentifierForConnection(
      string uniqueSourceIdentifierTemplate)
    {
      if (uniqueSourceIdentifierTemplate.IsNullOrEmpty<char>())
        return (string) null;
      string oldValue = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{{{{0}}}}}", (object) "connection");
      return uniqueSourceIdentifierTemplate.Replace(oldValue, string.Empty).Trim(':');
    }

    private static string GetNormalizedEndpointUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid endpointId)
    {
      string normalizedEndpointUrl = string.Empty;
      ServiceEndpoint serviceEndpoint = requestContext.GetService<IServiceEndpointService2>().GetServiceEndpoint(requestContext, projectId, endpointId);
      if (serviceEndpoint != null && serviceEndpoint.Url != (Uri) null)
        normalizedEndpointUrl = serviceEndpoint.Url.ToString().TrimEnd('/');
      return normalizedEndpointUrl;
    }
  }
}
