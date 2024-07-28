// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ContentDispositionBuilder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Net.Http.Headers;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class ContentDispositionBuilder
  {
    public static ContentDispositionHeaderValue CreateAttachment(string fileName) => ContentDispositionBuilder.Create("attachment", fileName);

    public static ContentDispositionHeaderValue CreateInline(string fileName) => ContentDispositionBuilder.Create("inline", fileName);

    private static ContentDispositionHeaderValue Create(string dispositionType, string fileName)
    {
      bool flag = true;
      if (!string.IsNullOrEmpty(fileName))
      {
        foreach (char c in fileName)
        {
          if (char.IsHighSurrogate(c))
          {
            flag = false;
            break;
          }
        }
      }
      ContentDispositionHeaderValue dispositionHeaderValue = new ContentDispositionHeaderValue(dispositionType)
      {
        FileName = fileName
      };
      if (flag)
        dispositionHeaderValue.FileNameStar = fileName;
      return dispositionHeaderValue;
    }
  }
}
