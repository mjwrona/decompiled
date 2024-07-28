// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.ContributedTemplateService
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class ContributedTemplateService : 
    ContributedTemplateServiceBase<ContributedTemplate>,
    IContributedTemplateService,
    IContributedTemplateServiceBase<ContributedTemplate>,
    IVssFrameworkService
  {
    internal const string c_templateContributionType = "ms.vss-notifications.template";
    private const string c_inputsProperty = "inputs";
    private const string c_templateProperty = "source";
    private const string c_templateAssetProperty = "sourceAsset";
    private const string c_templateReferenceProperty = "sourceTemplate";

    protected override ContributedTemplate ParseTemplate(
      IVssRequestContext requestContext,
      MustacheTemplateParser templateParser,
      Contribution templateContribution)
    {
      Dictionary<string, TemplateFields> dictionary = new Dictionary<string, TemplateFields>();
      NotificationTransformHandlebarHelpers.RegisterHelpers(templateParser);
      if (templateContribution.Includes != null)
      {
        foreach (string include in templateContribution.Includes)
        {
          ContributedTemplate template = this.GetTemplate(requestContext, include);
          if (template == null)
            throw new NotificationTemplateNotFoundException(CoreRes.TemplateNotFoundException((object) include));
          templateParser.RegisterPartial(include, template.RootExpression);
          foreach (string key in template.TemplateFields.Keys)
            dictionary[key] = template.TemplateFields[key];
        }
      }
      MustacheRootExpression mustacheRootExpression = (MustacheRootExpression) null;
      string property1 = templateContribution.GetProperty<string>("source");
      if (!string.IsNullOrEmpty(property1))
      {
        mustacheRootExpression = (MustacheRootExpression) templateParser.Parse(property1);
      }
      else
      {
        string property2 = templateContribution.GetProperty<string>("sourceAsset");
        if (!string.IsNullOrEmpty(property2))
        {
          IContributionService service = requestContext.GetService<IContributionService>();
          using (Stream stream = service.QueryAsset(requestContext, templateContribution.Id, property2).SyncResult<Stream>())
          {
            if (stream == null)
            {
              ContributionProviderDetails contributionProviderDetails = service.QueryContributionProviderDetails(requestContext, templateContribution.Id);
              bool flag;
              requestContext.RootContext.TryGetItem<bool>("InExtensionFallbackMode", out flag);
              requestContext.Trace(1002506, TraceLevel.Error, "Notifications", "TransformService", string.Format("QueryAsset returned null for asset {0} for email template {1} from contribution {2} version {3} (in fallback mode: {4})", (object) property2, (object) templateContribution.Id, contributionProviderDetails != null ? (object) contributionProviderDetails.Name : (object) "(null)", contributionProviderDetails != null ? (object) contributionProviderDetails.Version : (object) "(null)", (object) flag));
              throw new NotificationTemplateNotFoundException(CoreRes.TemplateNotFoundException((object) templateContribution.Id));
            }
            StreamReader streamReader = new StreamReader(stream);
            try
            {
              mustacheRootExpression = (MustacheRootExpression) templateParser.Parse(streamReader.ReadToEnd());
            }
            catch (ArgumentNullException ex)
            {
              requestContext.Trace(1002506, TraceLevel.Error, "Notifications", "TransformService", string.Format("Failed to load asset {0} for email template {1}", (object) property2, (object) templateContribution.Id));
              throw ex;
            }
          }
        }
        else
        {
          string property3 = templateContribution.GetProperty<string>("sourceTemplate");
          mustacheRootExpression = !string.IsNullOrEmpty(property3) ? (MustacheRootExpression) templateParser.Parse("{{contributedPartial " + property3 + " }}") : throw new NotificationTemplateNotFoundException(CoreRes.MustacheContributedTemplateUndefinedMessage((object) templateContribution.Id));
        }
      }
      JObject fields1;
      if (templateContribution.Properties.TryGetValue<JObject>("inputs", out fields1))
      {
        TemplateFields fields2 = this.ParseFields(templateParser, fields1);
        dictionary[templateContribution.Id] = fields2;
      }
      ContributedTemplate template1 = new ContributedTemplate();
      template1.Id = templateContribution.Id;
      template1.RootExpression = mustacheRootExpression;
      template1.TemplateFields = dictionary;
      return template1;
    }
  }
}
