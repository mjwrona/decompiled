// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.ReleaseWebHookHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public static class ReleaseWebHookHelper
  {
    public static CustomArtifactTriggerEvent GetCustomArtifactTriggerEventFromPayload(
      string eventPayload,
      IArtifactType artifactType)
    {
      if (artifactType == null)
        throw new ArgumentNullException(nameof (artifactType));
      if (string.IsNullOrEmpty(eventPayload))
        throw new ArgumentNullException(nameof (eventPayload));
      string template1;
      string template2;
      if (artifactType.ArtifactTriggerConfiguration?.WebhookPayloadMapping == null || !artifactType.ArtifactTriggerConfiguration.WebhookPayloadMapping.TryGetValue("definition", out template1) || !artifactType.ArtifactTriggerConfiguration.WebhookPayloadMapping.TryGetValue("version", out template2))
        throw new WebHookException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.ArtifactWebHookPayloadMappingNotDefinedCorrectly, (object) artifactType?.Name));
      JToken replacementContext = JToken.Parse(eventPayload);
      MustacheTemplateParser mustacheTemplateParser = new MustacheTemplateParser();
      string str1 = mustacheTemplateParser.ReplaceValues(template1, (object) replacementContext);
      string str2 = mustacheTemplateParser.ReplaceValues(template2, (object) replacementContext);
      if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
        throw new WebHookException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.CannotGetDefinitionOrVersionFromPayload, (object) eventPayload));
      return new CustomArtifactTriggerEvent()
      {
        Definition = str1,
        Version = str2,
        ArtifactType = artifactType.Name,
        UniqueSourceIdentifierTemplate = artifactType.UniqueSourceIdentifier
      };
    }
  }
}
