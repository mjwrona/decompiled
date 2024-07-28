// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Core.Server.EndpointContributionValidator
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Core.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7318EB94-86FC-4B6F-8A5A-8BD0659030A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Core.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Core.Server
{
  public static class EndpointContributionValidator
  {
    private static readonly HashSet<string> ValidAuthSchemes = new HashSet<string>()
    {
      "ms.vss-endpoint.endpoint-auth-scheme-azure-cert",
      "ms.vss-endpoint.endpoint-auth-scheme-azure-storage",
      "ms.vss-endpoint.endpoint-auth-scheme-basic",
      "ms.vss-endpoint.endpoint-auth-scheme-jwt",
      "ms.vss-endpoint.endpoint-auth-scheme-cert",
      "ms.vss-endpoint.endpoint-auth-scheme-workload-identity-federation",
      "ms.vss-endpoint.endpoint-auth-scheme-service-principal",
      "ms.vss-endpoint.endpoint-auth-scheme-managed-service-identity",
      "ms.vss-endpoint.endpoint-auth-scheme-publishprofile",
      "ms.vss-endpoint.endpoint-auth-scheme-token",
      "ms.vss-endpoint.endpoint-auth-scheme-none",
      "ms.vss-endpoint.endpoint-auth-scheme-oauth2",
      "ms.vss-endpoint.endpoint-auth-scheme-kubernetes",
      "ms.vss-endpoint.endpoint-auth-scheme-jiraconnectapp",
      "ms.vss-endpoint.endpoint-auth-scheme-installationtoken"
    };
    private static readonly List<EndpointContributionValidator.PropertyValidator> PropertyValidators = new List<EndpointContributionValidator.PropertyValidator>()
    {
      (EndpointContributionValidator.PropertyValidator) ((IVssRequestContext requestContext, ServiceEndpointType endpointType, ref string s) => EndpointContributionValidator.ValidateEndpointName(requestContext, endpointType, ref s)),
      (EndpointContributionValidator.PropertyValidator) ((IVssRequestContext requestContext, ServiceEndpointType endpointType, ref string s) => EndpointContributionValidator.ValidateDisplayName(requestContext, endpointType, ref s)),
      (EndpointContributionValidator.PropertyValidator) ((IVssRequestContext requestContext, ServiceEndpointType endpointType, ref string s) => EndpointContributionValidator.ValidateAuthenticationScheme(requestContext, endpointType, ref s)),
      (EndpointContributionValidator.PropertyValidator) ((IVssRequestContext requestContext, ServiceEndpointType endpointType, ref string s) => EndpointContributionValidator.ValidateDependencyEndpointUrls(requestContext, endpointType, ref s))
    };

    public static void CheckEndpointContribution(
      IVssRequestContext requestContext,
      JObject properties)
    {
      if (properties == null)
        throw new ArgumentNullException(nameof (properties));
      string errorMessage = Resources.EndpointTypeInvalidMessage();
      JsonSerializerSettings settings = new JsonSerializerSettings()
      {
        ContractResolver = (IContractResolver) new EndpointContractResolver()
      };
      ServiceEndpointType endpointType;
      try
      {
        endpointType = properties.ToObject<ServiceEndpointType>(JsonSerializer.Create(settings));
      }
      catch (Exception ex)
      {
        throw new EndpointContributionInvalidException(Resources.ParsingFailedForEndpointType((object) ex.Message), ex);
      }
      if (!EndpointContributionValidator.PropertyValidators.Select<EndpointContributionValidator.PropertyValidator, bool>((Func<EndpointContributionValidator.PropertyValidator, bool>) (validator => validator(requestContext, endpointType, ref errorMessage))).Aggregate<bool, bool>(true, (Func<bool, bool, bool>) ((s, b) => s & b)))
        throw new EndpointContributionInvalidException(errorMessage);
    }

    private static bool ValidateEndpointName(
      IVssRequestContext requestContext,
      ServiceEndpointType endpointType,
      ref string errorMessage)
    {
      bool flag = !string.IsNullOrWhiteSpace(endpointType.Name);
      errorMessage += flag ? string.Empty : Resources.EndpointNameInvalidMessage();
      return flag;
    }

    private static bool ValidateDisplayName(
      IVssRequestContext requestContext,
      ServiceEndpointType endpointType,
      ref string errorMessage)
    {
      bool flag = !string.IsNullOrWhiteSpace(endpointType.DisplayName);
      errorMessage += flag ? string.Empty : Resources.EndpointDisplayNameInvalidMessage();
      return flag;
    }

    private static bool ValidateAuthenticationScheme(
      IVssRequestContext requestContext,
      ServiceEndpointType endpointType,
      ref string errorMessage)
    {
      bool flag1 = endpointType.AuthenticationSchemes.Count != 0;
      errorMessage += flag1 ? string.Empty : Resources.EndpointTypeMissingAuthScheme();
      bool flag2 = endpointType.AuthenticationSchemes.All<ServiceEndpointAuthenticationScheme>((Func<ServiceEndpointAuthenticationScheme, bool>) (x => EndpointContributionValidator.ValidAuthSchemes.Any<string>((Func<string, bool>) (y => string.Equals(x.Scheme, y, StringComparison.OrdinalIgnoreCase)))));
      if (!flag2)
      {
        string str = endpointType.AuthenticationSchemes.Where<ServiceEndpointAuthenticationScheme>((Func<ServiceEndpointAuthenticationScheme, bool>) (x => !EndpointContributionValidator.ValidAuthSchemes.Any<string>((Func<string, bool>) (y => string.Equals(x.Scheme, y, StringComparison.OrdinalIgnoreCase))))).Aggregate<ServiceEndpointAuthenticationScheme, string>(string.Empty, (Func<string, ServiceEndpointAuthenticationScheme, string>) ((s, scheme) => s + scheme.Scheme + ","));
        errorMessage += Resources.EndpointTypeInvalidAuthSchemes((object) str);
      }
      return flag1 & flag2;
    }

    private static bool ValidateDependencyEndpointUrls(
      IVssRequestContext requestContext,
      ServiceEndpointType endpointType,
      ref string errorMessage)
    {
      bool flag = true;
      if (endpointType?.EndpointUrl?.DependsOn?.Map != null)
      {
        foreach (DependencyBinding dependencyBinding in endpointType.EndpointUrl.DependsOn.Map)
        {
          if (!string.IsNullOrEmpty(dependencyBinding.Value) && !Uri.TryCreate(dependencyBinding.Value.Trim(), UriKind.Absolute, out Uri _))
          {
            flag = false;
            errorMessage += Resources.InvalidUrlForEndpoint((object) dependencyBinding.Value);
          }
        }
      }
      return flag;
    }

    public static void ValidateContributionsAtPublishTime(
      IVssRequestContext requestContext,
      Dictionary<string, string> toBePublishedContributions,
      string publisherName,
      string extensionName)
    {
      EndpointContributionValidator.CheckIfContributionsReuseServiceEndpointType(toBePublishedContributions);
      EndpointContributionValidator.CheckIfContributionsConflictWithExisting(requestContext, toBePublishedContributions, publisherName, extensionName);
    }

    public static void CheckIfContributionsConflictWithExisting(
      IVssRequestContext requestContext,
      Dictionary<string, string> toBePublishedContributions,
      string publisherName,
      string extensionName)
    {
      ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
      List<ArtifactSpec> artifactSpecList = new List<ArtifactSpec>()
      {
        EndpointContributionValidator.CreateExtensionNameArtifactSpec(publisherName, extensionName, GalleryConstants.ExtensionNameArtifactKind)
      };
      List<string> propertyNameFilters = new List<string>()
      {
        "Microsoft.VisualStudio.Services.ServiceEndpointName" + (object) '*'
      };
      Dictionary<string, string> source = new Dictionary<string, string>();
      using (TeamFoundationDataReader properties = service.GetProperties(requestContext, (IEnumerable<ArtifactSpec>) artifactSpecList, (IEnumerable<string>) propertyNameFilters))
      {
        if (properties == null)
          return;
        foreach (ArtifactPropertyValue current in properties.CurrentEnumerable<ArtifactPropertyValue>())
        {
          foreach (PropertyValue propertyValue in current.PropertyValues)
          {
            if (propertyValue != null)
              source.Add(propertyValue.PropertyName, propertyValue.Value.ToString());
          }
        }
      }
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IDictionary<string, string>) toBePublishedContributions);
      if (source.Count > 0)
      {
        foreach (KeyValuePair<string, string> publishedContribution in toBePublishedContributions)
        {
          if (source.Contains<KeyValuePair<string, string>>(publishedContribution))
            dictionary.Remove(publishedContribution.Key);
        }
      }
      if (dictionary == null || dictionary.Count <= 0)
        return;
      HashSet<string> stringSet = new HashSet<string>();
      propertyNameFilters.Clear();
      propertyNameFilters.Add("Microsoft.VisualStudio.Services.ServiceEndpointName" + "*");
      using (TeamFoundationDataReader properties = service.GetProperties(requestContext, GalleryConstants.ExtensionNameArtifactKind, (IEnumerable<string>) propertyNameFilters))
      {
        if (properties == null)
          return;
        foreach (ArtifactPropertyValue current in properties.CurrentEnumerable<ArtifactPropertyValue>())
        {
          foreach (PropertyValue propertyValue in current.PropertyValues)
          {
            if (propertyValue != null)
              stringSet.Add(propertyValue.Value.ToString());
          }
        }
      }
      foreach (KeyValuePair<string, string> keyValuePair in dictionary)
      {
        if (stringSet.Contains(keyValuePair.Value))
          throw new EndpointContributionInvalidException(Resources.ContributionUsesExistingServiceEndpointName((object) keyValuePair.Key, (object) keyValuePair.Value));
      }
    }

    public static ArtifactSpec CreateExtensionNameArtifactSpec(
      string PublisherName,
      string ExtensionName,
      Guid artifactKind)
    {
      return new ArtifactSpec(artifactKind, EndpointContributionValidator.CreateFullyQualifiedName(PublisherName, ExtensionName), 0);
    }

    public static string CreateFullyQualifiedName(string publisherName, string extensionName) => string.Format("{0}.{1}", (object) publisherName, (object) extensionName);

    public static void CheckIfContributionsReuseServiceEndpointType(
      Dictionary<string, string> toBePublishedContributions)
    {
      using (IEnumerator<IGrouping<string, string>> enumerator = toBePublishedContributions.ToLookup<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (x => x.Value), (Func<KeyValuePair<string, string>, string>) (x => x.Key)).Where<IGrouping<string, string>>((Func<IGrouping<string, string>, bool>) (x => x.Count<string>() > 1)).GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          IGrouping<string, string> current = enumerator.Current;
          throw new EndpointContributionInvalidException(Resources.ContributionsWithSameServiceEndpointName((object) current.Aggregate<string>((Func<string, string, string>) ((s, v) => s + ", " + v)), (object) current.Key));
        }
      }
    }

    private delegate bool PropertyValidator(
      IVssRequestContext requestContext,
      ServiceEndpointType endpointType,
      ref string errorMessage);
  }
}
