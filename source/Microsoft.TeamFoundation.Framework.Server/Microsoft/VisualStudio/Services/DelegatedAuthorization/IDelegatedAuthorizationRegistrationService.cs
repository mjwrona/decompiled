// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.IDelegatedAuthorizationRegistrationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  [DefaultServiceImplementation(typeof (FrameworkDelegatedAuthorizationRegistrationService))]
  public interface IDelegatedAuthorizationRegistrationService : IVssFrameworkService
  {
    Registration Create(IVssRequestContext requestContext, Registration registration);

    Registration Create(
      IVssRequestContext requestContext,
      Registration registration,
      bool includeSecret);

    Registration Update(IVssRequestContext requestContext, Registration registration);

    Registration Update(
      IVssRequestContext requestContext,
      Registration registration,
      bool includeSecret);

    Registration Get(IVssRequestContext requestContext, Guid registrationId);

    Registration Get(IVssRequestContext requestContext, Guid registrationId, bool includeSecret);

    IList<Registration> List(IVssRequestContext requestContext);

    JsonWebToken GetSecret(IVssRequestContext requestContext, Registration registration);

    void Delete(IVssRequestContext requestContext, Guid registrationId);

    Registration RotateSecret(IVssRequestContext requestContext, Guid registrationId);
  }
}
