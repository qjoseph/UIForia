using System;
using System.Collections.Generic;
using System.Linq;
using UIForia.Compilers;
using UIForia.Elements;
using UIForia.Exceptions;
using UIForia.Parsing;
using UIForia.Src;
using UIForia.Style;
using UIForia.Style2;
using UIForia.Util;
using UnityEngine;

namespace UIForia {

    public class ModuleLoadException : Exception {

        public ModuleLoadException(string message) : base(message) { }

    }

    public abstract class Module : TypeProcessor.IDiagnosticProvider {

        public readonly Type type;
        public readonly bool IsBuiltIn;

        private LightList<StyleSheet2> styleSheets;

        private List<bool> conditionResults;
        private IList<StyleCondition> styleConditions;
        private Func<TemplateLookup, TemplateLocation?> templateResolver;

        internal readonly IList<ModuleReference> dependencies;
        internal readonly PagedLightList<ProcessedType> elementTypes;
        internal readonly Dictionary<string, ProcessedType> tagNameMap;
        internal readonly Dictionary<string, TemplateShell> shellMap;

        private readonly LightList<TemplateShell> templateShells;

        private string assetPath;
        private string moduleName;

        public string location { get; internal set; }

        protected Module() {

            if (!ModuleSystem.s_ConstructionAllowed) {
                throw new ModuleLoadException("Modules should never have their constructor called.");
            }

            this.type = GetType();
            this.moduleName = type.Name;
            this.dependencies = new List<ModuleReference>();
            this.templateShells = new LightList<TemplateShell>();
            this.IsBuiltIn = type == typeof(BuiltInElementsModule);
            this.shellMap = new Dictionary<string, TemplateShell>();
            this.elementTypes = new PagedLightList<ProcessedType>(32);
            this.tagNameMap = new Dictionary<string, ProcessedType>(31);
        }

        // internal Action zz__INTERNAL_DO_NOT_CALL; // use this for precompiled loading instead of doing type reflection to find caller type

        public virtual void Configure() { }

        protected void SetModuleName(string moduleName) {
            if (moduleName == null) {
                moduleName = GetType().Name;
            }

            this.moduleName = moduleName;
        }

        protected void AddDependency<TDependency>(string alias = null) where TDependency : Module, new() {
            if (typeof(TDependency).IsAbstract) {
                throw new InvalidArgumentException("Dependencies must be concrete classes. " + TypeNameGenerator.GetTypeName(typeof(TDependency)) + " is abstract");
            }

            dependencies.Add(new ModuleReference(typeof(TDependency), alias));
        }

        public virtual void BuildCustomStyles(IStyleCodeGenerator generator) { }

        protected void SetTemplateResolver(Func<TemplateLookup, TemplateLocation?> resolver) {
            this.templateResolver = resolver;
        }

        internal TemplateLocation? ResolveTemplatePath(TemplateLookup typeLookup) {
            if (templateResolver != null) {
                return templateResolver(typeLookup);
            }

            return new TemplateLocation(location + typeLookup.declaredTemplatePath, typeLookup.templateId);
        }

        protected internal void UpdateConditions(DisplayConfiguration displayConfiguration) {
            if (styleConditions == null) return;

            conditionResults = conditionResults ?? new List<bool>();
            conditionResults.Clear();

            for (int i = 0; i < styleConditions.Count; i++) {
                conditionResults.Add(styleConditions[i].fn(displayConfiguration));
            }
        }

        public ModuleCondition GetDisplayConditions() {
            return default;
        }

        public int GetDisplayConditionId(CharSpan conditionSpan) {
            if (styleConditions == null) return -1;
            for (int i = 0; i < styleConditions.Count; i++) {
                if (styleConditions[i].name == conditionSpan) {
                    return i;
                }
            }

            return -1;
        }

        public bool HasStyleCondition(string conditionName) {
            return HasStyleCondition(new CharSpan(conditionName));
        }

        public bool HasStyleCondition(CharSpan conditionName) {
            if (styleConditions == null) return false;
            for (int i = 0; i < styleConditions.Count; i++) {
                if (styleConditions[i].name == conditionName) {
                    return true;
                }
            }

            return false;
        }

        public Module GetDependency(string moduleName) {
            for (int i = 0; i < dependencies.Count; i++) {
                if (dependencies[i].GetAlias() == moduleName) {
                    return dependencies[i].GetModuleInstance();
                }
            }

            return null;
        }

        protected internal void RegisterDisplayCondition(string condition, Func<DisplayConfiguration, bool> fn) {
            if (styleConditions == null) {
                styleConditions = new List<StyleCondition>() {
                    new StyleCondition(0, condition, fn)
                };
                return;
            }

            for (int i = 0; i < styleConditions.Count; i++) {
                if (styleConditions[i].name == condition) {
                    styleConditions[i] = new StyleCondition(i, condition, fn);
                    return;
                }
            }

            styleConditions.Add(new StyleCondition(styleConditions.Count, condition, fn));
        }

        internal ProcessedType ResolveTagName(string moduleName, string tagName, TypeProcessor.DiagnosticWrapper diagnosticWrapper) {

            ProcessedType retn;

            if (string.IsNullOrEmpty(moduleName)) {
                // must be in this module or default.
                if (!IsBuiltIn) {
                    if (tagNameMap.TryGetValue(tagName, out retn)) {
                        return retn;
                    }
                }

                return ModuleSystem.BuiltInModule.tagNameMap.TryGetValue(tagName, out retn) ? retn : null;

            }

            // todo -- if we support <Using module="x" as="y"/> do the resolution here
            Module module = GetDependency(moduleName);

            if (module == null) {
                List<string> list = dependencies.Select(d => d.GetAlias()).ToList();
                diagnosticWrapper.AddDiagnostic($"Unable to resolve module `{moduleName}`. Available module names from current module ({GetType().GetTypeName()}) are {StringUtil.ListToString(list)}");
                return null;
            }

            if (module.tagNameMap.TryGetValue(tagName, out retn)) {
                return retn;
            }

            diagnosticWrapper.AddDiagnostic($"Unable to resolve tag name `{tagName}` from module {moduleName}.");
            return null;
        }

        public void AddDiagnostic(string message) { }

        public string GetModuleName() {
            return moduleName;
        }

        // doesn't resolve closed generics! We expect the compiler to handle this based on the open types
        internal ReadOnlySizedArray<ProcessedType> GetTemplateElements() {

            ListPage<ProcessedType> ptr = elementTypes.head;
            
            // todo cache this?
            SizedArray<ProcessedType> retn = new SizedArray<ProcessedType>(elementTypes.size / 2);

            while (ptr != null) {

                for (int j = 0; j < ptr.size; j++) {
                    ProcessedType t = ptr.data[j];

                    if (t.resolvedTemplateLocation != null) {

                        elementTypes.Add(t);

                    }

                }

                ptr = ptr.next;
            }

            return retn;

        }

        internal ReadOnlySizedArray<TemplateShell> GetTemplateShells() {

            // todo -- maybe include a setting that won't cache these 
            // UseTemplateResolveCache(false);
            if (templateShells.Count != 0) {
                return new ReadOnlySizedArray<TemplateShell>(templateShells.size, templateShells.array);
            }

            ListPage<ProcessedType> ptr = elementTypes.head;

            while (ptr != null) {

                for (int j = 0; j < ptr.size; j++) {
                    ProcessedType t = ptr.data[j];

                    if (t.resolvedTemplateLocation != null) {

                        if (!shellMap.TryGetValue(t.resolvedTemplateLocation.Value.filePath, out TemplateShell shell)) {
                            shell = new TemplateShell(this, t.resolvedTemplateLocation.Value.filePath);
                            templateShells.Add(shell);
                            shellMap[t.resolvedTemplateLocation.Value.filePath] = shell;
                        }

                    }

                }

                ptr = ptr.next;
            }

            return new ReadOnlySizedArray<TemplateShell>(templateShells.size, templateShells.array);
        }

        internal void SetTemplateData(Type type, TemplateData templateData) {
            // if moduleData == null
            // moduleData = new ModuleData();
            // moduleData.SetTemplateData(type, templateData);
        }

        public void MatchElementTypesToTemplateNodes() {
            ListPage<ProcessedType> ptr = elementTypes.head;

            while (ptr != null) {

                for (int j = 0; j < ptr.size; j++) {
                    ProcessedType t = ptr.data[j];

                    // todo -- are generics a problem here?
                    if (t.resolvedTemplateLocation != null) {

                        if (shellMap.TryGetValue(t.resolvedTemplateLocation.Value.filePath, out TemplateShell templateShell)) {
                            t.templateRootNode = templateShell.GetTemplateRoot(t.resolvedTemplateLocation.Value.templateId);
                        }
                        else {
                            t.templateRootNode = null;
                            Debug.Log($"Unable to find template for {t.rawType}");
                        }

                    }

                }

                ptr = ptr.next;
            }
        }

        public void AddElementType(ProcessedType processedType) {
            elementTypes.Add(processedType);
        }

    }

    internal class TemplateDataComparer : IComparer<TemplateData> {

        public static readonly TemplateDataComparer Instance = new TemplateDataComparer();

        public int Compare(TemplateData x, TemplateData y) {
            return string.CompareOrdinal(x.templateId, y.templateId);
        }

    }

    public class ModuleData<T> where T : Module {

        private static bool sorted;
        private static SizedArray<TemplateData> templateDataList;

        public static TemplateData GetTemplateData(string templateId) {

            if (!sorted) {
                sorted = true;
                Array.Sort(templateDataList.array, 0, templateDataList.size, TemplateDataComparer.Instance);
            }

            int start = 0;
            int end = templateDataList.size - 1;

            TemplateData[] array = templateDataList.array;

            while (start <= end) {
                int index = start + (end - start >> 1);

                int cmp = string.CompareOrdinal(array[index].templateId, templateId);

                if (cmp == 0) {
                    return array[index];
                }

                if (cmp < 0) {
                    start = index + 1;
                }
                else {
                    end = index - 1;
                }
            }

            return null;

        }

    }

}