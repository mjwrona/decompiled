// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Admin.Common.AzureResourceMapper
// Assembly: Microsoft.TeamFoundation.Admin.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4DC7473-FE52-49C1-BB5D-1E769BB5001D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Admin.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Admin.Common
{
  public static class AzureResourceMapper
  {
    private static readonly Dictionary<string, string> s_environmentSqlResourceMap = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "AzurePublicCloud",
        AadSupportedResources.AzureSql
      },
      {
        "AzureUSGovernmentCloud",
        AadSupportedResources.AzureGovSql
      },
      {
        "AzureChinaCloud",
        AadSupportedResources.AzureCnSql
      },
      {
        "AzureGermanCloud",
        AadSupportedResources.AzureDeSql
      },
      {
        "USSec",
        AadSupportedResources.AzureUSSecSql
      },
      {
        "USNat",
        AadSupportedResources.AzureUSNatSql
      }
    };

    public static string GetAzureCloudName(IAzureInstanceMetadataProvider provider)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>()
      {
        {
          "api-version",
          "2018-10-01"
        }
      };
      return ComputeMetadata.Parse(provider.GetMetadata("instance/compute", parameters)).AzEnvironment;
    }

    public static string GetSqlResourceForCloud(string cloudName)
    {
      string resourceForCloud;
      if (!AzureResourceMapper.s_environmentSqlResourceMap.TryGetValue(cloudName, out resourceForCloud))
        throw new Exception("Unknown azure environment " + cloudName);
      return resourceForCloud;
    }
  }
}
