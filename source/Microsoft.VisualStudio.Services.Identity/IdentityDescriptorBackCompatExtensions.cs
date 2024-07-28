// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityDescriptorBackCompatExtensions
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class IdentityDescriptorBackCompatExtensions
  {
    internal static IdentityDescriptor ToDescriptor(this IdentityDescriptor_BackCompat backCompat) => backCompat != null ? new IdentityDescriptor(backCompat.IdentityType, backCompat.Identifier) : (IdentityDescriptor) null;

    internal static IdentityDescriptor_BackCompat ToBackCompatDescriptor(
      this IdentityDescriptor descriptor)
    {
      return !(descriptor == (IdentityDescriptor) null) ? new IdentityDescriptor_BackCompat(descriptor.IdentityType, descriptor.Identifier) : (IdentityDescriptor_BackCompat) null;
    }

    internal static ICollection<IdentityDescriptor> ToDescriptorCollection(
      this ICollection<IdentityDescriptor_BackCompat> backCompat)
    {
      if (backCompat == null)
        return (ICollection<IdentityDescriptor>) null;
      ICollection<IdentityDescriptor> descriptorCollection = (ICollection<IdentityDescriptor>) new List<IdentityDescriptor>(backCompat.Count);
      foreach (IdentityDescriptor_BackCompat backCompat1 in (IEnumerable<IdentityDescriptor_BackCompat>) backCompat)
        descriptorCollection.Add(backCompat1.ToDescriptor());
      return descriptorCollection;
    }

    internal static ICollection<IdentityDescriptor_BackCompat> ToBackCompatDescriptorCollection(
      this ICollection<IdentityDescriptor> collection)
    {
      if (collection == null)
        return (ICollection<IdentityDescriptor_BackCompat>) null;
      ICollection<IdentityDescriptor_BackCompat> descriptorCollection = (ICollection<IdentityDescriptor_BackCompat>) new List<IdentityDescriptor_BackCompat>(collection.Count);
      foreach (IdentityDescriptor descriptor in (IEnumerable<IdentityDescriptor>) collection)
        descriptorCollection.Add(descriptor.ToBackCompatDescriptor());
      return descriptorCollection;
    }
  }
}
