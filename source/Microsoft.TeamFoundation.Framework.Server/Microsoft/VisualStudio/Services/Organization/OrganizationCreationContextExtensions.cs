// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.OrganizationCreationContextExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.VisualStudio.Services.Organization
{
  public static class OrganizationCreationContextExtensions
  {
    public static Microsoft.VisualStudio.Services.Organization.Client.Organization ToClient(
      this OrganizationCreationContext x)
    {
      if (x == null)
        return (Microsoft.VisualStudio.Services.Organization.Client.Organization) null;
      return new Microsoft.VisualStudio.Services.Organization.Client.Organization()
      {
        CreatorId = x.CreatorId,
        Name = x.AutoGenerateName ? (string) null : x.Name,
        PreferredGeography = x.PreferredGeography,
        PreferredRegion = x.PreferredRegion,
        PrimaryCollection = x.PrimaryCollection.ToClient(),
        Data = x.Data
      };
    }
  }
}
