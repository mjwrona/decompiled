// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityRepairChange
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using System;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal abstract class IdentityRepairChange
  {
    public Guid RequiredById { get; }

    public Guid ChangedId { get; }

    public DateTime ChangedDate { get; }

    public IdentityDescriptor PreviousDescriptor { get; }

    public IdentityDescriptor UpdatedDescriptor { get; }

    protected IdentityRepairChange(
      Guid requiredById,
      Guid changedId,
      DateTime changedDate,
      IdentityDescriptor previousDescriptor,
      IdentityDescriptor updatedDescriptor)
    {
      this.RequiredById = requiredById;
      this.ChangedId = changedId;
      this.ChangedDate = changedDate;
      if (IdentityDescriptorComparer.Instance.Equals(previousDescriptor, updatedDescriptor))
        return;
      this.PreviousDescriptor = previousDescriptor;
      this.UpdatedDescriptor = updatedDescriptor;
    }

    internal byte? PreviousTypeId => this.GetTypeIdIfChanged(this.PreviousDescriptor, this.UpdatedDescriptor);

    internal byte? UpdatedTypeId => this.GetTypeIdIfChanged(this.UpdatedDescriptor, this.PreviousDescriptor);

    internal string PreviousIdentifier => this.GetIdentifierIfChanged(this.PreviousDescriptor, this.UpdatedDescriptor);

    internal string UpdatedIdentifier => this.GetIdentifierIfChanged(this.UpdatedDescriptor, this.PreviousDescriptor);

    private byte? GetTypeIdIfChanged(
      IdentityDescriptor sourceDescriptor,
      IdentityDescriptor comparisonDescriptor)
    {
      if (sourceDescriptor == (IdentityDescriptor) null)
        return new byte?();
      byte typeIdFromName1 = IdentityTypeMapper.Instance.GetTypeIdFromName(sourceDescriptor.IdentityType);
      if (comparisonDescriptor == (IdentityDescriptor) null)
        return new byte?(typeIdFromName1);
      byte typeIdFromName2 = IdentityTypeMapper.Instance.GetTypeIdFromName(comparisonDescriptor.IdentityType);
      return (int) typeIdFromName1 != (int) typeIdFromName2 ? new byte?(typeIdFromName1) : new byte?();
    }

    private string GetIdentifierIfChanged(
      IdentityDescriptor sourceDescriptor,
      IdentityDescriptor comparisonDescriptor)
    {
      if (sourceDescriptor == (IdentityDescriptor) null)
        return (string) null;
      string identifier1 = sourceDescriptor.Identifier;
      if (comparisonDescriptor == (IdentityDescriptor) null)
        return identifier1;
      string identifier2 = comparisonDescriptor.Identifier;
      return identifier1 != identifier2 ? identifier1 : (string) null;
    }
  }
}
