// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.MigratingProfile
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Profile
{
  [DataContract]
  public class MigratingProfile
  {
    private const string s_area = "VisualStudio.Services.ProfileService";
    private const string s_layer = "BusinessLogicMigratingProfile";
    private const string s_featureFlagMigrateProfileIgnoreAttributesCheck = "VisualStudio.ProfileService.MigrateProfileIgnoreAttributesCheck";

    [DataMember]
    public Microsoft.VisualStudio.Services.Profile.Profile CoreProfile { get; set; }

    [DataMember]
    public List<Tuple<IList<ProfileAttribute>, IList<CoreProfileAttribute>>> Attributes { get; set; }

    [DataMember]
    public MigratingAvatar MigratingAvatar { get; set; }

    public bool IsEqual(
      IVssRequestContext requestContext,
      MigratingProfile toCompareMigratingProfile)
    {
      if (toCompareMigratingProfile == null)
      {
        requestContext.Trace(10002, TraceLevel.Error, "VisualStudio.Services.ProfileService", "BusinessLogicMigratingProfile", "toCompareMigratingProfile is null");
        return false;
      }
      if (this.CoreProfile == null || toCompareMigratingProfile.CoreProfile == null)
      {
        if (this.CoreProfile == null)
          requestContext.Trace(10003, TraceLevel.Error, "VisualStudio.Services.ProfileService", "BusinessLogicMigratingProfile", "CoreProfile is null");
        if (toCompareMigratingProfile.CoreProfile == null)
          requestContext.Trace(10003, TraceLevel.Error, "VisualStudio.Services.ProfileService", "BusinessLogicMigratingProfile", "toCompareMigratingProfile.CoreProfile is null");
        return false;
      }
      return this.IsCoreProfileEqual(requestContext, toCompareMigratingProfile.CoreProfile) && this.IsAvatarEqual(requestContext, toCompareMigratingProfile.MigratingAvatar) && this.IsApplicationAttributesEqual(requestContext, (IList<Tuple<IList<ProfileAttribute>, IList<CoreProfileAttribute>>>) toCompareMigratingProfile.Attributes);
    }

    private bool IsCoreProfileEqual(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Profile.Profile toCompareProfile)
    {
      bool? contactWithOffers1;
      int num;
      if (this.CoreProfile.DisplayName == toCompareProfile.DisplayName && this.CoreProfile.CountryName == toCompareProfile.CountryName && this.CoreProfile.EmailAddress == toCompareProfile.EmailAddress && this.CoreProfile.UnconfirmedEmailAddress == toCompareProfile.UnconfirmedEmailAddress && this.CoreProfile.Avatar.TimeStamp == toCompareProfile.Avatar.TimeStamp && this.CoreProfile.TermsOfServiceVersion == toCompareProfile.TermsOfServiceVersion && this.CoreProfile.TermsOfServiceAcceptDate == toCompareProfile.TermsOfServiceAcceptDate && this.CoreProfile.TimeStamp == toCompareProfile.TimeStamp)
      {
        bool? contactWithOffers2 = this.CoreProfile.ContactWithOffers;
        contactWithOffers1 = toCompareProfile.ContactWithOffers;
        num = contactWithOffers2.GetValueOrDefault() == contactWithOffers1.GetValueOrDefault() & contactWithOffers2.HasValue == contactWithOffers1.HasValue ? 1 : 0;
      }
      else
        num = 0;
      if (num != 0)
        return num != 0;
      StringBuilder stringBuilder = new StringBuilder("IsCoreProfileEqual failed Core attribute Name:");
      if (this.CoreProfile.DisplayName != toCompareProfile.DisplayName)
        stringBuilder.Append(" DisplayName");
      if (this.CoreProfile.CountryName != toCompareProfile.CountryName)
        stringBuilder.Append(" CountryName");
      if (this.CoreProfile.EmailAddress != toCompareProfile.EmailAddress)
        stringBuilder.Append(" EmailAddress");
      if (this.CoreProfile.UnconfirmedEmailAddress != toCompareProfile.UnconfirmedEmailAddress)
        stringBuilder.Append(" UnconfirmedEmailAddress");
      if (this.CoreProfile.Avatar.TimeStamp == toCompareProfile.Avatar.TimeStamp)
        stringBuilder.Append(" Avatar.TimeStamp");
      if (this.CoreProfile.TermsOfServiceVersion == toCompareProfile.TermsOfServiceVersion)
        stringBuilder.Append(" TermsOfServiceVersion");
      if (this.CoreProfile.TermsOfServiceAcceptDate == toCompareProfile.TermsOfServiceAcceptDate)
        stringBuilder.Append(" TermsOfServiceAcceptDate");
      if (this.CoreProfile.TimeStamp == toCompareProfile.TimeStamp)
        stringBuilder.Append(" CoreProfile.TimeStamp ");
      contactWithOffers1 = this.CoreProfile.ContactWithOffers;
      bool? contactWithOffers3 = toCompareProfile.ContactWithOffers;
      if (contactWithOffers1.GetValueOrDefault() == contactWithOffers3.GetValueOrDefault() & contactWithOffers1.HasValue == contactWithOffers3.HasValue)
        stringBuilder.Append(" ContactWithOffers");
      requestContext.Trace(10008, TraceLevel.Error, "VisualStudio.Services.ProfileService", "BusinessLogicMigratingProfile", stringBuilder.ToString());
      return num != 0;
    }

    private bool IsAvatarEqual(
      IVssRequestContext requestContext,
      MigratingAvatar toCompareMigratingAvatar)
    {
      int num = this.MigratingAvatar.HasAvatar == toCompareMigratingAvatar.HasAvatar ? 1 : 0;
      if (num != 0)
        return num != 0;
      bool hasAvatar = this.MigratingAvatar.HasAvatar;
      string str1 = hasAvatar.ToString();
      hasAvatar = toCompareMigratingAvatar.HasAvatar;
      string str2 = hasAvatar.ToString();
      string message = string.Format("IsAvatarEqual failed, this.MigratingAvatar.HasAvatar : {0} toCompareMigratingAvatar.HasAvatar : {1}", (object) str1, (object) str2);
      requestContext.Trace(10004, TraceLevel.Error, "VisualStudio.Services.ProfileService", "BusinessLogicMigratingProfile", message);
      return num != 0;
    }

    private bool IsApplicationAttributesEqual(
      IVssRequestContext requestContext,
      IList<Tuple<IList<ProfileAttribute>, IList<CoreProfileAttribute>>> toCompareAttributes)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.ProfileService.MigrateProfileIgnoreAttributesCheck"))
        return true;
      if (this.Attributes == null || toCompareAttributes == null)
      {
        if (this.Attributes == null)
          requestContext.Trace(10004, TraceLevel.Error, "VisualStudio.Services.ProfileService", "BusinessLogicMigratingProfile", "IsApplicationAttributesEqual failed, this.Attributes == null");
        if (toCompareAttributes == null)
          requestContext.Trace(10005, TraceLevel.Error, "VisualStudio.Services.ProfileService", "BusinessLogicMigratingProfile", "IsApplicationAttributesEqual failed, toCompareAttributes == null");
        return false;
      }
      if (this.Attributes.Count != toCompareAttributes.Count)
      {
        requestContext.Trace(10006, TraceLevel.Error, "VisualStudio.Services.ProfileService", "BusinessLogicMigratingProfile", string.Format("IsApplicationAttributesEqual failed, Attributes.Count didnot match, Attributes.Count :{0} ,toCompareAttributes.Count: {1}", (object) this.Attributes.Count, (object) toCompareAttributes.Count));
        return false;
      }
      for (int index1 = 0; index1 < this.Attributes.Count; ++index1)
      {
        IList<ProfileAttribute> source = this.Attributes[index1].Item1;
        int count1 = source.Count;
        if (count1 != 0)
        {
          string containerName = source.First<ProfileAttribute>().Descriptor.ContainerName;
          if (string.IsNullOrEmpty(containerName))
          {
            requestContext.Trace(10006, TraceLevel.Error, "VisualStudio.Services.ProfileService", "BusinessLogicMigratingProfile", "IsApplicationAttributesEqual: Container name is empty");
            return false;
          }
          IList<ProfileAttribute> attibuteContainer = this.GetAttibuteContainer(containerName, toCompareAttributes);
          if (attibuteContainer == null)
          {
            requestContext.Trace(10006, TraceLevel.Error, "VisualStudio.Services.ProfileService", "BusinessLogicMigratingProfile", "IsApplicationAttributesEqual: Unable to get a matching container {0}", (object) containerName);
            return false;
          }
          int count2 = attibuteContainer.Count;
          if (count1 != count2)
          {
            requestContext.Trace(10006, TraceLevel.Error, "VisualStudio.Services.ProfileService", "BusinessLogicMigratingProfile", "IsApplicationAttributesEqual: Attirbutes count did not match for {0}", (object) containerName);
            return false;
          }
          Stopwatch stopwatch = Stopwatch.StartNew();
          List<ProfileAttribute> list1 = source.OrderBy<ProfileAttribute, AttributeDescriptor>((Func<ProfileAttribute, AttributeDescriptor>) (x => x.Descriptor)).ToList<ProfileAttribute>();
          List<ProfileAttribute> list2 = attibuteContainer.OrderBy<ProfileAttribute, AttributeDescriptor>((Func<ProfileAttribute, AttributeDescriptor>) (x => x.Descriptor)).ToList<ProfileAttribute>();
          stopwatch.Stop();
          requestContext.Trace(10006, TraceLevel.Info, "VisualStudio.Services.ProfileService", "BusinessLogicMigratingProfile", "Time taken to sort attributes with count {0} is {1}", (object) count1, (object) stopwatch.ElapsedMilliseconds);
          for (int index2 = 0; index2 < list1.Count; ++index2)
          {
            ProfileAttribute profileAttribute1 = list1[index2];
            ProfileAttribute profileAttribute2 = list2[index2];
            if (!profileAttribute1.Descriptor.Equals((object) profileAttribute2.Descriptor) || profileAttribute1.Value != profileAttribute2.Value || profileAttribute1.TimeStamp != profileAttribute2.TimeStamp)
            {
              string message = string.Format("IsApplicationAttributesEqual failed, attribute did not match :{0}!={1}", (object) (profileAttribute1.Descriptor.ToString() + ";" + profileAttribute1.Value.ToString() + ";" + profileAttribute1.TimeStamp.ToString()), (object) (profileAttribute2.Descriptor.ToString() + ";" + profileAttribute2.Value.ToString() + ";" + profileAttribute2.TimeStamp.ToString()));
              requestContext.Trace(10007, TraceLevel.Error, "VisualStudio.Services.ProfileService", "BusinessLogicMigratingProfile", message);
              return false;
            }
          }
        }
      }
      return true;
    }

    private IList<ProfileAttribute> GetAttibuteContainer(
      string containerName,
      IList<Tuple<IList<ProfileAttribute>, IList<CoreProfileAttribute>>> toCompareAttributes)
    {
      if (string.IsNullOrEmpty(containerName))
        return (IList<ProfileAttribute>) null;
      for (int index = 0; index < toCompareAttributes.Count; ++index)
      {
        if (toCompareAttributes[index].Item1.Any<ProfileAttribute>() && string.Equals(containerName, toCompareAttributes[index].Item1.First<ProfileAttribute>().Descriptor.ContainerName, StringComparison.OrdinalIgnoreCase))
          return toCompareAttributes[index].Item1;
      }
      return (IList<ProfileAttribute>) null;
    }
  }
}
