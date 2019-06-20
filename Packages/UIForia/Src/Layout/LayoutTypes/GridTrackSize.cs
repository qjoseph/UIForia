using System.Diagnostics;

namespace UIForia.Layout.LayoutTypes {

    // repeat, grow, shrink
    // auto-fit, auto fill
    // value,
    // unit
    // pattern

    // GridCol = value | fn(values)

//    [StructLayout(LayoutKind.Explicit)]
//    public struct GridTrackSize2 {
//
//        [FieldOffset(0)] public GridTrackSizeType type;
//        [FieldOffset(4)] public float value;
//        [FieldOffset(8)] public GridTemplateUnit unit;
//        [FieldOffset(8)] public GridTrackSize2[] pattern;
//
//    }

    public enum GridTrackSizeType {

        Value,
        Repeat,
        Grow,
        Shrink,
        RepeatFit,
        RepeatFill

    }

    [DebuggerDisplay("(minValue = {nameof(minValue)}, minUnit = {nameof(minUnit)}, maxValue = {nameof(maxValue)}, maxUnit = {nameof(maxUnit)})")]
    public struct GridTrackSize {

        public readonly float minValue;
        public readonly float maxValue;

        public float value;
        public GridTemplateUnit unit;
        public GridTrackSizeType type;
        public GridTrackSize[] pattern;
        
        public readonly GridTemplateUnit minUnit;
        public readonly GridTemplateUnit maxUnit;

        public GridTrackSize(float value, GridTemplateUnit unit = GridTemplateUnit.Pixel) {
            this.minUnit = unit;
            this.minValue = value;
            this.maxUnit = unit;
            this.maxValue = value;
            this.value = value;
            this.unit = unit;
            this.type = GridTrackSizeType.Value;
            this.pattern = null;
        }

        public static GridTrackSize Unset => new GridTrackSize(0, GridTemplateUnit.Unset);
        public static GridTrackSize MaxContent => new GridTrackSize(1f, GridTemplateUnit.MaxContent);
        public static GridTrackSize MinContent => new GridTrackSize(1f, GridTemplateUnit.MinContent);
        public static GridTrackSize FractionalRemaining => new GridTrackSize(1f, GridTemplateUnit.FractionalRemaining);

        public static bool operator ==(GridTrackSize a, GridTrackSize b) {
            return a.minValue == b.minValue
                   && a.minUnit == b.minUnit
                   && a.maxValue == b.maxValue
                   && a.maxUnit == b.maxUnit;
        }

        public static bool operator !=(GridTrackSize a, GridTrackSize b) {
            return !(a == b);
        }

        public static implicit operator GridTrackSize(float value) {
            return new GridTrackSize(value, GridTemplateUnit.Pixel);
        }

        public static implicit operator GridTrackSize(int value) {
            return new GridTrackSize(value, GridTemplateUnit.Pixel);
        }

        public bool Equals(GridTrackSize other) {
            return minValue.Equals(other.minValue) && maxValue.Equals(other.maxValue) && minUnit == other.minUnit && maxUnit == other.maxUnit;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            return obj is GridTrackSize && Equals((GridTrackSize) obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = minValue.GetHashCode();
                hashCode = (hashCode * 397) ^ maxValue.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) minUnit;
                hashCode = (hashCode * 397) ^ (int) maxUnit;
                return hashCode;
            }
        }

    }

}