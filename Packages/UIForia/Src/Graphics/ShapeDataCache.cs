using UIForia.Text;
using UIForia.Util;
using Unity.Mathematics;
using UnityEngine;

namespace Vertigo {

    public class ShapeDataCache {

        public StructList<GeometryShape> shapes;

        public StructList<Vector3> positionList;
        public StructList<Vector3> normalList;
        public StructList<Color> colorList;
        public StructList<Vector4> texCoordList0;
        public StructList<Vector4> texCoordList1;
        public StructList<Vector4> texCoordList2;
        public StructList<Vector4> texCoordList3;
        public StructList<int> triangleList;

        public int shapeCount => shapes.size;
        public int vertexCount => positionList.size;
        public int triangleCount => triangleList.size;

        public ShapeDataCache(int capacity = 8) {
            shapes = new StructList<GeometryShape>();
            positionList = new StructList<Vector3>(capacity);
            normalList = new StructList<Vector3>(capacity);
            colorList = new StructList<Color>(capacity);
            texCoordList0 = new StructList<Vector4>(capacity);
            texCoordList1 = new StructList<Vector4>(capacity);
            texCoordList2 = new StructList<Vector4>(capacity);
            texCoordList3 = new StructList<Vector4>(capacity);
            triangleList = new StructList<int>(capacity * 2);
        }

        private void EnsureAdditionalCapacity(int vertCount, int triCount) {
            int requiredSize = positionList.size + vertCount;

            if (requiredSize >= positionList.array.Length) {
                requiredSize *= 2;
                System.Array.Resize(ref positionList.array, requiredSize);
                System.Array.Resize(ref normalList.array, requiredSize);
                System.Array.Resize(ref texCoordList0.array, requiredSize);
                System.Array.Resize(ref texCoordList1.array, requiredSize);
                System.Array.Resize(ref texCoordList2.array, requiredSize);
                System.Array.Resize(ref texCoordList3.array, requiredSize);
                System.Array.Resize(ref colorList.array, requiredSize);
            }

            triangleList.EnsureAdditionalCapacity(triCount);
        }

        public GeometryShape FillRect(float x, float y, float width, float height, in RenderState renderState) {
            Color color = renderState.fillColor;
            EnsureAdditionalCapacity(4, 6);

            Vector3 p0 = new Vector3(x, -y);
            Vector3 p1 = new Vector3(x + width, -y);
            Vector3 p2 = new Vector3(x + width, -(y + height));
            Vector3 p3 = new Vector3(x, -(y + height));

            Vector3 n0 = new Vector3(0, 0, -1);

            int startVert = vertexCount;
            int startTriangle = triangleCount;

            Vector3[] positions = positionList.array;
            Vector3[] normals = normalList.array;
            Color[] colors = colorList.array;
            int[] triangles = triangleList.array;

            positions[startVert + 0] = p0;
            positions[startVert + 1] = p1;
            positions[startVert + 2] = p2;
            positions[startVert + 3] = p3;

            normals[startVert + 0] = n0;
            normals[startVert + 1] = n0;
            normals[startVert + 2] = n0;
            normals[startVert + 3] = n0;

            colors[startVert + 0] = color;
            colors[startVert + 1] = color;
            colors[startVert + 2] = color;
            colors[startVert + 3] = color;

            Vector4[] uvChannel = GetUVChannel(renderState.texCoordChannel);

            if (uvChannel != null) {
                uvChannel[startVert + 0] = new Vector4(0, 1);
                uvChannel[startVert + 1] = new Vector4(1, 1);
                uvChannel[startVert + 2] = new Vector4(1, 0);
                uvChannel[startVert + 3] = new Vector4(0, 0);
                ComputeUVTransforms(renderState, uvChannel, new RangeInt(startVert, 4));
            }

            triangles[startTriangle + 0] = startVert + 0;
            triangles[startTriangle + 1] = startVert + 1;
            triangles[startTriangle + 2] = startVert + 2;
            triangles[startTriangle + 3] = startVert + 2;
            triangles[startTriangle + 4] = startVert + 3;
            triangles[startTriangle + 5] = startVert + 0;

            UpdateSizes(4, 6);

            GeometryShape retn = new GeometryShape() {
                geometryType = GeometryType.Physical,
                shapeType = ShapeType.Rect,
                vertexStart = startVert,
                vertexCount = 4,
                triangleStart = startTriangle,
                triangleCount = 6
            };

            shapes.Add(retn);

            return retn;
        }

        private Vector4[] GetUVChannel(TextureCoordChannel texCoordChannel) {
            switch (texCoordChannel) {
                case TextureCoordChannel.TextureCoord0:
                    return texCoordList0.array;
                case TextureCoordChannel.TextureCoord1:
                    return texCoordList1.array;
                case TextureCoordChannel.TextureCoord2:
                    return texCoordList2.array;
                case TextureCoordChannel.TextureCoord3:
                    return texCoordList3.array;
            }

            return null;
        }

        private void UpdateSizes(int vertCount, int triCount) {
            positionList.size += vertCount;
            normalList.size += vertCount;
            colorList.size += vertCount;
            texCoordList0.size += vertCount;
            texCoordList1.size += vertCount;
            texCoordList2.size += vertCount;
            texCoordList3.size += vertCount;
            triangleList.size += triCount;
        }

        private static void ComputeUVTransforms(in RenderState renderState, Vector4[] uvs, RangeInt range) {
            float tileX = renderState.uvTiling.x;
            float tileY = renderState.uvTiling.y;
            float offsetX = renderState.uvOffset.x;
            float offsetY = renderState.uvOffset.y;

            int start = range.start;
            int end = range.end;

            if (renderState.uvRotation == 0) {
                float minX = renderState.uvRect.x;
                float minY = renderState.uvRect.y;
                float width = renderState.uvRect.width;
                float height = renderState.uvRect.height;

                for (int i = start; i < end; i++) {
                    uvs[i].x = minX + (((uvs[i].x * tileX) + offsetX) * width);
                    uvs[i].y = minY + (((uvs[i].y * tileY) + offsetY) * height);
                }
            }
            else {
                float sin = math.sin(renderState.uvRotation * Mathf.Deg2Rad);
                float cos = math.cos(renderState.uvRotation * Mathf.Deg2Rad);
                float minX = renderState.uvRect.x;
                float minY = renderState.uvRect.y;
                float width = renderState.uvRect.width;
                float height = renderState.uvRect.height;
                float pivotX = renderState.uvPivot.x * tileX;
                float pivotY = renderState.uvPivot.y * tileY;

                for (int i = start; i < end; i++) {
                    float uvX = minX + (((uvs[i].x * tileX) + offsetX) * width) - pivotX;
                    float uvY = minY + (((uvs[i].y * tileY) + offsetY) * height) - pivotY;
                    uvs[i].x = pivotX + ((cos * uvX) - (sin * uvY));
                    uvs[i].y = pivotY + ((sin * uvX) + (cos * uvY));
                }
            }
        }

        public void Clear() {
            // todo -- just set sizes to 0
            shapes.QuickClear();
            positionList.QuickClear();
            normalList.QuickClear();
            colorList.QuickClear();
            texCoordList0.QuickClear();
            texCoordList1.QuickClear();
            texCoordList2.QuickClear();
            texCoordList3.QuickClear();
            triangleList.QuickClear();
        }

        public void CopyToMesh(GeometryShape shape, VertigoMesh mesh) { }

        public GeometryShape Text(float x, float y, TextInfo textInfo, in RenderState renderState) {

            CharInfo[] charInfos = textInfo.charInfoList.array;
            int charCount = textInfo.characterList.size;
            int vertIdx = vertexCount;
            int triIdx = triangleCount;
            int vertStart = vertIdx;
            int triangleStart = triIdx;

            EnsureAdditionalCapacity(charCount * 4, charCount * 6);

            int[] triangles = triangleList.array;
            Vector3[] positions = positionList.array;
            Vector3[] normals = normalList.array;
            Vector4[] uvChannel = GetUVChannel(renderState.texCoordChannel) ?? texCoordList0.array;
            Color[] colors = colorList.array;

            Vector3 normal = new Vector3(0, 0, -1);
            Color color = renderState.fillColor;

            float z = renderState.defaultZ;

            for (int i = 0; i < charCount; i++) {
                if (charInfos[i].character == ' ') continue;

                Vector2 topLeft = charInfos[i].layoutTopLeft;
                Vector2 bottomRight = charInfos[i].layoutBottomRight;

                Vector2 uvTopLeft = charInfos[i].uv0;
                Vector2 uvBottomRight = charInfos[i].uv1;

                float uv0x = uvTopLeft.x;
                float uv0y = uvTopLeft.y;
                float uv1x = uvBottomRight.x;
                float uv1y = uvBottomRight.y;

                positions[vertIdx + 0] = new Vector3(x + topLeft.x, -(y + bottomRight.y), z);
                positions[vertIdx + 1] = new Vector3(x + topLeft.x, -(y + topLeft.y), z);
                positions[vertIdx + 2] = new Vector3(x + bottomRight.x, -(y + topLeft.y), z);
                positions[vertIdx + 3] = new Vector3(x + bottomRight.x, -(y + bottomRight.y), z);

                // doesn't really make sense to to do UV offsets for text
                uvChannel[vertIdx + 0] = new Vector4(uv0x, uv0y);
                uvChannel[vertIdx + 1] = new Vector4(uv0x, uv1y);
                uvChannel[vertIdx + 2] = new Vector4(uv1x, uv1y);
                uvChannel[vertIdx + 3] = new Vector4(uv1x, uv0y);
                
                triangles[triIdx + 0] = vertIdx + 0;
                triangles[triIdx + 1] = vertIdx + 1;
                triangles[triIdx + 2] = vertIdx + 2;
                triangles[triIdx + 3] = vertIdx + 2;
                triangles[triIdx + 4] = vertIdx + 3;
                triangles[triIdx + 5] = vertIdx + 0;

                vertIdx += 4;
                triIdx += 6;
            }

            int vertEnd = vertIdx;
            
            for (int i = vertStart; i < vertEnd; i++) {
                colors[i] = color;
            }
            
            for (int i = vertStart; i < vertEnd; i++) {
                normals[i] = normal;
            }
            
            GeometryShape shape = new GeometryShape() {
                geometryType = GeometryType.Physical,
                shapeType = ShapeType.Text,
                vertexStart = vertStart,
                vertexCount = vertIdx - vertStart,
                triangleStart = triangleStart,
                triangleCount = triIdx - triangleStart
            };
            
            shapes.Add(shape);

            UpdateSizes(vertIdx - vertStart, triIdx - triangleStart);
            return shape;
        }

    }

}