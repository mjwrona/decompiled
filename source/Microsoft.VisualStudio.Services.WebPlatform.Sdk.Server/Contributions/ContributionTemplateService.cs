// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.ContributionTemplateService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  internal class ContributionTemplateService : IContributionTemplateService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IContributionTemplate GetTemplate(
      IVssRequestContext requestContext,
      string templateContributionId,
      string templateName)
    {
      IContributionTemplate template = (IContributionTemplate) null;
      Contribution templateContribution = requestContext.GetService<IContributionService>().QueryContribution(requestContext, templateContributionId);
      if (templateContribution != null)
        template = this.GetTemplate(requestContext, templateContribution, templateName);
      return template;
    }

    public IContributionTemplate GetTemplate(
      IVssRequestContext requestContext,
      Contribution templateContribution,
      string templateName)
    {
      string key = string.Format("templates:{0}:{1}", (object) templateContribution.GetHashCode(), (object) templateContribution.Id);
      IDictionary<string, IContributionTemplate> dictionary = templateContribution.GetAssociatedObject<IDictionary<string, IContributionTemplate>>(key);
      if (dictionary == null)
      {
        dictionary = (IDictionary<string, IContributionTemplate>) new Dictionary<string, IContributionTemplate>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        ContributionTemplateService.TemplateDefinition property = templateContribution.GetProperty<ContributionTemplateService.TemplateDefinition>("template");
        if (property != null)
        {
          if (!string.IsNullOrEmpty(property.Asset) && !string.IsNullOrEmpty(property.Type))
            dictionary[string.Empty] = this.LoadTemplate(requestContext, templateContribution, property);
          if (property.Named != null)
          {
            foreach (KeyValuePair<string, ContributionTemplateService.TemplateDefinition> keyValuePair in property.Named)
            {
              if (!string.IsNullOrEmpty(keyValuePair.Value.Asset) && !string.IsNullOrEmpty(keyValuePair.Value.Type))
                dictionary[keyValuePair.Key] = this.LoadTemplate(requestContext, templateContribution, keyValuePair.Value);
            }
          }
        }
        templateContribution.SetAssociatedObject<IDictionary<string, IContributionTemplate>>(key, dictionary);
      }
      IContributionTemplate template;
      dictionary.TryGetValue(templateName, out template);
      return template;
    }

    private IContributionTemplate LoadTemplate(
      IVssRequestContext requestContext,
      Contribution templateContribution,
      ContributionTemplateService.TemplateDefinition templateDefinition)
    {
      IContributionService service = requestContext.GetService<IContributionService>();
      string id = templateContribution.Id;
      string asset = templateDefinition.Asset;
      if (!string.IsNullOrEmpty(asset) && asset.Contains(":"))
      {
        string[] strArray = asset.Split(':');
        if (strArray.Length == 2)
        {
          id = strArray[0];
          asset = strArray[1];
        }
      }
      using (Stream stream = service.QueryAsset(requestContext, id, asset).SyncResult<Stream>())
      {
        if (stream == null)
          throw new ContributionAssetNotFoundException(WebFrameworkResources.TemplateAssetInvalidFormat((object) templateDefinition.Asset, (object) templateContribution.Id));
        IContributionTemplate contributionTemplate = (IContributionTemplate) null;
        using (StreamReader templateStream = new StreamReader(stream))
        {
          if (!templateDefinition.Type.Equals("hbs", StringComparison.OrdinalIgnoreCase))
            throw new ContributionTemplateTypeInvalidException(WebFrameworkResources.InvalidTemplateTypeFormat((object) templateDefinition.Type, (object) templateDefinition.Asset, (object) templateContribution.Id));
          contributionTemplate = (IContributionTemplate) new HandlebarContributionTemplate(requestContext, templateDefinition.Asset, templateDefinition.ContentType, templateStream);
        }
        return contributionTemplate;
      }
    }

    [DataContract]
    private class TemplateDefinition
    {
      [DataMember(Name = "asset")]
      public string Asset;
      [DataMember(Name = "type")]
      public string Type;
      [DataMember(Name = "contentType")]
      public string ContentType;
      [DataMember(Name = "named")]
      public Dictionary<string, ContributionTemplateService.TemplateDefinition> Named;
    }
  }
}
