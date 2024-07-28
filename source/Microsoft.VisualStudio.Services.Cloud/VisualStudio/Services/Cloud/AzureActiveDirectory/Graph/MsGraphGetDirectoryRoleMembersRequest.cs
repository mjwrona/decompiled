// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphGetDirectoryRoleMembersRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  public class MsGraphGetDirectoryRoleMembersRequest : 
    MicrosoftGraphClientRequest<MsGraphGetDirectoryRoleMembersResponse>
  {
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphGetDirectoryRoleMembersRequest";

    public Guid DirectoryRoleObjectId { get; set; }

    internal override MsGraphGetDirectoryRoleMembersResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.TraceConditionally(44750040, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetDirectoryRoleMembersRequest), (Func<string>) (() => string.Format("Calling Microsoft Graph API for Get 'GetDirectoryRoleMembers' with parameter = '{0}'", (object) this.DirectoryRoleObjectId)));
        return new MsGraphGetDirectoryRoleMembersResponse()
        {
          Members = (ISet<DirectoryObject>) ((IEnumerable<DirectoryObject>) (context.RunSynchronously<IDirectoryRoleMembersCollectionWithReferencesPage>((Func<Task<IDirectoryRoleMembersCollectionWithReferencesPage>>) (() => graphServiceClient.DirectoryRoles[this.DirectoryRoleObjectId.ToString()].Members.Request().GetAsync(new CancellationToken()))) ?? throw new MicrosoftGraphException("Microsoft Graph API Get 'GetDirectoryRoleMembers' call returned null"))).ToHashSet<DirectoryObject>()
        };
      }
      finally
      {
        context.Trace(44750042, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetDirectoryRoleMembersRequest), "Leaving Execute method for 'GetDirectoryRoleMembers'");
      }
    }

    public override string ToString() => "MsGraphGetDirectoryRoleMembersRequest{" + string.Format("DirectoryRoleObjectId={0}", (object) this.DirectoryRoleObjectId) + "}";
  }
}
