// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildTemplateContributionProvider
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildTemplateContributionProvider : IBuildTemplateContributionProvider
  {
    private static readonly string s_layer = nameof (BuildTemplateContributionProvider);
    private static readonly string s_rootPropertyName = "name";
    private static readonly string s_templateContributionIdentifier = "ms.vss-build.templates";
    public static readonly string[] s_contributionTargets = new string[1]
    {
      BuildTemplateContributionProvider.s_templateContributionIdentifier
    };

    public List<BuildDefinitionTemplate> GetTemplates(
      IVssRequestContext requestContext,
      string templateId = null)
    {
      List<BuildDefinitionTemplate> templates = new List<BuildDefinitionTemplate>();
      IContributionService service = requestContext.GetService<IContributionService>();
      IEnumerable<Contribution> source = service.QueryContributions(requestContext, (IEnumerable<string>) BuildTemplateContributionProvider.s_contributionTargets, queryOptions: ContributionQueryOptions.IncludeChildren);
      if (source == null || source.Count<Contribution>() == 0)
        requestContext.TraceInfo(12030090, BuildTemplateContributionProvider.s_layer, "No build template extension contributions were found");
      Dictionary<string, List<Contribution>> dictionary = new Dictionary<string, List<Contribution>>();
      foreach (Contribution contribution in source)
      {
        ContributionProviderDetails contributionProviderDetails = service.QueryContributionProviderDetails(requestContext, contribution.Id);
        requestContext.TraceInfo(12030089, BuildTemplateContributionProvider.s_layer, "Parsing contribution {0} with version {1}", (object) contribution.Id, (object) contributionProviderDetails.Version);
        List<Contribution> contributionList;
        if (!dictionary.TryGetValue(contributionProviderDetails.Name, out contributionList))
        {
          contributionList = new List<Contribution>();
          dictionary.Add(contributionProviderDetails.Name, contributionList);
        }
        contributionList.Add(contribution);
      }
      foreach (KeyValuePair<string, List<Contribution>> keyValuePair in dictionary)
      {
        try
        {
          string id = keyValuePair.Value.FirstOrDefault<Contribution>().Id;
          using (Stream stream = service.QueryAsset(requestContext, id, "Microsoft.VisualStudio.Services.VSIXPackage").SyncResult<Stream>())
          {
            stream.Seek(0L, SeekOrigin.Begin);
            using (Package package = Package.Open(stream))
            {
              foreach (Contribution contribution in keyValuePair.Value)
              {
                try
                {
                  string templateRoot = this.GetTemplateRoot(contribution);
                  PackagePart part = package.GetPart(new Uri(templateRoot, UriKind.Relative));
                  if (part != null)
                  {
                    BuildDefinitionTemplate templateFromFileStream = this.GetTemplateFromFileStream(part.GetStream(), templateRoot);
                    templateFromFileStream.Id = string.Format("{0}.{1}.{2}", (object) keyValuePair.Key, (object) contribution.Id, (object) templateFromFileStream.Id);
                    templates.Add(templateFromFileStream);
                    if (!string.IsNullOrEmpty(templateId))
                    {
                      if (string.Equals(templateFromFileStream.Id, templateId, StringComparison.OrdinalIgnoreCase))
                        return templates;
                    }
                  }
                  else
                    requestContext.TraceError(12030089, BuildTemplateContributionProvider.s_layer, "Cannot get package for URI {0} for extension {1}", (object) templateRoot, (object) keyValuePair.Key);
                }
                catch (Exception ex)
                {
                  requestContext.TraceError(12030089, BuildTemplateContributionProvider.s_layer, "Cannot get template for contribtution with id {0}, exception:{1}", (object) contribution.Id, (object) ex.ToString());
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(BuildTemplateContributionProvider.s_layer, ex);
        }
      }
      return templates;
    }

    private string GetTemplateRoot(Contribution contribution)
    {
      string str1;
      if (!contribution.Properties.TryGetValue<string>(BuildTemplateContributionProvider.s_rootPropertyName, out str1) || string.IsNullOrEmpty(str1))
        throw new InvalidTemplateException(BuildServerResources.ContributionDoesntTargetTemplate((object) contribution.Id, (object) BuildTemplateContributionProvider.s_rootPropertyName));
      string str2 = str1.StartsWith("/", StringComparison.OrdinalIgnoreCase) ? str1 : "/" + str1;
      return (str2.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? str2 : str2 + "/") + "template.json";
    }

    private BuildDefinitionTemplate GetTemplateFromFileStream(Stream fileStream, string path)
    {
      try
      {
        return ServerBuildDefinitionHelpers.GetTemplateFromStream(fileStream);
      }
      catch (Exception ex)
      {
        throw new InvalidTemplateException(BuildServerResources.BuildDefinitionTemplateSerializationError((object) path) + ex.Message);
      }
    }
  }
}
