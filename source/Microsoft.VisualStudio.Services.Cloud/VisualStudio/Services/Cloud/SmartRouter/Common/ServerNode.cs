// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common.ServerNode
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common
{
  public class ServerNode : IEquatable<ServerNode>
  {
    private int? m_hashCode;
    private string? m_color;

    public ServerNode(string roleName, string roleInstance, string ipAddress)
    {
      this.RoleName = roleName.CheckArgumentIsNotNullOrEmpty(roleName);
      this.RoleInstance = roleInstance.CheckArgumentIsNotNullOrEmpty(roleInstance);
      this.IPAddress = ipAddress.CheckArgumentIsNotNullOrEmpty(ipAddress);
    }

    public string RoleName { get; }

    public string RoleInstance { get; }

    public string IPAddress { get; }

    public string Color
    {
      get
      {
        if (this.m_color == null)
        {
          string[] strArray = this.RoleInstance.Split('_');
          this.m_color = strArray.Length != 0 ? strArray[strArray.Length - 1] : string.Empty;
        }
        return this.m_color;
      }
    }

    public bool IsColorMatch(ServerNode? other) => (object) other != null && string.Equals(this.Color, other.Color, StringComparison.OrdinalIgnoreCase);

    public bool Equals(ServerNode? other)
    {
      if ((object) other == null)
        return false;
      if ((object) this == (object) other)
        return true;
      return this.RoleName.Equals(other.RoleName) && this.RoleInstance.Equals(other.RoleInstance) && this.IPAddress.Equals(other.IPAddress);
    }

    public override bool Equals(object? obj) => this.Equals(obj as ServerNode);

    public static bool operator ==(ServerNode? a, ServerNode? b) => (object) a != null ? a.Equals(b) : (object) b == null;

    public static bool operator !=(ServerNode? a, ServerNode? b) => !(a == b);

    public override int GetHashCode()
    {
      if (!this.m_hashCode.HasValue)
        this.m_hashCode = new int?(this.RoleName.GetHashCode() ^ this.RoleInstance.GetHashCode() ^ this.IPAddress.GetHashCode());
      return this.m_hashCode.GetValueOrDefault();
    }
  }
}
