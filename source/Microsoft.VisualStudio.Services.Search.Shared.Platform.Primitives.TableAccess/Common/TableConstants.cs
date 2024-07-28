// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common.TableConstants
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common
{
  public class TableConstants
  {
    public static class RepositoryEntityProperties
    {
      public const string CollectionId = "CollectionId";
      public const string IndexStatus = "IndexStatus";
      public const string ProjectName = "ProjectName";
      public const string RepositoryId = "RepositoryId";
      public const string RepositoryName = "RepositoryName";
      public const string BranchName = "BranchName";
      public const string ProjectId = "ProjectId";
    }

    public static class AzureTableEntityProperties
    {
      public const string PartitionKey = "PartitionKey";
      public const string RowKey = "RowKey";
    }

    public static class RepositoryAzureTableEntityProperties
    {
      public const string DefaultBranch = "DefaultBranch";
      public const string IndexStatus = "IndexStatus";
      public const string ProjectId = "ProjectId";
    }

    public static class IndexingUnitChangeEventTableProperties
    {
      public const string Id = "Id";
      public const string IndexingUnitId = "IndexingUnitId";
      public const string State = "State";
      public const string ChangeType = "ChangeType";
    }

    public static class IndexingUnitTableProperties
    {
      public const string TFSEntityId = "TFSEntityId";
      public const string IndexingUnitType = "IndexingUnitType";
      public const string EntityType = "EntityType";
      public const string IndexingUnitId = "IndexingUnitId";
      public const string TFSEntityAttributes = "TFSEntityAttributes";
      public const string ParentUnitID = "ParentUnitID";
    }

    public static class ClassificationNodeTableProperties
    {
      public const string Id = "Id";
      public const string NodeType = "NodeType";
      public const string ParentId = "ParentId";
    }

    public static class ReindexingStatusTableProperties
    {
      public const string Status = "Status";
      public const string CollectionId = "CollectionId";
      public const string EntityType = "EntityType";
    }

    public static class TreeStoreTableProperties
    {
      public const string TFSEntityId = "TFSEntityId";
      public const string EntityType = "EntityType";
    }

    public static class CustomRepositoryTableProperties
    {
      public const string CollectionId = "CollectionId";
      public const string ProjectName = "ProjectName";
      public const string RepositoryName = "RepositoryName";
      public const string RepositoryId = "RepositoryId";
      public const string Properties = "Properties";
    }

    public static class HealthStatusTableProperties
    {
      public const string CollectionId = "CollectionId";
      public const string JobName = "JobName";
      public const string Mode = "Mode";
      public const string Data = "Data";
    }

    public static class PackageContainerTableProperties
    {
      public const string ContainerType = "ContainerType";
      public const string ContainerName = "ContainerName";
    }
  }
}
