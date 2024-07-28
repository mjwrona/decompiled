// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.DisabledFile
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class DisabledFile
  {
    public string FilePath { get; set; }

    public RejectionCode DisableReason { get; set; }

    public List<string> Branches { get; set; }

    public override string ToString() => this.FilePath + "|" + this.DisableReason.ToString();

    public void FromString(string disabledFile)
    {
      string[] strArray = disabledFile != null ? disabledFile.Split(new string[1]
      {
        "|"
      }, StringSplitOptions.RemoveEmptyEntries) : throw new ArgumentNullException(nameof (disabledFile));
      this.FilePath = strArray[0];
      this.DisableReason = (RejectionCode) Enum.Parse(typeof (RejectionCode), strArray[1]);
    }
  }
}
