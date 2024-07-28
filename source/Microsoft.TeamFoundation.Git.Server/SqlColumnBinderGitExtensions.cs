// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.SqlColumnBinderGitExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class SqlColumnBinderGitExtensions
  {
    public static Sha1Id GetSha1Id(ref this SqlColumnBinder binder, IDataReader reader) => new Sha1Id(binder.GetBytes(reader, false));

    public static Sha256Id GetSha256Id(ref this SqlColumnBinder binder, IDataReader reader) => new Sha256Id(binder.GetBytes(reader, false));

    public static Sha1Id? GetNullableSha1Id(ref this SqlColumnBinder binder, IDataReader reader)
    {
      byte[] bytes = binder.GetBytes(reader, true);
      return bytes == null || bytes.Length == 0 ? new Sha1Id?() : new Sha1Id?(new Sha1Id(bytes));
    }
  }
}
