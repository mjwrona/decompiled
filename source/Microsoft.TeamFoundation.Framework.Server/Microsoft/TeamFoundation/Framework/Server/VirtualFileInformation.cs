// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VirtualFileInformation
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VirtualFileInformation : FileInformationBase
  {
    private const int c_partitionLength = 3;

    public VirtualFileInformation(Guid repositoryId, string virtualFilename, string fileNamespace)
      : this(repositoryId, virtualFilename, fileNamespace, (byte[]) null)
    {
    }

    public VirtualFileInformation(
      Guid repositoryId,
      string virtualFilename,
      string fileNamespace,
      byte[] hashValue)
      : base(repositoryId, VirtualFileInformation.ComputeRelativeFilePath(virtualFilename, fileNamespace), hashValue)
    {
      this.VirtualFilename = virtualFilename;
      this.FileNamespace = fileNamespace;
      this.ContentType = "application/octet-stream";
    }

    public string VirtualFilename { get; private set; }

    public string FileNamespace { get; private set; }

    protected override string GetRelativePath() => VirtualFileInformation.ComputeRelativeFilePath(this.VirtualFilename, this.FileNamespace, false);

    private static string ComputeRelativeFilePath(
      string virtualFilename,
      string fileNamespace,
      bool validateArguments = true)
    {
      if (validateArguments)
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(virtualFilename, nameof (virtualFilename));
        if (virtualFilename.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
          throw new ArgumentException(FrameworkResources.InvalidCharactersInFilename(), nameof (virtualFilename));
      }
      if (string.IsNullOrWhiteSpace(fileNamespace))
        fileNamespace = "virtual";
      else if (validateArguments)
      {
        if (fileNamespace.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
          throw new ArgumentException(FrameworkResources.InvalidCharactersInFilepath(), nameof (fileNamespace));
        if (Path.IsPathRooted(fileNamespace))
          throw new ArgumentException(FrameworkResources.InvalidRootedFilenameOrPath(), nameof (fileNamespace));
      }
      return Path.Combine(fileNamespace, VirtualFileInformation.PartitionFileName(virtualFilename));
    }

    private static string PartitionFileName(string fileName)
    {
      string withoutExtension = Path.GetFileNameWithoutExtension(fileName);
      string extension = Path.GetExtension(fileName);
      int length1 = withoutExtension.Length;
      int length2 = extension.Length;
      StringBuilder stringBuilder = new StringBuilder(length1 + length1 / 3 + length2 + 3);
      for (int startIndex = 0; startIndex < length1; startIndex += 3)
      {
        if (startIndex > 0)
          stringBuilder.Append(Path.DirectorySeparatorChar);
        stringBuilder.Append(withoutExtension.Substring(startIndex, Math.Min(3, length1 - startIndex)));
      }
      stringBuilder.Append(new string('_', (3 - length1 % 3) % 3 + 1));
      stringBuilder.Append(extension);
      return stringBuilder.ToString();
    }
  }
}
