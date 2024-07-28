// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.AttributesQueryContext
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Profile
{
  public class AttributesQueryContext : ICloneable
  {
    public AttributesQueryContext(
      AttributesScope scope,
      DateTimeOffset? modifiedSince = null,
      int? modifiedAfterRevision = null,
      CoreProfileAttributes? coreAttributes = null,
      string containerName = null)
    {
      this.Scope = !scope.HasFlag((Enum) ~(AttributesScope.Core | AttributesScope.Application)) && (scope.HasFlag((Enum) AttributesScope.Application) || scope.HasFlag((Enum) AttributesScope.Core)) ? scope : throw new ArgumentException(string.Format("The scope '{0}' is not supported for this operation.", (object) scope));
      this.ModifiedSince = modifiedSince;
      this.ModifiedAfterRevision = modifiedAfterRevision;
      if (scope.HasFlag((Enum) AttributesScope.Application))
      {
        ProfileArgumentValidation.ValidateApplicationContainerName(containerName);
        this.ContainerName = containerName;
      }
      else
        this.ContainerName = (string) null;
      if (scope.HasFlag((Enum) AttributesScope.Core))
        this.CoreAttributes = new CoreProfileAttributes?((CoreProfileAttributes) ((int) coreAttributes ?? (int) ushort.MaxValue));
      else
        this.CoreAttributes = new CoreProfileAttributes?();
    }

    public AttributesQueryContext(AttributesScope scope, string containerName)
      : this(scope, coreAttributes: new CoreProfileAttributes?(CoreProfileAttributes.All), containerName: containerName)
    {
    }

    public AttributesQueryContext(
      AttributesScope scope,
      DateTimeOffset modifiedSince,
      string containerName = null)
      : this(scope, new DateTimeOffset?(modifiedSince), coreAttributes: new CoreProfileAttributes?(CoreProfileAttributes.All), containerName: containerName)
    {
    }

    public AttributesQueryContext(
      AttributesScope scope,
      int modifiedAfterRevision,
      string containerName = null)
      : this(scope, modifiedAfterRevision: new int?(modifiedAfterRevision), coreAttributes: new CoreProfileAttributes?(CoreProfileAttributes.All), containerName: containerName)
    {
    }

    [DataMember(IsRequired = true)]
    public AttributesScope Scope { get; private set; }

    [DataMember]
    public string ContainerName { get; private set; }

    [DataMember]
    public DateTimeOffset? ModifiedSince { get; private set; }

    [DataMember]
    public int? ModifiedAfterRevision { get; private set; }

    [DataMember]
    public CoreProfileAttributes? CoreAttributes { get; private set; }

    public override bool Equals(object obj) => obj != null && !(this.GetType() != obj.GetType()) && this.Equals(obj as AttributesQueryContext);

    public bool Equals(AttributesQueryContext other)
    {
      if (this.Scope == other.Scope && VssStringComparer.AttributesDescriptor.Equals(this.ContainerName, other.ContainerName))
      {
        DateTimeOffset? modifiedSince1 = this.ModifiedSince;
        DateTimeOffset? modifiedSince2 = other.ModifiedSince;
        if ((modifiedSince1.HasValue == modifiedSince2.HasValue ? (modifiedSince1.HasValue ? (modifiedSince1.GetValueOrDefault() == modifiedSince2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
        {
          int? modifiedAfterRevision1 = this.ModifiedAfterRevision;
          int? modifiedAfterRevision2 = other.ModifiedAfterRevision;
          return modifiedAfterRevision1.GetValueOrDefault() == modifiedAfterRevision2.GetValueOrDefault() & modifiedAfterRevision1.HasValue == modifiedAfterRevision2.HasValue;
        }
      }
      return false;
    }

    public override int GetHashCode() => (((this.Scope.GetHashCode() * 499 ^ (this.ContainerName != null ? this.ContainerName.ToLowerInvariant().GetHashCode() : 0)) * 499 ^ (this.ModifiedSince.HasValue ? this.ModifiedSince.GetHashCode() : 0)) * 499 ^ (this.ModifiedAfterRevision.HasValue ? this.ModifiedAfterRevision.GetHashCode() : 0)) * 499 ^ (this.CoreAttributes.HasValue ? this.CoreAttributes.GetHashCode() : 0);

    public object Clone() => this.MemberwiseClone();
  }
}
