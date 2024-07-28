// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.PathHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class PathHelper
  {
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "unescaped is a well recognized acronym")]
    public static string DBPathToServerPath(string path)
    {
      if (string.IsNullOrWhiteSpace(path))
        return (string) null;
      bool removeTrailingSlashIfPresent = path != "\\";
      return DBPath.DatabaseToUserPath(path, removeTrailingSlashIfPresent, false);
    }

    public static string UserToDBPath(string path) => !string.IsNullOrEmpty(path) ? DBPath.UserToDatabasePath(path, true) : (string) null;
  }
}
