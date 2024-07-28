// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.IResourceMetadataCacheService
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
{
  [DefaultServiceImplementation(typeof (ResourceMetadataCacheService))]
  public interface IResourceMetadataCacheService : IVssFrameworkService
  {
    bool TryGetValue(
      IVssRequestContext requestContext,
      ResourceMetadataCacheKey key,
      out IResourceMetadataCacheValue value);

    bool Remove(IVssRequestContext requestContext, ResourceMetadataCacheKey key);

    void Set(
      IVssRequestContext requestContext,
      ResourceMetadataCacheKey key,
      IResourceMetadataCacheValue value);
  }
}
