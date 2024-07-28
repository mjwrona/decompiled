// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Extensions.StringExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

namespace Microsoft.VisualStudio.Services.IdentityPicker.Extensions
{
  internal static class StringExtensions
  {
    public static string TrimToLower(this string input) => input.Trim().ToLowerInvariant();
  }
}
