// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.StatModeExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class StatModeExtensions
  {
    internal static GitPackObjectType ToPackType(this StatMode mode)
    {
      switch (mode & StatMode.S_IFMT)
      {
        case StatMode.S_IFDIR:
          return GitPackObjectType.Tree;
        case StatMode.S_IFREG:
        case StatMode.S_IFLNK:
          return GitPackObjectType.Blob;
        case StatMode.S_IFBLK | StatMode.S_IFREG:
          return GitPackObjectType.Commit;
        default:
          throw new TreeUnknownModeException(Resources.Format("TreeObjectFailedToParseException_UnknownMode", (object) mode));
      }
    }

    public static bool IsSymlink(this StatMode mode) => (mode & StatMode.S_IFMT) == StatMode.S_IFLNK;
  }
}
