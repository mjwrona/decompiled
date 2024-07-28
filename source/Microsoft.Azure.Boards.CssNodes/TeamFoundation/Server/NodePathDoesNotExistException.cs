// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.NodePathDoesNotExistException
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using Microsoft.Azure.Boards.CssNodes;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server
{
  [Serializable]
  public class NodePathDoesNotExistException : CommonStructureSubsystemServiceException
  {
    public NodePathDoesNotExistException()
    {
    }

    public NodePathDoesNotExistException(string nodePath)
      : this(nodePath, (Exception) null)
    {
    }

    public NodePathDoesNotExistException(string nodePath, Exception innerException)
      : base(ServerResources.CSS_NODE_DOES_NOT_EXIST_PATH((object) nodePath))
    {
    }

    protected NodePathDoesNotExistException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
