using UIForia.Style;
using UIForia.Util;
using UnityEngine;

namespace UIForia.Style {

    public struct StyleNode {

        public int index;

        public int normalIndex;
        public int hoverIndex;
        public int focusIndex;
        public int activeIndex;

        public int firstAttrIndex;
        public int firstSelectorIndex;
        public RangeInt nameRange; // maybe just use a string?
        public LineInfo lineInfo;

    }

    public struct StylePart {

        public int type;
        public int dataIndex;
        public LineInfo lineInfo;
        public int firstChild;
        public int nextSibling;

    }

    public struct StyleBlockNode {

        public int type;
        public int dataIndex;
        public int firstChild;
        public int nextSibling;

    }

    public struct AttrNode {

        public int styleIndex;
        public int blockIndex;
        public RangeInt keyRange;
        public RangeInt valueRange;
        public int firstChild;

    }

    public struct PropertyNode {

        public RangeInt keyRange;
        public RangeInt valueRange;
        public int nextSibling;

    }


    public class StyleFileShellBuilder {

        public StructList<StyleNode> styleNodes;
        public StructList<char> charBuffer;

        public StyleFileShellBuilder() {
            charBuffer = new StructList<char>(4096);
            styleNodes = new StructList<StyleNode>();
        }
        
        public StyleASTBuilder AddStyleNode(CharSpan styleName) {
            return default;
        }

        public bool Build() {
            // todo -- validate general structure here
            // ie no condition contains a style
            // no duplicate style names
            // no unknown references (only checking @theme, not @theme.whatever)
            return true;
        }

        public struct StyleASTBuilder {

            public void AddConditionNode(CharSpan condition) {
            }

            public void AddStyleProperty(CharSpan key, CharSpan value) {
                
            }

        }

        public void Clear() {
        }

    }

    public class StyleFileShell { }

}