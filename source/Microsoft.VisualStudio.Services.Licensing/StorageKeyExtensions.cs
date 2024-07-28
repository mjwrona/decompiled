// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.StorageKeyExtensions
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class StorageKeyExtensions
  {
    private static readonly string _area = "Licensing";
    private static readonly string _layer = nameof (StorageKeyExtensions);

    public static SubjectDescriptor ToSubjectDescriptor(
      this Guid storageKey,
      IVssRequestContext requestContext)
    {
      SubjectDescriptor descriptorByStorageKey = requestContext.GetService<IGraphIdentifierConversionService>().GetDescriptorByStorageKey(requestContext, storageKey);
      if (!(descriptorByStorageKey == new SubjectDescriptor()))
        return descriptorByStorageKey;
      requestContext.Trace(2716252, TraceLevel.Error, StorageKeyExtensions._area, StorageKeyExtensions._layer, string.Format("User found with invalid subjectDescriptor. storageKey:{0}", (object) storageKey));
      return descriptorByStorageKey;
    }

    public static IReadOnlyDictionary<Guid, SubjectDescriptor> ToSubjectDescriptors(
      this IEnumerable<Guid> storageKeys,
      IVssRequestContext requestContext)
    {
      IReadOnlyDictionary<Guid, SubjectDescriptor> descriptorsByStorageKeys = requestContext.GetService<IGraphIdentifierConversionService>().GetDescriptorsByStorageKeys(requestContext, storageKeys);
      List<KeyValuePair<Guid, SubjectDescriptor>> list = descriptorsByStorageKeys.Where<KeyValuePair<Guid, SubjectDescriptor>>((Func<KeyValuePair<Guid, SubjectDescriptor>, bool>) (entry => entry.Value == new SubjectDescriptor() || entry.Value.IsUnknownSubjectType())).ToList<KeyValuePair<Guid, SubjectDescriptor>>();
      list.ForEach((Action<KeyValuePair<Guid, SubjectDescriptor>>) (invalidEntry => requestContext.Trace(2716253, TraceLevel.Error, StorageKeyExtensions._area, StorageKeyExtensions._layer, string.Format("User found with invalid identifier(s). storageKey:{0}, subjectDescriptor:{1}", (object) invalidEntry.Key, (object) invalidEntry.Value))));
      return (IReadOnlyDictionary<Guid, SubjectDescriptor>) descriptorsByStorageKeys.Except<KeyValuePair<Guid, SubjectDescriptor>>((IEnumerable<KeyValuePair<Guid, SubjectDescriptor>>) list).ToDictionary<KeyValuePair<Guid, SubjectDescriptor>, Guid, SubjectDescriptor>((Func<KeyValuePair<Guid, SubjectDescriptor>, Guid>) (entry => entry.Key), (Func<KeyValuePair<Guid, SubjectDescriptor>, SubjectDescriptor>) (entry => entry.Value));
    }
  }
}
