// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Logging.ConsoleLogger
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;

namespace Microsoft.TeamFoundation.Common.Logging
{
  public class ConsoleLogger : TFLogger
  {
    public override void Error(string message) => Console.WriteLine("Error : " + message);

    public override void Info(string message) => Console.WriteLine("Info : " + message);

    public override void Warning(string message) => Console.WriteLine("Warning : " + message);
  }
}
