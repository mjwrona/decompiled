// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.TeamFieldData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public class TeamFieldData : IValidatable
  {
    public int DefaultValueIndex { get; set; }

    public TeamFieldValue[] TeamFieldValues { get; set; }

    public void Validate()
    {
      ArgumentUtility.CheckForNull<TeamFieldValue[]>(this.TeamFieldValues, "TeamFieldValues");
      HashSet<string> stringSet = new HashSet<string>();
      foreach (TeamFieldValue teamFieldValue in this.TeamFieldValues)
      {
        ArgumentUtility.CheckForNull<TeamFieldValue>(teamFieldValue, "teamFieldValue");
        teamFieldValue.Validate();
        if (stringSet.Contains(teamFieldValue.Value))
          throw new ArgumentException("TeamFieldValues");
        stringSet.Add(teamFieldValue.Value);
      }
    }
  }
}
