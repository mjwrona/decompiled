// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Plugins.NonFatalStreamErrorEventArgs
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3E9FDCC8-8891-4D47-89A2-C972B6459647
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.dll

using System;

namespace Microsoft.TeamFoundation.Analytics.Plugins
{
  public class NonFatalStreamErrorEventArgs : EventArgs
  {
    public NonFatalStreamErrorEventArgs(
      string table,
      int providerShard,
      int stream,
      string errorMessage)
    {
      this.Table = table;
      this.ProviderShard = providerShard;
      this.Stream = stream;
      this.ErrorMessage = errorMessage;
    }

    public string Table { get; private set; }

    public int ProviderShard { get; private set; }

    public int Stream { get; private set; }

    public string ErrorMessage { get; private set; }
  }
}
