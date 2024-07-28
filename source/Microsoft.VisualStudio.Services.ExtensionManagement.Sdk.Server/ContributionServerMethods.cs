// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ContributionServerMethods
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  public static class ContributionServerMethods
  {
    private static JsonSerializer s_serializer = new VssJsonMediaTypeFormatter().CreateJsonSerializer();
    private static MustacheTemplateParser s_defaultTemplateParser = new MustacheTemplateParser();

    public static string GetTemplateProperty(
      this Contribution contribution,
      IVssRequestContext requestContext,
      string propertyName,
      object replacementValues,
      MustacheTemplateParser customTemplateParser = null)
    {
      return ContributionServerMethods.ResolveTemplatedProperty(contribution.GetProperty<string>(propertyName), replacementValues, customTemplateParser);
    }

    public static string GetTemplateUriProperty(
      this Contribution contribution,
      IVssRequestContext requestContext,
      string propertyName,
      object replacementValues,
      MustacheTemplateParser customTemplateParser = null)
    {
      string property1 = contribution.GetProperty<string>(propertyName);
      string assetType = (string) null;
      if (property1 != null)
      {
        assetType = ContributionServerMethods.ResolveTemplatedProperty(property1, replacementValues, customTemplateParser);
        if (assetType == null || !assetType.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !assetType.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
          IContributionService service = requestContext.GetService<IContributionService>();
          string property2 = contribution.GetProperty<string>("::BaseUri");
          if (string.IsNullOrEmpty(property2) && requestContext != null)
          {
            ContributionProviderDetails contributionProviderDetails = service.QueryContributionProviderDetails(requestContext, contribution.Id);
            if (contributionProviderDetails != null && contributionProviderDetails.Properties != null)
              contributionProviderDetails.Properties.TryGetValue("::BaseUri", out property2);
          }
          if (!string.IsNullOrEmpty(property2))
          {
            string str = ContributionServerMethods.ResolveTemplatedProperty(property2, replacementValues, customTemplateParser);
            if (!string.IsNullOrEmpty(str))
            {
              if (string.IsNullOrEmpty(assetType))
                assetType = str;
              else
                assetType = str.TrimEnd('/') + "/" + assetType.TrimStart('/');
            }
          }
          else if (!string.IsNullOrEmpty(assetType))
            assetType = service.QueryAssetLocation(requestContext, contribution.Id, assetType);
        }
      }
      return assetType;
    }

    private static string ResolveTemplatedProperty(
      string propertyValue,
      object replacementValues,
      MustacheTemplateParser customTemplateParser = null)
    {
      if (!string.IsNullOrEmpty(propertyValue))
      {
        MustacheExpression mustacheExpression = (customTemplateParser == null ? ContributionServerMethods.s_defaultTemplateParser : customTemplateParser).Parse(propertyValue);
        if (mustacheExpression.IsContextBased)
          replacementValues = (object) ContributionServerMethods.ObjectToJToken(replacementValues);
        propertyValue = mustacheExpression.Evaluate(replacementValues);
      }
      return propertyValue;
    }

    private static JToken ObjectToJToken(object objectToConvert)
    {
      switch (objectToConvert)
      {
        case JToken jtoken:
        case null:
          return jtoken;
        default:
          jtoken = JToken.FromObject(objectToConvert, ContributionServerMethods.s_serializer);
          goto case null;
      }
    }
  }
}
