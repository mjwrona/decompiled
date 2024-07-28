// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.FeatureProviderScopes
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

namespace Microsoft.TeamFoundation.Reporting.DataServices
{
  public class FeatureProviderScopes
  {
    public static readonly string ExampleProvider = "ReferenceAdapter";
    internal static readonly string WorkItemQueriesLegacy = "WIT";
    public static readonly string WorkItemQueries = "WorkitemTracking.Queries";
    public static readonly string TestReports = "TestManagement.Reports";
    public static readonly string TestRunSummary = "TestManagement.RunSummary";
    public static readonly string TestAuthoringMetadata = "TestManagement.AuthoringMetadata";

    public static string ShimLegacyLookup(string scope)
    {
      if (scope == FeatureProviderScopes.WorkItemQueriesLegacy)
        scope = FeatureProviderScopes.WorkItemQueries;
      return scope;
    }
  }
}
