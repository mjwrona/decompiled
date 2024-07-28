// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DataSourceBinding
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class DataSourceBinding
  {
    private Dictionary<string, string> parameters;

    public DataSourceBinding()
    {
    }

    private DataSourceBinding(DataSourceBinding inputDefinitionToClone)
    {
      this.DataSourceName = inputDefinitionToClone.DataSourceName;
      this.EndpointId = inputDefinitionToClone.EndpointId;
      this.Target = inputDefinitionToClone.Target;
      this.ResultTemplate = inputDefinitionToClone.ResultTemplate;
      this.EndpointUrl = inputDefinitionToClone.EndpointUrl;
      this.ResultSelector = inputDefinitionToClone.ResultSelector;
      this.CallBackRequiredTemplate = inputDefinitionToClone.CallBackRequiredTemplate;
      this.CallbackContextTemplate = inputDefinitionToClone.CallbackContextTemplate;
      this.InitialContextTemplate = inputDefinitionToClone.InitialContextTemplate;
      this.RequestVerb = inputDefinitionToClone.RequestVerb;
      this.RequestContent = inputDefinitionToClone.RequestContent;
      inputDefinitionToClone.Parameters.Copy<string, string>((IDictionary<string, string>) this.Parameters);
    }

    [DataMember(EmitDefaultValue = false)]
    public string DataSourceName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Dictionary<string, string> Parameters
    {
      get
      {
        if (this.parameters == null)
          this.parameters = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.parameters;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public string EndpointId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Target { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ResultTemplate { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "By design.")]
    [DataMember(EmitDefaultValue = false)]
    public string EndpointUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ResultSelector { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string CallBackRequiredTemplate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string CallbackContextTemplate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string InitialContextTemplate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string RequestVerb { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string RequestContent { get; set; }

    public DataSourceBinding Clone() => new DataSourceBinding(this);
  }
}
