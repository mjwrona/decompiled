// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryEntityIdentifierBuilder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Directories
{
  internal class DirectoryEntityIdentifierBuilder
  {
    private static readonly IReadOnlyDictionary<string, string> DirectoryEntityTypesToIdentifierTypes = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>()
    {
      {
        "User",
        "user"
      },
      {
        "Group",
        "group"
      },
      {
        "ServicePrincipal",
        "servicePrincipal"
      }
    };
    private static readonly IReadOnlyDictionary<string, string> DirectoryNamesToIdentifierSources = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>()
    {
      {
        "ad",
        "ad"
      },
      {
        "aad",
        "aad"
      },
      {
        "wmd",
        "wmd"
      },
      {
        "vsd",
        "ims"
      }
    };

    public static DirectoryEntityIdentifier CreateEntityId(
      string entityType,
      string originDirectory,
      string originId)
    {
      if (entityType == null || originDirectory == null || originId == null)
        return (DirectoryEntityIdentifier) null;
      string type = (string) null;
      if (!DirectoryEntityIdentifierBuilder.DirectoryEntityTypesToIdentifierTypes.TryGetValue(entityType, out type))
        return (DirectoryEntityIdentifier) null;
      string source = (string) null;
      return !DirectoryEntityIdentifierBuilder.DirectoryNamesToIdentifierSources.TryGetValue(originDirectory, out source) ? (DirectoryEntityIdentifier) null : (DirectoryEntityIdentifier) new DirectoryEntityIdentifierV1(source, type, originId);
    }
  }
}
