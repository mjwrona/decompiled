// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ResourcesMoveRequestInternal
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class ResourcesMoveRequestInternal : ResourcesMoveRequest
  {
    private IEnumerable<ArmResourceIdentifier> resourceIdentifiers;

    internal string Upn { get; set; }

    internal string TenantId { get; set; }

    internal string Email => CommerceIdentityHelper.GetEmailFromUpn(this.Upn);

    internal ResourcesMoveRequestInternal(ResourcesMoveRequest resourcesMoveRequest)
    {
      this.TargetResourceGroup = resourcesMoveRequest.TargetResourceGroup;
      this.Resources = resourcesMoveRequest.Resources;
    }

    internal IEnumerable<ArmResourceIdentifier> ResourceIdentifiers => this.resourceIdentifiers ?? (this.resourceIdentifiers = this.Resources.Select<string, ArmResourceIdentifier>((Func<string, ArmResourceIdentifier>) (r => new ArmResourceIdentifier(r))));
  }
}
