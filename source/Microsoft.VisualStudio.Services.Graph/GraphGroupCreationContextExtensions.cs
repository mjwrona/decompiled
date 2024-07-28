// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphGroupCreationContextExtensions
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories;
using Microsoft.VisualStudio.Services.Graph.Client;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Graph
{
  public static class GraphGroupCreationContextExtensions
  {
    public static IDirectoryEntityDescriptor ToDirectoryEntityDescriptor(
      this GraphGroupOriginIdCreationContext creationContext)
    {
      string localId = (string) null;
      if (creationContext.StorageKey != Guid.Empty)
        localId = creationContext.StorageKey.ToString();
      return (IDirectoryEntityDescriptor) new DirectoryEntityDescriptor("Group", originId: creationContext.OriginId, localId: localId, properties: GraphToDirectoryService.CommonMemberMaterializationProperties);
    }

    public static IDirectoryEntityDescriptor ToDirectoryEntityDescriptor(
      this GraphGroupMailAddressCreationContext creationContext)
    {
      string localId = (string) null;
      if (creationContext.StorageKey != Guid.Empty)
        localId = creationContext.StorageKey.ToString();
      return (IDirectoryEntityDescriptor) new DirectoryEntityDescriptor("Group", localId: localId, properties: (IReadOnlyDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "Mail",
          (object) creationContext.MailAddress
        }
      }.AddRange<KeyValuePair<string, object>, Dictionary<string, object>>((IEnumerable<KeyValuePair<string, object>>) GraphToDirectoryService.CommonMemberMaterializationProperties));
    }
  }
}
