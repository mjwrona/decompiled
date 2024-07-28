// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestTrigger
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class PullRequestTrigger : ReleaseTriggerBase
  {
    public PullRequestTrigger()
    {
      this.TriggerType = ReleaseTriggerType.PullRequest;
      this.TriggerConditions = (IList<PullRequestFilter>) new List<PullRequestFilter>();
      this.PullRequestConfiguration = new PullRequestConfiguration();
    }

    [DataMember]
    public string ArtifactAlias { get; set; }

    [DataMember]
    public IList<PullRequestFilter> TriggerConditions { get; set; }

    [DataMember]
    public PullRequestConfiguration PullRequestConfiguration { get; set; }

    [DataMember]
    public string StatusPolicyName { get; set; }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      IList<PullRequestFilter> triggerConditions = this.TriggerConditions;
      if (triggerConditions != null)
        triggerConditions.ForEach<PullRequestFilter>((Action<PullRequestFilter>) (t => t.SetSecuredObject(token, requiredPermissions)));
      this.PullRequestConfiguration?.SetSecuredObject(token, requiredPermissions);
    }
  }
}
