using UIForia.Rendering;

namespace UIForia.Animation {

    public struct AnimationOptions {      

        public float loopTime;
        public int iterations;
        public float delay;
        public float duration;
        public float forwardStartDelay;
        public float reverseStartDelay;
        public AnimationDirection direction;
        public AnimationLoopType loopType;
        public EasingFunction timingFunction;

        public AnimationOptions(AnimationOptions copy) {
            this.duration = copy.duration;
            this.loopTime = copy.loopTime;
            this.iterations = copy.iterations;
            this.direction = copy.direction;
            this.delay = copy.delay;
            this.forwardStartDelay = copy.forwardStartDelay;
            this.reverseStartDelay = copy.reverseStartDelay;
            this.timingFunction = copy.timingFunction;
            this.loopType = loopType = AnimationLoopType.PingPong;
        }
        
        public AnimationOptions(float duration, float loopTime = -1) {
            this.duration = duration;
            this.loopTime = loopTime;
            this.iterations = 1;
            this.direction = AnimationDirection.Forward;
            this.delay = 0f;
            this.forwardStartDelay = 0f;
            this.reverseStartDelay = 0f;
            this.timingFunction = EasingFunction.Linear;
            this.loopType = loopType = AnimationLoopType.PingPong;
        }

        public AnimationOptions(float duration, EasingFunction easing) {
            this.duration = duration;
            this.iterations = 1;
            this.loopTime = 0f;
            this.delay = 0f;
            this.forwardStartDelay = 0f;
            this.reverseStartDelay = 0f;
            this.timingFunction = easing;
            this.direction = AnimationDirection.Forward;
            this.loopType = loopType = AnimationLoopType.PingPong;
        }

        public bool Equals(AnimationOptions other) {
            return this == other;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            return obj is AnimationOptions && Equals((AnimationOptions) obj);
        }

        public static bool operator ==(AnimationOptions a, AnimationOptions b) {
            return a.loopTime == b.loopTime
                   && a.iterations == b.iterations
                   && a.delay == b.delay
                   && a.direction == b.direction
                   && a.forwardStartDelay == b.forwardStartDelay
                   && a.reverseStartDelay == b.reverseStartDelay
                   && a.timingFunction == b.timingFunction;
        }

        public static bool operator !=(AnimationOptions a, AnimationOptions b) {
            return !(a == b);
        }
        
        public override int GetHashCode() {
            unchecked {
                int hashCode = loopTime.GetHashCode();
                hashCode = (hashCode * 397) ^ iterations;
                hashCode = (hashCode * 397) ^ delay.GetHashCode();
                hashCode = (hashCode * 397) ^ duration.GetHashCode();
                hashCode = (hashCode * 397) ^ forwardStartDelay.GetHashCode();
                hashCode = (hashCode * 397) ^ reverseStartDelay.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) direction;
                hashCode = (hashCode * 397) ^ (int) timingFunction;
                return hashCode;
            }
        }

    }

}