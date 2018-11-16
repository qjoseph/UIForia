﻿using System;
using System.Collections.Generic;
using UIForia.Animation;
using UIForia.Rendering;
using UIForia.StyleBindings;
using UIForia.Util;
using UnityEngine;

namespace UIForia.Systems {

    public class StyleSystem : IStyleSystem {

        protected readonly StyleAnimator animator;

        public event Action<UIElement, string> onTextContentChanged;
        public event Action<UIElement, StyleProperty> onStylePropertyChanged;

        private static readonly Stack<UIElement> s_ElementStack = new Stack<UIElement>();

        private readonly IntMap<ChangeSet> m_ChangeSets;

        public StyleSystem() {
            this.animator = new StyleAnimator();
            this.m_ChangeSets = new IntMap<ChangeSet>();
        }

        public void PlayAnimation(UIStyleSet styleSet, StyleAnimation animation, AnimationOptions overrideOptions = default(AnimationOptions)) {
            int animationId = animator.PlayAnimation(styleSet, animation, overrideOptions);
        }

        public void SetViewportRect(Rect viewport) {
            animator.SetViewportRect(viewport);
        }

        public void OnReset() {
            animator.Reset();
        }

        public void OnElementCreated(UIElement element) {
            if ((element.flags & UIElementFlags.TextElement) != 0) {
                ((UITextElement) element).onTextChanged += HandleTextChanged;
            }

            UITemplateContext context = element.TemplateContext;
            List<UIStyleGroup> baseStyles = element.templateRef.baseStyles;
            List<StyleBinding> constantStyleBindings = element.templateRef.constantStyleBindings;

            element.style.styleSystem = this;

            // todo -- push to style buffer & apply later on first run
            for (int i = 0; i < constantStyleBindings.Count; i++) {
                constantStyleBindings[i].Apply(element.style, context);
            }

            for (int i = 0; i < baseStyles.Count; i++) {
                element.style.AddStyleGroup(baseStyles[i]);
            }

            element.style.Initialize();

            var x = element.style.PreferredWidth;
            if (element.children != null) {
                for (int i = 0; i < element.children.Length; i++) {
                    OnElementCreated(element.children[i]);
                }
            }
        }

        public void OnUpdate() {
            animator.OnUpdate();

            if (onStylePropertyChanged == null) {
                return;
            }

            m_ChangeSets.ForEach(this, (id, changeSet, self) => {
                UIElement element = changeSet.element;
                int changeCount = changeSet.changes.Count;
                StyleProperty[] properties = changeSet.changes.List;
                for (int i = 0; i < changeCount; i++) {
                    // todo -- change this to be 1 invoke w/ property list instead of n invokes
                    self.onStylePropertyChanged.Invoke(element, properties[i]);
                }

                LightListPool<StyleProperty>.Release(ref changeSet.changes);
                changeSet.element = null;
            });

            m_ChangeSets.Clear();
        }

        public void OnDestroy() { }

        public void OnViewAdded(UIView view) { }

        public void OnViewRemoved(UIView view) { }

        public void OnElementEnabled(UIElement element) { }

        public void OnElementDisabled(UIElement element) { }

        public void OnElementDestroyed(UIElement element) { }


        private void AddToChangeSet(UIElement element, StyleProperty property) {
            ChangeSet changeSet;
            if (!m_ChangeSets.TryGetValue(element.id, out changeSet)) {
                changeSet = new ChangeSet(element, LightListPool<StyleProperty>.Get());
                m_ChangeSets[element.id] = changeSet;
            }

            changeSet.changes.Add(property);
        }

        // todo -- buffer & flush these instead of doing it all at once
        public void SetStyleProperty(UIElement element, StyleProperty property) {
            AddToChangeSet(element, property);

            if (!StyleUtil.IsPropertyInherited(property.propertyId)) {
                return;
            }

            for (int i = 0; i < element.children.Length; i++) {
                s_ElementStack.Push(element.children[i]);
            }

            while (s_ElementStack.Count > 0) {
                UIElement descendent = s_ElementStack.Pop();

                if (!descendent.style.SetInheritedStyle(property)) {
                    continue;
                }

                AddToChangeSet(descendent, property);

                if (descendent.children == null) {
                    continue;
                }

                for (int i = 0; i < descendent.children.Length; i++) {
                    s_ElementStack.Push(descendent.children[i]);
                }
            }
        }

        private void HandleTextChanged(UITextElement element, string text) {
            onTextContentChanged?.Invoke(element, text);
        }

        private struct ChangeSet {

            public UIElement element;
            public LightList<StyleProperty> changes;

            public ChangeSet(UIElement element, LightList<StyleProperty> changes) {
                this.element = element;
                this.changes = changes;
            }

        }

    }

}