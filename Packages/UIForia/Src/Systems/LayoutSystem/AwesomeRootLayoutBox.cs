using System;
using UIForia.Layout;
using UIForia.Util;

namespace UIForia.Systems {

    public class AwesomeRootLayoutBox : AwesomeLayoutBox {

        protected override float ComputeContentWidth() {
            return 0;
        }

        protected override float ComputeContentHeight() {
            return 0;
        }

        public override void OnChildrenChanged(LightList<AwesomeLayoutBox> childList) { }

        public override void RunLayoutHorizontal(int frameId) {
            LayoutSize size = default;
            firstChild.GetWidths(ref size);
            firstChild.ApplyLayoutHorizontal(0, size.preferred, size.preferred, LayoutFit.None, frameId);
        }

        public override void RunLayoutVertical() {
            throw new NotImplementedException();
        }

    }

}