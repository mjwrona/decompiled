// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement.ContributedFeature
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement
{
  [DataContract]
  public class ContributedFeature
  {
    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? ServiceInstanceType { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<ContributedFeatureSettingScope> Scopes { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool DefaultState { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<ContributedFeatureValueRule> DefaultValueRules { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<ContributedFeatureValueRule> OverrideRules { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<ContributedFeatureListener> FeatureStateChangedListeners { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IncludeAsClaim { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<string> Tags { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int Order { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Dictionary<string, object> FeatureProperties { get; set; }
  }
}
