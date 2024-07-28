// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DelegatedAuthorizationRegistrationServiceAdapter
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  internal class DelegatedAuthorizationRegistrationServiceAdapter : 
    IDelegatedAuthorizationRegistrationServiceAdapter
  {
    private readonly Lazy<IDelegatedAuthorizationRegistrationService> adaptee;

    public DelegatedAuthorizationRegistrationServiceAdapter(
      Lazy<IDelegatedAuthorizationRegistrationService> adaptee)
    {
      this.adaptee = adaptee;
    }

    public Registration Create(
      IVssRequestContext requestContext,
      Registration registration,
      bool includeSecret)
    {
      ArgumentUtility.CheckForNull<Registration>(registration, nameof (registration));
      CommonRegistration commonRegistration = registration.ToCommonRegistration();
      return this.adaptee.Value.Create(requestContext, commonRegistration.ToRegistration(), includeSecret);
    }

    public Registration Create(IVssRequestContext requestContext, Registration registration)
    {
      ArgumentUtility.CheckForNull<Registration>(registration, nameof (registration));
      CommonRegistration commonRegistration = registration.ToCommonRegistration();
      return this.adaptee.Value.Create(requestContext, commonRegistration.ToRegistration());
    }

    public Registration Update(
      IVssRequestContext requestContext,
      Registration registration,
      bool includeSecret)
    {
      ArgumentUtility.CheckForNull<Registration>(registration, nameof (registration));
      CommonRegistration commonRegistration = registration.ToCommonRegistration();
      return this.adaptee.Value.Update(requestContext, commonRegistration.ToRegistration(), includeSecret);
    }

    public Registration Update(IVssRequestContext requestContext, Registration registration)
    {
      ArgumentUtility.CheckForNull<Registration>(registration, nameof (registration));
      CommonRegistration commonRegistration = registration.ToCommonRegistration();
      return this.adaptee.Value.Update(requestContext, commonRegistration.ToRegistration());
    }
  }
}
