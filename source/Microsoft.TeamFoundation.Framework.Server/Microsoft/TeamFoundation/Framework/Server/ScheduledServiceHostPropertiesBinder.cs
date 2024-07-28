// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ScheduledServiceHostPropertiesBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class ScheduledServiceHostPropertiesBinder : 
    TeamFoundationServiceHostBinderBase<TeamFoundationServiceHostProperties>
  {
    private SqlColumnBinder IdColumn = new SqlColumnBinder("HostId");
    private SqlColumnBinder DataColumn = new SqlColumnBinder("Data");
    private SqlColumnBinder ServicingDetailsColumn = new SqlColumnBinder("ServicingDetails");

    protected override TeamFoundationServiceHostProperties Bind()
    {
      TeamFoundationServiceHostProperties serviceHostProperties = new TeamFoundationServiceHostProperties();
      serviceHostProperties.Id = this.IdColumn.GetGuid((IDataReader) this.Reader);
      serviceHostProperties.HostType = TeamFoundationHostType.ProjectCollection;
      TeamFoundationServiceHostProperties hostProperties = serviceHostProperties;
      string serializedObject = this.DataColumn.GetString((IDataReader) this.Reader, true);
      object obj;
      if (!string.IsNullOrEmpty(serializedObject) && TeamFoundationSerializationUtility.Deserialize<ServicingJobData>(serializedObject).ServicingItems.TryGetValue(ServicingItemConstants.CollectionProperties, out obj))
      {
        TeamProjectCollectionProperties collectionProperties = (TeamProjectCollectionProperties) obj;
        hostProperties.Name = collectionProperties.Name;
        hostProperties.Description = collectionProperties.Description;
        hostProperties.DatabaseId = collectionProperties.DatabaseId;
        hostProperties.Registered = false;
      }
      this.ProcessServicingDetails(hostProperties, this.ServicingDetailsColumn.GetString((IDataReader) this.Reader, true));
      return hostProperties;
    }
  }
}
