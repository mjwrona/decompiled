// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyConfigurationNotFoundException
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Policy.Server
{
  [Serializable]
  public class PolicyConfigurationNotFoundException : TeamFoundationServiceException
  {
    public PolicyConfigurationNotFoundException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractInt(sqlError, "configurationId"), PolicyConfigurationNotFoundException.VersionIdFromSql(TeamFoundationServiceException.ExtractInt(sqlError, "versionId")))
    {
    }

    private static int? VersionIdFromSql(int versionId) => versionId < 0 ? new int?() : new int?(versionId);

    public PolicyConfigurationNotFoundException(int configurationId, int? versionId = null)
    {
      string message;
      if (!versionId.HasValue)
        message = PolicyResources.Format("PolicyConfigurationNotFound", (object) configurationId);
      else
        message = PolicyResources.Format("PolicyConfigurationNotFoundWithVersion", (object) configurationId, (object) versionId);
      // ISSUE: explicit constructor call
      base.\u002Ector(message);
    }
  }
}
