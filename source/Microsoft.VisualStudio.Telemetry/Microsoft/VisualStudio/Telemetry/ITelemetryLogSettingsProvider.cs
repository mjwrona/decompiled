// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.ITelemetryLogSettingsProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry
{
  internal interface ITelemetryLogSettingsProvider
  {
    IEnumerable<KeyValuePair<string, string>> MainIdentifiers { get; set; }

    string Path { get; set; }

    string Folder { get; set; }

    string FilePath { get; }

    string GetCreateFolderPath();

    int GetNextUniqueId();
  }
}
