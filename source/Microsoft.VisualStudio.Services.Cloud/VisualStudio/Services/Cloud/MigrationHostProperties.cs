// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.MigrationHostProperties
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [DataContract]
  public class MigrationHostProperties
  {
    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public Guid ParentId { get; set; }

    [DataMember]
    public Guid[] ChildrenIds { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string PhysicalDirectory { get; set; }

    [DataMember]
    public string PlugInDirectory { get; set; }

    [DataMember]
    public TeamFoundationHostType HostType { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public string ServiceLevel { get; set; }

    [DataMember]
    public int StorageAccountId { get; set; }

    [DataMember]
    public TeamFoundationServiceHostStatus Status { get; set; }

    [DataMember]
    public string StatusReason { get; set; }

    [DataMember]
    public bool IsVirtual { get; set; }

    [DataMember]
    public bool IsInReadOnlyMode { get; set; }

    [DataMember]
    public bool HostCreated { get; set; }

    [IgnoreDataMember]
    internal bool IsLocal { get; set; }

    internal TeamFoundationServiceHostProperties ToServiceHostProperties(
      IVssRequestContext requestContext)
    {
      if (this.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
        throw new ArgumentException("migrationHostProperties");
      return new TeamFoundationServiceHostProperties(this.Id, this.ParentId == Guid.Empty || this.HostType == TeamFoundationHostType.Application ? requestContext.ServiceHost.InstanceId : this.ParentId, this.Name, this.Description, this.PhysicalDirectory, this.PlugInDirectory, this.Status, this.StatusReason, ServiceHostSubStatus.Migrating, this.HostType, DatabaseManagementConstants.InvalidDatabaseId, DateTime.MinValue, this.ServiceLevel, this.StorageAccountId);
    }

    public static MigrationHostProperties FromServiceHostProperties(
      TeamFoundationServiceHostProperties hostProperties)
    {
      MigrationHostProperties migrationHostProperties = new MigrationHostProperties();
      migrationHostProperties.Id = hostProperties.Id;
      migrationHostProperties.ParentId = hostProperties.HostType == TeamFoundationHostType.Application ? Guid.Empty : hostProperties.ParentId;
      migrationHostProperties.Name = hostProperties.Name;
      migrationHostProperties.PhysicalDirectory = hostProperties.PhysicalDirectory;
      migrationHostProperties.PlugInDirectory = hostProperties.PlugInDirectory;
      migrationHostProperties.HostType = hostProperties.HostType;
      migrationHostProperties.Description = hostProperties.Description;
      migrationHostProperties.ServiceLevel = hostProperties.ServiceLevel;
      migrationHostProperties.StorageAccountId = hostProperties.StorageAccountId;
      migrationHostProperties.Status = hostProperties.Status;
      migrationHostProperties.StatusReason = hostProperties.StatusReason;
      migrationHostProperties.IsVirtual = hostProperties.IsVirtualServiceHost();
      migrationHostProperties.HostCreated = true;
      if (hostProperties.Children != null)
        migrationHostProperties.ChildrenIds = hostProperties.Children.Select<TeamFoundationServiceHostProperties, Guid>((Func<TeamFoundationServiceHostProperties, Guid>) (h => h.Id)).ToArray<Guid>();
      return migrationHostProperties;
    }

    internal void ToStringBuilder(StringBuilder sb)
    {
      int totalWidth = 34;
      sb.AppendLine("        " + "Id".PadRight(totalWidth) + this.Id.ToString());
      sb.AppendLine("        " + "ParentId".PadRight(totalWidth) + this.ParentId.ToString());
      sb.AppendLine("        " + "Name".PadRight(totalWidth) + this.Name);
      sb.AppendLine("        " + "HostType".PadRight(totalWidth) + this.HostType.ToString());
      sb.AppendLine("        " + "IsVirtual".PadRight(totalWidth) + this.IsVirtual.ToString());
      sb.AppendLine("        " + "ServiceLevel".PadRight(totalWidth) + this.ServiceLevel);
      sb.AppendLine("        " + "StorageAccountId".PadRight(totalWidth) + this.StorageAccountId.ToString());
      sb.AppendLine("        " + "Status".PadRight(totalWidth) + this.Status.ToString());
      sb.AppendLine("        " + "StatusReason".PadRight(totalWidth) + this.StatusReason);
      sb.AppendLine("        ChildrenIds:");
      if (this.ChildrenIds == null)
        return;
      foreach (Guid childrenId in this.ChildrenIds)
        sb.AppendLine("            " + childrenId.ToString("D"));
    }
  }
}
