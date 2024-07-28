// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.MarshalByRefLogger
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;

namespace Microsoft.TeamFoundation.Common
{
  public class MarshalByRefLogger : TFLogger
  {
    private readonly ITFLogger m_logger;

    public MarshalByRefLogger(ITFLogger logger) => this.m_logger = logger != null ? logger : throw new ArgumentNullException(nameof (logger));

    public override void Error(string message) => this.m_logger.Error(message);

    public override void Info(string message) => this.m_logger.Info(message);

    public override void Warning(string message) => this.m_logger.Warning(message);
  }
}
