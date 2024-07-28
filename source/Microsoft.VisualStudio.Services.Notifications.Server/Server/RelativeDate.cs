// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.RelativeDate
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class RelativeDate : Date
  {
    private int m_direction;
    private int m_argument;

    public RelativeDate(int direction, int argument)
    {
      this.m_direction = direction;
      this.m_argument = argument;
    }

    public DateTime GetDateTime() => DateTime.Now.AddDays((double) (this.m_direction * this.m_argument));

    public override string ToString() => this.m_argument != 0 ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TODAY {0} {1}", (object) (char) (this.m_direction == -1 ? 45 : 43), (object) this.m_argument) : "TODAY ";
  }
}
