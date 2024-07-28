// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.ProfileExtensions
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Profile
{
  public static class ProfileExtensions
  {
    public static void ApplyAttributesChanges(
      this Microsoft.VisualStudio.Services.Profile.Profile profile,
      IList<ProfileAttribute> changedApplicationAttributes,
      IList<CoreProfileAttribute> changedCoreAttributes)
    {
      ProfileExtensions.ValidateProfile(profile);
      if ((changedApplicationAttributes == null || changedApplicationAttributes.Count == 0) && (changedCoreAttributes == null || changedCoreAttributes.Count == 0))
        return;
      ProfileExtensions.ValidateAttributesApplicability(profile, (IEnumerable<ProfileAttribute>) changedApplicationAttributes, (IEnumerable<CoreProfileAttribute>) changedCoreAttributes);
      if (profile.ApplicationContainer == null && changedApplicationAttributes != null && changedApplicationAttributes.Count > 0)
      {
        string containerName = changedApplicationAttributes.First<ProfileAttribute>().Descriptor.ContainerName;
        profile.ApplicationContainer = new AttributesContainer(containerName);
      }
      ProfileExtensions.UpdateCoreAttributes(profile, (IEnumerable<CoreProfileAttribute>) changedCoreAttributes);
      ProfileExtensions.UpdateApplicationAttributes(profile, (IEnumerable<ProfileAttribute>) changedApplicationAttributes);
    }

    private static void UpdateCoreAttributes(
      Microsoft.VisualStudio.Services.Profile.Profile profile,
      IEnumerable<CoreProfileAttribute> changedAttributes)
    {
      if (changedAttributes == null || !changedAttributes.Any<CoreProfileAttribute>())
        return;
      IDictionary<string, CoreProfileAttribute> coreAttributes = profile.CoreAttributes;
      foreach (CoreProfileAttribute changedAttribute in changedAttributes)
      {
        CoreProfileAttribute profileAttribute;
        if (coreAttributes.TryGetValue(changedAttribute.Descriptor.AttributeName, out profileAttribute))
        {
          if (profileAttribute.Revision < changedAttribute.Revision)
          {
            if (changedAttribute.Value == null)
            {
              coreAttributes.Remove(changedAttribute.Descriptor.AttributeName);
            }
            else
            {
              profileAttribute.Value = changedAttribute.Value;
              profileAttribute.Revision = changedAttribute.Revision;
            }
          }
        }
        else if (changedAttribute.Value != null)
          coreAttributes.Add(changedAttribute.Descriptor.AttributeName, changedAttribute);
        if (changedAttribute.Revision > profile.CoreRevision)
          profile.CoreRevision = changedAttribute.Revision;
        if (changedAttribute.Revision > profile.Revision)
          profile.Revision = changedAttribute.Revision;
      }
    }

    internal static void UpdateApplicationAttributes(
      Microsoft.VisualStudio.Services.Profile.Profile profile,
      IEnumerable<ProfileAttribute> changedAttributes)
    {
      if (changedAttributes == null || !changedAttributes.Any<ProfileAttribute>())
        return;
      IDictionary<string, ProfileAttribute> attributes = profile.ApplicationContainer.Attributes;
      foreach (ProfileAttribute changedAttribute in changedAttributes)
      {
        ProfileAttribute profileAttribute;
        if (attributes.TryGetValue(changedAttribute.Descriptor.AttributeName, out profileAttribute))
        {
          if (profileAttribute.Revision < changedAttribute.Revision)
          {
            if (changedAttribute.Value == null)
            {
              attributes.Remove(changedAttribute.Descriptor.AttributeName);
            }
            else
            {
              profileAttribute.Value = changedAttribute.Value;
              profileAttribute.Revision = changedAttribute.Revision;
            }
          }
        }
        else if (changedAttribute.Value != null)
          attributes.Add(changedAttribute.Descriptor.AttributeName, changedAttribute);
        if (changedAttribute.Revision > profile.ApplicationContainer.Revision)
          profile.ApplicationContainer.Revision = changedAttribute.Revision;
        if (changedAttribute.Revision > profile.Revision)
          profile.Revision = changedAttribute.Revision;
      }
    }

    private static void ValidateAttributesApplicability(
      Microsoft.VisualStudio.Services.Profile.Profile profile,
      IEnumerable<ProfileAttribute> changedAttributes,
      IEnumerable<CoreProfileAttribute> changedCoreAttributes)
    {
      if (changedCoreAttributes != null)
      {
        ProfileUtility.ValidateAttributesMetadata<object>((IEnumerable<ProfileAttributeBase<object>>) changedCoreAttributes);
        ProfileUtility.ValidateAttributesBelongToCoreContainer(changedCoreAttributes);
      }
      if (changedAttributes == null)
        return;
      ProfileUtility.ValidateAttributesMetadata<string>((IEnumerable<ProfileAttributeBase<string>>) changedAttributes);
      string containerName = profile.ApplicationContainer != null ? profile.ApplicationContainer.ContainerName : (string) null;
      ProfileUtility.ValidateAttributesBelongToOneApplicaitonContainer(changedAttributes, containerName);
    }

    private static void ValidateProfile(Microsoft.VisualStudio.Services.Profile.Profile profile)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Profile.Profile>(profile, nameof (profile));
      if (profile.ApplicationContainer == null)
        return;
      using (IEnumerator<KeyValuePair<string, CoreProfileAttribute>> enumerator = profile.CoreAttributes.Where<KeyValuePair<string, CoreProfileAttribute>>((Func<KeyValuePair<string, CoreProfileAttribute>, bool>) (kv => kv.Value.Revision > profile.Revision)).GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          KeyValuePair<string, CoreProfileAttribute> current = enumerator.Current;
          throw new ArgumentException(string.Format("Current profile object is in a bad state. The revision of attribute '{0}': '{1}' is greater than the revision of the profile object: {2}", (object) current.Key, (object) current.Value.Revision, (object) profile.Revision));
        }
      }
      using (IEnumerator<KeyValuePair<string, ProfileAttribute>> enumerator = profile.ApplicationContainer.Attributes.Where<KeyValuePair<string, ProfileAttribute>>((Func<KeyValuePair<string, ProfileAttribute>, bool>) (kv => kv.Value.Revision > profile.Revision)).GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          KeyValuePair<string, ProfileAttribute> current = enumerator.Current;
          throw new ArgumentException(string.Format("Current profile object is in a bad state. The revision of attribute '{0}': '{1}' is greater than the revision of the profile object: {2}", (object) current.Key, (object) current.Value.Revision, (object) profile.Revision));
        }
      }
    }
  }
}
