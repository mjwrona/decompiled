// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.ChangesetBatchNotFoundException
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using System;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class ChangesetBatchNotFoundException : ArgumentException
  {
    public ChangesetBatchNotFoundException(string message)
      : base(message)
    {
    }
  }
}
