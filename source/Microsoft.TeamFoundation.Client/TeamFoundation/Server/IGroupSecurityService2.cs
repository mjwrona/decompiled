// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.IGroupSecurityService2
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using System;

namespace Microsoft.TeamFoundation.Server
{
  [Obsolete("IGroupSecurityService2 is obsolete.  Please use the IIdentityManagementService2 or ISecurityService instead.", false)]
  public interface IGroupSecurityService2 : IGroupSecurityService
  {
    IResultCollection<Identity> GetIdentityChanges(int sequenceId, out int lastSequenceId);
  }
}
