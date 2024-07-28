// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Graph.GraphLabelNotFoundException`1
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server.Graph
{
  public class GraphLabelNotFoundException<TLabel> : Exception
  {
    public GraphLabelNotFoundException(TLabel label)
    {
      ref TLabel local1 = ref label;
      string message;
      if ((object) default (TLabel) == null)
      {
        TLabel label1 = local1;
        ref TLabel local2 = ref label1;
        if ((object) label1 == null)
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
