// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryEntityExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Directories
{
  public static class DirectoryEntityExtensions
  {
    public static readonly IEnumerable<string> PropertiesForApplyToIdentity = (IEnumerable<string>) new string[1]
    {
      "LocalDescriptor"
    };
    private const string Area = "DirectoryService";
    private const string Layer = "DirectoryEntityExtensions";

    public static bool CanApplyToIdentity(
      this IDirectoryEntityDescriptor entity,
      IVssIdentity identity,
      ITraceRequest tracer)
    {
      tracer.TraceSerializedConditionally(10026041, TraceLevel.Info, "DirectoryService", nameof (DirectoryEntityExtensions), "CanApplyToIdentity received input => {{\"entity\":{0},\"identity\":{1}}}", (object) entity, (object) identity);
      if (entity == null)
      {
        tracer.TraceSerializedConditionally(10026042, TraceLevel.Error, "DirectoryService", nameof (DirectoryEntityExtensions), "CanApplyToIdentity returned false because entity is null => {{\"entity\":{0},\"identity\":{1}}}", (object) entity, (object) identity);
        return false;
      }
      if (identity == null)
      {
        tracer.TraceSerializedConditionally(10026043, TraceLevel.Error, "DirectoryService", nameof (DirectoryEntityExtensions), "CanApplyToIdentity returned false because identity is null => {{\"entity\":{0},\"identity\":{1}}}", (object) entity, (object) identity);
        return false;
      }
      if (entity.LocalId == null)
      {
        tracer.TraceSerializedConditionally(10026044, TraceLevel.Error, "DirectoryService", nameof (DirectoryEntityExtensions), "CanApplyToIdentity returned false because entity local ID is null => {{\"entity\":{0},\"identity\":{1}}}", (object) entity, (object) identity);
        return false;
      }
      Guid result1 = new Guid();
      if (!Guid.TryParse(entity.LocalId, out result1) || result1 != identity.Id)
      {
        tracer.TraceSerializedConditionally(10026045, TraceLevel.Error, "DirectoryService", nameof (DirectoryEntityExtensions), "CanApplyToIdentity returned false because entity local ID does not match identity ID => {{\"entity\":{0},\"identity\":{1}}}", (object) entity, (object) identity);
        return false;
      }
      if (entity.EntityType != "User" && entity.EntityType != "Group" && entity.EntityType != "ServicePrincipal")
      {
        tracer.TraceSerializedConditionally(10026046, TraceLevel.Error, "DirectoryService", nameof (DirectoryEntityExtensions), "CanApplyToIdentity returned false because entity type is invalid => {{\"entity\":{0},\"identity\":{1}}}", (object) entity, (object) identity);
        return false;
      }
      if (entity.EntityType == "Group" != identity.IsContainer)
      {
        tracer.TraceSerializedConditionally(10026047, TraceLevel.Error, "DirectoryService", nameof (DirectoryEntityExtensions), "CanApplyToIdentity returned false because entity type does not match identity type => {{\"entity\":{0},\"identity\":{1}}}", (object) entity, (object) identity);
        return false;
      }
      if (entity.OriginDirectory != "vsd" && entity.OriginDirectory != "aad" && entity.OriginDirectory != "ad" && entity.OriginDirectory != "wmd")
      {
        tracer.TraceSerializedConditionally(10026048, TraceLevel.Error, "DirectoryService", nameof (DirectoryEntityExtensions), "CanApplyToIdentity returned false because entity origin directory is invalid => {{\"entity\":{0},\"identity\":{1}}}", (object) entity, (object) identity);
        return false;
      }
      if ((entity.OriginDirectory == "aad" ? 1 : 0) != (identity.IsContainer ? (identity.GetProperty<string>("SpecialType", string.Empty) == "AzureActiveDirectoryApplicationGroup" ? 1 : 0) : (identity.IsExternalUser ? 1 : 0)))
      {
        tracer.TraceSerializedConditionally(10026049, TraceLevel.Error, "DirectoryService", nameof (DirectoryEntityExtensions), "CanApplyToIdentity returned false because entity origin directory does not match identity origin directory => {{\"entity\":{0},\"identity\":{1}}}", (object) entity, (object) identity);
        return false;
      }
      if (entity.OriginId == null)
      {
        tracer.TraceSerializedConditionally(10026050, TraceLevel.Error, "DirectoryService", nameof (DirectoryEntityExtensions), "CanApplyToIdentity returned false because entity origin ID is null => {{\"entity\":{0},\"identity\":{1}}}", (object) entity, (object) identity);
        return false;
      }
      string property = identity.GetProperty<string>("http://schemas.microsoft.com/identity/claims/objectidentifier", string.Empty);
      if (!string.IsNullOrEmpty(property))
      {
        Guid result2 = new Guid();
        Guid result3 = new Guid();
        if (!Guid.TryParse(entity.OriginId, out result2) || !Guid.TryParse(property, out result3) || result2 != result3)
        {
          tracer.TraceSerializedConditionally(10026051, TraceLevel.Error, "DirectoryService", nameof (DirectoryEntityExtensions), "CanApplyToIdentity returned false because entity origin ID does not match identity origin ID => {{\"entity\":{0},\"identity\":{1}}}", (object) entity, (object) identity);
          return false;
        }
      }
      tracer.TraceSerializedConditionally(10026058, TraceLevel.Info, "DirectoryService", nameof (DirectoryEntityExtensions), "CanApplyToIdentity returned true => {{\"entity\":{0},\"identity\":{1}}}", (object) entity, (object) identity);
      return true;
    }

    public static bool TryApplyToIdentity(
      this IDirectoryEntityDescriptor entity,
      IVssIdentity identity,
      ITraceRequest tracer)
    {
      if (!entity.CanApplyToIdentity(identity, tracer))
        return false;
      entity.ApplyToIdentity(identity, tracer);
      return true;
    }

    private static void ApplyToIdentity(
      this IDirectoryEntityDescriptor entity,
      IVssIdentity identity,
      ITraceRequest tracer)
    {
      tracer.TraceSerializedConditionally(10026061, TraceLevel.Info, "DirectoryService", nameof (DirectoryEntityExtensions), "ApplyToIdentity received input => {{\"entity\":{0},\"identity\":{1}}}", (object) entity, (object) identity);
      entity.UpdateIdentityObjectId(identity, tracer);
      entity.UpdateIdentityAccountName(identity, tracer);
      entity.UpdateIdentityDisplayName(identity, tracer);
      entity.UpdateIdentityDescriptor(identity, tracer);
      tracer.TraceSerializedConditionally(10026078, TraceLevel.Info, "DirectoryService", nameof (DirectoryEntityExtensions), "ApplyToIdentity returned => {{\"entity\":{0},\"identity\":{1}}}", (object) entity, (object) identity);
    }

    private static void UpdateIdentityObjectId(
      this IDirectoryEntityDescriptor entity,
      IVssIdentity identity,
      ITraceRequest tracer)
    {
      if (entity.OriginDirectory != "aad" || entity.EntityType != "User")
        return;
      string property = identity.GetProperty<string>("http://schemas.microsoft.com/identity/claims/objectidentifier", string.Empty);
      if (string.IsNullOrEmpty(property))
      {
        identity.SetProperty("http://schemas.microsoft.com/identity/claims/objectidentifier", (object) entity.OriginId);
        tracer.TraceSerializedConditionally(10026062, TraceLevel.Info, "DirectoryService", nameof (DirectoryEntityExtensions), "ApplyToIdentity updated identity object ID {3} to match entity origin ID {2} => {{\"entity\":{0},\"identity\":{1}}}", (object) entity, (object) identity, (object) entity.OriginId, (object) property);
      }
      else
        tracer.TraceSerializedConditionally(10026063, TraceLevel.Info, "DirectoryService", nameof (DirectoryEntityExtensions), "ApplyToIdentity skipped update of nonempty identity object ID {3} versus entity origin ID {2} => {{\"entity\":{0},\"identity\":{1}}}", (object) entity, (object) identity, (object) entity.OriginId, (object) property);
    }

    private static void UpdateIdentityAccountName(
      this IDirectoryEntityDescriptor entity,
      IVssIdentity identity,
      ITraceRequest tracer)
    {
      if (string.IsNullOrEmpty(entity.PrincipalName))
        return;
      string property = identity.GetProperty<string>("Account", string.Empty);
      if (string.Equals(entity.PrincipalName, property))
      {
        tracer.TraceSerializedConditionally(10026064, TraceLevel.Info, "DirectoryService", nameof (DirectoryEntityExtensions), "ApplyToIdentity skipped update of already matching identity account name {3} and entity principal name {2} => {{\"entity\":{0},\"identity\":{1}}}", (object) entity, (object) identity, (object) entity.PrincipalName, (object) property);
      }
      else
      {
        identity.SetProperty("Account", (object) entity.PrincipalName);
        tracer.TraceSerializedConditionally(10026065, TraceLevel.Info, "DirectoryService", nameof (DirectoryEntityExtensions), "ApplyToIdentity updated identity account name {3} to match entity principal name {2} => {{\"entity\":{0},\"identity\":{1}}}", (object) entity, (object) identity, (object) entity.PrincipalName, (object) property);
      }
    }

    private static void UpdateIdentityDisplayName(
      this IDirectoryEntityDescriptor entity,
      IVssIdentity identity,
      ITraceRequest tracer)
    {
      if (string.IsNullOrEmpty(entity.DisplayName))
        return;
      string providerDisplayName = identity.ProviderDisplayName;
      string customDisplayName = identity.CustomDisplayName;
      if (string.Equals(entity.DisplayName, providerDisplayName) && string.Equals(entity.DisplayName, customDisplayName))
      {
        tracer.TraceSerializedConditionally(10026066, TraceLevel.Info, "DirectoryService", nameof (DirectoryEntityExtensions), "ApplyToIdentity skipped update of already matching identity provider display name {3} and custom display name {4} and entity display name {2} => {{\"entity\":{0},\"identity\":{1}}}", (object) entity, (object) identity, (object) entity.DisplayName, (object) identity.ProviderDisplayName, (object) identity.CustomDisplayName);
      }
      else
      {
        identity.ProviderDisplayName = entity.DisplayName;
        if (identity.CustomDisplayName != null)
          identity.CustomDisplayName = entity.DisplayName;
        tracer.TraceSerializedConditionally(10026067, TraceLevel.Info, "DirectoryService", nameof (DirectoryEntityExtensions), "ApplyToIdentity updated identity provider display name {3} and custom display name {4} to match entity display name {2} => {{\"entity\":{0},\"identity\":{1}}}", (object) entity, (object) identity, (object) entity.DisplayName, (object) providerDisplayName, (object) customDisplayName);
      }
    }

    private static void UpdateIdentityDescriptor(
      this IDirectoryEntityDescriptor entity,
      IVssIdentity identity,
      ITraceRequest tracer)
    {
      string descriptor1 = entity["LocalDescriptor"]?.ToString();
      if (string.IsNullOrEmpty(descriptor1))
        return;
      IdentityDescriptor descriptorFromString = IdentityParser.GetDescriptorFromString(descriptor1);
      IdentityDescriptor descriptor2 = identity.Descriptor;
      if (IdentityDescriptorComparer.Instance.Equals(descriptorFromString, descriptor2))
      {
        tracer.TraceSerializedConditionally(10026068, TraceLevel.Info, "DirectoryService", nameof (DirectoryEntityExtensions), "ApplyToIdentity skipped update of already matching identity descriptor {3} and entity local descriptor {2} => {{\"entity\":{0},\"identity\":{1}}}", (object) entity, (object) identity, (object) descriptorFromString, (object) descriptor2);
      }
      else
      {
        identity.Descriptor = descriptorFromString;
        tracer.TraceSerializedConditionally(10026069, TraceLevel.Info, "DirectoryService", nameof (DirectoryEntityExtensions), "ApplyToIdentity updated identity descriptor {3} to match entity local descriptor {2} => {{\"entity\":{0},\"identity\":{1}}}", (object) entity, (object) identity, (object) descriptorFromString, (object) descriptor2);
      }
    }
  }
}
