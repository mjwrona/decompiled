// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.TemplateFunctionPatterns
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class TemplateFunctionPatterns
  {
    public static readonly Regex FilterByProjects = new Regex("FilterByProjects\\((?<projects>[^)]+)\\)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    public static readonly Regex FilterByProjectTeams = new Regex("FilterByProjectTeams\\((?<project>[^,]+),(?<teams>[^)]+)\\)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    public static readonly Regex FilterByAreaPath = new Regex("FilterByAreaPath\\((?<project>[^,]+),(?<areaid>[^,]+),(?<operator>[^)]+)\\)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    public static readonly Regex FilterByBacklogNames = new Regex("FilterByBacklogNames\\((?<backlogNames>[^)]+)\\)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    public static readonly Regex FilterByWorkItemTypes = new Regex("FilterByWorkItemTypes\\((?<workItemTypes>[^)]+)\\)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    public static readonly Regex DateSKSubtract = new Regex("DateSK.Subtract\\(now\\(\\),duration'P(?<days>[0-9]+)D'\\)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    public static readonly Regex GetAreaPathFromId = new Regex("GetAreaPathFromId\\((?<areaid>[^)]+)\\)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    public static readonly Regex GetIterationPathFromId = new Regex("GetIterationPathFromId\\((?<iterationid>[^)]+)\\)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    public static readonly Regex GetTagNameFromId = new Regex("Tags\\/any\\(x:x\\/TagId ?eq ?(?<tagId>([^)])+)\\)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    public const string CommonSelectProperties = "CommonSelectProperties()";
    public const string CommonExpandProperties = "CommonExpandProperties()";
    public const string FilterByNonHiddenWorkItemTypes = "FilterByNonHiddenWorkItemTypes()";
  }
}
