// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.DataSourceDetails
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
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
    public string ResourceUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ResultSelector { get; set; }

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
