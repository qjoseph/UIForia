using System;
using UIForia.Compilers;
using UIForia.Elements;

namespace UIForia.Generated {

    public partial class UIForiaGeneratedTemplates_TestApp {
        
        public Func<UIElement, TemplateScope2, UIElement> Template_2c72910b8fc257e4fbc9d2bc804c2a54 = (UIForia.Elements.UIElement root, UIForia.Compilers.TemplateScope2 scope) =>
        {
            UIForia.Elements.UIElement targetElement_1;

            if (root == null)
            {
                root = scope.application.CreateElementFromPoolWithType(typeof(UIForia.Test.TestData.LoadTemplate0), default(UIForia.Elements.UIElement), 2, 0);
            }

            // <Text />    'I am text'
            targetElement_1 = scope.application.CreateElementFromPoolWithType(typeof(UIForia.Elements.UITextElement), root, 0, 0);
            ((UIForia.Elements.UITextElement)targetElement_1).text = "I am text";
            root.children.array[0] = targetElement_1;

            // <LoadTemplateHydrate if=="refTypeThing.intVal > 145" ctxvar:ctxVar0="14244" ctxvar:ctxVar1="144" attr:nonconst="{computed}" attr:first="true" attr:some-attr="this-is-attr" floatVal=="42f" onDidSomething=="HandleStuffDone(valueName)" intVal=="$ctxVar0 + $ctxVar1" intVal2=="refTypeThing.intVal"/>
            targetElement_1 = scope.application.CreateElementFromPoolWithType(typeof(UIForia.Test.TestData.LoadTemplateHydrate), root, 1, 3);
            targetElement_1.attributes.array[0] = new UIForia.Elements.ElementAttribute("nonconst", "");
            targetElement_1.attributes.array[1] = new UIForia.Elements.ElementAttribute("first", "true");
            targetElement_1.attributes.array[2] = new UIForia.Elements.ElementAttribute("some-attr", "this-is-attr");
            scope.application.HydrateTemplate(1, targetElement_1, new UIForia.Compilers.TemplateScope2(scope.application, default(UIForia.Systems.LinqBindingNode), default(UIForia.Util.StructList<UIForia.Compilers.SlotUsage>)));
            UIForia.Systems.LinqBindingNode.Get(scope.application, root, targetElement_1);
            root.children.array[1] = targetElement_1;
            return root;
        }; 

        public Action<UIElement, UIElement> Binding_OnCreate_4feee72f3b1592c45966fed00869c2d1 = (UIForia.Elements.UIElement __root, UIForia.Elements.UIElement __element) =>
        {
            UIForia.Test.TestData.LoadTemplateHydrate __castElement;
            UIForia.Test.TestData.LoadTemplate0 __castRoot;
            UIForia.Systems.ContextVariable<int> ctxVar_ctxVar0;
            UIForia.Systems.ContextVariable<int> ctxVar_ctxVar1;
            System.Action<string> evtFn;

            __castElement = ((UIForia.Test.TestData.LoadTemplateHydrate)__element);
            __castRoot = ((UIForia.Test.TestData.LoadTemplate0)__root);
            __castElement.bindingNode.CreateLocalContextVariable(new UIForia.Systems.ContextVariable<int>());
            ctxVar_ctxVar0 = ((UIForia.Systems.ContextVariable<int>)__castElement.bindingNode.GetLocalContextVariable("varName"));
            ctxVar_ctxVar0.value = 14244;
            __castElement.bindingNode.CreateLocalContextVariable(new UIForia.Systems.ContextVariable<int>());
            ctxVar_ctxVar1 = ((UIForia.Systems.ContextVariable<int>)__castElement.bindingNode.GetLocalContextVariable("varName"));
            ctxVar_ctxVar1.value = 144;

            // floatVal="42f"
            __castElement.floatVal = 42f;
        section_0_0:
            evtFn = (string valueName) =>
            {
                __castRoot.HandleStuffDone(valueName);
            };
            __castElement.onDidSomething += evtFn;
        };
public Action<UIElement, UIElement> Binding_OnUpdate_2ee8b0c6ab675ed4db36ca6068e10db5 = (UIForia.Elements.UIElement __root, UIForia.Elements.UIElement __element) =>
        {
            UIForia.Test.TestData.LoadTemplateHydrate __castElement;
            UIForia.Test.TestData.LoadTemplate0 __castRoot;
            UIForia.Test.TestData.RefTypeThing nullCheck;
            int ctxvar_ctxVar0;
            int ctxvar_ctxVar1;
            int __right;
            UIForia.Test.TestData.RefTypeThing nullCheck_0;
            int __right_0;

            __castElement = ((UIForia.Test.TestData.LoadTemplateHydrate)__element);
            __castRoot = ((UIForia.Test.TestData.LoadTemplate0)__root);
            nullCheck = __castRoot.refTypeThing;
            if (nullCheck == null)
            {
                goto section_0_0;
            }
            __castElement.SetEnabled(nullCheck.intVal > 145);

            // if="refTypeThing.intVal > 145"
            if (__castElement.isEnabled == false)
            {
                goto retn;
            }
        section_0_0:

            // nonconst="{computed}"
            __castElement.SetAttribute("nonconst", __castRoot.computed);

            // intVal="$ctxVar0 + $ctxVar1"
            ctxvar_ctxVar0 = ((UIForia.Systems.ContextVariable<int>)__castElement.bindingNode.GetContextVariable(0)).value;
            ctxvar_ctxVar1 = ((UIForia.Systems.ContextVariable<int>)__castElement.bindingNode.GetContextVariable(1)).value;
            __right = ctxvar_ctxVar0 + ctxvar_ctxVar1;
            if (__castElement.intVal != __right)
            {
                __castElement.intVal = __right;
                __castElement.HandleIntValChanged();
            }
        section_0_1:

            // intVal2="refTypeThing.intVal"
            nullCheck_0 = __castRoot.refTypeThing;
            if (nullCheck_0 == null)
            {
                goto section_0_2;
            }
            __right_0 = nullCheck_0.intVal;
            if (__castElement.intVal2 != __right_0)
            {
                __castElement.intVal2 = __right_0;
            }
        section_0_2:
            __castElement.OnUpdate();
        retn:
            return;
        };


    }

}