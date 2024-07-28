// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.AssociateException
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using Microsoft.VisualStudio.Services.ItemStore.Common.Telemetry;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  [Serializable]
  public class AssociateException : Exception
  {
    public AssociateException()
    {
    }

    public AssociateException(string message)
      : base(message)
    {
    }

    public AssociateException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected AssociateException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public ItemUploaderRecord TransferFilesTelemetryRecord { get; set; }
  }
}
