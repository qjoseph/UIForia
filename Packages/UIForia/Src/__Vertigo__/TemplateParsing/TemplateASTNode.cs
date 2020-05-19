using System;
using UIForia.Compilers;
using UIForia.Parsing;

namespace UIForia {

    [Serializable]
    public struct TemplateASTNode {

        // can change these to be their own lists + search for them by index since not every node has these 
        
        public TemplateNodeType templateNodeType;

        public int attributeRangeStart;
        public int attributeRangeEnd;

        public int lineNumber;
        public int columnNumber;

        public int parentId;
        public int childCount;
        public int nextSiblingIndex;
        public int firstChildIndex;
        public int index;

        public int attributeCount {
            get => attributeRangeEnd - attributeRangeStart;
        }

        public LineInfo GetLineInfo() {
            return new LineInfo(lineNumber, columnNumber);
        }

    }

}