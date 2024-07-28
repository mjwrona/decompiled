// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryService.Components.VsdIdentityResult
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Directories.DirectoryService.Components
{
  public class VsdIdentityResult
  {
    public Microsoft.VisualStudio.Services.Identity.Identity Identity { get; }

    public bool CanOverwrite { get; }

    public bool HasMatchingAadOriginId { get; }

    public bool HasMatchingAadUserPrincipalName { get; }

    public bool HasMatchingDomain { get; }

    public bool HasMatchingEntityType { get; }

    public bool HasMatchingLocalDescriptor { get; }

    public bool HasMatchingLocalId { get; }

    public bool HasMatchingMailAddress { get; }

    public bool HasMatchingOriginDirectory { get; }

    public bool HasMatchingOriginId { get; }

    public bool HasMatchingPrincipalName { get; }

    public bool IsValid { get; }

    public bool HasMatchingDomainAndOriginId { get; }

    public bool HasMatchingUnchangeableProperties { get; }

    public bool HasMatchingChangeableProperties { get; }

    public VsdIdentityResult(
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      bool canOverwrite,
      bool hasMatchingAadOriginId,
      bool hasMatchingAadUserPrincipalName,
      bool hasMatchingDomain,
      bool hasMatchingEntityType,
      bool hasMatchingLocalDescriptor,
      bool hasMatchingLocalId,
      bool hasMatchingMailAddress,
      bool hasMatchingOriginDirectory,
      bool hasMatchingOriginId,
      bool hasMatchingPrincipalName)
    {
      this.Identity = identity;
      this.CanOverwrite = canOverwrite;
      this.HasMatchingAadOriginId = hasMatchingAadOriginId;
      this.HasMatchingAadUserPrincipalName = hasMatchingAadUserPrincipalName;
      this.HasMatchingDomain = hasMatchingDomain;
      this.HasMatchingEntityType = hasMatchingEntityType;
      this.HasMatchingLocalDescriptor = hasMatchingLocalDescriptor;
      this.HasMatchingLocalId = hasMatchingLocalId;
      this.HasMatchingMailAddress = hasMatchingMailAddress;
      this.HasMatchingOriginDirectory = hasMatchingOriginDirectory;
      this.HasMatchingOriginId = hasMatchingOriginId;
      this.HasMatchingPrincipalName = hasMatchingPrincipalName;
      this.HasMatchingDomainAndOriginId = hasMatchingDomain & hasMatchingOriginId;
      this.HasMatchingUnchangeableProperties = hasMatchingAadOriginId & hasMatchingEntityType & hasMatchingLocalDescriptor & hasMatchingLocalId & hasMatchingOriginDirectory;
      this.HasMatchingChangeableProperties = hasMatchingAadUserPrincipalName & hasMatchingMailAddress & hasMatchingPrincipalName;
      this.IsValid = canOverwrite && this.HasMatchingDomainAndOriginId && this.HasMatchingUnchangeableProperties && this.HasMatchingChangeableProperties;
    }

    public bool DomainAndOriginIdAreEmptyOrMatch(string expectedDomain, string expectedOriginId) => this.DomainIsEmptyOrMatches(expectedDomain) && this.OriginIdIsEmptyOrMatches(expectedOriginId);

    private bool DomainIsEmptyOrMatches(string expectedDomain)
    {
      string y = this.Identity.GetProperty<string>("Domain", string.Empty)?.Trim();
      if (string.IsNullOrEmpty(y))
        return true;
      expectedDomain = expectedDomain?.Trim();
      return VssStringComparer.IdentityDomain.Equals(expectedDomain, y);
    }

    private bool OriginIdIsEmptyOrMatches(string expectedOriginId)
    {
      expectedOriginId = expectedOriginId?.Trim();
      string y = (string) null;
      if (this.Identity.IsExternalUser)
      {
        Guid property = this.Identity.GetProperty<Guid>("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.Empty);
        y = property == Guid.Empty ? string.Empty : property.ToString();
      }
      else if (VssStringComparer.IdentityDomain.Equals("Windows Live ID", this.Identity.GetProperty<string>("Domain", string.Empty).Trim()))
        y = this.Identity.GetProperty<string>("PUID", string.Empty);
      return string.IsNullOrEmpty(y) || VssStringComparer.IdentityOriginId.Equals(expectedOriginId, y);
    }
  }
}
