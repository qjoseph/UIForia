using System;
using System.Reflection;
using NUnit.Framework;
using UIForia;
using UIForia.Rendering;
using UIForia.Systems;
using Tests.Mocks;
using UnityEditor.VersionControl;
using UnityEngine;

[TestFixture]
public class StyleTests {

    [Template(TemplateType.String, @"
        <UITemplate>
            <Contents>
                <Group x-name='group1'>
                    <Group x-name='group2'/>
                </Group>                  
                <Group x-name='group3'/>                  
                <Group x-name='group4'/>                  
            </Contents>
        </UITemplate>
    ")]
    public class StyleSetTestThing : UIElement { }

    public T ComputedValue<T>(UIElement obj, string propertyName) {
        return (T) typeof(UIStyleSet).GetProperty(propertyName).GetValue(obj.style);
    }

    public T DefaultValue<T>(string propertyName) {
        return (T) typeof(DefaultStyleValues_Generated).GetField(propertyName, BindingFlags.Static | BindingFlags.Public).GetValue(null);
    }

    public void CallMethod(object target, string methodName, object arg0, object arg1) {
        target.GetType().GetMethod(methodName).Invoke(target, new object[] {arg0, arg1});
    }

    private void SetStyleValue<T>(UIStyle style, string propertyName, T value) {
        typeof(UIStyle).GetProperty(propertyName).SetValue(style, value);
    }

    private void RunIntTests(Action<string, string, string> TestBody) {
        TestBody(
            nameof(UIStyleSet.SetTextFontSize),
            nameof(UIStyleSet.TextFontSize),
            nameof(DefaultStyleValues_Generated.TextFontSize)
        );

        TestBody(
            nameof(UIStyleSet.SetFlexItemGrow),
            nameof(UIStyleSet.FlexItemGrow),
            nameof(DefaultStyleValues_Generated.FlexItemGrow)
        );

        TestBody(
            nameof(UIStyleSet.SetFlexItemShrink),
            nameof(UIStyleSet.FlexItemShrink),
            nameof(DefaultStyleValues_Generated.FlexItemShrink)
        );

        TestBody(
            nameof(UIStyleSet.SetFlexItemOrder),
            nameof(UIStyleSet.FlexItemOrder),
            nameof(DefaultStyleValues_Generated.FlexItemOrder)
        );

        TestBody(
            nameof(UIStyleSet.SetFlexItemOrder),
            nameof(UIStyleSet.FlexItemOrder),
            nameof(DefaultStyleValues_Generated.FlexItemOrder)
        );
    }

    private void RunSizeTests(Action<string, string, string> TestBody) {
        TestBody(nameof(UIStyleSet.SetMinWidth), nameof(UIStyleSet.MinWidth), nameof(DefaultStyleValues_Generated.MinWidth));
        TestBody(nameof(UIStyleSet.SetMaxWidth), nameof(UIStyleSet.MaxWidth), nameof(DefaultStyleValues_Generated.MaxWidth));
        TestBody(nameof(UIStyleSet.SetPreferredWidth), nameof(UIStyleSet.PreferredWidth), nameof(DefaultStyleValues_Generated.PreferredWidth));

        TestBody(nameof(UIStyleSet.SetMinHeight), nameof(UIStyleSet.MinHeight), nameof(DefaultStyleValues_Generated.MinHeight));
        TestBody(nameof(UIStyleSet.SetMaxHeight), nameof(UIStyleSet.MaxHeight), nameof(DefaultStyleValues_Generated.MaxHeight));
        TestBody(nameof(UIStyleSet.SetPreferredHeight), nameof(UIStyleSet.PreferredHeight), nameof(DefaultStyleValues_Generated.PreferredHeight));
    }

    [Test]
    public void IntProperties_UpdateComputedStyleWithValue() {
        Action<string, string, string> TestBody = (setFnName, computedPropertyName, defaultName) => {
            MockApplication view = new MockApplication(typeof(StyleSetTestThing));
            StyleSetTestThing root = (StyleSetTestThing) view.RootElement;

            Assert.AreEqual(DefaultValue<int>(defaultName), ComputedValue<int>(root, computedPropertyName));

            CallMethod(root.style, setFnName, ComputedValue<int>(root, computedPropertyName) + 1, StyleState.Normal);

            Assert.AreEqual(DefaultValue<int>(defaultName) + 1, ComputedValue<int>(root, computedPropertyName));
        };

        RunIntTests(TestBody);
    }

    [Test]
    public void IntProperties_UpdateComputedStyleWithUnsetValue() {
        Action<string, string, string> TestBody = (setFnName, computedPropertyName, defaultName) => {
            MockApplication view = new MockApplication(typeof(StyleSetTestThing));
            StyleSetTestThing root = (StyleSetTestThing) view.RootElement;

            Assert.AreEqual(DefaultValue<int>(defaultName), ComputedValue<int>(root, computedPropertyName));

            CallMethod(root.style, setFnName, DefaultValue<int>(defaultName) + 1, StyleState.Normal);
            CallMethod(root.style, setFnName, IntUtil.UnsetValue, StyleState.Normal);

            Assert.AreEqual(DefaultValue<int>(defaultName), ComputedValue<int>(root, computedPropertyName));
        };

        RunIntTests(TestBody);
    }

    [Test]
    public void IntProperties_EnterState() {
        Action<string, string, string> TestBody = (setFnName, propName, defaultName) => {
            MockApplication view = new MockApplication(typeof(StyleSetTestThing));
            StyleSetTestThing root = (StyleSetTestThing) view.RootElement;

            CallMethod(root.style, setFnName, DefaultValue<int>(defaultName) + 1, StyleState.Hover);

            Assert.AreEqual(DefaultValue<int>(defaultName), ComputedValue<int>(root, propName));

            root.style.EnterState(StyleState.Hover);

            Assert.AreEqual(DefaultValue<int>(defaultName) + 1, ComputedValue<int>(root, propName));
        };

        RunIntTests(TestBody);
    }

    [Test]
    public void IntProperties_EnterSecondState() {
        Action<string, string, string> TestBody = (setFnName, propName, defaultName) => {
            MockApplication view = new MockApplication(typeof(StyleSetTestThing));
            StyleSetTestThing root = (StyleSetTestThing) view.RootElement;

            CallMethod(root.style, setFnName, DefaultValue<int>(defaultName) + 1, StyleState.Hover);
            CallMethod(root.style, setFnName, DefaultValue<int>(defaultName) + 5, StyleState.Focused);

            root.style.EnterState(StyleState.Hover);

            Assert.AreEqual(DefaultValue<int>(defaultName) + 1, ComputedValue<int>(root, propName));

            root.style.EnterState(StyleState.Focused);

            Assert.AreEqual(DefaultValue<int>(defaultName) + 5, ComputedValue<int>(root, propName));

            root.style.ExitState(StyleState.Focused);

            Assert.AreEqual(DefaultValue<int>(defaultName) + 1, ComputedValue<int>(root, propName));
        };
        RunIntTests(TestBody);
    }

    [Test]
    public void IntProperties_ExitState() {
        Action<string, string, string> TestBody = (setFnName, propName, defaultName) => {
            MockApplication view = new MockApplication(typeof(StyleSetTestThing));
            StyleSetTestThing root = (StyleSetTestThing) view.RootElement;

            CallMethod(root.style, setFnName, DefaultValue<int>(defaultName) + 1, StyleState.Normal);
            CallMethod(root.style, setFnName, DefaultValue<int>(defaultName) + 2, StyleState.Hover);

            Assert.AreEqual(DefaultValue<int>(defaultName) + 1, ComputedValue<int>(root, propName));

            root.style.EnterState(StyleState.Hover);

            Assert.AreEqual(DefaultValue<int>(defaultName) + 2, ComputedValue<int>(root, propName));

            root.style.ExitState(StyleState.Hover);

            Assert.AreEqual(DefaultValue<int>(defaultName) + 1, ComputedValue<int>(root, propName));
        };
        RunIntTests(TestBody);
    }

    [Test]
    public void IntProperties_FromBaseStyle() {
        Action<string, string, string> TestBody = (setFnName, propName, defaultName) => {
            MockApplication view = new MockApplication(typeof(StyleSetTestThing));
            StyleSetTestThing root = (StyleSetTestThing) view.RootElement;

            UIStyle baseStyle = new UIStyle();
            SetStyleValue(baseStyle, propName, 5);

            UIStyleGroup group = new UIStyleGroup();
            group.name = "Name";
            group.normal = baseStyle;
            root.style.AddStyleGroup(group);

            Assert.AreEqual(5, ComputedValue<int>(root, propName));

            root.style.RemoveBaseStyle(baseStyle);

            Assert.AreEqual(DefaultValue<int>(defaultName), ComputedValue<int>(root, propName));
        };

        RunIntTests(TestBody);
    }

    [Test]
    public void IntProperties_OverrideBaseStyle() {
        Action<string, string, string> TestBody = (setFnName, propName, defaultName) => {
            MockApplication view = new MockApplication(typeof(StyleSetTestThing));
            StyleSetTestThing root = (StyleSetTestThing) view.RootElement;

            UIStyle baseStyle = new UIStyle();
            SetStyleValue(baseStyle, propName, 5);

            CallMethod(root.style, setFnName, 15, StyleState.Normal);
            UIStyleGroup group = new UIStyleGroup();
            group.name = "Name";
            group.normal = baseStyle;
            root.style.AddStyleGroup(group);
            Assert.AreEqual(15, ComputedValue<int>(root, propName));

            root.style.RemoveBaseStyle(baseStyle);

            Assert.AreEqual(15, ComputedValue<int>(root, propName));
        };
        RunIntTests(TestBody);
    }

    [Test]
    public void IntProperties_OverrideBaseStyleInState() {
        Action<string, string, string> TestBody = (setFnName, propName, defaultName) => {
            MockApplication view = new MockApplication(typeof(StyleSetTestThing));
            StyleSetTestThing root = (StyleSetTestThing) view.RootElement;

            UIStyle baseStyle = new UIStyle();
            SetStyleValue(baseStyle, propName, 5);

            CallMethod(root.style, setFnName, 15, StyleState.Hover);
            UIStyleGroup group = new UIStyleGroup();
            group.name = "Name";
            group.normal = baseStyle;
            root.style.AddStyleGroup(group);
            Assert.AreEqual(5, ComputedValue<int>(root, propName));

            root.style.EnterState(StyleState.Hover);
            Assert.AreEqual(15, ComputedValue<int>(root, propName));
        };
        RunIntTests(TestBody);
    }

    [Test]
    public void SizeProperties_UpdateComputedValue() {
        Action<string, string, string> TestBody = (setFnName, computedPropertyName, defaultName) => {
            MockApplication view = new MockApplication(typeof(StyleSetTestThing));
            StyleSetTestThing root = (StyleSetTestThing) view.RootElement;

            Assert.AreEqual(DefaultValue<UIMeasurement>(defaultName), ComputedValue<UIMeasurement>(root, computedPropertyName));

            CallMethod(root.style, setFnName, new UIMeasurement(99999), StyleState.Normal);

            Assert.AreEqual(new UIMeasurement(99999), ComputedValue<UIMeasurement>(root, computedPropertyName));
        };

        RunSizeTests(TestBody);
    }

    [Test]
    public void SizeProperties_UpdateComputedStyleWithUnsetValue() {
        Action<string, string, string> TestBody = (setFnName, computedPropertyName, defaultName) => {
            MockApplication view = new MockApplication(typeof(StyleSetTestThing));
            StyleSetTestThing root = (StyleSetTestThing) view.RootElement;

            Assert.AreEqual(DefaultValue<UIMeasurement>(defaultName), ComputedValue<UIMeasurement>(root, computedPropertyName));

            CallMethod(root.style, setFnName, new UIMeasurement(99999), StyleState.Normal);
            CallMethod(root.style, setFnName, UIMeasurement.Unset, StyleState.Normal);

            Assert.AreEqual(DefaultValue<UIMeasurement>(defaultName), ComputedValue<UIMeasurement>(root, computedPropertyName));
        };

        RunSizeTests(TestBody);
    }

    [Test]
    public void SizeProperties_EnterState() {
        Action<string, string, string> TestBody = (setFnName, propName, defaultName) => {
            MockApplication view = new MockApplication(typeof(StyleSetTestThing));
            StyleSetTestThing root = (StyleSetTestThing) view.RootElement;

            CallMethod(root.style, setFnName, new UIMeasurement(99999), StyleState.Hover);

            Assert.AreEqual(DefaultValue<UIMeasurement>(defaultName), ComputedValue<UIMeasurement>(root, propName));

            root.style.EnterState(StyleState.Hover);

            Assert.AreEqual(new UIMeasurement(99999), ComputedValue<UIMeasurement>(root, propName));
        };

        RunSizeTests(TestBody);
    }

    [Test]
    public void SizeProperties_EnterSecondState() {
        Action<string, string, string> TestBody = (setFnName, propName, defaultName) => {
            MockApplication view = new MockApplication(typeof(StyleSetTestThing));
            StyleSetTestThing root = (StyleSetTestThing) view.RootElement;

            CallMethod(root.style, setFnName, new UIMeasurement(1000), StyleState.Hover);
            CallMethod(root.style, setFnName, new UIMeasurement(5000), StyleState.Focused);

            root.style.EnterState(StyleState.Hover);

            Assert.AreEqual(new UIMeasurement(1000), ComputedValue<UIMeasurement>(root, propName));

            root.style.EnterState(StyleState.Focused);

            Assert.AreEqual(new UIMeasurement(5000), ComputedValue<UIMeasurement>(root, propName));

            root.style.ExitState(StyleState.Focused);

            Assert.AreEqual(new UIMeasurement(1000), ComputedValue<UIMeasurement>(root, propName));
        };
        RunSizeTests(TestBody);
    }

    [Test]
    public void SizeProperties_ExitState() {
        Action<string, string, string> TestBody = (setFnName, propName, defaultName) => {
            MockApplication view = new MockApplication(typeof(StyleSetTestThing));
            StyleSetTestThing root = (StyleSetTestThing) view.RootElement;

            CallMethod(root.style, setFnName, new UIMeasurement(1000), StyleState.Normal);
            CallMethod(root.style, setFnName, new UIMeasurement(2000), StyleState.Hover);

            Assert.AreEqual(new UIMeasurement(1000), ComputedValue<UIMeasurement>(root, propName));

            root.style.EnterState(StyleState.Hover);

            Assert.AreEqual(new UIMeasurement(2000), ComputedValue<UIMeasurement>(root, propName));

            root.style.ExitState(StyleState.Hover);

            Assert.AreEqual(new UIMeasurement(1000), ComputedValue<UIMeasurement>(root, propName));
        };
        RunSizeTests(TestBody);
    }

    [Test]
    public void SizeProperties_FromBaseStyle() {
        Action<string, string, string> TestBody = (setFnName, propName, defaultName) => {
            MockApplication view = new MockApplication(typeof(StyleSetTestThing));
            StyleSetTestThing root = (StyleSetTestThing) view.RootElement;

            UIStyle baseStyle = new UIStyle();
            SetStyleValue(baseStyle, propName, new UIMeasurement(5000));

            UIStyleGroup group = new UIStyleGroup();
            group.name = "Name";
            group.normal = baseStyle;
            root.style.AddStyleGroup(group);
            Assert.AreEqual(new UIMeasurement(5000), ComputedValue<UIMeasurement>(root, propName));

            root.style.RemoveBaseStyle(baseStyle);

            Assert.AreEqual(DefaultValue<UIMeasurement>(defaultName), ComputedValue<UIMeasurement>(root, propName));
        };

        RunSizeTests(TestBody);
    }

    [Test]
    public void SizeProperties_OverrideBaseStyle() {
        Action<string, string, string> TestBody = (setFnName, propName, defaultName) => {
            MockApplication view = new MockApplication(typeof(StyleSetTestThing));
            StyleSetTestThing root = (StyleSetTestThing) view.RootElement;

            UIStyle baseStyle = new UIStyle();
            SetStyleValue(baseStyle, propName, new UIMeasurement(5000));

            CallMethod(root.style, setFnName, new UIMeasurement(1500), StyleState.Normal);
            UIStyleGroup group = new UIStyleGroup();
            group.name = "Name";
            group.normal = baseStyle;
            root.style.AddStyleGroup(group);
            Assert.AreEqual(new UIMeasurement(1500), ComputedValue<UIMeasurement>(root, propName));

            root.style.RemoveBaseStyle(baseStyle);

            Assert.AreEqual(new UIMeasurement(1500), ComputedValue<UIMeasurement>(root, propName));
        };
        RunSizeTests(TestBody);
    }

    [Test]
    public void SizeProperties_OverrideBaseStyleInState() {
        Action<string, string, string> TestBody = (setFnName, propName, defaultName) => {
            MockApplication view = new MockApplication(typeof(StyleSetTestThing));
            StyleSetTestThing root = (StyleSetTestThing) view.RootElement;

            UIStyle baseStyle = new UIStyle();
            SetStyleValue(baseStyle, propName, new UIMeasurement(500));

            CallMethod(root.style, setFnName, new UIMeasurement(1500), StyleState.Hover);
            UIStyleGroup group = new UIStyleGroup();
            group.name = "Name";
            group.normal = baseStyle;
            root.style.AddStyleGroup(group);
            Assert.AreEqual(new UIMeasurement(500), ComputedValue<UIMeasurement>(root, propName));

            root.style.EnterState(StyleState.Hover);
            Assert.AreEqual(new UIMeasurement(1500), ComputedValue<UIMeasurement>(root, propName));
        };
        RunSizeTests(TestBody);
    }

    [Test]
    public void ConvertColorToStyleColor() {
        Color32 c = new Color32(128, 128, 128, 1);
        StyleColor styleColor = new StyleColor(c);
        Assert.AreEqual((Color)c, (Color)styleColor);

    }

}