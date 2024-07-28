// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview.CurrentUserData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97A9928B-E499-4978-909F-1EBC8C5535AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview
{
  [DataContract]
  public class CurrentUserData : AbstractProjectSecuredObject
  {
    [DataMember]
    public bool IsStakeHolder { get; set; }

    [DataMember]
    public bool HasProjectLevelEditPermission { get; set; }

    [DataMember]
    public Dictionary<UpsellTypes, bool> DismissedUpsells { get; set; }

    [DataMember]
    public bool HasManageProjectPropertiesPermission { get; set; }
  }
}
