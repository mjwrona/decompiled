// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.IgnoredGatesExtension
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class IgnoredGatesExtension
  {
    public static IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate> GetIgnoredGates(
      string ignoredGatesJson)
    {
      return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate>) ServerModelUtility.FromString<List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate>>(ignoredGatesJson);
    }

    public static string ToIgnoredGatesJson(IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate> ignoredGates) => ServerModelUtility.ToString((object) ignoredGates);

    public static IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.IgnoredGate> ToWebApi(
      this IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate> ignoredGates)
    {
      if (ignoredGates == null)
        return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.IgnoredGate>) null;
      List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.IgnoredGate> webApi = new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.IgnoredGate>();
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate ignoredGate1 in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.IgnoredGate>) ignoredGates)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.IgnoredGate ignoredGate2 = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.IgnoredGate()
        {
          Name = ignoredGate1.Name,
          LastModifiedOn = ignoredGate1.LastModifiedOn
        };
        webApi.Add(ignoredGate2);
      }
      return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.IgnoredGate>) webApi;
    }
  }
}
