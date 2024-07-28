// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.CommonProperty.FileBasedKeyValuesLoader
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.LocalLogger;
using Microsoft.VisualStudio.Telemetry.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Telemetry.CommonProperty
{
  internal class FileBasedKeyValuesLoader : IExternalKeyValuesLoader
  {
    internal const string LoadCommonPropsEventName = "VS/TelemetryApi/LoadCommonProps";
    internal const string LoadCommonPropsFaultDescription = "ExceptionLoadingFromFileId";
    internal static readonly string LoadCommonPropsFaultEventName = "VS/TelemetryApi/LoadCommonProps/Fault";

    internal FileBasedKeyValuesLoader()
    {
    }

    public IDictionary<string, object> GetData(TelemetrySession session, string filePath)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      if (File.Exists(filePath))
      {
        try
        {
          using (FileStream fileStream = ReparsePointAware.OpenFile(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
          {
            using (StreamReader streamReader = new StreamReader((Stream) fileStream))
              dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(streamReader.ReadToEnd());
          }
        }
        catch (Exception ex)
        {
          LocalFileLoggerService.Default.Log(LocalLoggerSeverity.Error, "Telemetry", ex.Message);
          session.PostFault(FileBasedKeyValuesLoader.LoadCommonPropsFaultEventName, "ExceptionLoadingFromFileId", ex);
        }
      }
      return (IDictionary<string, object>) dictionary ?? (IDictionary<string, object>) new Dictionary<string, object>(0);
    }
  }
}
