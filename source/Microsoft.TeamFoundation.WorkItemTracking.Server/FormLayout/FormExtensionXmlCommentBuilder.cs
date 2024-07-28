// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.FormExtensionXmlCommentBuilder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  public class FormExtensionXmlCommentBuilder
  {
    public static string GetCommentBlobForExtensionInjection(
      IEnumerable<Contribution> formContributions)
    {
      Dictionary<string, List<Contribution>> dictionary = new Dictionary<string, List<Contribution>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      StringBuilder stringBuilder = new StringBuilder();
      string str1 = "http://go.microsoft.com/fwlink/?LinkId=816513";
      if (formContributions != null)
        formContributions = FormExtensionXmlCommentBuilder.FilterValidFormContributions(formContributions);
      if (formContributions != null && formContributions.Any<Contribution>())
      {
        foreach (Contribution formContribution in formContributions)
        {
          ContributionIdentifier contributionIdentifier = new ContributionIdentifier(formContribution.Id);
          string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(contributionIdentifier.PublisherName, contributionIdentifier.ExtensionName);
          if (!dictionary.ContainsKey(fullyQualifiedName))
            dictionary.Add(fullyQualifiedName, new List<Contribution>()
            {
              formContribution
            });
          else
            dictionary[fullyQualifiedName].Add(formContribution);
        }
        stringBuilder.AppendLine(ServerResources.WorkItemExtensions());
        foreach (string key in dictionary.Keys)
        {
          List<Contribution> source1 = dictionary[key];
          ContributionIdentifier contributionIdentifier = new ContributionIdentifier(source1.First<Contribution>().Id);
          stringBuilder.AppendLine();
          stringBuilder.AppendLine(ServerResources.Extension());
          stringBuilder.AppendLine("\t" + ServerResources.Name() + ": " + contributionIdentifier.ExtensionName);
          stringBuilder.AppendLine("\t" + ServerResources.ExtensionId() + ": " + key);
          foreach (Contribution contribution in source1)
          {
            if (VssStringComparer.ExtensionType.Equals(contribution.Type, "ms.vss-work-web.work-item-form-page") || VssStringComparer.ExtensionType.Equals(contribution.Type, "ms.vss-work-web.work-item-form-group") || VssStringComparer.ExtensionType.Equals(contribution.Type, "ms.vss-work-web.work-item-form-control"))
            {
              stringBuilder.AppendLine();
              if (VssStringComparer.ExtensionType.Equals(contribution.Type, "ms.vss-work-web.work-item-form-page"))
                stringBuilder.AppendLine("\t" + ServerResources.PageContribution() + ":");
              else if (VssStringComparer.ExtensionType.Equals(contribution.Type, "ms.vss-work-web.work-item-form-group"))
                stringBuilder.AppendLine("\t" + ServerResources.GroupContribution() + ":");
              else if (VssStringComparer.ExtensionType.Equals(contribution.Type, "ms.vss-work-web.work-item-form-control"))
                stringBuilder.AppendLine("\t" + ServerResources.ControlContribution() + ":");
              stringBuilder.AppendLine("\t\t" + ServerResources.ContributionId() + ": " + contribution.Id);
              stringBuilder.AppendLine("\t\t" + ServerResources.Description() + ": " + contribution.Description);
            }
            if (VssStringComparer.ExtensionType.Equals(contribution.Type, "ms.vss-work-web.work-item-form-control") && contribution.Properties != null)
            {
              JToken property = contribution.Properties["inputs"];
              if (property != null && property.Any<JToken>())
              {
                stringBuilder.AppendLine("\t\t" + ServerResources.Inputs() + ":");
                foreach (JToken jtoken1 in (IEnumerable<JToken>) property)
                {
                  stringBuilder.AppendLine("\t\t\t" + ServerResources.ContributionId() + ": " + jtoken1.Value<string>((object) "id"));
                  stringBuilder.AppendLine("\t\t\t" + ServerResources.Description() + ": " + jtoken1.Value<string>((object) "description"));
                  string a = jtoken1.SelectToken("type") != null ? jtoken1.Value<string>((object) "type") : string.Empty;
                  if (!string.IsNullOrEmpty(a))
                  {
                    stringBuilder.AppendLine("\t\t\t" + ServerResources.Type() + ": " + a);
                    if (string.Equals(a, "WorkItemField", StringComparison.OrdinalIgnoreCase))
                    {
                      JToken jtoken2 = jtoken1.SelectToken("properties");
                      if (jtoken2 != null)
                      {
                        JToken source2 = jtoken2.SelectToken("workItemFieldTypes");
                        if (source2 != null && source2.Any<JToken>())
                        {
                          IEnumerable<string> source3 = source2.Values<string>();
                          if (source3 != null && source3.Count<string>() > 0)
                            stringBuilder.AppendLine("\t\t\t" + ServerResources.FieldType() + ": " + string.Join("; ", source3.ToArray<string>()));
                        }
                      }
                    }
                  }
                  JToken jtoken3 = jtoken1.SelectToken("validation");
                  if (jtoken3 != null)
                  {
                    string str2 = jtoken3.SelectToken("dataType") != null ? jtoken3.Value<string>((object) "dataType") : string.Empty;
                    if (!string.IsNullOrEmpty(str2))
                      stringBuilder.AppendLine("\t\t\t" + ServerResources.DataType() + ": " + str2);
                    if (jtoken3.SelectToken("isRequired") != null)
                      stringBuilder.AppendLine("\t\t\t" + ServerResources.IsRequired() + ": " + XmlConvert.ToString(jtoken3.Value<bool>((object) "isRequired")));
                  }
                  stringBuilder.AppendLine();
                }
              }
            }
          }
        }
        stringBuilder.AppendLine();
        stringBuilder.AppendLine(ServerResources.ExtensionMoreInformation());
        stringBuilder.AppendLine(str1);
      }
      return stringBuilder.ToString();
    }

    private static IEnumerable<Contribution> FilterValidFormContributions(
      IEnumerable<Contribution> contributions)
    {
      return contributions.Where<Contribution>((Func<Contribution, bool>) (c => ((IEnumerable<string>) WorkItemFormExtensionsConstants.ValidFormContributionTypes).Contains<string>(c.Type, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)));
    }
  }
}
