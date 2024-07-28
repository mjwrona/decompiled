// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.ContributedTemplateServiceBase`1
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.TextTemplating;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal abstract class ContributedTemplateServiceBase<T> where T : ContributedTemplateBase
  {
    protected const string c_templateCriteriaProperty = "criteria";
    protected const string c_templateRankProperty = "rank";
    protected const string c_Area = "Notifications";
    protected const string c_Layer = "TransformService";
    private const string c_associatedDataType = "Microsoft.VisualStudio.Services.Notifications.Server.ContributedTemplate";

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public T GetTemplate(IVssRequestContext requestContext, string templateContributionId)
    {
      if (string.IsNullOrEmpty(templateContributionId))
        throw new ArgumentNullException(CoreRes.MustacheTemplateInvalidArgumentsMessage());
      Contribution contribution = (Contribution) null;
      IContributionServiceWithData service = requestContext.GetService<IContributionServiceWithData>();
      string cacheKey = ContributionUtils.GetCacheKey("Microsoft.VisualStudio.Services.Notifications.Server.ContributedTemplate", (IEnumerable<string>) new string[1]
      {
        templateContributionId
      }, (HashSet<string>) null, new ContributionQueryOptions?());
      T associatedData1;
      if (!service.QueryContribution<T>(requestContext, templateContributionId, cacheKey, out contribution, out associatedData1))
      {
        if (contribution == null)
        {
          requestContext.ThrowIfContributionsInFallbackMode();
          throw new NotificationTemplateNotFoundException(CoreRes.TemplateNotFoundException((object) templateContributionId));
        }
        associatedData1 = this.ParseTemplate(requestContext, contribution);
        IContributionServiceWithData contributionServiceWithData = service;
        IVssRequestContext requestContext1 = requestContext;
        string associatedDataName = cacheKey;
        List<Contribution> contributionList = new List<Contribution>();
        contributionList.Add(contribution);
        // ISSUE: variable of a boxed type
        __Boxed<T> associatedData2 = (object) associatedData1;
        contributionServiceWithData.Set(requestContext1, associatedDataName, (IEnumerable<Contribution>) contributionList, (object) associatedData2);
      }
      return associatedData1;
    }

    public List<T> GetTemplatesForEvent(
      IVssRequestContext requestContext,
      string eventType,
      HashSet<string> templateTypes)
    {
      IEnumerable<string> contributionIds = (IEnumerable<string>) new string[1]
      {
        eventType
      };
      ContributionQueryOptions queryOptions = ContributionQueryOptions.IncludeChildren;
      IContributionServiceWithData service = requestContext.GetService<IContributionServiceWithData>();
      string cacheKey = ContributionUtils.GetCacheKey("Microsoft.VisualStudio.Services.Notifications.Server.ContributedTemplate", contributionIds, templateTypes, new ContributionQueryOptions?(queryOptions));
      IEnumerable<Contribution> contributions;
      List<T> associatedData;
      if (!service.QueryContributions<List<T>>(requestContext, contributionIds, templateTypes, queryOptions, (ContributionQueryCallback) null, (ContributionDiagnostics) null, cacheKey, out contributions, out associatedData))
      {
        associatedData = new List<T>();
        if (contributions.Any<Contribution>())
        {
          foreach (Contribution templateContribution in contributions)
          {
            T template = this.ParseTemplate(requestContext, templateContribution);
            associatedData.Add(template);
          }
        }
        service.Set(requestContext, cacheKey, contributions, (object) associatedData);
      }
      return associatedData;
    }

    public T SelectEventTemplate(
      IVssRequestContext requestContext,
      List<T> templateCandidates,
      JObject templateContext)
    {
      int num = int.MinValue;
      T obj = default (T);
      foreach (T templateCandidate in templateCandidates)
      {
        if (templateCandidate.Rank > num)
        {
          bool flag = true;
          if (templateCandidate.Criteria != null)
          {
            MustacheOptions evaluationOptions = HandlebarsTemplateEvaluationHelpers.CreateEvaluationOptions(requestContext, true);
            string b = templateCandidate.Criteria.Evaluate((object) templateContext, (Dictionary<string, object>) null, (MustacheEvaluationContext) null, (Dictionary<string, MustacheRootExpression>) null, evaluationOptions);
            if (string.IsNullOrEmpty(b) || string.Equals("false", b, StringComparison.OrdinalIgnoreCase))
              flag = false;
          }
          if (flag)
          {
            obj = templateCandidate;
            num = templateCandidate.Rank;
          }
        }
      }
      return obj;
    }

    private T ParseTemplate(IVssRequestContext requestContext, Contribution templateContribution)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      MustacheTemplateParser templateParser = vssRequestContext.GetService<IMustacheTemplateParserService>().GetTemplateParser(vssRequestContext);
      T template = this.ParseTemplate(requestContext, templateParser, templateContribution);
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

    protected abstract T ParseTemplate(
      IVssRequestContext requestContext,
      MustacheTemplateParser templateParser,
      Contribution templateContribution);

    protected TemplateFields ParseFields(MustacheTemplateParser templateParser, JObject fields)
    {
      TemplateFields templateFields = new TemplateFields();
      templateFields.ContextObject = fields;
      this.ParseFieldToken((JToken) fields, templateFields, templateParser, (string) null, (string) null, -1);
      return templateFields;
    }

    private void ParseFieldToken(
      JToken field,
      TemplateFields templateFields,
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
      switch (field.Type)
      {
        case JTokenType.Object:
          using (IEnumerator<KeyValuePair<string, JToken>> enumerator = ((JObject) field).GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              KeyValuePair<string, JToken> current = enumerator.Current;
              this.ParseFieldToken(current.Value, templateFields, templateParser, parentSelector1, current.Key, -1);
            }
            break;
          }
        case JTokenType.Array:
          JArray jarray = (JArray) field;
          for (int index = 0; index < jarray.Count; ++index)
            this.ParseFieldToken(jarray[index], templateFields, templateParser, parentSelector1, (string) null, index);
          break;
        case JTokenType.String:
          MustacheExpression mustacheExpression = templateParser.Parse(field.ToString());
          if (!mustacheExpression.IsContextBased)
            break;
          TemplateFieldReplacement fieldReplacement = new TemplateFieldReplacement();
          fieldReplacement.Expression = mustacheExpression;
          fieldReplacement.ParentSelector = parentSelector;
          fieldReplacement.Key = keyInParentObject;
          fieldReplacement.Index = indexInParentObject;
          if (templateFields.Replacements == null)
            templateFields.Replacements = new List<TemplateFieldReplacement>();
          templateFields.Replacements.Add(fieldReplacement);
          break;
      }
    }
  }
}
