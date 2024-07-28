// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.ClientCreationContextExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.VisualStudio.Services.Organization
{
  public static class ClientCreationContextExtensions
  {
    public static Microsoft.VisualStudio.Services.Organization.Client.Collection ToClient(
      this CollectionCreationContext x)
    {
      if (x == null)
        return (Microsoft.VisualStudio.Services.Organization.Client.Collection) null;
      return new Microsoft.VisualStudio.Services.Organization.Client.Collection()
      {
        Name = x.Name,
        Owner = x.OwnerId,
        PreferredGeography = x.PreferredGeography,
        PreferredRegion = x.PreferredRegion,
        Data = x.Data
      };
    }
  }
}
