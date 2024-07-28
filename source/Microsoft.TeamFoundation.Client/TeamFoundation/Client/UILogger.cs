// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.UILogger
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common;

namespace Microsoft.TeamFoundation.Client
{
  public class UILogger : TFLogger
  {
    public override void Error(string message) => UIHost.WriteError(LogCategory.Configuration, message);

    public override void Info(string message) => UIHost.WriteInfo(LogCategory.Configuration, message);

    public override void Warning(string message) => UIHost.WriteWarning(LogCategory.Configuration, message);
  }
}
