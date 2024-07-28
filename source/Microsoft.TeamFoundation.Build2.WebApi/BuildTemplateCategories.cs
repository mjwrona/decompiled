// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildTemplateCategories
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [GenerateAllConstants(null)]
  public static class BuildTemplateCategories
  {
    public static readonly string All = nameof (All);
    public static readonly string Build = nameof (Build);
    public static readonly string Utility = nameof (Utility);
    public static readonly string Test = nameof (Test);
    public static readonly string Package = nameof (Package);
    public static readonly string Deploy = nameof (Deploy);
    public static readonly string Tool = nameof (Tool);
    public static readonly string Custom = nameof (Custom);
    public static readonly string[] AllCategories = new string[8]
    {
      BuildTemplateCategories.All,
      BuildTemplateCategories.Build,
      BuildTemplateCategories.Utility,
      BuildTemplateCategories.Test,
      BuildTemplateCategories.Package,
      BuildTemplateCategories.Deploy,
      BuildTemplateCategories.Tool,
      BuildTemplateCategories.Custom
    };
  }
}
