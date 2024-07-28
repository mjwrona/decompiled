// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.IDelegatedAuthorizationRegistrationServiceAdapter
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public interface IDelegatedAuthorizationRegistrationServiceAdapter
  {
    Registration Create(
      IVssRequestContext requestContext,
      Registration registration,
      bool includeSecret);

    Registration Create(IVssRequestContext requestContext, Registration registration);

    Registration Update(
      IVssRequestContext requestContext,
      Registration registration,
      bool includeSecret);

    Registration Update(IVssRequestContext requestContext, Registration registration);
  }
}
