// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Proxy.GroupSecurityService2
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Server;
using System;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.Proxy
{
  [Obsolete("IGroupSecurityService2 is obsolete.  Please use the IIdentityManagementService2 or ISecurityService instead.", false)]
  internal class GroupSecurityService2 : 
    GroupSecurityService,
    IGroupSecurityService2,
    IGroupSecurityService
  {
    private Microsoft.TeamFoundation.Proxy.Security.GroupSecurityService2 proxy2;

    internal GroupSecurityService2(TfsTeamProjectCollection tfs, string url)
      : base(tfs, url)
    {
      this.proxy2 = new Microsoft.TeamFoundation.Proxy.Security.GroupSecurityService2(tfs);
    }

    public IResultCollection<Identity> GetIdentityChanges(int sequenceId, out int lastSequenceId)
    {
      try
      {
        SecurityValidation.CheckSequenceId(sequenceId, nameof (sequenceId));
        IResultCollection<Identity> identities;
        lastSequenceId = this.proxy2.GetIdentityChanges((IClientContext) new ClientContext(), sequenceId, out identities);
        return identities;
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }
  }
}
