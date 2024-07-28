// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseMetric
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabaseMetric
  {
    public DatabaseMetric(
      string name,
      string serverName,
      string databasePoolName,
      string databaseName,
      ApplicationIntent intent,
      double value)
      : this(DateTime.UtcNow, name, serverName, databasePoolName, databaseName, intent, value)
    {
    }

    public DatabaseMetric(
      DateTime timeStamp,
      string name,
      string serverName,
      string databasePoolName,
      string databaseName,
      ApplicationIntent intent,
      double value)
    {
      this.TimeStamp = timeStamp;
      this.Name = name;
      this.ServerName = serverName;
      this.DatabasePoolName = databasePoolName;
      this.DatabaseName = databaseName;
      this.Intent = intent == ApplicationIntent.ReadOnly ? "ReadOnly" : "ReadWrite";
      this.Value = value;
    }

    public DateTime TimeStamp { get; set; }

    public string Name { get; set; }

    public string ServerName { get; set; }

    public string DatabasePoolName { get; set; }

    public string DatabaseName { get; set; }

    public string Intent { get; set; }

    public double Value { get; set; }
  }
}
