// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.ParentNodeUriDoesNotExistException
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using Microsoft.Azure.Boards.CssNodes;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server
{
  [Serializable]
  public class ParentNodeUriDoesNotExistException : CommonStructureSubsystemServiceException
  {
    public ParentNodeUriDoesNotExistException()
    {
    }

    public ParentNodeUriDoesNotExistException(string uri)
      : this(new Uri(uri), (Exception) null)
    {
    }

    public ParentNodeUriDoesNotExistException(Uri uri)
      : this(uri, (Exception) null)
    {
    }

    public ParentNodeUriDoesNotExistException(string uri, Exception innerException)
      : this(new Uri(uri), innerException)
    {
    }

    public ParentNodeUriDoesNotExistException(Uri uri, Exception innerException)
      : base(ServerResources.CSS_PARENT_NODE_DOES_NOT_EXIST((object) uri.ToString()), innerException)
    {
    }

    protected ParentNodeUriDoesNotExistException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
