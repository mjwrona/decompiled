// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.SASTokenTTL
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public class SASTokenTTL
  {
    private const double DefaultExpiryHours = 4.0;
    private static readonly RegistryQuery ExpiryHoursRegistryPath = (RegistryQuery) "/Configuration/Packaging/SASTokenTTL";
    private IVssDateTimeProvider dateTimeProvider;

    public SASTokenTTL() => this.dateTimeProvider = VssDateTimeProvider.DefaultProvider;

    public SASTokenTTL(IVssDateTimeProvider dateTimeProvider) => this.dateTimeProvider = dateTimeProvider;

    public DateTimeOffset GetExpiry(IVssRequestContext requestContext) => new DateTimeOffset(this.dateTimeProvider.UtcNow.AddHours(requestContext.GetService<IVssRegistryService>().GetValue<double>(requestContext, in SASTokenTTL.ExpiryHoursRegistryPath, true, 4.0)));
  }
}
