﻿using System;
using UIForia.Selectors;
using UIForia.Elements;
using UIForia.Rendering;
using UIForia.Util;

namespace UIForia.Systems {

    public interface IStyleSystem : ISystem {

        event Action<UIElement, StructList<StyleProperty>> onStylePropertyChanged;

        void SetStyleProperty(UIElement element, StyleProperty propertyValue);
        
    }

}