// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DataImport.RegistryHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.DataImport
{
  public static class RegistryHelper
  {
    public static string GetDataImportSpecificKey(Guid importId, string key) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, key, (object) importId.ToString("D"));

    public static string GetDataImportSpecificKey(
      this IServicingContext servicingContext,
      string key)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, key, (object) servicingContext.GetDataImportId());
    }

    public static Guid GetDataImportId(this IServicingContext servicingContext) => new Guid(servicingContext.Tokens[ServicingTokenConstants.DataImportId]);

    public static Guid GetDataImportTargetHostId(
      this IServicingContext servicingContext,
      bool throwIfNotFound = true)
    {
      IVssRequestContext deploymentRequestContext = servicingContext.DeploymentRequestContext;
      Guid importTargetHostId = deploymentRequestContext.GetService<IVssRegistryService>().GetValue<Guid>(deploymentRequestContext, (RegistryQuery) RegistryHelper.GetTargetHostIdKey(servicingContext), Guid.Empty);
      if (throwIfNotFound && importTargetHostId == Guid.Empty)
        throw new ArgumentException("Missing target host id setting in the registry");
      return importTargetHostId;
    }

    public static void SetDataImportTargetHostId(
      this IServicingContext servicingContext,
      Guid hostId)
    {
      IVssRequestContext deploymentRequestContext = servicingContext.DeploymentRequestContext;
      deploymentRequestContext.GetService<IVssRegistryService>().SetValue<Guid>(deploymentRequestContext, RegistryHelper.GetTargetHostIdKey(servicingContext), hostId);
    }

    private static string GetTargetHostIdKey(IServicingContext servicingContext) => servicingContext.GetDataImportSpecificKey("/Configuration/DataImport/{0}/TargetHostId");
  }
}
