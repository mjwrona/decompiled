// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoveragePaths
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System.IO;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public static class CoveragePaths
  {
    private static RandomFileNameGenerator randomFileNameGenerator = new RandomFileNameGenerator();
    public const string CoverageScopesFileName = "coveragescopes";
    public const string PipelineCoverageSummaryFileName = "pipelinecoveragesummary";
    public const string IndexFileName = "index";
    public const string CompressedFileExtension = "zip";
    public const string UncompressedFileExtension = "json";
    public const string CompressedFolderName = "compressed";
    public const string UncompressedFolderName = "uncompressed";

    public static string GetCoverageScopesFilePath(PipelineCoverageDataType pipelineCoverageDataType)
    {
      string str = CoveragePaths.MaskInvalidChars("coveragescopes_" + CoveragePaths.randomFileNameGenerator.GetRandomFileNameWithoutExtension() + ".json");
      return ((pipelineCoverageDataType == PipelineCoverageDataType.Intermediate ? "coveragetemp" : "coverage") + "\\" + str).ToLower();
    }

    public static string GetScopeLevelFileCoverageDetailsIndexFilePath(
      string coverageScopeName,
      PipelineCoverageDataType pipelineCoverageDataType,
      CoverageDetailsFileType coverageDetailsFileType)
    {
      CoveragePaths.randomFileNameGenerator.GetRandomFileNameWithoutExtension();
      string str = CoveragePaths.MaskInvalidChars(coverageScopeName + "_index");
      return ((pipelineCoverageDataType == PipelineCoverageDataType.Intermediate ? "coveragetemp\\scopelevelfilecoveragedetails" : "coverage\\scopelevelfilecoveragedetails") + "\\" + (coverageDetailsFileType == CoverageDetailsFileType.Compressed ? "compressed" : "uncompressed") + "\\index\\" + str + ".json").ToLower();
    }

    public static string GetCoverageScopesFilePathPrefix(
      PipelineCoverageDataType pipelineCoverageDataType)
    {
      string str = "coveragescopes_";
      return ((pipelineCoverageDataType == PipelineCoverageDataType.Intermediate ? "coveragetemp" : "coverage") + "\\" + str).ToLower();
    }

    public static string GetScopeLevelFileCoverageDetailsFilePath(
      string coverageScopeName,
      PipelineCoverageDataType pipelineCoverageDataType,
      CoverageDetailsFileType coverageDetailsFileType,
      string id = null)
    {
      string withoutExtension = CoveragePaths.randomFileNameGenerator.GetRandomFileNameWithoutExtension();
      string str1 = CoveragePaths.MaskInvalidChars(string.IsNullOrWhiteSpace(id) ? coverageScopeName + "_" + withoutExtension : coverageScopeName + "_" + id);
      string str2 = coverageDetailsFileType == CoverageDetailsFileType.Compressed ? "zip" : "json";
      return ((pipelineCoverageDataType == PipelineCoverageDataType.Intermediate ? "coveragetemp\\scopelevelfilecoveragedetails" : "coverage\\scopelevelfilecoveragedetails") + "\\" + (coverageDetailsFileType == CoverageDetailsFileType.Compressed ? "compressed" : "uncompressed") + "\\" + str1 + "." + str2).ToLower();
    }

    public static string GetScopeLevelFileCoverageDetailsFilePathPrefix(
      string coverageScopeName,
      PipelineCoverageDataType pipelineCoverageDataType,
      CoverageDetailsFileType coverageDetailsFileType)
    {
      return Path.Combine(Path.GetDirectoryName(CoveragePaths.GetScopeLevelFileCoverageDetailsFilePath(coverageScopeName, pipelineCoverageDataType, coverageDetailsFileType)), CoveragePaths.MaskInvalidChars(coverageScopeName)).ToLower();
    }

    public static string GetPipelineCoverageSummaryFilePath(string coverageScopeName) => ("coverage\\" + CoveragePaths.MaskInvalidChars(coverageScopeName + "_pipelinecoveragesummary.json")).ToLower();

    public static string GetPipelineCoverageChangeSummaryPrefixPath(string coverageScopeName) => ("coverage\\scopelevelcoveragechanges\\" + CoveragePaths.MaskInvalidChars(coverageScopeName + "_")).ToLower();

    public static string GetFileCoverageChangeSummaryFilePath(string coverageScopeName, string id) => ("coverage\\scopelevelcoveragechanges\\" + CoveragePaths.MaskInvalidChars(coverageScopeName + "_" + id) + ".json").ToLower();

    public static string GetScopeLevelCoverageSummaryListFilePath(
      string coverageScopeName,
      string timestamp)
    {
      return ("coverage\\scopelevelcoveragesummarylist\\" + CoveragePaths.MaskInvalidChars(coverageScopeName + "_" + timestamp) + ".json").ToLower();
    }

    public static string GetScopeLevelCoverageSummaryListFilePathPrefix(string coverageScopeName) => ("coverage\\scopelevelcoveragesummarylist\\" + CoveragePaths.MaskInvalidChars(coverageScopeName + "_")).ToLower();

    public static string GetDirectoryCoverageSummaryFilePath(
      string coverageScopeName,
      string timestamp)
    {
      return "coverage\\scopeleveldirectorycoveragesummary\\compressed\\" + CoveragePaths.MaskInvalidChars(coverageScopeName + "_" + timestamp) + ".zip";
    }

    public static string GetScopeLevelDirectoryCoverageSummaryIndexFilePath(string coverageScopeName) => "coverage\\scopeleveldirectorycoveragesummary\\compressed\\index\\" + CoveragePaths.MaskInvalidChars(coverageScopeName + "_index") + ".json";

    private static string MaskInvalidChars(string fileName) => string.Join("_", fileName.ToLower().Split(Path.GetInvalidFileNameChars()));

    public static class Temp
    {
      public const string FolderPath = "coveragetemp";
      public const string ScopeLevelFileCoverageDetailsFolder = "coveragetemp\\scopelevelfilecoveragedetails";
    }

    public static class Final
    {
      public const string FolderPath = "coverage";
      public const string ScopeLevelFileCoverageDetailsFolder = "coverage\\scopelevelfilecoveragedetails";
      public const string CoverageChangesFolder = "coverage\\scopelevelcoveragechanges";
      public const string CoverageSummaryListFolder = "coverage\\scopelevelcoveragesummarylist";
      public const string DirectoryCoverageSummaryFolder = "coverage\\scopeleveldirectorycoveragesummary";
    }
  }
}
