// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyNotification
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using System;

namespace Microsoft.TeamFoundation.Policy.Server
{
  public abstract class PolicyNotification
  {
    protected PolicyNotification(Guid projectId, DateTime? timeStamp = null)
    {
      this.ProjectId = projectId;
      this.TimeStamp = new DateTime?(timeStamp ?? DateTime.Now);
    }

    public Guid ProjectId { get; private set; }

    public DateTime? TimeStamp { get; private set; }
  }
}
