// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityOidRepairChange
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using System;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityOidRepairChange : IdentityRepairChange
  {
    public Guid PreviousOid { get; }

    public Guid UpdatedOid { get; }

    public IdentityOidRepairChange(
      Guid requiredById,
      Guid changedId,
      DateTime changedDate,
      IdentityDescriptor previousDescriptor,
      IdentityDescriptor updatedDescriptor,
      Guid previousOid,
      Guid updatedOid)
      : base(requiredById, changedId, changedDate, previousDescriptor, updatedDescriptor)
    {
      if (!(previousOid != updatedOid))
        return;
      this.PreviousOid = previousOid;
      this.UpdatedOid = updatedOid;
    }
  }
}
