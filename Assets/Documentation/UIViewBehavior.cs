﻿using System;
using System.IO;
using UIForia.Elements;
using UIForia.Graphics;
using UIForia.Text;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace UIForia {

    public class UIViewBehavior : MonoBehaviour {

        public Type type;
        public string typeName;
        public new Camera camera;
        public Application application;
        public bool usePreCompiledTemplates;

        [HideInInspector] public string applicationName = "Game App 2";

        public TemplateSettings GetTemplateSettings(Type type) {
            TemplateSettings settings = new TemplateSettings();
            settings.rootType = type;
            settings.applicationName = applicationName;
            settings.assemblyName = "Assembly-CSharp";
            settings.outputPath = Path.Combine(UnityEngine.Application.dataPath, "UIForiaGenerated2");
            settings.codeFileExtension = "generated.cs";
            settings.preCompiledTemplatePath = "Assets/UIForia_Generated2/" + applicationName;
            settings.templateResolutionBasePath = Path.Combine(UnityEngine.Application.dataPath);

            return settings;
        }

        public void Start() {
#if UNITY_EDITOR
            AssertSize.AssertSizes();
#endif
            type = Type.GetType(typeName);
            if (type == null) return;

            TemplateSettings settings = GetTemplateSettings(type);
            // cmd = new CommandBuffer();
            // cmd.name = "DrawTest";
            // renderTexture = new RenderTexture(1024, 1024, 24, RenderTextureFormat.ARGB32);
            // mesh = new Mesh();
            // coverMesh = new Mesh();
#if UNITY_EDITOR
            application = usePreCompiledTemplates
                ? GameApplication.CreateFromPrecompiledTemplates(settings, camera, DoDependencyInjection)
                : GameApplication.CreateFromRuntimeTemplates(settings, camera, DoDependencyInjection);
#else
            application = GameApplication.CreateFromPrecompiledTemplates(settings, camera, DoDependencyInjection);
#endif

        }

        // optional!
        private void DoDependencyInjection(UIElement element) {
            // DiContainer.Inject(element);
        }

        private CommandBuffer commandBuffer;

        CommandBuffer cmd;
        private RenderTexture renderTexture;
        private Mesh mesh;
        private Mesh coverMesh;
        
        struct TextMaterialInfo {
                
            float zPosition;
            uint faceColor;
            uint outlineColor;
            uint glowColor;
            uint underlayColor;
            
            byte opacity;
            byte outlineWidth;
            byte outlineSoftness;
            byte faceDilate;
                
            byte glowOffset;
            byte glowOuter;
            byte glowInner;
            byte glowPower;
                
            ushort underlayX;
            ushort underlayY;
            ushort underlayDilate;
            ushort underlaySoftness;
            
            
                
        }
        public struct Character {

            // public int glyphRenderId; // could lookup, maybe no need to store per character. trade off is a dictionary or array lookup before rendering
            public ushort flags;
            public ushort codepoint;
            public float2 position;

            public ushort effectId;
            public ushort matrixId;
            public ushort vertexId;
            public ushort materialId; 

            // computable
      //      public int wordIndex;
      //      public ushort lineIndex;
      //      public ushort charIndexInWord;

        }
        
        unsafe void OnPreRender() {
            unsafe {
               // Debug.Log(sizeof(BurstCharInfo));
            }
            if (application != null && commandBuffer != null) {
                commandBuffer.Clear();
                application.Render(camera.pixelWidth, camera.pixelHeight, commandBuffer);
            }
            
        }

        private void Update() {

            if (application != null) {
                if (commandBuffer == null) {
                    commandBuffer = new CommandBuffer();
                    commandBuffer.name = "UIForia";
                    camera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, commandBuffer);
                }

                application.DPIScaleFactor = 1; // todo -- remove this
                application.Update();

            }

            // cmd.Clear();
            // cmd.SetRenderTarget(renderTexture);
            // cmd.ClearRenderTarget(true, true, Color.white);
            // float halfWidth = renderTexture.width * 0.5f;
            // float halfHeight = renderTexture.height * 0.5f;
            //
            // Matrix4x4 origin = Matrix4x4.TRS(new Vector3(-halfWidth, halfHeight, 0), Quaternion.identity, Vector3.one);
            // cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.Ortho(-halfWidth, halfWidth, -halfHeight, halfHeight, -100, 100));
            //
            // UIVertexHelper vertexHelper = UIVertexHelper.Create(Allocator.Temp);
            //
            // using (ShapeKit shapeKit = new ShapeKit(Allocator.Temp)) {
            //
            //     // Set BlendOp to min
            //     // Set Blend params to One One
            //     // Set ZWrite On
            //     // Set ZTest to NotEqual
            //     // set depth to # of parent masks
            //     // draw parent shapes top down
            //     // subtract 1 from depth each time
            //     // draw child shape at depth 0
            //     // draw a rect over the whole space
            //     int start = vertexHelper.currentVertCount;
            //     shapeKit.AddQuad(ref vertexHelper, 0, 0, 256, 256, Color.green);
            //     for (int i = start; i < vertexHelper.currentVertCount; i++) {
            //         vertexHelper.positions[i].z = -2f;
            //     }
            //
            //     start = vertexHelper.currentVertCount;
            //     shapeKit.AddQuad(ref vertexHelper, 32, 32, 256 - 48, 256 - 48, new Color32(0, 0, 0, 128));
            //
            //     for (int i = start; i < vertexHelper.currentVertCount; i++) {
            //         vertexHelper.positions[i].z = -1f;
            //     }
            //     shapeKit.SetDpiScale(1);
            //     shapeKit.SetAntiAliasWidth(0);//1.25f);
            //     shapeKit.AddCircle(ref vertexHelper, new Rect(128, 128, 256, 256), new EllipseProperties() {
            //         fitting = EllipseFitting.Ellipse,
            //     }, Color.red);
            //
            //
            //     //vertexHelper.Clear();
            //     shapeKit.AddRect(ref vertexHelper, 0, 0, 256, 256, new Color32(0, 0, 0, 0));
            //     vertexHelper.FillMesh(mesh);
            //
            // }
            //
            // vertexHelper.Dispose();
            // cmd.DrawMesh(mesh, origin, Resources.Load<Material>("UIForiaShape"));
            // // cmd.DrawMesh(coverMesh, origin, Resources.Load<Material>("UIForiaClipBlend"));
            // UnityEngine.Graphics.ExecuteCommandBuffer(cmd);

        }

        public void OnApplicationQuit() {
            application?.Dispose();
        }

    }

}