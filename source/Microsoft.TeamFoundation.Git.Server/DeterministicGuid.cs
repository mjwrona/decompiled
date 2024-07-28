// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.DeterministicGuid
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class DeterministicGuid
  {
    public static Guid Compute(string input)
    {
      byte[] hash;
      using (SHA512 shA512 = SHA512.Create())
        hash = shA512.ComputeHash(GitEncodingUtil.SafeUtf8NoBom.GetBytes(input));
      Array.Resize<byte>(ref hash, 16);
      return new Guid(hash);
    }

    public static Guid ComputeFromDeprecatedSha1(string input)
    {
      byte[] hash;
      using (SHA1 shA1 = SHA1.Create())
        hash = shA1.ComputeHash(GitEncodingUtil.SafeUtf8NoBom.GetBytes(input));
      Array.Resize<byte>(ref hash, 16);
      return new Guid(hash);
    }
  }
}
