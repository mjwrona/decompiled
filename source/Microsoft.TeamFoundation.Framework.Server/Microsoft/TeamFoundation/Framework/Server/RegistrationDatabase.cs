// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RegistrationDatabase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Registration/03")]
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Public)]
  public class RegistrationDatabase
  {
    private string m_Name;
    private string m_DatabaseName;
    private string m_SQLServerName;
    private string m_ConnectionString;
    private bool m_ExcludeFromBackup;

    public RegistrationDatabase()
    {
    }

    public RegistrationDatabase(
      string name,
      string databaseName,
      string serverName,
      string connectionString,
      bool excludeFromBackup)
    {
      this.m_Name = name;
      this.m_DatabaseName = databaseName;
      this.m_SQLServerName = serverName;
      this.m_ConnectionString = connectionString;
      this.m_ExcludeFromBackup = excludeFromBackup;
    }

    public string Name
    {
      get => this.m_Name;
      set => this.m_Name = value;
    }

    public string DatabaseName
    {
      get => this.m_DatabaseName;
      set => this.m_DatabaseName = value;
    }

    public string SQLServerName
    {
      get => this.m_SQLServerName;
      set => this.m_SQLServerName = value;
    }

    public string ConnectionString
    {
      get => this.m_ConnectionString;
      set => this.m_ConnectionString = value;
    }

    public bool ExcludeFromBackup
    {
      get => this.m_ExcludeFromBackup;
      set => this.m_ExcludeFromBackup = value;
    }

    internal string SourceRegistryPath { get; set; }

    internal Predicate<RegistrationDatabase> EqualsByName() => (Predicate<RegistrationDatabase>) (that => VssStringComparer.DatabaseName.Equals(this.Name, that.Name));
  }
}
