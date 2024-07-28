// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping.IHostIdMappingRouter
// Assembly: Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D6C9F672-10C9-4D5D-90D5-E07E56C1D4C0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Web;

namespace Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping
{
  public interface IHostIdMappingRouter
  {
    bool TryExtractMappingData(
      IVssRequestContext requestContext,
      HttpRequest request,
      out HostIdMappingData mappingData);

    HostIdMappingData GetMappingData(
      IVssRequestContext requestContext,
      string installationId,
      string qualifier);

    void AddHostIdMapping(
      IVssRequestContext requestContext,
      string providerId,
      string installationId,
      string qualifier,
      bool overrideExisting = false);

    void RemoveHostIdMapping(
      IVssRequestContext requestContext,
      string providerId,
      string installationId,
      string qualifier);

    bool OverrideOnDeletedOrganization(IVssRequestContext requestContext);
  }
}
