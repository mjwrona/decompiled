// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.WebApi.PolicyWebApiConstants
// Assembly: Microsoft.TeamFoundation.Policy.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E2CB80F-05BD-43A4-BD5A-A4654EDC6268
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Policy.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Policy.WebApi
{
  [GenerateAllConstants(null)]
  public class PolicyWebApiConstants
  {
    public const string AreaId = "FB13A388-40DD-4A04-B530-013A739C72EF";
    public const string AreaName = "policy";
    public const string ConfigurationResource = "configurations";
    public const string EvaluationsResource = "evaluations";
    public const string TypesResource = "types";
    public const string RevisionsResource = "revisions";
    public const string PolicyTypesLocationIdString = "44096322-2D3D-466A-BB30-D1B7DE69F61F";
    public static readonly Guid PolicyTypesLocationId = new Guid("44096322-2D3D-466A-BB30-D1B7DE69F61F");
    public const string PolicyConfigurationsLocationIdString = "DAD91CBE-D183-45F8-9C6E-9C1164472121";
    public static readonly Guid PolicyConfigurationsLocationId = new Guid("DAD91CBE-D183-45F8-9C6E-9C1164472121");
    public const string PolicyConfigurationRevisionsLocationIdString = "FE1E68A2-60D3-43CB-855B-85E41AE97C95";
    public static readonly Guid PolicyConfigurationRevisionsLocationId = new Guid("FE1E68A2-60D3-43CB-855B-85E41AE97C95");
    public const string PolicyEvaluationsByArtifactLocationIdString = "C23DDFF5-229C-4D04-A80B-0FDCE9F360C8";
    public static readonly Guid PolicyEvaluationsByArtifactLocationId = new Guid("C23DDFF5-229C-4D04-A80B-0FDCE9F360C8");
    public const string PolicyEvaluationLocationIdString = "46AECB7A-5D2C-4647-897B-0209505A9FE4";
    public static readonly Guid PolicyEvaluationLocationId = new Guid("46AECB7A-5D2C-4647-897B-0209505A9FE4");
  }
}
