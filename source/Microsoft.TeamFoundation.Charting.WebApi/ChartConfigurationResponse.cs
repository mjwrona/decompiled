// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Charting.WebApi.ChartConfigurationResponse
// Assembly: Microsoft.TeamFoundation.Charting.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D43D663A-A882-4856-B0B7-D0E666C5CC4A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Charting.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Charting.WebApi
{
  [DataContract]
  public class ChartConfigurationResponse : ChartingRestResponseBase, ISecuredObject
  {
    public ChartConfigurationResponse(ChartConfiguration chartConfiguration, string uri)
      : base(uri)
    {
      this.ChartConfiguration = chartConfiguration;
    }

    public ChartConfigurationResponse()
    {
    }

    [DataMember]
    public ChartConfiguration ChartConfiguration { get; private set; }

    Guid ISecuredObject.NamespaceId => ((ISecuredObject) this.ChartConfiguration).NamespaceId;

    int ISecuredObject.RequiredPermissions => ((ISecuredObject) this.ChartConfiguration).RequiredPermissions;

    string ISecuredObject.GetToken() => ((ISecuredObject) this.ChartConfiguration).GetToken();
  }
}
