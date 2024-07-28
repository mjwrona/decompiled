// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security.ReleaseManagementSecurityHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Common;
using System;
using System.Globalization;
using System.Text;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security
{
  public static class ReleaseManagementSecurityHelper
  {
    public const string EnvironmentTokenPrefix = "Environment";
    public static readonly char NamespaceSeparator = '/';
    public static readonly int MaxPathNameLength = 248;

    public static string GetToken(Guid projectId) => ReleaseManagementSecurityHelper.GetToken(projectId, (string) null, 0, 0);

    public static string GetToken(Guid projectId, string path) => ReleaseManagementSecurityHelper.GetToken(projectId, path, 0, 0);

    public static string GetToken(Guid projectId, string path, int releaseDefinitionId) => ReleaseManagementSecurityHelper.GetToken(projectId, path, releaseDefinitionId, 0);

    public static string GetToken(
      Guid projectId,
      string path,
      int releaseDefinitionId,
      int environmentId)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) projectId);
      if (!ReleaseManagementSecurityHelper.IsRootPath(path))
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) ReleaseManagementSecurityHelper.NamespaceSeparator, (object) ReleaseManagementSecurityHelper.GetFolderPathSecurityToken(path));
      if (releaseDefinitionId > 0)
      {
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) ReleaseManagementSecurityHelper.NamespaceSeparator, (object) releaseDefinitionId);
        if (environmentId > 0)
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}{3}", (object) ReleaseManagementSecurityHelper.NamespaceSeparator, (object) "Environment", (object) ReleaseManagementSecurityHelper.NamespaceSeparator, (object) environmentId);
      }
      return stringBuilder.ToString();
    }

    public static string GetToken(this ReleaseManagementSecurityInfo securityInfo)
    {
      if (securityInfo == null)
        throw new ArgumentNullException(nameof (securityInfo));
      return ReleaseManagementSecurityHelper.GetToken(securityInfo.ProjectId, securityInfo.Path, securityInfo.ReleaseDefinitionId, securityInfo.EnvironmentId);
    }

    public static string StripEnvironmentFromToken(this string token) => string.IsNullOrEmpty(token) || !token.Contains("Environment") ? token : token.Substring(0, token.IndexOf("Environment", StringComparison.OrdinalIgnoreCase));

    private static bool IsRootPath(string path) => path.IsNullOrEmpty<char>() || path.Equals("\\", StringComparison.OrdinalIgnoreCase) || path.Equals("/", StringComparison.OrdinalIgnoreCase);

    private static string GetFolderPathSecurityToken(string path)
    {
      string[] strArray = path.Split(new char[2]
      {
        '\\',
        '/'
      }, StringSplitOptions.RemoveEmptyEntries);
      for (int index = 0; index < strArray.Length; ++index)
      {
        string error;
        if (!FileSpec.IsLegalNtfsName(strArray[index], ReleaseManagementSecurityHelper.MaxPathNameLength, true, out error))
          throw new InvalidPathException(error);
      }
      return string.Join(ReleaseManagementSecurityHelper.NamespaceSeparator.ToString(), strArray);
    }
  }
}
