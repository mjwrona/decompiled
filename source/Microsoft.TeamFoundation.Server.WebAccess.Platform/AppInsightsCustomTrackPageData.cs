// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.AppInsightsCustomTrackPageData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class AppInsightsCustomTrackPageData
  {
    [DataMember(EmitDefaultValue = false)]
    public string PageName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Alias { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Dictionary<string, string> Properties { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Dictionary<string, object> Metrics { get; set; }
  }
}
