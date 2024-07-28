// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentitySystemUserService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Identity
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class IdentitySystemUserService : IVssIdentitySystemUserService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public Microsoft.VisualStudio.Services.Identity.Identity CreateVersionControlIdentity(
      IVssRequestContext collectionRequestContext,
      string domain,
      string accountName)
    {
      collectionRequestContext.CheckProjectCollectionRequestContext();
      Microsoft.VisualStudio.Services.Identity.Identity user = collectionRequestContext.GetService<IdentityService>().IdentityServiceInternal().CreateUser(collectionRequestContext, collectionRequestContext.ServiceHost.InstanceId, (string) null, domain, accountName, "Version Control identity");
      return collectionRequestContext.GetService<IVssIdentityRetrievalService>().GetActiveOrHistoricalMember(collectionRequestContext, user.Descriptor);
    }
  }
}
