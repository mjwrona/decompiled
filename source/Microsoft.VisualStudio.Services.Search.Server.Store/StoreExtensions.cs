// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Store.StoreExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Store, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 88C5F689-5CBE-419A-B234-7228E63B94DF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Store.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Microsoft.VisualStudio.Services.Search.Server.Store
{
  public static class StoreExtensions
  {
    public static int UploadFile(
      this ITeamFoundationFileService fileService,
      IVssRequestContext requestContext,
      MemoryStream ms,
      string fileName)
    {
      ms.Position = 0L;
      byte[] hashValue = MD5Util.CalculateMD5((Stream) ms, true);
      int fileId = 0;
      try
      {
        GenericInvoker.Instance.Invoke<bool>((Func<bool>) (() =>
        {
          fileId = fileService.UploadFile(requestContext, (Stream) ms, hashValue, ms.Length, CompressionType.None, OwnerId.CodeSense, Guid.Empty, fileName);
          return fileId > 0;
        }), requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MaxIndexingRetryCount"), requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/FaultMediumSeverityJobDelayInSec"), new TraceMetaData(1080553, "Indexing Pipeline", "Store"));
      }
      catch (AggregateException ex)
      {
        throw ex.InnerException;
      }
      return fileId;
    }

    public static string ReadFile(
      this ITeamFoundationFileService fileService,
      IVssRequestContext requestContext,
      string fileName)
    {
      Stream stream = fileService.RetrieveNamedFile(requestContext, OwnerId.CodeSense, fileName, false, out byte[] _, out long _, out CompressionType _);
      if (stream == null || !stream.CanRead)
        return string.Empty;
      using (StreamReader streamReader = new StreamReader(stream))
        return streamReader.ReadToEnd();
    }

    public static bool FileExists(
      this ITeamFoundationFileService fileService,
      IVssRequestContext requestContext,
      string fileName)
    {
      return fileService.TryGetFileId(requestContext, OwnerId.CodeSense, fileName, out int _);
    }

    public static void DeleteFile(
      this ITeamFoundationFileService fileService,
      IVssRequestContext requestContext,
      string fileName)
    {
      try
      {
        GenericInvoker.Instance.Invoke<bool>((Func<bool>) (() =>
        {
          fileService.DeleteNamedFiles(requestContext, OwnerId.CodeSense, (IEnumerable<string>) new Collection<string>()
          {
            fileName
          });
          return true;
        }), requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/MaxIndexingRetryCount"), requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/FaultMediumSeverityJobDelayInSec"), new TraceMetaData(1080553, "Indexing Pipeline", "Store"));
      }
      catch (AggregateException ex)
      {
        throw ex.InnerException;
      }
    }
  }
}
