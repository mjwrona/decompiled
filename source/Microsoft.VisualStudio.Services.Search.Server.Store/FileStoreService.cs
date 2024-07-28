// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Store.FileStoreService
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Store, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 88C5F689-5CBE-419A-B234-7228E63B94DF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Store.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Services.Search.Server.Store
{
  public class FileStoreService : IFileStoreService
  {
    [StaticSafe]
    private static volatile FileStoreService s_instance;
    [StaticSafe]
    private static object s_syncRoot = new object();
    private const string FileIdFormat = "{0}@{1}@{2}@{3}";
    private const string FileNewNamePrefix = "New@";

    private FileStoreService()
    {
    }

    [StaticSafe]
    public static FileStoreService Instance
    {
      get
      {
        if (FileStoreService.s_instance == null)
        {
          lock (FileStoreService.s_syncRoot)
          {
            if (FileStoreService.s_instance == null)
              FileStoreService.s_instance = new FileStoreService();
          }
        }
        return FileStoreService.s_instance;
      }
    }

    public void AddFile(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string branchName,
      string filePath,
      byte[] fileContent)
    {
      this.ValidateArguments(requestContext, projectName, repositoryName, branchName, filePath);
      if (fileContent == null)
        throw new ArgumentNullException(nameof (fileContent));
      ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
      string fileName = this.GetFileName(projectName, repositoryName, branchName, filePath);
      try
      {
        using (MemoryStream ms = new MemoryStream(fileContent))
          service.UploadFile(requestContext, ms, fileName);
      }
      catch (DuplicateFileNameException ex1)
      {
        int fileId = 0;
        try
        {
          using (MemoryStream ms = new MemoryStream(fileContent))
            fileId = service.UploadFile(requestContext, ms, "New@" + fileName);
        }
        catch (DuplicateFileNameException ex2)
        {
          service.DeleteFile(requestContext, "New@" + fileName);
          using (MemoryStream ms = new MemoryStream(fileContent))
            fileId = service.UploadFile(requestContext, ms, "New@" + fileName);
        }
        service.DeleteFile(requestContext, fileName);
        service.RenameFile(requestContext, (long) fileId, fileName);
      }
    }

    public string GetFile(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string branchName,
      string filePath)
    {
      this.ValidateArguments(requestContext, projectName, repositoryName, branchName, filePath);
      ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
      string fileName = this.GetFileName(projectName, repositoryName, branchName, filePath);
      string empty = string.Empty;
      string file = service.ReadFile(requestContext, fileName);
      if (string.IsNullOrEmpty(file))
      {
        file = service.ReadFile(requestContext, "New@" + fileName);
        if (!string.IsNullOrEmpty(file))
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("GetCustomFileNewContentFound", "Query Pipeline", 1.0);
        else
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("GetCustomFileContentNotFound", "Query Pipeline", 1.0);
      }
      return file;
    }

    public bool FileExists(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string branchName,
      string filePath)
    {
      this.ValidateArguments(requestContext, projectName, repositoryName, branchName, filePath);
      return requestContext.GetService<ITeamFoundationFileService>().FileExists(requestContext, this.GetFileName(projectName, repositoryName, branchName, filePath));
    }

    public void DeleteFile(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string branchName,
      string filePath)
    {
      this.ValidateArguments(requestContext, projectName, repositoryName, branchName, filePath);
      ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
      string fileName = this.GetFileName(projectName, repositoryName, branchName, filePath);
      if (!service.FileExists(requestContext, fileName))
        return;
      service.DeleteFile(requestContext, fileName);
    }

    private string GetFileName(
      string projectName,
      string repositoryName,
      string branchName,
      string filePath)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}@{1}@{2}@{3}", (object) projectName, (object) repositoryName, (object) branchName, (object) filePath);
    }

    private void ValidateArguments(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      string branchName,
      string filePath)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (string.IsNullOrWhiteSpace(projectName))
        throw new ArgumentNullException(nameof (projectName));
      if (string.IsNullOrWhiteSpace(repositoryName))
        throw new ArgumentNullException(nameof (repositoryName));
      if (string.IsNullOrWhiteSpace(branchName))
        throw new ArgumentNullException(nameof (branchName));
      if (string.IsNullOrWhiteSpace(filePath))
        throw new ArgumentNullException(nameof (filePath));
    }
  }
}
