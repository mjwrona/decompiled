// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.DatabaseUpdater
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class DatabaseUpdater
  {
    private const string c_headerPrefix = "-->#(";
    private const string c_serviceHeader = "-->#(Services)";
    private const string c_dropObjectsHeader = "-->#(DropObjects)";
    private static readonly Regex s_sqlObjectInfo = new Regex("-->#\\(name=\"(?<name>[\\[\\]\\w_\\.]+)\"\\s+type=\"(?<type>[\\w_\\s]+)\"(?<attribute>\\s+(?<attributeName>[a-zA-Z]*)=\\\"(?<attributeValue>[\\w_\\s.\\[\\]]+)\\\")*\\)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex s_setServiceVersionRegex = new Regex("\\s?EXEC\\s+prc_SetServiceVersion\\s+@serviceName\\s*=s*'(?<serviceName>\\w+)'\\s*,\\s*@version\\s*=s*(?<version>(-?\\d+))\\s*(,\\s*@minVersion\\s*=s*(?<minVersion>(-?\\d+)))*", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex s_dropRegex = new Regex("\\.*DROP\\s+(?<type>\\S+)\\s+(?<name>[A-Za-z0-9\\._]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly HashSet<string> s_ignoreLines = new HashSet<string>((IEnumerable<string>) new string[2]
    {
      "BEGIN TRAN",
      "COMMIT"
    }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private readonly ITFLogger m_logger;

    public DatabaseUpdater(ITFLogger logger) => this.m_logger = logger ?? (ITFLogger) new NullLogger();

    public int PerformUpdate(
      ISqlConnectionInfo connectionInfo,
      List<string> sqlScripts,
      IServicingResourceProvider resourceProvider)
    {
      List<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition> storedProcedures;
      using (DatabaseObjectDefinitionComponent componentRaw = connectionInfo.CreateComponentRaw<DatabaseObjectDefinitionComponent>())
      {
        this.m_logger.Info("Querying stored procedures.");
        storedProcedures = componentRaw.GetStoredProcedures();
        this.m_logger.Info("Found {0} stored procedures.", (object) storedProcedures.Count);
      }
      Dictionary<string, string> objects = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, string> dropObjects = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, int> serviceVersions = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      for (int index = 0; index < sqlScripts.Count; ++index)
      {
        string sqlScript = sqlScripts[index];
        this.m_logger.Info("Processing {0}", (object) sqlScript);
        this.ReadObjectsFromScript(sqlScript, resourceProvider, objects, dropObjects, serviceVersions);
      }
      this.m_logger.Info("Comparing sql objects ...");
      List<SqlBatch> updateScript = this.GenerateUpdateScript(storedProcedures, objects, dropObjects);
      if (updateScript.Count > 0)
      {
        List<ServiceVersionEntry> items;
        using (ResourceManagementComponent componentRaw = connectionInfo.CreateComponentRaw<ResourceManagementComponent>())
          items = componentRaw.QueryServiceVersion().GetCurrent<ServiceVersionEntry>().Items;
        foreach (ServiceVersionEntry serviceVersionEntry in items)
        {
          int num;
          if (serviceVersions.TryGetValue(serviceVersionEntry.ServiceName, out num) && num != serviceVersionEntry.Version)
            throw new TeamFoundationServicingException(string.Format("Stored procedures/functions cannot be updated in the database '{0}', because service '{1}' has version {2} in the database and version {3} in install script.", (object) connectionInfo.InitialCatalog, (object) serviceVersionEntry.ServiceName, (object) serviceVersionEntry.Version, (object) num));
        }
      }
      if (updateScript.Count > 0)
      {
        this.m_logger.Info("{0} procedures and functions must be updated.", (object) updateScript.Count);
        SqlScript sqlScript = new SqlScript("GeneratedScript", updateScript);
        using (SqlScriptResourceComponent componentRaw = connectionInfo.CreateComponentRaw<SqlScriptResourceComponent>())
          componentRaw.ExecuteScripts(new List<SqlScript>()
          {
            sqlScript
          }, (SqlParameter[]) null, (List<ServiceVersionEntry>) null, true, 3600);
      }
      this.m_logger.Info("{0} SQL objects have been updated.", (object) updateScript.Count);
      return updateScript.Count;
    }

    protected List<SqlBatch> GenerateUpdateScript(
      List<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition> databaseObjects,
      Dictionary<string, string> objects,
      Dictionary<string, string> dropObjects)
    {
      List<SqlBatch> updateScript = new List<SqlBatch>();
      for (int index = 0; index < databaseObjects.Count; ++index)
      {
        string fullName = databaseObjects[index].FullName;
        string str;
        if (!fullName.Equals("[dbo].[func_GetEndRangeChar]", StringComparison.OrdinalIgnoreCase) && objects.TryGetValue(fullName, out str) && !this.SqlObjectsEqual(str, databaseObjects[index].Content, (Predicate<string>) (line => line.StartsWith("CREATE PROCEDURE", StringComparison.OrdinalIgnoreCase) || line.StartsWith("CREATE FUNCTION", StringComparison.OrdinalIgnoreCase))))
        {
          this.m_logger.Info("{0} must be updated.", (object) fullName);
          SqlBatch alterObjectBatch = SqlScript.CreateAlterObjectBatch(str);
          updateScript.Add(alterObjectBatch);
        }
      }
      foreach (KeyValuePair<string, string> dropObject in dropObjects)
      {
        string str = (string) null;
        string key = dropObject.Key;
        if (string.Equals(dropObject.Value, "PROCEDURE", StringComparison.OrdinalIgnoreCase))
          str = "IF OBJECT_ID('" + key + "', N'P') IS NOT NULL DROP PROCEDURE " + key + ";";
        else if (string.Equals(dropObject.Value, "FUNCTION", StringComparison.OrdinalIgnoreCase))
          str = "IF OBJECT_ID('" + key + "', N'P') IS NOT NULL DROP FUNCTION " + key + ";";
        if (str != null)
          updateScript.Add(new SqlBatch() { Batch = str });
      }
      return updateScript;
    }

    public bool CanSkipUpdateScript(
      ISqlConnectionInfo connectionInfo,
      List<string> sqlScripts,
      IServicingResourceProvider resourceProvider)
    {
      List<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition> proceduresAndFunctions = this.GetAllStoredProceduresAndFunctions(connectionInfo);
      Dictionary<string, string> objects = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, string> dropObjects = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, int> serviceVersions = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      for (int index = 0; index < sqlScripts.Count; ++index)
      {
        string sqlScript = sqlScripts[index];
        this.m_logger.Info("Processing {0}", (object) sqlScript);
        if (this.ReadObjectsFromScript(sqlScript, resourceProvider, objects, dropObjects, serviceVersions))
        {
          this.m_logger.Info("Script contains objects other than stored procedures / functions / service versions.");
          return false;
        }
      }
      this.m_logger.Info("Comparing sql objects ...");
      if (!this.CompareSqlObjects(proceduresAndFunctions, objects, dropObjects))
        return false;
      this.m_logger.Info("Comparing service versions ...");
      if (!this.CompareServiceVersions(connectionInfo, serviceVersions))
        return false;
      this.m_logger.Info("Update can be skipped since it matches with database.");
      return true;
    }

    protected virtual List<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition> GetAllStoredProceduresAndFunctions(
      ISqlConnectionInfo connectionInfo)
    {
      List<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition> storedProcedures;
      using (DatabaseObjectDefinitionComponent componentRaw = connectionInfo.CreateComponentRaw<DatabaseObjectDefinitionComponent>())
      {
        this.m_logger.Info("Querying stored procedures.");
        storedProcedures = componentRaw.GetStoredProcedures();
        this.m_logger.Info("Querying functions.");
        storedProcedures.AddRange((IEnumerable<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition>) componentRaw.GetFunctions());
      }
      return storedProcedures;
    }

    protected virtual List<ServiceVersionEntry> GetDatabaseServiceVersions(
      ISqlConnectionInfo connectionInfo)
    {
      using (ResourceManagementComponent componentRaw = connectionInfo.CreateComponentRaw<ResourceManagementComponent>())
        return componentRaw.QueryServiceVersion().GetCurrent<ServiceVersionEntry>().Items;
    }

    protected bool ReadObjectsFromScript(
      string sqlScript,
      IServicingResourceProvider resourceProvider,
      Dictionary<string, string> objects,
      Dictionary<string, string> dropObjects,
      Dictionary<string, int> serviceVersions)
    {
      using (Stream servicingResource = resourceProvider.GetServicingResource(sqlScript))
      {
        if (servicingResource == null)
          throw new ArgumentException(string.Format("'{0}' not found.", (object) sqlScript), nameof (sqlScript));
        using (StreamReader reader = new StreamReader(servicingResource))
          return this.ReadObjectsFromScript(reader, objects, dropObjects, serviceVersions);
      }
    }

    protected bool ReadObjectsFromScript(
      StreamReader reader,
      Dictionary<string, string> objects,
      Dictionary<string, string> dropObjects,
      Dictionary<string, int> serviceVersions)
    {
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      string str1;
      while ((str1 = reader.ReadLine()) != null)
      {
        if (str1.StartsWith("-->#(Services)"))
          flag2 = true;
        else if (str1.StartsWith("-->#(DropObjects)"))
        {
          flag3 = true;
        }
        else
        {
          if (flag2)
          {
            Match match = DatabaseUpdater.s_setServiceVersionRegex.Match(str1);
            if (match.Success)
            {
              string key = match.Groups["serviceName"].Value;
              int num1 = int.Parse(match.Groups["version"].Value);
              int num2;
              if (serviceVersions.TryGetValue(key, out num2))
              {
                if (num2 != num1)
                  throw new InvalidOperationException(string.Format("'{0}' service is defined in more than 1 script and have different values: {1} and {2}.", (object) key, (object) num1, (object) num2));
                continue;
              }
              serviceVersions.Add(key, num1);
              continue;
            }
            if (!str1.StartsWith("-->#(") && !DatabaseUpdater.s_ignoreLines.Contains(str1))
              this.m_logger.Warning("Failed to parse the following as a Service Version Declaration: " + str1);
          }
          else if (flag3)
          {
            if (string.Equals(str1.Trim(), "GO", StringComparison.OrdinalIgnoreCase))
            {
              flag3 = false;
            }
            else
            {
              Match match = DatabaseUpdater.s_dropRegex.Match(str1);
              if (match.Success)
              {
                string a = match.Groups["type"].Value;
                if (string.Equals(a, "PROCEDURE", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "FUNCTION", StringComparison.OrdinalIgnoreCase))
                {
                  string text1 = match.Groups["name"].Value;
                  int length = text1.IndexOf('.');
                  string text2;
                  if (length > 0)
                  {
                    text2 = text1.Substring(0, length);
                    text1 = text1.Substring(length + 1);
                  }
                  else
                    text2 = "dbo";
                  string key = StringUtil.QuoteName(text2) + "." + StringUtil.QuoteName(text1);
                  dropObjects[key] = a;
                  objects.Remove(key);
                  continue;
                }
                flag1 = true;
                continue;
              }
              if (!str1.StartsWith("-->#(") && !DatabaseUpdater.s_ignoreLines.Contains(str1))
              {
                this.m_logger.Warning("Failed to parse the following as a Drop: " + str1);
                continue;
              }
              continue;
            }
          }
          flag2 = false;
          if (str1.StartsWith("-->#("))
          {
            Match match = DatabaseUpdater.s_sqlObjectInfo.Match(str1);
            if (match.Success)
            {
              string text3 = match.Groups["name"].Value;
              int length = text3.IndexOf('.');
              string text4;
              if (length > 0)
              {
                text4 = text3.Substring(0, length);
                text3 = text3.Substring(length + 1);
              }
              else
                text4 = "dbo";
              string a = match.Groups["type"].Value;
              string key = StringUtil.QuoteName(text4) + "." + StringUtil.QuoteName(text3);
              if (string.Equals(a, "PROCEDURE", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "FUNCTION", StringComparison.OrdinalIgnoreCase))
              {
                string str2 = this.ReadContent(reader, str1);
                objects[key] = str2;
                dropObjects.Remove(key);
              }
              else
                flag1 = true;
            }
          }
        }
      }
      return flag1;
    }

    private bool CompareSqlObjects(
      List<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition> databaseObjects,
      Dictionary<string, string> objects,
      Dictionary<string, string> dropObjects)
    {
      for (int index = 0; index < databaseObjects.Count; ++index)
      {
        string fullName = databaseObjects[index].FullName;
        if (!fullName.Equals("[dbo].[func_GetEndRangeChar]", StringComparison.OrdinalIgnoreCase))
        {
          string content1;
          if (objects.TryGetValue(fullName, out content1) && !this.SqlObjectsEqual(content1, databaseObjects[index].Content, (Predicate<string>) (line => line.StartsWith("CREATE PROCEDURE", StringComparison.OrdinalIgnoreCase) || line.StartsWith("CREATE FUNCTION", StringComparison.OrdinalIgnoreCase))))
          {
            this.m_logger.Info("{0} needs to be updated.", (object) fullName);
            return false;
          }
          objects.Remove(fullName);
        }
      }
      if (objects.Any<KeyValuePair<string, string>>())
      {
        this.m_logger.Info("Script contains objects that are not defined in the database: {0}", (object) string.Join<KeyValuePair<string, string>>(", ", (IEnumerable<KeyValuePair<string, string>>) objects.ToArray<KeyValuePair<string, string>>()));
        return false;
      }
      if (dropObjects.Any<KeyValuePair<string, string>>())
      {
        HashSet<string> currentNames = new HashSet<string>(databaseObjects.Select<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition, string>((Func<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition, string>) (x => x.FullName)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        string[] array = dropObjects.Keys.Where<string>((Func<string, bool>) (x => currentNames.Contains(x))).ToArray<string>();
        if (((IEnumerable<string>) array).Any<string>())
        {
          this.m_logger.Info("Script contains objects that must be dropped from the database: {0}", (object) string.Join(", ", array));
          return false;
        }
      }
      return true;
    }

    private bool SqlObjectsEqual(
      string content1,
      string content2,
      Predicate<string> firstLinePredicate)
    {
      string[] separator = new string[1]
      {
        Environment.NewLine
      };
      return this.SqlObjectsEqual(content1.Split(separator, StringSplitOptions.RemoveEmptyEntries), content2.Split(separator, StringSplitOptions.RemoveEmptyEntries), firstLinePredicate);
    }

    private bool SqlObjectsEqual(
      string[] lines1,
      string[] lines2,
      Predicate<string> firstLinePredicate)
    {
      int index1 = Array.FindIndex<string>(lines1, firstLinePredicate);
      int index2 = Array.FindIndex<string>(lines2, firstLinePredicate);
      if (index1 < 0 || index2 < 0)
        return false;
      int num = lines1.Length - index1;
      if (num != lines2.Length - index2)
        return false;
      bool flag1 = false;
      bool flag2 = false;
      for (int index3 = 0; index3 < num; ++index3)
      {
        string str1 = lines1[index1 + index3];
        string str2 = lines2[index2 + index3];
        if (flag1)
        {
          if (str1.Contains("-->#END DBG"))
          {
            flag1 = false;
            str1 = "-- END NOCOMPARE (END DBG)";
          }
          if (!flag2)
            str1 = string.Empty;
        }
        else if (str1.Contains("-->#BEGIN DBG"))
        {
          flag1 = true;
          str1 = !flag2 ? string.Empty : "-- BEGIN NOCOMPARE (BEGIN DBG)";
        }
        if (!string.Equals(str1.Trim(), str2.Trim(), StringComparison.Ordinal))
          return false;
      }
      return true;
    }

    private bool CompareServiceVersions(
      ISqlConnectionInfo connectionInfo,
      Dictionary<string, int> serviceVersions)
    {
      foreach (ServiceVersionEntry databaseServiceVersion in this.GetDatabaseServiceVersions(connectionInfo))
      {
        int num;
        if (serviceVersions.TryGetValue(databaseServiceVersion.ServiceName, out num) && num != databaseServiceVersion.Version)
        {
          this.m_logger.Info("Service {0} version {1} mismatch with script version {2}.", (object) databaseServiceVersion.ServiceName, (object) databaseServiceVersion.Version, (object) num);
          return false;
        }
        serviceVersions.Remove(databaseServiceVersion.ServiceName);
      }
      serviceVersions.Where<KeyValuePair<string, int>>((Func<KeyValuePair<string, int>, bool>) (x => x.Value == -1)).Select<KeyValuePair<string, int>, string>((Func<KeyValuePair<string, int>, string>) (x => x.Key)).ToHashSet<string>().ForEach<string>((Action<string>) (x => serviceVersions.Remove(x)));
      if (!serviceVersions.Any<KeyValuePair<string, int>>())
        return true;
      this.m_logger.Info("Script contains service version definitions that a missing in the database.");
      return false;
    }

    private string ReadContent(StreamReader reader, string header)
    {
      bool flag1 = false;
      bool flag2 = false;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(header);
      string str;
      while ((str = reader.ReadLine()) != null && !str.Equals("GO", StringComparison.OrdinalIgnoreCase))
      {
        if (flag2)
        {
          if (str.Contains("-->#END DBG"))
          {
            flag2 = false;
            str = "-- END NOCOMPARE (END DBG)";
          }
          if (!flag1)
            str = string.Empty;
        }
        else if (str.Contains("-->#BEGIN DBG"))
        {
          flag2 = true;
          str = !flag1 ? string.Empty : "-- BEGIN NOCOMPARE (BEGIN DBG)";
        }
        stringBuilder.AppendLine(str);
      }
      return stringBuilder.ToString();
    }
  }
}
