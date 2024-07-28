// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.IGraphIdentifierConversionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Graph
{
  [DefaultServiceImplementation(typeof (FrameworkGraphIdentifierConversionService))]
  internal interface IGraphIdentifierConversionService : IVssFrameworkService
  {
    Guid GetStorageKeyByDescriptor(IVssRequestContext requestContext, SubjectDescriptor descriptor);

    SubjectDescriptor GetDescriptorByStorageKey(IVssRequestContext requestContext, Guid storageKey);

    IReadOnlyDictionary<SubjectDescriptor, Guid> GetStorageKeysByDescriptors(
      IVssRequestContext requestContext,
      IEnumerable<SubjectDescriptor> descriptors);

    IReadOnlyDictionary<Guid, SubjectDescriptor> GetDescriptorsByStorageKeys(
      IVssRequestContext requestContext,
      IEnumerable<Guid> storageKeys);

    SubjectDescriptor GetCuidBasedDescriptorByLegacyDescriptor(
      IVssRequestContext requestContext,
      IdentityDescriptor identityDescriptor);

    IdentityDescriptor GetLegacyDescriptorByCuidBasedDescriptor(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor);

    SubjectDescriptor GetDescriptorByProviderInfo(
      IVssRequestContext requestContext,
      string domain,
      string originId);

    SubjectDescriptor GetDescriptorForCspPartnerByProviderInfo(
      IVssRequestContext requestContext,
      string domain,
      string originId);
  }
}
