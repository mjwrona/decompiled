// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.VertexNotFoundException`1
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  public class VertexNotFoundException<TVertex> : Exception
  {
    public VertexNotFoundException(TVertex vertex)
    {
      ref TVertex local1 = ref vertex;
      string message;
      if ((object) default (TVertex) == null)
      {
        TVertex vertex1 = local1;
        ref TVertex local2 = ref vertex1;
        if ((object) vertex1 == null)
        {
          message = (string) null;
          goto label_4;
        }
        else
          local1 = ref local2;
      }
      message = local1.ToString();
label_4:
      // ISSUE: explicit constructor call
      base.\u002Ector(message);
    }
  }
}
