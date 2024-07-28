// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.VisualStudioDirectoryHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class VisualStudioDirectoryHelper
  {
    internal static IEnumerable<IdentityDescriptor> GetIdentityDescriptors(
      IVssRequestContext context,
      IEnumerable<string> entityIds,
      string parameterName)
    {
      if (entityIds.IsNullOrEmpty<string>())
        return (IEnumerable<IdentityDescriptor>) Array.Empty<IdentityDescriptor>();
      DirectoryInternalConvertKeysRequest convertKeysRequest = new DirectoryInternalConvertKeysRequest();
      convertKeysRequest.ConvertFrom = "DirectoryEntityIdentifier";
      convertKeysRequest.ConvertTo = "VisualStudioIdentifier";
      convertKeysRequest.Directories = (IEnumerable<string>) new string[1]
      {
        "vsd"
      };
      convertKeysRequest.Keys = (IEnumerable<string>) entityIds.ToImmutableHashSet<string>((IEqualityComparer<string>) VssStringComparer.DirectoryKeyStringComparer);
      DirectoryInternalConvertKeysRequest request = convertKeysRequest;
      IDictionary<string, DirectoryInternalConvertKeyResult> results = VisualStudioDirectoryConvertKeysHelper.ConvertKeys(context, request).Results;
      string errorKey = (string) null;
      if (results.Any<KeyValuePair<string, DirectoryInternalConvertKeyResult>>((Func<KeyValuePair<string, DirectoryInternalConvertKeyResult>, bool>) (x =>
      {
        errorKey = x.Key;
        return string.IsNullOrWhiteSpace(x.Value?.Key) || x.Value.Exception != null;
      })))
        throw new ArgumentException(parameterName + " EntityId:" + errorKey + " is either invalid or not found");
      List<Guid> list1 = results.Values.Where<DirectoryInternalConvertKeyResult>((Func<DirectoryInternalConvertKeyResult, bool>) (keyResult => !string.IsNullOrWhiteSpace(keyResult.Key))).Select<DirectoryInternalConvertKeyResult, Guid>((Func<DirectoryInternalConvertKeyResult, Guid>) (keyResult => new Guid(keyResult.Key))).ToList<Guid>();
      List<IdentityDescriptor> list2 = context.GetService<IdentityService>().ReadIdentities(context, (IList<Guid>) list1, QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null)).Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (identity => identity.Descriptor)).ToList<IdentityDescriptor>();
      return list2.Any<IdentityDescriptor>() ? (IEnumerable<IdentityDescriptor>) list2 : throw new ArgumentException("None of the entity ids specified for " + parameterName + " is resolvable");
    }
  }
}
