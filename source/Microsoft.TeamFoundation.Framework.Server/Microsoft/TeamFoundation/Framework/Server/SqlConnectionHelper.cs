// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlConnectionHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class SqlConnectionHelper
  {
    public static void SetConnectionTimeout(SqlConnectionStringBuilder builder)
    {
      ArgumentUtility.CheckForNull<SqlConnectionStringBuilder>(builder, nameof (builder));
      if (builder.IntegratedSecurity || builder.ConnectTimeout != 15)
        return;
      builder.ConnectTimeout = 30;
    }

    public static string SanitizeConnectionString(string connectionString)
    {
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
      if (!connectionStringBuilder.IntegratedSecurity && connectionStringBuilder.ConnectTimeout == 30)
        connectionStringBuilder.Remove("Connect Timeout");
      return connectionStringBuilder.ConnectionString;
    }
  }
}
