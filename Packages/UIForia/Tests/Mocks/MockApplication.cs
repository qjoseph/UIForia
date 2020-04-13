using System;
using System.Collections.Generic;
using System.IO;
using UIForia;
using UIForia.Compilers;
using UIForia.Elements;
using UIForia.Systems;
using UnityEngine;
using Application = UIForia.Application;

namespace Tests.Mocks {

    public class MockApplication : Application {

        public static bool s_GenerateCode;
        public static bool s_UsePreCompiledTemplates;

        protected MockApplication(in ApplicationConfig config) : base(config) { }

        public static void Generate(bool shouldGenerate = true) {
            s_GenerateCode = shouldGenerate;
            s_UsePreCompiledTemplates = shouldGenerate;
        }

        public static Application Create<T>(TemplateLoader loader) where T : UIElement {
       
            ApplicationConfig config = new ApplicationConfig() {
                applicationType = ApplicationType.Test,
                templateLoader = loader
            };
            
            return Create(config);
            
        }
        
        public static Application Create<T>() where T : UIElement {
       
            ApplicationConfig config = new ApplicationConfig() {
                applicationType = ApplicationType.Test,
                templateLoader = TemplateLoader.RuntimeCompile(typeof(T))
            };
            
            return Create(config);
            
        }
        
        public static void PreCompile<T>(string appName) where T: UIElement, new() {
            string path = Path.GetFullPath(Path.Combine(UnityEngine.Application.dataPath, "..", "Packages", "UIForia", "Tests", "UIForiaGeneratedNew", appName));
            TemplateLoader.PreCompile(path, appName, typeof(T));
            
        }

        public static Application Create(TemplateLoader loader) {
            
            ApplicationConfig config = new ApplicationConfig() {
                applicationType = ApplicationType.Test,
                templateLoader = loader,
            };
            
            return Create(config);

        }
        
        public static MockApplication Setup<T>(string appName = null, List<Type> dynamicTemplateTypes = null) where T : UIElement {
            throw new NotImplementedException();
            // if (appName == null) {
            //     StackTrace stackTrace = new StackTrace();
            //     appName = stackTrace.GetFrame(1).GetMethod().Name;
            // }
            //
            // TemplateSettings settings = new TemplateSettings();
            // settings.applicationName = appName;
            // settings.templateRoot = "Data";
            // settings.assemblyName = typeof(MockApplication).Assembly.GetName().Name;
            // settings.outputPath = Path.Combine(UnityEngine.Application.dataPath, "..", "Packages", "UIForia", "Tests", "UIForiaGenerated");
            // settings.codeFileExtension = ".generated.xml.cs";
            // settings.preCompiledTemplatePath = "Assets/UIForia_Generated/" + appName;
            // settings.templateResolutionBasePath = Path.Combine(UnityEngine.Application.dataPath, "..", "Packages", "UIForia", "Tests");
            // settings.rootType = typeof(T);
            // settings.dynamicallyCreatedTypes = dynamicTemplateTypes;
            //
            // if (s_GenerateCode) {
            //     TemplateCodeGenerator.Generate(typeof(T), settings);
            // }
            //
            // Module module = ModuleSystem.LoadRootModule(typeof(T));
            //
            // MockApplication app = new MockApplication(s_UsePreCompiledTemplates, module, settings, null, null);
            // app.Initialize();
            // return app;
        }
        
        public new MockInputSystem InputSystem => (MockInputSystem) inputSystem;
        public UIElement RootElement => views[0].RootElement;

        public void SetViewportRect(Rect rect) {
            views[0].Viewport = rect;
        }


    }

}