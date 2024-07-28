// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.VisualStudioOnlineServiceRight
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  public class VisualStudioOnlineServiceRight : ServiceRightBase
  {
    private static readonly Version s_currentVersion = new Version(1, 0, 0, 0);
    public const string RightName = "VisualStudioOnlineService";

    public static VisualStudioOnlineServiceRight Create(
      VisualStudioOnlineServiceLevel serviceLevel,
      DateTimeOffset expirationDate)
    {
      VisualStudioOnlineServiceRight onlineServiceRight = new VisualStudioOnlineServiceRight();
      onlineServiceRight.ExpirationDate = expirationDate;
      onlineServiceRight.ServiceLevel = serviceLevel;
      return onlineServiceRight;
    }

    private VisualStudioOnlineServiceRight()
    {
    }

    public override Dictionary<string, object> Attributes => (Dictionary<string, object>) null;

    public override string Name => "VisualStudioOnlineService";

    public override Version Version => VisualStudioOnlineServiceRight.s_currentVersion;

    public override bool Equals(object obj) => base.Equals(obj);

    public override int GetHashCode() => base.GetHashCode();

    public override int CompareTo(object obj) => this.CompareTo(obj as VisualStudioOnlineServiceRight);

    public int CompareTo(VisualStudioOnlineServiceRight right)
    {
      int num1 = this.ServiceLevel.CompareTo((object) right.ServiceLevel);
      if (num1 != 0)
        return num1;
      int num2 = DateTimeOffset.Compare(this.ExpirationDate, right.ExpirationDate);
      return num2 != 0 ? num2 : 0;
    }
  }
}
