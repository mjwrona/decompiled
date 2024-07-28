// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Charting.WebApi.ChartConfiguration
// Assembly: Microsoft.TeamFoundation.Charting.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D43D663A-A882-4856-B0B7-D0E666C5CC4A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Charting.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Charting.WebApi
{
  [DataContract]
  public class ChartConfiguration : SecuredChartObject
  {
    [DataMember]
    public Guid? ChartId { get; set; }

    [DataMember]
    public string Scope { get; set; }

    [DataMember]
    public string GroupKey { get; set; }

    [DataMember]
    public string Title { get; set; }

    [DataMember]
    public IEnumerable<ColorConfiguration> UserColors { get; set; }

    [DataMember]
    public string ChartType { get; set; }

    [DataMember]
    public TransformOptions TransformOptions { get; set; }

    protected override void UpdateSecuredObjectOfChildren(ISecuredObject securedObject)
    {
      this.TransformOptions.SetSecuredObject(securedObject);
      if (this.UserColors == null)
        return;
      foreach (SecuredChartObject userColor in this.UserColors)
        userColor.SetSecuredObject(securedObject);
    }
  }
}
