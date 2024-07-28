// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.DeliveryContent
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class DeliveryContent
  {
    private IFieldContainer m_alteredEvent;
    private bool m_isStable;
    private int? m_hashCode;

    public IFieldContainer AlteredEvent
    {
      get => this.m_alteredEvent;
      set
      {
        this.ThrowIfStable();
        this.m_alteredEvent = value;
      }
    }

    internal bool IsStable
    {
      get => this.m_isStable;
      set
      {
        if (!value)
          throw new ArgumentException("value must be true!");
        this.m_isStable = true;
      }
    }

    private void ThrowIfStable()
    {
      if (this.m_isStable)
        throw new ArgumentException("DeliveryContent cannot be changed once stable");
    }

    public override bool Equals(object obj)
    {
      bool flag = base.Equals(obj);
      DeliveryContent deliveryContent = obj as DeliveryContent;
      if (!flag && deliveryContent != null && object.Equals((object) this.AlteredEvent, (object) deliveryContent.AlteredEvent))
        flag = true;
      return flag;
    }

    public override int GetHashCode()
    {
      if (!this.m_hashCode.HasValue)
        this.m_hashCode = new int?(this.AlteredEvent.SafeGetHashCode<IFieldContainer>());
      return this.m_hashCode.Value;
    }
  }
}
