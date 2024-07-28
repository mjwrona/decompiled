// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Common.Contracts.DataSourceBindingBase
// Assembly: Microsoft.TeamFoundation.DistributedTask.Common.Contracts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F0CB8220-D93B-49F7-B603-A5F8DA1FAAC3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Common.Contracts.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Common.Contracts
{
  [DataContract]
  public class DataSourceBindingBase : BaseSecuredObject
  {
    private Dictionary<string, string> m_parameters;

    public DataSourceBindingBase()
    {
    }

    protected DataSourceBindingBase(DataSourceBindingBase inputDefinitionToClone)
      : this(inputDefinitionToClone, (ISecuredObject) null)
    {
    }

    protected DataSourceBindingBase(
      DataSourceBindingBase inputDefinitionToClone,
      ISecuredObject securedObject)
      : base(securedObject)
    {
      this.DataSourceName = inputDefinitionToClone.DataSourceName;
      this.EndpointId = inputDefinitionToClone.EndpointId;
      this.Target = inputDefinitionToClone.Target;
      this.ResultTemplate = inputDefinitionToClone.ResultTemplate;
      this.EndpointUrl = inputDefinitionToClone.EndpointUrl;
      this.ResultSelector = inputDefinitionToClone.ResultSelector;
      this.RequestVerb = inputDefinitionToClone.RequestVerb;
      this.RequestContent = inputDefinitionToClone.RequestContent;
      this.CallbackContextTemplate = inputDefinitionToClone.CallbackContextTemplate;
      this.CallbackRequiredTemplate = inputDefinitionToClone.CallbackRequiredTemplate;
      this.InitialContextTemplate = inputDefinitionToClone.InitialContextTemplate;
      inputDefinitionToClone.Parameters.Copy<string, string>((IDictionary<string, string>) this.Parameters);
      this.CloneHeaders(inputDefinitionToClone.Headers);
    }

    [DataMember(EmitDefaultValue = false)]
    public string DataSourceName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Dictionary<string, string> Parameters
    {
      get
      {
        if (this.m_parameters == null)
          this.m_parameters = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_parameters;
      }
    }

    public DataSourceBindingBase Clone(ISecuredObject securedObject) => new DataSourceBindingBase(this, securedObject);

    private void CloneHeaders(List<AuthorizationHeader> headers)
    {
      if (headers == null)
        return;
      this.Headers = headers.Select<AuthorizationHeader, AuthorizationHeader>((Func<AuthorizationHeader, AuthorizationHeader>) (header => new AuthorizationHeader()
      {
        Name = header.Name,
        Value = header.Value
      })).ToList<AuthorizationHeader>();
    }

    [DataMember(EmitDefaultValue = false)]
    public string EndpointId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Target { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ResultTemplate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string RequestVerb { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string RequestContent { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string EndpointUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ResultSelector { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string CallbackContextTemplate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string CallbackRequiredTemplate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string InitialContextTemplate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<AuthorizationHeader> Headers { get; set; }
  }
}
