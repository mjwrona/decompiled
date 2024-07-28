// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Social.WebApi.SocialEngagementStatistics
// Assembly: Microsoft.VisualStudio.Services.Social.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0D2A928F-A131-41A8-A9E6-C3C26BFE105A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.WebApi.dll

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Social.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public class SocialEngagementStatistics
  {
    [DataMember]
    public int UserCount { get; set; }
  }
}
