// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Models.VsIdeCategoryIdParser
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Models
{
  public class VsIdeCategoryIdParser
  {
    private const int IdCount = 5;

    public static int[] ParseIds(Guid categoryId)
    {
      byte[] byteArray = categoryId.ToByteArray();
      int[] ids = new int[5];
      int num = 0;
      for (int startIndex = 0; startIndex < 10; startIndex += 2)
        ids[num++] = (int) BitConverter.ToInt16(byteArray, startIndex);
      return ids;
    }

    public static Guid ConcatIds(params int[] ids)
    {
      byte[] b = new byte[16];
      for (int index = 0; index < ids.Length; ++index)
        BitConverter.GetBytes(ids[index]).CopyTo((Array) b, index * 2);
      return new Guid(b);
    }

    public static int GetIntegerId(Guid id) => (int) BitConverter.ToInt16(id.ToByteArray(), 0);
  }
}
