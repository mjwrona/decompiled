// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.SuiteUpdateModel
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class SuiteUpdateModel
  {
    public SuiteUpdateModel(
      string name = "",
      ShallowReference parent = null,
      string queryString = "",
      bool? inheritDefaultConfigurations = null,
      ShallowReference[] defaultConfigurations = null)
    {
      if (!string.IsNullOrEmpty(name))
        this.Name = name;
      if (parent != null)
        this.Parent = new ShallowReference(parent);
      if (!string.IsNullOrEmpty(queryString))
        this.QueryString = queryString;
      this.InheritDefaultConfigurations = inheritDefaultConfigurations;
      if (defaultConfigurations == null)
        return;
      this.DefaultConfigurations = new List<ShallowReference>((IEnumerable<ShallowReference>) defaultConfigurations);
    }

    [DataMember]
    public string Name { get; private set; }

    [DataMember]
    public ShallowReference Parent { get; private set; }

    [DataMember]
    public string QueryString { get; private set; }

    [DataMember]
    public bool? InheritDefaultConfigurations { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public List<ShallowReference> DefaultConfigurations { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public List<ShallowReference> DefaultTesters { get; set; }
  }
}
