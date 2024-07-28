// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Models.GettingStartedModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Models
{
  public class GettingStartedModel
  {
    public Guid JobId { get; set; }

    public bool CreateTeamProject { get; set; }

    public Guid CollectionId { get; set; }

    public string AccountUrl { get; set; }

    public bool CollectionExists { get; set; }

    public bool IsProjectCreationLockdownMode { get; set; }

    public bool ProjectExists { get; set; }

    public bool ShowGitProjectSupport { get; set; }
  }
}
