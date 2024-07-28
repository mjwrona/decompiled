// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyCountByScopeRecord
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Policy.Server
{
  public sealed class PolicyCountByScopeRecord : BaseSecuredObject
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public PolicyCountByScopeRecord(string scope, int policyCount)
    {
      this.Scope = scope;
      this.PolicyCount = policyCount;
    }

    public string Scope { get; private set; }

    public int PolicyCount { get; private set; }
  }
}
