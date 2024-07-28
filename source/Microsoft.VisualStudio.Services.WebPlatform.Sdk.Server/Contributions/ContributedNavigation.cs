// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.ContributedNavigation
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  [DataContract]
  public class ContributedNavigation : WebSdkMetadata
  {
    [DataMember]
    public string Id;
    [DataMember]
    public string Name;
    [DataMember]
    public int Flags;
    [DataMember(EmitDefaultValue = false)]
    public string MenuId;
    [DataMember]
    public double Order = (double) int.MaxValue;
    [DataMember(EmitDefaultValue = false)]
    public string ApplicationId;
    [DataMember(EmitDefaultValue = false)]
    public object Icon;
    [DataMember(EmitDefaultValue = false)]
    public string IconName;
    [DataMember(EmitDefaultValue = false)]
    public string IconAsset;
    [DataMember(EmitDefaultValue = false)]
    public string NavGroup;
    [DataMember(EmitDefaultValue = false)]
    public string ComponentType;
    [DataMember(EmitDefaultValue = false)]
    public object ComponentProperties;
    [DataMember(EmitDefaultValue = false)]
    public string[] DisplayedDocTypes;
    [DataMember(EmitDefaultValue = false)]
    public bool SupportsMobile;
    [IgnoreDataMember]
    public string Type;
    [IgnoreDataMember]
    public string DefaultRoute;

    public ContributedNavigation(
      IVssRequestContext requestContext,
      ContributionNode contributionNode)
    {
      this.Id = contributionNode.Id;
      this.Type = contributionNode.Contribution.Type;
      int attributes;
      contributionNode.Contribution.Properties.TryGetValue<int>("::Attributes", out attributes);
      if ((attributes & 1) == 1)
        this.Flags |= 1;
      bool propertyValue1;
      if ((attributes & 64) == 64)
      {
        this.Flags |= 2;
        contributionNode.TryGetProperty<string>(requestContext, "navGroup", out this.NavGroup);
        if (contributionNode.TryGetProperty<bool>(requestContext, "noncollapsible", out propertyValue1) & propertyValue1)
          this.Flags |= 64;
        if (contributionNode.TryGetProperty<string>(requestContext, "componentType", out this.ComponentType))
          contributionNode.TryGetProperty<object>(requestContext, "componentProperties", out this.ComponentProperties);
        contributionNode.TryGetProperty<string>(requestContext, "defaultRoute", out this.DefaultRoute);
        contributionNode.TryGetProperty<string>(requestContext, "menuId", out this.MenuId);
      }
      contributionNode.TryGetProperty<double>(requestContext, "order", out this.Order);
      contributionNode.TryGetProperty<string>(requestContext, "name", out this.Name);
      if (contributionNode.TryGetProperty<bool>(requestContext, "disabled", out propertyValue1) & propertyValue1)
        this.Flags |= 16;
      if (contributionNode.TryGetProperty<bool>(requestContext, "hidden", out propertyValue1) & propertyValue1)
        this.Flags |= 32;
      contributionNode.TryGetProperty<string[]>(requestContext, "displayedDocTypes", out this.DisplayedDocTypes);
      contributionNode.TryGetProperty<string>(requestContext, "iconName", out this.IconName);
      contributionNode.TryGetProperty<string>(requestContext, "iconAsset", out this.IconAsset);
      contributionNode.TryGetProperty<string>(requestContext, "applicationId", out this.ApplicationId);
      contributionNode.TryGetProperty<bool>(requestContext, "supportsMobile", out this.SupportsMobile);
      string propertyValue2;
      if (contributionNode.TryGetProperty<string>(requestContext, "icon", out propertyValue2))
      {
        this.Icon = (object) this.ResolveIconProperty(requestContext, propertyValue2, contributionNode, attributes);
      }
      else
      {
        IDictionary<string, string> propertyValue3;
        if (!contributionNode.TryGetProperty<IDictionary<string, string>>(requestContext, "icon", out propertyValue3))
          return;
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) propertyValue3)
        {
          string str = this.ResolveIconProperty(requestContext, keyValuePair.Value, contributionNode, attributes);
          if (!string.IsNullOrEmpty(str))
            dictionary[keyValuePair.Key] = str;
        }
        this.Icon = (object) dictionary;
      }
    }

    private string ResolveIconProperty(
      IVssRequestContext requestContext,
      string propertyValue,
      ContributionNode contributionNode,
      int attributes)
    {
      Uri result;
      return !Uri.TryCreate(propertyValue, UriKind.Absolute, out result) ? ((attributes & 64) != 64 ? requestContext.GetService<IContributionService>().QueryAssetLocation(requestContext, contributionNode.Id, propertyValue) : propertyValue) : (!result.Scheme.Equals("asset", StringComparison.OrdinalIgnoreCase) ? result.AbsolutePath : requestContext.GetService<IContributionService>().QueryAssetLocation(requestContext, contributionNode.Id, result.GetComponents(UriComponents.Host | UriComponents.Path, UriFormat.UriEscaped)));
    }

    public override string ToString() => this.Id + "[" + this.Type + "]";
  }
}
