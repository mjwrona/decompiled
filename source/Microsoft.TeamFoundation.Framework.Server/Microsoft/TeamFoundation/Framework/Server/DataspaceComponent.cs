// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataspaceComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DataspaceComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<DataspaceComponent>(1, true),
      (IComponentCreator) new ComponentCreator<DataspaceComponent2>(2),
      (IComponentCreator) new ComponentCreator<DataspaceComponent3>(3),
      (IComponentCreator) new ComponentCreator<DataspaceComponent4>(4)
    }, "Dataspace");

    public DataspaceComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual List<Dataspace> CreateDataspaces(
      Guid dataspaceidentifier,
      IEnumerable<KeyValuePair<string, int>> categoryDatabaseMapping,
      DataspaceState initialState)
    {
      return new List<Dataspace>();
    }

    public virtual void DeleteDataspace(Guid dataspaceidentifier, string category) => throw new ServiceVersionNotSupportedException("Dataspace", 1, 1);

    public virtual List<Dataspace> QueryDataspaces() => new List<Dataspace>();

    public virtual Dataspace QueryDataspace(string dataspaceCategory, Guid dataspaceIdentifier) => new Dataspace()
    {
      DataspaceCategory = dataspaceCategory,
      DataspaceIdentifier = dataspaceIdentifier,
      DataspaceId = 0,
      DatabaseId = this.RequestContext.ServiceHost.ServiceHostInternal().DatabaseId,
      State = DataspaceState.Active
    };

    public virtual Dataspace QueryDataspace(int dataspaceId) => throw new ServiceVersionNotSupportedException("Dataspace", 1, 1);

    public virtual void SoftDeleteDataspace(Guid dataspaceidentifier, string category) => throw new ServiceVersionNotSupportedException("Dataspace", 1, 1);

    public virtual List<Dataspace> UpdateDataspaces(
      string dataspaceCategory,
      Guid? dataspaceIdentifier,
      int? newDatabaseId,
      DataspaceState? newDataspaceState)
    {
      throw new ServiceVersionNotSupportedException("Dataspace", 1, 3);
    }

    public virtual Dataspace CreateSplitDataspace(
      int dataspaceId,
      Guid dataspaceIdentifier,
      string dataspaceCategory,
      int databaseId,
      DataspaceState dataspaceState)
    {
      throw new ServiceVersionNotSupportedException("Dataspace", 1, 4);
    }
  }
}
