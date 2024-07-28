// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Graph.GetUserThumbnailRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.IO;

namespace Microsoft.VisualStudio.Services.Aad.Graph
{
  public class GetUserThumbnailRequest : AadGraphClientRequest<GetUserThumbnailResponse>
  {
    public Guid ObjectId { get; set; }

    internal override GetUserThumbnailResponse Execute(
      IVssRequestContext context,
      GraphConnection connection)
    {
      User user1 = new User();
      user1.ObjectId = this.ObjectId.ToString();
      User user2 = user1;
      byte[] numArray = (byte[]) null;
      try
      {
        using (Stream streamProperty = connection.GetStreamProperty((GraphObject) user2, GraphProperty.ThumbnailPhoto, "image/jpeg"))
          numArray = this.GetByteArrayFromStream(streamProperty);
      }
      catch (ObjectNotFoundException ex)
      {
        context.TraceException(48151623, TraceLevel.Warning, "VisualStudio.Services.Aad", "Graph", (Exception) ex);
      }
      return new GetUserThumbnailResponse()
      {
        Thumbnail = numArray
      };
    }

    private byte[] GetByteArrayFromStream(Stream stream)
    {
      if (stream == null || stream.Length < 1L)
        return (byte[]) null;
      byte[] buffer = new byte[65536];
      using (MemoryStream memoryStream = new MemoryStream(1048576))
      {
        int count;
        while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
          memoryStream.Write(buffer, 0, count);
        return memoryStream.ToArray();
      }
    }
  }
}
