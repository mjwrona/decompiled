// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.CosmosDBLicensingStore
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DocDB;
using Microsoft.VisualStudio.Services.Licensing.CosmosDataAccess;
using System;

namespace Microsoft.VisualStudio.Services.Licensing.DataAccess
{
  internal class CosmosDBLicensingStore
  {
    private const string s_area = "VisualStudio.Services.LicensingService";
    private const string s_layer = "CosmosDBLicensingStore";

    internal static IExtensionLicensingComponent CreateExtensionLicensingComponent(
      IVssRequestContext requestContext)
    {
      try
      {
        requestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
      }
      catch (UnexpectedHostTypeException ex)
      {
        requestContext.TraceException(1035302, "VisualStudio.Services.LicensingService", nameof (CosmosDBLicensingStore), (Exception) ex);
        throw;
      }
      return (IExtensionLicensingComponent) VssRequestContextExtensions.CreateDocDBComponent<LicensingCosmosComponent>(requestContext);
    }

    internal static ILicensingComponent CreateLicensingComponent(IVssRequestContext requestContext)
    {
      try
      {
        requestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
      }
      catch (UnexpectedHostTypeException ex)
      {
        requestContext.TraceException(1035302, "VisualStudio.Services.LicensingService", nameof (CosmosDBLicensingStore), (Exception) ex);
        throw;
      }
      return (ILicensingComponent) VssRequestContextExtensions.CreateDocDBComponent<LicensingCosmosComponent>(requestContext);
    }
  }
}
