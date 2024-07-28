// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.SignInResourceVisibility
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  public static class SignInResourceVisibility
  {
    public const string Visibility = "visibility";
    public const string Default = "0";
    public const string Public = "1";

    public static bool IsVisibility(string expectedVisibility, string visibility) => string.Equals(expectedVisibility, visibility, StringComparison.OrdinalIgnoreCase);

    public static StringBuilder AppendVisibility(
      this StringBuilder queryStringBuilder,
      string visibility)
    {
      return string.IsNullOrEmpty(visibility) ? queryStringBuilder : queryStringBuilder.Append("&visibility=" + visibility);
    }
  }
}
