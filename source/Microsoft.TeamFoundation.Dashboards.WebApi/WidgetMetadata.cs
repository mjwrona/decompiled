// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.WidgetMetadata
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  [DataContract]
  public class WidgetMetadata : DashboardSecuredObject
  {
    public WidgetMetadata CloneWithSecuredObject(ISecuredObject securedObject)
    {
      WidgetMetadata widgetMetadata = this.Clone();
      widgetMetadata.SetSecuredObject(securedObject);
      return widgetMetadata;
    }

    public WidgetMetadata Clone()
    {
      WidgetMetadata widgetMetadata = this.MemberwiseClone() as WidgetMetadata;
      if (this.LightboxOptions != null)
        widgetMetadata.LightboxOptions = new LightboxOptions(this.LightboxOptions.Width, this.LightboxOptions.Height, new bool?(this.LightboxOptions.Resizable));
      return widgetMetadata;
    }

    [DataMember]
    public string TypeId { get; set; }

    [DataMember]
    public string PublisherName { get; set; }

    [DataMember]
    public List<string> Targets { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string CatalogIconUrl { get; set; }

    [DataMember]
    public List<string> Tags { get; set; }

    [DataMember]
    public List<string> Keywords { get; set; }

    [DataMember]
    public string LoadingImageUrl { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public bool IsVisibleFromCatalog { get; set; }

    [DataMember]
    public bool IsEnabled { get; set; }

    [IgnoreDataMember]
    public bool CanStoreCrossProjectSettings { get; set; }

    [DataMember]
    public Uri ContentUri { get; set; }

    [DataMember]
    public IEnumerable<WidgetSize> AllowedSizes { get; set; }

    [DataMember]
    public string ConfigurationContributionId { get; set; }

    [DataMember]
    public string ConfigurationContributionRelativeId { get; set; }

    [DataMember]
    public bool ConfigurationRequired { get; set; }

    [DataMember]
    public string DefaultSettings { get; set; }

    [DataMember]
    public IEnumerable<WidgetScope> SupportedScopes { get; set; }

    [DataMember]
    public string ContributionId { get; set; }

    [DataMember]
    public bool IsNameConfigurable { get; set; }

    [DataMember]
    public string CatalogInfoUrl { get; set; }

    [DataMember]
    public LightboxOptions LightboxOptions { get; set; }

    [DataMember]
    public bool AnalyticsServiceRequired { get; set; }

    public bool IsValid() => ((this.AllowedSizes == null ? 0 : (this.AllowedSizes.Any<WidgetSize>((Func<WidgetSize, bool>) (ws => ws.IsValid())) ? 1 : 0)) & (this.SupportedScopes == null ? (false ? 1 : 0) : (this.SupportedScopes.Count<WidgetScope>() > 0 ? 1 : 0))) != 0 && this.ContributionId != null && this.ContributionId.Length <= 1000 && this.Name != null && this.Name.Length < 256;

    public static WidgetMetadata CreateDisabledInstance(string contributionId) => new WidgetMetadata()
    {
      IsEnabled = false,
      ContributionId = contributionId
    };

    protected override void SetSecuredChildren(ISecuredObject securedObject)
    {
      if (this.LightboxOptions == null)
        return;
      this.LightboxOptions.SetSecuredObject(securedObject);
    }
  }
}
