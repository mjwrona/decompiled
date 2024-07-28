// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyConfigurationRecord
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Policy.Server
{
  public sealed class PolicyConfigurationRecord
  {
    private const int c_notCreatedYetId = -1;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public PolicyConfigurationRecord(
      int configurationRevisionId,
      int configurationId,
      Guid typeId,
      Guid projectId,
      bool isEnabled,
      bool isBlocking,
      bool isEnterpriseManaged,
      string settings,
      Guid creatorId,
      DateTime creationDate,
      bool isDeleted = false)
    {
      ArgumentUtility.CheckForNull<string>(settings, nameof (settings));
      this.ConfigurationId = configurationId;
      this.ConfigurationRevisionId = configurationRevisionId;
      this.TypeId = typeId;
      this.ProjectId = projectId;
      this.IsEnabled = isEnabled;
      this.IsBlocking = isBlocking;
      this.Settings = settings;
      this.CreatorId = creatorId;
      this.CreationDate = creationDate;
      this.IsDeleted = isDeleted;
      this.IsEnterpriseManaged = isEnterpriseManaged;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public PolicyConfigurationRecord(
      Guid typeId,
      Guid projectId,
      bool isEnabled,
      bool isBlocking,
      bool isEnterpriseManaged,
      string settings,
      Guid creatorId)
      : this(-1, -1, typeId, projectId, isEnabled, isBlocking, isEnterpriseManaged, settings, creatorId, DateTime.MinValue)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public PolicyConfigurationRecord(
      int configurationId,
      Guid typeId,
      Guid projectId,
      bool isEnabled,
      bool isBlocking,
      bool isEnterpriseManaged,
      string settings,
      Guid creatorId)
      : this(-1, configurationId, typeId, projectId, isEnabled, isBlocking, isEnterpriseManaged, settings, creatorId, DateTime.MinValue)
    {
    }

    public bool ExistsInDatabase => this.ConfigurationId != -1;

    public int ConfigurationId { get; private set; }

    public int ConfigurationRevisionId { get; private set; }

    public Guid TypeId { get; private set; }

    public Guid ProjectId { get; private set; }

    public bool IsEnabled { get; private set; }

    public bool IsBlocking { get; private set; }

    public string Settings { get; private set; }

    public bool IsEnterpriseManaged { get; private set; }

    public Guid CreatorId { get; private set; }

    public DateTime CreationDate { get; private set; }

    public bool IsDeleted { get; private set; }
  }
}
