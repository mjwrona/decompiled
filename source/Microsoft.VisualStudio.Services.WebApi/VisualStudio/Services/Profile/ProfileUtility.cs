// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.ProfileUtility
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Profile
{
  internal static class ProfileUtility
  {
    private const string _attributesBelongingToDifferentContainersErrorMessage = "Collection of attributes has one attribute belonging to container '{0}' and another belonging to container '{1}'.";
    private const string _attributeDoesNotBelongToContainerErrorMessage = "Attribute '{0}' does not belong to container '{1}'.";
    private const string _applicationContainerNameCannotBeErrorMessage = "Application container name cannot be '{0}'.";
    private const string _anAplicationAttributesBelongsToContainerErrorMessage = "One of the application attributes '{0}' belongs to container '{1}.'";
    private const string _attributeMissingNecessaryInformationErrorMessage = "One of the attributes is missing necessary information to perform this operation.";
    private const string _attributeMissingNecessaryInformationDetailedErrorMessage = "The attribute '{0}' is missing necessary information to perform this operation.";
    private const string _coreAttributeNotSupported = "Core attribute '{0}' is not supported.";

    internal static object GetCorrectlyTypedCoreAttribute(string coreAttributeName, object value)
    {
      if (VssStringComparer.AttributesDescriptor.Equals("ContactWithOffers", coreAttributeName))
        return (object) Convert.ToBoolean(value, (IFormatProvider) CultureInfo.InvariantCulture);
      if (VssStringComparer.AttributesDescriptor.Equals("TermsOfServiceVersion", coreAttributeName))
        return (object) Convert.ToInt32(value, (IFormatProvider) CultureInfo.InvariantCulture);
      if (VssStringComparer.AttributesDescriptor.Equals("TermsOfServiceAcceptDate", coreAttributeName) || VssStringComparer.AttributesDescriptor.Equals("DateCreated", coreAttributeName))
      {
        switch (value)
        {
          case DateTimeOffset _:
            return value;
          case DateTime dateTime:
            return (object) new DateTimeOffset(dateTime);
          default:
            DateTimeOffset result;
            return (object) (DateTimeOffset.TryParse(value as string, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result) ? result : new DateTimeOffset());
        }
      }
      else if (VssStringComparer.AttributesDescriptor.Compare("Avatar", coreAttributeName) == 0)
      {
        switch (value)
        {
          case Avatar _:
            return value;
          case string _:
            return (object) JObject.Parse((string) value).ToObject<Avatar>();
          default:
            return (object) JObject.FromObject(value).ToObject<Avatar>();
        }
      }
      else
      {
        if (VssStringComparer.AttributesDescriptor.Equals("DisplayName", coreAttributeName) || VssStringComparer.AttributesDescriptor.Equals("CountryName", coreAttributeName) || VssStringComparer.AttributesDescriptor.Equals("EmailAddress", coreAttributeName) || VssStringComparer.AttributesDescriptor.Equals("UnconfirmedEmailAddress", coreAttributeName) || VssStringComparer.AttributesDescriptor.Equals("PublicAlias", coreAttributeName))
          return value;
        throw new ArgumentException(string.Format("Core attribute '{0}' is not supported.", (object) coreAttributeName));
      }
    }

    internal static CoreProfileAttribute ExtractCoreAttribute<T>(ProfileAttributeBase<T> attribute)
    {
      CoreProfileAttribute coreAttribute = new CoreProfileAttribute();
      coreAttribute.Descriptor = attribute.Descriptor;
      coreAttribute.Revision = attribute.Revision;
      coreAttribute.TimeStamp = attribute.TimeStamp;
      coreAttribute.Value = ProfileUtility.GetCorrectlyTypedCoreAttribute(attribute.Descriptor.AttributeName, (object) attribute.Value);
      return coreAttribute;
    }

    internal static ProfileAttribute ExtractApplicationAttribute(
      ProfileAttributeBase<object> attribute)
    {
      ProfileAttribute applicationAttribute = new ProfileAttribute();
      applicationAttribute.Descriptor = attribute.Descriptor;
      applicationAttribute.Revision = attribute.Revision;
      applicationAttribute.TimeStamp = attribute.TimeStamp;
      applicationAttribute.Value = attribute.Value as string;
      return applicationAttribute;
    }

    internal static void ValidateAttributes<T>(
      IEnumerable<ProfileAttributeBase<T>> attributes,
      string applicationContainerName = null)
    {
      if (attributes == null)
        return;
      ProfileUtility.ValidateAttributesMetadata<T>(attributes);
      ProfileUtility.ValidateAttributesAreEitherCoreAttributesOrBelongToOneApplicationContainer<T>(attributes, applicationContainerName);
    }

    internal static void ValidateAttributesMetadata<T>(
      IEnumerable<ProfileAttributeBase<T>> attributes)
    {
      if (attributes == null)
        return;
      foreach (ProfileAttributeBase<T> attribute in attributes)
      {
        if (attribute.Descriptor == null || attribute.Descriptor.AttributeName == null || attribute.Descriptor.ContainerName == null || attribute.Revision < 0)
        {
          if (attribute.Descriptor == null)
            throw new ArgumentException("One of the attributes is missing necessary information to perform this operation.", nameof (attributes));
          throw new ArgumentException(string.Format("The attribute '{0}' is missing necessary information to perform this operation.", (object) attribute.Descriptor), nameof (attributes));
        }
      }
    }

    internal static void ValidateAttributesBelongToCoreContainer(
      IEnumerable<CoreProfileAttribute> coreAttributes)
    {
      using (IEnumerator<CoreProfileAttribute> enumerator = coreAttributes.Where<CoreProfileAttribute>((Func<CoreProfileAttribute, bool>) (changedAttribute => VssStringComparer.AttributesDescriptor.Compare(changedAttribute.Descriptor.ContainerName, "Core") != 0)).GetEnumerator())
      {
        if (enumerator.MoveNext())
          throw new ArgumentException(string.Format("Attribute '{0}' does not belong to container '{1}'.", (object) enumerator.Current.Descriptor, (object) "Core"), nameof (coreAttributes));
      }
    }

    private static void ValidateAttributesAreEitherCoreAttributesOrBelongToOneApplicationContainer<T>(
      IEnumerable<ProfileAttributeBase<T>> attributes,
      string applicationContainerName = null)
    {
      foreach (ProfileAttributeBase<T> profileAttributeBase in attributes.Where<ProfileAttributeBase<T>>((Func<ProfileAttributeBase<T>, bool>) (changedAttribute => VssStringComparer.AttributesDescriptor.Compare(changedAttribute.Descriptor.ContainerName, "Core") != 0)))
      {
        if (applicationContainerName == null)
          applicationContainerName = profileAttributeBase.Descriptor.ContainerName;
        else if (VssStringComparer.AttributesDescriptor.Compare(applicationContainerName, profileAttributeBase.Descriptor.ContainerName) != 0)
          throw new ArgumentException(string.Format("Collection of attributes has one attribute belonging to container '{0}' and another belonging to container '{1}'.", (object) applicationContainerName, (object) profileAttributeBase.Descriptor.ContainerName), nameof (attributes));
      }
    }

    internal static void ValidateAttributesBelongToOneApplicaitonContainer(
      IEnumerable<ProfileAttribute> attributes,
      string applicationContainerName = null)
    {
      if (VssStringComparer.AttributesDescriptor.Compare(applicationContainerName, "Core") == 0)
        throw new ArgumentException(string.Format("Application container name cannot be '{0}'.", (object) "Core"), nameof (applicationContainerName));
      foreach (ProfileAttribute attribute in attributes)
      {
        if (VssStringComparer.AttributesDescriptor.Compare(attribute.Descriptor.ContainerName, "Core") == 0)
          throw new ArgumentException(string.Format("One of the application attributes '{0}' belongs to container '{1}.'", (object) attribute.Descriptor, (object) "Core"), nameof (attributes));
        if (applicationContainerName == null)
          applicationContainerName = attribute.Descriptor.ContainerName;
        else if (VssStringComparer.AttributesDescriptor.Compare(applicationContainerName, attribute.Descriptor.ContainerName) != 0)
          throw new ArgumentException(string.Format("Collection of attributes has one attribute belonging to container '{0}' and another belonging to container '{1}'.", (object) applicationContainerName, (object) attribute.Descriptor.ContainerName), nameof (attributes));
      }
    }
  }
}
