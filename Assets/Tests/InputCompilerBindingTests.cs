using NUnit.Framework;
using Src;
using Src.Compilers;
using System.Collections.Generic;
using System.Reflection;
using Src.Input;
using Src.Util;
using UnityEngine;

[TestFixture]
public class InputCompiler_KeyUpAttributes {

    public class KeyUpTestThing : UIElement {

        [OnKeyUp]
        public void HandleAnyKeyUpWithArgument(KeyboardInputEvent evt) { }

        [OnKeyUp]
        public void HandleAnyKeyUpWithoutArgument() { }

        [OnKeyUp(KeyCode.A)]
        public void HandleSpecificKeyUp() { }

        [OnKeyUp(KeyCode.A, EventModifiers.Alt)]
        public void HandleSpecificKeyUpWithModifier() { }

        [OnKeyUp(KeyCode.A)]
        public void HandleSpecificKeyUpWithArgument(KeyboardInputEvent evt) { }

        [OnKeyUp(KeyCode.A, EventModifiers.Alt)]
        public void HandleSpecificKeyUpWithModifierWithArgument(KeyboardInputEvent evt) { }

        [OnKeyUpWithFocus]
        public void HandleFocusAnyKeyUpWithArgument(KeyboardInputEvent evt) { }

        [OnKeyUpWithFocus]
        public void HandleFocusAnyKeyUpWithoutArgument() { }

        [OnKeyUpWithFocus(KeyCode.A)]
        public void HandleFocusSpecificKeyUp() { }

        [OnKeyUpWithFocus(KeyCode.A, EventModifiers.Alt)]
        public void HandleFocusSpecificKeyUpWithModifier() { }

        [OnKeyUpWithFocus(KeyCode.A)]
        public void HandleFocusSpecificKeyUpWithArgument(KeyboardInputEvent evt) { }

        [OnKeyUpWithFocus(KeyCode.A, EventModifiers.Alt)]
        public void HandleFocusSpecificKeyUpWithModifierWithArgument(KeyboardInputEvent evt) { }

    }

    [SetUp]
    public void Setup() {
        ContextDefinition ctx = new ContextDefinition(typeof(KeyUpTestThing));
        InputBindingCompiler compiler = new InputBindingCompiler(ctx);
        handlers = compiler.CompileKeyboardInputAttributes(typeof(KeyUpTestThing));
    }

    public List<KeyboardEventHandler> handlers;

    [Test]
    public void KeyUp_HandleAnyKeyPressWithArgument() {
        MethodInfo info = typeof(KeyUpTestThing).GetMethod(nameof(KeyUpTestThing.HandleAnyKeyUpWithArgument));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandler<KeyUpTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyUp, handler.eventType);
        Assert.AreEqual(false, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.None, handler.requiredModifiers);
        Assert.AreEqual(KeyCodeUtil.AnyKey, handler.keyCode);
    }

    [Test]
    public void KeyUp_HandleAnyKeyUpWithoutArgument() {
        MethodInfo info = typeof(KeyUpTestThing).GetMethod(nameof(KeyUpTestThing.HandleAnyKeyUpWithoutArgument));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandlerIgnoreEvent<KeyUpTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyUp, handler.eventType);
        Assert.AreEqual(false, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.None, handler.requiredModifiers);
        Assert.AreEqual(KeyCodeUtil.AnyKey, handler.keyCode);
    }

    [Test]
    public void KeyUp_HandleSpecificKeyUp() {
        MethodInfo info = typeof(KeyUpTestThing).GetMethod(nameof(KeyUpTestThing.HandleSpecificKeyUp));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandlerIgnoreEvent<KeyUpTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyUp, handler.eventType);
        Assert.AreEqual(false, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.None, handler.requiredModifiers);
        Assert.AreEqual(KeyCode.A, handler.keyCode);
    }

    [Test]
    public void KeyUp_HandleSpecificKeyUpWithModifier() {
        MethodInfo info = typeof(KeyUpTestThing).GetMethod(nameof(KeyUpTestThing.HandleSpecificKeyUpWithModifier));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandlerIgnoreEvent<KeyUpTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyUp, handler.eventType);
        Assert.AreEqual(false, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.Alt, handler.requiredModifiers);
        Assert.AreEqual(KeyCode.A, handler.keyCode);
    }

    [Test]
    public void KeyUp_HandleSpecificKeyUpWithArgument() {
        MethodInfo info = typeof(KeyUpTestThing).GetMethod(nameof(KeyUpTestThing.HandleSpecificKeyUpWithArgument));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandler<KeyUpTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyUp, handler.eventType);
        Assert.AreEqual(false, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.None, handler.requiredModifiers);
        Assert.AreEqual(KeyCode.A, handler.keyCode);
    }

    [Test]
    public void KeyUp_HandleSpecificKeyUpWithModifierWithArgument() {
        MethodInfo info = typeof(KeyUpTestThing).GetMethod(nameof(KeyUpTestThing.HandleSpecificKeyUpWithModifierWithArgument));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandler<KeyUpTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyUp, handler.eventType);
        Assert.AreEqual(false, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.Alt, handler.requiredModifiers);
        Assert.AreEqual(KeyCode.A, handler.keyCode);
    }

    [Test]
    public void KeyUp_HandleFocusAnyKeyUpWithArgument() {
        MethodInfo info = typeof(KeyUpTestThing).GetMethod(nameof(KeyUpTestThing.HandleFocusAnyKeyUpWithArgument));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandler<KeyUpTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyUp, handler.eventType);
        Assert.AreEqual(true, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.None, handler.requiredModifiers);
        Assert.AreEqual(KeyCodeUtil.AnyKey, handler.keyCode);
    }

    [Test]
    public void KeyUp_HandleFocusAnyKeyUpWithoutArgument() {
        MethodInfo info = typeof(KeyUpTestThing).GetMethod(nameof(KeyUpTestThing.HandleFocusAnyKeyUpWithoutArgument));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandlerIgnoreEvent<KeyUpTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyUp, handler.eventType);
        Assert.AreEqual(true, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.None, handler.requiredModifiers);
        Assert.AreEqual(KeyCodeUtil.AnyKey, handler.keyCode);
    }

    [Test]
    public void KeyUp_HandleFocusSpecificKeyUp() {
        MethodInfo info = typeof(KeyUpTestThing).GetMethod(nameof(KeyUpTestThing.HandleFocusSpecificKeyUp));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandlerIgnoreEvent<KeyUpTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyUp, handler.eventType);
        Assert.AreEqual(true, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.None, handler.requiredModifiers);
        Assert.AreEqual(KeyCode.A, handler.keyCode);
    }

    [Test]
    public void KeyUp_HandleFocusSpecificKeyUpWithModifier() {
        MethodInfo info = typeof(KeyUpTestThing).GetMethod(nameof(KeyUpTestThing.HandleFocusSpecificKeyUpWithModifier));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandlerIgnoreEvent<KeyUpTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyUp, handler.eventType);
        Assert.AreEqual(true, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.Alt, handler.requiredModifiers);
        Assert.AreEqual(KeyCode.A, handler.keyCode);
    }

    [Test]
    public void KeyUp_HandleFocusSpecificKeyUpWithArgument() {
        MethodInfo info = typeof(KeyUpTestThing).GetMethod(nameof(KeyUpTestThing.HandleFocusSpecificKeyUpWithArgument));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandler<KeyUpTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyUp, handler.eventType);
        Assert.AreEqual(true, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.None, handler.requiredModifiers);
        Assert.AreEqual(KeyCode.A, handler.keyCode);
    }

    [Test]
    public void KeyUp_HandleFocusSpecificKeyUpWithModifierWithArgument() {
        MethodInfo info = typeof(KeyUpTestThing).GetMethod(nameof(KeyUpTestThing.HandleFocusSpecificKeyUpWithModifierWithArgument));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandler<KeyUpTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyUp, handler.eventType);
        Assert.AreEqual(true, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.Alt, handler.requiredModifiers);
        Assert.AreEqual(KeyCode.A, handler.keyCode);
    }

    public static KeyboardEventHandler GetHandlerForMethod(MethodInfo info, List<KeyboardEventHandler> handlers) {
        return handlers.Find((h) => h.methodInfo == info);
    }

}

[TestFixture]
public class InputCompiler_KeyDownAttributes {

    public class KeyDownTestThing : UIElement {

        [OnKeyDown]
        public void HandleAnyKeyPressWithArgument(KeyboardInputEvent evt) { }

        [OnKeyDown]
        public void HandleAnyKeyPressWithoutArgument() { }

        [OnKeyDown(KeyCode.A)]
        public void HandleSpecificKeyPress() { }

        [OnKeyDown(KeyCode.A, EventModifiers.Alt)]
        public void HandleSpecificKeyPressWithModifier() { }

        [OnKeyDown(KeyCode.A)]
        public void HandleSpecificKeyPressWithArgument(KeyboardInputEvent evt) { }

        [OnKeyDown(KeyCode.A, EventModifiers.Alt)]
        public void HandleSpecificKeyPressWithModifierWithArgument(KeyboardInputEvent evt) { }

        [OnKeyDownWithFocus]
        public void HandleFocusAnyKeyPressWithArgument(KeyboardInputEvent evt) { }

        [OnKeyDownWithFocus]
        public void HandleFocusAnyKeyPressWithoutArgument() { }

        [OnKeyDownWithFocus(KeyCode.A)]
        public void HandleFocusSpecificKeyPress() { }

        [OnKeyDownWithFocus(KeyCode.A, EventModifiers.Alt)]
        public void HandleFocusSpecificKeyPressWithModifier() { }

        [OnKeyDownWithFocus(KeyCode.A)]
        public void HandleFocusSpecificKeyPressWithArgument(KeyboardInputEvent evt) { }

        [OnKeyDownWithFocus(KeyCode.A, EventModifiers.Alt)]
        public void HandleFocusSpecificKeyPressWithModifierWithArgument(KeyboardInputEvent evt) { }

    }

    public List<KeyboardEventHandler> handlers;

    [SetUp]
    public void Setup() {
        ContextDefinition ctx = new ContextDefinition(typeof(KeyDownTestThing));
        InputBindingCompiler compiler = new InputBindingCompiler(ctx);
        handlers = compiler.CompileKeyboardInputAttributes(typeof(KeyDownTestThing));
    }

    [Test]
    public void KeyDown_HandleAnyKeyPressWithArgument() {
        MethodInfo info = typeof(KeyDownTestThing).GetMethod(nameof(KeyDownTestThing.HandleAnyKeyPressWithArgument));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandler<KeyDownTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyDown, handler.eventType);
        Assert.AreEqual(false, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.None, handler.requiredModifiers);
        Assert.AreEqual(KeyCodeUtil.AnyKey, handler.keyCode);
    }

    [Test]
    public void KeyDown_HandleAnyKeyPressWithoutArgument() {
        MethodInfo info = typeof(KeyDownTestThing).GetMethod(nameof(KeyDownTestThing.HandleAnyKeyPressWithoutArgument));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandlerIgnoreEvent<KeyDownTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyDown, handler.eventType);
        Assert.AreEqual(false, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.None, handler.requiredModifiers);
        Assert.AreEqual(KeyCodeUtil.AnyKey, handler.keyCode);
    }

    [Test]
    public void KeyDown_HandleSpecificKeyPress() {
        MethodInfo info = typeof(KeyDownTestThing).GetMethod(nameof(KeyDownTestThing.HandleSpecificKeyPress));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandlerIgnoreEvent<KeyDownTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyDown, handler.eventType);
        Assert.AreEqual(false, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.None, handler.requiredModifiers);
        Assert.AreEqual(KeyCode.A, handler.keyCode);
    }

    [Test]
    public void KeyDown_HandleSpecificKeyPressWithModifier() {
        MethodInfo info = typeof(KeyDownTestThing).GetMethod(nameof(KeyDownTestThing.HandleSpecificKeyPressWithModifier));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandlerIgnoreEvent<KeyDownTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyDown, handler.eventType);
        Assert.AreEqual(false, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.Alt, handler.requiredModifiers);
        Assert.AreEqual(KeyCode.A, handler.keyCode);
    }

    [Test]
    public void KeyDown_HandleSpecificKeyPressWithArgument() {
        MethodInfo info = typeof(KeyDownTestThing).GetMethod(nameof(KeyDownTestThing.HandleSpecificKeyPressWithArgument));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandler<KeyDownTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyDown, handler.eventType);
        Assert.AreEqual(false, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.None, handler.requiredModifiers);
        Assert.AreEqual(KeyCode.A, handler.keyCode);
    }

    [Test]
    public void KeyDown_HandleSpecificKeyPressWithModifierWithArgument() {
        MethodInfo info = typeof(KeyDownTestThing).GetMethod(nameof(KeyDownTestThing.HandleSpecificKeyPressWithModifierWithArgument));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandler<KeyDownTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyDown, handler.eventType);
        Assert.AreEqual(false, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.Alt, handler.requiredModifiers);
        Assert.AreEqual(KeyCode.A, handler.keyCode);
    }

    [Test]
    public void KeyDown_HandleFocusAnyKeyPressWithArgument() {
        MethodInfo info = typeof(KeyDownTestThing).GetMethod(nameof(KeyDownTestThing.HandleFocusAnyKeyPressWithArgument));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandler<KeyDownTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyDown, handler.eventType);
        Assert.AreEqual(true, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.None, handler.requiredModifiers);
        Assert.AreEqual(KeyCodeUtil.AnyKey, handler.keyCode);
    }

    [Test]
    public void KeyDown_HandleFocusAnyKeyPressWithoutArgument() {
        MethodInfo info = typeof(KeyDownTestThing).GetMethod(nameof(KeyDownTestThing.HandleFocusAnyKeyPressWithoutArgument));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandlerIgnoreEvent<KeyDownTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyDown, handler.eventType);
        Assert.AreEqual(true, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.None, handler.requiredModifiers);
        Assert.AreEqual(KeyCodeUtil.AnyKey, handler.keyCode);
    }

    [Test]
    public void KeyDown_HandleFocusSpecificKeyPress() {
        MethodInfo info = typeof(KeyDownTestThing).GetMethod(nameof(KeyDownTestThing.HandleFocusSpecificKeyPress));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandlerIgnoreEvent<KeyDownTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyDown, handler.eventType);
        Assert.AreEqual(true, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.None, handler.requiredModifiers);
        Assert.AreEqual(KeyCode.A, handler.keyCode);
    }

    [Test]
    public void KeyDown_HandleFocusSpecificKeyPressWithModifier() {
        MethodInfo info = typeof(KeyDownTestThing).GetMethod(nameof(KeyDownTestThing.HandleFocusSpecificKeyPressWithModifier));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandlerIgnoreEvent<KeyDownTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyDown, handler.eventType);
        Assert.AreEqual(true, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.Alt, handler.requiredModifiers);
        Assert.AreEqual(KeyCode.A, handler.keyCode);
    }

    [Test]
    public void KeyDown_HandleFocusSpecificKeyPressWithArgument() {
        MethodInfo info = typeof(KeyDownTestThing).GetMethod(nameof(KeyDownTestThing.HandleFocusSpecificKeyPressWithArgument));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandler<KeyDownTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyDown, handler.eventType);
        Assert.AreEqual(true, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.None, handler.requiredModifiers);
        Assert.AreEqual(KeyCode.A, handler.keyCode);
    }

    [Test]
    public void KeyDown_HandleFocusSpecificKeyPressWithModifierWithArgument() {
        MethodInfo info = typeof(KeyDownTestThing).GetMethod(nameof(KeyDownTestThing.HandleFocusSpecificKeyPressWithModifierWithArgument));
        KeyboardEventHandler handler = GetHandlerForMethod(info, handlers);

        Assert.IsInstanceOf<KeyboardEventHandler<KeyDownTestThing>>(handler);
        Assert.AreEqual(InputEventType.KeyDown, handler.eventType);
        Assert.AreEqual(true, handler.requiresFocus);
        Assert.AreEqual(EventModifiers.Alt, handler.requiredModifiers);
        Assert.AreEqual(KeyCode.A, handler.keyCode);
    }


    public static KeyboardEventHandler GetHandlerForMethod(MethodInfo info, List<KeyboardEventHandler> handlers) {
        return handlers.Find((h) => h.methodInfo == info);
    }

}