// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.Widget
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  [DataContract]
  public class Widget : DashboardSecuredObject
  {
    private WidgetMetadata metaData;

    public Widget()
    {
      this.Size = new WidgetSize();
      this.Position = new WidgetPosition();
      this.ArtifactId = string.Empty;
      this.metaData = new WidgetMetadata();
    }

    public Widget(Widget widget)
    {
      this.Name = widget.Name;
      this.Size = widget.Size.Clone();
      this.Position = widget.Position.Clone();
      this.ArtifactId = string.Empty;
      this.metaData = widget.metaData.Clone();
      this.Settings = widget.Settings;
    }

    [DataMember(EmitDefaultValue = false)]
    public Guid? Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ETag { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public WidgetPosition Position { get; set; }

    [DataMember]
    public WidgetSize Size { get; set; }

    [DataMember]
    public string Settings { get; set; }

    [DataMember]
    public SemanticVersion SettingsVersion { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public WidgetSize[] AllowedSizes { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ArtifactId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Dashboard Dashboard { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool AreSettingsBlockedForUser { get; set; }

    public void SetMetaData(WidgetMetadata metadata) => this.metaData = metadata;

    [DataMember]
    public bool IsEnabled => this.metaData.IsEnabled;

    [DataMember]
    public Uri ContentUri => !this.metaData.IsEnabled ? (Uri) null : this.metaData.ContentUri;

    [DataMember]
    public string ContributionId
    {
      get => this.metaData.ContributionId;
      set
      {
        if (this.metaData == null)
          this.metaData = new WidgetMetadata();
        this.metaData.ContributionId = value;
      }
    }

    [DataMember]
    public string TypeId => this.metaData.TypeId;

    [DataMember]
    public string ConfigurationContributionId => this.metaData.ConfigurationContributionId;

    [DataMember]
    public string ConfigurationContributionRelativeId => this.metaData.ConfigurationContributionRelativeId;

    [DataMember]
    public bool IsNameConfigurable => this.metaData.IsNameConfigurable;

    [DataMember]
    public string LoadingImageUrl => this.metaData.LoadingImageUrl;

    [DataMember(EmitDefaultValue = false)]
    public LightboxOptions LightboxOptions => this.metaData.LightboxOptions;

    [IgnoreDataMember]
    public bool CanStoreCrossProjectSettings => this.metaData.CanStoreCrossProjectSettings;

    public bool IsNew()
    {
      if (!this.Id.HasValue)
        return true;
      Guid? id = this.Id;
      Guid empty = Guid.Empty;
      if (!id.HasValue)
        return false;
      return !id.HasValue || id.GetValueOrDefault() == empty;
    }

    public bool IsPositioned() => this.Position.IsPositioned();

    protected override void SetSecuredChildren(ISecuredObject securedObject)
    {
      if (this.Dashboard != null)
        this.Dashboard.SetSecuredObject(securedObject);
      if (this.LightboxOptions != null)
        this.LightboxOptions.SetSecuredObject(securedObject);
      if (this.Position != null)
        this.Position.SetSecuredObject(securedObject);
      if (this.Size != null)
        this.Size.SetSecuredObject(securedObject);
      if (this.SettingsVersion != null)
        this.SettingsVersion.SetSecuredObject(securedObject);
      if (this.AllowedSizes == null)
        return;
      foreach (DashboardSecuredObject allowedSiz in this.AllowedSizes)
        allowedSiz.SetSecuredObject(securedObject);
    }
  }
}
