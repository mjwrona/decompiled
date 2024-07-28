// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Routing.MVC.ParameterSubsegment
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server.Routing.MVC
{
  internal sealed class ParameterSubsegment : PathSubsegment
  {
    public ParameterSubsegment(string parameterName)
    {
      if (parameterName.StartsWith("*", StringComparison.Ordinal))
      {
        this.ParameterName = parameterName.Substring(1);
        this.IsCatchAll = true;
      }
      else
        this.ParameterName = parameterName;
    }

    public bool IsCatchAll { get; private set; }

    public string ParameterName { get; private set; }
  }
}
