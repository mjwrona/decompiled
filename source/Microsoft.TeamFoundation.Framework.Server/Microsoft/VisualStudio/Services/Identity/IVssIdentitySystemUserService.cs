// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IVssIdentitySystemUserService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Identity
{
  [DefaultServiceImplementation(typeof (IdentitySystemUserService))]
  public interface IVssIdentitySystemUserService : IVssFrameworkService
  {
    Microsoft.VisualStudio.Services.Identity.Identity CreateVersionControlIdentity(
      IVssRequestContext collectionRequestContext,
      string domain,
      string accountName);
  }
}
