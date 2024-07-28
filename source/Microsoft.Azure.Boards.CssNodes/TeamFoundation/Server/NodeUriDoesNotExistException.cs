// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.NodeUriDoesNotExistException
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using Microsoft.Azure.Boards.CssNodes;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server
{
  [Serializable]
  public class NodeUriDoesNotExistException : CommonStructureSubsystemServiceException
  {
    public NodeUriDoesNotExistException()
    {
    }

    public NodeUriDoesNotExistException(string uri)
      : this(new Uri(uri), (Exception) null)
    {
    }

    public NodeUriDoesNotExistException(Uri uri)
      : this(uri, (Exception) null)
    {
    }

    public NodeUriDoesNotExistException(string uri, Exception innerException)
      : this(new Uri(uri), innerException)
    {
    }

    public NodeUriDoesNotExistException(Uri uri, Exception innerException)
      : base(ServerResources.CSS_NODE_DOES_NOT_EXIST_URI((object) uri.ToString()), innerException)
    {
    }

    protected NodeUriDoesNotExistException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
