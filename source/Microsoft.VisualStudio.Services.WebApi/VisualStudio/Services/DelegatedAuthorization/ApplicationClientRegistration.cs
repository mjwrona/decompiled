// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.ApplicationClientRegistration
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  [DataContract]
  public class ApplicationClientRegistration : CommonRegistration
  {
    public ApplicationClientRegistration(Registration registration)
      : base(registration)
    {
      this.RegistrationLocation = registration.RegistrationLocation;
      this.SetupUri = registration.SetupUri;
      this.IsWellKnown = registration.IsWellKnown;
      this.AccessHash = registration.AccessHash;
      this.TenantIds = registration.TenantIds;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Uri RegistrationLocation { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Uri SetupUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsWellKnown { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal string AccessHash { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<Guid> TenantIds { get; set; }

    public override Registration ToRegistration()
    {
      Registration registration = base.ToRegistration();
      registration.RegistrationLocation = this.RegistrationLocation;
      registration.IsWellKnown = this.IsWellKnown;
      registration.SetupUri = this.SetupUri;
      registration.AccessHash = this.AccessHash;
      registration.TenantIds = this.TenantIds;
      return registration;
    }
  }
}
