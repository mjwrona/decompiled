// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.ClientContributionProviderService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal class ClientContributionProviderService : 
    IClientContributionProviderService,
    IVssFrameworkService
  {
    private const string c_sharedDataKey = "_contributions";
    private const string c_extensionDisplayNameProperty = "ExtensionDisplayName";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void AddContribution(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      string contributionId)
    {
      ContributionNode contribution = requestContext.GetService<IContributionManagementService>().GetContribution(requestContext, contributionId);
      if (contribution == null)
        return;
      this.AddContributions(requestContext, sharedData, (IEnumerable<ContributionNode>) new ContributionNode[1]
      {
        contribution
      });
    }

    public void AddContributionsForType(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      string contributionType)
    {
      IEnumerable<ContributionNode> contributionsByType = requestContext.GetService<IContributionManagementService>().GetContributionsByType(requestContext, contributionType);
      if (contributionsByType == null || !contributionsByType.Any<ContributionNode>())
        return;
      this.AddContributions(requestContext, sharedData, contributionsByType);
    }

    public void AddContributions(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      IEnumerable<ContributionNode> contributions)
    {
      IContributionService service = requestContext.GetService<IContributionService>();
      object obj;
      ClientContributionProviderService.ContributionData contributionData;
      if (sharedData.TryGetValue("_contributions", out obj) && obj is ClientContributionProviderService.ContributionData)
      {
        contributionData = obj as ClientContributionProviderService.ContributionData;
      }
      else
      {
        contributionData = new ClientContributionProviderService.ContributionData()
        {
          Contributions = (IDictionary<string, IDictionary<string, object>>) new Dictionary<string, IDictionary<string, object>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase),
          Relationships = (IDictionary<string, ClientContributionProviderService.ContributionRelationships>) new Dictionary<string, ClientContributionProviderService.ContributionRelationships>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase),
          Types = (IDictionary<string, IDictionary<string, bool>>) new Dictionary<string, IDictionary<string, bool>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase),
          Providers = (IDictionary<string, ClientContributionProviderService.ClientContributionProviderDetails>) new Dictionary<string, ClientContributionProviderService.ClientContributionProviderDetails>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
        };
        sharedData.Add("_contributions", (object) contributionData);
      }
      foreach (ContributionNode contribution in contributions)
      {
        IDictionary<string, object> dictionary1 = (IDictionary<string, object>) null;
        if (contribution.Contribution.Properties != null)
        {
          dictionary1 = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          foreach (KeyValuePair<string, JToken> property in contribution.Contribution.Properties)
            dictionary1[property.Key] = contribution.GetProperty<object>(requestContext, property.Key);
        }
        contributionData.Contributions[contribution.Id] = dictionary1;
        IDictionary<string, bool> dictionary2;
        if (!contributionData.Types.TryGetValue(contribution.Contribution.Type, out dictionary2))
        {
          dictionary2 = (IDictionary<string, bool>) new Dictionary<string, bool>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          contributionData.Types[contribution.Contribution.Type] = dictionary2;
        }
        dictionary2[contribution.Id] = true;
        ClientContributionProviderService.ContributionRelationships contributionRelationships;
        if (!contributionData.Relationships.TryGetValue(contribution.Contribution.Id, out contributionRelationships))
        {
          contributionRelationships = new ClientContributionProviderService.ContributionRelationships();
          contributionData.Relationships[contribution.Id] = contributionRelationships;
        }
        if (contribution.Children != null)
        {
          if (contributionRelationships.Children == null)
            contributionRelationships.Children = (IDictionary<string, bool>) new Dictionary<string, bool>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          foreach (ContributionNode child in contribution.Children)
            contributionRelationships.Children[child.Id] = true;
        }
        if (contribution.Parents != null)
        {
          if (contributionRelationships.Parents == null)
            contributionRelationships.Parents = (IDictionary<string, bool>) new Dictionary<string, bool>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          foreach (ContributionNode parent in contribution.Parents)
            contributionRelationships.Parents[parent.Id] = true;
        }
        int num;
        int length;
        if ((contribution.Contribution.GetProperty<int>("::Attributes") & 64) != 64 && (num = contribution.Id.IndexOf(".")) > 0 && (length = contribution.Id.IndexOf(".", num + 1)) > 0)
        {
          string key = contribution.Id.Substring(0, length);
          if (!contributionData.Providers.ContainsKey(key))
          {
            ContributionProviderDetails contributionProviderDetails1 = service.QueryContributionProviderDetails(requestContext, contribution.Id);
            if (contributionProviderDetails1 != null)
            {
              string str1;
              if (!contributionProviderDetails1.Properties.TryGetValue("ExtensionDisplayName", out str1))
                str1 = key;
              ClientContributionProviderService.ClientContributionProviderDetails contributionProviderDetails2 = new ClientContributionProviderService.ClientContributionProviderDetails()
              {
                Version = contributionProviderDetails1.Version,
                DisplayName = str1
              };
              if (string.IsNullOrEmpty(contributionProviderDetails2.Version))
                contributionProviderDetails2.Version = contribution.Contribution.GetProperty<string>("::Version");
              string str2;
              if (contributionProviderDetails1.Properties.TryGetValue("::BaseUri", out str2))
                contributionProviderDetails2.BaseUri = str2;
              string str3;
              if (contributionProviderDetails1.Properties.TryGetValue("::FallbackBaseUri", out str3))
                contributionProviderDetails2.FallbackBaseUri = str3;
              contributionData.Providers[contributionProviderDetails1.Name] = contributionProviderDetails2;
            }
          }
        }
      }
    }

    [DataContract]
    public class ClientContributionProviderDetails : WebSdkMetadata
    {
      [DataMember(EmitDefaultValue = false, Name = "displayName")]
      public string DisplayName;
      [DataMember(EmitDefaultValue = false, Name = "version")]
      public string Version;
      [DataMember(EmitDefaultValue = false, Name = "baseUri")]
      public string BaseUri;
      [DataMember(EmitDefaultValue = false, Name = "fallbackBaseUri")]
      public string FallbackBaseUri;
    }

    [DataContract]
    public class ContributionRelationships : WebSdkMetadata
    {
      [DataMember(EmitDefaultValue = false, Name = "children")]
      public IDictionary<string, bool> Children;
      [DataMember(EmitDefaultValue = false, Name = "parents")]
      public IDictionary<string, bool> Parents;
    }

    [DataContract]
    public class ContributionData : WebSdkMetadata
    {
      [DataMember(Name = "contributions", EmitDefaultValue = false)]
      public IDictionary<string, IDictionary<string, object>> Contributions;
      [DataMember(Name = "relationships", EmitDefaultValue = false)]
      public IDictionary<string, ClientContributionProviderService.ContributionRelationships> Relationships;
      [DataMember(Name = "types", EmitDefaultValue = false)]
      public IDictionary<string, IDictionary<string, bool>> Types;
      [DataMember(Name = "providers", EmitDefaultValue = false)]
      public IDictionary<string, ClientContributionProviderService.ClientContributionProviderDetails> Providers;
    }
  }
}
