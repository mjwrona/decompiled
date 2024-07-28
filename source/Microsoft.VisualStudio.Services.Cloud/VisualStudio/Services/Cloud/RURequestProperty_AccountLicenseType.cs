// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RURequestProperty_AccountLicenseType
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.DevOps.Licensing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Licensing;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class RURequestProperty_AccountLicenseType : RURequestProperty_Enum<AccountLicenseType>
  {
    public override bool ShouldOutputEntityToTelemetry { get; protected set; }

    public override object GetRequestValue(IVssRequestContext requestContext) => (object) (byte) this.GetCachedAccountLicenseType(requestContext);

    private AccountLicenseType GetCachedAccountLicenseType(IVssRequestContext requestContext)
    {
      AccountLicenseType accountLicenseType1;
      if (requestContext.RootContext.TryGetItem<AccountLicenseType>(RequestContextItemsKeys.RUCompatibleLicense, out accountLicenseType1))
        return accountLicenseType1;
      AccountLicenseType accountLicenseType2 = requestContext.GetRUCompatibleAccountLicenseType();
      requestContext.RootContext.Items[RequestContextItemsKeys.RUCompatibleLicense] = (object) accountLicenseType2;
      return accountLicenseType2;
    }
  }
}
