﻿using UIForia.Graphics;
using UIForia.Util;
using UIForia.Util.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace UIForia.Systems {

    internal unsafe struct MergeRenderContexts_Managed : IJob {

        public DataList<DrawInfo>.Shared drawList;
        public GCHandleList<Mesh> meshListHandle;
        public PerThreadObjectPool<RenderContext2> contextPoolHandle;

        public void Execute() {

            ThreadSafePool<RenderContext2> pool = contextPoolHandle.GetPool();
            LightList<Mesh> meshList = meshListHandle.Get();
            
            meshList.QuickClear();
            
            int drawListSize = 0;
            int meshListSize = 0;
            
            for (int i = 0; i < pool.perThreadData.Length; i++) {
                RenderContext2 renderContext = pool.perThreadData[i];

                if (renderContext == null) {
                    continue;
                }

                drawListSize += renderContext.drawList.size;
                meshListSize += renderContext.meshList.size;
                
            }

            drawList.EnsureCapacity(drawListSize);
            meshList.EnsureCapacity(meshListSize);

            for (int i = 0; i < pool.perThreadData.Length; i++) {
                RenderContext2 renderContext = pool.perThreadData[i];

                if (renderContext == null) {
                    continue;
                }

                drawList.AddRange(renderContext.drawList.array, renderContext.drawList.size);
                meshList.AddRange(renderContext.meshList);

            }
            
        }

    }

}