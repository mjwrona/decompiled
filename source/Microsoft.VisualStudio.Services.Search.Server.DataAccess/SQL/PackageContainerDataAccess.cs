// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql.PackageContainerDataAccess
// Assembly: Microsoft.VisualStudio.Services.Search.Server.DataAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3B684226-797D-4C9F-9AC1-E10D39E316D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.DataAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.Sql
{
  public class PackageContainerDataAccess : SqlAzureDataAccess, IPackageContainerDataAccess
  {
    public PackageContainerDataAccess()
    {
    }

    internal PackageContainerDataAccess(ITableAccessPlatform tableAccessPlatform)
      : base(tableAccessPlatform)
    {
    }

    List<PackageContainer> IPackageContainerDataAccess.AddOrUpdatePackageContainers(
      IVssRequestContext requestContext,
      List<PackageContainer> packageContainers,
      bool merge)
    {
      this.ValidateNotNullOrEmptyList<PackageContainer>("PackageContainer", (IList<PackageContainer>) packageContainers);
      using (PackageContainerComponent component = requestContext.CreateComponent<PackageContainerComponent>())
        return component.AddTableEntityBatch(packageContainers, merge);
    }

    List<PackageContainer> IPackageContainerDataAccess.GetPackageContainers(
      IVssRequestContext requestContext,
      int topCount)
    {
      TableEntityFilterList filterList = new TableEntityFilterList();
      using (PackageContainerComponent component = requestContext.CreateComponent<PackageContainerComponent>())
        return component.RetriveTableEntityList(topCount, filterList);
    }

    PackageContainer IPackageContainerDataAccess.GetPackageContainer(
      IVssRequestContext requestContext,
      Guid packageContainerId)
    {
      using (PackageContainerComponent component = requestContext.CreateComponent<PackageContainerComponent>())
        return this.InvokeTableOperation<PackageContainer>((Func<PackageContainer>) (() => component.GetPackageContainer(packageContainerId)));
    }

    List<PackageContainer> IPackageContainerDataAccess.GetPackageContainers(
      IVssRequestContext requestContext,
      PackageContainerType packageContainerType,
      int topCount)
    {
      TableEntityFilterList entityFilterList = new TableEntityFilterList();
      entityFilterList.Add(new TableEntityFilter("ContainerType", "eq", ((int) packageContainerType).ToString()));
      TableEntityFilterList filterList = entityFilterList;
      using (PackageContainerComponent component = requestContext.CreateComponent<PackageContainerComponent>())
        return component.RetriveTableEntityList(topCount, filterList);
    }

    int IPackageContainerDataAccess.DeletePackageContainers(
      IVssRequestContext requestContext,
      List<Guid> packageContainerIds)
    {
      this.ValidateNotNullOrEmptyList<Guid>(nameof (packageContainerIds), (IList<Guid>) packageContainerIds);
      int containersDeleted = 0;
      using (PackageContainerComponent component = requestContext.CreateComponent<PackageContainerComponent>())
        this.InvokeTableOperation<List<PackageContainer>>((Func<List<PackageContainer>>) (() =>
        {
          containersDeleted = component.DeletePackageContainers(packageContainerIds);
          return (List<PackageContainer>) null;
        }));
      return containersDeleted;
    }
  }
}
