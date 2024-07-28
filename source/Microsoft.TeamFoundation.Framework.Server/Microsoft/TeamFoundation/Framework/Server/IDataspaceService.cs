// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IDataspaceService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (DataspaceService))]
  public interface IDataspaceService : IVssFrameworkService
  {
    void CreateDataspace(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      DataspaceState initialState);

    void CreateDataspace(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      int databaseId);

    void CreateDataspaces(
      IVssRequestContext requestContext,
      string[] dataspaceCategories,
      Guid dataspaceIdentifier);

    void CreateDataspaces(
      IVssRequestContext requestContext,
      string[] dataspaceCategories,
      Guid dataspaceIdentifier,
      DataspaceState initialState);

    Dataspace QueryDataspace(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier,
      bool throwOnMissing);

    Dataspace QueryDataspace(IVssRequestContext requestContext, int dataspaceId);

    void UpdateDataspaces(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid? dataspaceIdentifier,
      int? newDatabaseId,
      DataspaceState? newDataspaceState);

    void DeleteDataspace(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier);

    ISqlConnectionInfo GetConnectionInfo(IVssRequestContext requestContext, int dataspaceId);

    ISqlConnectionInfo GetConnectionInfo(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier);

    ITeamFoundationDatabaseProperties GetDatabaseProperties(
      IVssRequestContext requestContext,
      int dataspaceId);

    ITeamFoundationDatabaseProperties GetDatabaseProperties(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier);
  }
}
