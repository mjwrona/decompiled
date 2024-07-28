// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.FileContentsProviderExtensions
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.IO;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public static class FileContentsProviderExtensions
  {
    public static bool TryGetFileContents(
      this IFileContentsProvider fileContentsProvider,
      IVssRequestContext requestContext,
      string filePath,
      out string contents)
    {
      contents = (string) null;
      try
      {
        contents = fileContentsProvider.GetFileContents(requestContext, filePath);
        return true;
      }
      catch (FileNotFoundException ex)
      {
        return false;
      }
      catch (FileContentsProviderException ex)
      {
        return false;
      }
    }
  }
}
