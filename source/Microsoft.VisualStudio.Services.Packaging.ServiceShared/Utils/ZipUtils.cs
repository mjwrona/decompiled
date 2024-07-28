// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.ZipUtils
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.IO;
using System.Text;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public static class ZipUtils
  {
    public static void ValidateZipEndOfCentralDirectoryIsInRange(Stream packageStream)
    {
      long position = packageStream.Position;
      try
      {
        using (BinaryReader binaryReader = new BinaryReader(packageStream, Encoding.UTF8, true))
        {
          long num1 = Math.Min(packageStream.Length, 65557L);
          for (long index = 22; index < num1; ++index)
          {
            packageStream.Seek(-index, SeekOrigin.End);
            if (binaryReader.ReadUInt32() == 101010256U)
            {
              packageStream.Seek(16L, SeekOrigin.Current);
              long num2 = index - 22L;
              if ((long) binaryReader.ReadUInt16() == num2)
                return;
            }
          }
          throw new InvalidPackageException(Resources.Error_InvalidPackageCorruptZip());
        }
      }
      finally
      {
        packageStream.Position = position;
      }
    }
  }
}
