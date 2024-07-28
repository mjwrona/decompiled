// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.ConnectionStringUtility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.ComponentModel;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ConnectionStringUtility
  {
    private const string c_initialCatalogKeyword = "Initial Catalog";
    private const string c_dataSourceToken = "Data Source=";

    public static string MaskPassword(string connectionString)
    {
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
      if (string.IsNullOrEmpty(connectionStringBuilder.Password))
        return connectionString;
      connectionStringBuilder.Password = "******";
      return connectionStringBuilder.ToString();
    }

    public static string ReplaceInitialCatalog(string connectionString, string databaseName)
    {
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
      if (string.IsNullOrEmpty(databaseName))
        connectionStringBuilder.Remove("Initial Catalog");
      else
        connectionStringBuilder.InitialCatalog = databaseName;
      return connectionStringBuilder.ToString();
    }

    public static string DecryptAndNormalizeConnectionString(string inputString)
    {
      try
      {
        string connectionString = inputString;
        if (inputString.IndexOf("Data Source=", StringComparison.OrdinalIgnoreCase) == -1)
          connectionString = EncryptionUtility.TryDecryptSecretInsecure(inputString);
        return new SqlConnectionStringBuilder(connectionString).ConnectionString;
      }
      catch (Exception ex)
      {
        return (string) null;
      }
    }
  }
}
