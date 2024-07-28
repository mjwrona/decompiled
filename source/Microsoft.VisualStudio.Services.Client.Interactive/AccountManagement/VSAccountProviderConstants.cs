// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.VSAccountProviderConstants
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public static class VSAccountProviderConstants
  {
    public static readonly Guid AccountProviderIdentifier = Guid.Parse("552AFC90145A469C8C31943595DB4A66");
    public const string PersonalizationAccountPropertyName = "VisualStudioPersonalizationAccount";
    public const string IsMSAPropertyName = "IsMSA";
    public const string AdalSigninExtraQueryParameters = "site_id=501454&display=popup&nux=1";
  }
}
