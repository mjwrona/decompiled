// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WebApi.AnalyticsView
// Assembly: Microsoft.VisualStudio.Services.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F858B048-FFE8-4E5F-8EBC-9B25DAC23DF8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.WebApi
{
  [DataContract]
  public class AnalyticsView
  {
    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public AnalyticsViewVisibility Visibility { get; set; }

    [DataMember]
    public AnalyticsViewType ViewType { get; set; }

    [DataMember]
    public IdentityRef CreatedBy { get; set; }

    [DataMember]
    public DateTime CreatedDate { get; set; }

    [DataMember]
    public IdentityRef LastModifiedBy { get; set; }

    [DataMember]
    public DateTime LastModifiedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<AnalyticsViewMessage> Messages { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Definition { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public AnalyticsViewQuery Query { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    [DataMember]
    public string Url { get; set; }

    [IgnoreDataMember]
    public AnalyticsViewScope ViewScope { get; set; }

    public AnalyticsView(AnalyticsViewCreateParameters viewCreateParameters)
    {
      this.Id = Guid.NewGuid();
      this.Name = viewCreateParameters.Name;
      this.Description = viewCreateParameters.Description;
      this.Visibility = viewCreateParameters.Visibility;
      this.ViewType = viewCreateParameters.ViewType;
      this.Definition = viewCreateParameters.Definition;
    }

    public AnalyticsView()
    {
    }
  }
}
