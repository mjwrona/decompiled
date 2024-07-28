// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.TestUtilities
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public static class TestUtilities
  {
    public static Microsoft.Azure.Storage.StorageException BuildStorageException(
      HttpStatusCode statusCode,
      string errorCode = null)
    {
      string input = string.Format("<RequestResult><HTTPStatusCode>{0}</HTTPStatusCode><HttpStatusMessage>Accepted</HttpStatusMessage><TargetLocation>Primary</TargetLocation><ServiceRequestID>{1}</ServiceRequestID><ContentMd5 /><ContentCrc64 /><Etag /><RequestDate>{3}</RequestDate><StartTime>{3}</StartTime><EndTime>{3}</EndTime><Error><Code>{2}</Code><Message>0:The table specified does not exist.\nRequestId:{1}\nTime:{3}</Message></Error></RequestResult>", (object) (int) statusCode, (object) Guid.NewGuid(), (object) errorCode, (object) DateTime.UtcNow.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      Microsoft.Azure.Storage.RequestResult res = new Microsoft.Azure.Storage.RequestResult();
      Stream stream = input.ToStream();
      using (XmlReader reader = XmlReader.Create(stream, new XmlReaderSettings()
      {
        Async = true
      }))
        res.ReadXmlAsync(reader).GetAwaiter().GetResult();
      return new Microsoft.Azure.Storage.StorageException(res, (string) null, (Exception) null);
    }

    public static Microsoft.Azure.Cosmos.Table.StorageException BuildTableStorageException(
      HttpStatusCode statusCode,
      string errorCode = null)
    {
      Microsoft.Azure.Cosmos.Table.RequestResult res = new Microsoft.Azure.Cosmos.Table.RequestResult()
      {
        HttpStatusCode = (int) statusCode
      };
      Type type = typeof (Microsoft.Azure.Cosmos.Table.RequestResult);
      type.GetProperty("HttpStatusMessage", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue((object) res, (object) "Accepted");
      type.GetProperty("TargetLocation", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue((object) res, (object) Microsoft.Azure.Cosmos.Table.StorageLocation.Primary);
      type.GetProperty("ServiceRequestID", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue((object) res, (object) Guid.NewGuid().ToString());
      type.GetProperty("RequestDate", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue((object) res, (object) DateTime.UtcNow.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      type.GetProperty("StartTime", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue((object) res, (object) DateTimeOffset.UtcNow);
      type.GetProperty("EndTime", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue((object) res, (object) DateTimeOffset.UtcNow);
      type.GetProperty("ErrorCode", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue((object) res, (object) errorCode);
      return new Microsoft.Azure.Cosmos.Table.StorageException(res, (string) null, (Exception) null);
    }
  }
}
