using UIForia.Util;
using UnityEngine;

namespace UIForia.Style {

    public class ColorParser : IStylePropertyParser {

        public bool TryParse(CharStream stream, PropertyId propertyId, out StyleProperty2 property) {
            if (stream.TryParseColorProperty(out Color32 color)) {
                property = StyleProperty2.FromValue(propertyId, color);
                return true;
            }

            property = default;
            return false;
        }

    }

}