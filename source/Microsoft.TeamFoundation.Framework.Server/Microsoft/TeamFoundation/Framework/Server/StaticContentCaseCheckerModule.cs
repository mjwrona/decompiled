// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StaticContentCaseCheckerModule
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.IO;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class StaticContentCaseCheckerModule : IHttpModule
  {
    public void Init(HttpApplication application) => application.BeginRequest += new EventHandler(this.BeginRequest);

    private void BeginRequest(object sender, EventArgs e)
    {
      HttpContext context = ((HttpApplication) sender).Context;
      if (!context.Items.Contains((object) HttpContextConstants.IsStaticContentRequest))
        return;
      context.Response.AddHeader("X-CaseChecked", "true");
      string correctlyCasedFileName = this.GetCorrectlyCasedFileName(context.Request.PhysicalPath);
      if (correctlyCasedFileName == null || correctlyCasedFileName.Length < context.Request.PhysicalApplicationPath.Length)
        return;
      string a = correctlyCasedFileName.Substring(context.Request.PhysicalApplicationPath.Length - 1).Replace('\\', '/');
      string str = context.Request.FilePath;
      string b = context.Request.ServerVariables.Get("UNENCODED_URL");
      string applicationPath = context.Request.ApplicationPath;
      if (applicationPath != "/" && str.StartsWith(applicationPath, StringComparison.InvariantCultureIgnoreCase))
      {
        str = str.Substring(applicationPath.Length);
        if (!string.IsNullOrEmpty(b))
          b = b.Substring(applicationPath.Length);
      }
      if (string.Equals(a, str, StringComparison.InvariantCultureIgnoreCase) && !string.Equals(a, str, StringComparison.InvariantCulture))
        throw new HttpException(404, "Requests for static content must exactly match the casing of the file name and path. Requested path \"" + context.Request.FilePath + "\" does not match actual path \"" + a + "\". Casing must match exactly to support use of case-sensitive CDNs.");
      if (!string.IsNullOrEmpty(b) && !string.Equals(str, b, StringComparison.InvariantCulture))
        throw new HttpException(404, "Requests for static content must exactly match the file name and path and should not rely on IIS filtering. Requested unfiltered path \"" + b + "\" does not match \"" + str + "\". Most likely it contains \"//\" which will not work with CDNs.");
    }

    private string GetCorrectlyCasedFileName(string path)
    {
      FileInfo fileInfo = new FileInfo(path);
      if (!fileInfo.Exists)
        return (string) null;
      FileInfo[] files = this.GetCorrectlyCasedDirectory(fileInfo.Directory).GetFiles(fileInfo.Name);
      return files.Length != 1 ? (string) null : files[0].FullName;
    }

    private DirectoryInfo GetCorrectlyCasedDirectory(DirectoryInfo dir)
    {
      if (dir.Parent == null)
        return dir;
      DirectoryInfo correctlyCasedDirectory = this.GetCorrectlyCasedDirectory(dir.Parent);
      if (correctlyCasedDirectory == null)
        return (DirectoryInfo) null;
      DirectoryInfo[] directories = correctlyCasedDirectory.GetDirectories(dir.Name);
      return directories.Length != 1 ? (DirectoryInfo) null : directories[0];
    }

    public void Dispose()
    {
    }
  }
}
