// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Xml;
// using System.Xml.Linq;
// using JetBrains.Annotations;
// using UIForia.Elements;
// using UIForia.Parsing.Expressions;
// using UIForia.Templates;
// using UIForia.Util;
// using UnityEngine;
//
// namespace UIForia.Parsing {
//
//     [UsedImplicitly]
//     public class TemplateParserDeprecatedXml : TemplateParser_Deprecated {
//
//         private TemplateShell_Deprecated shell;
//         private StructList<AttributeDefinition> attributes;
//         private StructList<AttributeDefinition> injectedAttributes;
//
//         public override void OnSetup() {
//             attributes = attributes ?? new StructList<AttributeDefinition>(16);
//             injectedAttributes = injectedAttributes ?? new StructList<AttributeDefinition>(16);
//         }
//
//         public override bool TryParse(string contents, TemplateShell_Deprecated templateShell) {
//             this.shell = templateShell;
//
//             XElement root;
//
//             try {
//                 root = XElement.Load(new XmlTextReader(contents, XmlNodeType.Element, XMLTemplateParser.s_XmlParserContext), LoadOptions.SetLineInfo | LoadOptions.PreserveWhitespace);
//             }
//             catch (Exception exception) {
//                 Debug.Log(exception);
//                 return ReportParseError(exception.Message);
//             }
//
//             root.MergeTextNodes();
//
//             ParseUsings(root);
//             ParseStyles(root);
//             ParseContents(root);
//
//             return true;
//         }
//
//         private void ParseContents(XElement root) {
//             IEnumerable<XElement> contentElements = root.GetChildren("Contents");
//
//             shell.templateRootNodes = new SizedArray<TemplateRootNode>(contentElements.Count());
//
//             foreach (XElement contentElement in contentElements) {
//                 if (TryParseContents(contentElement, out TemplateRootNode retn)) {
//                     shell.templateRootNodes.Add(retn);
//                 }
//             }
//         }
//
//         private bool TryParseContents(XElement contentRoot, out TemplateRootNode retn) {
//             retn = default;
//
//             SetErrorContext(contentRoot);
//
//             XAttribute attr = contentRoot.GetAttribute("id");
//
//             string templateId = null;
//
//             if (attr != null) {
//                 templateId = attr.Value.Trim();
//             }
//             
//             attributes.QuickClear();
//
//             // maybe tag name should be root? We don't actually know what element we are parsing at this point
//             ParseAttributes("Contents", contentRoot.Attributes(), attributes, injectedAttributes, out string genericTypeResolver, out string requireType);
//
//             LineInfo lineInfo = new LineInfo(((IXmlLineInfo) contentRoot).LineNumber, ((IXmlLineInfo) contentRoot).LinePosition);
//
//             retn = new TemplateRootNode(templateId, shell, ValidateRootAttributes(contentRoot, attributes.Clone()), lineInfo) {
//                 genericTypeResolver = genericTypeResolver,
//                 requireType = requireType
//             };
//
//             ParseChildren(retn, contentRoot.Nodes());
//
//             return true;
//         }
//
//         private void ParseChildren(TemplateNode parent, IEnumerable<XNode> nodes) {
//             string textContext = string.Empty;
//             foreach (XNode node in nodes) {
//                 IXmlLineInfo lineInfo2 = node;
//                 SetErrorContext(lineInfo2.LineNumber, lineInfo2.LinePosition);
//
//                 switch (node.NodeType) {
//                     case XmlNodeType.Text: {
//                         XText textNode = (XText) node;
//
//                         if (string.IsNullOrWhiteSpace(textNode.Value)) {
//                             continue;
//                         }
//
//                         textContext += textNode.Value.Trim();
//
//                         continue;
//                     }
//
//                     case XmlNodeType.Element: {
//                         XElement element = (XElement) node;
//
//                         if (textContext.Length > 0) {
//                             IXmlLineInfo textLineInfo = element.PreviousNode;
//                             CreateOrUpdateTextNode(parent, textContext, new LineInfo(textLineInfo.LineNumber, textLineInfo.LinePosition));
//                             textContext = string.Empty;
//                         }
//
//                         string tagName = element.Name.LocalName;
//                         string namespaceName = element.Name.NamespaceName;
//
//                         attributes.QuickClear();
//                         injectedAttributes.QuickClear();
//
//                         ParseAttributes(tagName, element.Attributes(), attributes, injectedAttributes, out string genericTypeResolver, out string requireType);
//
//                         ReadOnlySizedArray<AttributeDefinition> childAttributes = ReadOnlySizedArray<AttributeDefinition>.CopyFrom(attributes.array, attributes.size);
//                         ReadOnlySizedArray<AttributeDefinition> childInjectedAttributes = ReadOnlySizedArray<AttributeDefinition>.CopyFrom(injectedAttributes.array, injectedAttributes.size);
//
//                         TemplateNode templateNode;
//                       
//                         IXmlLineInfo xmlLineInfo = element;
//                         LineInfo lineInfo = new LineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
//
//                         if (namespaceName == "define") {
//                             parent.TryCreateSlotNode(tagName, childAttributes, childInjectedAttributes, lineInfo, SlotType.Define, requireType, out templateNode);
//                         }
//                         else if (namespaceName == "override") {
//                             parent.TryCreateSlotNode(tagName, childAttributes, childInjectedAttributes, lineInfo, SlotType.Override, requireType, out templateNode);
//                         }
//                         else if (namespaceName == "forward") {
//                             parent.TryCreateSlotNode(tagName, childAttributes, childInjectedAttributes, lineInfo, SlotType.Forward, requireType, out templateNode);
//                         }
//                         else if (string.IsNullOrEmpty(namespaceName) && string.Equals(tagName, "Repeat", StringComparison.Ordinal)) {
//                             parent.TryCreateRepeatNode(childAttributes, lineInfo, out templateNode);
//                         }
//                         else {
//                             if (!string.IsNullOrEmpty(genericTypeResolver)) {
//                                 genericTypeResolver.InlineReplace('[', '<').InlineReplace(']', '>');
//                             }
//
//                             if (!parent.TryCreateElementNode(namespaceName, tagName, childAttributes, lineInfo, genericTypeResolver, requireType, out templateNode)) {
//                                 // todo -- diagnostic
//                                 Debug.Log("Unable to create element " + tagName);
//                             }
//                         }
//
//                         if (templateNode == null) {
//                             continue;
//                         }
//
//                         ParseChildren(templateNode, element.Nodes());
//
//                         continue;
//                     }
//
//                     case XmlNodeType.Comment:
//                         continue;
//                 }
//
//                 ReportParseError($"Unable to handle node type: {node.NodeType}");
//             }
//
//             if (textContext.Length != 0) {
//                 CreateOrUpdateTextNode(parent, textContext, parent.lineInfo); // todo -- line info probably wrong
//             }
//         }
//         
//         private void SetErrorContext(XElement element) {
//             IXmlLineInfo info = element;
//             SetErrorContext(info.LineNumber, info.LinePosition);
//         }
//
//         private static void CreateOrUpdateTextNode(TemplateNode parent, string textContent, in LineInfo lineInfo) {
//             if (parent is TextNode textParent) {
//                 if (parent.ChildCount == 0) {
//                     TextTemplateProcessor.ProcessTextExpressions(textContent, textParent.textExpressionList);
//                 }
//                 else {
//                     TextNode node = new TextNode(TypeProcessor.GetProcessedType(typeof(UITextElement)), default, lineInfo);
//                     TextTemplateProcessor.ProcessTextExpressions(textContent, node.textExpressionList);
//                     parent.AddChild(node);
//                 }
//             }
//             else {
//                 TextNode node = new TextNode(TypeProcessor.GetProcessedType(typeof(UITextElement)), default, lineInfo);
//                 TextTemplateProcessor.ProcessTextExpressions(textContent, node.textExpressionList);
//                 parent.AddChild(node);
//             }
//         }
//
//         private ReadOnlySizedArray<AttributeDefinition> ValidateRootAttributes(XElement root, StructList<AttributeDefinition> attributes) {
//             if (attributes == null) return default;
//
//             SetErrorContext(root);
//
//             for (int i = 0; i < attributes.size; i++) {
//                 ref AttributeDefinition attr = ref attributes.array[i];
//
//                 // contents should remove `id` attr
//                 if (attr.type == AttributeType.Attribute && attr.key == "id") {
//                     attributes.RemoveAt(i--);
//                     continue;
//                 }
//
//                 if (attr.type == AttributeType.Conditional) {
//                     ReportParseError($"<Contents> cannot contain conditional bindings. Ignoring {attr.rawValue}.");
//                     attributes.RemoveAt(i--);
//                     continue;
//                 }
//
//                 if (attr.type == AttributeType.Property) {
//                     ReportParseError($"<Contents> cannot contain property bindings. Ignoring {attr.rawValue}.");
//                     attributes.RemoveAt(i--);
//                 }
//             }
//
//             return new ReadOnlySizedArray<AttributeDefinition>(attributes.size, attributes.array);
//         }
//
//         private void ParseAttributes(string tagName, IEnumerable<XAttribute> xmlAttributes, StructList<AttributeDefinition> attributes, StructList<AttributeDefinition> injectedAttributes, out string genericTypeResolver, out string requireType) {
//             genericTypeResolver = null;
//             requireType = null;
//
//             foreach (XAttribute attr in xmlAttributes) {
//                 string prefix = attr.Name.NamespaceName;
//                 string name = attr.Name.LocalName.Trim();
//
//                 int line = ((IXmlLineInfo) attr).LineNumber;
//                 int column = ((IXmlLineInfo) attr).LinePosition;
//                 SetErrorContext(line, column);
//
//                 if (name == "id" && string.IsNullOrEmpty(prefix)) {
//                     prefix = "attr";
//                 }
//
//                 if (prefix == "generic" && name == "type") {
//                     genericTypeResolver = attr.Value;
//                     continue;
//                 }
//
//                 if (prefix == "require" && name == "type") {
//                     requireType = attr.Value;
//                     continue;
//                 }
//
//                 if (prefix.StartsWith("inject.")) {
//                     prefix = prefix.Substring("inject.".Length);
//                     HandleAttribute(prefix, name, line, column, attr.Value, injectedAttributes);
//                     continue;
//                 }
//
//                 HandleAttribute(prefix, name, line, column, attr.Value, attributes);
//             }
//         }
//
//         private void HandleAttribute(string prefix, string name, int line, int column, string value, StructList<AttributeDefinition> attributes) {
//             AttributeType attributeType = AttributeType.Property;
//             AttributeFlags flags = 0;
//
//             // once:if=""
//             // enable:if=""
//             // todo -- not valid everywhere
//             if (name.Contains(".once") || name.Contains(".const")) {
//                 name = name.Replace(".once", "");
//                 name = name.Replace(".const", "");
//                 flags |= AttributeFlags.Const;
//             }
//
//             // todo -- validate this syntax
//             if (name.Contains(".enable")) {
//                 name = name.Replace(".enable", "");
//                 flags |= AttributeFlags.EnableOnly;
//             }
//
//             if (name == "if") {
//                 attributeType = AttributeType.Conditional;
//             }
//             else if (prefix == string.Empty) {
//                 if (name == "style") {
//                     attributeType = AttributeType.Style;
//                     name = "style";
//                 }
//                 else if (name.StartsWith("style.")) {
//                     attributeType = AttributeType.InstanceStyle;
//                     name = name.Substring("style.".Length);
//                 }
//             }
//             else {
//                 switch (prefix) {
//                     case "property":
//                         break;
//
//                     case "attr": {
//                         attributeType = AttributeType.Attribute;
//                         if (value[0] != '{' || value[value.Length - 1] != '}') {
//                             flags |= AttributeFlags.Const;
//                         }
//
//                         break;
//                     }
//
//                     case "slot": {
//                         attributeType = AttributeType.Slot;
//                         break;
//                     }
//
//                     case "mouse":
//                         attributeType = AttributeType.Mouse;
//                         break;
//
//                     case "key":
//                         attributeType = AttributeType.Key;
//                         break;
//
//                     case "drag":
//                         attributeType = AttributeType.Drag;
//                         break;
//
//                     case "onChange":
//                         attributeType = AttributeType.ChangeHandler;
//                         break;
//
//                     case "touch":
//                         attributeType = AttributeType.Touch;
//                         break;
//
//                     case "controller":
//                         attributeType = AttributeType.Controller;
//                         break;
//
//                     case "style":
//                         attributeType = AttributeType.InstanceStyle;
//                         if (name.Contains(".")) {
//                             if (name.StartsWith("hover.")) {
//                                 flags |= AttributeFlags.StyleStateHover;
//                                 name = name.Substring("hover.".Length);
//                             }
//                             else if (name.StartsWith("focus.")) {
//                                 flags |= AttributeFlags.StyleStateFocus;
//                                 name = name.Substring("focus.".Length);
//                             }
//                             else if (name.StartsWith("active.")) {
//                                 flags |= AttributeFlags.StyleStateActive;
//                                 name = name.Substring("active.".Length);
//                             }
//                             else {
//                                 ReportParseError($"Unable to handle style state declaration '{name.Split('.')[0]}' Expected 'active', 'focus', or 'hover'");
//                                 return;
//                             }
//                         }
//
//                         break;
//
//                     case "evt":
//                         attributeType = AttributeType.Event;
//                         break;
//
//                     case "ctx":
//
//                         attributeType = AttributeType.Context;
//
//                         if (name == "element" || name == "parent" || name == "root" || name == "evt") {
//                             ReportParseError($"`{name} is a reserved name and cannot be used as a context variable name");
//                             return;
//                         }
//
//                         break;
//
//                     case "var":
//                         attributeType = AttributeType.ImplicitVariable;
//
//                         if (name == "element" || name == "parent" || name == "root" || name == "evt") {
//                             ReportParseError($"`{name} is a reserved name and cannot be used as a context variable name");
//                             return;
//                         }
//
//                         break;
//
//                     case "sync":
//                         attributeType = AttributeType.Property;
//                         flags |= AttributeFlags.Sync;
//                         break;
//
//                     case "expose":
//                         attributeType = AttributeType.Expose;
//                         if (name == "element" || name == "parent" || name == "root" || name == "evt") {
//                             ReportParseError($"`{name} is a reserved name and cannot be used as a context variable name");
//                             return;
//                         }
//
//                         break;
//
//                     case "alias":
//                         attributeType = AttributeType.Alias;
//                         if (name == "element" || name == "parent" || name == "root" || name == "evt") {
//                             ReportParseError($"`{name} is a reserved name and cannot be used as a context variable name");
//                             return;
//                         }
//
//                         break;
//
//                     default:
//                         ReportParseError("Unknown attribute prefix: " + prefix);
//                         return;
//                 }
//             }
//
//             string raw = string.Empty;
//
//             attributes.Add(new AttributeDefinition(raw, attributeType, flags, name, value, shell, line, column));
//         }
//
//         private void ParseStyles(XElement root) {
//             IEnumerable<XElement> styleElements = root.GetChildren("Style");
//
//             foreach (XElement styleElement in styleElements) {
//                 if (TryParseStyle(styleElement, out PendingStyleDefinition styleDefinition)) {
//                     shell.styles.Add(styleDefinition);
//                 }
//             }
//         }
//
//         private bool TryParseStyle(XElement styleElement, out PendingStyleDefinition retn) {
//             XAttribute aliasAttr = styleElement.GetAttribute("as");
//             XAttribute importPathAttr = styleElement.GetAttribute("path") ?? styleElement.GetAttribute("src");
//
//             SetErrorContext(styleElement);
//             retn = default;
//
//             string rawText = string.Empty;
//             // styles can have either a class path or a body
//             foreach (XNode node in styleElement.Nodes()) {
//                 switch (node.NodeType) {
//                     case XmlNodeType.Text:
//                         rawText += ((XText) node).Value;
//                         continue;
//
//                     case XmlNodeType.Element:
//                         return ReportParseError("<Style> can only have text children, no elements");
//
//                     case XmlNodeType.Comment:
//                         continue;
//                 }
//
//                 return ReportParseError($"Unable to handle node type: {node.NodeType}");
//             }
//
//             string alias = PendingStyleDefinition.k_EmptyAliasName;
//             if (aliasAttr != null && !string.IsNullOrEmpty(aliasAttr.Value)) {
//                 alias = aliasAttr.Value.Trim();
//             }
//
//             // if we have a body, expect import path to be null
//             if (!string.IsNullOrEmpty(rawText) && !string.IsNullOrWhiteSpace(rawText)) {
//                 if (importPathAttr != null && !string.IsNullOrEmpty(importPathAttr.Value)) {
//                     return ReportParseError("Expected 'path' or 'src' to be null when a body is provided to a style tag");
//                 }
//
//                 retn = new PendingStyleDefinition(alias, FilePath + ".style", rawText);
//                 return true;
//             }
//
//             // if we have no body then expect path to be set
//             if (importPathAttr == null || string.IsNullOrEmpty(importPathAttr.Value)) {
//                 return ReportParseError("Expected 'path' or 'src' to be provided when a body is not provided in a style tag");
//             }
//
//             retn = new PendingStyleDefinition(alias, importPathAttr.Value.Trim());
//             return true;
//         }
//
//         private void ParseUsings(XElement root) {
//             IEnumerable<XElement> usingElements = root.GetChildren("Using");
//
//             foreach (XElement usingElement in usingElements) {
//                 if (TryParseUsing(usingElement, out UsingDeclaration declaration)) {
//                     shell.usings.Add(declaration);
//                 }
//             }
//
//             for (int i = 0; i < shell.usings.size; i++) {
//                 if (shell.usings.array[i].name != null) {
//                     shell.referencedNamespaces.Add(shell.usings.array[i].name);
//                 }
//             }
//         }
//
//         private bool TryParseUsing(XElement element, out UsingDeclaration retn) {
//             SetErrorContext(element);
//
//             XAttribute namespaceAttr = element.GetAttribute("namespace");
//             XAttribute elementAttr = element.GetAttribute("element");
//             XAttribute pathAttr = element.GetAttribute("path");
//             retn = default;
//
//             if (elementAttr != null || pathAttr != null) {
//                 if (elementAttr == null) {
//                     return ReportParseError("<Using> tag requires `element` attribute if `path` is provided");
//                 }
//
//                 if (pathAttr == null) {
//                     return ReportParseError("<Using> tag requires `path` attribute if `element` is provided");
//                 }
//
//                 retn = new UsingDeclaration() {
//                     name = elementAttr.Value.Trim(),
//                     pathName = pathAttr.Value.Trim(),
//                     type = UsingDeclarationType.Element,
//                     lineNumber = ((IXmlLineInfo) element).LineNumber
//                 };
//
//                 return true;
//             }
//
//             if (namespaceAttr == null) {
//                 return ReportParseError("<Using/> tags require a 'namespace' attribute");
//             }
//
//             string value = namespaceAttr.Value.Trim();
//             if (string.IsNullOrEmpty(value)) {
//                 return ReportParseError("<Using/> tags require a 'namespace' attribute with a value");
//             }
//
//             retn = new UsingDeclaration() {
//                 name = value,
//                 type = UsingDeclarationType.Namespace,
//                 lineNumber = ((IXmlLineInfo) element).LineNumber
//             };
//
//             return true;
//         }
//
//         public static string InvalidSlotOverride(string verb, TemplateNodeDebugData parentData, string childTagName) {
//             return $"Slot overrides can only be defined as a direct child of an expanded template. <{parentData.tagName}> is not an expanded template and cannot support slot {verb} <{childTagName}>";
//         }
//
//     }
//
// }