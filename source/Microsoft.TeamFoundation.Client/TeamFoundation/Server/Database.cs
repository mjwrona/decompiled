// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Database
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server
{
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Registration/03")]
  public class Database
  {
    private string m_Name;
    private string m_DatabaseName;
    private string m_SQLServerName;
    private string m_ConnectionString;
    private bool m_ExcludeFromBackup;

    public Database()
    {
    }

    public Database(
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

    internal Predicate<Database> EqualsByName() => (Predicate<Database>) (that => VssStringComparer.DatabaseName.Equals(this.Name, that.Name));
  }
}
