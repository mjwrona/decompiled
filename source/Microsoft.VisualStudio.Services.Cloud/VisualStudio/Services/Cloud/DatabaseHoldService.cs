// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.DatabaseHoldService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class DatabaseHoldService : IDatabaseHoldService, IVssFrameworkService
  {
    private static readonly TimeSpan s_leaseTime = TimeSpan.FromSeconds(30.0);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckDeploymentRequestContext();

    public bool ObtainDatabaseHold(
      IVssRequestContext deploymentRequestContext,
      Guid key,
      int targetDatabaseId,
      string reason,
      TimeSpan timeUntilAbandon,
      ITFLogger logger)
    {
      deploymentRequestContext.CheckDeploymentRequestContext();
      if (logger == null)
        logger = (ITFLogger) new NullLogger();
      IVssRegistryService service = deploymentRequestContext.GetService<IVssRegistryService>();
      using (this.AcquireDatabaseHoldLease(deploymentRequestContext, targetDatabaseId))
      {
        RegistryQuery query = new RegistryQuery(string.Format("/DatabaseHold/{0}/**", (object) targetDatabaseId));
        Dictionary<string, string> dictionary = service.Read(deploymentRequestContext, in query).ToDictionary<RegistryItem, string, string>((Func<RegistryItem, string>) (x => x.Path), (Func<RegistryItem, string>) (x => x.Value), (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
        if (dictionary.Any<KeyValuePair<string, string>>())
        {
          string input = this.EmptyOrValue(dictionary, targetDatabaseId, "/DatabaseHold/{0}/Key");
          Guid guid = Guid.Parse(input);
          if (key != guid)
          {
            string str1 = this.EmptyOrValue(dictionary, targetDatabaseId, "/DatabaseHold/{0}/Reason");
            string str2 = this.EmptyOrValue(dictionary, targetDatabaseId, "/DatabaseHold/{0}/ObtainedOn");
            string s = this.EmptyOrValue(dictionary, targetDatabaseId, "/DatabaseHold/{0}/CanAbandonAt");
            DateTime maxValue = DateTime.MaxValue;
            if (!string.IsNullOrEmpty(s))
              maxValue = DateTime.Parse(s, (IFormatProvider) null, DateTimeStyles.RoundtripKind);
            if (DateTime.UtcNow > maxValue)
            {
              logger.Warning(string.Format("Database Hold for {0} already claimed on {1} for {2} key {3} was eligible for abandonment at {4}, hold will be treated as abandoned", (object) targetDatabaseId, (object) str2, (object) str1, (object) input, (object) s));
            }
            else
            {
              logger.Info(string.Format("Database Hold for {0} already claimed on {1} for {2} key {3}", (object) targetDatabaseId, (object) str2, (object) str1, (object) input));
              return false;
            }
          }
        }
        DateTime utcNow = DateTime.UtcNow;
        DateTime dateTime = utcNow + timeUntilAbandon;
        service.Write(deploymentRequestContext, (IEnumerable<RegistryItem>) new RegistryItem[4]
        {
          new RegistryItem(string.Format("/DatabaseHold/{0}/ObtainedOn", (object) targetDatabaseId), utcNow.ToString("O")),
          new RegistryItem(string.Format("/DatabaseHold/{0}/CanAbandonAt", (object) targetDatabaseId), dateTime.ToString("O")),
          new RegistryItem(string.Format("/DatabaseHold/{0}/Reason", (object) targetDatabaseId), reason),
          new RegistryItem(string.Format("/DatabaseHold/{0}/Key", (object) targetDatabaseId), key.ToString("D"))
        });
        logger.Info(string.Format("Database Hold on {0} obtained for {1} with key {2}, can abandon at {3}", (object) targetDatabaseId, (object) reason, (object) key, (object) dateTime.ToString("O")));
      }
      return true;
    }

    public bool ReleaseDatabasehold(
      IVssRequestContext deploymentRequestContext,
      int targetDatabaseId,
      Guid key,
      ITFLogger logger)
    {
      deploymentRequestContext.CheckDeploymentRequestContext();
      if (logger == null)
        logger = (ITFLogger) new NullLogger();
      IVssRegistryService service = deploymentRequestContext.GetService<IVssRegistryService>();
      using (this.AcquireDatabaseHoldLease(deploymentRequestContext, targetDatabaseId))
      {
        Guid guid = service.GetValue<Guid>(deploymentRequestContext, (RegistryQuery) string.Format("/DatabaseHold/{0}/Key", (object) targetDatabaseId), Guid.Empty);
        if (guid != key)
        {
          logger.Info(string.Format("Database hold on {0} not held for {1} instead it is held for {2}", (object) targetDatabaseId, (object) key, (object) guid));
          return false;
        }
        service.DeleteEntries(deploymentRequestContext, string.Format("/DatabaseHold/{0}/**", (object) targetDatabaseId));
        logger.Info(string.Format("Database hold on {0} held for {1} has been released", (object) targetDatabaseId, (object) key));
      }
      return true;
    }

    public IList<DatabaseHoldData> ListDatabaseHolds(
      IVssRequestContext deploymentRequestContext,
      ITFLogger logger)
    {
      deploymentRequestContext.CheckDeploymentRequestContext();
      if (logger == null)
        logger = (ITFLogger) new NullLogger();
      IVssRegistryService service = deploymentRequestContext.GetService<IVssRegistryService>();
      Dictionary<int, DatabaseHoldData> dictionary = new Dictionary<int, DatabaseHoldData>();
      IVssRequestContext requestContext = deploymentRequestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @new RegistryQuery("/DatabaseHold/**");
      foreach (RegistryItem registryItem in service.Read(requestContext, in local))
      {
        logger.Info("Found Path: " + registryItem.Path + " set to " + registryItem.Value);
        int registryKey = this.ParseRegistryKey(registryItem.Path);
        if (registryKey == DatabaseManagementConstants.InvalidDatabaseId)
        {
          logger.Info("Failed to parse Database Id from " + registryItem.Path);
        }
        else
        {
          DatabaseHoldData databaseHoldData;
          if (!dictionary.TryGetValue(registryKey, out databaseHoldData))
          {
            databaseHoldData = new DatabaseHoldData()
            {
              DatabaseId = registryKey
            };
            dictionary[registryKey] = databaseHoldData;
          }
          if (string.Equals(string.Format("/DatabaseHold/{0}/Key", (object) registryKey), registryItem.Path))
            databaseHoldData.Key = Guid.Parse(registryItem.Value);
          else if (string.Equals(string.Format("/DatabaseHold/{0}/Reason", (object) registryKey), registryItem.Path))
            databaseHoldData.Reason = registryItem.Value;
        }
      }
      return (IList<DatabaseHoldData>) dictionary.Values.ToList<DatabaseHoldData>();
    }

    private int ParseRegistryKey(string key)
    {
      if (key.Length <= "/DatabaseHold".Length)
        return DatabaseManagementConstants.InvalidDatabaseId;
      int result;
      return !int.TryParse(key.Substring("/DatabaseHold".Length + 1).Split('/')[0], out result) ? DatabaseManagementConstants.InvalidDatabaseId : result;
    }

    private string EmptyOrValue(Dictionary<string, string> dictionary, int databaseId, string key)
    {
      string key1 = string.Format(key, (object) databaseId);
      string empty;
      dictionary.TryGetValue(key1, out empty);
      if (string.IsNullOrEmpty(empty))
        empty = string.Empty;
      return empty;
    }

    private IDisposable AcquireDatabaseHoldLease(
      IVssRequestContext deploymentRequestContext,
      int databaseId)
    {
      string leaseName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DatabaseHold-{0}", (object) databaseId);
      return (IDisposable) deploymentRequestContext.GetService<ILeaseService>().AcquireLease(deploymentRequestContext, leaseName, DatabaseHoldService.s_leaseTime, DatabaseHoldService.s_leaseTime);
    }
  }
}
