// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataImport.DataImportConnectionStringHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server.DataImport
{
  public class DataImportConnectionStringHelper
  {
    private readonly string m_connectionString;
    private readonly Guid m_importId;
    public static readonly string CollationPropertyName = "Collation";
    public static readonly string SnapshotTablePropertyName = "SnapshotTableExists";
    public static readonly string SnapshotTableToCheck = "tbl_Group";
    public static readonly string SnapshotFullTableNameToCheck = "[dbo].[" + DataImportConnectionStringHelper.SnapshotTableToCheck + "]";
    public static readonly string ContainsExportedDataPropertyName = "ContainsExportedData";
    public static readonly string[] RequiredExtendedProperties = new string[5]
    {
      CollectionMoveConstants.SnapshotStateExtendedProperty,
      TeamFoundationSqlResourceComponent.ExtendedPropertyDatabaseType,
      TeamFoundationSqlResourceComponent.ExtendedPropertyServiceLevelStamp,
      TeamFoundationSqlResourceComponent.ExtendedPropertySnapshotCollectionId,
      DataImportConstants.DataImportCollectionIdExtendedProperty
    };
    public static readonly IEnumerable<string> RequiredAttributes = (IEnumerable<string>) new string[2]
    {
      DataImportConnectionStringHelper.CollationPropertyName,
      DataImportConnectionStringHelper.SnapshotTablePropertyName
    };
    public static readonly IEnumerable<string> RequiredDatabaseProperties = (IEnumerable<string>) new ReadOnlyCollection<string>((IList<string>) ((IEnumerable<string>) DataImportConnectionStringHelper.RequiredExtendedProperties).Concat<string>(DataImportConnectionStringHelper.RequiredAttributes).ToList<string>());
    public static readonly TimeSpan ConnectionTimeout = TimeSpan.FromSeconds(30.0);
    private static readonly TimeSpan s_defaultConnectionWaitTime = TimeSpan.FromSeconds(45.0);
    private const string c_area = "DataImport";
    private const string c_layer = "DataImportConnectionStringHelper";

    public DataImportConnectionStringHelper(Guid importId, string connectionString)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(connectionString, nameof (connectionString));
      this.m_connectionString = connectionString;
      this.m_importId = importId;
    }

    public void ValidateConnectionString(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(this.m_connectionString);
      if (connectionStringBuilder.IntegratedSecurity)
        throw new InvalidImportSourceConnectionStringException(FrameworkResources.ConnectionStringMustUseSQLAuth());
      if (!connectionStringBuilder.Encrypt)
        throw new InvalidImportSourceConnectionStringException(FrameworkResources.ConnectionStringMustUseEncryption());
      requestContext.TraceAlways(15080821, TraceLevel.Info, "DataImport", nameof (DataImportConnectionStringHelper), DataImportConnectionStringHelper.ConnectionStringHashLogMessage(this.m_importId, this.m_connectionString));
      this.CheckConnectWithTimeout(requestContext);
      if (!this.IsInDatabaseRole(SqlConnectionInfoFactory.Create(this.m_connectionString), DatabaseRoles.TfsExecRole))
        throw new UserNotInDatabaseRoleException(FrameworkResources.ConnectionStringUserMustBeInRole((object) connectionStringBuilder.UserID, (object) DatabaseRoles.TfsExecRole));
    }

    internal virtual void CheckConnectWithTimeout(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      TimeSpan timeout = vssRequestContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(vssRequestContext, new RegistryQuery("/Configuration/DataImport/WaitForConnectionTimeout"), DataImportConnectionStringHelper.s_defaultConnectionWaitTime);
      string str = (string) null;
      try
      {
        str = ConnectionStringUtility.MaskPassword(this.m_connectionString);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15080810, TraceLevel.Error, "DataImport", nameof (DataImportConnectionStringHelper), ex);
      }
      requestContext.TraceAlways(15080811, TraceLevel.Info, "DataImport", nameof (DataImportConnectionStringHelper), string.Format("Starting to connect to {0} will wait {1}, request's {2}:{3}", (object) str, (object) timeout, (object) "IsCanceled", (object) requestContext.IsCanceled));
      ConcurrentDictionary<int, string> messages = new ConcurrentDictionary<int, string>();
      messages[0] = string.Empty;
      try
      {
        using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
        {
          ISqlConnectionInfo connectionInfo = SqlConnectionInfoFactory.Create(this.m_connectionString);
          Task task = this.ConnectionValidationTaskAsync(messages, connectionInfo, cancellationTokenSource.Token);
          try
          {
            task.Wait(timeout);
            cancellationTokenSource.Cancel();
          }
          catch (AggregateException ex)
          {
            requestContext.TraceException(15080812, TraceLevel.Error, "DataImport", nameof (DataImportConnectionStringHelper), (Exception) ex);
            throw ex.InnerExceptions.First<Exception>();
          }
          catch (Exception ex)
          {
            requestContext.TraceException(15080813, TraceLevel.Error, "DataImport", nameof (DataImportConnectionStringHelper), ex);
            throw;
          }
          if (task.IsCompleted && !task.IsFaulted)
          {
            if (!task.IsCanceled)
              goto label_15;
          }
          requestContext.TraceAlways(15080814, TraceLevel.Error, "DataImport", nameof (DataImportConnectionStringHelper), string.Format("{0}:{1}, {2}:{3}, {4}:{5}", (object) "IsCompleted", (object) task.IsCompleted, (object) "IsFaulted", (object) task.IsFaulted, (object) "IsCanceled", (object) task.IsCanceled));
          throw new TimeoutException(string.Format("Waited {0} to connect to {1}", (object) timeout, (object) str));
        }
label_15:
        requestContext.TraceAlways(15080815, TraceLevel.Info, "DataImport", nameof (DataImportConnectionStringHelper), "Completed");
      }
      finally
      {
        requestContext.TraceAlways(15080818, TraceLevel.Info, "DataImport", nameof (DataImportConnectionStringHelper), "Detail from sub task: " + string.Join(Environment.NewLine, ((IEnumerable<KeyValuePair<int, string>>) messages.ToArray()).OrderBy<KeyValuePair<int, string>, int>((Func<KeyValuePair<int, string>, int>) (x => x.Key)).Select<KeyValuePair<int, string>, string>((Func<KeyValuePair<int, string>, string>) (x => x.Value))));
        if (requestContext.IsCanceled)
          requestContext.TraceAlways(15080816, TraceLevel.Error, "DataImport", nameof (DataImportConnectionStringHelper), "The Request has already been canceled");
      }
    }

    private async Task ConnectionValidationTaskAsync(
      ConcurrentDictionary<int, string> messages,
      ISqlConnectionInfo connectionInfo,
      CancellationToken cancellationToken)
    {
      ConcurrentDictionary<int, string> messages1 = messages;
      try
      {
        using (SqlConnection connection = connectionInfo.CreateSqlConnection())
        {
          await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
          Log("Connected");
          using (ExtendedAttributeComponent componentRaw = connectionInfo.CreateComponentRaw<ExtendedAttributeComponent>())
          {
            Log("Created component");
            componentRaw.ReadDatabaseAttribute("ThisIsNotAnAttribute");
            Log("Read Database Attribute");
          }
        }
      }
      catch (Exception ex)
      {
        Log(string.Format("Caught Exception: {0}", (object) ex));
        throw;
      }
      Log("Finished");

      void Log(string message) => messages1[messages1.Count] = "[" + DateTime.UtcNow.ToString("O") + "]" + message;
    }

    public virtual IDictionary<string, string> GetDatabaseProperties()
    {
      Dictionary<string, string> databaseProperties = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      ISqlConnectionInfo connectionInfo = SqlConnectionInfoFactory.Create(this.m_connectionString);
      try
      {
        using (ExtendedAttributeComponent componentRaw = connectionInfo.CreateComponentRaw<ExtendedAttributeComponent>())
        {
          string[] strArray = componentRaw.ReadDatabaseAttributes(DataImportConnectionStringHelper.RequiredExtendedProperties);
          for (int index = 0; index < DataImportConnectionStringHelper.RequiredExtendedProperties.Length; ++index)
            databaseProperties.Add(DataImportConnectionStringHelper.RequiredExtendedProperties[index], strArray[index]);
        }
        using (CollationComponent componentRaw = connectionInfo.CreateComponentRaw<CollationComponent>())
          databaseProperties.Add(DataImportConnectionStringHelper.CollationPropertyName, componentRaw.GetDatabaseCollation());
        using (DatabaseObjectDefinitionComponent componentRaw = connectionInfo.CreateComponentRaw<DatabaseObjectDefinitionComponent>())
          databaseProperties.Add(DataImportConnectionStringHelper.SnapshotTablePropertyName, componentRaw.GetTables(DataImportConnectionStringHelper.SnapshotTableToCheck).Any<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition>((Func<DatabaseObjectDefinitionComponent.DatabaseObjectDefinition, bool>) (x => string.Equals(x.FullName, DataImportConnectionStringHelper.SnapshotFullTableNameToCheck))).ToString());
      }
      catch (DatabaseConnectionException ex)
      {
        throw new InvalidImportSourceConnectionStringException(FrameworkResources.InvalidImportConnectionString((object) ConnectionStringUtility.MaskPassword(this.m_connectionString)), (Exception) ex);
      }
      return (IDictionary<string, string>) databaseProperties;
    }

    public static string GetRequiredProperty(
      IDictionary<string, string> properties,
      string propertyName)
    {
      ArgumentUtility.CheckForNull<IDictionary<string, string>>(properties, nameof (properties));
      ArgumentUtility.CheckStringForNullOrEmpty(propertyName, nameof (propertyName));
      string requiredProperty;
      if (!properties.TryGetValue(propertyName, out requiredProperty) || string.IsNullOrEmpty(requiredProperty))
        throw new SourceIsNotADetachedDatabaseException((Exception) new MissingSourceExtendedPropertyException(DataImportResources.MissingSourceExtendedProperty((object) propertyName)));
      return requiredProperty;
    }

    public static void CheckStaticProperties(IDictionary<string, string> properties)
    {
      ArgumentUtility.CheckForNull<IDictionary<string, string>>(properties, nameof (properties));
      DataImportConnectionStringHelper.ValidateDatabaseType(properties);
      DataImportConnectionStringHelper.ValidateExtendedProperty(DataImportConnectionStringHelper.GetRequiredProperty(properties, CollectionMoveConstants.SnapshotStateExtendedProperty), CollectionMoveConstants.SnapshotStateExtendedProperty, CollectionMoveConstants.SnapshotStateComplete);
      DataImportConnectionStringHelper.RequiredDatabaseProperties.Except<string>((IEnumerable<string>) new string[1]
      {
        DataImportConnectionStringHelper.SnapshotTablePropertyName
      }).ForEach<string>((Action<string>) (propertyName => DataImportConnectionStringHelper.GetRequiredProperty(properties, propertyName)));
      DataImportConnectionStringHelper.CheckSnapshotTable(properties);
      DataImportConnectionStringHelper.CheckForExportedData(properties);
    }

    private static void CheckForExportedData(IDictionary<string, string> properties)
    {
      string str;
      bool result;
      if (((!properties.TryGetValue(DataImportConnectionStringHelper.ContainsExportedDataPropertyName, out str) ? 0 : (bool.TryParse(str, out result) ? 1 : 0)) & (result ? 1 : 0)) != 0)
        throw new SourceContainsExportedDataException();
    }

    private static void CheckSnapshotTable(IDictionary<string, string> properties)
    {
      string str;
      bool result;
      if (!properties.TryGetValue(DataImportConnectionStringHelper.SnapshotTablePropertyName, out str) || !bool.TryParse(str, out result) || !result)
        throw new SourceIsMissingSnapshotTablesException();
    }

    public static string ConnectionStringHashLogMessage(Guid importId, string connectionString)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("FullHash:");
      if (string.IsNullOrEmpty(connectionString))
      {
        stringBuilder.Append(" <empty>");
      }
      else
      {
        using (SHA512Managed shA512Managed = new SHA512Managed())
        {
          byte[] bytes1 = Encoding.ASCII.GetBytes(connectionString + importId.ToString());
          foreach (byte num in shA512Managed.ComputeHash(bytes1))
            stringBuilder.Append(num.ToString("X2"));
          stringBuilder.Append(" Password Hash:");
          try
          {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            if (string.IsNullOrEmpty(connectionStringBuilder.Password))
            {
              stringBuilder.Append(" <empty>");
            }
            else
            {
              byte[] bytes2 = Encoding.ASCII.GetBytes(connectionStringBuilder.Password + importId.ToString());
              foreach (byte num in shA512Managed.ComputeHash(bytes2))
                stringBuilder.Append(num.ToString("X2"));
            }
          }
          catch (Exception ex)
          {
            stringBuilder.Append(string.Format(" <Exception> {0}", (object) ex));
          }
        }
      }
      return stringBuilder.ToString();
    }

    internal virtual bool IsInDatabaseRole(ISqlConnectionInfo connectionInfo, string role)
    {
      using (TeamFoundationSqlSecurityComponent componentRaw = connectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>())
        return componentRaw.IsRoleMember(role);
    }

    private static void ValidateDatabaseType(IDictionary<string, string> properties)
    {
      string requiredProperty = DataImportConnectionStringHelper.GetRequiredProperty(properties, TeamFoundationSqlResourceComponent.ExtendedPropertyDatabaseType);
      if (string.Equals(requiredProperty, TeamFoundationSqlResourceComponent.DatabaseTypeConfiguration, StringComparison.OrdinalIgnoreCase))
        throw new SourceIsTFSConfigurationDatabaseException();
      DataImportConnectionStringHelper.ValidateExtendedProperty(requiredProperty, TeamFoundationSqlResourceComponent.ExtendedPropertyDatabaseType, TeamFoundationSqlResourceComponent.DatabaseTypeCollection);
    }

    private static void ValidateExtendedProperty(string value, string name, string expected)
    {
      if (!string.Equals(value, expected, StringComparison.OrdinalIgnoreCase))
        throw new SourceIsNotADetachedDatabaseException((Exception) new InvalidSourceExtendedPropertyValueException(name, expected, value));
    }
  }
}
