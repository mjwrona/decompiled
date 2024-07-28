// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.ILogger
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  public interface ILogger
  {
    void Error(string message);

    void Error(string message, params object[] args);

    void Warning(string message);

    void Warning(string message, params object[] args);

    void Info(string message);

    void Info(string message, params object[] args);
  }
}
