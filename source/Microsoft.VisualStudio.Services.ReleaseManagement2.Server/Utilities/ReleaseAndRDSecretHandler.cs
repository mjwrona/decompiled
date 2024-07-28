// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.ReleaseAndRDSecretHandler
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public static class ReleaseAndRDSecretHandler
  {
    public static string RetrieveFile(IVssRequestContext requestContext, int fileId) => new StreamReader(requestContext.GetService<ITeamFoundationFileService>().RetrieveFile(requestContext, (long) fileId, false, out byte[] _, out long _, out CompressionType _)).ReadToEnd();

    public static bool CheckForSecretinFileContent(string fileContent, string secretPattern)
    {
      string pattern = "\\b" + secretPattern + "\\b";
      TimeSpan matchTimeout = TimeSpan.FromSeconds(4.0);
      return Regex.Match(fileContent, pattern, RegexOptions.None, matchTimeout).Success;
    }
  }
}
