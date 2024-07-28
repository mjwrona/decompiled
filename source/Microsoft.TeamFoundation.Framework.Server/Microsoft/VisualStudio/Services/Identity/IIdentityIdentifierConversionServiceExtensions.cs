// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IIdentityIdentifierConversionServiceExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  public static class IIdentityIdentifierConversionServiceExtensions
  {
    public static IReadOnlyDictionary<Guid, IdentityDescriptor> GetDescriptorsByMasterIds(
      this IIdentityIdentifierConversionService identifierConversionService,
      IVssRequestContext requestContext,
      IEnumerable<Guid> masterIds)
    {
      Dictionary<Guid, IdentityDescriptor> descriptorsByMasterIds = new Dictionary<Guid, IdentityDescriptor>();
      foreach (Guid masterId in masterIds)
      {
        if (!descriptorsByMasterIds.ContainsKey(masterId))
          descriptorsByMasterIds.Add(masterId, identifierConversionService.GetDescriptorByMasterId(requestContext, masterId));
      }
      return (IReadOnlyDictionary<Guid, IdentityDescriptor>) descriptorsByMasterIds;
    }
  }
}
