// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.AnalyticsODataDefaults
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.OData;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public static class AnalyticsODataDefaults
  {
    public static readonly DefaultQuerySettings DefaultQuerySettings = new DefaultQuerySettings()
    {
      EnableSelect = true,
      EnableExpand = true,
      EnableCount = true,
      EnableFilter = true,
      EnableOrderBy = true,
      EnableSkipToken = true,
      MaxTop = new int?(int.MaxValue)
    };
    public static readonly ODataValidationSettings DefaultValidationSettings = new ODataValidationSettings()
    {
      MaxTop = new int?(int.MaxValue),
      MaxExpansionDepth = 3,
      AllowedQueryOptions = AllowedQueryOptions.Supported,
      MaxNodeCount = int.MaxValue
    };
    public static readonly ODataMessageWriterSettings DefaultODataMessageWriterSettings = new ODataMessageWriterSettings()
    {
      EnableMessageStreamDisposal = false,
      Validations = ValidationKinds.None,
      MessageQuotas = new ODataMessageQuotas()
      {
        MaxReceivedMessageSize = long.MaxValue
      }
    };
  }
}
