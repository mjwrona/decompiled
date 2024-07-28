// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IDeploymentUserIdentityStore
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Identity
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DefaultServiceImplementation(typeof (FrameworkDeploymentUserIdentityStore))]
  public interface IDeploymentUserIdentityStore : IVssFrameworkService
  {
    IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<Guid> identityIds);

    IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<IdentityDescriptor> identityDescriptors);

    IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentities(
      IVssRequestContext requestContext,
      IList<SubjectDescriptor> subjectDescriptors);

    void CreateIdentities(IVssRequestContext requestContext, IList<Microsoft.VisualStudio.Services.Identity.Identity> identities);

    bool UpdateIdentities(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      bool allowMetadataUpdates);
  }
}
