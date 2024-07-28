// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.Utility
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.TestImpact.Server.Common.Properties;
using Microsoft.TeamFoundation.TestImpact.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [CLSCompliant(false)]
  public static class Utility
  {
    public static Uri CheckUri(string arg, string name, string serviceName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(arg, name, serviceName);
      try
      {
        return new Uri(arg);
      }
      catch (UriFormatException ex)
      {
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.InvalidArgument, (object) name, (object) arg), name).Expected(serviceName);
      }
    }

    public static bool CheckCodeChanges(Microsoft.TeamFoundation.TestImpact.WebApi.Contracts.CodeChange[] changes) => changes == null || changes.Length == 0;

    public static Exception MapTestExecutionServiceException(TestImpactServiceSqlException ex) => ex is TestImpactObjectNotFoundSqlException ? (Exception) new TestImpactObjectNotFoundException(ex.Message, (Exception) ex) : (Exception) ex;
  }
}
