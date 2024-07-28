// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Plugins.AnalyticsShardInvalidator
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3E9FDCC8-8891-4D47-89A2-C972B6459647
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using Microsoft.VisualStudio.Services.Analytics.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Analytics.Plugins
{
  public class AnalyticsShardInvalidator
  {
    private const string TraceLayer = "AnalyticsInvalidateShard";
    private string m_table;
    private int m_providerShard;
    public const string c_RegistryPathInvalidFieldsRoot = "/Service/Analytics/InvalidFields/";
    public const string c_FieldListWildcard = "*";

    public AnalyticsShardInvalidator(string table, int providerShard)
    {
      this.m_table = table;
      this.m_providerShard = providerShard;
    }

    [Conditional("DEBUG")]
    private void AssertTableName(string table)
    {
      if (!((IEnumerable<string>) StageTableNames.All).Contains<string>(table))
        throw new ArgumentException(AnalyticsSdkPluginResources.TABLE_NOT_FOUND_IN_STAGE_TABLE_NAMES(), nameof (table));
    }

    public bool InvalidateShardIfRegistrySet(IVssRequestContext requestContext)
    {
      string str1 = "/Service/Analytics/InvalidFields/" + this.m_table;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string str2 = service.GetValue(requestContext, (RegistryQuery) str1, (string) null);
      if (str2 == null)
        return false;
      if (string.Compare(str2, "*", StringComparison.OrdinalIgnoreCase) == 0)
        str2 = (string) null;
      this.InvalidateShard(requestContext, false, str2, false);
      service.DeleteEntries(requestContext, str1);
      return true;
    }

    public void InvalidateShard(
      IVssRequestContext requestContext,
      bool disableCurrentStream = false,
      bool keysOnly = false)
    {
      this.InvalidateShard(requestContext, disableCurrentStream, (string) null, keysOnly);
    }

    public static void SetInvalidateShardRegistry(
      IVssRequestContext requestContext,
      string table,
      string[] fields)
    {
      string path = "/Service/Analytics/InvalidFields/" + table;
      requestContext.GetService<IVssRegistryService>().SetValue<string>(requestContext, path, fields != null ? string.Join(",", fields) : "*");
    }

    private void InvalidateShard(
      IVssRequestContext requestContext,
      bool disableCurrentStream,
      string fields,
      bool keysOnly)
    {
      requestContext.Trace(14010009, TraceLevel.Info, "AnalyticsStaging", "AnalyticsInvalidateShard", string.Format("Invalidating table={0}, shard={1}, disableCurrentStream={2} fields={3} keysOnly={4}", (object) this.m_table, (object) this.m_providerShard, (object) disableCurrentStream, (object) (fields ?? "(all)"), (object) keysOnly));
      requestContext.GetClient<AnalyticsHttpClient>().InvalidateShardAsync(new StageShardInvalid()
      {
        InvalidFields = (IList<string>) fields?.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
        DisableCurrentStream = disableCurrentStream,
        KeysOnly = keysOnly
      }, this.m_table, this.m_providerShard, cancellationToken: requestContext.CancellationToken).SyncResult();
    }
  }
}
