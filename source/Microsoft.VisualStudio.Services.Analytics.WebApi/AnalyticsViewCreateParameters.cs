// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WebApi.AnalyticsViewCreateParameters
// Assembly: Microsoft.VisualStudio.Services.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F858B048-FFE8-4E5F-8EBC-9B25DAC23DF8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.WebApi
{
  [DataContract]
  public class AnalyticsViewCreateParameters
  {
    public AnalyticsViewCreateParameters()
    {
      this.Visibility = AnalyticsViewVisibility.Undefined;
      this.ViewType = AnalyticsViewType.Undefined;
    }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public AnalyticsViewVisibility Visibility { get; set; }

    [DataMember]
    public AnalyticsViewType ViewType { get; set; }

    [DataMember]
    public string Definition { get; set; }
  }
}
