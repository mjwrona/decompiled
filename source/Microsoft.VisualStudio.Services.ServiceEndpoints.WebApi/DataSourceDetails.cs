// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DataSourceDetails
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [DataContract]
  public class DataSourceDetails
  {
    private Dictionary<string, string> parameters;

    public DataSourceDetails()
    {
    }

    private DataSourceDetails(DataSourceDetails dataSourceDetailsToClone)
    {
      this.DataSourceName = dataSourceDetailsToClone.DataSourceName;
      this.DataSourceUrl = dataSourceDetailsToClone.DataSourceUrl;
      this.ResourceUrl = dataSourceDetailsToClone.ResourceUrl;
      this.ResultSelector = dataSourceDetailsToClone.ResultSelector;
      this.InitialContextTemplate = dataSourceDetailsToClone.InitialContextTemplate;
      this.RequestVerb = dataSourceDetailsToClone.RequestVerb;
      this.RequestContent = dataSourceDetailsToClone.RequestContent;
      Dictionary<string, string> parameters = dataSourceDetailsToClone.Parameters;
      if (parameters != null)
        parameters.Copy<string, string>((IDictionary<string, string>) this.Parameters);
      this.CloneHeaders((IList<AuthorizationHeader>) dataSourceDetailsToClone.Headers);
    }

    [DataMember(EmitDefaultValue = false)]
    public string DataSourceName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DataSourceUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string RequestVerb { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string RequestContent { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ResourceUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ResultSelector { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string InitialContextTemplate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<AuthorizationHeader> Headers { get; set; }

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

    public DataSourceDetails Clone() => new DataSourceDetails(this);

    private void CloneHeaders(IList<AuthorizationHeader> headers)
    {
      if (headers == null)
        return;
      this.Headers = headers.Select<AuthorizationHeader, AuthorizationHeader>((Func<AuthorizationHeader, AuthorizationHeader>) (header => new AuthorizationHeader()
      {
        Name = header.Name,
        Value = header.Value
      })).ToList<AuthorizationHeader>();
    }
  }
}
