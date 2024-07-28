// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.Exceptions.LeaseLostException
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos.ChangeFeed.Exceptions
{
  [Serializable]
  internal class LeaseLostException : Exception
  {
    private static readonly string DefaultMessage = "The lease was lost.";

    public LeaseLostException()
    {
    }

    public LeaseLostException(DocumentServiceLease lease)
      : base(LeaseLostException.DefaultMessage)
    {
      this.Lease = lease;
    }

    public LeaseLostException(string message)
      : base(message)
    {
    }

    public LeaseLostException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public LeaseLostException(DocumentServiceLease lease, bool isGone)
      : base(LeaseLostException.DefaultMessage)
    {
      this.Lease = lease;
      this.IsGone = isGone;
    }

    public LeaseLostException(DocumentServiceLease lease, Exception innerException, bool isGone)
      : base(LeaseLostException.DefaultMessage, innerException)
    {
      this.Lease = lease;
      this.IsGone = isGone;
    }

    protected LeaseLostException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.Lease = (DocumentServiceLease) info.GetValue(nameof (Lease), typeof (DocumentServiceLease));
      this.IsGone = (bool) info.GetValue(nameof (IsGone), typeof (bool));
    }

    public DocumentServiceLease Lease { get; }

    public bool IsGone { get; }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("Lease", (object) this.Lease);
      info.AddValue("IsGone", this.IsGone);
    }
  }
}
