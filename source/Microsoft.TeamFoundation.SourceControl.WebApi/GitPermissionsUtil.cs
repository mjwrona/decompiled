// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitPermissionsUtil
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  public static class GitPermissionsUtil
  {
    public static GitPermissionScope GetTokenScope(string securityToken)
    {
      ArgumentUtility.CheckForNull<string>(securityToken, nameof (securityToken));
      int length = securityToken.Length;
      if (length <= 44)
        return GitPermissionScope.Project;
      if (length <= 81)
        return GitPermissionScope.Repository;
      if (length <= 86)
        return GitPermissionScope.NonBranchRef;
      bool flag = length == 91 || length == 92;
      for (int index = 86; index < length; ++index)
      {
        if (flag)
          flag = (int) securityToken[index] == (int) "heads/"[index - 86];
        if (securityToken[index] == '/')
        {
          if (index != length - 1)
            return GitPermissionScope.Branch;
          return flag ? GitPermissionScope.BranchesRoot : GitPermissionScope.NonBranchRef;
        }
      }
      return flag ? GitPermissionScope.BranchesRoot : GitPermissionScope.NonBranchRef;
    }

    public static Guid? GetProjectIdFromToken(string securityToken) => GitPermissionsUtil.GetGuidFromToken(securityToken, 1);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Guid? GetRepoIdFromToken(string securityToken) => GitPermissionsUtil.GetGuidFromToken(securityToken, 2);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetRefNameFromToken(string securityToken)
    {
      ArgumentUtility.CheckForNull<string>(securityToken, nameof (securityToken));
      string[] strArray = securityToken.Trim('/').Split('/');
      if (strArray.Length < 4)
        return (string) null;
      StringBuilder stringBuilder = new StringBuilder(strArray[3]);
      for (int index = 4; index < strArray.Length; ++index)
      {
        stringBuilder.Append('/');
        if (index == 4)
        {
          stringBuilder.Append(strArray[index]);
        }
        else
        {
          byte[] parsedObjectId;
          if (!GitUtils.TryGetByteArrayFromString(strArray[index], strArray[index].Length, out parsedObjectId))
            throw new InvalidOperationException("Bug in GitPermissionsUtil: unparseable subtoken");
          stringBuilder.Append(GitUtils.SafeUnicodeLENoBOM.GetString(parsedObjectId, 0, parsedObjectId.Length));
        }
      }
      return stringBuilder.ToString();
    }

    private static Guid? GetGuidFromToken(string securityToken, int index)
    {
      ArgumentUtility.CheckForNull<string>(securityToken, nameof (securityToken));
      string[] strArray = securityToken.Split(new char[1]
      {
        '/'
      }, StringSplitOptions.RemoveEmptyEntries);
      return strArray.Length > index ? new Guid?(new Guid(strArray[index])) : new Guid?();
    }
  }
}
