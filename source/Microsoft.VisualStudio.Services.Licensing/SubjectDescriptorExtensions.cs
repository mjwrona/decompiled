// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.SubjectDescriptorExtensions
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class SubjectDescriptorExtensions
  {
    public static Guid ToStorageKey(
      this SubjectDescriptor subjectDescriptor,
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<IGraphIdentifierConversionService>().GetStorageKeyByDescriptor(requestContext, subjectDescriptor);
    }

    public static Guid ToEnterpriseStorageKey(
      this SubjectDescriptor subjectDescriptor,
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      Guid storageKeyByDescriptor = vssRequestContext.GetService<IGraphIdentifierConversionService>().GetStorageKeyByDescriptor(vssRequestContext, subjectDescriptor);
      return !(storageKeyByDescriptor == new Guid()) && IdentityIdChecker.IsStorageKey(storageKeyByDescriptor) ? storageKeyByDescriptor : throw new StorageKeyNotFoundException(subjectDescriptor);
    }

    public static IReadOnlyDictionary<SubjectDescriptor, Guid> ToStorageKeys(
      this IEnumerable<SubjectDescriptor> subjectDescriptors,
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<IGraphIdentifierConversionService>().GetStorageKeysByDescriptors(requestContext, subjectDescriptors);
    }

    public static IReadOnlyDictionary<SubjectDescriptor, Guid> ToEnterpriseStorageKeys(
      this IEnumerable<SubjectDescriptor> subjectDescriptors,
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      return vssRequestContext.GetService<IGraphIdentifierConversionService>().GetStorageKeysByDescriptors(vssRequestContext, subjectDescriptors);
    }
  }
}
