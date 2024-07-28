// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.HttpFileCollectionBaseExtensions
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class HttpFileCollectionBaseExtensions
  {
    public static List<HttpPostedTcmAttachment> ToHttpPostedAttachments(
      this HttpFileCollectionBase fileCollection)
    {
      List<HttpPostedTcmAttachment> postedAttachments = new List<HttpPostedTcmAttachment>();
      for (int index = 0; index < fileCollection.Count; ++index)
        postedAttachments.Add(fileCollection[index].ToHttpPostedAttachment());
      return postedAttachments;
    }

    public static HttpPostedTcmAttachment ToHttpPostedAttachment(this HttpPostedFileBase file)
    {
      using (MemoryStream destination = new MemoryStream())
      {
        file.InputStream.CopyTo((Stream) destination);
        return new HttpPostedTcmAttachment()
        {
          AttachmentContent = Convert.ToBase64String(destination.ToArray()),
          ContentLength = file.ContentLength,
          ContentType = file.ContentType,
          FileName = file.FileName
        };
      }
    }
  }
}
