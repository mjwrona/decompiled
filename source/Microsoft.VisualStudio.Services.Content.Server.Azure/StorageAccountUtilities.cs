// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.StorageAccountUtilities
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.MultiDomainExceptions;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public static class StorageAccountUtilities
  {
    public static string StrongBoxConnectionStringDrawer = FrameworkServerConstants.StorageConnectionsDrawerName;
    public const string StrongBoxStorageAccountInfoDrawer = "StorageAccountInfo";
    public const string LocationModeDrawer = "StorageAccountInfo";
    public const string LocationModeKey = "LocationMode";
    public const string StorageAccountKeyBaseNameKey = "StorageAccountKeyBaseName";
    public const string StorageAccountCountKey = "StorageAccountCount";
    internal const string AzureConnStringFormat = "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}";
    internal const string AzureConnStringPtn = "^DefaultEndpointsProtocol=(http[s]?);AccountName=(.+);AccountKey=(.+)$";
    private const string EmulatorUri = "UseDevelopmentStorage=true";
    private static readonly string TraceArea = "ServiceShared";
    private static readonly string TraceLayer = nameof (StorageAccountUtilities);
    private static readonly TraceData TraceData = new TraceData()
    {
      Area = StorageAccountUtilities.TraceArea,
      Layer = StorageAccountUtilities.TraceLayer
    };
    internal static readonly int TracePointReadAllStorageAccounts = 5701800;
    private const string DefaultDiagnosticContainerName = "proddumps";

    public static string GetStorageAccountKeyBaseName(IVssRequestContext context)
    {
      ITeamFoundationStrongBoxService strongBoxService = context.IsSystemContext ? context.GetService<ITeamFoundationStrongBoxService>() : throw new ArgumentException("StorageAccountUtilities.GetStorageAccountKeyBaseName requires an elevated request context");
      StrongBoxItemInfo itemInfo = strongBoxService.GetItemInfo(context, "StorageAccountInfo", "StorageAccountKeyBaseName", false);
      return strongBoxService.GetString(context, itemInfo);
    }

    internal static IEnumerable<StrongBoxConnectionString> ReadAllStorageAccounts(
      IVssRequestContext context,
      PhysicalDomainInfo physicalDomainInfo = null)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(context, StorageAccountUtilities.TraceData, StorageAccountUtilities.TracePointReadAllStorageAccounts, nameof (ReadAllStorageAccounts)))
      {
        Func<int, int, int> sbIndexTask = (Func<int, int, int>) ((sbItemIndex, count) =>
        {
          if (physicalDomainInfo != null && !physicalDomainInfo.DomainId.Equals(WellKnownDomainIds.DefaultDomainId))
            sbItemIndex = count - 1;
          return sbItemIndex;
        });
        Func<int, int, AzureStorageAccountInfo, bool> addConnStrTask = (Func<int, int, AzureStorageAccountInfo, bool>) ((sbItemIndex, count, accountInfo) =>
        {
          bool flag = false;
          if (physicalDomainInfo == null)
          {
            if (sbItemIndex < count)
              flag = true;
          }
          else if (StorageAccountUtilities.DomainContainsShard(physicalDomainInfo, accountInfo))
            flag = true;
          return flag;
        });
        int count1;
        IEnumerable<StrongBoxConnectionString> connectionStrings = StorageAccountUtilities.GetSBConnectionStrings(context, sbIndexTask, addConnStrTask, out count1);
        int num = count1;
        int actualShardCount = connectionStrings.Count<StrongBoxConnectionString>();
        if (physicalDomainInfo != null)
          num = physicalDomainInfo.Shards.Count;
        if (num != actualShardCount)
        {
          if (physicalDomainInfo == null)
            throw new CannotReadStorageAccountException(string.Format("{0} shards were expected for the legacy domain as per the {1}, but only {2} were retrieved from StrongBox.", (object) num, (object) "StorageAccountCount", (object) actualShardCount));
          if (context.ServiceHost.IsProduction)
            throw new InvalidDomainShardListException(string.Format("{0} shards were expected for DomainId {1}, but only {2} were retrieved from StrongBox.", (object) num, (object) physicalDomainInfo.DomainId.Serialize(), (object) actualShardCount));
        }
        context.TraceConditionally(StorageAccountUtilities.TracePointReadAllStorageAccounts, TraceLevel.Info, StorageAccountUtilities.TraceArea, StorageAccountUtilities.TraceLayer, (Func<string>) (() => string.Format("{0} returning {1} accounts for {2}", (object) nameof (ReadAllStorageAccounts), (object) actualShardCount, physicalDomainInfo != null ? (object) ("DomainId " + physicalDomainInfo.DomainId.Serialize()) : (object) "legacy domain (null PhysicalDomainInfo)")));
        return (IEnumerable<StrongBoxConnectionString>) connectionStrings.OrderBy<StrongBoxConnectionString, string>((Func<StrongBoxConnectionString, string>) (a => StorageAccountUtilities.GetAccountInfo(a.ConnectionString).Name)).ToList<StrongBoxConnectionString>();
      }
    }

    internal static IEnumerable<StrongBoxConnectionString> ReadAllStorageAccountsForDomainId(
      IVssRequestContext context,
      IDomainId domainId = null)
    {
      Func<int, int, AzureStorageAccountInfo, bool> addConnStrTask = (Func<int, int, AzureStorageAccountInfo, bool>) ((sbItemIndex, count, accountInfo) =>
      {
        if (!(domainId != (IDomainId) null))
          return true;
        Match match = new Regex("d([0-9]+)s([0-9]+)$").Match(accountInfo.Name);
        if (!match.Success)
          return false;
        string str = match.Groups[1].Value;
        return domainId.Serialize().Equals(str);
      });
      return (IEnumerable<StrongBoxConnectionString>) StorageAccountUtilities.GetSBConnectionStrings(context, (Func<int, int, int>) ((sbItemIndex, count) => -1), addConnStrTask, out int _).OrderBy<StrongBoxConnectionString, string>((Func<StrongBoxConnectionString, string>) (a => StorageAccountUtilities.GetAccountInfo(a.ConnectionString).Name)).ToList<StrongBoxConnectionString>();
    }

    private static IEnumerable<StrongBoxConnectionString> GetSBConnectionStrings(
      IVssRequestContext context,
      Func<int, int, int> sbIndexTask,
      Func<int, int, AzureStorageAccountInfo, bool> addConnStrTask,
      out int count)
    {
      ITeamFoundationStrongBoxService strongBoxService = context.IsSystemContext ? context.GetService<ITeamFoundationStrongBoxService>() : throw new ArgumentException("StorageAccountUtilities.ReadAllStorageAccounts requires an elevated request context");
      string lookupKey1 = "StorageAccountKeyBaseName";
      string lookupKey2 = "StorageAccountCount";
      string str1;
      try
      {
        StrongBoxItemInfo itemInfo1 = strongBoxService.GetItemInfo(context, "StorageAccountInfo", lookupKey1, false);
        str1 = strongBoxService.GetString(context, itemInfo1);
        StrongBoxItemInfo itemInfo2 = strongBoxService.GetItemInfo(context, "StorageAccountInfo", lookupKey2, false);
        count = int.Parse(strongBoxService.GetString(context, itemInfo2));
      }
      catch (Exception ex)
      {
        throw new CannotReadStorageAccountException("Failed to retrieve settings required for storage account information from strongbox. Both " + lookupKey1 + " and " + lookupKey2 + " are needed.", ex);
      }
      if (count <= 0 || string.IsNullOrWhiteSpace(str1))
        throw new CannotReadStorageAccountException("Missing settings required for using sharding storage accounts. Both " + lookupKey1 + " and " + lookupKey2 + " are needed.");
      List<StrongBoxConnectionString> source = new List<StrongBoxConnectionString>();
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      bool isProduction = context.ServiceHost.IsProduction;
      int num = -1;
      if (isProduction)
        num = sbIndexTask(num, count);
      bool flag;
      do
      {
        ++num;
        string str2 = string.Format("{0}{1}", (object) str1, (object) num);
        StrongBoxItemInfo itemInfo;
        try
        {
          itemInfo = strongBoxService.GetItemInfo(context, StorageAccountUtilities.StrongBoxConnectionStringDrawer, str2, false);
        }
        catch (Exception ex)
        {
          throw new CannotReadStorageAccountException("StrongBox.GetItemInfo failed to read item " + str2 + " from drawer " + StorageAccountUtilities.StrongBoxConnectionStringDrawer, ex);
        }
        flag = itemInfo != null;
        if (!flag && num < count)
          throw new CannotReadStorageAccountException(string.Format("StrongBox doesn't contain an item at lookupKey {0} (index {1} is < {2}).", (object) str2, (object) num, (object) count));
        if (flag)
        {
          string connectionString1 = strongBoxService.GetString(context, itemInfo);
          StrongBoxConnectionString connectionString2 = !string.IsNullOrWhiteSpace(connectionString1) ? new StrongBoxConnectionString(connectionString1, str2) : throw new CannotReadStorageAccountException("StrongBox contains an item at lookupKey " + str2 + ", but its value is empty.");
          AzureStorageAccountInfo accountInfo = StorageAccountUtilities.GetAccountInfo(connectionString2.ConnectionString);
          if (accountInfo == null)
            throw new CannotReadStorageAccountException("StrongBox contains an item at lookupKey " + str2 + ", but its value isn't an Azure Storage connection string.");
          if (addConnStrTask(num, count, accountInfo))
          {
            if (isProduction)
            {
              string name = accountInfo.Name;
              string str3;
              if (dictionary.TryGetValue(name, out str3))
                throw new CannotReadStorageAccountException("StrongBox contains Azure Storage account name " + name + " at more than one item. This item is " + str2 + ". Previously seen at " + str3 + ".");
              dictionary.Add(name, str2);
            }
            source.Add(connectionString2);
          }
        }
      }
      while (flag);
      return (IEnumerable<StrongBoxConnectionString>) source.OrderBy<StrongBoxConnectionString, string>((Func<StrongBoxConnectionString, string>) (a => StorageAccountUtilities.GetAccountInfo(a.ConnectionString).Name)).ToList<StrongBoxConnectionString>();
    }

    private static bool DomainContainsShard(
      PhysicalDomainInfo domainInfo,
      AzureStorageAccountInfo accountInfo)
    {
      ArgumentUtility.CheckForNull<PhysicalDomainInfo>(domainInfo, nameof (domainInfo));
      ArgumentUtility.CheckForNull<AzureStorageAccountInfo>(accountInfo, nameof (accountInfo));
      return accountInfo.IsDevelopment ? domainInfo.DomainId.Equals(WellKnownDomainIds.OriginalDomainId) : domainInfo.Shards.Contains(accountInfo.Name);
    }

    internal static string GetLocationMode(IVssRequestContext context)
    {
      context.CheckSystemRequestContext();
      context.CheckDeploymentRequestContext();
      ITeamFoundationStrongBoxService service = context.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(context, "StorageAccountInfo", "LocationMode", false);
      return itemInfo != null ? service.GetString(context, itemInfo) : (string) null;
    }

    public static AzureStorageAccountInfo GetAccountInfo(string connString)
    {
      Match match = Regex.Match(connString, "^DefaultEndpointsProtocol=(http[s]?);AccountName=(.+);AccountKey=(.+)$");
      if (match.Success)
        return AzureStorageAccountInfo.Create(match.Groups[1].ToString(), match.Groups[2].ToString());
      return string.Equals(connString, "UseDevelopmentStorage=true", StringComparison.OrdinalIgnoreCase) ? AzureStorageAccountInfo.GetDevelopment() : (AzureStorageAccountInfo) null;
    }

    public static Microsoft.Azure.Storage.RetryPolicies.LocationMode? ParseLocationMode(
      string locationModeString)
    {
      Microsoft.Azure.Storage.RetryPolicies.LocationMode? locationMode = new Microsoft.Azure.Storage.RetryPolicies.LocationMode?();
      StorageAccountUtilities.TryParseLocationMode(locationModeString, out locationMode);
      return locationMode;
    }

    internal static bool TryParseLocationMode(
      string locationModeString,
      out Microsoft.Azure.Storage.RetryPolicies.LocationMode? locationMode)
    {
      if (string.Equals(locationModeString, "PrimaryOnly", StringComparison.OrdinalIgnoreCase))
        locationMode = new Microsoft.Azure.Storage.RetryPolicies.LocationMode?(Microsoft.Azure.Storage.RetryPolicies.LocationMode.PrimaryOnly);
      else if (string.Equals(locationModeString, "SecondaryOnly", StringComparison.OrdinalIgnoreCase))
        locationMode = new Microsoft.Azure.Storage.RetryPolicies.LocationMode?(Microsoft.Azure.Storage.RetryPolicies.LocationMode.SecondaryOnly);
      else if (string.Equals(locationModeString, "PrimaryThenSecondary", StringComparison.OrdinalIgnoreCase))
        locationMode = new Microsoft.Azure.Storage.RetryPolicies.LocationMode?(Microsoft.Azure.Storage.RetryPolicies.LocationMode.PrimaryThenSecondary);
      else if (string.Equals(locationModeString, "SecondaryThenPrimary", StringComparison.OrdinalIgnoreCase))
        locationMode = new Microsoft.Azure.Storage.RetryPolicies.LocationMode?(Microsoft.Azure.Storage.RetryPolicies.LocationMode.SecondaryThenPrimary);
      else
        locationMode = new Microsoft.Azure.Storage.RetryPolicies.LocationMode?();
      return locationMode.HasValue;
    }

    public static Microsoft.Azure.Cosmos.Table.LocationMode? ParseTableLocationMode(
      string locationModeString)
    {
      Microsoft.Azure.Cosmos.Table.LocationMode? locationMode = new Microsoft.Azure.Cosmos.Table.LocationMode?();
      StorageAccountUtilities.TryParseTableLocationMode(locationModeString, out locationMode);
      return locationMode;
    }

    internal static bool TryParseTableLocationMode(
      string locationModeString,
      out Microsoft.Azure.Cosmos.Table.LocationMode? locationMode)
    {
      if (string.Equals(locationModeString, "PrimaryOnly", StringComparison.OrdinalIgnoreCase))
        locationMode = new Microsoft.Azure.Cosmos.Table.LocationMode?(Microsoft.Azure.Cosmos.Table.LocationMode.PrimaryOnly);
      else if (string.Equals(locationModeString, "SecondaryOnly", StringComparison.OrdinalIgnoreCase))
        locationMode = new Microsoft.Azure.Cosmos.Table.LocationMode?(Microsoft.Azure.Cosmos.Table.LocationMode.SecondaryOnly);
      else if (string.Equals(locationModeString, "PrimaryThenSecondary", StringComparison.OrdinalIgnoreCase))
        locationMode = new Microsoft.Azure.Cosmos.Table.LocationMode?(Microsoft.Azure.Cosmos.Table.LocationMode.PrimaryThenSecondary);
      else if (string.Equals(locationModeString, "SecondaryThenPrimary", StringComparison.OrdinalIgnoreCase))
        locationMode = new Microsoft.Azure.Cosmos.Table.LocationMode?(Microsoft.Azure.Cosmos.Table.LocationMode.SecondaryThenPrimary);
      else
        locationMode = new Microsoft.Azure.Cosmos.Table.LocationMode?();
      return locationMode.HasValue;
    }

    public static string GetDiagnosticsConnectionString(IVssRequestContext context)
    {
      ITeamFoundationStrongBoxService service = context.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(context, FrameworkServerConstants.ConfigurationSecretsDrawerName, true);
      StrongBoxItemInfo itemInfo = service.GetItemInfo(context, drawerId, "DiagnosticsConnectionString", false);
      return service.GetString(context, itemInfo);
    }

    public static bool TryUploadBlobToDiagnosticStorageAccount(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer,
      IEnumerable<string> uploadPayload,
      string blobName,
      string containerName = "proddumps")
    {
      AzureBlobClient azureBlobClient = new AzureBlobClient(StorageAccountUtilities.GetDiagnosticsConnectionString(requestContext.To(TeamFoundationHostType.Deployment).Elevate()), (ITFLogger) null);
      byte[] array = uploadPayload.SelectMany<string, byte>((Func<string, IEnumerable<byte>>) (s => (IEnumerable<byte>) Encoding.ASCII.GetBytes(s + "\n"))).ToArray<byte>();
      int num = 10;
      bool flag1 = false;
      TimeSpan timeSpan = TimeSpan.FromMilliseconds(1000.0);
      bool flag2;
      do
      {
        try
        {
          if (!flag1)
          {
            azureBlobClient.CreateContainerIfNotExist(containerName, BlobContainerPublicAccessType.Off);
            flag1 = true;
            tracer.TraceAlways("Container: " + containerName + " was created or it already exists.");
          }
          using (Stream stream = (Stream) new MemoryStream(array))
          {
            azureBlobClient.UploadBlob(containerName, blobName, stream);
            tracer.TraceAlways(string.Format("Uploaded blob: {0} into the container: {1} for the hostId: {2}", (object) blobName, (object) containerName, (object) requestContext.ServiceHost.InstanceId));
          }
          return true;
        }
        catch (Exception ex)
        {
          flag2 = --num > 0 && !requestContext.CancellationToken.IsCancellationRequested && AsyncHttpRetryHelper.IsTransientException(ex, requestContext.CancellationToken);
          if (!flag2)
          {
            tracer.TraceException(ex);
          }
          else
          {
            Thread.Sleep(timeSpan);
            timeSpan = timeSpan.Add(timeSpan);
          }
        }
      }
      while (flag2);
      return false;
    }
  }
}
