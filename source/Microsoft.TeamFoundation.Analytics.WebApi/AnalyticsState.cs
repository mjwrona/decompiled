// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.WebApi.AnalyticsState
// Assembly: Microsoft.TeamFoundation.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 055A63EC-DEB5-4484-8793-E8DBCC9AC203
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Analytics.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Analytics.WebApi
{
  [DataContract]
  public class AnalyticsState
  {
    [DataMember]
    public bool Enable { get; set; }
  }
}
