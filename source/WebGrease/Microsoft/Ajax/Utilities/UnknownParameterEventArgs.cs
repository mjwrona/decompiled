// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.UnknownParameterEventArgs
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public class UnknownParameterEventArgs : EventArgs
  {
    public IList<string> Arguments { get; private set; }

    public int Index { get; set; }

    public string SwitchPart { get; set; }

    public string ParameterPart { get; set; }

    public UnknownParameterEventArgs(IList<string> arguments) => this.Arguments = arguments;
  }
}
