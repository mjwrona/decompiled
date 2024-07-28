// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryEntityBuilder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Directories
{
  public static class DirectoryEntityBuilder
  {
    private static readonly IReadOnlyDictionary<System.Type, string> ClassTypesToEntityTypes = (IReadOnlyDictionary<System.Type, string>) new Dictionary<System.Type, string>()
    {
      {
        typeof (IDirectoryUser),
        "User"
      },
      {
        typeof (IDirectoryGroup),
        "Group"
      },
      {
        typeof (IDirectoryServicePrincipal),
        "ServicePrincipal"
      },
      {
        typeof (DirectoryUser),
        "User"
      },
      {
        typeof (DirectoryGroup),
        "Group"
      },
      {
        typeof (DirectoryServicePrincipal),
        "ServicePrincipal"
      }
    };
    private static readonly HashSet<string> ValidOriginDirectories = new HashSet<string>()
    {
      "ad",
      "aad",
      "ghb",
      "vsd",
      "wmd"
    };

    public static IDirectoryEntity BuildEntity(
      string entityType,
      string originDirectory,
      string originId,
      string entityId = null,
      string localDirectory = null,
      string localId = null,
      string principalName = null,
      string displayName = null,
      string entityOrigin = null,
      IDictionary<string, object> properties = null)
    {
      DirectoryEntity directoryEntity;
      switch (entityType)
      {
        case "User":
          directoryEntity = (DirectoryEntity) new DirectoryUser();
          break;
        case "Group":
          directoryEntity = (DirectoryEntity) new DirectoryGroup();
          break;
        case "ServicePrincipal":
          directoryEntity = (DirectoryEntity) new DirectoryServicePrincipal();
          break;
        default:
          throw new DirectoryEntityTypeException("Cannot create an IDirectoryEntity with entity type '" + entityType + "'");
      }
      if (!DirectoryEntityBuilder.ValidOriginDirectories.Contains(originDirectory))
        throw new DirectoryParameterException("Cannot create an IDirectoryEntity with origin directory '" + originDirectory + "'");
      if (string.IsNullOrEmpty(originId))
        throw new DirectoryParameterException("Cannot create an IDirectoryEntity with null or empty origin ID");
      directoryEntity.EntityId = entityId ?? DirectoryEntityIdentifierBuilder.CreateEntityId(entityType, originDirectory, originId)?.Encode();
      directoryEntity.EntityType = entityType;
      directoryEntity.EntityOrigin = entityOrigin;
      directoryEntity.OriginDirectory = originDirectory;
      directoryEntity.OriginId = originId;
      directoryEntity.LocalDirectory = localDirectory;
      directoryEntity.LocalId = localId;
      directoryEntity.Properties = properties ?? (IDictionary<string, object>) new Dictionary<string, object>();
      directoryEntity.Properties.SetIfNotNullAndNotConflicting<string, object>("PrincipalName", (object) principalName, nameof (principalName), nameof (properties));
      directoryEntity.Properties.SetIfNotNullAndNotConflicting<string, object>("DisplayName", (object) displayName, nameof (displayName), nameof (properties));
      return (IDirectoryEntity) directoryEntity;
    }

    public static TEntity BuildEntity<TEntity>(
      string originDirectory,
      string originId,
      string entityId = null,
      string localDirectory = null,
      string localId = null,
      string principalName = null,
      string displayName = null,
      IDictionary<string, object> properties = null,
      string entityOrigin = null)
    {
      string str1 = (string) null;
      if (!DirectoryEntityBuilder.ClassTypesToEntityTypes.TryGetValue(typeof (TEntity), out str1))
        throw new DirectoryEntityTypeException("Cannot create IDirectoryEntity of type '" + typeof (TEntity).FullName + "'");
      string entityType = str1;
      string originDirectory1 = originDirectory;
      string originId1 = originId;
      string entityId1 = entityId;
      string localDirectory1 = localDirectory;
      string str2 = entityOrigin;
      string localId1 = localId;
      string principalName1 = principalName;
      string displayName1 = displayName;
      string entityOrigin1 = str2;
      IDictionary<string, object> properties1 = properties;
      return (TEntity) DirectoryEntityBuilder.BuildEntity(entityType, originDirectory1, originId1, entityId1, localDirectory1, localId1, principalName1, displayName1, entityOrigin1, properties1);
    }
  }
}
