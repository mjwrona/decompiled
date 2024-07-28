// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Native.RemoteRefUtil
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Native
{
  internal class RemoteRefUtil
  {
    public static IEnumerable<Reference> GetHeadRefs(
      IVssRequestContext requestContext,
      Uri gitUri,
      string username,
      string password)
    {
      return RemoteRefUtil.GetRefs(requestContext, gitUri, username, password).Where<Reference>((Func<Reference, bool>) (x => !x.IsRemoteTrackingBranch && !x.CanonicalName.Equals("HEAD") && !x.CanonicalName.EndsWith("^{}") && !x.CanonicalName.StartsWith("refs/pull")));
    }

    public static IEnumerable<Reference> GetRefs(
      IVssRequestContext requestContext,
      Uri gitUri,
      string username,
      string password)
    {
      if (gitUri == (Uri) null)
        throw new ArgumentNullException(nameof (gitUri));
      string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
      Directory.CreateDirectory(path);
      try
      {
        Repository.Init(path);
        string url1 = Uri.EscapeUriString(gitUri.ToString());
        using (Repository repository = new Repository(path))
          return !string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(password) ? repository.Network.ListReferences(url1, (CredentialsHandler) ((url, fromUrl, types) => (Credentials) new UsernamePasswordCredentials()
          {
            Username = username,
            Password = password
          })) : repository.Network.ListReferences(url1);
      }
      finally
      {
        Directory.Delete(path, true);
      }
    }
  }
}
