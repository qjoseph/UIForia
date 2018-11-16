using System.Diagnostics;

namespace UIForia.Rendering {

    public static class IntUtil {

        public const int UnsetValue = int.MaxValue;
        
        [DebuggerStepThrough]
        public static bool IsDefined(int value) {
            return value < int.MaxValue;
        }

    }

}