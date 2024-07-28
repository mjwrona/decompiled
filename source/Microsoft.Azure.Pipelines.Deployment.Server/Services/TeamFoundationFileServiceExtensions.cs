// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Services.TeamFoundationFileServiceExtensions
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.IO;

namespace Microsoft.Azure.Pipelines.Deployment.Services
{
  internal static class TeamFoundationFileServiceExtensions
  {
    public static int SaveToFile<T>(
      this ITeamFoundationFileService fileService,
      IVssRequestContext requestContext,
      T content)
    {
      using (Stream streamFromString = TeamFoundationFileServiceExtensions.GenerateStreamFromString(JsonUtility.ToString((object) content)))
      {
        long position = streamFromString.Position;
        byte[] md5 = MD5Util.CalculateMD5(streamFromString, true);
        streamFromString.Position = position;
        return fileService.UploadFile(requestContext, streamFromString, md5, streamFromString.Length, CompressionType.None, OwnerId.Deployment, Guid.Empty, Guid.NewGuid().ToString());
      }
    }

    public static T RetrieveFromFile<T>(
      this ITeamFoundationFileService fileService,
      IVssRequestContext requestContext,
      int fileId)
    {
      string end;
      using (StreamReader streamReader = new StreamReader(fileService.RetrieveFile(requestContext, (long) fileId, false, out byte[] _, out long _, out CompressionType _)))
        end = streamReader.ReadToEnd();
      return JsonUtility.FromString<T>(end);
    }

    private static Stream GenerateStreamFromString(string s)
    {
      MemoryStream streamFromString = new MemoryStream();
      StreamWriter streamWriter = new StreamWriter((Stream) streamFromString);
      streamWriter.Write(s);
      streamWriter.Flush();
      streamFromString.Position = 0L;
      return (Stream) streamFromString;
    }
  }
}
