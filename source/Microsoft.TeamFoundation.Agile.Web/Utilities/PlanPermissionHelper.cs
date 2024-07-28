// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.PlanPermissionHelper
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using System;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  public class PlanPermissionHelper
  {
    public static string GetToken(Guid projectId, Guid planId)
    {
      string rootToken = PlanSecurityGroupConstants.RootToken;
      Guid guid = projectId;
      string str1 = guid.ToString();
      string str2 = PlanSecurityGroupConstants.PathSeparator.ToString();
      guid = planId;
      string str3 = guid.ToString();
      return rootToken + str1 + str2 + str3;
    }

    public static string GetProjectAdminToken(Guid projectId) => PlanSecurityGroupConstants.RootToken + projectId.ToString();
  }
}
