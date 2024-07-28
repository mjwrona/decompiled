// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TraceKeywords
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.Text;

namespace Microsoft.TeamFoundation
{
  public class TraceKeywords
  {
    public static readonly string Framework = TraceKeywords.BuildKeyword(nameof (Framework));
    public static readonly string VersionControl = TraceKeywords.BuildKeyword(nameof (VersionControl));
    public static readonly string WorkItemTracking = TraceKeywords.BuildKeyword(nameof (WorkItemTracking));
    public static readonly string Warehouse = TraceKeywords.BuildKeyword("Reports");
    public static readonly string TeamBuild = TraceKeywords.BuildKeyword("Build");
    public static readonly string SharePoint = TraceKeywords.BuildKeyword("Wss");
    public static readonly string TestManagement = TraceKeywords.BuildKeyword(nameof (TestManagement));
    public static readonly string LabManagement = TraceKeywords.BuildKeyword(nameof (LabManagement));
    public static readonly string TSWebAccess = TraceKeywords.BuildKeyword(nameof (TSWebAccess));
    public static readonly string TeamExplorer = TraceKeywords.BuildKeyword(nameof (TeamExplorer));
    public static readonly string TeamNavigator = TraceKeywords.BuildKeyword(nameof (TeamNavigator));
    public static readonly string Discussion = TraceKeywords.BuildKeyword(nameof (Discussion));
    public static readonly string ConnectDialog = TraceKeywords.BuildKeyword(nameof (ConnectDialog));
    public static readonly string API = TraceKeywords.BuildKeyword(nameof (API));
    public static readonly string Authentication = TraceKeywords.BuildKeyword(nameof (Authentication));
    public static readonly string Authorization = TraceKeywords.BuildKeyword(nameof (Authorization));
    public static readonly string Database = TraceKeywords.BuildKeyword(nameof (Database));
    public static readonly string General = TraceKeywords.BuildKeyword(nameof (General));
    public static readonly string BlobStorage = TraceKeywords.BuildKeyword(nameof (BlobStorage));
    public static readonly string Security = TraceKeywords.BuildKeyword(nameof (Security));
    public static readonly string Urls = TraceKeywords.BuildKeyword(nameof (Urls));
    public static readonly string KeywordPrefix = "TFS";
    public static readonly string KeywordDelimiter = ".";

    public static string BuildKeyword(params string[] keywordFragments)
    {
      if (keywordFragments == null || keywordFragments.Length == 0)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string keywordFragment in keywordFragments)
      {
        if (!string.IsNullOrEmpty(keywordFragment))
        {
          if (stringBuilder.Length > 0)
            stringBuilder.Append(TraceKeywords.KeywordDelimiter);
          stringBuilder.Append(keywordFragment);
        }
      }
      return stringBuilder.ToString();
    }
  }
}
