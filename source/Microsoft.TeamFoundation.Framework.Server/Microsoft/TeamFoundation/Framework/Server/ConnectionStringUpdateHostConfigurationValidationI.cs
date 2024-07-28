// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ConnectionStringUpdateHostConfigurationValidationItem
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ConnectionStringUpdateHostConfigurationValidationItem : 
    HostConfigurationValidationItem
  {
    private string m_hostName;
    private Dictionary<string, object> m_dataspaceCategories = new Dictionary<string, object>((IEqualityComparer<string>) VssStringComparer.DatabaseCategory);

    public ConnectionStringUpdateHostConfigurationValidationItem(
      string hostName,
      string connectionString)
      : base(HostConfigurationValidationItemType.ConnectionStringUpdate, string.Empty)
    {
      this.m_hostName = hostName;
      this.ConnectionString = connectionString;
    }

    public ConnectionStringUpdateHostConfigurationValidationItem(string message)
      : base(HostConfigurationValidationItemType.ConnectionStringUpdate, message)
    {
    }

    public string ConnectionString { get; private set; }

    public override string Message
    {
      get => !string.IsNullOrEmpty(base.Message) ? base.Message : FrameworkResources.UpdateConnectionStringWithSchemasAction((object) this.m_hostName, (object) string.Join(", ", this.DataspaceCategories.Keys.ToArray<string>()));
      protected set => base.Message = value;
    }

    public Dictionary<string, object> DataspaceCategories => this.m_dataspaceCategories;
  }
}
