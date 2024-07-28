// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Utils.MaintenanceMessageUtils
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Git.Server.Utils
{
  public static class MaintenanceMessageUtils
  {
    private static Regex c_sanitizeHtmlTagsRegex = new Regex("<.*?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static Regex c_hrefContentRegex = new Regex("href=\"(.*)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private const string c_defaultMessageForMaintenanceRepo = "The repository is currently being maintained and will be available soon. Questions? Contact <a href=\"https://azure.microsoft.com/support/devops/\">Microsoft customer support</a>";

    public static string GetSanitizedMaintenanceMessage(IVssRequestContext context)
    {
      string messageFromRegistry = MaintenanceMessageUtils.GetMaintenanceMessageFromRegistry(context);
      string empty = string.Empty;
      System.Text.RegularExpressions.Match match = MaintenanceMessageUtils.c_hrefContentRegex.Match(messageFromRegistry);
      if (match.Success)
        empty = match.Groups[1].Value;
      string str = MaintenanceMessageUtils.SanitizeMaintenanceMessage(messageFromRegistry);
      return !string.IsNullOrEmpty(empty) ? str + " " + empty : str;
    }

    public static string GetMaintenanceMessage(IVssRequestContext context) => MaintenanceMessageUtils.GetMaintenanceMessageFromRegistry(context);

    private static string GetMaintenanceMessageFromRegistry(IVssRequestContext context) => context.GetService<IVssRegistryService>().GetValue<string>(context, (RegistryQuery) "/Service/Git/Setting/RepoInMaintenanceMessage", "The repository is currently being maintained and will be available soon. Questions? Contact <a href=\"https://azure.microsoft.com/support/devops/\">Microsoft customer support</a>");

    private static string SanitizeMaintenanceMessage(string input) => MaintenanceMessageUtils.c_sanitizeHtmlTagsRegex.Replace(input, string.Empty);
  }
}
