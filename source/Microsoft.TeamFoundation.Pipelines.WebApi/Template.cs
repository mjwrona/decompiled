// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.WebApi.Template
// Assembly: Microsoft.TeamFoundation.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 29F2A1B3-A3F7-4291-91FA-6C4508EECB65
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Pipelines.WebApi
{
  [DataContract]
  public class Template
  {
    private IReadOnlyList<TemplateParameterDefinition> m_parameters;
    private IReadOnlyList<TemplateDataSourceBinding> m_dataSourceBindings;
    private IReadOnlyList<TemplateAsset> m_assets;

    [DataMember]
    public string Category { get; set; }

    [DataMember]
    public string Content { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public int RecommendedWeight { get; set; }

    [DataMember]
    public string IconUrl { get; set; }

    [DataMember]
    public IReadOnlyList<TemplateParameterDefinition> Parameters
    {
      get
      {
        if (this.m_parameters == null)
          this.m_parameters = (IReadOnlyList<TemplateParameterDefinition>) new List<TemplateParameterDefinition>().AsReadOnly();
        return this.m_parameters;
      }
      set
      {
        if (value == null)
          return;
        this.m_parameters = (IReadOnlyList<TemplateParameterDefinition>) new List<TemplateParameterDefinition>((IEnumerable<TemplateParameterDefinition>) value).AsReadOnly();
      }
    }

    [DataMember]
    public IReadOnlyList<TemplateDataSourceBinding> DataSourceBindings
    {
      get
      {
        if (this.m_dataSourceBindings == null)
          this.m_dataSourceBindings = (IReadOnlyList<TemplateDataSourceBinding>) new List<TemplateDataSourceBinding>().AsReadOnly();
        return this.m_dataSourceBindings;
      }
      set
      {
        if (value == null)
          return;
        this.m_dataSourceBindings = (IReadOnlyList<TemplateDataSourceBinding>) new List<TemplateDataSourceBinding>((IEnumerable<TemplateDataSourceBinding>) value).AsReadOnly();
      }
    }

    [DataMember]
    public IReadOnlyList<TemplateAsset> Assets
    {
      get
      {
        if (this.m_assets == null)
          this.m_assets = (IReadOnlyList<TemplateAsset>) new List<TemplateAsset>().AsReadOnly();
        return this.m_assets;
      }
      set
      {
        if (value == null)
          return;
        this.m_assets = (IReadOnlyList<TemplateAsset>) new List<TemplateAsset>((IEnumerable<TemplateAsset>) value).AsReadOnly();
      }
    }
  }
}
