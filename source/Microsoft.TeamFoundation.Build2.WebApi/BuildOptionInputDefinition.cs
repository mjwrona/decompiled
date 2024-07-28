// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildOptionInputDefinition
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class BuildOptionInputDefinition : BaseSecuredObject
  {
    [DataMember(Name = "Options", EmitDefaultValue = false)]
    private Dictionary<string, string> m_Options;
    [DataMember(Name = "Help", EmitDefaultValue = false)]
    private Dictionary<string, string> m_HelpDocuments;

    public BuildOptionInputDefinition()
      : this((ISecuredObject) null)
    {
    }

    internal BuildOptionInputDefinition(ISecuredObject securedObject)
      : base(securedObject)
    {
      this.InputType = BuildOptionInputType.String;
      this.DefaultValue = string.Empty;
      this.Required = false;
    }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Label { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DefaultValue { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool Required { get; set; }

    [DataMember(Name = "Type")]
    public BuildOptionInputType InputType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string VisibleRule { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string GroupName { get; set; }

    public Dictionary<string, string> Options
    {
      get
      {
        if (this.m_Options == null)
          this.m_Options = new Dictionary<string, string>();
        return this.m_Options;
      }
      set => this.m_Options = value;
    }

    public Dictionary<string, string> HelpDocuments
    {
      get
      {
        if (this.m_HelpDocuments == null)
          this.m_HelpDocuments = new Dictionary<string, string>();
        return this.m_HelpDocuments;
      }
      set => this.m_HelpDocuments = new Dictionary<string, string>((IDictionary<string, string>) value);
    }
  }
}
