// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.ASTableException
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System;
using System.Net;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  [Serializable]
  internal abstract class ASTableException : ApplicationException
  {
    public abstract HttpStatusCode HttpStatusCode { get; }

    public int ErrorIndex { get; set; }

    public ASTableException(string msg, int index)
      : base(msg)
    {
      this.ErrorIndex = index;
    }

    protected ASTableException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.ErrorIndex = (int) info.GetValue("m_errorIndex", typeof (int));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("m_errorIndex", this.ErrorIndex);
    }
  }
}
