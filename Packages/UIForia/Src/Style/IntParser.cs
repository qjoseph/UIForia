using UIForia.Util;

namespace UIForia.Style {

    public class IntParser : IStylePropertyParser {

        public bool TryParse(CharStream stream, PropertyId propertyId, out StyleProperty2 property) {
            if (stream.TryParseInt(out int value)) {
                property = new StyleProperty2(propertyId, value);
                return true;
            }

            property = default;
            return false;
        }

    }

}