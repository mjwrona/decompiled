// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamFieldSettings
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class TeamFieldSettings : ITeamFieldSettings
  {
    public TeamFieldSettings()
    {
      this.TeamFieldValues = Array.Empty<ITeamFieldValue>();
      this.DefaultValueIndex = 0;
    }

    public int DefaultValueIndex { get; set; }

    public ITeamFieldValue[] TeamFieldValues { get; set; }
  }
}
