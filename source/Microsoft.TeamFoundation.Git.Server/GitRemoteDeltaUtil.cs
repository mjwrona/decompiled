// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitRemoteDeltaUtil
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitRemoteDeltaUtil : IDisposable
  {
    private const string c_origin = "origin";
    private const string c_packedRefs = "packed-refs";
    private const string c_whiteSpace = " ";
    private string m_tempDirectory;

    public GitRemoteDeltaUtil() => this.m_tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

    public string ConstructDeltaPack(
      IVssRequestContext requestContext,
      ITfsGitRepository existingRepository,
      Uri originGitUri,
      string originUsername,
      string originPassword)
    {
      this.CleanupDirectory();
      DirectoryInfo directory = Directory.CreateDirectory(this.m_tempDirectory);
      Repository.Init(directory.FullName, true);
      StringBuilder stringBuilder = new StringBuilder();
      foreach (TfsGitRef tfsGitRef in existingRepository.Refs.All())
      {
        stringBuilder.Append((object) tfsGitRef.ObjectId);
        stringBuilder.Append(" ");
        stringBuilder.AppendLine(tfsGitRef.Name);
      }
      using (StreamWriter streamWriter = new StreamWriter(Path.Combine(directory.FullName, "packed-refs"), false, GitEncodingUtil.SafeUtf8NoBom))
        streamWriter.Write(stringBuilder.ToString());
      using (TfsGitOdbBackend backend = new TfsGitOdbBackend(requestContext, directory, GitServerUtils.GetContentDB(existingRepository)))
      {
        using (Repository repository = new Repository(directory.FullName))
        {
          repository.ObjectDatabase.AddBackend((OdbBackend) backend, int.MaxValue);
          FetchOptions fetchOptions = new FetchOptions();
          fetchOptions.CredentialsProvider = (CredentialsHandler) ((url, fromUrl, types) => (Credentials) new UsernamePasswordCredentials()
          {
            Username = originUsername,
            Password = originPassword
          });
          fetchOptions.TagFetchMode = new TagFetchMode?(TagFetchMode.None);
          FetchOptions options = fetchOptions;
          repository.Network.Remotes.Add("origin", Uri.EscapeUriString(originGitUri.ToString()), string.Empty);
          List<string> list = Microsoft.TeamFoundation.Git.Server.Native.RemoteRefUtil.GetHeadRefs(requestContext, originGitUri, originUsername, originPassword).Select<Reference, string>((Func<Reference, string>) (bb => bb.CanonicalName)).ToList<string>();
          try
          {
            GlobalSettings.SetEnableOfsDelta(false);
            Commands.Fetch(repository, "origin", (IEnumerable<string>) list, options, "fetching from remote");
          }
          finally
          {
            GlobalSettings.SetEnableOfsDelta(true);
          }
          return Directory.EnumerateFileSystemEntries(directory.FullName, "*.pack", SearchOption.AllDirectories).FirstOrDefault<string>();
        }
      }
    }

    public void Dispose() => this.CleanupDirectory();

    private void CleanupDirectory()
    {
      if (!Directory.Exists(this.m_tempDirectory))
        return;
      GitRemoteDeltaUtil.DeleteDirectory(this.m_tempDirectory);
    }

    private static void DeleteDirectory(string directory)
    {
      foreach (string enumerateDirectory in Directory.EnumerateDirectories(directory))
        GitRemoteDeltaUtil.DeleteDirectory(enumerateDirectory);
      foreach (string enumerateFile in Directory.EnumerateFiles(directory))
      {
        FileInfo fileInfo = new FileInfo(enumerateFile);
        fileInfo.Attributes = FileAttributes.Normal;
        fileInfo.Delete();
      }
      Directory.Delete(directory);
    }
  }
}
