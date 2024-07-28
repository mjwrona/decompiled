// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WitIdentityRef
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  public class WitIdentityRef : WorkItemSecuredObject
  {
    public WitIdentityRef()
      : this((string) null)
    {
    }

    public WitIdentityRef(string token)
      : base(16, token)
    {
    }

    public string DistinctDisplayName { get; set; }

    public IdentityRef IdentityRef { get; set; }

    public override string ToString() => this.IdentityRef?.DisplayName;
  }
}
