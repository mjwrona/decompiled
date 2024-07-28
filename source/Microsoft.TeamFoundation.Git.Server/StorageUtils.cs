// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.StorageUtils
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class StorageUtils
  {
    public const int SizeOfGuidInBytes = 16;
    public const int MaxBlockBlobPutChunkSize = 4194304;
    public const string PackFileExtension = ".pack";
    public const string IndexFileExtension = ".idx";
    public const string GraphFileExtension = ".graph";
    public const string OdbIsolationBitmapFileExtension = ".odbiso";
    public const string ReachabilityBitmapFileExtension = ".reach";
    public const int DefaultIntervalsBeforeDeletable = 2;
    public const int DefaultDaysBeforeDeletable = 7;
    public const int DefaultIntervalsBeforeDeletingFiles = 3;
    public const int DefaultNumDaysBeforeDeletingFiles = 14;
    public static readonly Guid ClearDeletableFilesNotificationId = new Guid("0D1C8B50-101F-4342-B113-51123A2AF52E");

    public static string GetPackFileName(Sha1Id packId) => packId.ToString() + ".pack";

    public static string GetOdbFileName(Sha1Id fileId, KnownFileType fileType)
    {
      switch (fileType)
      {
        case KnownFileType.RawPackfile:
        case KnownFileType.DerivedPackFile:
          return StorageUtils.GetPackFileName(fileId);
        case KnownFileType.Graph:
          return fileId.ToString() + ".graph";
        case KnownFileType.ReachabilityBitmapCollection:
          return fileId.ToString() + ".reach";
        case KnownFileType.Index:
          return fileId.ToString() + ".idx";
        default:
          throw new NotImplementedException(string.Format("{0}: {1}", (object) nameof (fileType), (object) fileType));
      }
    }

    public static string GetRepoFileName(Guid fileId, KnownFileType fileType)
    {
      if (fileType == KnownFileType.OdbIsolationBitmap)
        return fileId.ToString() + ".odbiso";
      throw new NotImplementedException(string.Format("{0}: {1}", (object) nameof (fileType), (object) fileType));
    }

    public static Sha1Id CreateUniqueId()
    {
      byte[] numArray = new byte[20];
      Array.Copy((Array) Guid.NewGuid().ToByteArray(), (Array) numArray, 16);
      return new Sha1Id(numArray);
    }
  }
}
