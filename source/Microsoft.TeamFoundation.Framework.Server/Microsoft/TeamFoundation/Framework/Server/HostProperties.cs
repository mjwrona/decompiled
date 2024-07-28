// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostProperties
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class HostProperties
  {
    private int m_storageAccountId = -1;
    private static readonly Regex s_tfError = new Regex("^(TF|VS)\\d+:", RegexOptions.Compiled);
    private const string c_area = "HostProperties";
    private const string c_layer = "HostManagement";

    public HostProperties()
    {
    }

    public HostProperties(HostProperties other)
      : this(other.Id, other.ParentId, other.Name, other.Description, other.PhysicalDirectory, other.PlugInDirectory, other.Status, other.StatusReason, other.SubStatus, other.HostType, other.DatabaseId, other.LastUserAccess, other.ServiceLevel, other.StorageAccountId, other.BackupData)
    {
    }

    public HostProperties(
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
      VirtualHostBackupData backUpData = null)
    {
      this.Id = id;
      this.ParentId = parentId;
      this.Name = name;
      this.Description = description;
      this.DatabaseId = databaseId;
      this.PhysicalDirectory = physicalDirectory;
      this.PlugInDirectory = plugInDirectory;
      this.Status = status;
      this.StatusReason = statusReason;
      this.SubStatus = subStatus;
      this.HostType = hostType;
      this.LastUserAccess = lastUserAccess;
      this.Registered = true;
      this.ServiceLevel = serviceLevel;
      this.StorageAccountId = storageAccountId;
      this.BackupData = backUpData;
    }

    public Guid Id { get; set; }

    public Guid ParentId { get; set; }

    public string Name { get; set; }

    public int DatabaseId { get; set; }

    public string PhysicalDirectory { get; set; }

    public string PlugInDirectory { get; set; }

    public TeamFoundationServiceHostStatus Status { get; set; }

    public string StatusReason { get; set; }

    public ServiceHostSubStatus SubStatus { get; set; }

    public DateTime LastUserAccess { get; set; }

    public string Description { get; set; }

    public bool Registered { get; set; }

    public string ServiceLevel { get; set; }

    public int StorageAccountId
    {
      get => this.m_storageAccountId;
      set => this.m_storageAccountId = value;
    }

    [XmlIgnore]
    internal DateTime LastAccessTime { get; set; }

    [XmlIgnore]
    public TeamFoundationHostType HostType { get; set; }

    [XmlElement("HostType")]
    public int HostTypeValue
    {
      get => (int) this.HostType;
      set => this.HostType = (TeamFoundationHostType) value;
    }

    [XmlIgnore]
    internal VirtualHostBackupData BackupData { get; set; }

    internal void UpdateProperties(HostProperties hostProperties)
    {
      if (hostProperties.ParentId != Guid.Empty)
        this.ParentId = hostProperties.ParentId;
      if (hostProperties.DatabaseId != DatabaseManagementConstants.InvalidDatabaseId)
        this.DatabaseId = hostProperties.DatabaseId;
      if (!string.IsNullOrEmpty(hostProperties.Name))
        this.Name = hostProperties.Name;
      if (hostProperties.Description != null)
        this.Description = hostProperties.Description;
      if (hostProperties.HostType != TeamFoundationHostType.Unknown)
        this.HostType = hostProperties.HostType;
      if (!string.IsNullOrEmpty(hostProperties.StatusReason))
        this.StatusReason = hostProperties.StatusReason;
      else if (hostProperties.Status == TeamFoundationServiceHostStatus.Starting || hostProperties.Status == TeamFoundationServiceHostStatus.Started)
        this.StatusReason = (string) null;
      if (hostProperties.SubStatus != ServiceHostSubStatus.Unchanged)
        this.SubStatus = hostProperties.SubStatus;
      if (hostProperties.ServiceLevel != null)
        this.ServiceLevel = hostProperties.ServiceLevel;
      if (hostProperties.StorageAccountId == -1)
        return;
      this.StorageAccountId = hostProperties.StorageAccountId;
    }

    internal string ToBriefString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} (Type: {1}, Id: {2} DatabaseId {3})", (object) this.Name, (object) this.HostType, (object) this.Id, (object) this.DatabaseId);

    internal void ThrowShutdownException(HostRequestType type = HostRequestType.Default) => this.ThrowShutdownException(HostProperties.IsExternalRequest(type));

    internal void ThrowShutdownException(IVssRequestContext requestContext) => this.ThrowShutdownException(HostProperties.IsExternalRequest(requestContext));

    private void ThrowShutdownException(bool isExternalRequest)
    {
      string message;
      if (isExternalRequest)
      {
        if (HostProperties.IsPublicError(this.StatusReason))
        {
          message = this.StatusReason;
        }
        else
        {
          message = (this.HostType & TeamFoundationHostType.Application) != TeamFoundationHostType.Application ? FrameworkResources.CollectionIsNotProcessingRequests((object) this.Name) : FrameworkResources.ApplicationIsNotProcessingRequests();
          if (!string.IsNullOrEmpty(this.StatusReason))
            message = FrameworkResources.HostOfflineWithAdministratorReasonFormatString((object) message, (object) FrameworkResources.HostOfflineAdministratorReason((object) this.StatusReason));
        }
      }
      else
        message = string.Format("Status Reason='{0}' HostId='{1}' Name='{2}'", (object) this.StatusReason, (object) this.Id, (object) this.Name);
      Exception exception = (Exception) new HostShutdownException(message);
      try
      {
        if (AbuseSkipCircuitBreakerService.SkipCircuitBreakers)
        {
          if (this.StatusReason != null)
          {
            if (this.StatusReason.Equals("abuse"))
              exception.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceRawAlwaysOn(1180896582, TraceLevel.Warning, nameof (HostProperties), "HostManagement", "Failed to evaluate StatusReason - will not add DontTriggerCircuitBreaker flag. StatusReason = " + (this.StatusReason ?? "null"));
      }
      throw exception;
    }

    public override string ToString() => "[" + this.ToBriefString() + "]";

    private static bool IsExternalRequest(HostRequestType type) => type == HostRequestType.AspNet || type == HostRequestType.Ssh;

    private static bool IsExternalRequest(IVssRequestContext requestContext) => requestContext is AspNetRequestContext || requestContext is SshRequestContext;

    private static bool IsPublicError(string message) => !string.IsNullOrEmpty(message) && HostProperties.s_tfError.IsMatch(message);
  }
}
