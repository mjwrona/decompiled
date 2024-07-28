// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.DatabaseRegistrationInfo
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class DatabaseRegistrationInfo
  {
    public string ConnectionString { get; private set; }

    public string ServiceLevel { get; private set; }

    public TeamFoundationDatabaseFlags Flags { get; private set; }

    public int Tenants { get; private set; }

    public DatabaseLoginInfo[] Logins { get; private set; }

    public DatabaseRegistrationInfo(
      ITeamFoundationDatabaseProperties databaseProperties,
      string userId,
      string password,
      string credentialName)
      : this(databaseProperties, new DatabaseLoginInfo[1]
      {
        new DatabaseLoginInfo(userId, password, credentialName)
      })
    {
    }

    public DatabaseRegistrationInfo(
      ITeamFoundationDatabaseProperties databaseProperties,
      DatabaseLoginInfo[] logins)
    {
      this.ConnectionString = databaseProperties.ConnectionInfoWrapper.ConnectionString;
      this.ServiceLevel = databaseProperties.ServiceLevel;
      this.Flags = databaseProperties.Flags;
      this.Tenants = databaseProperties.Tenants;
      this.Logins = logins;
    }
  }
}
