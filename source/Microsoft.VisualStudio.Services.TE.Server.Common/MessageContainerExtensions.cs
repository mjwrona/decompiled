// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.MessageContainerExtensions
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Test.WebApi;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  internal static class MessageContainerExtensions
  {
    internal static TestExecutionServiceCommand ToTestAgentMessage(this MessageContainer message)
    {
      if (message == null)
        return (TestExecutionServiceCommand) null;
      return new TestExecutionServiceCommand()
      {
        MessageId = message.MessageId,
        MessageType = message.MessageType,
        Body = message.Body
      };
    }
  }
}
