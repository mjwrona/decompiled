// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SchemaCompareUtil
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class SchemaCompareUtil
  {
    private static readonly string[] s_commonExclusions = new string[9]
    {
      "fn_diagramobjects",
      "sp_alterdiagram",
      "sp_creatediagram",
      "sp_dropdiagram",
      "sp_helpdiagramdefinition",
      "sp_helpdiagrams",
      "sp_renamediagram",
      "sp_upgraddiagrams",
      "tbl_AccountHostMappingTempBackup"
    };

    public static ISqlConnectionInfo CreateConnectionInfo(string sqlInstance, string databaseName) => SqlConnectionInfoFactory.Create(new SqlConnectionStringBuilder()
    {
      InitialCatalog = databaseName,
      DataSource = sqlInstance,
      IntegratedSecurity = true
    }.ConnectionString);

    public static bool CompareServiceVersionTables(
      string sqlInstance,
      IEnumerable<string> databaseNames,
      string preferredDatabase,
      ITFLogger logger)
    {
      bool flag = true;
      logger.Info("Comparing service version tables ...");
      if (databaseNames.Count<string>() < 1)
        return flag;
      if (preferredDatabase == null)
        preferredDatabase = databaseNames.First<string>();
      ISqlConnectionInfo connectionInfo1 = SchemaCompareUtil.CreateConnectionInfo(sqlInstance, preferredDatabase);
      foreach (string databaseName in databaseNames.Where<string>((Func<string, bool>) (cs => !preferredDatabase.Equals(cs, StringComparison.Ordinal))))
      {
        ISqlConnectionInfo connectionInfo2 = SchemaCompareUtil.CreateConnectionInfo(sqlInstance, databaseName);
        if (!SchemaCompareUtil.CompareServiceVersionTables(connectionInfo1, connectionInfo2, logger))
          flag = false;
      }
      return flag;
    }

    private static bool CompareServiceVersionTables(
      ISqlConnectionInfo connectionInfo1,
      ISqlConnectionInfo connectionInfo2,
      ITFLogger logger)
    {
      string initialCatalog1 = connectionInfo1.InitialCatalog;
      string initialCatalog2 = connectionInfo2.InitialCatalog;
      bool flag = true;
      logger.Info("Comparing service version tables in the following: {0} and {1},", (object) initialCatalog1, (object) initialCatalog2);
      Dictionary<string, ServiceVersionEntry> dictionary1;
      using (ResourceManagementComponent2 componentRaw = connectionInfo1.CreateComponentRaw<ResourceManagementComponent2>())
        dictionary1 = componentRaw.QueryServiceVersion().GetCurrent<ServiceVersionEntry>().ToDictionary<ServiceVersionEntry, string>((Func<ServiceVersionEntry, string>) (e => e.ServiceName), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, ServiceVersionEntry> dictionary2;
      using (ResourceManagementComponent2 componentRaw = connectionInfo2.CreateComponentRaw<ResourceManagementComponent2>())
        dictionary2 = componentRaw.QueryServiceVersion().GetCurrent<ServiceVersionEntry>().ToDictionary<ServiceVersionEntry, string>((Func<ServiceVersionEntry, string>) (e => e.ServiceName), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (ServiceVersionEntry serviceVersionEntry1 in dictionary1.Values)
      {
        ServiceVersionEntry serviceVersionEntry2;
        if (!dictionary2.TryGetValue(serviceVersionEntry1.ServiceName, out serviceVersionEntry2))
        {
          logger.Error("{0} service only registered in {1}.", (object) serviceVersionEntry1.ServiceName, (object) initialCatalog1);
          flag = false;
        }
        else if (serviceVersionEntry1.Version != serviceVersionEntry2.Version || serviceVersionEntry1.MinVersion != serviceVersionEntry2.MinVersion)
        {
          logger.Error("Mismatch found. Service: {0}. In {1} database: Version: {2}, MinVersion: {3}. In {4} database: Version: {5}, MinVersion: {6}.", (object) serviceVersionEntry1.ServiceName, (object) initialCatalog1, (object) serviceVersionEntry1.Version, (object) serviceVersionEntry1.MinVersion, (object) initialCatalog2, (object) serviceVersionEntry2.Version, (object) serviceVersionEntry2.MinVersion);
          flag = false;
        }
      }
      foreach (ServiceVersionEntry serviceVersionEntry in dictionary2.Values)
      {
        if (!dictionary1.TryGetValue(serviceVersionEntry.ServiceName, out ServiceVersionEntry _))
        {
          logger.Error("{0} service only registered in {1}.", (object) serviceVersionEntry.ServiceName, (object) initialCatalog2);
          flag = false;
        }
      }
      return flag;
    }

    public static bool CompareDatabaseSchemas(
      string sqlInstance,
      IEnumerable<string> databaseNames,
      string preferredDatabase,
      ITFLogger logger,
      List<string> testAttachments,
      HashSet<string> excludeObjects = null)
    {
      bool flag = true;
      logger.Info("Comparing database schemas ...");
      if (databaseNames.Count<string>() < 1)
        return flag;
      if (preferredDatabase == null)
        preferredDatabase = databaseNames.First<string>();
      foreach (string db2 in databaseNames.Where<string>((Func<string, bool>) (cs => !preferredDatabase.Equals(cs, StringComparison.Ordinal))))
      {
        if (!SchemaCompareUtil.CompareDatabaseSchemas(sqlInstance, preferredDatabase, db2, logger, testAttachments, excludeObjects))
          flag = false;
      }
      return flag;
    }

    public static bool CompareDatabaseSchemas(
      string sqlInstance,
      string db1,
      string db2,
      ITFLogger logger,
      List<string> testAttachments,
      HashSet<string> excludeObjects = null,
      bool ignoreTablesOnlyInDb1 = false)
    {
      if (excludeObjects == null)
        excludeObjects = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      excludeObjects.UnionWith((IEnumerable<string>) SchemaCompareUtil.s_commonExclusions);
      logger.Info("Excluding following objects while comparing schema of " + db1 + " with " + db2);
      logger.Info(string.Join(Environment.NewLine, (IEnumerable<string>) excludeObjects));
      logger.Info("Comparing schema of " + db1 + " with " + db2);
      bool flag1 = true;
      int num1 = 5;
      SProcComparer sprocComparer = new SProcComparer(db1, db2);
      FunctionComparer functionComparer = new FunctionComparer(db1, db2);
      while (num1 > 0)
      {
        try
        {
          string end;
          using (StreamReader streamReader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("CompareDbs.sql")))
            end = streamReader.ReadToEnd();
          string connectionString = new SqlConnectionStringBuilder()
          {
            DataSource = sqlInstance,
            IntegratedSecurity = true,
            ConnectTimeout = ((int) TimeSpan.FromSeconds(30.0).TotalSeconds)
          }.ToString();
          using (TeamFoundationDataTierComponent componentRaw = SqlConnectionInfoFactory.Create(connectionString).CreateComponentRaw<TeamFoundationDataTierComponent>(logger: logger))
          {
            if (!componentRaw.CheckIfDatabaseExists(db1))
            {
              logger.Warning("Database {0} does not exist.", (object) db1);
              flag1 = false;
            }
            if (!componentRaw.CheckIfDatabaseExists(db2))
            {
              logger.Warning("Database {0} does not exist.", (object) db2);
              flag1 = false;
            }
          }
          if (!flag1)
            return false;
          HashSet<string> stringSet = new HashSet<string>();
          using (SqlConnection connection = new SqlConnection(connectionString))
          {
            using (SqlCommand sqlCommand = new SqlCommand(end, connection))
            {
              connection.Open();
              sqlCommand.Parameters.Add(new SqlParameter("@db1Name", (object) db1));
              sqlCommand.Parameters.Add(new SqlParameter("@db2Name", (object) db2));
              sqlCommand.CommandTimeout = (int) TimeSpan.FromMinutes(3.0).TotalSeconds;
              using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
              {
                while (sqlDataReader.Read())
                {
                  string str1 = (string) sqlDataReader["schemaName"];
                  string str2 = (string) sqlDataReader["tableName"];
                  int num2 = (int) sqlDataReader["existsInDb"];
                  if (excludeObjects.Contains(str2))
                  {
                    logger.Info("Table {0}.{1} only exists in {2} database. TABLE EXISTS IN EXCLUSION LIST, IGNORING..", (object) str1, (object) str2, num2 == 1 ? (object) db1 : (object) db2);
                  }
                  else
                  {
                    if (num2 == 1)
                    {
                      stringSet.Add(str1 + "." + str2);
                      if (ignoreTablesOnlyInDb1)
                        continue;
                    }
                    flag1 = false;
                    logger.Error("Table {0}.{1} only exists in {2} database.", (object) str1, (object) str2, num2 == 1 ? (object) db1 : (object) db2);
                  }
                }
                sqlDataReader.NextResult();
                while (sqlDataReader.Read())
                {
                  string str3 = (string) sqlDataReader["schemaName"];
                  string str4 = (string) sqlDataReader["tableName"];
                  string str5 = (string) sqlDataReader["columnName"];
                  int num3 = (int) sqlDataReader["existsInDb"];
                  string str6 = (string) sqlDataReader["reason"];
                  if (excludeObjects.Contains(str4))
                  {
                    logger.Info("{0}.{1}.{2} column only exists in {3} database. TABLE EXISTS IN EXCLUSION LIST, IGNORING..", (object) str3, (object) str4, (object) str5, num3 == 1 ? (object) db1 : (object) db2);
                  }
                  else
                  {
                    flag1 = false;
                    if (num3 != 3)
                    {
                      logger.Error("{0}.{1}.{2} column only exists in {3} database.", (object) str3, (object) str4, (object) str5, num3 == 1 ? (object) db1 : (object) db2);
                    }
                    else
                    {
                      object obj1 = sqlDataReader[nameof (db1) + str6];
                      object obj2 = sqlDataReader[nameof (db2) + str6];
                      logger.Error("{0}.{1}.{2} columns do not match. Reason: {3}. Value1: {4}, Value2: {5}", (object) str3, (object) str4, (object) str5, (object) str6, obj1, obj2);
                    }
                  }
                }
                sqlDataReader.NextResult();
                while (sqlDataReader.Read())
                {
                  string str7 = (string) sqlDataReader["schemaName"];
                  string str8 = (string) sqlDataReader["tableName"];
                  string str9 = (string) sqlDataReader["indexName"];
                  int num4 = (int) sqlDataReader["existsInDb"];
                  string str10 = (string) sqlDataReader["reason"];
                  if (excludeObjects.Contains(str8))
                    logger.Info("{0}.{1}.{2} index only exists in {3} database. TABLE EXISTS IN EXCLUSION LIST, IGNORING..", (object) str7, (object) str8, (object) str9, num4 == 1 ? (object) db1 : (object) db2);
                  else if (excludeObjects.Contains(str9))
                    logger.Info("{0}.{1}.{2} index only exists in {3} database. INDEX EXISTS IN EXCLUSION LIST, IGNORING..", (object) str7, (object) str8, (object) str9, num4 == 1 ? (object) db1 : (object) db2);
                  else if (num4 != 1 || !ignoreTablesOnlyInDb1 || !stringSet.Contains(str7 + "." + str8))
                  {
                    flag1 = false;
                    if (num4 != 3)
                    {
                      logger.Error("{0}.{1}.{2} index only exists in {3} database.", (object) str7, (object) str8, (object) str9, num4 == 1 ? (object) db1 : (object) db2);
                    }
                    else
                    {
                      object obj3 = sqlDataReader[nameof (db1) + str10];
                      object obj4 = sqlDataReader[nameof (db2) + str10];
                      logger.Error("{0}.{1}.{2} indexes do not match. Reason: {3}. Value1: {4}, Value2: {5}", (object) str7, (object) str8, (object) str9, (object) str10, obj3, obj4);
                    }
                  }
                }
                sqlDataReader.NextResult();
                while (sqlDataReader.Read())
                {
                  string str11 = (string) sqlDataReader["schemaName"];
                  string str12 = (string) sqlDataReader["tableName"];
                  string str13 = (string) sqlDataReader["indexName"];
                  string str14 = (string) sqlDataReader["columnName"];
                  int num5 = (int) sqlDataReader["existsInDb"];
                  string str15 = (string) sqlDataReader["reason"];
                  if (excludeObjects.Contains(str12))
                    logger.Info("{0}.{1} index contains {2} only in {3} database. TABLE EXISTS IN EXCLUSION LIST, IGNORING..", (object) str12, (object) str13, (object) str14, num5 == 1 ? (object) db1 : (object) db2);
                  else if (num5 != 1 || !ignoreTablesOnlyInDb1 || !stringSet.Contains(str11 + "." + str12))
                  {
                    flag1 = false;
                    if (num5 != 3)
                    {
                      logger.Error("{0}.{1} index contains {2} only in {3} database.", (object) str12, (object) str13, (object) str14, num5 == 1 ? (object) db1 : (object) db2);
                    }
                    else
                    {
                      object obj5 = sqlDataReader[nameof (db1) + str15];
                      object obj6 = sqlDataReader[nameof (db2) + str15];
                      logger.Error("{6} columns in {0}.{1}.{2} index do not match. Reason: {3}. Value1: {4}, Value2: {5}", (object) str11, (object) str12, (object) str13, (object) str15, obj5, obj6, (object) str14);
                    }
                  }
                }
                sqlDataReader.NextResult();
                while (sqlDataReader.Read())
                {
                  string str16 = (string) sqlDataReader["schemaName"];
                  string str17 = (string) sqlDataReader["tvpName"];
                  int num6 = (int) sqlDataReader["existsInDb"];
                  flag1 = false;
                  logger.Error("TVP {0}.{1} only exists in {2} database.", (object) str16, (object) str17, num6 == 1 ? (object) db1 : (object) db2);
                }
                sqlDataReader.NextResult();
                while (sqlDataReader.Read())
                {
                  flag1 = false;
                  string str18 = (string) sqlDataReader["schemaName"];
                  string str19 = (string) sqlDataReader["tvpName"];
                  string str20 = (string) sqlDataReader["columnName"];
                  int num7 = (int) sqlDataReader["existsInDb"];
                  string str21 = (string) sqlDataReader["reason"];
                  if (num7 != 3)
                  {
                    logger.Error("TVP column - {0}.{1}.{2} only exists in {3} database.", (object) str18, (object) str19, (object) str20, num7 == 1 ? (object) db1 : (object) db2);
                  }
                  else
                  {
                    object obj7 = sqlDataReader["tvp1" + str21];
                    object obj8 = sqlDataReader["tvp2" + str21];
                    logger.Error("{0}.{1}.{2} columns do not match. Reason: {3}. Value1: {4}, Value2: {5}", (object) str18, (object) str19, (object) str20, (object) str21, obj7, obj8);
                  }
                }
                sqlDataReader.NextResult();
                while (sqlDataReader.Read())
                {
                  string str22 = (string) sqlDataReader["schemaName"];
                  string str23 = (string) sqlDataReader["procName"];
                  int num8 = (int) sqlDataReader["existsInDb"];
                  if (excludeObjects.Contains(str23))
                  {
                    logger.Info("{0}.{1} sproc only exists in {2} database. SPROC EXISTS IN EXCLUSION LIST, IGNORING..", (object) str22, (object) str23, num8 == 1 ? (object) db1 : (object) db2);
                  }
                  else
                  {
                    flag1 = false;
                    logger.Error("{0}.{1} sproc only exists in {2} database.", (object) str22, (object) str23, num8 == 1 ? (object) db1 : (object) db2);
                  }
                }
                sqlDataReader.NextResult();
                bool flag2 = false;
                while (sqlDataReader.Read())
                {
                  string str24 = (string) sqlDataReader["schemaName"];
                  string str25 = (string) sqlDataReader["procName"];
                  string str26 = (string) sqlDataReader["definition1"];
                  string str27 = (string) sqlDataReader["definition2"];
                  if (!string.IsNullOrEmpty(str26) && !string.IsNullOrEmpty(str27))
                  {
                    if (excludeObjects.Contains(str25))
                      logger.Info("{0}.{1} procedures do not match. SPROC EXISTS IN EXCLUSION LIST, IGNORING..", (object) str24, (object) str25);
                    else if (!sprocComparer.AreEqual(str24 + "." + str25, str26, str27))
                    {
                      flag2 = true;
                      logger.Error("{0}.{1} procedures do not match.", (object) str24, (object) str25);
                    }
                  }
                }
                if (flag2)
                {
                  logger.Error("Stored procedures do not match. Please diff the following directories: {0} {1}", (object) sprocComparer.Db1Dir, (object) sprocComparer.Db2Dir);
                  flag1 = false;
                }
                sqlDataReader.NextResult();
                while (sqlDataReader.Read())
                {
                  string str28 = (string) sqlDataReader["schemaName"];
                  string str29 = (string) sqlDataReader["funcName"];
                  int num9 = (int) sqlDataReader["existsInDb"];
                  if (excludeObjects.Contains(str29))
                  {
                    logger.Info("{0}.{1} function only exists in {2} database. FUNCTION EXISTS IN EXCLUSION LIST, IGNORING..", (object) str28, (object) str29, num9 == 1 ? (object) db1 : (object) db2);
                  }
                  else
                  {
                    flag1 = false;
                    logger.Error("{0}.{1} function only exists in {2} database.", (object) str28, (object) str29, num9 == 1 ? (object) db1 : (object) db2);
                  }
                }
                sqlDataReader.NextResult();
                bool flag3 = false;
                while (sqlDataReader.Read())
                {
                  string str30 = (string) sqlDataReader["schemaName"];
                  string str31 = (string) sqlDataReader["funcName"];
                  string str32 = (string) sqlDataReader["definition1"];
                  string str33 = (string) sqlDataReader["definition2"];
                  if (!string.IsNullOrEmpty(str32) && !string.IsNullOrEmpty(str33))
                  {
                    if (excludeObjects.Contains(str31))
                      logger.Info("{0} functions do not match. FUNCTION EXISTS IN EXCLUSION LIST, IGNORING..", (object) str31);
                    else if (!functionComparer.AreEqual(str30 + "." + str31, str32, str33))
                    {
                      flag3 = true;
                      logger.Error("{0} functions do not match", (object) str31);
                    }
                  }
                }
                if (flag3)
                {
                  logger.Error("Functions do not match. Please diff the following directories: {0} {1}", (object) functionComparer.Db1Dir, (object) functionComparer.Db2Dir);
                  flag1 = false;
                }
                sqlDataReader.NextResult();
                while (sqlDataReader.Read())
                {
                  string str34 = (string) sqlDataReader["schemaName"];
                  string str35 = (string) sqlDataReader["tableName"];
                  byte num10 = (byte) sqlDataReader["lockEscalation1"];
                  byte num11 = (byte) sqlDataReader["lockEscalation2"];
                  flag1 = false;
                  logger.Error(string.Format("Lock escalations of table {0}.{1} do not match. {2}: {3}. {4}: {5}", (object) str34, (object) str35, (object) db1, (object) num10, (object) db2, (object) num11));
                }
                break;
              }
            }
          }
        }
        catch (SqlException ex)
        {
          if (ex.Number == 1205)
          {
            --num1;
          }
          else
          {
            logger.Error("Comparing db schema ran into Sql exception : {0}, {1}.", (object) ex.Message, (object) ex.StackTrace);
            flag1 = false;
            break;
          }
        }
      }
      if (flag1)
        logger.Info("Schemas of {0} and {1} match.", (object) db1, (object) db2);
      else if (testAttachments != null)
      {
        string str = SchemaCompareUtil.ZipDiffFiles(sprocComparer.RootDir, "different_sql_objects.zip");
        if (str != null)
          testAttachments.Add(str);
      }
      return flag1;
    }

    private static string ZipDiffFiles(string rootDir, string zipFileName)
    {
      if (!Directory.Exists(rootDir) || !Directory.EnumerateFiles(rootDir, "*.sql", SearchOption.AllDirectories).Any<string>())
        return (string) null;
      string path = Path.Combine(rootDir, zipFileName);
      int length = rootDir.Length;
      if (!rootDir.EndsWith("\\"))
        ++length;
      using (Stream stream1 = (Stream) File.Create(path))
      {
        using (ZipArchive zipArchive = new ZipArchive(stream1, ZipArchiveMode.Create, true))
        {
          foreach (string enumerateFile in Directory.EnumerateFiles(rootDir, "*.sql", SearchOption.AllDirectories))
          {
            FileInfo fileInfo = new FileInfo(enumerateFile);
            ZipArchiveEntry entry = zipArchive.CreateEntry(enumerateFile.Substring(length), CompressionLevel.Optimal);
            entry.LastWriteTime = (DateTimeOffset) fileInfo.LastWriteTimeUtc;
            using (Stream destination = entry.Open())
            {
              using (Stream stream2 = (Stream) File.Open(enumerateFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                stream2.CopyTo(destination);
            }
          }
        }
      }
      return path;
    }
  }
}
