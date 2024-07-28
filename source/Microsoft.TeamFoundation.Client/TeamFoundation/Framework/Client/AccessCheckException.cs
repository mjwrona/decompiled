// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.AccessCheckException
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Client
{
  [Serializable]
  public class AccessCheckException : TeamFoundationSecurityServiceException
  {
    private IdentityDescriptor m_identityDescriptor;

    public AccessCheckException(string message)
      : base(message)
    {
    }

    public AccessCheckException(string message, Exception ex)
      : base(message, ex)
    {
    }

    protected AccessCheckException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public string IdentityName => this.GetProperty<string>(AccessCheckExceptionProperties.DisplayName);

    public string Token => this.GetProperty<string>(AccessCheckExceptionProperties.Token);

    public int RequiredPermissions => this.GetProperty<int?>(AccessCheckExceptionProperties.RequestedPermissions).GetValueOrDefault();

    public Guid NamespaceId => this.GetProperty<Guid?>(AccessCheckExceptionProperties.NamespaceId).GetValueOrDefault();

    public IdentityDescriptor IdentityDescriptor
    {
      get
      {
        if (this.m_identityDescriptor == null)
        {
          string property1 = this.GetProperty<string>(AccessCheckExceptionProperties.DescriptorIdentifier);
          string property2 = this.GetProperty<string>(AccessCheckExceptionProperties.DescriptorIdentityType);
          if (!string.IsNullOrEmpty(property1) && !string.IsNullOrEmpty(property2))
            this.m_identityDescriptor = new IdentityDescriptor(property2, property1);
        }
        return this.m_identityDescriptor;
      }
    }
  }
}
