// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.XamlBuildDefinition
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class XamlBuildDefinition : DefinitionReference
  {
    [DataMember(Name = "_links", EmitDefaultValue = false)]
    private ReferenceLinks m_links;

    public XamlBuildDefinition() => this.Type = DefinitionType.Xaml;

    [DataMember(EmitDefaultValue = false)]
    public BuildController Controller { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int BatchSize { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DefinitionTriggerType TriggerType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int ContinuousIntegrationQuietPeriod { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string DefaultDropLocation { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string BuildArgs { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime CreatedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public BuildReason SupportedReasons { get; set; }

    [Obsolete("Use LastBuildRef instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ShallowReference LastBuild
    {
      get => (ShallowReference) this.LastBuildRef;
      set => this.LastBuildRef = new XamlBuildReference()
      {
        Id = value.Id,
        Name = value.Name,
        Url = value.Url
      };
    }

    [DataMember(Name = "LastBuild", IsRequired = false, EmitDefaultValue = false)]
    public XamlBuildReference LastBuildRef { get; set; }

    public ReferenceLinks Links
    {
      get
      {
        if (this.m_links == null)
          this.m_links = new ReferenceLinks();
        return this.m_links;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public BuildRepository Repository { get; set; }
  }
}
