// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingStepPath
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ServicingStepPath
  {
    private static readonly StringComparer s_stringComparer = StringComparer.OrdinalIgnoreCase;

    public ServicingStepPath(string operation, string group, string step)
    {
      this.Operation = operation ?? throw new ArgumentNullException(nameof (operation));
      this.Group = group ?? throw new ArgumentNullException(nameof (group));
      this.Step = step ?? throw new ArgumentNullException(nameof (step));
    }

    public string Operation { get; }

    public string Group { get; }

    public string Step { get; }

    public override bool Equals(object obj) => obj is ServicingStepPath servicingStepPath && ServicingStepPath.s_stringComparer.Equals(this.Operation, servicingStepPath.Operation) && ServicingStepPath.s_stringComparer.Equals(this.Group, servicingStepPath.Group) && ServicingStepPath.s_stringComparer.Equals(this.Step, servicingStepPath.Step);

    public override int GetHashCode() => ((29666369 * -1521134295 + ServicingStepPath.s_stringComparer.GetHashCode(this.Operation)) * -1521134295 + ServicingStepPath.s_stringComparer.GetHashCode(this.Group)) * -1521134295 + ServicingStepPath.s_stringComparer.GetHashCode(this.Step);
  }
}
