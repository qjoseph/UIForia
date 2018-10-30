using System;
using System.Collections.Generic;
using Src.Elements;
using Src.Util;

namespace Src {

    public class UIGraphicTemplate : UITemplate {

        public UIGraphicTemplate(List<UITemplate> childTemplates, List<AttributeDefinition> attributes = null) : base(childTemplates, attributes) { }

        public override Type elementType => typeof(UIGraphicElement);
        
        public override MetaData CreateScoped(TemplateScope inputScope) {
            UIGraphicElement graphic = new UIGraphicElement();
            graphic.ownChildren = ArrayPool<UIElement>.Empty;
            graphic.templateChildren = ArrayPool<UIElement>.Empty;
            return GetCreationData(graphic, inputScope.context);
        }

    }

}