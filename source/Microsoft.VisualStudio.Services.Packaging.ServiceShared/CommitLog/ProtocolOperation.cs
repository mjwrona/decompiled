// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.ProtocolOperation
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog
{
  public class ProtocolOperation : IEquatable<ProtocolOperation>
  {
    public ProtocolOperation(IProtocol protocol, string operationName, string version)
      : this(protocol.ToString(), operationName, version)
    {
    }

    public ProtocolOperation(string protocol, string operationName, string version)
    {
      this.Protocol = protocol;
      this.OperationName = operationName;
      this.Version = version;
    }

    public ProtocolOperation(string compressedForm)
    {
      string[] strArray = compressedForm.Split('/');
      this.Protocol = strArray.Length == 3 ? strArray[0] : throw new ArgumentException(Resources.Error_InvalidProtocolOperation((object) nameof (compressedForm)));
      this.OperationName = strArray[1];
      this.Version = strArray[2];
    }

    public string Protocol { get; }

    public string OperationName { get; }

    public string Version { get; }

    public static ProtocolOperation ParseFromCompressedForm(string compressedForm)
    {
      string[] strArray = compressedForm.Split('/');
      return strArray.Length == 3 ? new ProtocolOperation(strArray[0], strArray[1], strArray[2]) : throw new InvalidOperationException(Resources.Error_InvalidProtocolOperation((object) nameof (compressedForm)));
    }

    public override string ToString() => this.Protocol + "/" + this.OperationName + "/" + this.Version;

    public bool Equals(ProtocolOperation other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return string.Equals(this.Protocol, other.Protocol) && string.Equals(this.OperationName, other.OperationName) && string.Equals(this.Version, other.Version);
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((ProtocolOperation) obj);
    }

    public override int GetHashCode()
    {
      string protocol = this.Protocol;
      int num1 = (protocol != null ? protocol.GetHashCode() : 0) * 397;
      string operationName = this.OperationName;
      int hashCode1 = operationName != null ? operationName.GetHashCode() : 0;
      int num2 = (num1 ^ hashCode1) * 397;
      string version = this.Version;
      int hashCode2 = version != null ? version.GetHashCode() : 0;
      return num2 ^ hashCode2;
    }
  }
}
