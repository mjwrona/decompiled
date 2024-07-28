// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.WebLayoutXmlHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  public static class WebLayoutXmlHelper
  {
    public static readonly int MaxNumberOfSections = 4;
    public static readonly string WorkItemFormHistoryLabel = InternalsResourceStrings.Format("WorkItemFormHistory");
    public static readonly string WorkItemFormLinksLabel = InternalsResourceStrings.Format("WorkItemFormLinks");
    public static readonly string WorkItemFormAttachmentsLabel = InternalsResourceStrings.Format("WorkItemFormAttachments");
    public static readonly string RelatedWorkControlId = "Related Work";
    public static readonly string DeploymentControlId = "Deployments";
    public static readonly string DevelopmentControlId = "Development";
    public static readonly string WorkItemFormHistoryPageIdSuffix = "System_History";
    public static readonly string WorkItemFormLinksPageIdSuffix = "System_Links";
    public static readonly string WorkItemFormAttachmentsPageIdSuffix = "System_Attachments";
    public static readonly string SealedGroupSuffix = "WideGroup";
    public static readonly HashSet<string> HeaderFields = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "System.Reason",
      "System.AssignedTo",
      "System.History",
      "System.IterationPath",
      "System.State",
      "System.Title",
      "System.AreaPath"
    };
    public static readonly HashSet<string> HeaderControls = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      WellKnownControlNames.AttachmentsControl,
      WellKnownControlNames.LinksControl
    };
    public static readonly HashSet<string> HeaderControlTypes = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      WellKnownControlNames.FieldControl,
      WellKnownControlNames.DateControl
    };
    public static readonly HashSet<string> IgnoredHeaderControlsForControlTypeValidation = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "System.AreaPath",
      "System.IterationPath",
      "System.History"
    };
    private static readonly HashSet<string> IgnoredRequiredFields = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "System.ChangedBy",
      "System.ChangedDate",
      "System.Id"
    };
    public static readonly HashSet<string> WideControls = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      WellKnownControlNames.HtmlControl,
      WellKnownControlNames.WebpageControl,
      WellKnownControlNames.AssociatedAutomationControl
    };
    private static readonly HashSet<string> ResizableControls = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      WellKnownControlNames.HtmlControl,
      WellKnownControlNames.WebpageControl,
      WellKnownControlNames.LinksControl
    };
    public static readonly HashSet<string> WebLayoutSystemControls = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      WellKnownControlNames.AttachmentsControl,
      WellKnownControlNames.WorkItemLogControl
    };
    public static readonly HashSet<string> SystemPageIdSuffixes = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      WebLayoutXmlHelper.WorkItemFormHistoryPageIdSuffix,
      WebLayoutXmlHelper.WorkItemFormLinksPageIdSuffix,
      WebLayoutXmlHelper.WorkItemFormAttachmentsPageIdSuffix
    };
    public static readonly HashSet<string> OtherFirstPartyControls = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      WellKnownControlNames.ClassificationControl,
      WellKnownControlNames.DevelopmentControl,
      WellKnownControlNames.DeploymentsControl,
      WellKnownControlNames.DateControl,
      WellKnownControlNames.FieldControl,
      WellKnownControlNames.LabelControl,
      WellKnownControlNames.ParameterSetControl,
      WellKnownControlNames.TestStepsControl
    };
    public static readonly HashSet<string> ReplaceableOrHidableSystemControlFields = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "System.Reason",
      "System.AreaPath",
      "System.IterationPath"
    };
    public static readonly HashSet<string> RecognizableControlType = new HashSet<string>(WebLayoutXmlHelper.OtherFirstPartyControls.Union<string>((IEnumerable<string>) LayoutConstants.SystemControls).Union<string>((IEnumerable<string>) LayoutConstants.WideControls));

    public static void ValidateRequiredFieldsOnLayout(
      XmlElement fieldsElement,
      XmlElement workflowElement,
      XmlElement formElement,
      Action<string> requiredFieldNotInBothLayoutsErrorAction)
    {
      XmlElement xmlElement = formElement.SelectSingleNode(ProvisionTags.Layout) as XmlElement;
      XmlElement webLayoutElement = formElement.SelectSingleNode("WebLayout") as XmlElement;
      if (xmlElement == null || webLayoutElement == null)
        return;
      string xpath1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "//{0}[descendant::{1}]/@{2}", (object) ProvisionTags.FieldReference, (object) ProvisionTags.RequiredRule, (object) ProvisionAttributes.ReferenceName);
      string format = ".//{0}[translate(@{1}, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '{2}']";
      foreach (string fieldName in (fieldsElement != null ? (IEnumerable<string>) fieldsElement.SelectNodes(xpath1).Cast<XmlAttribute>().Select<XmlAttribute, string>((Func<XmlAttribute, string>) (a => a.Value)).ToList<string>() : Enumerable.Empty<string>()).Concat<string>(workflowElement != null ? (IEnumerable<string>) workflowElement.SelectNodes(xpath1).Cast<XmlAttribute>().Select<XmlAttribute, string>((Func<XmlAttribute, string>) (a => a.Value)).ToList<string>() : Enumerable.Empty<string>()).Distinct<string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName))
      {
        string xpath2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, (object) ProvisionTags.Control, (object) ProvisionAttributes.ControlFieldName, (object) fieldName.ToLowerInvariant());
        int count = xmlElement.SelectNodes(xpath2).Count;
        int num = webLayoutElement.SelectNodes(xpath2).Count;
        if (num == 0)
          num = WebLayoutXmlHelper.CountFieldControlInControlContribution(fieldName, webLayoutElement);
        if (count == 0 && num > 0)
          requiredFieldNotInBothLayoutsErrorAction(fieldName);
        if (count != 0 && num == 0 && !WebLayoutXmlHelper.IgnoredRequiredFields.Contains<string>(fieldName, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase) && !WebLayoutXmlHelper.HeaderFields.Contains<string>(fieldName, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase))
          requiredFieldNotInBothLayoutsErrorAction(fieldName);
      }
    }

    public static void ValidateWebLayoutControls(
      XmlElement webLayoutElement,
      Action<string> systemControlNotAllowedErrorAction,
      Action controlHeightNotAllowedErrorAction,
      Action<string> controlNotRecognizedWarningAction)
    {
      string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.Control);
      XmlNodeList xmlNodeList = webLayoutElement.SelectNodes(xpath);
      HashSet<string> stringSet = new HashSet<string>();
      XmlNode xmlNode = webLayoutElement.SelectSingleNode(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.SystemControls));
      foreach (XmlElement xmlElement in xmlNodeList)
      {
        xmlElement.GetAttribute(ProvisionAttributes.ControlFieldName);
        string attribute1 = xmlElement.GetAttribute(ProvisionAttributes.ControlType);
        string attribute2 = xmlElement.GetAttribute(ProvisionAttributes.ControlHeight);
        XmlNode parentNode = xmlElement.ParentNode;
        xmlElement.GetAttribute(ProvisionAttributes.ControlName);
        if (xmlNode == null || !xmlNode.Name.Equals(parentNode.Name, StringComparison.InvariantCultureIgnoreCase))
        {
          if (!stringSet.Contains(attribute1))
          {
            if (WebLayoutXmlHelper.WebLayoutSystemControls.Contains(attribute1))
              systemControlNotAllowedErrorAction(attribute1);
            stringSet.Add(attribute1);
          }
          if (!string.IsNullOrEmpty(attribute2) && !WebLayoutXmlHelper.ResizableControls.Contains(attribute1))
            controlHeightNotAllowedErrorAction();
          if (!WebLayoutXmlHelper.RecognizableControlType.Contains(attribute1))
            controlNotRecognizedWarningAction(attribute1);
        }
      }
    }

    public static void ValidateWebLayoutSystemControls(
      XmlElement webLayoutElement,
      ValidateWebLayoutSystemControlErrorActions errorActions)
    {
      HashSet<string> stringSet1 = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HashSet<string> stringSet2 = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      XmlNode xmlNode = webLayoutElement.SelectSingleNode(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.SystemControls));
      if (xmlNode == null)
        return;
      foreach (XmlElement xmlElement in xmlNode.ChildNodes.OfType<XmlElement>())
      {
        string attribute1 = xmlElement.GetAttribute(ProvisionAttributes.ControlFieldName);
        string attribute2 = xmlElement.GetAttribute(ProvisionAttributes.ControlReplacesFieldReferenceName);
        string attribute3 = xmlElement.GetAttribute(ProvisionAttributes.ControlType);
        string attribute4 = xmlElement.GetAttribute(ProvisionAttributes.ControlName);
        string attribute5 = xmlElement.GetAttribute(ProvisionAttributes.Visible);
        int num = string.IsNullOrEmpty(attribute5) ? 1 : (XmlConvert.ToBoolean(attribute5) ? 1 : 0);
        bool flag = false;
        if (!string.IsNullOrEmpty(attribute2))
        {
          if (!WebLayoutXmlHelper.ReplaceableOrHidableSystemControlFields.Contains(attribute2))
            errorActions.SystemControlReplacesFieldErrorAction(string.Join(", ", (IEnumerable<string>) WebLayoutXmlHelper.ReplaceableOrHidableSystemControlFields));
          flag = true;
        }
        if (num == 0 && !WebLayoutXmlHelper.ReplaceableOrHidableSystemControlFields.Contains(attribute1))
          errorActions.SystemControlHiddenFieldErrorAction(string.Join(", ", (IEnumerable<string>) WebLayoutXmlHelper.ReplaceableOrHidableSystemControlFields));
        if (!string.IsNullOrEmpty(attribute1) && !WebLayoutXmlHelper.HeaderControls.Contains(attribute3))
        {
          if (!flag && !WebLayoutXmlHelper.HeaderFields.Contains(attribute1))
            errorActions.SystemControlFieldErrorAction(attribute1);
          if (flag && WebLayoutXmlHelper.HeaderFields.Contains(attribute1))
            errorActions.SystemControlDuplicateErrorAction(attribute1);
          if (!stringSet1.Add(attribute1))
            errorActions.SystemControlDuplicateErrorAction(attribute1);
          if (flag && stringSet1.Contains(attribute2))
            errorActions.SystemControlReplacesDuplicateFieldErrorAction(attribute2);
          if (!WebLayoutXmlHelper.HeaderControlTypes.Contains(attribute3) && !WebLayoutXmlHelper.IgnoredHeaderControlsForControlTypeValidation.Contains(attribute1))
            errorActions.SystemControlTypeErrorAction(attribute3);
        }
        else
        {
          if (!WebLayoutXmlHelper.HeaderControls.Contains(attribute3))
            errorActions.SystemControlTypeErrorAction(attribute3);
          if (!stringSet1.Add(attribute3))
            errorActions.SystemControlDuplicateErrorAction(attribute3);
          if (!stringSet2.Add(attribute4))
            errorActions.SystemControlDuplicateErrorAction(attribute4);
          if (flag)
            errorActions.SystemControlTypeErrorAction(attribute2);
        }
      }
    }

    public static void ValidateWebLayoutExtensions(
      XmlElement webLayoutElement,
      IEnumerable<InstalledExtension> formExtensions,
      Func<string, string[], bool> doesFieldExist)
    {
      List<InstalledExtension> declaredExtensions;
      WebLayoutXmlHelper.ValidateWebLayoutExtensionsList(webLayoutElement, formExtensions, out declaredExtensions);
      Dictionary<string, Contribution> dictionary = declaredExtensions.SelectMany<InstalledExtension, Contribution>((Func<InstalledExtension, IEnumerable<Contribution>>) (ext => ext.Contributions)).ToDictionary<Contribution, string>((Func<Contribution, string>) (c => c.Id), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      WebLayoutXmlHelper.ValidateWebLayoutPageContributions(webLayoutElement, dictionary);
      WebLayoutXmlHelper.ValidateWebLayoutGroupContributions(webLayoutElement, dictionary);
      WebLayoutXmlHelper.ValidateWebLayoutControlContributions(webLayoutElement, dictionary, doesFieldExist);
    }

    private static int CountFieldControlInControlContribution(
      string fieldName,
      XmlElement webLayoutElement)
    {
      string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}[{1}[{2}[@{3}='{4}']]]", (object) ProvisionTags.ControlContribution, (object) ProvisionTags.ControlContributionInputs, (object) ProvisionTags.ControlContributionInput, (object) ProvisionAttributes.InputValue, (object) fieldName);
      return webLayoutElement.SelectNodes(xpath).Count;
    }

    private static void ValidateWebLayoutExtensionsList(
      XmlElement webLayoutElement,
      IEnumerable<InstalledExtension> formExtensions,
      out List<InstalledExtension> declaredExtensions)
    {
      declaredExtensions = new List<InstalledExtension>();
      string xpath1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.Extension);
      string xpath2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//*[local-name()='{0}' or local-name()='{1}' or local-name()='{2}']", (object) ProvisionTags.PageContribution, (object) ProvisionTags.GroupContribution, (object) ProvisionTags.ControlContribution);
      XmlNodeList xmlNodeList1 = webLayoutElement.SelectNodes(xpath1);
      XmlNodeList xmlNodeList2 = webLayoutElement.SelectNodes(xpath2);
      HashSet<string> values = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (xmlNodeList1.Count == 0 && xmlNodeList2.Count > 0)
        throw new FormExtensionNotDeclaredException();
      foreach (XmlElement xmlElement in xmlNodeList1)
      {
        string extensionId = xmlElement.GetAttribute(ProvisionAttributes.ExtensionId);
        InstalledExtension installedExtension = formExtensions != null ? formExtensions.FirstOrDefault<InstalledExtension>((Func<InstalledExtension, bool>) (ext => string.Equals(GalleryUtil.CreateFullyQualifiedName(ext.PublisherName, ext.ExtensionName), extensionId, StringComparison.OrdinalIgnoreCase))) : (InstalledExtension) null;
        if (installedExtension != null)
        {
          if (!declaredExtensions.Any<InstalledExtension>((Func<InstalledExtension, bool>) (ex => GalleryUtil.CreateFullyQualifiedName(ex.PublisherName, ex.ExtensionName).Equals(extensionId, StringComparison.OrdinalIgnoreCase))))
            declaredExtensions.Add(installedExtension);
          if (!installedExtension.Contributions.Any<Contribution>((Func<Contribution, bool>) (c => ((IEnumerable<string>) WorkItemFormExtensionsConstants.ValidFormContributionTypes).Contains<string>(c.Type, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))))
            throw new ExtensionDoesNotContainValidFormContribution(extensionId);
        }
        else
          values.Add(extensionId);
      }
      if (values.Count > 0)
        throw new FormExtensionNotFoundOrNoFormContribution(string.Join(", ", (IEnumerable<string>) values));
    }

    private static IEnumerable<string> GetDuplicateContributionNodes(XmlNodeList contributionNodes) => contributionNodes.Cast<XmlElement>().Select<XmlElement, string>((Func<XmlElement, string>) (xe => xe.GetAttribute(ProvisionAttributes.ContributionId))).GroupBy<string, string>((Func<string, string>) (s => s), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Where<IGrouping<string, string>>((Func<IGrouping<string, string>, bool>) (g => g.Count<string>() > 1)).Select<IGrouping<string, string>, string>((Func<IGrouping<string, string>, string>) (g => g.Key));

    private static void ValidateWebLayoutPageContributions(
      XmlElement webLayoutElement,
      Dictionary<string, Contribution> idToFormContribution)
    {
      string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.PageContribution);
      XmlNodeList contributionNodes1 = webLayoutElement.SelectNodes(xpath);
      WebLayoutXmlHelper.ValidateWebLayoutContributionNodes(contributionNodes1, idToFormContribution, "ms.vss-work-web.work-item-form-page");
      IEnumerable<string> contributionNodes2 = WebLayoutXmlHelper.GetDuplicateContributionNodes(contributionNodes1);
      if (contributionNodes2.Any<string>())
        throw new DuplicatePageContributionsException(contributionNodes2.First<string>());
    }

    private static void ValidateWebLayoutGroupContributions(
      XmlElement webLayoutElement,
      Dictionary<string, Contribution> idToFormContribution)
    {
      string xpath1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.GroupContribution);
      WebLayoutXmlHelper.ValidateWebLayoutContributionNodes(webLayoutElement.SelectNodes(xpath1), idToFormContribution, "ms.vss-work-web.work-item-form-group");
      string xpath2 = ".//" + ProvisionTags.Page;
      string xpathGroupContributionNodes = ".//" + ProvisionTags.GroupContribution;
      webLayoutElement.SelectNodes(xpath2).Cast<XmlElement>().ToList<XmlElement>().ForEach((Action<XmlElement>) (pageNode =>
      {
        IEnumerable<string> contributionNodes = WebLayoutXmlHelper.GetDuplicateContributionNodes(pageNode.SelectNodes(xpathGroupContributionNodes));
        if (contributionNodes.Any<string>())
          throw new DuplicateGroupContributionsException(pageNode.GetAttribute(ProvisionAttributes.Label), contributionNodes.First<string>());
      }));
    }

    private static void ValidateWebLayoutControlContributions(
      XmlElement webLayoutElement,
      Dictionary<string, Contribution> idToFormContribution,
      Func<string, string[], bool> doesFieldExist)
    {
      string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.ControlContribution);
      foreach (XmlElement selectNode in webLayoutElement.SelectNodes(xpath))
      {
        WebLayoutXmlHelper.ValidateWebLayoutContributionNode(selectNode, idToFormContribution, "ms.vss-work-web.work-item-form-control");
        string attribute = selectNode.GetAttribute(ProvisionAttributes.ContributionId);
        Contribution contribution = idToFormContribution[attribute];
        if (contribution != null && contribution.Properties != null)
          WebLayoutXmlHelper.ValidateWebLayoutControlContributionInputsWithXmlElement(contribution, doesFieldExist, selectNode);
      }
    }

    private static void ValidateWebLayoutContributionNodes(
      XmlNodeList contributionNodes,
      Dictionary<string, Contribution> idToFormContribution,
      string expectedType)
    {
      foreach (XmlElement contributionNode in contributionNodes)
        WebLayoutXmlHelper.ValidateWebLayoutContributionNode(contributionNode, idToFormContribution, expectedType);
    }

    private static void ValidateWebLayoutContributionNode(
      XmlElement contributionNode,
      Dictionary<string, Contribution> idToFormContribution,
      string expectedType)
    {
      string attribute = contributionNode.GetAttribute(ProvisionAttributes.ContributionId);
      if (!idToFormContribution.ContainsKey(attribute))
        throw new ContributionNotFoundOrNotInListedExtensionsElement(attribute);
      if (idToFormContribution[attribute].Type != expectedType)
        throw new InvalidContributionTypeException(attribute, expectedType.ToString());
    }

    internal static void ValidateWebLayoutControlContributionInputs(
      Contribution contribution,
      Func<string, string[], bool> doesFieldExist,
      IDictionary<string, object> inputValues)
    {
      JToken property = contribution.Properties["inputs"];
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, string> dictionary2 = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, string[]> dictionary3 = new Dictionary<string, string[]>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HashSet<string> source1 = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (property != null)
      {
        foreach (JToken jtoken1 in (IEnumerable<JToken>) property)
        {
          string key = jtoken1.Value<string>((object) "id");
          JToken jtoken2 = jtoken1.SelectToken("validation");
          string str1 = jtoken1.Value<string>((object) "type");
          JToken jtoken3 = jtoken1.SelectToken("properties");
          if (!string.IsNullOrWhiteSpace(key) && !dictionary1.ContainsKey(key))
          {
            bool flag = false;
            string str2 = "String";
            if (jtoken2 != null)
            {
              flag = jtoken2.SelectToken("isRequired") != null ? jtoken2.Value<bool>((object) "isRequired") : flag;
              str2 = jtoken2.SelectToken("dataType") != null ? jtoken2.Value<string>((object) "dataType") : str2;
            }
            dictionary1[key] = str2;
            if (flag)
              source1.Add(key);
          }
          if (!string.IsNullOrWhiteSpace(str1))
            dictionary2[key] = str1;
          if (jtoken3 != null)
          {
            JToken source2 = jtoken3.SelectToken("workItemFieldTypes");
            dictionary3[key] = source2 == null || !source2.Any<JToken>() ? (string[]) null : source2.Values<string>().ToArray<string>();
          }
        }
      }
      if (dictionary1.Keys.Count == 0)
        return;
      foreach (string key in (IEnumerable<string>) inputValues.Keys)
      {
        string str3 = inputValues[key].ToString();
        source1.Remove(key);
        string contributionInputType;
        if (!dictionary1.TryGetValue(key, out contributionInputType))
          throw new InvalidContributionInputIdException(contribution.Id, key);
        InputDataType result;
        if (Enum.TryParse<InputDataType>(contributionInputType, out result))
        {
          switch (result)
          {
            case InputDataType.Number:
              if (!double.TryParse(str3, NumberStyles.Number, (IFormatProvider) NumberFormatInfo.InvariantInfo, out double _))
                throw new InvalidContributionInputValueException(contribution.Id, key, str3);
              break;
            case InputDataType.Boolean:
              if (!bool.TryParse(str3, out bool _))
                throw new InvalidContributionInputValueException(contribution.Id, key, str3);
              break;
          }
        }
        else
        {
          if (!contributionInputType.Equals("Field", StringComparison.OrdinalIgnoreCase))
            throw new InvalidContributionInputTypeException(contribution.Id, key, contributionInputType);
          if (!doesFieldExist(str3, (string[]) null))
            throw new InvalidContributionInputValueException(contribution.Id, key, str3);
        }
        string str4;
        if (dictionary2.TryGetValue(key, out str4) && str4.Equals("WorkItemField", StringComparison.OrdinalIgnoreCase))
        {
          string[] strArray = (string[]) null;
          if (dictionary3.ContainsKey(key))
            strArray = dictionary3[key];
          if (!doesFieldExist(str3, strArray))
            throw new InvalidContributionInputValueException(contribution.Id, key, str3);
        }
      }
      if (source1.Count > 0)
        throw new ContributionInputNotSpecifiedException(contribution.Id, source1.FirstOrDefault<string>());
    }

    private static void ValidateWebLayoutControlContributionInputsWithXmlElement(
      Contribution contribution,
      Func<string, string[], bool> doesFieldExist,
      XmlElement contributionNode)
    {
      string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "./{0}/{1}", (object) ProvisionTags.ControlContributionInputs, (object) ProvisionTags.ControlContributionInput);
      XmlNodeList xmlNodeList = contributionNode.SelectNodes(xpath);
      Dictionary<string, object> inputValues = new Dictionary<string, object>();
      foreach (XmlElement xmlElement in xmlNodeList)
      {
        string attribute1 = xmlElement.GetAttribute("Id");
        string attribute2 = xmlElement.GetAttribute("Value");
        inputValues.Add(attribute1, (object) attribute2);
      }
      WebLayoutXmlHelper.ValidateWebLayoutControlContributionInputs(contribution, doesFieldExist, (IDictionary<string, object>) inputValues);
    }

    public static void GeneratePageAndGroupIds(
      string processName,
      string workItemTypeName,
      string oobFirstPageLabel,
      XmlElement webLayoutElement,
      Action<string> invalidOrDuplicatedPageLabelErrorAction,
      Action<string> invalidOrDuplicatedGroupLabelErrorAction,
      Action<string> invalidControlsInGroupErrorAction,
      Action<string, string> duplicateControlsInGroupErrorAction)
    {
      HashSet<string> stringSet1 = new HashSet<string>((IEnumerable<string>) WebLayoutXmlHelper.SystemPageIdSuffixes, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      string xpath1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.Page);
      XmlNodeList xmlNodeList1 = webLayoutElement.SelectNodes(xpath1);
      foreach (XmlElement xmlElement in xmlNodeList1)
      {
        string attribute1 = xmlElement.GetAttribute(ProvisionAttributes.Label);
        if (stringSet1.Contains(attribute1))
          invalidOrDuplicatedPageLabelErrorAction(attribute1);
        stringSet1.Add(attribute1);
        string str;
        if (xmlNodeList1.Count == 1 && (string.IsNullOrEmpty(oobFirstPageLabel) || string.Equals(attribute1, oobFirstPageLabel)))
          str = LayoutHelper.NormalizeLayoutElementId((processName ?? string.Empty) + "." + workItemTypeName + "." + workItemTypeName);
        else
          str = LayoutHelper.NormalizeLayoutElementId((processName ?? string.Empty) + "." + workItemTypeName + "." + attribute1);
        xmlElement.SetAttribute(ProvisionAttributes.WebLayoutElementId, str);
        HashSet<string> stringSet2 = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        HashSet<string> existingIds = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        string xpath2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.Section);
        foreach (XmlNode selectNode1 in xmlElement.SelectNodes(xpath2))
        {
          foreach (XmlElement selectNode2 in selectNode1.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.Group)))
          {
            HashSet<string> stringSet3 = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            string attribute2 = selectNode2.GetAttribute(ProvisionAttributes.Label);
            string xpath3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}|.//{1}", (object) ProvisionTags.Control, (object) ProvisionTags.ControlContribution);
            XmlNodeList xmlNodeList2 = selectNode2.SelectNodes(xpath3);
            bool flag1 = false;
            if (xmlNodeList2.Count > 0)
            {
              bool flag2 = false;
              for (int i = 0; i < xmlNodeList2.Count; ++i)
              {
                string attribute3 = ((XmlElement) xmlNodeList2[i]).GetAttribute(ProvisionAttributes.ControlFieldName);
                if (!string.IsNullOrEmpty(attribute3))
                {
                  if (stringSet3.Contains(attribute3))
                    duplicateControlsInGroupErrorAction(attribute3, attribute2);
                  stringSet3.Add(attribute3);
                }
                string attribute4 = ((XmlElement) xmlNodeList2[i]).GetAttribute(ProvisionAttributes.ControlType);
                if (WebLayoutXmlHelper.WideControls.Contains(attribute4))
                  flag1 = true;
                if (flag1 && i != xmlNodeList2.Count - 1 | flag2)
                  invalidControlsInGroupErrorAction(attribute2);
                if (!TFStringComparer.ControlType.Equals(attribute4, WellKnownControlNames.LabelControl))
                  flag2 = true;
              }
            }
            if (!flag1 && stringSet2.Contains(attribute2))
              invalidOrDuplicatedGroupLabelErrorAction(attribute2);
            string id = LayoutHelper.NormalizeLayoutElementId(str + "." + attribute2);
            if (!flag1)
              stringSet2.Add(attribute2);
            else
              id = string.Format("{0}.{1}", (object) WebLayoutXmlHelper.GenerateUniqueId(id, existingIds), (object) WebLayoutXmlHelper.SealedGroupSuffix);
            selectNode2.SetAttribute(ProvisionAttributes.WebLayoutElementId, id);
          }
        }
      }
    }

    public static string GenerateUniqueId(string id, HashSet<string> existingIds)
    {
      if (!existingIds.Contains(id))
      {
        existingIds.Add(id);
        return id;
      }
      int num = 2;
      string uniqueId;
      do
      {
        uniqueId = id + num.ToString();
        ++num;
      }
      while (existingIds.Contains(uniqueId));
      existingIds.Add(uniqueId);
      return uniqueId;
    }

    public static void RemovePageAndGroupIds(XmlElement webLayoutElement)
    {
      webLayoutElement.Attributes.RemoveNamedItem(ProvisionAttributes.WebLayoutElementId);
      string xpath1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.Page);
      foreach (XmlNode selectNode in webLayoutElement.SelectNodes(xpath1))
        selectNode.Attributes.RemoveNamedItem(ProvisionAttributes.WebLayoutElementId);
      string xpath2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.Group);
      foreach (XmlNode selectNode in webLayoutElement.SelectNodes(xpath2))
        selectNode.Attributes.RemoveNamedItem(ProvisionAttributes.WebLayoutElementId);
    }

    public static void ValidateLinksControls(
      IMetadataProvisioningHelper provisioningHelper,
      XmlElement webLayoutElement)
    {
      XmlNodeList linksControlNodes = webLayoutElement.SelectNodes(".//" + ProvisionTags.Group + "//" + ProvisionTags.Control + "[@" + ProvisionAttributes.ControlType + "='" + WellKnownControlNames.LinksControl + "']");
      LinksValidator.Validate(provisioningHelper, linksControlNodes);
    }
  }
}
