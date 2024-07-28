// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationServiceHostProperties
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationServiceHostProperties : HostProperties
  {
    private List<Guid> m_childrenHosts;
    private List<TeamFoundationServiceHostProperties> m_childrenHostProperties;
    private List<ServicingJobDetail> m_servicingDetails;

    public TeamFoundationServiceHostProperties()
    {
    }

    public TeamFoundationServiceHostProperties(TeamFoundationServiceHostProperties other)
      : this(other.Id, other.ParentId, other.Name, other.Description, other.PhysicalDirectory, other.PlugInDirectory, other.Status, other.StatusReason, other.SubStatus, other.HostType, other.DatabaseId, other.LastUserAccess, other.ServiceLevel, other.StorageAccountId, other.BackupData)
    {
    }

    public TeamFoundationServiceHostProperties(
      Guid id,
      Guid parentId,
      string name,
      string description,
      string physicalDirectory,
      string plugInDirectory,
      TeamFoundationServiceHostStatus status,
      string statusReason,
      ServiceHostSubStatus subStatus,
      TeamFoundationHostType hostType,
      int databaseId,
      DateTime lastUserAccess,
      string serviceLevel,
      int storageAccountId,
      VirtualHostBackupData backupData = null)
      : base(id, parentId, name, description, physicalDirectory, plugInDirectory, status, statusReason, subStatus, hostType, databaseId, lastUserAccess, serviceLevel, storageAccountId, backupData)
    {
      this.Registered = true;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TeamFoundationServiceHostProperties\r\n[\r\n        Id:                 {0}\r\n        ParentId:           {1}\r\n        Name:               {2}\r\n        Description:        {3}\r\n        PhysicalDirectory:  {4}\r\n        PlugInDirectory:    {5}\r\n        Status:             {6}\r\n        StatusReason:       {7}\r\n        SubStatus:          {8}\r\n        HostType:           {9}\r\n        LastUserAccess:     {10}\r\n        Registered:         {11}\r\n        DatabaseId:         {12}\r\n        ServiceLevel:       {13}\r\n        StorageAccountId:   {14}\r\n]", (object) this.Id, (object) this.ParentId, (object) this.Name, (object) this.Description, (object) this.PhysicalDirectory, (object) this.PlugInDirectory, (object) this.Status, (object) this.StatusReason, (object) this.SubStatus, (object) this.HostType, (object) this.LastUserAccess, (object) this.Registered, (object) this.DatabaseId, (object) this.ServiceLevel, (object) this.StorageAccountId);

    [XmlIgnore]
    public List<TeamFoundationServiceHostProperties> Children
    {
      get => this.m_childrenHostProperties ?? new List<TeamFoundationServiceHostProperties>();
      internal set => this.m_childrenHostProperties = value;
    }

    [XmlIgnore]
    public List<Guid> ChildrenHosts
    {
      get
      {
        if (this.m_childrenHosts == null)
          this.m_childrenHosts = new List<Guid>(1);
        return this.m_childrenHosts;
      }
    }

    [XmlIgnore]
    public List<ServicingJobDetail> ServicingDetails
    {
      get => this.m_servicingDetails;
      internal set => this.m_servicingDetails = value;
    }

    internal TeamFoundationServiceHostProperties Clone()
    {
      TeamFoundationServiceHostProperties serviceHostProperties = new TeamFoundationServiceHostProperties();
      serviceHostProperties.Id = this.Id;
      serviceHostProperties.ParentId = this.ParentId;
      serviceHostProperties.Name = this.Name;
      serviceHostProperties.DatabaseId = this.DatabaseId;
      serviceHostProperties.Description = this.Description;
      serviceHostProperties.PhysicalDirectory = this.PhysicalDirectory;
      serviceHostProperties.PlugInDirectory = this.PlugInDirectory;
      serviceHostProperties.Status = this.Status;
      serviceHostProperties.StatusReason = this.StatusReason;
      serviceHostProperties.SubStatus = this.SubStatus;
      serviceHostProperties.HostType = this.HostType;
      serviceHostProperties.LastUserAccess = this.LastUserAccess;
      serviceHostProperties.Registered = this.Registered;
      serviceHostProperties.Children = this.Children;
      serviceHostProperties.ServicingDetails = this.ServicingDetails;
      serviceHostProperties.ServiceLevel = this.ServiceLevel;
      serviceHostProperties.StorageAccountId = this.StorageAccountId;
      serviceHostProperties.BackupData = this.BackupData;
      return serviceHostProperties;
    }
  }
}
