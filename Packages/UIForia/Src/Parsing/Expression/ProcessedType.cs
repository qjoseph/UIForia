using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using UIForia.Attributes;
using UIForia.Compilers;
using UIForia.Elements;
using UIForia.Src;
using UIForia.Util;
using UnityEngine.Assertions;

namespace UIForia.Parsing {

    [DebuggerDisplay("{rawType.Name}")]
    public class ProcessedType {

        public readonly Type rawType;

        public readonly int id;

        internal string tagName;
        internal string elementPath;
        internal string templatePath;
        internal string templateId;
        internal string implicitStyles;
        internal string[] importedStyleSheets;
        private bool gotReflection;

        public MethodInfo updateMethod;
        public MethodInfo createMethod;
        private Expression ctorExpr;

        private Flags flags;
        internal Module module;
        internal TemplateRootNode templateRootNode;
        internal TemplateLocation? resolvedTemplateLocation;
        
        private ConstructorInfo ctor;
        
        private ReadOnlySizedArray<PropertyChangeHandlerDesc> changeHandlers;
        
        private static int currentTypeId = -1;
        
        internal DateTime lastTemplateParseTime;
        public TemplateFileShell templateFileShell;

        [Flags]
        private enum Flags {

            RequiresBeforePropertyUpdates = 1 << 0,
            RequiresAfterPropertyUpdates = 1 << 1,
            RequiresOnEnable = 1 << 2,
            RequiresUpdateFn = 1 << 3,
            IsDynamic = 1 << 4,
            IsUnresolvedGeneric = 1 << 5,
            IsContainerElement = 1 << 6,
            IsTextElement = 1 << 7

        }

        internal ProcessedType(Type rawType, string elementPath, string templatePath, string templateId, string tagName, string implicitStyles, string[] styleSheets) {
            this.id = Interlocked.Add(ref currentTypeId, 1);
            this.rawType = rawType;
            this.tagName = tagName;
            this.elementPath = elementPath;
            this.templatePath = templatePath;
            this.templateId = templateId;
            this.implicitStyles = implicitStyles;
            this.importedStyleSheets = styleSheets;

            if (templatePath == null && templateId == null) {
                IsContainerElement = true;
            }

            if (typeof(UITextElement).IsAssignableFrom(rawType)) {
                flags |= Flags.IsTextElement;
            }

            this.IsUnresolvedGeneric = rawType.IsGenericTypeDefinition;
            
            // todo -- this is really expensive, consider deferring until we actually need it in the template compiler.
            // this.requiresUpdateFn = ReflectionUtil.IsOverride(rawType.GetMethod(nameof(UIElement.OnUpdate), BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null));
            // this.requiresOnEnable = ReflectionUtil.IsOverride(rawType.GetMethod(nameof(UIElement.OnEnable), BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null));
            // this.requiresBeforePropertyUpdates = ReflectionUtil.IsOverride(rawType.GetMethod(nameof(UIElement.OnBeforePropertyBindings), BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null));
            // this.requiresAfterPropertyUpdates = ReflectionUtil.IsOverride(rawType.GetMethod(nameof(UIElement.OnAfterPropertyBindings), BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null));
        }

        public bool requiresUpdateFn {
            get => (flags & Flags.RequiresUpdateFn) != 0;
            private set {
                if (value) {
                    flags |= Flags.RequiresUpdateFn;
                }
                else {
                    flags &= ~Flags.RequiresUpdateFn;
                }
            }
        }

        public bool requiresOnEnable {
            get => (flags & Flags.RequiresOnEnable) != 0;
            private set {
                if (value) {
                    flags |= Flags.RequiresOnEnable;
                }
                else {
                    flags &= ~Flags.RequiresOnEnable;
                }
            }
        }

        public bool requiresBeforePropertyUpdates {
            get => (flags & Flags.RequiresBeforePropertyUpdates) != 0;
            private set {
                if (value) {
                    flags |= Flags.RequiresBeforePropertyUpdates;
                }
                else {
                    flags &= ~Flags.RequiresBeforePropertyUpdates;
                }
            }
        }

        public bool isDynamic {
            get => (flags & Flags.IsDynamic) != 0;
            set {
                if (value) {
                    flags |= Flags.IsDynamic;
                }
                else {
                    flags &= ~Flags.IsDynamic;
                }
            }
        }

        public bool requiresAfterPropertyUpdates {
            get => (flags & Flags.RequiresAfterPropertyUpdates) != 0;
            set {
                if (value) {
                    flags |= Flags.RequiresAfterPropertyUpdates;
                }
                else {
                    flags &= ~Flags.RequiresAfterPropertyUpdates;
                }
            }
        }

        public bool IsUnresolvedGeneric {
            get => (flags & Flags.IsUnresolvedGeneric) != 0;
            set {
                if (value) {
                    flags |= Flags.IsUnresolvedGeneric;
                }
                else {
                    flags &= ~Flags.IsUnresolvedGeneric;
                }
            }
        }

        public bool IsContainerElement {
            get => (flags & Flags.IsContainerElement) != 0;
            set {
                if (value) {
                    flags |= Flags.IsContainerElement;
                }
                else {
                    flags &= ~Flags.IsContainerElement;
                }
            }
        }

        public bool IsTextElement {
            get => (flags & Flags.IsTextElement) != 0;
        }

        public bool DeclaresTemplate {
            get => templatePath != null;
        }

        public void GetChangeHandlers(string memberName, StructList<PropertyChangeHandlerDesc> retn) {
            // if (methods == null) {
            //     MethodInfo[] candidates = ReflectionUtil.GetInstanceMethods(rawType);
            //     for (int i = 0; i < candidates.Length; i++) {
            //         IEnumerable<OnPropertyChanged> attrs = candidates[i].GetCustomAttributes<OnPropertyChanged>();
            //         methods = methods ?? new StructList<PropertyChangeHandlerDesc>();
            //         foreach (OnPropertyChanged a in attrs) {
            //             methods.Add(new PropertyChangeHandlerDesc() {
            //                 methodInfo = candidates[i],
            //                 memberName = a.propertyName
            //             });
            //         }
            //     }
            // }
            //
            // if (methods == null) {
            //     return;
            // }
            //
            // for (int i = 0; i < methods.size; i++) {
            //     if (methods.array[i].memberName == memberName) {
            //         retn.Add(methods[i]);
            //     }
            // }

            for (int i = 0; i < changeHandlers.size; i++) {
                if (changeHandlers.array[i].memberName == memberName) {
                    retn.Add(changeHandlers.array[i]);
                }
            }
            
        }
        
        public static ProcessedType CreateFromGenericDefinition(Type currentType) {
            Assert.IsTrue(currentType.IsGenericTypeDefinition);
            throw new NotImplementedException();
        }
        
        public static ProcessedType ResolveGeneric(Type resolvedType, ProcessedType generic) {
            ProcessedType retn = new ProcessedType(resolvedType, generic.elementPath, generic.templatePath, generic.templateId, generic.tagName, generic.implicitStyles, generic.importedStyleSheets);
            retn.templateRootNode = generic.templateRootNode;
            retn.module = generic.module;
            return retn;
        }

        public static ProcessedType CreateFromDynamicType(Type type, string templateShellFilePath, string nodeTemplateId) {
            throw new NotImplementedException();
        }

        internal static ProcessedType CreateFromType(Type type) {
            LightList<string> styles = null;
            string[] styleSheets = null;

            string tagName = type.Name;
            string elementPath = null;
            string implicitStyleNames = null;
            string templatePath = null;
            string templateId = null;

            bool templateProvided = false;
            bool isContainer = false;

            if (type.IsGenericTypeDefinition) {
                tagName = tagName.Split('`')[0];
            }
            
            // this could probably be done the other way around, use type cache to get all types with attributes and blast through each one
            
            foreach (Attribute attr in type.GetCustomAttributes()) {
                
                switch (attr) {
                    
                    case TemplateTagNameAttribute templateTagNameAttr when elementPath != null && elementPath != templateTagNameAttr.filePath:
                        ModuleSystem.LogDiagnosticError($"File paths were different {elementPath}, {templateTagNameAttr.filePath} for element type {TypeNameGenerator.GetTypeName(type)}");
                        return null;

                    case TemplateTagNameAttribute templateTagNameAttr:
                        tagName = templateTagNameAttr.tagName;
                        elementPath = templateTagNameAttr.filePath;
                        continue;

                    case TemplateAttribute templateAttribute when elementPath != null && elementPath != templateAttribute.elementPath:
                        ModuleSystem.LogDiagnosticError($"File paths were different {elementPath}, {templateAttribute.elementPath} for element type {TypeNameGenerator.GetTypeName(type)}");
                        return null;

                    case TemplateAttribute templateAttribute: {
                        elementPath = templateAttribute.elementPath;
                        templatePath = templateAttribute.templatePath;
                        templateId = templateAttribute.templateId;
                    
                        if (isContainer) {
                            ModuleSystem.LogDiagnosticError($"Element cannot be a container and provide a template. {TypeNameGenerator.GetTypeName(type)} is both.");
                            return null;
                        }

                        templateProvided = true;

                        continue;
                    }

                    case RecordFilePathAttribute recordFilePathAttribute when elementPath != null && elementPath != recordFilePathAttribute.filePath:
                        ModuleSystem.LogDiagnosticError($"File paths were different {elementPath}, {recordFilePathAttribute.filePath} for element type {TypeNameGenerator.GetTypeName(type)}");
                        return null;

                    case RecordFilePathAttribute recordFilePathAttribute:
                        elementPath = recordFilePathAttribute.filePath;
                        continue;

                    case ImportStyleSheetAttribute importStyleSheetAttribute when elementPath != null && elementPath != importStyleSheetAttribute.filePath:
                        ModuleSystem.LogDiagnosticError($"File paths were different {elementPath}, {importStyleSheetAttribute.filePath} for element type {TypeNameGenerator.GetTypeName(type)}");
                        return null;

                    case ImportStyleSheetAttribute importStyleSheetAttribute:
                        styles = styles ?? LightList<string>.Get();
                        styles.Add(importStyleSheetAttribute.styleSheet);
                        elementPath = importStyleSheetAttribute.filePath;
                        continue;

                    case StyleAttribute styleAttribute when elementPath != null && elementPath != styleAttribute.filePath:
                        ModuleSystem.LogDiagnosticError($"File paths were different {elementPath}, {styleAttribute.filePath} for element type {TypeNameGenerator.GetTypeName(type)}");
                        return null;

                    case StyleAttribute styleAttribute:
                        elementPath = styleAttribute.filePath;
                        implicitStyleNames = styleAttribute.styleNames;
                        break;

                    case ContainerElementAttribute containerAttr when elementPath != null && elementPath != containerAttr.filePath:
                        ModuleSystem.LogDiagnosticError($"File paths were different {elementPath}, {containerAttr.filePath} for element type {TypeNameGenerator.GetTypeName(type)}");
                        return null;

                    case ContainerElementAttribute containerAttr: {
                        if (templateProvided) {
                            ModuleSystem.LogDiagnosticError($"Element cannot be a container and provide a template. {TypeNameGenerator.GetTypeName(type)} is both.");
                            return null;
                        }
                    
                        elementPath = containerAttr.filePath;
                        isContainer = true;
                        break;
                    }
                }

            }

            if (styles != null) {
                styleSheets = styles.ToArray();
                styles.Release();
            }

            return new ProcessedType(type, elementPath, templatePath, templateId, tagName, implicitStyleNames, styleSheets);
        }
        
        public ConstructorInfo GetConstructor() {
            if (ctor == null) {
                ctor = rawType.GetConstructor(Type.EmptyTypes);
            }
            return ctor;
        }

        public void EnsureReflectionData() {
            if (gotReflection) return;
            gotReflection = true;
            // threadsafe in that we dont care about duplicating the work since its always the same result if run twice
            updateMethod = rawType.GetMethod(nameof(UIElement.OnUpdate), Type.EmptyTypes);
            createMethod = rawType.GetMethod(nameof(UIElement.OnCreate), Type.EmptyTypes);
            requiresUpdateFn = ReflectionUtil.IsOverride(updateMethod);
            changeHandlers = TypeProcessor.GetChangeHandlers(rawType);
        }

        public bool TryGetChangeHandlers(string key, PropertyChangedType changeType, StructList<PropertyChangeHandlerDesc> results) {
            EnsureReflectionData();
            results.size = 0;
            
            for (int i = 0; i < changeHandlers.size; i++) {
                ref PropertyChangeHandlerDesc handler = ref changeHandlers.array[i];
                if ((handler.changeType & changeType) != 0 && handler.memberName == key) {
                    results.Add(handler);
                }
            }

            return results.size != 0;
        }

        
        public Expression GetConstructorExpression() {
            if (ctorExpr == null) {
                ctorExpr = ExpressionFactory.New(GetConstructor());
            }

            return ctorExpr;
        }

    }

    public struct PropertyChangeHandlerDesc {

        public MethodInfo methodInfo;
        public ParameterInfo[] parameterInfos;
        public string memberName;
        public PropertyChangedType changeType;

    }

}