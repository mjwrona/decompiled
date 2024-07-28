// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataImportHelper
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class DataImportHelper
  {
    public static bool IsDataImportFlow(IVssRequestContext requestContext)
    {
      try
      {
        bool flag;
        if (((!requestContext.IsFeatureEnabled("VisualStudio.LicensingService.ShortCircuitCommerceCheckForDataImport") ? 0 : (requestContext.Items.TryGetValue<bool>("IsDataImportWorkFlow", out flag) ? 1 : 0)) & (flag ? 1 : 0)) != 0)
          return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1030166, "Licensing", nameof (DataImportHelper), ex);
      }
      return false;
    }
  }
}
