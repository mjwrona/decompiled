// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ConnectedServiceComponent
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ConnectedServiceComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<ConnectedServiceComponent>(1, true)
    }, "ConnectedService");

    public ConnectedServiceComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    internal void AddConnectedService(
      ConnectedServiceMetadata connectedServicemetadata,
      Guid projectId)
    {
      this.PrepareStoredProcedure("prc_AddConnectedService");
      this.BindString("@name", connectedServicemetadata.Name, 256, false, SqlDbType.NVarChar);
      this.BindGuid("@projectId", projectId);
      this.BindByte("@kind", (byte) connectedServicemetadata.Kind);
      this.BindString("@friendlyName", connectedServicemetadata.FriendlyName, 512, true, SqlDbType.NVarChar);
      this.BindString("@description", connectedServicemetadata.Description, int.MaxValue, true, SqlDbType.NVarChar);
      this.BindString("@serviceUri", connectedServicemetadata.ServiceUri, int.MaxValue, true, SqlDbType.NVarChar);
      this.BindGuid("@authenticatedBy", connectedServicemetadata.AuthenticatedBy);
      this.ExecuteNonQuery();
    }

    internal ConnectedServiceMetadata AddConnectedServiceProjectAssociation(
      string name,
      Guid projectId,
      string teamProject)
    {
      this.PrepareStoredProcedure("prc_AddConnectedServiceProjectAssociation");
      this.BindString("@name", name, 256, false, SqlDbType.NVarChar);
      this.BindGuid("@projectId", projectId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ConnectedServiceMetadata>((ObjectBinder<ConnectedServiceMetadata>) new ServiceMetadataBinder(teamProject));
        List<ConnectedServiceMetadata> items = resultCollection.GetCurrent<ConnectedServiceMetadata>().Items;
        return items != null && items.Count > 0 ? items[0] : (ConnectedServiceMetadata) null;
      }
    }

    internal List<ConnectedServiceMetadata> GetConnectedServices(Guid projectId, string teamProject)
    {
      this.PrepareStoredProcedure("prc_GetConnectedServices");
      this.BindGuid("@projectId", projectId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ConnectedServiceMetadata>((ObjectBinder<ConnectedServiceMetadata>) new ServiceMetadataBinder(teamProject));
        List<ConnectedServiceMetadata> items = resultCollection.GetCurrent<ConnectedServiceMetadata>().Items;
        return items != null && items.Count > 0 ? items : new List<ConnectedServiceMetadata>();
      }
    }

    internal ConnectedServiceMetadata GetConnectedService(
      string name,
      Guid projectId,
      string teamProject)
    {
      this.PrepareStoredProcedure("prc_GetConnectedService");
      this.BindString("@name", name, 256, false, SqlDbType.NVarChar);
      if (projectId != Guid.Empty)
        this.BindGuid("@projectId", projectId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ConnectedServiceMetadata>((ObjectBinder<ConnectedServiceMetadata>) new ServiceMetadataBinder(teamProject));
        List<ConnectedServiceMetadata> items = resultCollection.GetCurrent<ConnectedServiceMetadata>().Items;
        return items != null && items.Count > 0 ? items[0] : (ConnectedServiceMetadata) null;
      }
    }

    internal bool DeleteProjectAssociationsAndConnectedServiceIfNotReferenced(
      string name,
      Guid projectId)
    {
      this.PrepareStoredProcedure("prc_DeleteConnectedService");
      this.BindString("@name", name, 256, false, SqlDbType.NVarChar);
      this.BindGuid("@projectId", projectId);
      return Convert.ToBoolean(this.ExecuteScalar(), (IFormatProvider) CultureInfo.InvariantCulture);
    }
  }
}
