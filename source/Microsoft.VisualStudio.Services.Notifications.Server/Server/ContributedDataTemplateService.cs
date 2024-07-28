// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.ContributedDataTemplateService
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class ContributedDataTemplateService : 
    ContributedTemplateServiceBase<ContributedDataTemplate>,
    IContributedDataTemplateService,
    IContributedTemplateServiceBase<ContributedDataTemplate>,
    IVssFrameworkService
  {
    private const string c_dataProperty = "data";

    protected override ContributedDataTemplate ParseTemplate(
      IVssRequestContext requestContext,
      MustacheTemplateParser templateParser,
      Contribution templateContribution)
    {
      Dictionary<string, TemplateFields> dictionary = new Dictionary<string, TemplateFields>();
      JObject inputs1;
      if (templateContribution.Properties.TryGetValue<JObject>("data", out inputs1))
      {
        TemplateFields inputs2 = this.ParseInputs(templateParser, inputs1);
        dictionary[templateContribution.Id] = inputs2;
      }
      ContributedDataTemplate contributedDataTemplate = new ContributedDataTemplate();
      contributedDataTemplate.Id = templateContribution.Id;
      contributedDataTemplate.TemplateFields = dictionary;
      ContributedDataTemplate template = contributedDataTemplate;
      string property = templateContribution.GetProperty<string>("criteria");
      if (!string.IsNullOrEmpty(property))
      {
        template.Criteria = templateParser.Parse(property);
        template.Rank = templateContribution.GetProperty<int>("rank", 1);
      }
      else
        template.Rank = templateContribution.GetProperty<int>("rank");
      return template;
    }

    private TemplateFields ParseInputs(MustacheTemplateParser templateParser, JObject inputs)
    {
      TemplateFields templateInputs = new TemplateFields();
      templateInputs.ContextObject = inputs;
      this.ParseInputToken((JToken) inputs, templateInputs, templateParser, (string) null, (string) null, -1);
      return templateInputs;
    }

    private void ParseInputToken(
      JToken input,
      TemplateFields templateInputs,
      MustacheTemplateParser templateParser,
      string parentSelector,
      string keyInParentObject,
      int indexInParentObject)
    {
      string parentSelector1 = parentSelector;
      if (string.IsNullOrEmpty(parentSelector1))
        parentSelector1 = string.Empty;
      if (!string.IsNullOrEmpty(keyInParentObject))
        parentSelector1 = parentSelector1 + "['" + keyInParentObject + "']";
      else if (indexInParentObject >= 0)
        parentSelector1 = parentSelector1 + "[" + indexInParentObject.ToString() + "]";
      switch (input.Type)
      {
        case JTokenType.Object:
          using (IEnumerator<KeyValuePair<string, JToken>> enumerator = ((JObject) input).GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              KeyValuePair<string, JToken> current = enumerator.Current;
              this.ParseInputToken(current.Value, templateInputs, templateParser, parentSelector1, current.Key, -1);
            }
            break;
          }
        case JTokenType.Array:
          JArray jarray = (JArray) input;
          for (int index = 0; index < jarray.Count; ++index)
            this.ParseInputToken(jarray[index], templateInputs, templateParser, parentSelector1, (string) null, index);
          break;
        case JTokenType.String:
          MustacheExpression mustacheExpression = templateParser.Parse(input.ToString());
          if (!mustacheExpression.IsContextBased)
            break;
          TemplateFieldReplacement fieldReplacement = new TemplateFieldReplacement();
          fieldReplacement.Expression = mustacheExpression;
          fieldReplacement.ParentSelector = parentSelector;
          fieldReplacement.Key = keyInParentObject;
          fieldReplacement.Index = indexInParentObject;
          if (templateInputs.Replacements == null)
            templateInputs.Replacements = new List<TemplateFieldReplacement>();
          templateInputs.Replacements.Add(fieldReplacement);
          break;
      }
    }
  }
}
